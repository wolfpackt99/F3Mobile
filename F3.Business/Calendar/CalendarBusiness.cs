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
using FireSharp;
using FireSharp.Config;
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

        public async Task<bool> Publish()
        {
            var sites = (await GetCalendarList()).Items;
            var thisweek = new List<EventViewModel>();
            var tasks = sites.Select(async s =>
            {
                var events = await GetEvents(s.Id, false);
                MetaDataViewModel meta = null;
                try
                {
                    meta = JsonConvert.DeserializeObject<MetaDataViewModel>(s.Description);
                }
                catch (Exception exp)
                {

                }

                var list = new List<EventViewModel>();
                var cal = new CalenderViewModel
                {
                    Id = s.Id,
                    Name = s.Summary,
                    Description = s.Description,
                    MetaData = meta,
                    Location = s.Location,
                    Type = meta.type,
                    TimeZone = s.TimeZone,
                    Items = events.Items.Select(ev =>
                    {
                        var evModel = new EventViewModel
                        {
                            CalendarId = s.Id,
                            CalendarName = s.Summary,
                            Start = ev.Start.DateTime ?? Convert.ToDateTime(ev.Start.Date),
                            Title = ev.Summary,
                            Description = ev.Description,
                            Location = s.Location,
                            Time = meta != null ? meta.Time : string.Empty,
                            Region = meta.Region,
                            Type = meta != null ? meta.type ?? string.Empty : string.Empty,
                            TimeZone = s.TimeZone
                        };
                        try
                        {
                            var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(ev.Description);
                            evModel.Preblast = json.ContainsKey("preblast") ? json["preblast"] : null;
                            evModel.Tag = json.ContainsKey("tag") ? json["tag"] : null;
                            evModel.IsCustomDateTime = json.ContainsKey("custom");
                            if (evModel.IsCustomDateTime)
                            {
                                evModel.StartTime = ev.Start.DateTime;
                                evModel.EndTime = ev.End.DateTime;
                                if (!ev.Start.DateTime.HasValue && !ev.End.DateTime.HasValue)
                                {
                                    evModel.IsAllDay = true;
                                }
                                    
                                evModel.Location = ev.Location ?? s.Location;
                                
                            }
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
            var secret = ConfigurationManager.AppSettings.Get("FirebaseUserToken");
            var fb = new FirebaseSharp.Portable.Firebase(rootUri, secret);

            await fb.DeleteAsync("/events");
            await fb.DeleteAsync("/thisweek");
            var taskOfEvents = x.OrderBy(s => s.Name).Select(item => fb.PostAsync("/events", JsonConvert.SerializeObject(item)));
            var taskOfThisWeek = thisweek.EmptyIfNull().Where(s => s!= null).OrderBy(s => s.Start).Select(item => fb.PostAsync("/thisweek", JsonConvert.SerializeObject(item)));
            await Task.WhenAll(taskOfEvents);
            await Task.WhenAll(taskOfThisWeek);
            return true;
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

    }
}