using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MHD.Original.Award.ViewModels
{
    public class ChapterViewModel
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public string Name { get; set; }

        public int PartNumber { get; set; }

        public int TotalNumber { get; set; }

        public int PrevPartId { get; set; }
        
        public int NextPartId { get; set; }

        public List<PageViewModel> PageList { get; set; }
    }
}