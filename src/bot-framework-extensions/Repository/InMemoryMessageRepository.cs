using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bot_framework_extensions.Repository
{
    public class InMemoryMessageRepository : IMessageRepository
    {
        static List<dynamic> _datas = new List<dynamic>();

        public string RepositoryName => "inMemory";

        public async Task SaveMessageAsync(string conversationID, string message)
        {
            dynamic model = new
            {
                ConversationID = conversationID,
                Message = message,
                Date = DateTime.Now
            };

            _datas.Add(model);
            await Task.CompletedTask;
        }
    }
}
