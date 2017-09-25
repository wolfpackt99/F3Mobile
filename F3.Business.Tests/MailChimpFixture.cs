using System;
using System.Threading.Tasks;
using F3.ViewModels;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using F3.Infrastructure.Extensions;

namespace F3.Business.Tests
{
    [TestClass]
    public class MailChimpFixture
    {
        [TestMethod]
        public async Task TestSubscribe()
        {
            var mc = new MailChimpBusiness();
            var result = await mc.Add(new Contact
            {
                FirstName = "Darrrance",
                LastName = "Trent",
                F3Name = "willy " + DateTime.Now.Ticks,
                Email = string.Format("trenton.jones+{0}@gmail.com",DateTime.Now.Ticks)
            });
            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task GetActivityTest()
        {
            
            var mx = new MailChimpBusiness();
            var result = await mx.Latest();
            
            result.Should().NotBeNullOrEmpty();
            foreach (var item in result) {
                System.Diagnostics.Debug.WriteLine(item.F3Name + " " + item.SignupDate);
            }
        }

        [TestMethod]
        public async Task IsNameTakenTest()
        {
            var mx = new MailChimpBusiness();
            var result = await mx.CheckName("wingman");
            foreach (var item in result.EmptyIfNull())
            {
                System.Diagnostics.Debug.WriteLine(item.F3Name + " ::" + item.FirstName + " " + item.LastName);
            }
        }
    }
}
