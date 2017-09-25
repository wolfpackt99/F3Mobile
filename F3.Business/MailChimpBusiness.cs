using MailChimp.Net;
using MailChimp.Net.Core;
using MailChimp.Net.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using F3.Infrastructure.Extensions;

namespace F3.Business
{
    public class MailChimpBusiness : ISubscribe
    {

        public async Task<bool> Add(ViewModels.Contact contact)
        {
            var mc = new MailChimpManager(ConfigurationManager.AppSettings.Get("MailChimpApiKey"));
            var newMember = new Member
            {
                EmailAddress = contact.Email,
                StatusIfNew = Status.Subscribed,
                Status = Status.Subscribed,
                
            };
        
            newMember.MergeFields.Add("FNAME", contact.FirstName);
            newMember.MergeFields.Add("LNAME", contact.LastName);
            newMember.MergeFields.Add("F3NAME", contact.F3Name);
            newMember.MergeFields.Add("WORKOUT", contact.Workout);
            newMember.MergeFields.Add("EH", contact.EH);
            newMember.MergeFields.Add("TWITTER", contact.Twitter);
            newMember.MergeFields.Add("NOTES", " ");
            newMember.Interests.Add("eb2db7a8fd", true);
   
            
            try
            {
                var member = await mc.Members.AddOrUpdateAsync(ConfigurationManager.AppSettings["F3List"], newMember);
                
            }
            catch (MailChimpException mexp)
            {
                System.Diagnostics.Debug.WriteLine(mexp);
                throw;
            }
            return true;
        }

        public async Task<IEnumerable<F3.ViewModels.Contact>> Latest()
        {
            var date = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            var mc = new MailChimpManager(ConfigurationManager.AppSettings.Get("MailChimpApiKey"));

            var list = await mc.Members.GetAllAsync(ConfigurationManager.AppSettings["F3List"], new MemberRequest
            {
                
                SinceTimestamp = date
            }).ConfigureAwait(false);



            return list.Select(x => new ViewModels.Contact
            {
                F3Name = x.MergeFields["F3NAME"].ToString(),
                SignupDate = Convert.ToDateTime(x.TimestampOpt)
            }).OrderByDescending(s => s.SignupDate);
        }

        public async Task<IEnumerable<ViewModels.Contact>> CheckName(string f3Name)
        {
            var mc = new MailChimpManager(ConfigurationManager.AppSettings.Get("MailChimpApiKey"));

            var results = await mc.Members.SearchAsync(new MemberSearchRequest
            {
                ListId = ConfigurationManager.AppSettings.Get("F3List"),
                Query = f3Name
            });

            return results.FullSearch.Members.EmptyIfNull().Select(a => new ViewModels.Contact {
                F3Name = a.MergeFields["F3NAME"].ToString(),
                FirstName = a.MergeFields["FNAME"].ToString(),
                LastName = a.MergeFields["LNAME"].ToString()
            });
        }
    }
}
