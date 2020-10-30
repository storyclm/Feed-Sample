using System.Collections.Generic;

namespace FeedSample
{
    public class FeedOptions
    {
        public virtual string Endpoint { get; set; }

        public virtual string HttpClientName { get; set; }

        public virtual IDictionary<string, string> Feeds { get; set; }
    }
}
