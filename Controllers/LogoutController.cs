using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using SynkNote.Output;

namespace SynkNote.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogoutController : ControllerBase
    {
        // GET api/<controller>
        [HttpPost]
        public string Logout([FromForm]string id, [FromForm]string token)
        {
            if (id == null || token == null) return ErrorReturner.Make(ReturnCode.InvalidInput);

            User user = new Database().GetUser(ObjectId.Parse(id));
            if (user == null) return ErrorReturner.Make(ReturnCode.UserNotFound);

            if (user.ValidateTokenAndDestroy(token)) // Valid token
                return Returner.Success();
            else
                return ErrorReturner.Make(ReturnCode.InvalidToken);
        }
    }
}