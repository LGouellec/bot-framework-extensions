using System;
using System.Collections.Generic;
using System.Text;

namespace ai_chatbot_support_mock.NLP.Model
{
    class Entity
    {
        public IEnumerable<string> Values { get; set; } = new List<string>();
        public string Name { get; set; }
        public int StartIndex { get; set; }
        public int StopIndex { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Entity && ((Entity)obj).Name.Equals(Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
