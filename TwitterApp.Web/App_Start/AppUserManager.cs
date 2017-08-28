using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using TwitterApp.Data.Providers;
using TwitterApp.Web.Models;

namespace TwitterApp.Web
{
    public class AppUserManager : UserManager<ApplicationUser>
    {
        public AppUserManager(IUserStore<ApplicationUser> store)
        : base(store)
        {
        }

        public static AppUserManager Create(IdentityFactoryOptions<AppUserManager> options, IOwinContext context)
        {
            var userStore = new UserStore(context.Get<UserProvider>());
            var manager = new AppUserManager(userStore);
            return manager;
        }
    }
}