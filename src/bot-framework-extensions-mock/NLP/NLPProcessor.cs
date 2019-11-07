using ai_chatbot_support_mock.NLP.LevenshteinImpl;
using ai_chatbot_support_mock.NLP.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ai_chatbot_support_mock.NLP
{
    internal class NLPProcessor
    {
        // MAYEBE : Auto learning

        private readonly ILevenshteinCalculator _levenshteinCalculator = LevenshteinCalculator.DefaultImplementation;
        private readonly Model.NLPModel _model = null;
        private readonly string[] _missedWords = new string[] { "i", "me", "my", "mine", "myself", "you", "you", "your", "yours", "yourself", "he", "him", "his", "himself", "she", "her", "hers", "herself", "it", "its", "itself", "we", "us", "our", "ours", "ourselves", "you", "your", "yours", "yourselves", "they", "them", "their", "theirs", "themselves", "or", "and", "each", "one", "another", "other", "from", "a", "an", "to" };
        private readonly string[] _verbs = new string[] { "is", "was", "were", "has", "been", "got", "do", "does" };
        private readonly string[] _differents = new string[] { ",", "?", ":", ".", "!", ";", "/" };

        public NLPProcessor(Model.NLPModel model)
        {
            _model = model;
        }

        public void Annotate(Annotation annotation)
        {
            annotation.KeyWords = GetKeywords(annotation.AlteredText).ToList();
        }

        public NLPResult Process(Annotation annotation)
        {
            List<Tuple<Intent, double>> intents = GetIntents(annotation.AlteredText, annotation.KeyWords.ToArray());
            var entities = GetEntities(intents.OrderByDescending(t => t.Item2).FirstOrDefault()?.Item1, annotation);

            return new NLPResult
            {
                Entities = entities.ToArray(),
                Intents = intents.ToDictionary(t => t.Item1.Name, t => t.Item2)
            };
        }

        #region Private

        private IEnumerable<string> GetKeywords(string text)
        {
            string[] words = text.Split(' ');
            return words.Where(s => !_missedWords.Contains(s) && !_verbs.Contains(s) && !_differents.Contains(s)).ToList();
        }

        private List<Tuple<Intent, double>> GetIntents(string text, string[] words)
        {
            List<Tuple<Intent, double>> list = new List<Tuple<Intent, double>>();
            
            foreach(var intent in _model.Intents)
            {
                var utterances = _model.Utterances.Where(u => u.Intent.Equals(intent));
                Utterance bestUtterance = null;
                int countMaxCommonWords = 0;

                foreach (var utt in utterances)
                {
                    var keyWordsUtterances = GetKeywords(utt.Text);
                    var listCommunWords = words.Join(keyWordsUtterances, o => o.ToLower(), i => i.ToLower(), (s, p) => p).ToList();
                    if (listCommunWords.Count > countMaxCommonWords)
                    {
                        bestUtterance = utt;
                        countMaxCommonWords = listCommunWords.Count;
                    }
                }

                if(bestUtterance == null)
                    list.Add(new Tuple<Intent, double>(intent, 0d));
                else
                {
                    var results = _levenshteinCalculator.GetResults(text, bestUtterance.Text);
                    double inverseResult = 100 - (results.Score / (text.Split(' ').Length * 1.0d));
                    inverseResult += countMaxCommonWords;
                    double score = (inverseResult * 1) / 100d;
                    score = score >= 1 ? 0.99 : score;
                    list.Add(new Tuple<Intent, double>(intent, score));
                }
            }
            return list;
        }

        private IEnumerable<NLPEntity> GetEntities(Intent intent, Annotation annotation)
        {
            List<NLPEntity> listEntity = new List<NLPEntity>();
            Dictionary<Entity, List<string>> dataEntity = new Dictionary<Entity, List<string>>();
            var utterances = _model.Utterances.Where(u => u.Intent.Equals(intent));
            var allValues = utterances.SelectMany(u => u.GetEntitiesValues());

            foreach(var k in allValues)
            {
                var join = annotation.KeyWords.Join(k.Value, s => s.ToLower(), o => o.ToLower(), (p, i) => i);

                if (join.Any())
                {
                    foreach (var s in join)
                    {
                        var startIndex = annotation.AlteredText.IndexOf(s.ToLower());
                        if(!listEntity.Any(n => n.Entity.Equals(s, StringComparison.InvariantCultureIgnoreCase) && n.Type == k.Key.Name && n.StartIndex == startIndex && n.EndIndex == startIndex + s.Length))
                            listEntity.Add(new NLPEntity()
                            {
                                Entity = s,
                                StartIndex = startIndex,
                                EndIndex = startIndex + s.Length,
                                Type = k.Key.Name
                            });
                    }
                }
                else
                {
                    var entity = _model.Entities.FirstOrDefault(e => e.Equals(k.Key));
                    var value = entity.Values.Select(s => s.ToLower()).Join(annotation.KeyWords, s => s.ToLower(), o => o.ToLower(), (i, p) => p);
                    foreach(var v in value)
                    {
                        var startIndex = annotation.AlteredText.IndexOf(v);
                        if (!listEntity.Any(n => n.Entity.Equals(v, StringComparison.InvariantCultureIgnoreCase) && n.Type == entity.Name && n.StartIndex == startIndex && n.EndIndex == startIndex + v.Length))
                            listEntity.Add(new NLPEntity()
                            {
                                Entity = v,
                                StartIndex = startIndex,
                                EndIndex = startIndex + v.Length,
                                Type = entity.Name
                            });
                    }
                }
            }
            
            return listEntity;
        }

        #endregion
    }
}
