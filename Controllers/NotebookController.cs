using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using SynkNote.Models;
using SynkNote.Output;

namespace SynkNote.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotebookController : ControllerBase
    {
        [HttpPost]
        public async Task<string> Create([FromForm]string userId, [FromForm]string  token, [FromForm]string name)
        {
            if (userId == null || token == null || name == null)
                return ErrorReturner.Make(ReturnCode.InvalidInput);

            User user = await new User().Get(ObjectId.Parse(userId), token);
            if (user.Id == null) return ErrorReturner.Make(ReturnCode.UserNotFound);

            string newToken = user.ValidateToken(token); // Validate and update token
            if (newToken == null) return ErrorReturner.Make(ReturnCode.InvalidToken);

            Notebook notebook = new Notebook
            {
                Name = name,
                Owner = ObjectId.Parse(userId)
            };

            await new Database().NotebookCollection.InsertOneAsync(notebook);
            Console.WriteLine(name);
            return JsonConvert.SerializeObject(new
            {
                Token = newToken,
                NotebookId = notebook.Id
            });
        }

        [HttpGet]
        public async Task<string> GetAll([FromForm]string userId, [FromForm]string token)
        {
            if (userId == null || token == null)
                return ErrorReturner.Make(ReturnCode.InvalidInput);

            User user = await new User().Get(ObjectId.Parse(userId), token);
            if (user.Id == null) return ErrorReturner.Make(ReturnCode.UserNotFound);

            string newToken = user.ValidateToken(token); // Validate and update token
            if (newToken == null) return ErrorReturner.Make(ReturnCode.InvalidToken);

            var filter = Builders<Notebook>.Filter.Eq(a => a.Owner, ObjectId.Parse(userId));
            var notebooks = await new Database().NotebookCollection.FindAsync(filter);
            if (notebooks == null) return ErrorReturner.Make(ReturnCode.NotebookNotFound);

            return JsonConvert.SerializeObject(new
            {
                Token = newToken,
                Notebooks = notebooks
            });
        }
    }
}