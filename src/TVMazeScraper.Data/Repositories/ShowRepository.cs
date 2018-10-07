using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvMazeScraper.Data;
using TvMazeScraper.Data.Models;

namespace TVMazeScraper.Data.Repositories
{
    public class ShowRepository : IShowRepository
    {
        private readonly TVMazeScrapperDBContext dbContext;
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger logger;

        public ShowRepository(TVMazeScrapperDBContext dbContext, ILoggerFactory loggerFactory)
        {
            this.dbContext = dbContext;
            this.loggerFactory = loggerFactory;
        }

        public async Task Add(Show show)
        {
            await dbContext.Shows.AddAsync(show);
            await Save();
        }        
        public async Task<int> GetMaxShowId()
        {
            return await dbContext.Shows.DefaultIfEmpty().MaxAsync(s => s.Id);
        }

        public async Task<ICollection<Show>> GetShows(int pageSize, int pageNumber)
        {
            var skip = (pageNumber - 1) * pageSize;
            return await dbContext.Shows
                .OrderBy(s => s.Id)
                .Skip(skip)
                .Take(pageSize)
                .Include(s => s.ShowPeople)
                .ThenInclude(sp => sp.Person)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task Save()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
