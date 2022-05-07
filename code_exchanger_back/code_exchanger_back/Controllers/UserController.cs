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
    }
}
