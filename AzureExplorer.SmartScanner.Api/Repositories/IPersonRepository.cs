using System.Threading.Tasks;
using AzureExplorer.SmartScanner.Models;

namespace AzureExplorer.SmartScanner.Repositories
{
    public interface IPersonRepository{
        Task<PersonDetails> CreatePerson(PersonDetails person);
        Task<PersonDetails> GetPersonByPersonId(string personId);
    }
}