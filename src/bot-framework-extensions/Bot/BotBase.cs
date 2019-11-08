using bot_framework_extensions.Converter;
using bot_framework_extensions.Dialog;
using bot_framework_extensions.Extension;
using bot_framework_extensions.Repository;
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
        protected delegate Task InternalMiddleware(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default);

        private readonly IList<InternalMiddleware> internalMiddlewares = new List<InternalMiddleware>();

        protected DialogSet Dialogs { get; set; }
        protected readonly ILogger _logger;

        #region Property

        public IDialogFactory DialogFactory { get; }
        public IMessageRepository MessageRepository { get; }
        public ITranslateHandler TranslateHandler { get; }

        #endregion


        public BotBase(ITranslateHandler translateHandler, IMessageRepository messageRepository, IDialogFactory dialogFactory, ILogger<BotBase> logger)
        {
            _logger = logger;
            DialogFactory = dialogFactory;
            MessageRepository = messageRepository;
            TranslateHandler = translateHandler;
        }

        #region Ctor

        public BotBase(ITranslateHandler translateHandler, IMessageRepository messageRepository, IDialogFactory dialogFactory)
                :this(translateHandler, messageRepository, dialogFactory, null)
        {
        }

        public BotBase(ITranslateHandler translateHandler, IMessageRepository messageRepository)
                : this(translateHandler, messageRepository, null, null)
        {
        }

        public BotBase(ITranslateHandler translateHandler)
                : this(translateHandler, null, null, null)
        {
        }

        public BotBase() : this(null) { }

        #endregion

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            var context = new TurnContextAdapter(turnContext, _logger);
            context.UseMessageRepository(MessageRepository).UseTranslateHandler(TranslateHandler);
                
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
            if (Dialogs != null)
            {
                foreach (var d in DialogFormCaching._dialogs.Where(kp => kp.Key.Equals(context.Activity.Conversation.Id)))
                    if (Dialogs.Find(d.Value.Id) == null)
                        Dialogs.Add(d.Value);
            }
        }

        #endregion

        #region Internal Methods Middlewares

        protected BotBase AddInternalMiddleware(InternalMiddleware middleware)
        {
            internalMiddlewares.Add(middleware);
            return this;
        }

        protected virtual async Task RunMiddleware(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await Run(turnContext, 0, cancellationToken);
        }

        private async Task Run(ITurnContext turnContext, int index, CancellationToken cancellationToken = default)
        {
            if (index < internalMiddlewares.Count - 1)
                await internalMiddlewares[index](turnContext, async (token) => { await Run(turnContext, index + 1, cancellationToken); }, cancellationToken);
            else if(internalMiddlewares.Count > 0)
                await internalMiddlewares[index](turnContext, async (token) => { await Task.CompletedTask; }, cancellationToken);
        }

        #endregion

    }
}
