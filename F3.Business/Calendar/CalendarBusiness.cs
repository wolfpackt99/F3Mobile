using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using F3.Business.Calendar;
using F3.Infrastructure;
using F3.Infrastructure.Cache;
using F3.Infrastructure.Extensions;
using F3.Infrastructure.GoogleAuth;
using F3.ViewModels.Calendar;
using FluentDateTime;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Newtonsoft.Json;
using Ninject;
using Nustache.Core;

namespace F3.Business.Calendar
{
    public class CalendarBusiness : ICalendarBusiness
    {


        public async Task<Events> GetEvents(string id, bool all = true)
        {
            var service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = ServiceAccount.Instance.Credential,
                ApplicationName = "Calendar API Sample"
            });

            EventsResource.ListRequest request = service.Events.List(id);
            request.TimeMin = DateTime.Now.Previous(DayOfWeek.Sunday).BeginningOfDay();
            if (!all)
            {
                //get just this weeks
                request.TimeMax = DateTime.Now.Next(DayOfWeek.Saturday).EndOfDay();
            }
            else
            {
                request.TimeMax = DateTime.Now.NextMonth();
            }

            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            request.SingleEvents = true;

            request.MaxResults = 5;
            request.ShowDeleted = false;


            var result = await request.ExecuteAsync();
            return result;


            //return ProcessResults(request);
        }

        public async Task<IEnumerable<Events>> GetAllEvents(bool all = true)
        {

            var list = await GetCalendarList();
            var sites = list.Items;

            var tasks = sites.Select(s => GetEvents(s.Id, all));

            var enumerable = tasks as Task<Events>[] ?? tasks.ToArray();
            await Task.WhenAll(enumerable);


            return enumerable.Select(site => site.Result).OrderBy(s => s.Summary);

        }

        public async Task<CalendarList> GetCalendarList()
        {
            var service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = ServiceAccount.Instance.Credential,
                ApplicationName = "Calendar API Sample"
            });

            CalendarListResource.ListRequest request = service.CalendarList.List();

            request.MaxResults = 100;
            request.ShowDeleted = false;
            request.ShowHidden = false;


            return ProcessListResults(request);
        }

        #region Private Methods
        private static Events ProcessResults(EventsResource.ListRequest request)
        {
            try
            {
                Events result = request.Execute();
                var allRows = new List<Event>();

                //// Loop through until we arrive at an empty page
                while (result.Items != null)
                {
                    //Add the rows to the final list
                    allRows.AddRange(result.Items);

                    // We will know we are on the last page when the next page token is
                    // null.
                    // If this is the case, break.
                    if (result.NextPageToken == null)
                    {
                        break;
                    }
                    // Prepare the next page of results
                    request.PageToken = result.NextPageToken;

                    // Execute and process the next page request
                    result = request.Execute();
                }
                Events allData = result;
                allData.Items = allRows.Take(5).ToList();
                return allData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        // Just loops though getting all the rows.  
        private static CalendarList ProcessListResults(CalendarListResource.ListRequest request)
        {
            try
            {
                CalendarList result = request.Execute();
                var allRows = new List<CalendarListEntry>();

                //// Loop through until we arrive at an empty page
                while (result.Items != null)
                {
                    //Add the rows to the final list
                    allRows.AddRange(result.Items);

                    // We will know we are on the last page when the next page token is
                    // null.
                    // If this is the case, break.
                    if (result.NextPageToken == null)
                    {
                        break;
                    }
                    // Prepare the next page of results
                    request.PageToken = result.NextPageToken;

                    // Execute and process the next page request
                    result = request.Execute();
                }
                CalendarList allData = result;
                allData.Items = allRows;
                return allData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        #endregion

        public async Task<bool> RequestDate(QRequest request)
        {
            var html = Render.FileToString("qrequest", request);
            var to = new MailAddressCollection { new MailAddress("") };
            var smtpClient = new SmtpClient();
            var mailMsg = new MailMessage("", "", "Sub", html);
            mailMsg.IsBodyHtml = true;
            smtpClient.Send(mailMsg);
            return true;
        }

        public async Task<bool> Publish()
        {
            var sites = (await GetCalendarList()).Items;
            var thisweek = new List<EventViewModel>();
            var tasks = sites.Select(async s =>
            {
                var events = await GetEvents(s.Id);
                var list = new List<EventViewModel>();
                var cal = new CalenderViewModel
                {
                    Id = s.Id,
                    Name = s.Summary,
                    Description = s.Description,
                    Location = s.Location,
                    Items = events.Items.Select(ev =>
                    {
                        var evModel = new EventViewModel
                        {
                            CalendarId = s.Id,
                            CalendarName = s.Summary,
                            Start = ev.Start.DateTime ?? Convert.ToDateTime(ev.Start.Date),
                            Title = ev.Summary,
                            Description = ev.Description,
                            Location = s.Location
                        };
                        try
                        {
                            var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(ev.Description);
                            evModel.Preblast = json.ContainsKey("preblast") ? json["preblast"] : null;
                            evModel.Tag = json.ContainsKey("tag") ? json["tag"] : null;
                            
                        }
                        catch (Exception exp)
                        {
                        }

                        return evModel;
                    }).AsEnumerable()
                };
                thisweek.AddRange(isThisWeek(cal));

                return cal;
            });

            var x = await Task.WhenAll(tasks);
            var rootUri = ConfigurationManager.AppSettings.Get("FirebaseUri");
            var authToken = ConfigurationManager.AppSettings.Get("FirebaseAuthToken");
            var fb = new FirebaseSharp.Portable.Firebase(rootUri, authToken);

            await fb.DeleteAsync("/events");
            await fb.DeleteAsync("/thisweek");
            var taskOfEvents = x.OrderBy(s => s.Name).Select(item => fb.PostAsync("/events", JsonConvert.SerializeObject(item)));
            var taskOfThisWeek = thisweek.EmptyIfNull().OrderBy(s => s.Start).Select(item => fb.PostAsync("/thisweek", JsonConvert.SerializeObject(item)));
            await Task.WhenAll(taskOfEvents);
            await Task.WhenAll(taskOfThisWeek);
            return true;
        }

        public List<EventViewModel> isThisWeek(CalenderViewModel cvm)
        {
            if (cvm.Items != null)
            {
                var now = DateTime.Now;
                return cvm.Items.Where(e => 
                    e.Start.Value <= now.Next(DayOfWeek.Saturday).EndOfDay()).ToList();
            }
            return new List<EventViewModel>();
        }

    }
}