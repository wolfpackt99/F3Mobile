using System;
using System.Threading.Tasks;
using F3.Business.Leaderboard;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Strava.Athletes;

namespace F3.Business.Tests
{
    [TestClass]
    public class StravaFixture
    {
        [TestMethod, Ignore]
        public async Task GetClubTest()
        {
            var sb = new StravaBusiness();
            var club = await sb.GetData();
            club.Should().NotBeNull();
            
        }

        [TestMethod, Ignore]
        public async Task GetActivities()
        {
            var sb = new StravaBusiness();
            var act = await sb.GetActivities();
            act.Should().NotBeNull();
        }

        [TestMethod, Ignore]
        public async Task TestSetAuth()
        {
            var sb = new StravaBusiness();
            await sb.SetAuthToken(new StravaAuth
            {
                AccessToken = "",
                Athlete = new AthleteSummary()
                {
                    Id = 6508764,
                    FirstName = "Test"
                }
            });
        }
    }
}
