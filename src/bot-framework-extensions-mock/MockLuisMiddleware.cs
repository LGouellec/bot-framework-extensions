using ai_chatbot_support_crosscutting.Analyzer;
using ai_chatbot_support_crosscutting.Recognizer;
using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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
