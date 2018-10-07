using System.Collections.Generic;
using System.Threading.Tasks;
using TVMazeScraper.Models;

namespace TVMazeScraper
{
    public interface IScraper
    {
        Task Run();
        Task<IList<Person>> FindNewPeople(List<Show> shows);

    }
}
