using Microsoft.AspNetCore.Mvc;
using code_exchanger_back.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using code_exchanger_back.Services;

namespace code_exchanger_back.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : Controller
    {
        private DBConnector dBConnector;

        public UserController(DBConnector dBConnector)
        {
            this.dBConnector = dBConnector;
        }

        [HttpGet("check")]
        public IActionResult CheckUser([FromQuery] string username, [FromQuery] string password)
        {
            User possibleUser = dBConnector.GetUserByUserName(username);
            if (possibleUser is null) return BadRequest("user with this login does not exist");
            if (PasswordFunctions.CheckPasswords(possibleUser.password, PasswordFunctions.GetHash(password))) return Ok();
            return BadRequest("wrong password");
        }

        [HttpPost("")]
        public IActionResult CreateUser([FromQuery] string username, [FromQuery] string password)
        {
            if (dBConnector.GetUserByUserName(username) is not null)
                return BadRequest("user with same username exists");
            if (!PasswordFunctions.CheckString(username)) return BadRequest("login contains prohibited characters");
            if (password.Length < 6) return BadRequest("password too short");
            if (password.Length > 128) return BadRequest("password too long");
            if (username.Length < 2 || username.Length > 20) return BadRequest("Length of login should be no less 2 and no more 20");
            if (!PasswordFunctions.CheckString(password)) return BadRequest("password contains prohibited characters");
            dBConnector.CreateUser(dBConnector.GetAmountUsers() + 1, username, PasswordFunctions.GetHash(password));
            return Ok();
        }

        [HttpGet("")]
        public IActionResult GetUserContent([FromQuery] string username, [FromQuery] string password)
        {
            User possibleUser = dBConnector.GetUserByUserName(username);
            if (possibleUser is null) return BadRequest("user with this login does not exist");
            if (PasswordFunctions.CheckPasswords(possibleUser.password, PasswordFunctions.GetHash(password)))
                return Ok(dBConnector.GetContentByUserID(possibleUser.ID));
            return BadRequest("wrong password");
        }

        [HttpDelete("")]
        public IActionResult DeleteUser([FromQuery] string username, [FromQuery] string password)
        {
            User possibleUser = dBConnector.GetUserByUserName(username);
            if (possibleUser is null) return BadRequest("user with this login does not exist");
            if (!PasswordFunctions.CheckPasswords(possibleUser.password, PasswordFunctions.GetHash(password))) 
                return BadRequest("wrong password");
            dBConnector.DeleteUser(username);
            dBConnector.DeleteContentByAuthor(possibleUser.ID);
            return Ok();
        }

        [HttpPut("")]
        public IActionResult UpdatePassword([FromQuery] string username, [FromQuery] string old_password, [FromQuery] string new_password)
        {
            User possibleUser = dBConnector.GetUserByUserName(username);
            if (possibleUser is null) return BadRequest("user with this login does not exist");
            if (!PasswordFunctions.CheckPasswords(possibleUser.password, PasswordFunctions.GetHash(old_password)))
                return BadRequest("wrong password");
            if (!PasswordFunctions.CheckString(new_password)) return BadRequest("password contains prohibited characters");
            if (new_password.Length < 6) return BadRequest("password too short");
            if (new_password.Length > 128) return BadRequest("password too long");
            dBConnector.UpdatePassword(username, PasswordFunctions.GetHash(new_password));
            return Ok();
        }
    }
}
