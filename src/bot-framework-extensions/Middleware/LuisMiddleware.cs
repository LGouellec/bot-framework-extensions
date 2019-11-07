using bot_framework_extensions.Extension;
using bot_framework_extensions.Middleware;
using bot_framework_extensions.Recognizer;
using Microsoft.Bot.Builder;
using System.Threading;
using System.Threading.Tasks;

namespace bot_framework_extensions.Luis
{
    public class LuisMiddleware : Microsoft.Bot.Builder.IMiddleware, IMiddleware<LuisMiddleware>
    {
        private readonly ILuisRecognizer _recognizer = null;

        public LuisMiddleware(ILuisRecognizer recognizer)
        {
            _recognizer = recognizer;
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(turnContext.Activity.Text) && _recognizer != null)
            {
                var result = await _recognizer.RecognizeAsync(turnContext, cancellationToken);
                turnContext.Inject(result);
            }
            await next(cancellationToken);
        }
    }
}
