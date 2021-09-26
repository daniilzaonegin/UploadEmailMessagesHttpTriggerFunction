using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using UploadEmailMessagesHttpTriggerFunction.Model;
using System.Reflection;

namespace UploadEmailMessagesHttpTriggerFunction
{
    public static class UploadEmailMessagesHttpTriggerFunction
    {
        [FunctionName("UploadEmailMessagesHttpTriggerFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation($"C# HTTP trigger function executed at {DateTime.Now}");

            using StreamReader bodyStream = new StreamReader(req.Body);
            string bodyStr = await bodyStream.ReadToEndAsync();
            EmailInfo email = JsonConvert.DeserializeObject<EmailInfo>(bodyStr);

            await CreateQueueIfNotExists(log, context);

            CloudStorageAccount storageAccount = GetCloudStorageAccount(context);

            CloudQueueClient cloudQueueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue cloudQueue = cloudQueueClient.GetQueueReference("email-queue");
            string message = JsonConvert.SerializeObject(email);
            CloudQueueMessage cloudQueueMessage = new CloudQueueMessage(message, false);

            await cloudQueue.AddMessageAsync(cloudQueueMessage);

            return new OkObjectResult("Email is send to queue");
        }

        private static async Task CreateQueueIfNotExists(ILogger logger, ExecutionContext executionContext)
        {
            CloudStorageAccount storageAccount = GetCloudStorageAccount(executionContext);
            CloudQueueClient cloudQueueClient = storageAccount.CreateCloudQueueClient();
            string[] queues = new[] { "email-queue" };
            foreach (string item in queues)
            {
                CloudQueue cloudQueue = cloudQueueClient.GetQueueReference(item);
                await cloudQueue.CreateIfNotExistsAsync();
            }            
        }

        private static CloudStorageAccount GetCloudStorageAccount(ExecutionContext executionContext)
        {
            IConfigurationRoot config = 
                new ConfigurationBuilder().SetBasePath(executionContext.FunctionAppDirectory)
                        .AddJsonFile("local.settings.json", true, true)
                        .AddEnvironmentVariables()
                        .Build();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(config["CloudStorageAccount"]);
            
            return storageAccount;
        }
    }
}
