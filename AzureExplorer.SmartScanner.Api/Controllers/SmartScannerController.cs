using AzureExplorer.SmartScanner.Models;
using AzureExplorer.SmartScanner.Repositories;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using System.Configuration;
using System.Web.Http.Cors;
using Microsoft.Azure.Devices.Client;
using System.Text;

namespace AzureExplorer.SmartScanner.Controllers
{
    [RoutePrefix("api/smartScanner")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SmartScannerController : ApiController
    {
        private IPersonRepository _personRepository;
        private DeviceClient s_deviceClient;
        private readonly TransportType s_transportType = TransportType.Mqtt;
        public SmartScannerController()
        {
            //_personRepository = personRepository;
            _personRepository = new PersonRepository(new MongoDBContext());
            s_deviceClient = DeviceClient.CreateFromConnectionString(ConfigurationManager.AppSettings["IOTDeviceConnectionString"], s_transportType);
        }
        [HttpGet]
        [Route("getString")]
        public string GetString()
        {
            return "Hello World from SmartScanner";
        }
        [HttpPost]
        [Route("savePerson")]
        public async Task<bool> SavePerson(PersonDetails personDetails)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["FaceApiBaseUrl"]);
            var content = new StringContent(JsonConvert.SerializeObject(personDetails), System.Text.Encoding.UTF8, "application/json");
            var result = await client.PostAsync("api/AzureFace/AddPerson", content);
            result.EnsureSuccessStatusCode();
            personDetails.PersonId = await result.Content.ReadAsStringAsync();
            await _personRepository.CreatePerson(personDetails);
            return true;
        }
        [HttpPost]
        [Route("getPersonDetails")]
        public async Task<PersonScanResult> GetPersonDetails(PersonDetails personDetails)
        {
            var responseData = new PersonScanResult();
            try
            {
                responseData.MessageId = personDetails.MessageId;
                var personId = await GetPersonId(personDetails);
                var personData = await _personRepository.GetPersonByPersonId(personId);
                var covidInfo = await GetCovidInfo(personData.AdhaarNo);
                responseData.PersonDetails = personData;
                responseData.CovidInfo = covidInfo;                
            }
            catch (Exception ex)
            {
                responseData.ExceptionMsg = ex;
            }
            return responseData;
        }
        [HttpPost]
        [Route("uploadImage")]
        public async Task<string> UploadImage(PersonDetails personDetails)
        {
            try
            {
                personDetails.MessageId = Guid.NewGuid().ToString();
                await SendDeviceToCloudMessagesAsync(personDetails);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
            return personDetails.MessageId;
        }
        private async Task SendDeviceToCloudMessagesAsync(PersonDetails personDetails)
        {
            // Create JSON message
            //personDetails.Image = null;
            string messageBody = JsonConvert.SerializeObject(personDetails);
            var message = new Message(Encoding.ASCII.GetBytes(messageBody))
            {
                ContentType = "application/json",
                ContentEncoding = "utf-8",
            };

            // Send the telemetry message
            await s_deviceClient.SendEventAsync(message);
        }
        private async Task<string> GetPersonId(PersonDetails personDetails)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["FaceApiBaseUrl"]);
            var content = new StringContent(JsonConvert.SerializeObject(personDetails), System.Text.Encoding.UTF8, "application/json");
            var result = await client.PostAsync("api/AzureFace/IdentifyPerson", content);
            result.EnsureSuccessStatusCode();
            var personId = await result.Content.ReadAsStringAsync();
            return personId;
        }
        private async Task<CovidInfo> GetCovidInfo(string adhaarNo)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["CovidApiBaseUrl"]);
            var result = await client.GetAsync("api/covidDetails/getCovidInfo?adhaarNo=" + adhaarNo);
            result.EnsureSuccessStatusCode();
            var covidResult = await result.Content.ReadAsStringAsync();
            var covidInfo = JsonConvert.DeserializeObject<CovidInfo>(covidResult);
            return covidInfo;
        }

    }
}
