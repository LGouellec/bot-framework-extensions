using System;
using System.Collections.Generic;
using System.Text;

namespace ai_chatbot_support_mock.NLP.Model
{
    class Intent
    {
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Intent && ((Intent)obj).Name.Equals(Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
