using bot_framework_extensions.Converter;
using bot_framework_extensions.Dialog;
using bot_framework_extensions.Luis;
using bot_framework_extensions.Middleware;
using bot_framework_extensions.Options;
using bot_framework_extensions.Recognizer;
using bot_framework_extensions.Repository;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace bot_framework_extensions
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddChatBot<TBot>(this IServiceCollection services, IConfiguration configuration, Action<ChatBotFrameworkOptions> actionChatBotFrameworkOptions) where TBot : class, IBot
        {
            ChatBotFrameworkOptions options = new ChatBotFrameworkOptions();
            actionChatBotFrameworkOptions(options);

            services.AddOptions();
            services.Configure<SpellOptions>(configuration.GetSection("spell"));
            services.Configure<TranslateOptions>(configuration.GetSection("translate"));
            services.Configure<LuisOptions>(configuration.GetSection("luis"));
            services.Configure<MessageRepositoryOptions>(configuration.GetSection("messageRepository"));

            switch (options.TypeMessageRepository)
            {
                case CHABOT_MESSAGE_REPOSITORY.IN_MEMORY:
                    services.AddSingleton<IMessageRepository, InMemoryMessageRepository>()
                            .AddSingleton<SaveConversationMiddleware>();
                    break;
                case CHABOT_MESSAGE_REPOSITORY.API:
                    services.AddSingleton<IMessageRepository, RestClientMessageRepository>()
                        .AddSingleton<SaveConversationMiddleware>();
                    break;
                case CHABOT_MESSAGE_REPOSITORY.CUSTOM:
                    services.AddSingleton(options.CustomTypeMessageRepository())
                        .AddSingleton<SaveConversationMiddleware>();
                    break;
            }

            if(options.EnableDetectionUserLanguage)
                services.AddSingleton<ITranslateHandler, TranslateHandler>()
                        .AddSingleton<ITextConverter, TextConverter>();

            if(options.EnableLuisDetection)
            {
                services.AddSingleton<LuisMiddleware>();

                var serviceProviderBis = services.BuildServiceProvider();

                var spellOpt = serviceProviderBis.GetService<IOptions<SpellOptions>>()?.Value;
                var translateOpt = serviceProviderBis.GetRequiredService<IOptions<TranslateOptions>>()?.Value;
                var luisOpt = serviceProviderBis.GetRequiredService<IOptions<LuisOptions>>()?.Value;
                services.AddLuisRecognizer<LuisRecognizer>(opt =>
                {
                    opt.UseSpellService = options.UsingSpellBeforeLuis;
                    opt.UseTranslateService = options.UsingTranslateBeforeLuis;

                    opt.SpellOptions = spellOpt;
                    opt.TranslateOptions = translateOpt;
                    opt.LuisModelLanguage = options.LuisModelLanguage;

                    if (luisOpt != null)
                    {
                        opt.LuisApplication = new Microsoft.Bot.Builder.AI.Luis.LuisApplication(luisOpt.applicationId, luisOpt.endpointKey, luisOpt.endpoint);
                        opt.LuisPrediction = new Microsoft.Bot.Builder.AI.Luis.LuisPredictionOptions { IncludeAllIntents = true };
                    }
                });
            }

            var serviceProvider = services.BuildServiceProvider();

            services.AddTransient<IDialogFactory, DialogFactory>();

            return services.AddBot<TBot>(opt =>
            {
                var conversationState = new ConversationState(options.Storage);
                opt.State.Add(conversationState);

                //// Custom middleware
                if (options.EnableLuisDetection)
                    opt.Middleware.Add<LuisMiddleware>(services);

                if (options.TypeMessageRepository != CHABOT_MESSAGE_REPOSITORY.NONE)
                    opt.Middleware.Add<SaveConversationMiddleware>(services);

                opt.Middleware.Add(new AutoSaveStateMiddleware(opt.State.ToArray()));

                // // Catches any errors that occur during a conversation turn and logs them.
                opt.OnTurnError = async (context, exception) =>
                {
                    var logger = serviceProvider.GetService<ILogger<TBot>>();

                    logger.LogInformation($"Sorry, it looks like something went wrong. Message : {exception.Message}. StackTrace : {exception.StackTrace}");
                    await context.SendActivityAsync($"Sorry, it looks like something went wrong. Message : {exception.Message}. StackTrace : {exception.StackTrace}", "notTranslate");
                };
            });
        }
    }
}
