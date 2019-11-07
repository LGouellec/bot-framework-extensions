using Bot.Builder.Community.Dialogs.FormFlow;
using Bot.Builder.Community.Dialogs.FormFlow.Luis.Models;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bot_framework_extensions.Extension
{
    internal class DialogFormCaching
    {
        public static IDictionary<string, Microsoft.Bot.Builder.Dialogs.Dialog> _dialogs = new Dictionary<string, Microsoft.Bot.Builder.Dialogs.Dialog>();
    }

    public static class FormFlowExtensions
    {
        public static async Task Call<T>(this DialogContext context, object entities = null, object options = null, CancellationToken cancellationToken = default)
            where T : class
        {
            Microsoft.Bot.Builder.Dialogs.Dialog diag = context.Dialogs.Find(typeof(T).Name);
            if (diag == null)
                return;

            if(diag is FormDialog<T>)
            {
                FormDialog<T> formDialog = diag as FormDialog<T>;
                var field = formDialog.GetType().GetField("_entities", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    entities = ConvertToLuisModels(entities) ?? Enumerable.Empty<EntityRecommendation>();
                    field.SetValue(formDialog, entities);
                }
            }

            await context.BeginDialogAsync(typeof(T).Name, options, cancellationToken);
        }

        public static async Task Call<T>(this DialogContext context, FormDialog<T> form, object entities = null, object options = null, CancellationToken cancellationToken = default)
            where T : class
        {
            Microsoft.Bot.Builder.Dialogs.Dialog diag = context.Dialogs.Find(typeof(T).Name);
            if (diag != null)
                await context.Call<T>(entities, options, cancellationToken);
            else
            {
                context.Dialogs.Add(form);
                DialogFormCaching._dialogs.Add(context.Context.Activity.Conversation.Id, form);
                await context.Call<T>(entities, options, cancellationToken);
            }
        }

        public static async Task<IFormBuilder<T>> Field<T>(this IFormBuilder<T> builder, string name, Func<Task<string>> prompt, ActiveDelegate<T> active = null, ValidateAsyncDelegate<T> validate = null)
            where T : class
        {
            string prmpt = await prompt();
            return builder.Field(name, prmpt, active, validate);
        }

        public static async Task<IFormBuilder<T>> Field<T>(this Task<IFormBuilder<T>> builder, string name, Func<Task<string>> prompt, ActiveDelegate<T> active = null, ValidateAsyncDelegate<T> validate = null)
            where T : class
        {
            string prmpt = await prompt();
            var b = await builder;
            return b.Field(name, prmpt, active, validate);
        }

        #region < Private Methods >

        private class EntityTypeParameter
        {
            public int startIndex { get; set; }
            public int endIndex { get; set; }
            public string text { get; set; }
            public string type { get; set; }
        }

        private static IEnumerable<EntityRecommendation> ConvertToLuisModels(object result)
        {
            if (!(result is RecognizerResult))
                return Enumerable.Empty<EntityRecommendation>();

            var res = result as RecognizerResult;
            if(res.Properties.ContainsKey("luisResult") && res.Properties["luisResult"] is LuisResult)
            {
                var luisResult = res.Properties["luisResult"] as LuisResult;
                return luisResult.Entities.Select(e => new EntityRecommendation
                {
                    EndIndex = e.EndIndex,
                    StartIndex = e.StartIndex,
                    Role = e.Type,
                    Type = e.Type,
                    Entity = e.Entity
                }).ToList();
            }
            else if(res.Entities["$instance"] != null)
            {
                List<EntityRecommendation> list = new List<EntityRecommendation>();
                var instance = res.Entities["$instance"] as JObject;
                foreach(var token in instance)
                {
                    string type = token.Key;
                    var e = JsonConvert.DeserializeObject<EntityTypeParameter[]>(token.Value.ToString());
                    foreach(var d in e)
                    {
                        list.Add(new EntityRecommendation
                        {
                            EndIndex = d.endIndex,
                            StartIndex = d.startIndex,
                            Entity = d.text,
                            Role = type,
                            Type = type
                        });
                    }
                }
                return list;
            }
            else
                return Enumerable.Empty<EntityRecommendation>();
        }

        #endregion
    }
}
