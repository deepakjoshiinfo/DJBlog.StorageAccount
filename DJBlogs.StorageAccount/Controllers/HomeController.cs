using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DJBlogs.StorageAccount.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using DJBlogs.StorageAccount.Utility;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Auth;

namespace DJBlogs.StorageAccount.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Get a reference to a container that's available for anonymous access.
            CloudBlobContainer container = new CloudBlobContainer(new Uri(AppConstant.BlobContainerURL));
            //New for core
            OperationContext context = new OperationContext();
            BlobRequestOptions options = new BlobRequestOptions();
            // List blobs in the container.
            var list = container.ListBlobsSegmentedAsync(null, true, BlobListingDetails.All, null, null, options, context);
            var _list = new List<FileModel>();
            foreach (IListBlobItem blobItem in list.Result.Results)
            {
                _list.Add(new FileModel()
                {
                    URL = blobItem.Uri.ToString(),
                    Name = Path.GetFileName(blobItem.Uri.ToString())
                });
                //Console.WriteLine(blobItem.Uri);
            }
            return View(_list);
        }

        public IActionResult UploadFile()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return Content("files not selected");
            foreach (var file in files)
            {
                var stream = file.OpenReadStream();
                // Create storagecredentials object by reading the values from the configuration (appsettings.json)
                //deeppawncloudcustomer - Access keys
                StorageCredentials storageCredentials = new StorageCredentials("djblogs", AppConstant.Accesskey);

                // Create cloudstorage account by passing the storagecredentials
                CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);

                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                // Get reference to the blob container by passing the name by reading the value from the configuration (appsettings.json)
                CloudBlobContainer container = blobClient.GetContainerReference("images");

                // Get the reference to the block blob from the container
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(file.FileName);

                // Upload the file
                await blockBlob.UploadFromStreamAsync(stream);
            }
            return RedirectToAction("Index");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
