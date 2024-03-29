﻿using bot_framework_extensions.Converter;
using bot_framework_extensions.Dialog;
using bot_framework_extensions.Luis;
using bot_framework_extensions.Recognizer;
using bot_sample.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;

namespace bot_sample.Bot
{
    public class BotSample : LuisChatBot
    {
        public BotSample(ITranslateHandler translateHandler, IDialogFactory dialogFactory, ILuisRecognizer recognizer, ITextConverter textConverter, BotAccessors botAccessors) :
            base(translateHandler, null, dialogFactory, recognizer, textConverter)
        {
            Dialogs = dialogFactory.UseDialogAccessor(botAccessors.DialogStateAccessor)
                        .Create<StockDialog>()
                        .Create<TextPrompt>("prompt")
                        .Build();
        }

        protected override async Task OnTurn(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var dialogContext = await Dialogs.CreateContextAsync(turnContext, cancellationToken);
            if (turnContext.Activity.Type == ActivityTypes.Message && dialogContext.ActiveDialog == null)
            {
                await dialogContext.Context.SendActivityAsync("Hello, What can I do for you ?");
            }
        }

        [LuisIntent("StockExchange")]
        public async Task StockIntent(DialogContext context, RecognizerResult result, CancellationToken token)
        {
            await context.BeginDialogAsync(StockDialog.Id, result, token);
        }
    }
}