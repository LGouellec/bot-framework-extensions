using bot_framework_extensions.Analyzer;
using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace bot_framework_extensions.Recognizer
{
    public interface ILuisRecognizer : IRecognizer
    {
        LuisRecognizerOptions Options { get; }
        void BuildLuisRecognizer(Action<LuisRecognizerOptions> action);
    }

    public interface ILuisRecognizerPipeline : ILuisRecognizer
    {
        ILuisRecognizerPipeline Pipeline<T>(Func<T> func) where T : class, ITextAnalyzer;
        ILuisRecognizerPipeline Pipeline<T>(T analyzer) where T : class, ITextAnalyzer;
    }
}
