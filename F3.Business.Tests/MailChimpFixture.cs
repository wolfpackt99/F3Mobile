using System;
using System.Threading.Tasks;
using F3.ViewModels;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
                FirstName = "Terrence",
                LastName = "Trent",
                Email = string.Format("trenton.jones+{0}@gmail.com",DateTime.Now.Ticks)
            });
            result.Should().BeTrue();
        }
    }
}
