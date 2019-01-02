using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynkNote.Models
{
    public class NoteSkeleton
    {
        [BsonId]
        [JsonProperty("id")]
        public ObjectId Id { get; set; }

        [BsonElement("lastEdited")]
        [JsonProperty("lastEdited")]
        public DateTime LastEdited { get; set; }

        [BsonElement("location")]
        [JsonProperty("location")]
        public string Location { get; set; }

        public NoteSkeleton(ObjectId id, DateTime lastEdited, string location)
        {
            this.Id = id;
            this.LastEdited = lastEdited;
            this.Location = location;
        }
    }
}
