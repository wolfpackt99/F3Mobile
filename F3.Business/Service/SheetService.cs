using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Google.Apis.Calendar.v3.CalendarService;
using F3.ViewModels;

namespace F3.Business.Service
{
    public class SheetService : ISheetService
    {
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static readonly string ApplicationName = "My Project";
        static readonly string SpreadsheetId = "1HswVlq9g5uEf_TLYh5manXMVSzuuZPU_exygPDn-bC8";
        static SheetsService service;

        public async Task<List<WorkoutViewModel>> GetWorkouts()
        {
            var counter = 0;
            List<WorkoutViewModel> aos = new List<WorkoutViewModel>();
            try
            {

                var range = $"Sheet1!A:U";
                IList<IList<object>> values = GetSheetData(range);

                if (values != null && values.Count > 0)
                {
                    values.RemoveAt(0);
                    aos = values.Select(row => new WorkoutViewModel
                    {
                        Address = row[0].ToString(),
                        Latitude = Convert.ToDecimal(row[1].ToString()),
                        Longitude = Convert.ToDecimal(row[2].ToString()),
                        City = row[3].ToString(),
                        State = row[4].ToString(),
                        Zipcode = row[5].ToString(),
                        Name = row[6].ToString(),
                        Day = row[7].ToString(),
                        Time = row[8].ToString(),
                        Group = row[9].ToString(),
                        Url = row[10].ToString(),
                        Type = row[11].ToString(),
                        Q = row[12].ToString(),
                        Region = row[13].ToString(),
                        Notes = row[14].ToString(),
                        DayOfWeek = Convert.ToInt32(row[15].ToString()),
                        Show = row[16].ToString() == "1" ? true : false,
                        CalendarID = row[17].ToString(),
                        DisplayLocation = row[18].ToString(),
                        LocationHint = row.Count == 20 ? row[19].ToString() : string.Empty

                    }).ToList();
                    counter++;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No data found.");
                }
            }
            catch (Exception exp)
            {
                throw;
            }


            return aos;
        }

        public async Task<IEnumerable<SocialCalendarViewModel>> GetSocialCalendars()
        {
            IEnumerable<SocialCalendarViewModel> socialCalendars = new List<SocialCalendarViewModel>();
            try
            {
                IList<IList<object>> values = GetSheetData("SocialCalendars!A:C");

                if (values != null && values.Count > 0)
                {
                    values.RemoveAt(0);
                    socialCalendars = values.Select(row => new SocialCalendarViewModel
                    {
                        Name = row[0].ToString(),
                        CalendarID = row[1].ToString(),
                        Show = row[2].ToString() == "1" ? true : false,
                    });
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No data found.");
                }
            }
            catch (Exception exp)
            {
                throw;
            }
            return socialCalendars;
        }

        private static IList<IList<object>> GetSheetData(string range)
        {
            var service = GetGoogleService();
            SpreadsheetsResource.ValuesResource.GetRequest request =
                                    service.Spreadsheets.Values.Get(SpreadsheetId, range);

            var response = request.Execute();
            IList<IList<object>> values = response.Values;
            return values;
        }

        private static SheetsService GetGoogleService()
        {
            GoogleCredential credential;

            using (var stream = new FileStream("My Project-8b9b66adb6de.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            return service;
        }
    }
}
