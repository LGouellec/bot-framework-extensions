using System;
using System.Collections.Generic;
using System.Text;

namespace bot_framework_extensions.Luis
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    [Serializable]
    public class LuisIntentAttribute : Attribute
    {
        public readonly string IntentName;

        public LuisIntentAttribute(string intentName)
        {
            SetField.NotNull(out this.IntentName, nameof(intentName), intentName);
        }
    }
}
