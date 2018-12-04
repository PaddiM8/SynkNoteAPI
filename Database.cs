using MongoDB.Bson;
using MongoDB.Driver;
using SynkNote.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynkNote
{
    public class Database
    {

        public MongoClient Client { get; }
        public IMongoCollection<User> UserCollection
        {
            get { return Db.GetCollection<User>("Users"); }
        }

        public IMongoCollection<Note> NoteCollection
        {
            get { return Db.GetCollection<Note>("Notes"); }
        }

        public IMongoCollection<Notebook> NotebookCollection
        {
            get { return Db.GetCollection<Notebook>("Notebooks"); }
        }

        private IMongoDatabase Db { get; }

        public Database()
        {
            this.Client = new MongoClient(Config.DatabaseConnectionString);
            this.Db = Client.GetDatabase("SynkNote");
        }

          public User GetUser(ObjectId objectId)
          {
              IMongoCollection<User> userCollection = Db.GetCollection<User>("Users");
              var userFilter = Builders<User>.Filter.Eq(a => a.Id, objectId);
              return userCollection.Find(userFilter).First();
          }

          public User GetUser(string email)
          {
              IMongoCollection<User> userCollection = Db.GetCollection<User>("Users");
              var userFilter = Builders<User>.Filter.Eq(a => a.Email, email);
              return userCollection.Find(userFilter).First();
        }
    }
}
