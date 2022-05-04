using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace code_exchanger_back.Models
{
    public class Content
    {
        public string code { get; set; }

        public object creation_time { get; set; }

        public long authorID { get; set; }

        public string link { get; set; }

        public byte language { get; set; }

        public byte[] password { get; set; }

        [Key]
        public long ID { get; set; }
    }
}
