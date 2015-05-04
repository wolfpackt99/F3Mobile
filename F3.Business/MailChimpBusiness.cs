using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F3.Business.Fng;
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
            var name = new NameVar
            {
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                F3Name = contact.F3Name,
                Workout = contact.Workout
            };
            var result = mc.Subscribe(ConfigurationManager.AppSettings.Get("F3List"), email, name, doubleOptIn: false,
                sendWelcome: true);
            return true;
        }
    }
}
