using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DotJira
{
    public class Assignee
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        
        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }
    }
}
