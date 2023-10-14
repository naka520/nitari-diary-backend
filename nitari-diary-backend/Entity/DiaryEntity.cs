
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using System.Text.Json.Serialization;

namespace nitari_diary_backend.Entity
{
    public class DiaryEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Date { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string EnglishV { get; set; }
        public string ImageUrl { get; set; }
        public string Type { get; set; }
        public string CreatedAt { get; set; }
        [JsonIgnore]
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}
