using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MHD.Original.Award.ViewModels
{
    public class HeaderViewModel
    {
        public bool IsLogOn { get; set; }

        public string Name { get; set; }

        public string CoverUrl { get; set; }

        public HeaderViewModel()
        {
            IsLogOn = false;
            Name = string.Empty;
            CoverUrl = string.Empty;
        }
    }
}