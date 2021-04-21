using System.Threading.Tasks;
using AzureExplorer.SmartScanner.Models;
using MongoDB.Driver;
namespace AzureExplorer.SmartScanner.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private IMongoCollection<PersonDetails> _personCollection;
        public PersonRepository(MongoDBContext mongoDBContext)
        {
            _personCollection = mongoDBContext.PersonList;
        }
        public async Task<PersonDetails> CreatePerson(PersonDetails person)
        {
            person.Image = null;
            await _personCollection.InsertOneAsync(person);
            return person;
        }
        public async Task<PersonDetails> GetPersonByPersonId(string personId)
        {
            FilterDefinition<PersonDetails> filter;
            filter = Builders<PersonDetails>.Filter.Eq(co => co.PersonId, personId);
            return await _personCollection.Find(filter).FirstOrDefaultAsync();
        }

    }
}