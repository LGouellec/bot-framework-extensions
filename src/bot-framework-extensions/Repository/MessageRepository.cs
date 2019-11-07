using bot_framework_extensions.Options;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace bot_framework_extensions.Repository
{
    public abstract class MessageRepository : IMessageRepository
    {
        protected readonly MessageRepositoryOptions _options;

        public abstract string RepositoryName { get; }

        public MessageRepository(IOptions<MessageRepositoryOptions> options)
        {
            _options = options.Value;
        }

        public abstract Task SaveMessageAsync(string conversationID, string message);
    }
}
