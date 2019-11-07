using System.Collections.Generic;
using System.Threading.Tasks;

namespace bot_framework_extensions.Repository
{
    public interface IMessageRepository
    {
        string RepositoryName { get; }
        Task SaveMessageAsync(string conversationID, string message);
    }
}
