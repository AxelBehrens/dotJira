using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DotJira
{
    public class Comments
    {
         [JsonIgnore]
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("body")]
        public string body { get; set; }
    }
}
