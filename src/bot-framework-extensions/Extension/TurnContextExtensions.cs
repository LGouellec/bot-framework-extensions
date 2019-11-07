using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace bot_framework_extensions.Extension
{
    public static class TurnContextExtensions
    {
        public static void Inject<T>(this ITurnContext context, T service) where T : class
        {
            if (!context.TurnState.ContainsKey(typeof(T).Name))
                context.TurnState.Add(typeof(T).Name, service);
            else
                context.TurnState[typeof(T).Name] = service;
        }

        public static T Get<T>(this ITurnContext context) where T : class
        {
            return context.TurnState.Get<T>(typeof(T).Name);
        }
    }
}
