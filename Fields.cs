using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DotJira
{
    public class Fields
    {
        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("status")]
        public Status Status { get; set; }

        [JsonProperty("assignee")]
        public Assignee Assignee { get; set; }

        [JsonProperty("customfield_11000")]
        public string FileName { get; set; }

        [JsonProperty("customfield_10502")]
        public string BUID { get; set; }

        [JsonProperty("customfield_10501")]
        public string WCID { get; set; }

        [JsonProperty("comment")]
        public Comment Comment { get; set; }
    }
}
