using System;
using System.Threading;
using System.Threading.Tasks;
using F3.Infrastructure.GoogleAuth;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F3.Business.Tests
{
    [TestClass]
    public class TokenFixture
    {
        //[TestMethod]
        public async Task TestGetToken()
        {
            var work = await ServiceAccount.Instance.Credential.RequestAccessTokenAsync(new CancellationToken());
            work.ShouldBeEquivalentTo(true);
            ServiceAccount.Instance.Credential.Token.Should().NotBeNull();
        }
    }
}
