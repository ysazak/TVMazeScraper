using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TvMazeScraper.API.Models;
using TvMazeScraper.Data.Models;
using TVMazeScraper.Data.Repositories;

namespace TvMazeScraper.API.Controllers
{
    [Produces("application/json")]
    [Route("api/Shows")]
    public class ShowsController : Controller
    {
        private readonly IShowRepository showRepository;

        public ShowsController(IShowRepository showRepository)
        {
            this.showRepository = showRepository;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<IList<ShowDTO>>> GetShows([FromQuery]int page = 1, [FromQuery]int pageSize = 25)
        {
            if (page < 1) return BadRequest($"{nameof(page)} parameter value should be min 1. (Current value: {pageSize})");
            if (pageSize < 1 || pageSize > 1000) BadRequest($"{nameof(pageSize)} parameter value should be between 1 and 1000. (Current value: {pageSize})");

            var showList = await showRepository.GetShows(pageSize, page);
            var result = new List<ShowDTO>();
            foreach (var item in showList)
            {
                result.Add(new ShowDTO
                {
                    Id = item.Id,
                    Name = item.Name,
                    Cast = item.ShowPeople.OrderByDescending(sp => sp.Person?.Birthday).Select(p => new CastDTO
                    {
                        Id = p.PersonId,
                        Name = p.Person?.Name,
                        Birthday = p.Person?.Birthday
                    }).ToList()
                });
            }
            return result;
        }
    }
}