using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using TwitterApp.Common.Interfaces;
using TwitterApp.Data.Providers;

namespace TwitterApp.Data.Test
{
    [TestClass]
    public class TwitterMessagesProviderTest
    {
        IMessagesProvider _provider = new TwitterMessagesProvider("OSVmi0KrMY66FbCykFQzEWoH9", "9n0vNxKq0uxXmi5QxnUontDyXRXELZZvzWifFM9n0uhJ4xfeUL", "363192712-pDPmZOIOACD4SRhfNer56mDsrcT0ebwOYMEvdXr3", "F78GWPtxONkJI7vQ8QvANd6xoMkxjcGnRUUMaLmIni1Uu");

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task GetTopMessages_nullUserName_ArgumentNullException()
        {
            var result = await _provider.GetMessages(null, 10, null);
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task GetTopMessages_emptyUserName_ArgumentNullException()
        {
            var result = await _provider.GetMessages(string.Empty, 10, null);
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task GetTopMessages_whiteSpaveUserName_ArgumentNullException()
        {
            var result = await _provider.GetMessages(" ", 10, null);
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetTopMessages_invalidCount_ArgumentException()
        {
            var result = await _provider.GetMessages("arxskoven", 0, null);
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task SendNewMessage_null_ArgumentNullException()
        {
            var result = await _provider.SendNewMessage(null);
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task SendNewMessage_empty_ArgumentNullException()
        {
            var result = await _provider.SendNewMessage(string.Empty);
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task SendNewMessage_whiteSpace_ArgumentNullException()
        {
            var result = await _provider.SendNewMessage("   ");
            Assert.IsNull(result);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public async Task GetTopMessages_ValidUserNameAndCount_notEmpty()
        {
            var result = await _provider.GetMessages("arxskoven", 10, null);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Any());
        }

        [TestMethod]
        [TestCategory("Integration")]
        public async Task SendNewMessage_ValidMessage_true()
        {
            var result = await _provider.SendNewMessage("hello world from API!");
            Assert.IsTrue(result);
        }
    }
}
