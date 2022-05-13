using Microsoft.AspNetCore.Mvc;
using code_exchanger_back.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using code_exchanger_back.Services;
using code_exchanger_back.Settings;

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
            if (possibleUser is null) 
                return BadRequest(Settings.ErrorMessages.NoUser);
            if (PasswordFunctions.CheckPasswords(possibleUser.password, PasswordFunctions.GetHash(password))) return Ok();
            return BadRequest(Settings.ErrorMessages.WrongUserPassword);
        }

        [HttpPost("")]
        public IActionResult CreateUser([FromQuery] string username, [FromQuery] string password)
        {
            if (dBConnector.GetUserByUserName(username) is not null)
                return BadRequest(Settings.ErrorMessages.UserAlreadyExist);
            if (!PasswordFunctions.CheckString(username)) 
                return BadRequest(Settings.ErrorMessages.ProhibitedSymbols);
            if (password.Length < 6) 
                return BadRequest(Settings.ErrorMessages.ShortPassword);
            if (password.Length > 128) 
                return BadRequest(Settings.ErrorMessages.LongPassword);
            if (username.Length < 2 || username.Length > 20)
                return BadRequest(Settings.ErrorMessages.InvalidLoginLength);
            if (!PasswordFunctions.CheckString(password)) 
                return BadRequest(Settings.ErrorMessages.ProhibitedSymbols);
            dBConnector.CreateUser(dBConnector.GetAmountUsers() + 1, username, PasswordFunctions.GetHash(password));
            return Ok();
        }

        [HttpGet("")]
        public IActionResult GetUserContent([FromQuery] string username, [FromQuery] string password)
        {
            User possibleUser = dBConnector.GetUserByUserName(username);
            if (possibleUser is null) 
                return BadRequest(Settings.ErrorMessages.NoUser);
            if (PasswordFunctions.CheckPasswords(possibleUser.password, PasswordFunctions.GetHash(password)))
                return Ok(dBConnector.GetContentByUserID(possibleUser.ID));
            return BadRequest(Settings.ErrorMessages.WrongUserPassword);
        }

        [HttpDelete("")]
        public IActionResult DeleteUser([FromQuery] string username, [FromQuery] string password)
        {
            User possibleUser = dBConnector.GetUserByUserName(username);
            if (possibleUser is null) 
                return BadRequest(Settings.ErrorMessages.NoUser);
            if (!PasswordFunctions.CheckPasswords(possibleUser.password, PasswordFunctions.GetHash(password))) 
                return BadRequest(Settings.ErrorMessages.WrongUserPassword);
            dBConnector.DeleteUser(username);
            dBConnector.DeleteContentByAuthor(possibleUser.ID);
            return Ok();
        }

        [HttpPut("")]
        public IActionResult UpdatePassword([FromQuery] string username, [FromQuery] string old_password, [FromQuery] string new_password)
        {
            User possibleUser = dBConnector.GetUserByUserName(username);
            if (possibleUser is null) 
                return BadRequest(Settings.ErrorMessages.NoUser);
            if (!PasswordFunctions.CheckPasswords(possibleUser.password, PasswordFunctions.GetHash(old_password)))
                return BadRequest(Settings.ErrorMessages.WrongUserPassword);
            if (!PasswordFunctions.CheckString(new_password)) 
                return BadRequest(Settings.ErrorMessages.ProhibitedSymbols);
            if (new_password.Length < 6) 
                return BadRequest(Settings.ErrorMessages.ShortPassword);
            if (new_password.Length > 128) 
                return BadRequest(Settings.ErrorMessages.LongPassword);
            dBConnector.UpdatePassword(username, PasswordFunctions.GetHash(new_password));
            return Ok();
        }
    }
}
