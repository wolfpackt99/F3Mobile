using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F3.Business.Calendar;
using F3.Infrastructure.GoogleAuth;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F3.Business.Tests
{
    //[TestClass]
    public class CalenderFixture
    {
        [TestMethod]
        public void CredentialTest()
        {
            ServiceAccount.Instance.Credential.Should().NotBeNull();
        }

        [TestMethod]
        public async Task GetEventsFixture()
        {
            var cb = new CalendarBusiness();

            var items = await cb.GetEvents("sn0n3fk13inesa7cju33fpejg4@group.calendar.google.com");
            items.Should().NotBeNull();
            items.Items.Should().HaveCount(c => c > 0);
        }

        [TestMethod]
        public async Task GetCaledarListFixture()
        {
            var cb = new CalendarBusiness();
            var sut = await cb.GetCalendarList();
            sut.Items.Should().HaveCount(c => c > 0);
        }
    }
}
