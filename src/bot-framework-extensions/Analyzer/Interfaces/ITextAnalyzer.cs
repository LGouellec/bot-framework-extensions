using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace bot_framework_extensions.Analyzer
{
    public interface ITextAnalyzer
    {
        Task<string> Analyze(ContextAnalyzer context, string text);
    }
}
