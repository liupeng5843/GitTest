using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MHD.Original.Award.ViewModels
{
    public class PageViewModel
    {
        public int Id { get; set; }

        public int PartId { get; set; }

        public int PageNumber { get; set; }

        public string PageUrl { get; set; }
    }
}