using bot_framework_extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace bot_framework_extensions.Analyzer
{
    public class TranslateAnalyzer : ITextAnalyzer
    {
        #region Inner class

        internal class DetectedLanguage
        {
            public string language { get; set; }
            public double score { get; set; }
        }

        internal class Translation
        {
            public string text { get; set; }
            public string to { get; set; }
        }

        internal class TranslatorModel
        {
            public DetectedLanguage detectedLanguage { get; set; }
            public List<Translation> translations { get; set; }
        }

        #endregion

        private readonly TranslateOptions _options = null;
        protected string toLanguage = "en-US";

        private class TranslateText
        {
            public string Text { get; set; }
        }

        public TranslateAnalyzer(TranslateOptions options)
        {
            _options = options;
        }

        internal void UseLuisModelLanguage(string luisModelLanguage) => toLanguage = luisModelLanguage;

        public async Task<string> Analyze(ContextAnalyzer ctx, string text)
        {
            string host = _options.endPoint;
            string route = "/translate?api-version=3.0&to=" + toLanguage;
            string subscriptionKey = _options.Key;

            Object[] body = new Object[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // Set the method to POST
                request.Method = HttpMethod.Post;
                // Construct the full URI
                request.RequestUri = new Uri(host + route);
                // Add the serialized JSON object to your request
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                // Add the authorization header
                request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                // Send request, get response

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

                var jsonResponse = await response.Content.ReadAsStringAsync();
                // Print the response
                var model = JsonConvert.DeserializeObject<TranslatorModel[]>(jsonResponse).FirstOrDefault();
                if (model.detectedLanguage != null)
                {
                    ctx.LanguageDetected = true;
                    ctx.Language = model.detectedLanguage.language;
                }

                return model.translations.FirstOrDefault().text;
            }

        }
    }
}
