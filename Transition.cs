using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DotJira
{
    public class Transition
    {
        [JsonProperty("expand")]
        public string Expand { get; set; }

        [JsonProperty("transitions")]
        public List<Transitions> Transitions { get; set; }
    }
}
