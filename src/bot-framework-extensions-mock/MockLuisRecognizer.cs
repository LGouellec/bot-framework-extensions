using ai_chatbot_support_mock.NLP;
using ai_chatbot_support_mock.NLP.Model;
using bot_framework_extensions.Recognizer;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ai_chatbot_support_mock
{
    internal class MockRecognizer : IRecognizer
    {
        private readonly NLPModel _model = FactoryNLPModel.CreateNewModel();

        public async Task<RecognizerResult> RecognizeAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                NLPProcessor processor = new NLPProcessor(_model);
                var annotation = new Annotation(turnContext.Activity.Text);
                processor.Annotate(annotation);
                var result = processor.Process(annotation);

                return new RecognizerResult
                {
                    Text = turnContext.Activity.Text,
                    Intents = result.Intents.ToDictionary(kp => kp.Key, kp => new IntentScore { Score = kp.Value}),
                    Properties = new Dictionary<string, object>
                    {
                        {
                            "luisResult", new LuisResult{
                                Intents = result.Intents.Select(kp => new IntentModel{Intent = kp.Key, Score = kp.Value}).ToList(),
                                Query = turnContext.Activity.Text,
                                AlteredQuery = annotation.AlteredText,
                                Entities = result.Entities.Select(p => new EntityModel{EndIndex = p.EndIndex, StartIndex = p.StartIndex, Type = p.Type, Entity = p.Entity } ).ToList(),
                                TopScoringIntent = new IntentModel{Intent = result.GetTopIntent().Item1, Score = result.GetTopIntent().Item2 }
                            }
                        }
                    }
                    // ,Entities = new JObject(JArray.FromObject(result.Entities))
                    // TODO SAMPLE : {  "$instance": {    "SiteCode": [      {        "startIndex": 41,        "endIndex": 44,        "text": "car",        "type": "SiteCode"      }    ],    "Application": [      {        "startIndex": 3,        "endIndex": 10,        "text": "girafon",        "type": "Application",        "score": 0.9267187      }    ]  },  "SiteCode": [    [      "CAR"    ]  ],  "Application": [    "girafon"  ]}
                };
            });
        }

        public Task<T> RecognizeAsync<T>(ITurnContext turnContext, CancellationToken cancellationToken) where T : IRecognizerConvert, new()
        {
            throw new NotImplementedException();
        }
    }

    public class MockLuisRecognizer : LuisRecognizer
    {
        public MockLuisRecognizer()
        {
        }

        public override void BuildLuisRecognizer(Action<LuisRecognizerOptions> action)
        {
            action(this.Options);
            _internalRecognizer = new MockRecognizer();
        }
    }
}
