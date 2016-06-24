using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MHD.Original.Award.Models
{
    public class UserDataModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string AuthorName { get; set; }
        public bool IsSign { get; set; }
    }
}