using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SynkNote.Enums;
using SynkNote.Models;
using SynkNote.Output;

namespace SynkNote.Controllers
{
    [Route("api/[controller]")]
    public class NoteController : Controller
    {
        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<string> Get(string id, [FromForm]string userId, [FromForm]string token)
        {
            if (id == null || userId == null || token == null)
                return ErrorReturner.Make(ReturnCode.InvalidInput);

            User user = await new User().Get(ObjectId.Parse(userId), token);
            if (user.Id == null)
                return ErrorReturner.Make(ReturnCode.UserNotFound);

            string newToken = user.ValidateToken(token);
            if (newToken != null) // Validate and update token
            {
                var noteFilter = Builders<Note>.Filter.Eq(a => a.Id, ObjectId.Parse(id));
                Note note = await new Database().NoteCollection.FindAsync(noteFilter)
                                                               .Result.FirstOrDefaultAsync();

                if (note == null)
                    return ErrorReturner.Make(ReturnCode.NoteNotFound);
                if (note.Owner != ObjectId.Parse(userId))
                    return ErrorReturner.Make(ReturnCode.PermissionDenied);

                return JsonConvert.SerializeObject(new
                {
                    token = newToken,
                    location = note.Location,
                    notebookId = note.NotebookId,
                    content = note.Content

                });
            } else
            {
                return ErrorReturner.Make(ReturnCode.InvalidToken);
            }
        }

        // POST api/<controller>
        [HttpPost]
        public async Task<string> Create([FromForm]string userId, [FromForm]string token, [FromForm]string location,
                             [FromForm]string notebookId, [FromForm]string content = null)
        {
            if (userId == null || token == null || location == null || notebookId == null)
                return ErrorReturner.Make(ReturnCode.InvalidInput);

            User user = await new User().Get(ObjectId.Parse(userId), token);
            if (user.Id == null)
                return ErrorReturner.Make(ReturnCode.UserNotFound);

            string newToken = user.ValidateToken(token); // Validate and update token
            if (newToken == null)
                return ErrorReturner.Make(ReturnCode.InvalidToken);

            Note note = new Note
            {
                Owner      = ObjectId.Parse(userId),
                LastEdited = DateTime.UtcNow,
                NotebookId = ObjectId.Parse(notebookId),
                Location   = location,
                Content    = content
            };

            await new Database().NoteCollection.InsertOneAsync(note);
            return JsonConvert.SerializeObject(new
            {
                token = newToken,
                id = note.Id
            });
        }

        // PUT api/<controller>/5/edit
        [HttpGet("{id}/edit")]
        public string Edit(string id, [FromForm]string userId, [FromForm]string token, [FromForm]string content)
        {
            if (id == null || userId == null || token == null)
                return ErrorReturner.Make(ReturnCode.InvalidInput);

            string noteActionReturn = NoteAction
            (
                ObjectId.Parse(id),
                ObjectId.Parse(userId),
                token,
                ActionType.Update,
                Builders<Note>.Update.Set("content", content)
            ).Result;

            if (noteActionReturn.StartsWith("[Error] ")) return noteActionReturn;
            else return Returner.Custom("token", noteActionReturn);
        }

        // PUT api/<controller>/5/edit
        [HttpGet("{id}/move")]
        public string Move(string id, [FromForm]string userId, [FromForm]string token)
        {
            if (id == null || userId == null || token == null)
                return ErrorReturner.Make(ReturnCode.InvalidInput);
            return "Not implemented.";
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public string Delete(string id, [FromForm]string userId, [FromForm]string token)
        {
            if (id == null ||userId == null || token == null)
                return ErrorReturner.Make(ReturnCode.InvalidInput);

            string noteActionReturn = NoteAction
            (
                ObjectId.Parse(id),
                ObjectId.Parse(userId),
                token,
                ActionType.Delete
            ).Result;

            if (noteActionReturn.StartsWith("[Error] ")) return noteActionReturn;
            else return Returner.Custom("token", noteActionReturn);
        }

        private async Task<string> NoteAction(ObjectId id, ObjectId userId, string token, ActionType actionType, UpdateDefinition<Note> update = null)
        {
            User user = await new User().Get(userId, token);
            if (user.Id == null) return ErrorReturner.Make(ReturnCode.UserNotFound);

            string newToken = user.ValidateToken(token);
            if (newToken == null) return ErrorReturner.Make(ReturnCode.InvalidToken);

            var noteFilter = Builders<Note>.Filter.Eq(a => a.Id, id);
            var noteCollection = new Database().NoteCollection;

            Note note = await noteCollection.FindAsync(noteFilter).Result.FirstOrDefaultAsync();
            if (note == null) return ErrorReturner.Make(ReturnCode.NoteNotFound); // If note not found
            if (note.Owner != userId) return ErrorReturner.Make(ReturnCode.PermissionDenied); // If not the owner

            if (actionType == ActionType.Delete)                    // Delete note
                DeleteNote(noteCollection, noteFilter);
            else if (actionType == ActionType.Update)               // Update note
                UpdateNote(noteCollection, noteFilter, update);

            return newToken;
        }

        private void DeleteNote(IMongoCollection<Note> noteCollection, FilterDefinition<Note> filter)
        {
            noteCollection.DeleteOne(filter);
        }

        private void UpdateNote(IMongoCollection<Note> noteCollection, FilterDefinition<Note> filter, UpdateDefinition<Note> update)
        {
            noteCollection.UpdateOne
            (
                filter,
                update
            );
        }
    }
}
