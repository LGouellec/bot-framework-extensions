using bot_framework_extensions.Options;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace bot_framework_extensions.Repository
{
    internal class RestClientMessageRepositoryOptions
    {
        public string Url { get; set; }
        public string Api { get; set; }
        public string ApiKeyHeader { get; set; }
        public string ApiKeyValue { get; set; }

        public RestClientMessageRepositoryOptions(MessageRepositoryOptions options)
        {
            if (options.value != null)
            {
                if (options.value.ContainsKey("url"))
                    Url = options.value["url"].ToString();
                if (options.value.ContainsKey("api"))
                    Api = options.value["api"].ToString();
                if (options.value.ContainsKey("apiKeyHeader"))
                    ApiKeyHeader = options.value["apiKeyHeader"].ToString();
                if (options.value.ContainsKey("apiKeyValue"))
                    ApiKeyValue = options.value["apiKeyValue"].ToString();
            }
        }
    }

    public class RestClientMessageRepository : MessageRepository
    {
        private readonly RestClientMessageRepositoryOptions configuration;

        public RestClientMessageRepository(IOptions<MessageRepositoryOptions> options) 
            : base(options)
        {
            configuration = new RestClientMessageRepositoryOptions(options.Value);
        }

        public override string RepositoryName => "rest";

        public override async Task SaveMessageAsync(string conversationID, string message)
        {
            var client = new RestClient(configuration.Url);
            var request = new RestRequest(configuration.Api, Method.POST);
            if (!string.IsNullOrEmpty(configuration.ApiKeyHeader) && !string.IsNullOrEmpty(configuration.ApiKeyValue))
                request.AddHeader(configuration.ApiKeyHeader, configuration.ApiKeyValue);

            dynamic model = new
            {
                ConversationID = conversationID,
                Message = message,
                Date = DateTime.Now
            };

            request.AddJsonBody(model);
            var response = await client.ExecutePostTaskAsync(request);
        }
    }
}
