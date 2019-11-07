using System.Collections.Generic;

namespace ai_chatbot_support_mock.NLP
{
    internal class Annotation
    {
        public string AlteredText { get; set; }
        public string Text { get; private set; }
        public IList<string> KeyWords { get; internal set; }
        public string Language { get; internal set; }

        public Annotation(string text)
        {
            Text = text;
            AlteredText = text.ToLowerInvariant();
            KeyWords = new List<string>();
        }
    }
}
