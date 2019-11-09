using Bot.Builder.Community.Dialogs.FormFlow;
using bot_framework_extensions.Extension;
using Microsoft.Bot.Builder;
using System;

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
        public IForm<StockExchangeQuery> Build(ITurnContext context)
        {
            var builder = new FormBuilder<StockExchangeQuery>()
                .Field(nameof(StockExchangeQuery.Society), (state) => string.IsNullOrEmpty(state?.Society))
                .OnCompletion(async (c, q) =>
                {
                    await c.Context.SendActivityAsync($"{q?.Society} : 15USD", "notTranslate");
                    c.ClearCache();
                });
            return builder.Build();
        }
    }
}
