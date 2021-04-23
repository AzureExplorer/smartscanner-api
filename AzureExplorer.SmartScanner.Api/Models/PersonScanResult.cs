using System;

namespace AzureExplorer.SmartScanner.Models
{
    public class PersonScanResult
    {
        public string MessageId { get; set; }
        public PersonDetails PersonDetails { get; set; }
        public CovidInfo CovidInfo { get; set; }
        public Exception ExceptionMsg { get; set; }
    }
}