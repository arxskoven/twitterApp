using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.Threading.Tasks;
using TwitterApp.Common;
using TwitterApp.Common.Interfaces;
using TwitterApp.Data.Providers;

namespace TwitterApp.Data.Test
{
    [TestClass]
    public class UserProviderTest
    {
        IUserProvider _provider = new UserProvider(ConfigurationManager.ConnectionStrings["AppUser"].ConnectionString);

        [TestInitialize]
        public void Initialize()
        {
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetUserByEmail_null_ArgumentNullException()
        {
            var user = _provider.GetUserByEmail(null);
            Assert.IsNotNull(user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetUserByEmail_empty_ArgumentNullException()
        {
            var user = _provider.GetUserByEmail("");
            Assert.IsNotNull(user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetUserByEmail_whiteSpace_ArgumentNullException()
        {
            var user = _provider.GetUserByEmail("    ");
            Assert.IsNotNull(user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveUser_null_ArgumentNullException()
        {
            var result = _provider.SaveUser(null);
            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveUser_nullEmail_ArgumentNullException()
        {
            var result = _provider.SaveUser(new AppUser(0, null, "", AppUserType.Regular));
            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveUser_emptyEmail_ArgumentNullException()
        {
            var result = _provider.SaveUser(new AppUser(0, string.Empty, "", AppUserType.Regular));
            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveUser_whiteSpaceEmail_ArgumentNullException()
        {
            var result = _provider.SaveUser(new AppUser(0, "   ", "", AppUserType.Regular));
            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task RemoveUser_null_ArgumentNullException()
        {
            var result = await _provider.RemoveUser(null);
            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task RemoveUser_empty_ArgumentNullException()
        {
            var result = await _provider.RemoveUser(string.Empty);
            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task RemoveUser_whiteSpace_ArgumentNullException()
        {
            var result = await _provider.RemoveUser("      ");
            Assert.IsTrue(result);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void SaveUser_valid_true()
        {
            var result = _provider.SaveUser(new AppUser(0, "aldo.sandoval@ymail.com", "", AppUserType.Administrator));
            Assert.IsTrue(result);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void GetUserByEmail_valid_notNull()
        {
            var user = _provider.GetUserByEmail("aldo.sandoval@ymail.com");
            Assert.IsNotNull(user);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public async Task RemoveUser_valid_true()
        {
            var result = await _provider.RemoveUser("aldo.sandoval@ymail.com");
            Assert.IsTrue(result);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void GetUsers_notNull()
        {
            var users = _provider.GetUsers();
            Assert.IsNotNull(users);
        }
    }
}
