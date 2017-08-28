using System.Collections.Generic;
using System.Threading.Tasks;

namespace TwitterApp.Common.Interfaces
{
    /// <summary>
    /// Messages provider contract, get messages and save new messages.
    /// </summary>
    public interface IMessagesProvider
    {
        Task<IList<string>> GetMessages(string userName, int count, string hashTag);

        Task<bool> SendNewMessage(string message);
    }
}
