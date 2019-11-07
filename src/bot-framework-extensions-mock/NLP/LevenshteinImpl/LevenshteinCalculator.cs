using System;
using System.Collections.Generic;
using System.Text;

namespace ai_chatbot_support_mock.NLP.LevenshteinImpl
{
    public class LevenshteinCalculator : ILevenshteinCalculator
    {
        private ILevenshtein _calculator = null;

        #region ctor

        public LevenshteinCalculator() { }

        #endregion

        #region Default Impl

        public static ILevenshteinCalculator DefaultImplementation
        {
            get
            {
                LevenshteinCalculator instance = new LevenshteinCalculator();
                instance.UseBasicImplementation();
                return instance;
            }
        }

        #endregion

        #region Use

        public ILevenshteinCalculator UseBasicImplementation()
        {
            this._calculator = new Levenshtein();
            return this;
        }

        public ILevenshteinCalculator DetectKeyWord()
        {
            return this;
        }

        #endregion

        #region Implementation

        public LevenshteinResult GetResults(string source, string target)
        {
            int? result = _calculator?.Get(source, target);
            return new LevenshteinResult { Score = result.HasValue ? result.Value : 0 };
        }

        #endregion
    }
}
