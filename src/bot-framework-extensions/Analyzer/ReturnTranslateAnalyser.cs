using bot_framework_extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace bot_framework_extensions.Analyzer
{
    public class ReturnTranslateAnalyser : ITextAnalyzer
    {
        private readonly TranslateOptions _options = null;
        protected string toLanguage = "en-US";

        private class TranslateText
        {
            public string Text { get; set; }
        }

        public ReturnTranslateAnalyser(TranslateOptions options)
        {
            _options = options;
        }

        public async Task<string> Analyze(ContextAnalyzer ctx, string text)
        {
            string host = _options.endPoint;
            string route = "/translate?api-version=3.0&from=en&to=" + toLanguage;
            string subscriptionKey = _options.Key;

            System.Object[] body = new System.Object[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);

            var proxy = new WebProxy()
            {
                Address = new Uri("http://141.194.11.225:8000/"),

                UseDefaultCredentials = true,
                // *** These creds are given to the proxy server, not the web server ***
                Credentials = new NetworkCredential(
                userName: _options.user,
                password: _options.pwd)
            };
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.AllowAutoRedirect = false;
            using (var client = new HttpClient(httpClientHandler))
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
                //read response
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
