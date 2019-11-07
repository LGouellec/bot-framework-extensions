using bot_framework_extensions.Converter;
using bot_framework_extensions.Repository;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace bot_framework_extensions.Bot
{
    public class TurnContextAdapter : ITurnContext
    {
        private readonly ILogger _logger;
        private readonly ITurnContext _adapter;
        private IMessageRepository _messageRepository;
        private ITranslateHandler _translateHandler;

        public TurnContextAdapter(ITurnContext context, ILogger logger)
        {
            _logger = logger;
            _adapter = context;
            _adapter.OnSendActivities(SendActivity);
        }

        #region Adapter Implementation

        public BotAdapter Adapter => _adapter.Adapter;

        public TurnContextStateCollection TurnState => _adapter.TurnState;

        public Activity Activity => _adapter.Activity;

        public bool Responded => _adapter.Responded;

        public async Task DeleteActivityAsync(string activityId, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _adapter.DeleteActivityAsync(activityId, cancellationToken);
        }

        public async Task DeleteActivityAsync(ConversationReference conversationReference, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _adapter.DeleteActivityAsync(conversationReference, cancellationToken);
        }

        public ITurnContext OnDeleteActivity(DeleteActivityHandler handler)
        {
            return _adapter.OnDeleteActivity(handler);
        }

        public ITurnContext OnSendActivities(SendActivitiesHandler handler)
        {
            return _adapter.OnSendActivities(handler);
        }

        public ITurnContext OnUpdateActivity(UpdateActivityHandler handler)
        {
            return _adapter.OnUpdateActivity(handler);
        }

        public async Task<ResourceResponse[]> SendActivitiesAsync(IActivity[] activities, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _adapter.SendActivitiesAsync(activities, cancellationToken);
        }

        public async Task<ResourceResponse> SendActivityAsync(string textReplyToSend, string speak = null, string inputHint = "acceptingInput", CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _adapter.SendActivityAsync(textReplyToSend, speak, inputHint, cancellationToken);
        }

        public async Task<ResourceResponse> SendActivityAsync(IActivity activity, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _adapter.SendActivityAsync(activity, cancellationToken);
        }

        public async Task<ResourceResponse> UpdateActivityAsync(IActivity activity, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _adapter.UpdateActivityAsync(activity, cancellationToken);
        }

        #endregion

        #region Middlewares 

        public TurnContextAdapter UseMessageRepository(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
            return this;
        }

        public TurnContextAdapter UseTranslateHandler(ITranslateHandler handler)
        {
            _translateHandler = handler;
            return this;
        }

        private async Task SaveActivity(Activity activity)
        {
            if(_messageRepository != null)
                await _messageRepository.SaveMessageAsync(activity.Conversation.Id, activity.Text);
        }

        private async Task TranslateActivity(Activity activity)
        {
            if (_translateHandler != null && (string.IsNullOrEmpty(activity.Speak) || !activity.Speak.Equals("notTranslate")))
                activity.Text = await _translateHandler.Translate(activity.Conversation.Id, activity.Text);
        }

        #endregion

        #region Send Activities

        private async Task<ResourceResponse[]> SendActivity(ITurnContext turnContext, List<Activity> activities, Func<Task<ResourceResponse[]>> next)
        {
            foreach(var a in activities)
            {
                await TranslateActivity(a);
                await SaveActivity(a);
            }

            var responses = await _adapter.Adapter.SendActivitiesAsync(this, activities.ToArray(), default);
            var sentNonTraceActivity = false;

            for (var index = 0; index < responses.Length; index++)
            {
                var activity = activities[index];

                activity.Id = responses[index].Id;

                sentNonTraceActivity |= activity.Type != ActivityTypes.Trace;
            }

            return responses;
        }

        #endregion
    }
}
