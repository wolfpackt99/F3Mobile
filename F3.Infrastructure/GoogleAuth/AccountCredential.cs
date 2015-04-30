using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Google.Apis;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;

namespace F3.Infrastructure.GoogleAuth
{
    public class ServiceAccountToken : IAccessToken
    {
        public string AccessToken
        {
            get
            {
                var b = ServiceAccount.Instance.Credential.RequestAccessTokenAsync(new CancellationToken());
                var x = b.Result;
                return ServiceAccount.Instance.Credential.Token.AccessToken;
            }
        }
    }

    public class ServiceAccount
    {

        public static string[] Scopes = new[] { "https://www.google.com/m8/feeds" };

        private static readonly Lazy<ServiceAccount> LazyAccount = new Lazy<ServiceAccount>(() => new ServiceAccount());
        public static ServiceAccount Instance { get { return LazyAccount.Value; } }
        private ServiceAccount()
        {
            string certificateFile = string.Format("{0}\\{1}",Environment.CurrentDirectory, ConfigurationManager.AppSettings.Get("CertificateName"));
            string serviceAccountEmail = ConfigurationManager.AppSettings.Get("ClientId");
            var certificate = new X509Certificate2(certificateFile, "notasecret", X509KeyStorageFlags.Exportable);
            var credential = new ServiceAccountCredential(
               new ServiceAccountCredential.Initializer(serviceAccountEmail)
               {
                   User = ConfigurationManager.AppSettings.Get("UserToImpersonate"),
                   Scopes = Scopes
               }.FromCertificate(certificate));

            Credential = credential;
        }

        public ServiceAccountCredential Credential { get; set; }
    }
}
