using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TVMazeScraper.Models;

namespace TVMazeScraper
{
    public interface ITVMazeService
    {
        Task<List<Show>> GetShows(int pageNumber, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<Cast>> GetShowCast(int showId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
