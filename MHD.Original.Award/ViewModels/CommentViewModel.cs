using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EFDal.Models;
using EFDal.Models.Enums;

namespace MHD.Original.Award.ViewModels
{
    public class CommentViewModel : original_discuss_info
    {
        public string UserName { get; set; }
        public string PortraitUrl { get; set; }

        public List<CommentViewModel> ChildrenComment;
    }
}