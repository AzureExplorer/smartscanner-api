using AzureExplorer.SmartScanner.Models;
using AzureExplorer.SmartScanner.Repositories;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using System.Configuration;
using System.Web.Http.Cors;

namespace AzureExplorer.SmartScanner.Controllers
{
    [RoutePrefix("api/smartScanner")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SmartScannerController : ApiController
    {
        private IPersonRepository _personRepository;
        public SmartScannerController()
        {
            //_personRepository = personRepository;
            _personRepository = new PersonRepository(new MongoDBContext());
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
                var personId = await GetPersonId(personDetails);
                var personData = await _personRepository.GetPersonByPersonId(personId);
                var covidInfo = await GetCovidInfo(personData.AdhaarNo);
                responseData=new PersonScanResult
                {
                    PersonDetails = personData,
                    CovidInfo = covidInfo
                };
            }
            catch(Exception ex)
            {
                responseData.ExceptionMsg = ex;
            }
            return responseData;
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
