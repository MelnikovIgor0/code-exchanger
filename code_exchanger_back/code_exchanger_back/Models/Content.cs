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

        public DateTime creation_time { get; set; }

        public ulong authorID { get; set; }

        public byte language { get; set; }

        public byte[] password { get; set; }

        [Key]
        public ulong ID { get; set; }
    }
}
