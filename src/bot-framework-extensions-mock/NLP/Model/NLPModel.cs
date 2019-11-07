using System;
using System.Collections.Generic;
using System.Text;

namespace ai_chatbot_support_mock.NLP.Model
{
    class NLPModel
    {
        public string Name { get; set; }
        public Intent[] Intents { get; set; }
        public Entity[] Entities { get; set; }
        public Utterance[] Utterances { get; set; }
    }
}
