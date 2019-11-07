using bot_framework_extensions.Analyzer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bot_framework_extensions.Recognizer
{
    public static class LuisRecognizerExtensions
    {
        public static IServiceCollection AddLuisRecognizer<TLuis>(this IServiceCollection services, Action<LuisRecognizerOptions> configureAction = null) where TLuis : class, ILuisRecognizer, new()
        {
            return services.AddSingleton<ILuisRecognizer, TLuis>((isp) =>
            {
                TLuis luis = new TLuis();
                luis.BuildLuisRecognizer(configureAction);

               if (luis.Options.UseSpellService && luis is ILuisRecognizerPipeline)
                   (luis as ILuisRecognizerPipeline).UseSpellService();

               if (luis.Options.UseTranslateService && luis is ILuisRecognizerPipeline)
                   (luis as ILuisRecognizerPipeline).UseTranslateService();

                foreach (var middleware in luis.Options.Middlewares)
                    (luis as ILuisRecognizerPipeline).Pipeline(middleware);

               return luis;
           });
        }

        internal static ILuisRecognizerPipeline UseSpellService(this ILuisRecognizerPipeline luisRecognizer)
        {
            SpellAnalyzer spell = new SpellAnalyzer(luisRecognizer.Options.SpellOptions);
            return luisRecognizer.Pipeline(spell);
        }

        internal static ILuisRecognizerPipeline UseTranslateService(this ILuisRecognizerPipeline luisRecognizer)
        {
            TranslateAnalyzer translate = new TranslateAnalyzer(luisRecognizer.Options.TranslateOptions);
            return luisRecognizer.Pipeline(translate);
        }
    }
}
