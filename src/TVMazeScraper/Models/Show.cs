using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TVMazeScraper.Models
{
    public class Show
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public List<Person> Actors { get; set; }

        public Show()
        {
            Actors = new List<Person>();
        }
    }
}
