using ai_chatbot_support_mock.NLP.LevenshteinImpl;
using System;
using System.Collections.Generic;
using System.Text;

namespace ai_chatbot_support_mock.NLP
{
    public interface ILevenshteinCalculator
    {
        LevenshteinResult GetResults(string source, string target);
    }
}
