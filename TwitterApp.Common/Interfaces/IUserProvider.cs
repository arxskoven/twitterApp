using System.Collections.Generic;
using System.Threading.Tasks;

namespace TwitterApp.Common.Interfaces
{
    /// <summary>
    /// User provider interface, provides the contract for user management methods.
    /// </summary>
    public interface IUserProvider
    {
        AppUser GetUserById(int id);

        AppUser GetUserByEmail(string email);

        Task<List<AppUser>> GetUsers();

        bool SaveUser(AppUser user);

        Task<bool> RemoveUser(string email);
    }
}
