using Breffi.Story.Expert;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace FeedSample
{
    class Program
    {

        public const int DefaultConnectionLimit = 25;
        public static IConfigurationRoot Configuration;
        public static IServiceProvider ServiceProvider;

        static Program()
        {
            ServicePointManager.DefaultConnectionLimit = DefaultConnectionLimit;
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            Configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json", true)
                .Build();

            var services = ConfigureServices();
            ServiceProvider = services.BuildServiceProvider();
        }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddLogging(s =>
            {
                s.AddConsole();
                s.SetMinimumLevel(LogLevel.Debug);
            });

            services.AddOptions();
            services.Configure<FeedOptions>(Configuration.GetSection("Feed"));
            services.AddHttpClient("Feed", (p, c) =>
            {
                var options = p.GetService<IOptions<FeedOptions>>().Value;
                c.BaseAddress = new Uri(options.Endpoint);
            });
            //.ConfigurePrimaryHttpMessageHandler(p =>
            //{
            //    var options = p.GetService<IOptions<IdOptions>>().Value;
            //    return new StoryAuthHttpHandler
            //    {
            //        Endpoint = new Uri(options.AuthEndpoint),
            //        ClientId = options.ClientId,
            //        Secret = options.Secret,
            //        ClientCertificateOptions = ClientCertificateOption.Manual,
            //        ServerCertificateCustomValidationCallback =
            //            (httpRequestMessage, cert, cetChain, policyErrors) => true
            //    };
            //});

            services.AddFeedClient<BudgetEvent, FeedOptions>();
            services.AddFeedClient<QuestEvent, FeedOptions>();
            services.AddFeedClient<Licence, FeedOptions>();
            services.AddFeedClient<Employee, FeedOptions>();
            services.AddTransient(typeof(Synchronizer<>));
            return services;
        }


        static async Task Main(string[] args)
        {
            var synchronizer = ServiceProvider.GetService<Synchronizer<QuestEvent>>();
            await synchronizer.RunAsync();
        }
    }
}
