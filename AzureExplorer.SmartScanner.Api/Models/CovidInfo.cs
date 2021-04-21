using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AzureExplorer.SmartScanner.Models
{
    public class CovidInfo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [JsonProperty("adhaarNo")]
        public string AdhaarNo { get; set; }
        [JsonProperty("covidDetails")]
        public CovidDetails CovidDetails { get; set; }
    }
    public class CovidDetails
    {
        [JsonProperty("vaccinationInfoList")]
        public List<VaccinationInfo> VaccinationInfoList { get; set; }
        [JsonProperty("covidHistory")]
        public List<CovidHistory> CovidHistory { get; set; }
    }
    public class VaccinationInfo
    {
        [JsonProperty("hospitalName")]
        public string HospitalName { get; set; }
        [JsonProperty("dose")]
        public int Dose { get; set; }
        [JsonProperty("vaccinationDate")]
        public DateTime VaccinationDate { get; set; }
    }
    public class CovidHistory
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("date")]
        public DateTime Date { get; set; }
    }
}