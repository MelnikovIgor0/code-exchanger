using Microsoft.AspNetCore.Mvc;
using code_exchanger_back.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using code_exchanger_back.Services;

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

        //[HttpGet("users/{id}")]
        //public IActionResult GetUsers(int id)
        //{
        //    return Ok(dBConnector.GetUser(id));
        //}
    }
}
