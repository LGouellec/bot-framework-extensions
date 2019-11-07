using bot_framework_extensions.Analyzer;
using bot_framework_extensions.Options;
using Microsoft.Bot.Builder.AI.Luis;
using System.Collections.Generic;

namespace bot_framework_extensions.Recognizer
{
    public class LuisRecognizerOptions
    {
        public LuisApplication LuisApplication { get; set; }
        public LuisPredictionOptions LuisPrediction { get; set; }
        public bool IncludeApiResults { get; set; } = false;
        public bool UseSpellService { get; set; } = false;
        public bool UseTranslateService { get; set; } = false;
        public SpellOptions SpellOptions { get; set; }
        public TranslateOptions TranslateOptions { get; set; }
        public IList<ITextAnalyzer> Middlewares { get; set; } = new List<ITextAnalyzer>();
    }
}
