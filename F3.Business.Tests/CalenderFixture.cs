using F3.Business.Calendar;
using F3.Business.Service;
using F3.Infrastructure.Extensions;
using F3.Infrastructure.GoogleAuth;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace F3.Business.Tests
{
    [TestClass]
    public class CalenderFixture
    {
        [TestMethod, Ignore]
        public void CredentialTest()
        {
            ServiceAccount.Instance.Credential.Should().NotBeNull();
        }

        [TestMethod, Ignore]
        public async Task GetEventsFixture()
        {
            var cb = new CalendarBusiness();

            var items = await cb.GetEvents("sn0n3fk13inesa7cju33fpejg4@group.calendar.google.com");
            items.Should().NotBeNull();
            items.Items.Should().HaveCount(c => c > 0);
        }

        [TestMethod, Ignore]
        public async Task GetCalendarListFixture()
        {
            var cb = new CalendarBusiness();
            var sut = await cb.GetCalendarList();
            foreach (var x in sut.Items.OrderBy(o => o.Summary).ToList()) {
                Console.WriteLine(x.Summary + ", " + x.Id);
            }
            sut.Items.Should().HaveCount(c => c > 0);
        }

        [TestMethod, Ignore]
        public async Task GetAllEventsForWeek()
        {
            var cb = new CalendarBusiness();
            var sut = await cb.GetAllEvents(false);
            sut.Should().NotBeEmpty();
            sut.First().Summary.ShouldBeEquivalentTo("Anvil");
            sut.Any(s => s.Summary == "SOBLSD").ShouldBeEquivalentTo(true);
            sut.FirstOrDefault(s => s.Summary == "SOBLSD").Items.EmptyIfNull().Should().HaveCount(1);
        }
        
        [TestMethod, Ignore]
        public async Task GetAllEventsForAll()
        {
            var cb = new CalendarBusiness();
            var sut = await cb.GetAllEvents();
            sut.Should().NotBeEmpty();
            sut.First().Summary.ShouldBeEquivalentTo("Anvil");
        }

        [TestMethod, Ignore]
        public async Task PublishNewTest()
        {
            var cb = new CalendarBusiness();
            cb.WorkoutBusiness = new SheetService();
            Maps.ModelMaps.InitMaps();
            var sut = await cb.PublishNew();
            sut.Should().BeTrue();
        }

        [TestMethod, Ignore]
        public async Task TodayIsSundayShouldHaveEventsForNextWeek()
        {
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            var cb = new CalendarBusiness();
            var sut = await cb.GetAllEvents(false);
            sut.Count().Should().BeGreaterThan(1);

        }
    }
}
