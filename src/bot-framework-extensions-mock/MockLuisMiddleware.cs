using bot_framework_extensions.Analyzer;
using System.Threading.Tasks;

namespace ai_chatbot_support_mock
{
    public class MockLuisMiddleware : ITextAnalyzer
    {
        public Task<string> Analyze(ContextAnalyzer context, string text)
        {
            context.LanguageDetected = true;
            context.Language = "en-US";
            return Task.FromResult(text);
        }
    }
}
