using Bot.Builder.Community.Dialogs.FormFlow;
using bot_framework_extensions.Extension;
using bot_sample.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Collections.Generic;

namespace bot_sample.Dialogs
{
    public class StockDialog : WaterfallDialog
    {
        public StockDialog(string dialogId, IEnumerable<WaterfallStep> steps = null) 
            : base(dialogId, steps)
        {
            AddStep(async (context, token) =>
            {
                StockExchangeBuider builder = new StockExchangeBuider();
                var form = builder.Build(context.Context);
                var dialog = FormDialog.FromForm(() => form, FormOptions.PromptInStart);

                return await context.Call(dialog, context.Options as RecognizerResult);
            })
            .AddStep(async (c, t) =>
            {
                return await c.PromptAsync("prompt", new PromptOptions
                {
                    Prompt = c.Context.Activity.CreateReply("Thanks you ! Bye !")
                });
            });
        }

        public static string Id => typeof(StockDialog).Name;
    }
}
