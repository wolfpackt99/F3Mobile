using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using F3.Business.Calendar;
using F3.Infrastructure;
using F3.Infrastructure.Cache;
using F3.Infrastructure.GoogleAuth;
using F3.ViewModels.Calendar;
using FluentDateTime;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
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


            var result = request.Execute();
            return result;


            //return ProcessResults(request);
        }

        public async Task<IEnumerable<Events>> GetAllEvents(bool all = true, bool bust = false)
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
            var to = new MailAddressCollection{new MailAddress("")};
            var smtpClient = new SmtpClient();
            var mailMsg = new MailMessage("", "", "Sub", html);
            mailMsg.IsBodyHtml = true;
            smtpClient.Send(mailMsg);
            return true;
        }

         
    }
}