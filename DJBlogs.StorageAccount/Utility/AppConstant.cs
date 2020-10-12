using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DJBlogs.StorageAccount.Utility
{
    public class AppConstant
    {
        public static string Accesskey = Environment.GetEnvironmentVariable("Accesskey");
        public static string BlobContainerURL = "https://djblogs.blob.core.windows.net/images";
    }
}
