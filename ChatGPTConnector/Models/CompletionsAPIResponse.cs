using Newtonsoft.Json;
using System.Collections.Generic;

namespace Znode.Engine.ERPConnector.Model.ChatGPTConnector
{
    public class CompletionsAPIResponse
    {
        public class TextResponse
        {
            [JsonProperty("text")]
            public string Text { get; set; }
        }
        [JsonProperty("choices")]
        public List<TextResponse> Choices { get; set; }
    }
}
