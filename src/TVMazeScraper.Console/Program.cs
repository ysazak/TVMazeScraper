using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using TvMazeScraper.Data;
using TVMazeScraper.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace TVMazeScraper.Console
{
    internal class Program
    {
        private static ITVMazeService tvMazeService;
        private static TVMazeHttpClient tvMazeHttpClient;
        private static IScraper scrapper;
        private static TVMazeScrapperDBContext dbContext;
        private static IShowRepository showRepository;

        static async Task Main(string[] args)
        {
            System.Console.WriteLine("Program is starting up...");

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            dbContext.Database.EnsureCreated();

            await scrapper.Run();

            System.Console.WriteLine("Scraping is completed.");
            System.Console.ReadLine();
        }

        static void ConfigureServices(ServiceCollection serviceCollection)
        {
            var configBuilder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json");
            var configuration = configBuilder.Build();
            var connectionStr = configuration.GetConnectionString("DefaultConnection"); 

            serviceCollection.AddDbContext<TVMazeScrapperDBContext>(options => options.UseSqlServer(connectionStr))
                .AddTransient<IShowRepository, ShowRepository>()
                .AddTransient<IPersonRepository, PersonRepository>()
                .AddTransient<IScraper, Scraper>()
                .AddTransient<ITVMazeService, TVMazeService>()
                .AddTransient<ITVMazeService, TVMazeService>()
                .AddSingleton(new LoggerFactory()
                                .AddConsole(configuration.GetSection("Logging"))
                                .AddDebug())
                .AddLogging();

            serviceCollection.AddHttpClient<TVMazeHttpClient>(client =>
            {
                client.BaseAddress = new Uri("https://api.tvmaze.com/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "TVMazeScraper");
            })
           .SetHandlerLifetime(TimeSpan.FromMinutes(5))
           .AddPolicyHandler(TVMazeHttpClient.GetRetryPolicy());
            

            var services = serviceCollection.BuildServiceProvider();

            tvMazeService = services.GetService<ITVMazeService>();
            tvMazeHttpClient = services.GetService<TVMazeHttpClient>();
            scrapper = services.GetService<IScraper>();
            dbContext = services.GetService<TVMazeScrapperDBContext>();
            showRepository = services.GetService<IShowRepository>();
        }
    }
}
