using F3.Infrastructure.Extensions;
using F3.ViewModels.Calendar;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F3.Business.Service
{
    public class FirebaseService
    {
        public async Task Publish(List<CalenderViewModel> x, IEnumerable<EventViewModel> thisweek)
        {
            var rootUri = ConfigurationManager.AppSettings.Get("FirebaseUri");
            var secret = ConfigurationManager.AppSettings.Get("FirebaseUserToken");
            var fb = new FirebaseSharp.Portable.Firebase(rootUri, secret);

            await fb.DeleteAsync("/events");
            await fb.DeleteAsync("/thisweek");
            var taskOfEvents = x.OrderBy(s => s.Name).Select(item => fb.PostAsync("/events", JsonConvert.SerializeObject(item)));
            var taskOfThisWeek = thisweek.EmptyIfNull().Where(s => s != null).OrderBy(s => s.Start).Select(item => fb.PostAsync("/thisweek", JsonConvert.SerializeObject(item)));
            await Task.WhenAll(taskOfEvents);
            await Task.WhenAll(taskOfThisWeek);
        }
    }
}
