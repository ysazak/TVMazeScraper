using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TvMazeScraper.API.Models
{
    public class ShowDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<CastDTO> Cast { get; set; }
        public ShowDTO()
        {
            Cast = new List<CastDTO>();
        }
    }
}
