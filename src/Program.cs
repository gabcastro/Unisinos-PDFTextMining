using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PDFTextMining.Runtime;
using PDFTextMining.Runtime.FileManager;
using Serilog;

namespace PDFTextMining
{
    class Program
    {
        public static IConfigurationRoot configuration;
        static void Main(string[] args)
        {
            // Initialize serilog logger
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .CreateLogger();

            try
            {
                MainAsync().Wait();
            }
            catch (Exception e)
            {
                Log.Fatal(e.Message);
            }
        }

        static async Task MainAsync()
        {
            // Create service collection
            Log.Information("Creating service collection");
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // Create service provider
            Log.Information("Building service provider");
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            try
            {
                Log.Information("Starting service");
                await serviceProvider.GetService<App>().Execute();
                Log.Information("Ending service");
            }
            catch (Exception ex)
            {
                Log.Fatal("Error running service ", ex.Message);
                throw ex;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Add logging
            serviceCollection.AddSingleton(LoggerFactory.Create(builder =>
            {
                builder
                    .AddSerilog(dispose: true);
            }));

            serviceCollection.AddLogging();

            // Build configuration
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();

            // Add access to generic IConfigurationRoot
            serviceCollection.AddSingleton<IConfigurationRoot>(configuration);

            // Add app
            serviceCollection.AddTransient<App>();
        }
    }
}
