using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F3.Business.Auth;
using F3.Infrastructure.Extensions;
using F3.ViewModels;
using Strava.Activities;
using Strava.Api;
using Strava.Athletes;
using Strava.Authentication;
using Strava.Clients;
using Strava.Clubs;
using Strava.Common;
using Strava.Http;

namespace F3.Business.Leaderboard
{
    public class StravaBusiness
    {
        private static readonly StaticAuthentication StaticAuthentication = new StaticAuthentication(ConfigurationManager.AppSettings.Get("strava"));

        public async Task<IEnumerable<User>> GetData()
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

            var athletes = new List<AthleteSummary>();
            page = 1;
            moreResults = true;
            while (moreResults)
            {
                var pageResults = await GetClubMembersAsync(ConfigurationManager.AppSettings.Get("stravaClubId"), page, 200);
                if (!pageResults.Any())
                {
                    moreResults = false;
                }
                athletes.AddRange(pageResults);
                page++;
            }
            var clubMembers = await client.GetClubMembersAsync(ConfigurationManager.AppSettings.Get("stravaClubId"));

            var users = await athletes.ForkJoin(async s => await DoTheNeedful(s, activities));
            return users;

        }

        private async Task<User> DoTheNeedful(AthleteSummary arg, List<ActivitySummary> activities)
        {
            var userActivities =
                activities.Where(
                    d => d.DateTimeStart >= new DateTime(2016, 1, 1, 0, 0, 0, DateTimeKind.Utc) && d.Athlete.Id == arg.Id).ToList();

            var athleteClient = new AthleteClient(StaticAuthentication);
            var athlete = await athleteClient.GetAthleteAsync(arg.Id.ToString());
            var user = new User
            {
                ProfilePic = athlete.ProfileMedium,
                Activities = userActivities,
                FirstName = arg.FirstName,
                LastName = arg.LastName,
                ActivityCount = userActivities.Count(),
                TotalMiles = userActivities.Sum(e => Convert.ToDecimal(e.Distance * 0.000621371))
            };

            return user;
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
            string getUrl = string.Format("{0}/{1}/members?page={2}&per_page={3}&access_token={4}",
                Endpoints.Club,
                clubId,
                page,
                perPage,
                StaticAuthentication.AccessToken);
            string json = await WebRequest.SendGetAsync(new Uri(getUrl));

            return Unmarshaller<List<AthleteSummary>>.Unmarshal(json);
        }
    }


}
