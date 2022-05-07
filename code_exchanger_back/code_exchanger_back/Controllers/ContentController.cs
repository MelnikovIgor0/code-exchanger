using Microsoft.AspNetCore.Mvc;
using code_exchanger_back.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using code_exchanger_back.Services;
using System.Security.Cryptography;

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
                return BadRequest("wrong user password");
            if (content_password != "" && !PasswordFunctions.CheckString(content_password)) 
                return BadRequest("password contains prohibited characters");
            if (content_password is not null && content_password.Length > 128)
                return BadRequest("password too long");
            if (content is not null && content.Length > 262144) return BadRequest("your code is too big");
            string link = dBConnector.CreateRecord(content, dBConnector.GetMaxContentID() + 1, possibleUser is null ? 0 : possibleUser.ID,
                language, PasswordFunctions.GetHash(content_password));
            return Ok(link);
        }

        [HttpGet("")]
        public IActionResult GetContent([FromQuery] string link, [FromQuery] string password = null)
        {
            Content content = dBConnector.GetContent(link);
            if (content is null) return NotFound("there's no code");
            if (!PasswordFunctions.CheckPasswords(PasswordFunctions.GetHash(password), content.password))
                return BadRequest("wrong content password");
            return Ok(content);
        }

        [HttpDelete("")]
        public IActionResult DeleteContent([FromQuery] string link, [FromQuery] string username, 
            [FromQuery] string user_password, [FromQuery] string content_password = null)
        {
            Content possibleContent = dBConnector.GetContent(link);
            if (possibleContent is null) return NotFound("there's no code");
            if (username is null) return BadRequest("you have no permission to delete this code");
            User possibleUser = dBConnector.GetUserByUserName(username);
            if (possibleUser is null) return BadRequest("user with this login does not exist");
            if (!PasswordFunctions.CheckPasswords(possibleUser.password, PasswordFunctions.GetHash(user_password)))
                return BadRequest("wrong user password");
            if (possibleContent.authorID == 0 ||  possibleContent.authorID != possibleUser.ID) 
                return BadRequest("you have no permission to delete this code");
            if (!PasswordFunctions.CheckPasswords(PasswordFunctions.GetHash(content_password), possibleContent.password))
                return BadRequest("wrong content password");
            dBConnector.DeleteContent(possibleContent.link);
            return Ok();
        }

        [HttpPut("")]
        public IActionResult UpdateContent([FromQuery] string link, [FromQuery] string new_content, [FromQuery] string username, 
            [FromQuery] string user_password, [FromQuery] string content_password = null)
        {
            User possibleUser = dBConnector.GetUserByUserName(username);
            if (possibleUser is null) return BadRequest("you have no permission to change this code");
            if (!PasswordFunctions.CheckPasswords(possibleUser.password, PasswordFunctions.GetHash(user_password)))
                return BadRequest("wrong user password");
            Content possibleContent = dBConnector.GetContent(link);
            if (possibleContent is null) return NotFound("there's no code");
            if (!PasswordFunctions.CheckPasswords(PasswordFunctions.GetHash(content_password), possibleContent.password))
                return BadRequest("wrong content password");
            if (possibleContent.authorID == 0 || possibleContent.authorID != possibleUser.ID)
                return BadRequest("you have no permission to change this code");
            if (new_content is not null && new_content.Length > 262144) return BadRequest("your code is too big");
            dBConnector.UpdateRecord(link, new_content);
            return Ok();
        }
    }
}
