using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvMazeScraper.Data.Models;

namespace TvMazeScraper.Data
{
    public class TVMazeScrapperDBContext : DbContext
    {
        public DbSet<Show> Shows { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<ShowPerson> ShowPerson { get; set; }


        public TVMazeScrapperDBContext(DbContextOptions<TVMazeScrapperDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShowPerson>().HasKey(sp => new { sp.ShowId, sp.PersonId });

            modelBuilder.Entity<Show>().HasKey("Id");
            modelBuilder.Entity<Show>().Property(s => s.Id).ValueGeneratedNever();
            modelBuilder.Entity<Show>().ToTable("Show");

            modelBuilder.Entity<Person>().HasKey("Id");
            modelBuilder.Entity<Person>().Property(c => c.Id).ValueGeneratedNever();
            modelBuilder.Entity<Person>().ToTable("Person");


        }
    }
}
