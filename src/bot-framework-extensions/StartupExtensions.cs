using Microsoft.Bot.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace bot_framework_extensions
{
    public static class StartupExtensions
    {
        // TODO : Same with MOCK
        public static IServiceCollection AddChatBot<TBot>(this IServiceCollection services, Action<ChatBotFrameworkOptions> actionChatBotFrameworkOptions) where TBot : class, IBot
        {
            // Translate Handler + Message Repository
            //public static IServiceCollection AddMessageRepository(this IServiceCollection services)
            //{
            //    var serviceProvider = services.BuildServiceProvider();
            //    var messageRepository = serviceProvider.GetServices<IMessageRepository>();
            //    var opt = serviceProvider.GetService<IOptions<MessageRepositoryOptions>>()?.Value;
            //    IMessageRepository repository = null;
            //    foreach (var msgRepo in messageRepository)
            //        if (msgRepo.RepositoryName.Equals(opt.repository))
            //        {
            //            repository = msgRepo;
            //            break;
            //        }

            //    var wrappedDescriptors = services.Where(s => s.ServiceType == typeof(IMessageRepository)).ToList();
            //    foreach (var descriptor in wrappedDescriptors)
            //        services.Remove(descriptor);

            //    services.AddSingleton(repository);
            //    return services;
            //}

            //public static IServiceCollection AddTranslateHandler(this IServiceCollection services)
            //{
            //    services.AddSingleton<ITranslateHandler, TranslateHandler>();
            //    return services;
            //}

            // CHATBOT middleware
            // services.AddSingleton<LuisMiddleware>()
            //                .AddSingleton<Middlewares.SaveConversationMiddleware>();

            // LUIS PART
            //var serviceProvider = services.BuildServiceProvider();

            //var spellOpt = serviceProvider.GetService<IOptions<SpellOptions>>()?.Value;
            //var translateOpt = serviceProvider.GetRequiredService<IOptions<TranslateOptions>>()?.Value;
            //var luisOpt = serviceProvider.GetRequiredService<IOptions<LuisOptions>>()?.Value;
            //services.AddLuisRecognizer<LuisRecognizer>(opt => {
            //    opt.UseSpellService = true;
            //    opt.UseTranslateService = true;

            //    opt.SpellOptions = spellOpt;
            //    opt.TranslateOptions = translateOpt;

            //    if (luisOpt != null)
            //    {
            //        opt.LuisApplication = new Microsoft.Bot.Builder.AI.Luis.LuisApplication(luisOpt.applicationId, luisOpt.endpointKey, luisOpt.endpoint);
            //        opt.LuisPrediction = new Microsoft.Bot.Builder.AI.Luis.LuisPredictionOptions { IncludeAllIntents = true };
            //    }
            //})
            //    .AddSingleton<ITextConverter, TextConverter>();


            // Chatbot Normally
            //var serviceProvider = services.BuildServiceProvider();
            //services.AddTransient<IDialogFactory, DialogFactory>();

            //return services.AddBot<AIChatBot>(opt => {
            //    var conversationState = new ConversationState(new MemoryStorage());
            //    opt.State.Add(conversationState);

            //    var secretKey = configuration.GetSection("botFileSecret")?.Value;

            //    // Loads .bot configuration file and adds a singleton that your Bot can access through dependency injection.
            //    if (File.Exists(@"./N2SupportChatabot.bot"))
            //    {
            //        var botConfig = BotConfiguration.Load(@".\AIChatBot.bot", secretKey);
            //        services.AddSingleton(sp => botConfig);

            //        // Retrieve current endpoint.
            //        var service = botConfig.Services.Where(s => s.Type == "endpoint" && s.Name == "development").FirstOrDefault();
            //        if (!(service is EndpointService endpointService))
            //        {
            //            throw new InvalidOperationException($"The .bot file does not contain a development endpoint.");
            //        }
            //        opt.CredentialProvider = new SimpleCredentialProvider(endpointService.AppId, endpointService.AppPassword);
            //    }
            //    // Custom middleware
            //    opt.Middleware.Add<LuisMiddleware>(services);
            //    opt.Middleware.Add<Middlewares.SaveConversationMiddleware>(services);
            //    opt.Middleware.Add(new AutoSaveStateMiddleware(opt.State.ToArray()));
            //    // Catches any errors that occur during a conversation turn and logs them.
            //    opt.OnTurnError = async (context, exception) =>
            //    {
            //        var logger = serviceProvider.GetService<ILogger<AIChatBot>>();
            //        context.LogConnectorClient(logger);
            //        logger.LogInformation($"Sorry, it looks like something went wrong. Message : {exception.Message}. StackTrace : {exception.StackTrace}");
            //        await context.SendActivityAsync($"Sorry, it looks like something went wrong. Message : {exception.Message}. StackTrace : {exception.StackTrace}", "notTranslate");
            //    };
            //});

            return services;
        }
    }
}
