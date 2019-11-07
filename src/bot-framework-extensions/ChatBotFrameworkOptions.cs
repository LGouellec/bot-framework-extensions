using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace bot_framework_extensions
{
    public enum CHABOT_MESSAGE_REPOSITORY
    {
        NONE,
        API,
        IN_MEMORY,
        CUSTOM
    }

    public class ChatBotFrameworkOptions
    {
        private readonly IList<IMiddleware> _internalMiddlewares = new List<IMiddleware>();

        public bool EnableLuisDetection { get; set; } = true;
        public bool UsingSpellBeforeLuis { get; set; } = true;
        public bool UsingTranslateBeforeLuis { get; set; } = true;
        public bool EnableDetectionUserLanguage { get; set; } = true;
        public string LuisModelLanguage { get; set; } = "en-US";
        public CHABOT_MESSAGE_REPOSITORY TypeMessageRepository { get; set; } = CHABOT_MESSAGE_REPOSITORY.NONE;
        public Type CustomTypeMessageRepository { get; set; }
        public List<IMiddleware> Middlewares { get; set; } = new List<IMiddleware>();
    }
}
