using bot_framework_extensions.State;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace bot_framework_extensions.Converter
{
    public interface ITranslateHandler
    {
        Task<string> Translate(string conversationID, string text);
    }

    public class TranslateHandler : ITranslateHandler
    {
        private readonly ITextConverter _textConverter;

        public TranslateHandler(ITextConverter textConverter)
        {
            _textConverter = textConverter;        
        }

        public async Task<string> Translate(string conversationID, string text)
        {
            var language = ManagerConversationLanguage.Instance.GetLanguage(conversationID);
            return await _textConverter.Translate(language, text);
        }
    }
}
