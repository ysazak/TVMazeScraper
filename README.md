# TVMaze Scraper
Project scrapes all tv shows (incl. casts list) from http://www.tvmaze.com/api, persists in local SQL Server database, and provides WebAPI for the data.

Technologies: .Net Core 2.1 and Entity Framework Core

## Projects
  * TVMazeScraper: Class Library. contains scraper class which fetches data from TVMaze and persists the data in database. 
  * TvMazeScraper.API: ASP.NET WebAPI project. provides scraped data
  
    API Usage ->
    
          * Http GET : http://localhost:50479/api/shows?page=1&pagesize=100 (page and pagesize parameters are optional)
          * Swagger UI: http://localhost:50479/swagger/index.html
          
  * TVMazeScraper.Console: Console application for invoking TVMazeScraper library to scrape shows
  * TVMazeScraper.Data: Data access library which contains repository classes for db operations.
