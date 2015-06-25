using System;
using System.Threading.Tasks;
using F3.Business.News;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F3.Business.Tests
{
    //[TestClass]
    public class NewsFixture
    {
        [TestMethod]
        public async Task GetNewsTest()
        {
            var news = new NewsBusiness();
            var result = await news.GetNews();
            result.Should().NotBeEmpty();
        }
    }
}
