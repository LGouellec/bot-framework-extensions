using bot_framework_extensions.Converter;
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
