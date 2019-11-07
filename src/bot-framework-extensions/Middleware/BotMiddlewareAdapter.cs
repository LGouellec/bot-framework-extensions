using Microsoft.Bot.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace bot_framework_extensions.Middleware
{
    public static class BotBuilderMiddlewareExtension
    {
        public static void Add<TMiddleware>(this IList<IMiddleware> middleware, IServiceCollection services)
            where TMiddleware : IMiddleware
        {
            middleware.Add(new BotMiddlewareAdapter<TMiddleware>(services));
        }
    }

    public class BotMiddlewareAdapter<TMiddleware> : IMiddleware
        where TMiddleware : IMiddleware
    {
        private readonly Lazy<TMiddleware> middleware;

        public BotMiddlewareAdapter(IServiceCollection services)
        {
            middleware = new Lazy<TMiddleware>(() =>
                services.BuildServiceProvider().GetRequiredService<TMiddleware>());
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {
            FixDirectLine(turnContext);
            await middleware.Value.OnTurnAsync(turnContext, next, cancellationToken);
        }

        private void FixDirectLine(ITurnContext turnContext)
        {
            if (turnContext.Activity.Recipient == null)
                turnContext.Activity.Recipient = new Microsoft.Bot.Schema.ChannelAccount();
        }            
    }
}
