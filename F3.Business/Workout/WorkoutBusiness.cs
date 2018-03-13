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
using WorkoutModel = F3.ViewModels.WorkoutViewModel;

namespace F3.Business.Workout
{
    public class WorkoutBusiness : IWorkoutBusiness
    {
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static readonly string ApplicationName = "My Project";
        static readonly string SpreadsheetId = "1HswVlq9g5uEf_TLYh5manXMVSzuuZPU_exygPDn-bC8";
        static readonly string sheet = "Sheet1";
        static SheetsService service;

        public async Task<List<WorkoutModel>> GetMasterList()
        {
            var counter = 0;
            List<WorkoutModel> aos = new List<WorkoutModel>();
            try
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

                var range = $"{sheet}!A:R";
                SpreadsheetsResource.ValuesResource.GetRequest request =
                        service.Spreadsheets.Values.Get(SpreadsheetId, range);
                
                var response = request.Execute();
                IList<IList<object>> values = response.Values;
                
                if (values != null && values.Count > 0)
                {
                    values.RemoveAt(0);
                    foreach (var row in values)
                    {

                        // Print columns A to F, which correspond to indices 0 and 4.
                        aos.Add(new WorkoutModel {
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
                            CalendarID = row[17].ToString()

                        });
                        counter++;
                    }
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
    }
}
