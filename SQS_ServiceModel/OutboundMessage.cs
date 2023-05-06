using System.Text.Json.Serialization;

namespace SQS_ServiceModel
{
    public class OutboundMessage : MessageBase
    {
        [JsonIgnore]
        public override string TypeOfMessage => nameof(OutboundMessage);

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("xmlData")]
        public string? XmlData { get; set; }
    }
}
