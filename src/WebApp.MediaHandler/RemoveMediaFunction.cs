using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using WebApp.Messaging;

using Newtonsoft.Json;

namespace WebApp.MediaHandler
{
    public static class RemoveMediaFunction
    {
        [FunctionName("RemoveMedia")]
        public static void Run([QueueTrigger("eventqueue", Connection = "AzureWebJobsStorage")]WebAppEvent message, TraceWriter log)
        {
            log.Info($"ObjectIds: {string.Join(", ", message.ObjectIds)}");
        }
    }
}
