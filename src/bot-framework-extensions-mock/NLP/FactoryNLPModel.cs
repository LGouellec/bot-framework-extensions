using ai_chatbot_support_mock.NLP.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ai_chatbot_support_mock.NLP
{
    internal static class FactoryNLPModel
    {
        internal static NLPModel CreateNewModel()
        {
            bool defaultModel = false;
            DEFAULT_MODEL:

            string path = Path.Combine(GetApplicationRoot(), @"NLP/Model/model-nlp-mock.json");
            if (!defaultModel && File.Exists(path))
            {
                System.Console.WriteLine($@"{path} found");
                string data = File.ReadAllText(path);
                try
                {
                    return JsonConvert.DeserializeObject<NLPModel>(data);
                }
                catch(JsonException)
                {
                    defaultModel = true;
                    goto DEFAULT_MODEL;
                }
            }
            else
            {
                System.Console.WriteLine($@"{path} not found");
                NLPModel model = new NLPModel
                {
                    Name = "Model NLP Mock Luis Recognizer",
                    Intents = new Intent[]
                {
                    new Intent{Name = "CreateJira"},
                    new Intent{Name = "Test"},
                    new Intent{Name = "Information"}
                },
                    Entities = new Entity[]
                {
                    new Entity{Name = "Application"},
                    new Entity{Name = "Criticity", Values = new List<string>{"Blocker","Minor","Major"} },
                    new Entity{Name = "SiteCode", Values = new List<string>{"CAR", "BKR", "BBG", "CNO", "UMO", "UFR", "NKE","CTX","CPY","URG"} }
                },
                    Utterances = new Utterance[]
                {
                    new Utterance{
                        Intent = new Intent{Name = "CreateJira"},
                        Text = "i have a problem with my application ogu2",
                        Entities = new Entity[]
                        {
                            new Entity{Name= "Application", StartIndex=37, StopIndex = 41}
                        }
                    },
                    new Utterance{
                        Intent = new Intent{Name = "CreateJira"},
                        Text = "i have a problem with my application srvp",
                        Entities = new Entity[]
                        {
                            new Entity{Name= "Application", StartIndex=37, StopIndex = 41}
                        }
                    },
                    new Utterance{
                        Intent = new Intent{Name = "CreateJira"},
                        Text = "my application doesn't work",
                    },
                    new Utterance{
                        Intent = new Intent{Name = "CreateJira"},
                        Text = "srvp doesn't send message anymore",
                        Entities = new Entity[]
                        {
                            new Entity{Name= "Application", StartIndex=0, StopIndex = 3}
                        }
                    },
                    new Utterance
                    {
                        Intent = new Intent{Name="CreateJira"},
                        Text = "i have a problem in cno",
                        Entities = new Entity[]
                        {
                            new Entity{Name= "SiteCode", StartIndex=20, StopIndex = 23}
                        }
                    },
                    new Utterance
                    {
                        Intent = new Intent{Name="CreateJira"},
                        Text = "I have a minor problem with my girafon application is not working in cno",
                        Entities = new Entity[]
                        {   new Entity{Name= "Criticity", StartIndex=9, StopIndex = 13},
                            new Entity{Name= "Application", StartIndex=31, StopIndex = 37},
                            new Entity{Name= "SiteCode", StartIndex=69, StopIndex = 71}
                        }
                    },
                    new Utterance
                    {
                        Intent = new Intent{Name="CreateJira"},
                        Text = "there is a bug with ogu2",
                        Entities = new Entity[]
                        {
                            new Entity{Name= "Application", StartIndex=20, StopIndex = 23}
                        }
                    },
                    new Utterance
                    {
                        Intent = new Intent{Name="CreateJira"},
                        Text = "i need create a jira"
                    },
                    new Utterance
                    {
                        Intent = new Intent{Name="CreateJira"},
                        Text = "i have a problem with my girafon",
                        Entities = new Entity[]
                        {
                            new Entity{Name= "Application", StartIndex=25, StopIndex = 32}
                        }
                    },
                    new Utterance{
                        Intent = new Intent{Name = "Test"},
                        Text = "test"
                    },
                    new Utterance{
                        Intent = new Intent{Name = "Test"},
                        Text = "i want to do some test"
                    },
                    new Utterance{
                        Intent = new Intent{Name = "Information"},
                        Text = "search Information"
                    },
                    new Utterance
                    {
                        Intent = new Intent{Name = "CreateJira"},
                        Text = "I have a mojor Problem",
                        Entities = new Entity[]
                        {
                            new Entity{Name= "Criticity", StartIndex=9, StopIndex = 13}
                        }

                    }
                }
                };
                return model;
            }
        }

        private static string GetApplicationRoot()
        {
            var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            exePath = exePath.Replace(@"file:\", "").Replace(@"file:", "");
            return exePath;
        }
    }
}
