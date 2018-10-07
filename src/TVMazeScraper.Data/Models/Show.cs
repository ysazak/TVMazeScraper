using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TvMazeScraper.Data.Models
{
    public class Show
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<ShowPerson> ShowPeople { get; set; }
    }
}
