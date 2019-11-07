using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bot_framework_extensions.Options
{
    public class MessageRepositoryOptions : ICloneable
    {
        public string repository { get; set; }
        public IDictionary<string, object> value { get; set; }

        public virtual object Clone()
        {
            return new MessageRepositoryOptions()
            {
                repository = this.repository,
                value = this.value.ToDictionary(kp => kp.Key, e => e.Value)
            };
        }
    }
}
