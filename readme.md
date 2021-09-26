# Azure Function for sending emails to SendGrid
Configuration:
```js
{
    "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "CloudStorageAccount": "Enter your azure cloud storage account",
    "SendgridAPIKey": "Enter your SendGridAPIKey"
  }
}
```