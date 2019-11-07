using bot_framework_extensions.Bot;
using bot_framework_extensions.Converter;
using bot_framework_extensions.Dialog;
using bot_framework_extensions.Extension;
using bot_framework_extensions.Recognizer;
using bot_framework_extensions.Repository;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace bot_framework_extensions.Luis
{
    public abstract class LuisChatBot : BotBase
    {
        private static IDictionary<string, MethodInfo> _members = null;
        private static readonly object _locker = new object();

        protected readonly ITextConverter _textConverter = null;
        protected readonly ILuisRecognizer _recognizer = null;

        #region Ctor

        public LuisChatBot(ITranslateHandler translateHandler, IMessageRepository messageRepository, IDialogFactory dialogFactory, ILogger<BotBase> logger)
            : base(translateHandler, messageRepository, dialogFactory, logger)
        {
            MakeServicesFromAttributes();
        }

        public LuisChatBot(ITranslateHandler translateHandler, IMessageRepository messageRepository, IDialogFactory dialogFactory, ILuisRecognizer recognizer, ITextConverter textConverter, ILogger<BotBase> logger)
            : this(translateHandler ,messageRepository, dialogFactory, logger)
        {
            _recognizer = recognizer;
            _textConverter = textConverter;
        }

        #endregion

        #region Abstract 

        protected abstract Task OnTurn(ITurnContext turnContext, CancellationToken cancellationToken);

        #endregion

        #region < IBot >

        protected override async Task OnTurnImplementationAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            if (_textConverter != null)
                turnContext.Inject(_textConverter);

            var dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                var results = await dialogContext.ContinueDialogAsync(cancellationToken);

                if ((results.Status == DialogTurnStatus.Cancelled || results.Status == DialogTurnStatus.Empty)
                      || (results.Status == DialogTurnStatus.Complete && dialogContext.ActiveDialog == null))
                    // Test not recognize luis when finishing active dialog
                if (results.Status == DialogTurnStatus.Cancelled || results.Status == DialogTurnStatus.Empty)
                {
                    DialogFormCaching._dialogs.Remove(turnContext.Activity.Conversation.Id);
                    var foundDialog = _dialogs.Find(turnContext.Activity.Text);
                    if (foundDialog != null)
                    {
                        await dialogContext.BeginDialogAsync(foundDialog.Id, null, cancellationToken);
                    }
                    else
                    {
                        var result = await Recognize(turnContext, cancellationToken);
                        if (!result)
                            await OnTurn(turnContext, cancellationToken);
                    }
                }
                else
                    await OnTurn(turnContext, cancellationToken);
            }
            else
                await OnTurn(turnContext, cancellationToken);
        }

        #endregion

        #region < Init >

        private void MakeServicesFromAttributes()
        {
            lock (_locker)
            {
                if (_members == null)
                {
                    _members = new Dictionary<string, MethodInfo>();
                    var members = GetType().GetMethods()
                              .Where(m => m.GetCustomAttributes(typeof(LuisIntentAttribute), false).Length > 0)
                              .Select(m => (m.GetCustomAttributes<LuisIntentAttribute>().Select(t => t.IntentName), m));

                    foreach (var m in members)
                        foreach (var intentKey in m.Item1)
                            _members.Add(intentKey, m.m);
                }
            }
        }
        
        #endregion

        private async Task<bool> Recognize(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);
            RecognizerResult result = turnContext.Get<RecognizerResult>();
            if (result != null)
            {
                var topIntent = result?.GetTopScoringIntent();

                string memberKey = _members.Select(kp => kp.Key).FirstOrDefault((kp) => kp.Equals(topIntent.Value.intent, StringComparison.InvariantCultureIgnoreCase));
                if (memberKey is null)
                    return false;
                Task task = (Task)_members[memberKey].Invoke(this, new object[] { dialogContext, result, cancellationToken });
                await task;
                result = null;
                return true;
            }
            else
                return false;
        }
    }
}
