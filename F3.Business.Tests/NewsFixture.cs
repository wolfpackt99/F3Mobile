using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using F3.Business.News;
using FluentAssertions;
using FluentAssertions.Formatting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F3.Business.Tests
{
    //[TestClass]
    public class NewsFixture
    {
        //[TestMethod]
        public async Task GetNewsTest()
        {
            var news = new NewsBusiness();
            var result = await news.GetNews();
            result.Should().NotBeEmpty();
        }

        [TestMethod]
        public async Task AddNews()
        {
            var news = new NewsBusiness();
            var list = new List<ViewModels.News>
            {
                new ViewModels.News
            {
                Show = true,
                Id = Guid.NewGuid(),
                Approved = true,
                Description = "Area 51 is working with Church on The Street (COTS). We have committed to assist one Sunday a month and will need 5 volunteers each month.",
                End = new DateTime(2015,12,31),
                Link = "http://www.signupgenius.com/go/9040b44a5ab23a57-church1?utm_source=Pax&utm_campaign=8c5dc78a00-Newsletter_9_6_2015&utm_medium=email&utm_term=0_e2070eb2f3-8c5dc78a00-53412961",
                Order = 1,
                Start = new DateTime(2015,9,6),
                Title = "Church on the Street"
            },
                new ViewModels.News
            {
                Show = true,
                Id = Guid.NewGuid(),
                Approved = true,
                Description = "Area 51 is working with Church on The Street (COTS). We have committed to assist one Sunday a month and will need 5 volunteers each month.",
                End = new DateTime(2015,12,31),
                Link = "http://www.signupgenius.com/go/9040b44a5ab23a57-church1?utm_source=Pax&utm_campaign=8c5dc78a00-Newsletter_9_6_2015&utm_medium=email&utm_term=0_e2070eb2f3-8c5dc78a00-53412961",
                Order = 1,
                Start = new DateTime(2015,9,6),
                Title = "Church on the Street"
            }

            };
            await news.AddNews(list);
        }
    }
}
