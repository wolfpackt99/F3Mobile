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
using Google.Apis.Calendar.v3;
using Google.Apis.Util.Store;

namespace F3.Infrastructure.GoogleAuth
{
    public class ServiceAccount
    {

        public static string[] Scopes = new[]
        {
            CalendarService.Scope.Calendar,
            CalendarService.Scope.CalendarReadonly
        };

        private static readonly Lazy<ServiceAccount> LazyAccount = new Lazy<ServiceAccount>(() => new ServiceAccount());

        public static ServiceAccount Instance { get { return LazyAccount.Value; } }

        private ServiceAccount()
        {
            string serviceAccountEmail = ConfigurationManager.AppSettings.Get("ServiceAccountEmail");
            var certificate = LoadCertificate(ConfigurationManager.AppSettings.Get("CertificateName"));

            var credential = new ServiceAccountCredential(
               new ServiceAccountCredential.Initializer(serviceAccountEmail)
               {
                   //User = ConfigurationManager.AppSettings.Get("UserToImpersonate"),
                   Scopes = Scopes
               }.FromCertificate(certificate));

            Credential = credential;
        }

        public ServiceAccountCredential Credential { get; set; }

        private X509Certificate2 LoadCertificate(string certificateName)
        {
            var certificate = new X509Certificate2(Properties.Resources.F3Test_c745d0ff8c8e, "notasecret", X509KeyStorageFlags.Exportable);
            return certificate;
        }
    }
}
