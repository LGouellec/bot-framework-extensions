using System;
using System.Collections.Generic;
using System.Linq;

namespace bot_framework_extensions.State
{
    public class ManagerConversationLanguage
    {
        private static readonly object _lock = new object();
        private static readonly IDictionary<string, (string, DateTime)> _languagesDetected = new Dictionary<string, (string, DateTime)>();

        private ManagerConversationLanguage() { }

        public static ManagerConversationLanguage Instance { get; } = new ManagerConversationLanguage();

        public void LanguageDetected(string conversationID, string language)
        {
            lock (_lock)
            {
                // Remove old conversation (1h)
                var listKeys = _languagesDetected.Where(kp => kp.Value.Item2.AddHours(1) <= DateTime.Now).Select(kp => kp.Key).ToList();
                foreach (var key in listKeys)
                    _languagesDetected.Remove(key);

                if (!_languagesDetected.ContainsKey(conversationID))
                    _languagesDetected.Add(conversationID, (language, DateTime.Now));
            }
        }

        public string GetLanguage(string conversationID)
        {
            if (_languagesDetected.ContainsKey(conversationID))
                return _languagesDetected[conversationID].Item1;
            else
                return "en-US";
        }
    }
}
