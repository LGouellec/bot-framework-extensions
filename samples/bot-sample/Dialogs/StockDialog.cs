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
        internal class StockDialogResult
        {
            public string Compagny { get; set; }
            public float Price { get; set; }

        }

        public StockDialog(string dialogId, IEnumerable<WaterfallStep> steps = null) 
            : base(dialogId, steps)
        {
            AddStep(async (context, token) =>
            {
                StockExchangeBuider builder = new StockExchangeBuider();
                var form = await builder.Build(context.Context);
                var dialog = FormDialog.FromForm(() => form, FormOptions.PromptInStart);

                await context.Call(dialog, context.Options as RecognizerResult);
                return await context.EndDialogAsync();
            });
        }

        public static string Id => typeof(StockDialog).Name;
    }
}
