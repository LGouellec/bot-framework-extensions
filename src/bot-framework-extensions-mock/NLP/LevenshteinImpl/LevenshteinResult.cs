using System;
using System.Collections.Generic;
using System.Text;

namespace ai_chatbot_support_mock.NLP.LevenshteinImpl
{
    public class LevenshteinResult
    {
        public IEnumerable<string> KeyWords { get; set; } = new List<string>();
        public IEnumerable<string> WordsFound { get; set; } = new List<string>();
        public int Score { get; set; }
    }
}
