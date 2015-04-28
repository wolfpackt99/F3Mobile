﻿using System;
using System.Threading.Tasks;
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
            var x = new GoogleContactBusiness();
            var list = await x.GetContacts();
            list.Should().NotBeNull("Should be able to get contacts.");
        }
    }
}