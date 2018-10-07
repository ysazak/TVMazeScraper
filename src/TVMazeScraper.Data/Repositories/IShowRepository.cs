using System.Collections.Generic;
using System.Threading.Tasks;
using TvMazeScraper.Data.Models;

namespace TVMazeScraper.Data.Repositories
{
    public interface IShowRepository
    {
        Task<int> GetMaxShowId();
        Task Add(Show show);
        Task<ICollection<Show>> GetShows(int pageSize, int pageNumber);
        Task Save();
    }
}