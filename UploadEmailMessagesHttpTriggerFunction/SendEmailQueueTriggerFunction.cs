using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using System;
using System.Text.Json;
using UploadEmailMessagesHttpTriggerFunction.Model;

namespace UploadEmailMessagesHttpTriggerFunction
{
    public static class SendEmailQueueTriggerFunction
    {
        [FunctionName("SendEmailQueueTriggerFunction")]
        public static void Run(
            [QueueTrigger("email-queue", Connection = "CloudStorageAccount")]string myQueueItem,
            [SendGrid(ApiKey ="SendgridAPIKey")] out SendGridMessage sendGridMessage,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            try
            {
                string queueItem = myQueueItem.ToString();
                EmailInfo messageInfo = JsonSerializer.Deserialize<EmailInfo>(queueItem);
                sendGridMessage = new SendGridMessage
                {
                    From = new EmailAddress("applicationmailto@gmail.com", "Task4U")
                };
                sendGridMessage.AddTo(messageInfo.To);
                sendGridMessage.SetSubject(messageInfo.Subject);
                if (!string.IsNullOrEmpty(messageInfo.Cc))
                    sendGridMessage.AddCc(messageInfo.Cc);
                sendGridMessage.AddContent("text/html", messageInfo.Body);
            }
            catch (Exception ex)
            {
                sendGridMessage = new SendGridMessage();
                log.LogError($"Error occured while processing QueueItem {myQueueItem}" +
                    $"Exception - {ex}");
            }
        }
    }
}
