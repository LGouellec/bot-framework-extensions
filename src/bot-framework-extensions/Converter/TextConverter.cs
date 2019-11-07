using bot_framework_extensions.Analyzer;
using bot_framework_extensions.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace bot_framework_extensions.Converter
{
    public class TextConverter : TranslateAnalyzer, ITextConverter
    {
     
        public TextConverter(IOptions<TranslateOptions> options)
           : base(options?.Value)
        {
        }

        public TextConverter(TranslateOptions options) 
            : base(options)
        {
        }

        public async Task<string> Translate(string language, string text)
        {
            ContextAnalyzer ctx = new ContextAnalyzer();
            toLanguage = language;
            return await Analyze(ctx, text);
        }
    }
}
