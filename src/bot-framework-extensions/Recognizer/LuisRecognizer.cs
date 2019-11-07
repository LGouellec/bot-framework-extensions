﻿using bot_framework_extensions.Analyzer;
using Microsoft.Bot.Builder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace bot_framework_extensions.Recognizer
{
    public class LuisRecognizer : ILuisRecognizerPipeline
    {
        protected readonly IList<ITextAnalyzer> _middlewares = new List<ITextAnalyzer>();
        protected Microsoft.Bot.Builder.IRecognizer _internalRecognizer = null;

        public LuisRecognizerOptions Options { get; internal set; } = new LuisRecognizerOptions();

        #region ILuisRecognizer Implementation

        public virtual void BuildLuisRecognizer(Action<LuisRecognizerOptions> action)
        {
            action(Options);
            var proxy = new WebProxy()
            {
                Address = new Uri("http://141.194.11.225:8000/"),

                UseDefaultCredentials = false,

                // *** These creds are given to the proxy server, not the web server ***
                //Credentials = new NetworkCredential(
                //userName: _options.user,
                //password: _options.pwd)
            };
            HttpClientHandler httpClientHandler = new HttpClientHandler() { Proxy = proxy };
            _internalRecognizer = new Microsoft.Bot.Builder.AI.Luis.LuisRecognizer(Options.LuisApplication, Options.LuisPrediction, Options.IncludeApiResults, httpClientHandler);
        }
        public async Task<RecognizerResult> RecognizeAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await RunMiddlewares(turnContext, cancellationToken);
            return await _internalRecognizer.RecognizeAsync(turnContext, cancellationToken);
        }

        public async Task<T> RecognizeAsync<T>(ITurnContext turnContext, CancellationToken cancellationToken) where T : IRecognizerConvert, new()
        {
            await RunMiddlewares(turnContext, cancellationToken);
            return await _internalRecognizer.RecognizeAsync<T>(turnContext, cancellationToken);
        }

        #endregion

        #region ILuisRecognizerPipeline Implementation

        public ILuisRecognizerPipeline Pipeline<T>(Func<T> func) where T : class, ITextAnalyzer 
            => Pipeline(func());

        public ILuisRecognizerPipeline Pipeline<T>(T analyzer) where T : class, ITextAnalyzer
        {
            if (_middlewares.OfType<T>().Any())
                return this;

            _middlewares.Add(analyzer);
            return this;
        }

        #endregion

        #region Run Middleware

        private async Task RunMiddlewares(ITurnContext context, CancellationToken cancellationToken)
        {
            ContextAnalyzer ctx = new ContextAnalyzer();
            var enumerator = _middlewares.GetEnumerator();
            while (enumerator.MoveNext())
            {
                context.Activity.Text = await enumerator.Current.Analyze(ctx, context.Activity.Text);
                if (ctx.LanguageDetected)
                {
                    ManagerConversationLanguage.Instance.LanguageDetected(context.Activity.Conversation.Id, ctx.Language);
                    ctx.LanguageDetected = false;
                    ctx.Language = string.Empty;
                }
            }
        }

        #endregion
    }
}