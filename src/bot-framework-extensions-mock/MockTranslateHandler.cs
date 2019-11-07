using ai_chatbot_support_crosscutting.Converter;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ai_chatbot_support_mock
{
    public class MockTranslateHandler : ITranslateHandler
    {
        public async Task<string> Translate(string conversationID, string text)
        {
            return await Task.FromResult(text);
        }
    }
}
