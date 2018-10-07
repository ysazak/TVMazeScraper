﻿using System;
using System.Collections.Generic;

namespace TvMazeScraper.Data.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? Birthday { get; set; }
        public virtual ICollection<ShowPerson> ShowPeople { get; set; }
    }
}
