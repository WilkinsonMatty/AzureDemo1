using AzureDemo1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System.IO;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AzureDemo1.Controllers
{
    public class ImageUploadController : Controller
    {
        // GET: ImageUpload
        public ActionResult Index()
        {
            string m = TempData.Keys.Contains("flashMessage") ? TempData["flashMessage"].ToString() : "";
            return View(new UploadViewModel() { message = m });
        }

        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase uploadFile)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("matty000storage_AzureStorageConnectionString"));
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("incoming-images");

            string blobName = Guid.NewGuid().ToString("D");

            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);
            cloudBlockBlob.UploadFromStream(uploadFile.InputStream);

            CloudTableClient cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            CloudTable cloudTable = cloudTableClient.GetTableReference("imageUploadTable");

            ImageEntity imageEntity = new ImageEntity() {
                OriginalImageBlobName = blobName,
                OriginalFileName = System.IO.Path.GetFileName(uploadFile.FileName)
            };

            cloudTable.Execute(TableOperation.Insert(imageEntity));

            CloudQueueClient cloudQueueclient = cloudStorageAccount.CreateCloudQueueClient();
            CloudQueue cloudQueue = cloudQueueclient.GetQueueReference("image-file-queue");

            cloudQueue.AddMessage(new CloudQueueMessage(blobName));


            //return View(new UploadViewModel() { message = "would upload " + uploadFile.FileName });
            TempData["flashMessage"] = "successfully uploaded " + uploadFile.FileName;
            return RedirectToAction("Index");

        }

        [HttpGet]
        public ActionResult ListImages()
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("matty000storage_AzureStorageConnectionString"));
            CloudTableClient cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            CloudTable cloudTable = cloudTableClient.GetTableReference("imageUploadTable");
            TableQuery<ImageEntity> tableQuery = new TableQuery<ImageEntity>().Where(TableQuery.GenerateFilterCondition("WatermarkedImageBlobName", QueryComparisons.NotEqual, null));
            var results = cloudTable.ExecuteQuery(tableQuery).ToList<ImageEntity>();
                


            return View(results);
        }

        [HttpGet]
        public ActionResult GetImage(string targetBlob)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("matty000storage_AzureStorageConnectionString"));
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("incoming-images");
            CloudBlob cloudBlob =   cloudBlobContainer.GetBlobReference(targetBlob);
            MemoryStream memoryStream = new MemoryStream();
            cloudBlob.DownloadToStream(memoryStream);

            Response.AddHeader($"Content-Disposition", "inline; filename={targetBlob}.jpg" ); //Set it as inline instead of attached.

            return File(memoryStream.ToArray(),"image/jpeg");
        }
    }
}