using Microsoft.WindowsAzure.Storage;
    

using System;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ImageWatermark
{
    public static class Function1
    {

        //public static void Run([QueueTrigger("image-upload", Connection = "matty000storage_STORAGE")]string myQueueItem, TraceWriter log)

        //[FunctionName("Function1")]
        public static void Run(string myQueueItem, ILogger log)
        {
            log.Info($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
