using ai_chatbot_support_crosscutting.Converter;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ai_chatbot_support_mock
{
    public class MockTextConverter : ITextConverter
    {
        public Task<string> Translate(string language, string text)
        {
            return Task.FromResult(text);
        }
    }
}
