using Microsoft.AspNetCore.Mvc;
using code_exchanger_back.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace code_exchanger_back.Controllers
{
    [ApiController]
    public class UserController : Controller
    {
        DataBaseContext MainDataBase;
        
        public UserController()
        {
            MainDataBase = new DataBaseContext(new DbContextOptions<DataBaseContext>());
        }

        [HttpGet("users/{id}")]
        public IActionResult GetUsers(int id)
        {
            MainDataBase.Database.OpenConnection();
            var con = (Npgsql.NpgsqlConnection)MainDataBase.Database.GetDbConnection();
            string data = (string)(new NpgsqlCommand($"SELECT \"username\" FROM \"Users\" WHERE \"ID\"={id}", con)).ExecuteScalar();
            return Ok(data.ToString());
        }
    }
}
