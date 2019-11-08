using Bot.Builder.Community.Dialogs.FormFlow;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace bot_sample.Bot
{
    [Serializable]
    internal class StockExchangeQuery
    {
        [Prompt("Please enter the name of the society")]
        public string Society { get; set; }
    }

    internal class StockExchangeBuider
    {
        private async Task Action(DialogContext c, StockExchangeQuery query)
        {
            await c.Context.SendActivityAsync("Apple : 15USD", "notTranslate");
            await c.EndDialogAsync();
        }

        public async Task<IForm<StockExchangeQuery>> Build(ITurnContext context)
        {
            var builder = new FormBuilder<StockExchangeQuery>()
                .Field(nameof(StockExchangeQuery.Society), (state) => string.IsNullOrEmpty(state?.Society));
            return await Task.FromResult(builder.OnCompletion(this.Action).Build());
        }
    }
}
