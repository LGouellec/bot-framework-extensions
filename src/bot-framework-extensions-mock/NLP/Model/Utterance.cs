using System;
using System.Collections.Generic;
using System.Text;

namespace ai_chatbot_support_mock.NLP.Model
{
    class Utterance
    {
        public string Text { get; set; }
        public Intent Intent { get; set; }
        public Entity[] Entities { get; set; } = new Entity[0];

        public Dictionary<Entity, List<string>> GetEntitiesValues()
        {
            Dictionary<Entity, List<string>> values = new Dictionary<Entity, List<string>>();
            foreach(var entity in Entities)
            {
                if (values.ContainsKey(entity))
                    values[entity].Add(Text.Substring(entity.StartIndex, entity.StopIndex - entity.StartIndex));
                else
                    values.Add(entity, new List<string>() { Text.Substring(entity.StartIndex, entity.StopIndex - entity.StartIndex) });
            }

            return values;
        }
    }
}
