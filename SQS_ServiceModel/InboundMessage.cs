using System.Text.Json.Serialization;

namespace SQS_ServiceModel
{
    public class InboundMessage : MessageBase
    {
        [JsonIgnore]
        public override string TypeOfMessage => nameof(InboundMessage);

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("xmlData")]
        public string? XmlData { get; set; }
        
    }
}
