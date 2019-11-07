using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace bot_framework_extensions.State
{
    public class MessageConversation
    {
        public ObjectId Id { get; set; }
        public string ConversationID { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
    }
}
