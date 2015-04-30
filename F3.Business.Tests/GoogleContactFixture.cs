using System;
using System.Threading.Tasks;
using F3.Infrastructure;
using F3.Infrastructure.GoogleAuth;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F3.Business.Tests
{
    [TestClass]
    public class GoogleContactFixture
    {
        //[TestMethod]
        public async Task GetContactsTest()
        {


            var x = new GoogleContactBusiness
            {
                Token = new ServiceAccountToken()
            };
            var list = await x.GetContacts();
            list.Should().NotBeNull("Should be able to get list of contacts.");
        }
    }
}
