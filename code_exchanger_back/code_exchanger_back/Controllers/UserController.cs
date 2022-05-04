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
    public class UserController : Controller
    {
        private DBConnector dBConnector;


        public UserController(DBConnector dBConnector)
        {
            this.dBConnector = dBConnector;
        }

        [HttpGet("user/{id}")]
        public IActionResult GetUser(int id)
        {
            return Ok(dBConnector.GetUserByID(id));
        }

        [HttpPost("user/create")]
        public IActionResult CreateUser([FromQuery] string username, [FromQuery] string password)
        {
            if (dBConnector.GetUserByUserName(username) is not null)
                return BadRequest("user with same username exists");
            bool ok = true;
            foreach (char c in password)
                if (!(c>= 'A' && c <= 'Z' || c >= 'a' && c <= 'z' || c >= '0' && c <= '9'))
                    ok = false;
            if (!ok) return BadRequest("password contains prohibited characters");
            SHA512 hasher = new SHA512Managed();
            dBConnector.CreateUser(dBConnector.GetAmountUsers() + 1, username, 
                hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
            return Ok(dBConnector.GetUserByUserName(username));
        }
    }
}
