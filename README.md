# AzureDemo1
This adds a watermark to an image uploaded by the user. The back end uses Azure blobs, an Azure queue, an Azure table, and an Azure function.

~~See it in action here http://image-water-mark-demo.azurewebsites.net/~~
The site is paused to keep costs down. I turn it on and off as needed.

However, you can see a video of it in action here:
[https://youtu.be/SLSaL8lquVg](https://youtu.be/SLSaL8lquVg)


## What's Used

The front end is an ASP.NET MVC app that has been published to Azure. The back end is a series of Azure components as described below.

Posting a file writes the file data itself to a new blob. Metadata containing the blob location and original file name is written to an Azure Table. Also, a message is placed into an Azure Queue, triggering an Azure function to run that adds the watermark, writes another blob with the modified image, and adds the location of this new blob to the same metadata document.

On the front end, the contents of the metadata table are used to display a list of the watermarked images and to ultimately serve them up to the browser.
