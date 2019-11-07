using Microsoft.Bot.Builder;
using System.Threading;
using System.Threading.Tasks;

namespace bot_framework_extensions.Middleware
{
    public interface IMiddleware<T> where T : class
    {
        Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken));
    }
}
