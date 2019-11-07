using System;
using System.Collections.Generic;
using System.Text;

namespace bot_framework_extensions.Analyzer
{
    public class ContextAnalyzer
    {
        public bool LanguageDetected { get; set; } = false;
        public string Language { get; set; } = string.Empty;
    }
}
