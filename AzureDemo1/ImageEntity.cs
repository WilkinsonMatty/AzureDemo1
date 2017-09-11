using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureDemo1
{
    public class ImageEntity : TableEntity
    {
        public string OriginalImageBlobName {
            get {
                return this.RowKey;
            }
            set
            {
                this.RowKey = value;
            }
        }
        public string WatermarkedImageBlobName { get; set; }
        public string OriginalFileName { get; set; }

        public ImageEntity() {
            this.PartitionKey = "1";
        }
    }
}