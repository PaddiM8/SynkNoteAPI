using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using SynkNote.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SynkNote.Models
{
    public class Token
    {
        public string TokenString { get; set; }

        public Tuple<Token, string> Generate()
        {
            var cryptRNG = new RNGCryptoServiceProvider();
            byte[] tokenBuffer = new byte[Config.TokenLength];
            cryptRNG.GetBytes(tokenBuffer);

            long expiry = DateTime.UtcNow.AddMonths(1).Ticks;
            string rawToken = Convert.ToBase64String(tokenBuffer);
            string token = rawToken;
            string tokenHash = PBKDF2.Hash(rawToken);

            this.TokenString = tokenHash + expiry.ToString();

            return new Tuple<Token, string>(this, token);
        }

        public string Update(ObjectId userId, int tokenId)
        {
            Delete(userId, tokenId);

            var filter = Builders<User>.Filter.Where(x => x.Id == userId && x.Tokens.Any(i => i.TokenString == TokenString));
            Tuple<Token, string> newToken = new Token().Generate();
            new Database().UserCollection.UpdateOne(
                filter,
                Builders<User>.Update.Set(x => x.Tokens[-1], newToken.Item1) // Set token with updated value
            );

            return newToken.Item2;
        }

        public DateTime GetExpiry(bool isRaw)
        {
            if (isRaw)
                return new DateTime(long.Parse(TokenString.Substring(Config.TokenLength)));
            else
                return new DateTime(long.Parse(TokenString.Substring(59)));
        }

        public void Delete(ObjectId userId, int tokenId)
        {
            var filter = Builders<User>.Filter.Where(x => x.Id == userId && x.Tokens.Any(i => i.TokenString == TokenString));
            new Database().UserCollection.UpdateOne(
                filter,
                Builders<User>.Update.Pull("tokens", TokenString)
            );
        }
    }
}
