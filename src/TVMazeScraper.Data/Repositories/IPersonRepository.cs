using System.Collections.Generic;
using System.Threading.Tasks;
using TvMazeScraper.Data.Models;

namespace TVMazeScraper.Data.Repositories
{
    public interface IPersonRepository
    {
        Task Add(Person person);
        Task<IList<Person>> GetActors();
    }
}