using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookMyHotelTests.BookMyHotel_Tenants.Common.Tests.UtilitiesTests
{
    [TestClass]
    public class UtilitiesTests
    {
        [TestMethod]
        public void GetUser()
        {
            var host = "bookings.wtp.bg1.trafficmanager.net";
            var hostpieces = host.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            var user = hostpieces[2];

            Assert.AreEqual("bg1", user);
        }

        [TestMethod]
        public void GetUser2()
        {
            var host = "localhost:5000";
            string[] hostpieces = host.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            var subdomain = hostpieces[0];
            Assert.AreEqual("localhost:5000", subdomain);
        }
    }
}
