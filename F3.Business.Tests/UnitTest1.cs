using System;
using System.Threading.Tasks;
using F3.Business.Leaderboard;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F3.Business.Tests
{
    [TestClass]
    public class StravaFixture
    {
        [TestMethod]
        public async Task GetClubTest()
        {
            var sb = new StravaBusiness();
            var club = await sb.GetData();
            club.Should().NotBeNull();
            
        }
    }
}
