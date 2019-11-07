using ai_chatbot_support_crosscutting;
using ai_chatbot_support_crosscutting.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ai_chatbot_support_mock
{
    public class MockMessageRepository : IMessageRepository
    {
        static List<dynamic> _datas = new List<dynamic>();

        public string RepositoryName => "mockMessageRepository";

        public async Task<IEnumerable<MessageConversation>> GetConversation(string conversationID)
        {
            return await Task.FromResult(Enumerable.Empty<MessageConversation>());
        }

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
