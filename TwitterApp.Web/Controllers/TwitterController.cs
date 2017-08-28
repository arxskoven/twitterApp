using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using TwitterApp.Common.Interfaces;
using TwitterApp.Data.Providers;

namespace TwitterApp.Web.Controllers
{
    /// <summary>
    /// Twitter API controller
    /// </summary>
    [Authorize]
    public class TwitterController : ApiController
    {
        IMessagesProvider _provider;

        public TwitterController()
        {
            _provider = new TwitterMessagesProvider(
              ConfigurationManager.AppSettings["Twitter.ConsumerKey"].ToString(),
              ConfigurationManager.AppSettings["Twitter.ConsumerSecret"].ToString(),
              ConfigurationManager.AppSettings["Twitter.AccessToken"].ToString(),
              ConfigurationManager.AppSettings["Twitter.AccessTokenSecret"].ToString());
        }

        public TwitterController(IMessagesProvider provider)
        {
            _provider = provider;
        }

        [HttpGet]
        [Route("api/twitter/getMessages")]
        public async Task<IList<string>> GetMessages([FromUri] string userName = null, [FromUri] int count = 10, [FromUri] string hashTag = null)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                userName = User.Identity.Name;
            }

            return await _provider.GetMessages(userName, count, hashTag);
        }

        [HttpPost]
        [Route("api/twitter/sendMessage")]
        public async Task<bool> SendMessage([FromUri] string message)
        {
            return await _provider.SendNewMessage(message);
        }
    }
}
