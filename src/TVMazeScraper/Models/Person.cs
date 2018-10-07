using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace TVMazeScraper.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime? Birthday { get; set; }
    }

    internal class PersonEqualityComparer : IEqualityComparer<Person>
    {
        public bool Equals(Person x, Person y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(Person obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
