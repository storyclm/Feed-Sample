using Breffi.Story;
using Microsoft.Extensions.Logging;
using RightPerception.Story.SDK.Feed;
using System;
using System.Threading.Tasks;

namespace FeedSample
{
    public class Synchronizer<T>
        where T : IStoryObject
    {
        internal readonly FeedQueryBuilder<T> _builder;
        internal readonly ILogger _logger;

        public Synchronizer(
            FeedQueryBuilder<T> builder, 
            ILogger<Synchronizer<T>> logger)
        {
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
            _logger = logger ?? throw new ArgumentException(nameof(logger));
        }

        public virtual async Task RunAsync()
        {
            while (true)
            {
                try
                {
                    await FeedAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
                finally
                {
                    await Task.Delay(10_000);
                }
            }
        }


        async Task FeedAsync()
        {
            using (var state = new ContinuationSync<long?>(typeof(T).Name))
            {
                var builder = _builder
                    .Forward()
                    .PageSize(10);

                if (state.Token.HasValue)
                    builder = builder.UseCursor(state.Token.Value);

                foreach (var page in builder.Build())
                {
                    await page.Items.ThrottleAsync(async item =>
                    {
                        await item.Save();
                    }, 10);

                    state.Token = page.Cursor;
                    _logger.LogInformation($"Cursor: {state.Token}");
                }
            }
        }
    }
}
