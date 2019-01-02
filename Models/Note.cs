using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SynkNote.Models
{
    public class Note
    {
        [BsonId]
        [JsonProperty("id")]
        public ObjectId Id { get; set; }

        [BsonElement("Owner")]
        [JsonProperty("owner")]
        public ObjectId Owner { get; set; }

        [BsonElement("lastEdited")]
        [JsonProperty("lastEdited")]
        public DateTime LastEdited { get; set; }

        [BsonElement("notebookId")]
        [JsonProperty("notebookId")]
        public ObjectId NotebookId { get; set; }

        [BsonElement("location")]
        [JsonProperty("location")]
        public string Location { get; set; }

        [BsonElement("content")]
        [JsonProperty("content")]
        public string Content { get; set; }

        [BsonElement("diffs")]
        [JsonProperty("content")]
        public string[] Diffs { get; set; }
    }
}
