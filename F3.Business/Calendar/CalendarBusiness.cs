using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using F3.Business.Calendar;
using F3.Infrastructure;
using F3.Infrastructure.GoogleAuth;
using FluentDateTime;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Ninject;

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
            request.TimeMin = DateTime.Now.Previous(DayOfWeek.Sunday);
            if (!all)
            {
                request.TimeMax = DateTime.Now.Next(DayOfWeek.Saturday);
            }
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            request.SingleEvents = true;
            request.MaxResults = 15;
            request.ShowDeleted = false;


            return ProcessResults(request);
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
                allData.Items = allRows;
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
        
        
    }
}