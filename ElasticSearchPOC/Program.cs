using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace ElasticSearchPOC
{
    class Program
    {
        static void Main(string[] args)
        {
            var svc = new TaggingService("securities");
            for (var i = 0; i < 50; i++)
            {
                Console.WriteLine($"Processing iteration #{i + 1}");
                var tags = GetTags(200_000);

//            IndexMany(svc, tags);
                BulkAll(svc, tags);
            }

            Console.WriteLine("Finished.");
            Console.Read();
        }

        private static void IndexMany(TaggingService svc, IEnumerable<Tag> tags)
        {
            Console.WriteLine("Starting IndexMany Insert");
            var sw = new Stopwatch();
            sw.Start();
//            var response = svc.IndexMany(tags);
            var response = svc.IndexMany(tags);
            sw.Stop();
            Console.WriteLine($"Done IndexMany Insert in {sw.Elapsed} seconds");

            if (response.Errors)
            {
                foreach (var itemWithError in response.ItemsWithErrors)
                {
                    Console.WriteLine("Failed to index document {0}: {1}", itemWithError.Id, itemWithError.Error);
                }
            }
        }

        private static void BulkAll(TaggingService svc, IEnumerable<Tag> tags)
        {
            Console.WriteLine("Starting BulkAll Insert");
            var sw = new Stopwatch();
            sw.Start();
            var response = svc.BulkAll(tags, allResponse =>
            {
                Console.WriteLine($"Processing page #{allResponse.Page}");
            });
            sw.Stop();
            Console.WriteLine($"Done BulkAll Insert in {sw.Elapsed} seconds");            
        }

        private static IEnumerable<Tag> GetTags(int count = 100_000)
        {
            var rnd = new Random();
            
            var tags = new List<Tag>();
            for (var i = 0; i < count; i++)
            {
                var next = rnd.Next(1, 1000);
                tags.Add(new Tag
                {
                    Id = Guid.NewGuid(),
                    Sid = next,
                    From = DateTime.Today.AddDays(-rnd.Next(1, 1000)),
                    To = DateTime.Today.AddDays(rnd.Next(1, 1000)),
                    TagName = $"Tag Name {next}",
                    TagValue = $"Tag Value {next}",
                });
            }

            return tags;
        }
    }
}
