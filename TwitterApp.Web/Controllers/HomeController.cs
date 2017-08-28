using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using TwitterApp.Common;

namespace TwitterApp.Web.Controllers
{
    [Authorize]
    [RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claims = identity.Claims;

            // If role in claims is Administrator, send it to Admin View.
            var role = claims.Where(c => c.Type == ClaimTypes.Role);
            if (role != null && role.Any())
            {
                var r = role.FirstOrDefault().Value;
                AppUserType type;
                if (Enum.TryParse(r, out type))
                {
                    if (type == AppUserType.Administrator)
                    {
                        return View("Admin");
                    }
                }
            }

            return View();
        }

        public ActionResult Admin()
        {
            return View();
        }
    }
}