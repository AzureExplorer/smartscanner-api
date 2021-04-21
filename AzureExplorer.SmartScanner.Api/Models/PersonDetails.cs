using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace AzureExplorer.SmartScanner.Models
{
    public class PersonDetails
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [JsonProperty("image")]
        public byte[] Image { get; set; }
        [JsonProperty("personId")]
        public string PersonId { get; set; }
        [JsonProperty("fName")]
        public string FName { get; set;}
        [JsonProperty("lName")]
        public string LName { get; set; }
        [JsonProperty("adhaarNo")]
        public string AdhaarNo { get; set; }

    }
}