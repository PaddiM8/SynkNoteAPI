using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using SynkNote.Algorithms;
using SynkNote.Output;

namespace SynkNote.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {

        // GET api/<controller>
        [HttpPost]
        public async Task<string> Login([FromForm]string email, [FromForm]string password)
        {
            if (email == null || password == null) return ErrorReturner.Make(ReturnCode.InvalidInput);
            User user = await new User().Get(email, password); //new Database().GetUser(email);

            if (user == null)  return ErrorReturner.Make(ReturnCode.UserNotFound);
            if (PBKDF2.Validate(password, user.PasswordHash))
            {
                return JsonConvert.SerializeObject(new
                {
                    id = user.Id,
                    token = user.CreateToken().Result
                });
            }
            else
            {
                return ErrorReturner.Make(ReturnCode.IncorrectPassword);
            }
        }
    }
}
