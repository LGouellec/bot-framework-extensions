using ai_chatbot_support_mock.NLP.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ai_chatbot_support_mock.NLP
{
    public class NLPEntity
    {
        public string Entity { get; set; }
        public string Type { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
    }

    public class NLPResult
    {
        public Dictionary<string, double> Intents { get; set; }
        public NLPEntity[] Entities { get; set; }

        public (string, double) GetTopIntent()
        {
            var intent = Intents.OrderByDescending(kp => kp.Value)?.FirstOrDefault(i => i.Value >= 0.30);
            if (intent.HasValue)
                return (intent.Value.Key, intent.Value.Value);
            else
                return default;
        }
    }
}
