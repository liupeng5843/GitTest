using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EFDal.Models.Enums;

namespace MHD.Original.Award.ViewModels
{
    public class VoteBookViewModel
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public string BookCoverUrl { get; set; }
        public int AuthorId { get; set; }
        public string Pseudonym { get; set; }
        public string LastChapterName { get; set; }
        public string Brief { get; set; }
        public int Position { get; set; }
        public int Score { get; set; }
        public int WeekScore { get; set; }
        public int MonthScore { get; set; }
        public int TotalScore { get; set; }
        public string ThirdPartyUrl { get; set; }
        public bool IsHot { get; set; }
        public int PinnedLevel { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime OnShelfTime { get; set; }
        public DragonAwardType AwardType { get; set; }

        public List<FansViewModel> FansList;

        public List<ChapterViewModel> ChapterList;
    }
}