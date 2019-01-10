using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using F3.Business.Storage;
using F3.Infrastructure.Extensions;
using F3.ViewModels;
using log4net.Plugin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Strava.Activities;
using Strava.Api;
using Strava.Athletes;
using Strava.Authentication;
using Strava.Clients;
using Strava.Common;
using Strava.Statistics;
using WebRequest = Strava.Http.WebRequest;

namespace F3.Business.Leaderboard
{
    public interface IStravaBusiness
    {
        Task<IEnumerable<ActivitySummary>> GetActivities(long? athleteId = null);
        Task<IEnumerable<User>> GetData();

        /// <summary>
        /// temporary fix for user issue
        /// </summary>
        /// <param name="clubId"></param>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <returns></returns>
        Task<List<AthleteSummary>> GetClubMembersAsync(string clubId, int page, int perPage);

        Task<string> GetAuthToken(string code);
        Task SetAuthToken(StravaAuth authAthlete);
    }

    public class StravaBusiness : IStravaBusiness
    {
        private static readonly StaticAuthentication StaticAuthentication = new StaticAuthentication(ConfigurationManager.AppSettings.Get("strava"));

        public async Task<IEnumerable<ActivitySummary>> GetActivities(long? athleteId = null)
        {
            ClubClient client = new ClubClient(StaticAuthentication);
            List<ActivitySummary> activities = new List<ActivitySummary>();
            var moreResults = true;
            var page = 1;
            while (moreResults)
            {
                var act = await client.GetLatestClubActivitiesAsync(ConfigurationManager.AppSettings.Get("stravaClubId"), page, 200);
                if (!act.Any())
                {
                    moreResults = false;
                }
                activities.AddRange(act);
                page++;
            }
            if (activities.Any() && athleteId.HasValue)
            {
                return activities.Where(a => a.Athlete.Id == athleteId.Value);
            }
            return activities;
        }

        public async Task<IEnumerable<User>> GetData()
        {
            //var moreResults = true;
            //var page = 1;
            //var client = new ClubClient(StaticAuthentication);

            //var athletes = new List<AthleteSummary>();
            //while (moreResults)
            //{
            //    var clubId = ConfigurationManager.AppSettings["stravaClubId"];
            //    var pageResults = await GetClubMembersAsync(clubId, page, 200);
            //    if (!pageResults.Any())
            //    {
            //        moreResults = false;
            //    }
            //    athletes.AddRange(pageResults);
            //    page++;
            //}
            var authedAthletes = await GetAuthedAthletes();

            //var auth_only = athletes.Where(s => authedAthletes.Any(a => a.Athlete.Id == s.Id));

            var users = await authedAthletes.ForkJoin(async s => await DoTheNeedful(s.Athlete, s.AccessToken));
            return users;

        }

        private async Task<User> DoTheNeedful(AthleteSummary arg, string token)
        {
            var user = new User();
            try
            {
                var activities = new List<ActivitySummary>();
                var authToken = token;
                if (authToken != null)
                {
                    var userActivities = await GetActivityByUser(arg.Id, authToken);
                    activities.AddRange(userActivities);
                }

                
                user = new User
                {
                    ProfilePic = arg.ProfileMedium,
                    StravaId = arg.Id,
                    FirstName = arg.FirstName,
                    LastName = arg.LastName,
                    ActivityCount = activities.Count
                };
                user.TotalMiles = activities.Sum(e => Convert.ToDecimal(e.Distance * 0.000621371));
                var stats = GetStatsForUser(activities.ToList()).ToList();
                user.Stats = stats;
                user.Running = activities.Where(t => t.RawType.ToLower() == "run").Sum(e => Convert.ToDecimal(e.Distance * 0.000621371));
                
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.WriteLine(exp.Message);
                //throw;
            }
            return user;
        }

        

        private IEnumerable<Stat> GetStatsForUser(List<ActivitySummary> activities)
        {
            var actByType = activities.EmptyIfNull()
                        .GroupBy(s => s.RawType)
                        .Select(s => new Stat
                        {
                            Activity = s.FirstOrDefault().RawType,
                            Mileage = s.Sum(m => Convert.ToDecimal(m.Distance * 0.000621371))
                        });
            return actByType;
        }

        private async Task<IEnumerable<ActivitySummary>> GetActivityByUser(long athleteId, string authToken)
        {
            var actClient = new ActivityClient(new StaticAuthentication(authToken));

            var moreResults = true;
            var page = 1;

            var activities = new List<ActivitySummary>();
            while (moreResults)
            {
                var actPage = await actClient.GetActivitiesAfterAsync(new DateTime(2019, 1, 1, 0, 0, 0, DateTimeKind.Utc), page, 200);
                if (!actPage.Any())
                {
                    moreResults = false;
                }
                activities.AddRange(actPage);
                page++;
            }

            return activities;
        }

        /// <summary>
        /// temporary fix for user issue
        /// </summary>
        /// <param name="clubId"></param>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <returns></returns>
        public async Task<List<AthleteSummary>> GetClubMembersAsync(string clubId, int page, int perPage)
        {
            try
            {
                string getUrl = string.Format("{0}/{1}/members?page={2}&per_page={3}&access_token={4}",
                Endpoints.Club,
                clubId,
                page,
                perPage,
                StaticAuthentication.AccessToken);
                string json = await WebRequest.SendGetAsync(new Uri(getUrl));
                return Unmarshaller<List<AthleteSummary>>.Unmarshal(json);
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.WriteLine(exp.Message);
                throw;
            }
        }

        public async Task<string> GetAuthToken(string code)
        {

            var clientId = ConfigurationManager.AppSettings.Get("stravaClientId");
            var clientSecret = ConfigurationManager.AppSettings.Get("stravaClientSecret");
            var httpClient = new HttpClient();

            //get the token from the server using client secret
            string url = string.Format("https://www.strava.com/oauth/token?client_id={0}&client_secret={1}&code={2}", clientId, clientSecret, code);

            var resp = await httpClient.PostAsync(new Uri(url), null);
            string str = await resp.Content.ReadAsStringAsync();

            StravaAuth athlete = Unmarshaller<StravaAuth>.Unmarshal(str);
            await SetAuthToken(athlete);
            return athlete.AccessToken;
            //await FirebaseClient.Instance.Value.PutAsync(string.Format("athletes/{0}", auth.Id), JsonConvert.SerializeObject(auth));
        }

        public async Task<IEnumerable<StravaAuth>> GetAuthedAthletes()
        {
            var storageResponse = await StorageClient.Instance.Value.GetAsync("/athletes");
            var data = storageResponse.ResultAs<Dictionary<string, StravaAuth>>();

            var athletes = data.Select(s => s.Value);
            return athletes;
        }

        public async Task SetAuthToken(StravaAuth authAthlete)
        {
            var storageResponse = await StorageClient.Instance.Value.GetAsync("/athletes");
            var data = storageResponse.ResultAs<Dictionary<string, StravaAuth>>();

            var athletes = data.Select(s => s.Value);

            var found = athletes.FirstOrDefault(a => a.Athlete != null && a.Athlete.Id == authAthlete.Athlete.Id);
            if (found != null)
            {
                string key = data.FirstOrDefault(f => f.Value == found).Key;
                var resp = await StorageClient.Instance.Value.UpdateAsync("/athletes/" + key, authAthlete);
                var x = resp.StatusCode;
            }
            else
            {
                await StorageClient.Instance.Value.PushAsync("/athletes", authAthlete);
            }
        }
    }

    public class StravaAuth
    {
        [JsonProperty("athlete")]
        public AthleteSummary Athlete { get; set; }
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

    }
}
