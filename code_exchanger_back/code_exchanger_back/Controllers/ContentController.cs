﻿using Microsoft.AspNetCore.Mvc;
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
        private int CRATCHID = 10;


        public ContentController(DBConnector dBConnector)
        {
            this.dBConnector = dBConnector;
        }

        [HttpPost("create/{content}")]
        public IActionResult CreateRecord(string content, [FromQuery] short language, [FromQuery] string password)
        {
            if (password is not null)
            {
                bool ok = true;
                foreach (char c in password)
                    if (!(c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z' || c >= '0' && c <= '9'))
                        ok = false;
                if (!ok) return BadRequest("password contains prohibited characters");
            }
            string link = dBConnector.CreateRecord(content, CRATCHID++, 1, language, 
                new SHA512Managed().ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
            return Ok(link);
        }

        [HttpGet("{link}")]
        public IActionResult GetContent(string link)
        {
            Content content = dBConnector.GetContent(link);
            return Ok(content);
        }
    }
}