using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EFDal.Models.Enums;

namespace MHD.Original.Award.ViewModels
{
    public class SearchBookListViewModel
    {
        public int CurrentUserId { get; set; }

        public int PageCount { get; set; }
        
        public string Keywords { get; set; }

        public DragonAwardType AwardType { get; set; }

        public List<VoteBookViewModel> VoteBookList;
    }
}