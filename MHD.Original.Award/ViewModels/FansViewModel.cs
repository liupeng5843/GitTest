using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MHD.Original.Award.ViewModels
{
    public class FansViewModel
    {
        public int UserId{get;set;}
        public string Name { get; set; }
        public int Position { get; set; }
        public int SupportScore { get; set; }
        public string PortraitUrl { get; set; }
    }
}