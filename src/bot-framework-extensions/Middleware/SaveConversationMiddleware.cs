using bot_framework_extensions.Middleware;
using bot_framework_extensions.Repository.Interfaces;
using Microsoft.Bot.Builder;
using System.Threading;
using System.Threading.Tasks;

namespace bot_framework_extensions.Middleware
{
    public class SaveConversationMiddleware : IMiddleware, IMiddleware<SaveConversationMiddleware>
    {
        private readonly IMessageRepository _messageRepository;

        public SaveConversationMiddleware(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _messageRepository.SaveMessageAsync(turnContext.Activity.Conversation.Id, turnContext.Activity.Text);
            await next(cancellationToken);
        }
    }
}
