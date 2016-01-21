using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FireSharp.Config;

namespace F3.Business.Storage
{
    public class StorageClient
    {
        public static readonly Lazy<FireSharp.FirebaseClient> Instance = new Lazy<FireSharp.FirebaseClient>(GetStorageClient); 

        private static FireSharp.FirebaseClient GetStorageClient()
        {

            var rootUri = ConfigurationManager.AppSettings.Get("FirebaseUri");
            var secret = ConfigurationManager.AppSettings.Get("FirebaseUserToken");
            var fb = new FireSharp.FirebaseClient(new FirebaseConfig()
            {
                AuthSecret = secret,
                BasePath = rootUri
            });

            return fb;

            
        }
    }
}
