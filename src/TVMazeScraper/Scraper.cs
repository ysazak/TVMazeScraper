using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TvMazeScraper.Data;
using System.Linq;
using dataModels = TvMazeScraper.Data.Models;
using System.Collections.Concurrent;
using TVMazeScraper.Models;
using Microsoft.EntityFrameworkCore;
using TVMazeScraper.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace TVMazeScraper
{
    public class Scraper : IScraper
    {
        private readonly ITVMazeService tvMazeService;
        private readonly IShowRepository showRepository;
        private readonly IPersonRepository personRepository;
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger logger;

        private const int TVMazePageSizeCount = 250;

        public Scraper(ITVMazeService tvMazeService, IShowRepository showRepository, IPersonRepository personRepository, ILoggerFactory loggerFactory)
        {
            this.tvMazeService = tvMazeService;
            this.showRepository = showRepository;
            this.personRepository = personRepository;
            this.loggerFactory = loggerFactory;
            logger = loggerFactory.CreateLogger("Scraper");
        }

        public static int GetPageNumberofId(int showId)
        {
            return (int)Math.Floor((double)showId / (double)TVMazePageSizeCount);
        }

        public async Task Run()
        {
            var maxShowId = await showRepository.GetMaxShowId();

            int pageNumber = GetPageNumberofId(maxShowId);
            bool firstLoop = true;
            logger.LogInformation($"Scraper starting");

            while (true)
            {
                var newShows = await GetShowsFromService(pageNumber);

                if (newShows.Count == 0) break;

                // Import newest shows from the last imported page
                if (firstLoop)
                {
                    firstLoop = false;
                    newShows = newShows.Where(s => s.Id > maxShowId).ToList();
                    if (newShows.Count == 0)
                    {
                        pageNumber++;
                        continue;
                    }
                }

                var actorIds = newShows.SelectMany(s => s.Actors, (a, b) => b.Id).Distinct();
                var newActors = await FindNewPeople(newShows);

                foreach (var newActor in newActors)
                {
                    var p = new dataModels.Person { Id = newActor.Id, Name = newActor.Name, Birthday = newActor.Birthday };
                    await personRepository.Add(p);
                }

                foreach (var newShow in newShows)
                {
                    var show = new dataModels.Show { Id = newShow.Id, Name = newShow.Name, ShowPeople = new List<dataModels.ShowPerson>() };
                    foreach (var actor in newShow.Actors)
                    {
                        show.ShowPeople.Add(new dataModels.ShowPerson { ShowId = show.Id, PersonId = actor.Id });
                    }

                    await showRepository.Add(show);
                }
                logger.LogInformation("Saving the new shows to db");
                await showRepository.Save();
                pageNumber++;
            }
        }

        private async Task<List<Show>> GetShowsFromService(int page)
        {
            try
            {
                logger.LogInformation($"Fetching page {page}...");
                var shows = await tvMazeService.GetShows(page);
                logger.LogInformation($"{shows.Count} shows are fetched for page {page}");

                if (shows.Count == 0) return await Task.FromResult(shows);

                foreach (var show in shows)
                {
                    var casts = await tvMazeService.GetShowCast(show.Id);
                    logger.LogInformation($"{casts.Count} actors are fetched for show Id {show.Id}");
                    show.Actors.AddRange(casts.TakeWhile(c => c.Person != null)
                                                .Select(c => c.Person)
                                                .Distinct(new PersonEqualityComparer())
                                                .ToList());
                }

                return shows;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, null);
                throw;
            }
        }

        public async Task<IList<Person>> FindNewPeople(List<Show> shows)
        {
            var existingActors = (await personRepository.GetActors()).Select(p => p.Id);

            var newActors = shows.SelectMany(s => s.Actors)
                                    .Distinct(new PersonEqualityComparer())
                                    .Where(e => !existingActors.Contains(e.Id));

            return newActors.ToList();
        }
    }
}
