using bot_framework_extensions.Converter;
using bot_framework_extensions.Dialog;
using bot_framework_extensions.Extension;
using bot_framework_extensions.Repository;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace bot_framework_extensions.Bot
{
    public abstract class BotBase : IBot
    {
        private delegate Task InternalMiddleware(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default);

        private readonly IDialogFactory _dialogFactory;
        private readonly IMessageRepository _messageRepository;
        private readonly ITranslateHandler _translateHandler;

        private readonly IList<InternalMiddleware> internalMiddlewares = new List<InternalMiddleware>();

        protected DialogSet _dialogs;
        protected readonly ILogger _logger;

        public BotBase(ITranslateHandler translateHandler, IMessageRepository messageRepository, IDialogFactory dialogFactory, ILogger<BotBase> logger)
        {
            _logger = logger;
            _dialogFactory = dialogFactory;
            _messageRepository = messageRepository;
            _translateHandler = translateHandler;
            internalMiddlewares.Add(SecureMiddleware);
            internalMiddlewares.Add(CancelMiddleware);
            internalMiddlewares.Add(HelpMiddleware);
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            var context = new TurnContextAdapter(turnContext, _logger);
            context.UseMessageRepository(_messageRepository).UseTranslateHandler(_translateHandler);
                
            CompleteDialogSet(context);

            await RunMiddleware(context, cancellationToken);
            await OnTurnImplementationAsync(context, cancellationToken);
            await Task.CompletedTask;
        }

        #region Abstract 

        protected abstract Task OnTurnImplementationAsync(ITurnContext turnContext, CancellationToken cancellation = default);

        #endregion

        #region Private 

        private void CompleteDialogSet(ITurnContext context)
        {
            if (_dialogs != null)
            {
                foreach (var d in DialogFormCaching._dialogs.Where(kp => kp.Key.Equals(context.Activity.Conversation.Id)))
                    if (_dialogs.Find(d.Value.Id) == null)
                        _dialogs.Add(d.Value);
            }
        }

        #endregion

        #region Internal Methods Middlewares

        protected virtual async Task SecureMiddleware(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            var result = turnContext.Get<RecognizerResult>();
            if (result != null)
            {
                // maybe not only top scoring intent
                (string intent, double score) = result.GetTopScoringIntent();
                if (intent.Equals("BadUsage"))
                {
                    string text = turnContext.Activity.Text;
                    var luisResult = result.Properties["luisResult"] as LuisResult;
                    foreach (var entitie in luisResult?.Entities)
                    {
                        text = text.Replace(entitie.Entity, string.Concat(Enumerable.Repeat("*", entitie.Entity.Length)));
                    }
                    turnContext.Activity.Text = text;

                    await turnContext.SendActivityAsync("Warning bad usage, it's forbidden to send password, login by skype or other chats service");
                    await turnContext.SendActivityAsync("Some sensitive information has been erased and/or replaced.");
                    await turnContext.SendActivityAsync("TODO : Display usage of chatbot ...");
                }
            }

            await next(cancellationToken);
        }

        protected virtual async Task CancelMiddleware(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            var result = turnContext.Get<RecognizerResult>();
            if (result != null)
            {
                // maybe not only top scoring intent
                (string intent, double score) = result.GetTopScoringIntent();
                if (intent.Equals("Cancel"))
                {
                    var dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);
                    await dialogContext.CancelDialogs(cancellationToken);
                    await dialogContext.BeginDialogAsync(nameof(CancelDialog), null, cancellationToken);
                }
                else
                    await next(cancellationToken);
            }
            else
                await next(cancellationToken);
        }

        protected virtual async Task HelpMiddleware(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            var result = turnContext.Get<RecognizerResult>();
            if (result != null)
            {
                // maybe not only top scoring intent
                (string intent, double score) = result.GetTopScoringIntent();
                if (intent.Equals("Help"))
                {
                    var dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);
                    await dialogContext.CancelDialogs(cancellationToken);
                    await dialogContext.BeginDialogAsync(nameof(HelpDialog), null, cancellationToken);
                }
                else
                    await next(cancellationToken);
            }
            else
                await next(cancellationToken);
        }

        protected virtual async Task RunMiddleware(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await Run(turnContext, 0, cancellationToken);
        }

        private async Task Run(ITurnContext turnContext, int index, CancellationToken cancellationToken = default)
        {
            if (index < internalMiddlewares.Count - 1)
                await internalMiddlewares[index](turnContext, async (token) => { await Run(turnContext, index + 1, cancellationToken); }, cancellationToken);
            else
                await internalMiddlewares[index](turnContext, async (token) => { await Task.CompletedTask; }, cancellationToken);
        }

        #endregion

    }
}
