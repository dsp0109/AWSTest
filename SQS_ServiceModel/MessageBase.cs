using Amazon.SQS.Model;
using System.Text.Json.Serialization;

namespace SQS_ServiceModel
{
    public abstract class MessageBase
    {
        /// <summary>
        /// The type of message to convert into
        /// </summary>
        [JsonIgnore]
        public virtual string TypeOfMessage => nameof(NotImplementedException);

        [JsonIgnore]
        public virtual IDictionary<string, MessageAttributeValue>? AdditionalMessageAttributes { get; set; }

    }
}