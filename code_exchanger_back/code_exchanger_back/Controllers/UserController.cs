using Microsoft.AspNetCore.Mvc;
using code_exchanger_back.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace code_exchanger_back.Controllers
{
    [ApiController]
    public class UserController : Controller
    {
        public DataBaseContext db = new DataBaseContext(new DbContextOptions<DataBaseContext>());

        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            return Ok(db.users);
        }
    }
}
