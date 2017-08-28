using Microsoft.AspNet.Identity;
using TwitterApp.Common;

namespace TwitterApp.Web.Models
{
    public class ApplicationUser : IUser<string>
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string PasswordHash { get; set; }

        public AppUserType Type { get; set; }

        public ApplicationUser()
        {
        }

        public ApplicationUser(string id, string userName, string passwordHash, AppUserType type)
        {
            Id = id;
            UserName = userName;
            Type = type;
            PasswordHash = passwordHash;
        }
    }
}