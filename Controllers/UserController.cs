using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using SynkNote.Enums;
using SynkNote.Output;

namespace SynkNote.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        // POST api/<controller>
        [HttpPost]
        public async Task<string> Create([FromForm]string email, [FromForm]string password)
        {
            if (email == null || password == null)
                return ErrorReturner.Make(ReturnCode.InvalidInput);

            var user = new User(email, password);
            var database = new Database();

            // If user already exists with that email
            if (new User().Exists(email).Result)
                return ErrorReturner.Make(ReturnCode.EmailExists);

            await database.UserCollection.InsertOneAsync(user);
            return Returner.Custom("id", user.Id.ToString());
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public string Delete(string id, [FromForm]string token)
        {
            if (id == null || token == null)
                return ErrorReturner.Make(ReturnCode.InvalidInput);

            var database = new Database();
            var userFilter = Builders<User>.Filter.Eq(a => a.Id, ObjectId.Parse(id));
            User user = database.UserCollection.Find(userFilter).First();

            if (user == null) return ErrorReturner.Make(ReturnCode.UserNotFound);
            if (user.ValidateTokenAndDestroy(token))
            {
                database.UserCollection.DeleteOne(userFilter);
                return Returner.Success();
            } else
            {
                return ErrorReturner.Make(ReturnCode.InvalidToken);
            }
        }
    }
}
