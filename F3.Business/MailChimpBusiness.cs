using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F3.Business.Fng;
using MailChimp;
using MailChimp.Helper;
using MailChimp.Lists;

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
                Workout = contact.Workout,
                EH = contact.EH,
                Twitter = contact.Twitter,
                Groupings = new List<Grouping>
                {
                    new Grouping
                    {
                        Id = 4293,
                        GroupNames = new List<string>
                        {
                            "Newsletter"
                        }
                    }
                }
            };
            var result = mc.Subscribe(ConfigurationManager.AppSettings.Get("F3List"), email, name, doubleOptIn: false,
                sendWelcome: true);
            return true;
        }

        public async Task<IEnumerable<F3.ViewModels.Contact>> Latest()
        {
            var mc = new MailChimpManager(ConfigurationManager.AppSettings.Get("MailChimpApiKey"));
            var x = mc.GetAllMembersForList(ConfigurationManager.AppSettings.Get("F3List"), limit: 10, sort_field: "optin_time", sort_dir: "DESC");

            var contacts = x.Data.Select(FillDetails);
            
            return contacts;
        }

        private F3.ViewModels.Contact FillDetails(MemberInfo arg)
        {
            return new F3.ViewModels.Contact
            {
                F3Name = (string)arg.MemberMergeInfo["F3NAME"]
            };
        }
    }
}
