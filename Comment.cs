using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DotJira
{
   public class Comment
    {
        [JsonProperty("startAt")]
        public int StartAt { get; set; }

        [JsonProperty("maxResults")]
        public int MaxResults { get; set; }

        [JsonProperty("total")]
        public int total { get; set; }

        [JsonProperty("comments")]
        public List<Comments> Comments { get; set; }

       
    }
}
