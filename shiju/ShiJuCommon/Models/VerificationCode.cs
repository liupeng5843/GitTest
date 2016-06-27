using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiJu.Models
{
    public class VerificationCode
    {
        [Key]
        public string PhoneNumber { get; set; }
        public string Code { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
