using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;
using TwitterApp.Common;
using TwitterApp.Common.Interfaces;
using TwitterApp.Web.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace TwitterApp.Web
{
    public class UserStore : 
        IUserStore<ApplicationUser>, 
        IUserPasswordStore<ApplicationUser>, 
        IUserLockoutStore<ApplicationUser, string>,
        IUserTwoFactorStore<ApplicationUser, string>,
        IUserClaimStore<ApplicationUser, string>
    {
        IUserProvider _provider;

        public UserStore()
        {
        }

        public UserStore(IUserProvider provider)
        {
            _provider = provider;
        }

        public Task CreateAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var appUser = new AppUser(0, user.UserName, user.PasswordHash, user.Type);
            _provider.SaveUser(appUser);
            user.Id = appUser.Id.ToString();

            return Task.FromResult(user);
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            _provider.RemoveUser(user.UserName);
            return Task.FromResult<object>(null);
        }

        public Task<ApplicationUser> FindByIdAsync(string id)
        {
            ApplicationUser result = null;
            var idInt = 0;
            if (int.TryParse(id, out idInt))
            {
                var user = _provider.GetUserById(idInt);
                if (user != null)
                {
                    result = new ApplicationUser(user.Id.ToString(), user.Email, user.Password, user.Type);
                }
            }

            return Task.FromResult(result);
        }

        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            ApplicationUser result = null;
            var user = _provider.GetUserByEmail(userName);
            if (user != null)
            {
                result = new ApplicationUser(user.Id.ToString(), user.Email, user.Password, user.Type);
            }

            return Task.FromResult(result);
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            var id = 0;
            if (user == null && !int.TryParse(user.Id, out id))
            {
                throw new ArgumentNullException(nameof(user));
            }

            _provider.SaveUser(new AppUser(id, user.UserName, user.PasswordHash, user.Type));
            return Task.FromResult<object>(null);
        }

        public void Dispose()
        {
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            return Task.FromResult(string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset lockoutEnd)
        {
            throw new NotImplementedException();
        }

        public Task<int> IncrementAccessFailedCountAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task ResetAccessFailedCountAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetAccessFailedCountAsync(ApplicationUser user)
        {
            return Task.FromResult(0);
        }

        public Task<bool> GetLockoutEnabledAsync(ApplicationUser user)
        {
            return Task.FromResult(false);
        }

        public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user)
        {
            return Task.FromResult(false);
        }

        public Task<IList<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            IList<Claim> claims = new List<Claim>()
            {                
                new Claim(ClaimTypes.Role.ToString(), user.Type.ToString()),
            };

            return Task.FromResult(claims);
        }

        public Task AddClaimAsync(ApplicationUser user, Claim claim)
        {
            throw new NotImplementedException();
        }

        public Task RemoveClaimAsync(ApplicationUser user, Claim claim)
        {
            throw new NotImplementedException();
        }
    }
}