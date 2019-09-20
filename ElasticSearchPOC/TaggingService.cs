using System;
using System.Collections.Generic;
using Nest;

namespace ElasticSearchPOC
{
    public class TaggingService
    {
        private ElasticClient _client;

        public TaggingService(string indexName)
        {
            InitElasticClient(indexName);
        }

        private void InitElasticClient(string indexName)
        {
            var node = new Uri("http://localhost:9200");
            var connectionSettings = new ConnectionSettings(node)
                .DefaultMappingFor<Tag>(i => i
                    .IndexName(indexName)                    
                    .IdProperty(p => p.Id)
                )
                .EnableDebugMode()
                .PrettyJson()
                .RequestTimeout(TimeSpan.FromMinutes(2));

            _client = new ElasticClient(connectionSettings);
        }

        public BulkResponse IndexMany(IEnumerable<Tag> tags)
        {
            //            var response = await _client.IndexManyAsync(tags);
            var response = _client.IndexMany(tags);

            return response;
        }

        public BulkAllObserver BulkAll(IEnumerable<Tag> tags, Action<BulkAllResponse> onNext)
        {
            var response = _client.BulkAll(tags, b => b
//                    .Index("people")
                    .BackOffTime("30s")
                    .BackOffRetries(2)
                    .RefreshOnCompleted()
                    .MaxDegreeOfParallelism(Environment.ProcessorCount)
                    .Size(1000)
                )
                .Wait(TimeSpan.FromMinutes(15), onNext);

            return response;
        }        
    }
}
