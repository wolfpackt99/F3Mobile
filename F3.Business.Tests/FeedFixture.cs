using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F3.Business.Tests
{
    [TestClass]
    public class FeedFixture
    {
        [TestMethod]
        public void GetPostsTest()
        {
            var x = new Feed();
            var posts = x.GetPosts();
            posts.Should().HaveCount(c => c > 0);

        }
    }
}
