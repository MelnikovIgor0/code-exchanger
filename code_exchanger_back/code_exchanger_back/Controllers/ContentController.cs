using Microsoft.AspNetCore.Mvc;
using code_exchanger_back.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using code_exchanger_back.Services;
using System.Security.Cryptography;
using code_exchanger_back.Settings;

namespace code_exchanger_back.Controllers
{
    [ApiController]
    [Route("content")]
    public class ContentController : Controller
    {
        private DBConnector dBConnector;

        public ContentController(DBConnector dBConnector)
        {
            this.dBConnector = dBConnector;
        }

        [HttpPost("")]
        public IActionResult CreateRecord([FromQuery] string content, [FromQuery] short language, 
            [FromQuery] string username = null, [FromQuery] string user_password = null, [FromQuery] string content_password = null)
        {
            User possibleUser = dBConnector.GetUserByUserName(username);
            if (possibleUser is not null && !PasswordFunctions.CheckPasswords(possibleUser.password, PasswordFunctions.GetHash(user_password)))
                return BadRequest(Settings.ErrorMessages.WrongUserPassword);
            if (content_password != "" && !PasswordFunctions.CheckString(content_password)) 
                return BadRequest(Settings.ErrorMessages.ProhibitedSymbols);
            if (content_password is not null && content_password.Length > 128)
                return BadRequest(Settings.ErrorMessages.LongPassword);
            if (content is not null && content.Length > 262144) 
                return BadRequest(Settings.ErrorMessages.BigCode);
            string link = dBConnector.CreateRecord(content, dBConnector.GetMaxContentID() + 1, possibleUser is null ? 0 : possibleUser.ID,
                language, PasswordFunctions.GetHash(content_password));
            return Ok(link);
        }

        [HttpGet("")]
        public IActionResult GetContent([FromQuery] string link, [FromQuery] string password = null)
        {
            Content content = dBConnector.GetContent(link);
            if (content is null) 
                return NotFound(Settings.ErrorMessages.NoCode);
            if (!PasswordFunctions.CheckPasswords(PasswordFunctions.GetHash(password), content.password))
                return BadRequest(Settings.ErrorMessages.WrongContentPassword);
            return Ok(content);
        }

        [HttpDelete("")]
        public IActionResult DeleteContent([FromQuery] string link, [FromQuery] string username, 
            [FromQuery] string user_password, [FromQuery] string content_password = null)
        {
            Content possibleContent = dBConnector.GetContent(link);
            if (possibleContent is null) 
                return NotFound(Settings.ErrorMessages.NoCode);
            if (username is null) 
                return BadRequest(Settings.ErrorMessages.NoPermissionDelete);
            User possibleUser = dBConnector.GetUserByUserName(username);
            if (possibleUser is null) 
                return BadRequest(Settings.ErrorMessages.NoUser);
            if (!PasswordFunctions.CheckPasswords(possibleUser.password, PasswordFunctions.GetHash(user_password)))
                return BadRequest(Settings.ErrorMessages.WrongUserPassword);
            if (possibleContent.authorID == 0 || possibleContent.authorID != possibleUser.ID) 
                return BadRequest(Settings.ErrorMessages.NoPermissionDelete);
            if (!PasswordFunctions.CheckPasswords(PasswordFunctions.GetHash(content_password), possibleContent.password))
                return BadRequest(Settings.ErrorMessages.WrongContentPassword);
            dBConnector.DeleteContent(possibleContent.link);
            return Ok();
        }

        [HttpPut("")]
        public IActionResult UpdateContent([FromQuery] string link, [FromQuery] string new_content, [FromQuery] string username, 
            [FromQuery] string user_password, [FromQuery] string content_password = null)
        {
            User possibleUser = dBConnector.GetUserByUserName(username);
            if (possibleUser is null) 
                return BadRequest(Settings.ErrorMessages.NoPermissionChange);
            if (!PasswordFunctions.CheckPasswords(possibleUser.password, PasswordFunctions.GetHash(user_password)))
                return BadRequest(Settings.ErrorMessages.WrongUserPassword);
            Content possibleContent = dBConnector.GetContent(link);
            if (possibleContent is null) 
                return NotFound(Settings.ErrorMessages.NoCode);
            if (!PasswordFunctions.CheckPasswords(PasswordFunctions.GetHash(content_password), possibleContent.password))
                return BadRequest(Settings.ErrorMessages.WrongContentPassword);
            if (possibleContent.authorID == 0 || possibleContent.authorID != possibleUser.ID)
                return BadRequest(Settings.ErrorMessages.NoPermissionChange);
            if (new_content is not null && new_content.Length > 262144) 
                return BadRequest(Settings.ErrorMessages.BigCode);
            dBConnector.UpdateRecord(link, new_content);
            return Ok();
        }
    }
}
