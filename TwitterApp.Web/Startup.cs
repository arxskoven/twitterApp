using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TwitterApp.Web.Startup))]
namespace TwitterApp.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
