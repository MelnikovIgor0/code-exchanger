﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace code_exchanger_back.Models
{
    public class Users
    {
        [Key]
        public ulong ID { get; set; }

        public string username { get; set; }

        public byte[] password { get; set; }
    }
}