using AutoMapper;
using F3.Business.Service;
using F3.Infrastructure.Extensions;
using F3.Infrastructure.GoogleAuth;
using F3.ViewModels.Calendar;
using FluentDateTime;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Newtonsoft.Json;
using Ninject;
using Nustache.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace F3.Business.Calendar
{
    public class CalendarBusiness : ICalendarBusiness
    {
        [Inject]
        public ISheetService WorkoutBusiness { get; set; }

        public async Task<Events> GetEvents(string id, bool all = true)
        {
            var service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = ServiceAccount.Instance.Credential,
                ApplicationName = "Calendar API Sample"
            });
            try
            {


                EventsResource.ListRequest request = service.Events.List(id);
                request.TimeMin = GetCorrectStart();
                if (!all)
                {
                    //get just this weeks
                    request.TimeMax = GetCorrectEndOfWeek();
                }
                else
                {
                    request.TimeMax = DateTime.UtcNow.NextMonth();
                }

                //request.

                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
                request.SingleEvents = true;

                request.MaxResults = 5;
                request.ShowDeleted = false;


                var result = await request.ExecuteAsync();
                return result;
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.WriteLine(id);
                System.Diagnostics.Debug.WriteLine(exp.Message);
                throw;
            }

            //return ProcessResults(request);
        }

        private DateTime GetCorrectStart()
        {
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            if (easternTime.DayOfWeek == DayOfWeek.Sunday)
            {
                return easternTime.BeginningOfDay().ToUniversalTime();
            }
            return easternTime.Previous(DayOfWeek.Sunday).BeginningOfDay().ToUniversalTime();
        }

        private DateTime GetCorrectEndOfWeek()
        {
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            if (easternTime.DayOfWeek == DayOfWeek.Sunday)
            {
                return easternTime.Next(DayOfWeek.Sunday).Next(DayOfWeek.Sunday).ToUniversalTime();
            }
            return easternTime.Next(DayOfWeek.Sunday).Next(DayOfWeek.Sunday).ToUniversalTime();
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

        public async Task<bool> Publish() {
            return await PublishNew();
        }

        public async Task<bool> PublishNew()
        {
            //get workouts from sheets
            var ws = new SheetService();
            var workouts = (await ws.GetWorkouts()).EmptyIfNull().Where(wo => wo.Show == true);
            //get social calendars from sheets
            var socialCalendarIds = await ws.GetSocialCalendars();

            var extraCalendars = Mapper.Map<List<CalenderViewModel>>((await GetCalendarList()).Items);
            //factor out the all but social calendars
            var socialCalendars = extraCalendars.Where(cal => socialCalendarIds.EmptyIfNull().Select(s => s.CalendarID).Contains(cal.Id));

            var retVal = new List<CalenderViewModel>();

            retVal.AddRange(Mapper.Map<List<CalenderViewModel>>(workouts));
            retVal.AddRange(socialCalendars);

            var x = await retVal.ForkJoin(async cal => await GetThisWeekEventViewModels(cal));
            //todo: get items.

            //publish to firebase
            var fbSvc = new FirebaseService();
            await fbSvc.Publish(retVal, retVal.SelectMany(cal => cal.Items));
            return retVal != null && retVal.Count > 0;
        }

        public List<EventViewModel> isThisWeek(CalenderViewModel cvm)
        {
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            if (cvm.Items != null)
            {
                var now = DateTime.Now;
                return cvm.Items.Where(e =>
                    e.Start.Value > GetCorrectStart().BeginningOfDay() &&
                    e.Start.Value <= GetCorrectEndOfWeek().EndOfDay()).ToList();
            }
            return new List<EventViewModel>();
        }

        public async Task<CalenderViewModel> GetThisWeekEventViewModels(CalenderViewModel calendar)
        {
            var events = await GetEvents(calendar.Id, false);
            var evModels = events.Items.EmptyIfNull().Select(ev =>
            {
                var evModel = new EventViewModel
                {
                    CalendarId = calendar.Id,
                    CalendarName = calendar.Name,
                    Start = ev.Start.DateTime ?? Convert.ToDateTime(ev.Start.Date),
                    Title = ev.Summary,
                    Description = ev.Description,
                    Location = calendar.Location,
                    TimeZone = calendar.TimeZone,
                    Time = calendar.Time,
                    Region = calendar.Region,
                    Type = calendar.Type ?? string.Empty,

                };

                //parse event description json object
                try
                {
                    var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(ev.Description);
                    evModel.Preblast = json.ContainsKey("preblast") ? json["preblast"] : null;
                    evModel.Tag = json.ContainsKey("tag") ? json["tag"] : null;
                    evModel.CustomDescription = json.ContainsKey("description") ? json["description"] : null;
                    evModel.IsCustomDateTime = json.ContainsKey("custom");
                    if (evModel.IsCustomDateTime)
                    {
                        evModel.StartTime = ev.Start.DateTime;
                        evModel.EndTime = ev.End.DateTime;
                        if (!ev.Start.DateTime.HasValue && !ev.End.DateTime.HasValue)
                        {
                            evModel.IsAllDay = true;
                        }

                        evModel.Location = ev.Location ?? calendar.Location;

                    }
                }
                catch (Exception exp)
                {
                    if (!exp.Message.Contains("Value cannot be null."))
                    {
                        System.Diagnostics.Debug.WriteLine("-----calitem------");
                        System.Diagnostics.Debug.WriteLine(calendar.Name + "::" + ev.Summary + "::" + ev.Description + "::" + ev.Start.DateTime.ToString());
                        System.Diagnostics.Debug.WriteLine(exp.Message);
                        System.Diagnostics.Debug.WriteLine("-----endcalitem-----");
                    }
                }

                return evModel;
            });
            calendar.Items = evModels;
            return calendar;
        }
    }
}