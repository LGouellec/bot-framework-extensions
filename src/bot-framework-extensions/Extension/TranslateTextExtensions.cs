using bot_framework_extensions.Converter;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bot_framework_extensions.Extension
{
    public static class TranslateTextExtensions
    {
        public static string GetLanguageForTranslate(this ITurnContext ctx)
        {
            string conversationID = ctx.Activity.Conversation.Id;
            return ManagerConversationLanguage.Instance.GetLanguage(conversationID);
        }

        public static async Task<string> Translate(this ITurnContext context, string text)
        {
            string translateText = text;
            ITextConverter textConverter = context.Get<ITextConverter>();
            if (textConverter != null)
            {
                var language = GetLanguageForTranslate(context);
                translateText = await textConverter.Translate(language, text);
            }
            return translateText;
        }

        public static async Task<Activity> CreateReply(this Activity activity, string text, ITurnContext context)
        {
            text = await context.Translate(text);
            return activity.CreateReply(text);
        }

        public static async Task<ResourceResponse> SendTranslateActivityAsync(this ITurnContext context, string textReplyToSend, string speak = null, string inputHint = "acceptingInput", CancellationToken cancellationToken = default(CancellationToken))
        {
            textReplyToSend = await context.Translate(textReplyToSend);
            return await context.SendActivityAsync(textReplyToSend, speak, inputHint, cancellationToken);
        }
    }
}
