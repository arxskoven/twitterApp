using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using TwitterApp.Common;
using TwitterApp.Common.Interfaces;
using TwitterApp.Data.Providers;

namespace TwitterApp.Web.Controllers
{
    /// <summary>
    /// User API controller
    /// </summary>
    [Authorize]      
    public class UserController : ApiController
    {
        IUserProvider _provider;

        public UserController()
        {
            _provider = UserProvider.Create();
        }

        public UserController(IUserProvider provider)
        {
            _provider = provider;
        }

        [HttpGet]
        [Route("api/user/getUsers")]
        public async Task<IList<AppUser>> GetUsers()
        {
            return await _provider.GetUsers();
        }

        [HttpPost]
        [Route("api/user/removeUser")]
        public bool SendMessage([FromUri] string email)
        {
            return _provider.RemoveUser(email).Result;
        }
    }
}
