using Microsoft.AspNetCore.Mvc;
using code_exchanger_back.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using code_exchanger_back.Services;

namespace code_exchanger_back.Controllers
{
    [ApiController]
    [Route("content")]
    public class ContentController : Controller
    {
        private DBConnector dBConnector;
        private int CRATCHID = 10;


        public ContentController(DBConnector dBConnector)
        {
            this.dBConnector = dBConnector;
        }

        [HttpPost("create/{content}")]
        public IActionResult CreateRecord(string content)
        {
            string link = dBConnector.CreateRecord(content, CRATCHID++);
            return Ok(link);
        }

        [HttpGet("{link}")]
        public IActionResult GetContent(string link)
        {
            Content content = dBConnector.GetContent(link);
            return Ok(content.code);
        }
    }
}
