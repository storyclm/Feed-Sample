using Breffi.Story;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FeedSample
{
    public static class Extensions
    {
        public static async Task Save(this IStoryObject o)
        {
            if (o == null) return;
            var path = Path.Combine("Data", $"{o.GetType().Name}Feed");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

           await File.WriteAllTextAsync(Path.Combine(path, $"{o.Id}.json"), JsonConvert.SerializeObject(o, Formatting.Indented), Encoding.UTF8);
        }
    }
}
