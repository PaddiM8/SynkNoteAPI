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

            return JsonConvert.SerializeObject(new
            {
                token = newToken,
                notebookId = notebook.Id
            });
        }

        [HttpGet]
        public async Task<string> GetAll([FromQuery]string userId, [FromQuery]string token)
        {
            if (userId == null || token == null)
                return ErrorReturner.Make(ReturnCode.InvalidInput);

            User user = await new User().Get(ObjectId.Parse(userId), token);
            if (user.Id == null) return ErrorReturner.Make(ReturnCode.UserNotFound);

            string newToken = user.ValidateToken(token); // Validate and update token
            if (newToken == null) return ErrorReturner.Make(ReturnCode.InvalidToken);

            var filter = Builders<Notebook>.Filter.Eq(a => a.Owner, ObjectId.Parse(userId));
            var notebooks = await new Database().NotebookCollection.FindAsync(filter).Result.ToListAsync();

            return JsonConvert.SerializeObject(new
            {
                token = newToken,
                notebooks
            });
        }

        [HttpGet("{id}")]
        public async Task<string> GetAllNotes(string id, [FromQuery]string userId, [FromQuery]string token)
        {
            if (userId == null || token == null)
                return ErrorReturner.Make(ReturnCode.InvalidInput);

            User user = await new User().Get(ObjectId.Parse(userId), token);
            if (user.Id == null) return ErrorReturner.Make(ReturnCode.UserNotFound);

            string newToken = user.ValidateToken(token); // Validate and update token
            if (newToken == null) return ErrorReturner.Make(ReturnCode.InvalidToken);

            var filter = Builders<Note>.Filter.Eq(a => a.NotebookId, ObjectId.Parse(id));
            var notes = await new Database().NoteCollection.FindAsync(filter).Result.ToListAsync();
            var noteSkeletons = new List<NoteSkeleton>();

            foreach (var note in notes)
            {
                noteSkeletons.Add(new NoteSkeleton(note.Id, note.LastEdited, note.Location));
            }

            return JsonConvert.SerializeObject(new
            {
                token = newToken,
                noteSkeletons
            });
        }

    }
}