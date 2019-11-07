using bot_framework_extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace bot_framework_extensions.Analyzer
{
    public class SpellAnalyzer : ITextAnalyzer
    {
        #region Inner class

        internal class BingSpellCheckResponse
        {
            public BingSpellCheckFlaggedToken[] FlaggedTokens { get; set; }
        }
        internal class BingSpellCheckSuggestion
        {
            public string Suggestion { get; set; }

            public double Score { get; set; }
        }
        internal class BingSpellCheckFlaggedToken
        {
            public int Offset { get; set; }

            public string Token { get; set; }

            public string Type { get; set; }

            public BingSpellCheckSuggestion[] Suggestions { get; set; }
        }

        #endregion

        public readonly SpellOptions _options = null;

        public SpellAnalyzer(SpellOptions options)
        {
            _options = options;
        }
        public async Task<string> Analyze(ContextAnalyzer ctx, string text)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _options.key);
                string uri = _options.host + _options.path + _options.@params;
                List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("text", text));
                using (FormUrlEncodedContent content = new FormUrlEncodedContent(values))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                    using (HttpResponseMessage response = await client.PostAsync(uri, content))
                    {
                        string contentString = await response.Content.ReadAsStringAsync();
                        var spellCheckResponse = JsonConvert.DeserializeObject<BingSpellCheckResponse>(contentString);

                        StringBuilder sb = new StringBuilder();
                        int previousOffset = 0;

                        foreach (var flaggedToken in spellCheckResponse.FlaggedTokens)
                        {
                            // Append the text from the previous offset to the current misspelled word offset
                            sb.Append(text.Substring(previousOffset, flaggedToken.Offset - previousOffset));

                            // Append the corrected word instead of the misspelled word
                            sb.Append(flaggedToken.Suggestions.First().Suggestion);

                            // Increment the offset by the length of the misspelled word
                            previousOffset = flaggedToken.Offset + flaggedToken.Token.Length;
                        }

                        // Append the text after the last misspelled word.
                        if (previousOffset < text.Length)
                        {
                            sb.Append(text.Substring(previousOffset));
                        }

                        return sb.ToString();
                    }
                }
            }
        }
    }
}
