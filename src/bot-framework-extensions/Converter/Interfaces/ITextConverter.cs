using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace bot_framework_extensions.Converter
{
    public interface ITextConverter
    {
        Task<string> Translate(string language, string text);
    }
}
