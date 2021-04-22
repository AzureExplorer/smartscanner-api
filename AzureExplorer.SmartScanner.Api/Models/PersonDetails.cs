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
        [JsonProperty("associateId")]
        public string AssociateId { get; set; }
        [JsonProperty("designation")]
        public string Designation { get; set; }
        [JsonProperty("officeLocation")]
        public string OfficeLocation { get; set; }
        [JsonProperty("fName")]
        public string FName { get; set;}
        [JsonProperty("lName")]
        public string LName { get; set; }
        [JsonProperty("adhaarNo")]
        public string AdhaarNo { get; set; }
        [JsonProperty("temperature")]
        public Temperature Temperature { get; set; }

    }
    public class Temperature
    {
        [JsonProperty("maxTemp")]
        public decimal MaxTemp { get; set; }
        [JsonProperty("currentTemp")]
        public decimal CurrentTemp { get; set; }
    }
}