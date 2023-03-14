using Newtonsoft.Json;

namespace Znode.Engine.ERPConnector.Model.ChatGPTConnector
{
    public class CompletionsApiRequest
    {
        [JsonProperty("model")]
        public string Model { get; set; }
        [JsonProperty("prompt")]
        public string Prompt { get; set; }
        [JsonProperty("temperature")]
        public int Temparature { get; set; }
        [JsonProperty("max_tokens")]
        public int MaxTokens { get; set; }
        [JsonProperty("top_p")]
        public int TopP { get; set; }
        [JsonProperty("frequency_penalty")]
        public int FrequencyPenalty { get; set; }
        [JsonProperty("presence_penalty")]
        public int PresencePenalty { get; set; }
    }
}
