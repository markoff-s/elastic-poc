using System;

namespace ElasticSearchPOC
{
    public class Tag
    {
        public Guid Id { get; set; }
        public long Sid { get; set; }
        public string TagName { get; set; }
        public string TagValue { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}