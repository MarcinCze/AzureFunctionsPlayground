using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace StorageFunctions
{
    public static class MessageQueueTrigger
    {
        [FunctionName("MessageQueueTrigger")]
        public static void Run([QueueTrigger("docs-to-process", Connection = "AzureWebJobsStorage")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
