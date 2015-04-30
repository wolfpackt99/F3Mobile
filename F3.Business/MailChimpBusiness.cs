using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailChimp;
using MailChimp.Helper;

namespace F3.Business
{
    public class MailChimpBusiness : ISubscribe
    {

        public async Task<bool> Add(ViewModels.Contact contact)
        {
            var mc = new MailChimpManager(ConfigurationManager.AppSettings.Get("MailChimpApiKey"));
            var email = new EmailParameter
            {
                Email = contact.Email
            };
            var result = mc.Subscribe(ConfigurationManager.AppSettings.Get("F3List"), email, doubleOptIn: false,
                sendWelcome: true);
            return true;
        }
    }
}
