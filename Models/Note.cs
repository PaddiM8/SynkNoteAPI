using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynkNote.Models
{
    public class Note
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("Owner")]
        public ObjectId Owner { get; set; }

        [BsonElement("lastEdited")]
        public DateTime LastEdited { get; set; }

        [BsonElement("notebookId")]
        public ObjectId NotebookId { get; set; }

        [BsonElement("location")]
        public string Location { get; set; }

        [BsonElement("content")]
        public string Content { get; set; }

        [BsonElement("diffs")]
        public string[] Diffs { get; set; }
    }
}
