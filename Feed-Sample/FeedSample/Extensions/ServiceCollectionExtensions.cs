using Breffi.Story;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RightPerception.Story.SDK.Feed;
using System.Net.Http;

namespace FeedSample
{
    public static class ServiceCollectionExtensions
    {
        public static void AddFeedClient<T, O>(this IServiceCollection services)  
            where T : IStoryObject
            where O : FeedOptions
        {
            services.AddTransient(p =>
            {
                var options = p.GetService<IOptions<O>>().Value;
                var clientFactory = p.GetService<IHttpClientFactory>();
                var client = clientFactory.CreateClient(options.HttpClientName);
                return new FeedQueryBuilder<T>(client).WithPath(options.Feeds[typeof(T).Name]);
            });
        }
    }
}
