using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TvMazeScraper.Data;
using TvMazeScraper.Data.Models;

namespace TVMazeScraper.Data.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly TVMazeScrapperDBContext dbContext;

        public PersonRepository(TVMazeScrapperDBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task Add(Person person)
        {
            await dbContext.People.AddAsync(person);
        }

        public async Task<IList<Person>> GetActors()
        {
            return await dbContext.People.AsNoTracking().ToListAsync();
        }
    }
}
