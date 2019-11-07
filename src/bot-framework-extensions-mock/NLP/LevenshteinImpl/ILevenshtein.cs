using System;
using System.Collections.Generic;
using System.Text;

namespace ai_chatbot_support_mock.NLP.LevenshteinImpl
{
    internal interface ILevenshtein
    {
        int Get(string a, string b);
    }
}
