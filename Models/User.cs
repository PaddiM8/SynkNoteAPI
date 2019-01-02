using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using SynkNote.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using SynkNote.Algorithms;
using SynkNote.Enums;
using Newtonsoft.Json;

namespace SynkNote
{
    public class User
    {
        [BsonId]
        [JsonProperty("id")]
        public ObjectId Id { get; set; }

        [BsonElement("email")]
        [JsonProperty("email")]
        public string Email { get; set; }

        [BsonElement("passwordHash")]
        [JsonProperty("passwordHash")]
        public string PasswordHash { get; set; }

        [BsonElement("tokens")]
        [JsonProperty("tokens")]
        public List<Token> Tokens { get; set; }

        public User()
        {
            // ...
        }

        public User(string email, string password)
        {
            this.Email = email;
            this.PasswordHash = PBKDF2.Hash(password);
            this.Tokens = new List<Token>();
        }

        public async Task<User> Get(string email, string password)
        {
            var userFilter = Builders<User>.Filter.Eq(a => a.Email, email);
            var user = await new Database().UserCollection.FindAsync(userFilter)
                                                          .Result.FirstOrDefaultAsync();

            if (user != null)
                return user;
            return this;
        }

        public async Task<User> Get(ObjectId userId, string token)
        {
            var userFilter = Builders<User>.Filter.Eq(a => a.Id, userId);
            var user = await new Database().UserCollection.FindAsync(userFilter)
                                                          .Result.FirstOrDefaultAsync();

            if (user != null)
            {
                this.Id = user.Id;
                this.Email = user.Email;
                this.PasswordHash = user.PasswordHash;
                this.Tokens = user.Tokens;
            }

            return this;
        }

        public async Task<bool> Exists(string email)
        {
            var usersWithEmail = await new Database().UserCollection
                                                     .FindAsync(a => a.Email == email);
            return usersWithEmail.Any() && Config.Environment == Env.Production;
        }

        public async Task<string> CreateToken()
        {
            // Token Generation
            var token = new Token().Generate();

            await new Database().UserCollection.UpdateOneAsync(
                Builders<User>.Filter.Eq(a => a.Id, Id),    // Find by ObjectID
                Builders<User>.Update.Push("tokens", token.Item1) // Push token
            );

            return token.Item2;
        }

        public string ValidateToken(string inputToken)
        {
            for (int i = 0; i < Tokens.Count; i++)
            {
                if (Tokens[i].GetExpiry(false) <= DateTime.UtcNow)
                {
                    // Delete token
                    Tokens[i].Delete(Id, i);
                    continue;
                }

                if (PBKDF2.Validate(inputToken, Tokens[i].TokenString.Substring(0, 59)))
                {
                    // Update token
                    return Tokens[i].Update(Id, i);
                }
            }

            return null;
        }

        public bool ValidateTokenAndDestroy(string inputToken)
        {
            for (int i = 0; i < Tokens.Count; i++)
            {
                if (Tokens[i].GetExpiry(false) <= DateTime.UtcNow) continue;
                if (PBKDF2.Validate(inputToken, Tokens[i].TokenString.Substring(0, 59)))
                {
                    Tokens[i].Delete(Id, i);

                    return true;
                }
            }

            return false;
        }
    }
}
