using bot_framework_extensions.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace bot_framework_extensions.Repository
{
    internal class MongoDbMessageRepositoryOptions
    {
        public string ConnectionString { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string DataBase { get; set; }
        public string Collection { get; set; }

        public MongoDbMessageRepositoryOptions(MessageRepositoryOptions options)
        {
            if (options.value != null)
            {
                if (options.value.ContainsKey("connectionString"))
                    ConnectionString = options.value["connectionString"].ToString();
                if (options.value.ContainsKey("host"))
                    Host = options.value["host"].ToString();
                if (options.value.ContainsKey("port"))
                    Port = Convert.ToInt32(options.value["port"]);
                if (options.value.ContainsKey("database"))
                    DataBase = options.value["database"].ToString();
                if (options.value.ContainsKey("collection"))
                    Collection = options.value["collection"].ToString();
            }
        }
    }

    public class MongoDbMessageRepository : MessageRepository
    {
        private readonly MongoDbMessageRepositoryOptions configuration;

        public MongoDbMessageRepository(IOptions<MessageRepositoryOptions> options) 
            : base(options)
        {
            configuration = new MongoDbMessageRepositoryOptions(options.Value);
        }

        public override string RepositoryName => "mongoDB";

        public override async Task SaveMessageAsync(string conversationID, string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                var mongo = MakeClient();
               
                var db = mongo.GetDatabase(configuration.DataBase);

                if (!db.ListCollectionNames().ToList().Where(s => s.Equals(configuration.Collection)).Any())
                    db.CreateCollection(configuration.Collection);

                var history = db.GetCollection<object>(configuration.Collection);

                dynamic model = new
                {
                    ConversationID = conversationID,
                    Message = message,
                    Date = DateTime.Now
                };

                await history.InsertOneAsync(model);
            }
        }

        public override async Task<System.Collections.Generic.IEnumerable<MessageConversation>> GetConversation(string conversationID)
        {
            if (!string.IsNullOrEmpty(conversationID))
            {
                var mongo = MakeClient();

                var db = mongo.GetDatabase(configuration.DataBase);
                var history = db.GetCollection<MessageConversation>(configuration.Collection);

                var result = await history.Find(s => s.ConversationID.Equals(conversationID)).ToListAsync();
                return result;
            }
            return await Task.FromResult(Enumerable.Empty<MessageConversation>());
        }

        private MongoClient MakeClient()
        {
            if (!string.IsNullOrEmpty(configuration.ConnectionString))
                return new MongoClient(configuration.ConnectionString);
            else
                return new MongoClient(new MongoClientSettings()
                {
                    Server = new MongoServerAddress(configuration.Host, configuration.Port)
                });
            //return new MongoClient("mongodb://@127.0.0.1:27017/chatbot");
        }
    }
}
