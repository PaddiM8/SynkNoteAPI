using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynkNote.Models
{
    public class Notebook
    {
        [BsonId]
        [JsonProperty("id")]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [BsonElement("owner")]
        [JsonProperty("owner")]
        public ObjectId Owner { get; set; }
    }
}
