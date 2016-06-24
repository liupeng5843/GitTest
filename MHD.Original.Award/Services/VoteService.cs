using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MHD.Original.Award.Services
{
    public static class VoteService
    {
        private static DateTime voteStartTime = new DateTime(2016, 5, 1);

        public static int GetMonthNumberNow()
        {
            return DateTime.Now.Month;
        }

        public static int GetWeekNumberNow()
        {
            return (DateTime.Now.Month - voteStartTime.Month) * 4 + ((DateTime.Now.Day - 1) / 7 > 3 ? 3 : ((DateTime.Now.Day-1) / 7));
            //var mills = (DateTime.Now - voteStartTime).Days / 7;
            //return (DateTime.Now - voteStartTime).Days / 7 + 1;
        }
    }
}