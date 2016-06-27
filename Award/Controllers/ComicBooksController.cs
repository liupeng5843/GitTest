using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using EFDal.Models.Enums;
using MHD.Original.Award.Models;
using MHD.Original.Award.Services;
using MHD.Original.Award.ViewModels;

namespace MHD.Original.Award.Controllers
{
    public class ComicBooksController : Controller
    {
        // GET: /Books/
        private static BookComicService bookComicService = new BookComicService();
        private static Object obj = new Object();


        [HttpPost]
        public ActionResult GetCurrentLoginUserId()
        {
            var originalIdentity = HttpContext.User.Identity as OriginalIdentity;
            var userId = originalIdentity != null ? originalIdentity.UserData.Id : 0;
            return Json(new { UserId = userId });
        }

        //只有漫画大奖是日榜 //现在是所有奖项要日榜
        public ActionResult GetLastDayRankList(DragonAwardType type)
        {
            var lastDayValue = (DateTime.Now - new DateTime(2016, 5, 1)).Days;//从五月一号开始
            var rankList = bookComicService.GetOriginalFinalists(lastDayValue, type, VoteRankType.DayRank, 10);
            return PartialView(rankList);
        }

        //获取上周排行榜
        [OutputCache(Duration = 10)]
        public ActionResult GetBookWeekRankList(DragonAwardType awardType, int weekNumber = 0)
        {
            var originalIdentity = HttpContext.User.Identity as OriginalIdentity;
            var userId = originalIdentity != null ? originalIdentity.UserData.Id : 0;

            ViewBag.UserId = userId;
            //最小5月1开始
            var rankList = bookComicService.GetOriginalFinalists(weekNumber, awardType, VoteRankType.WeekRank, 5);
            return PartialView(rankList);
        }


        //根据奖项类型获取上周排行榜 (弃用)
        [OutputCache(Duration = 10)]
        public ActionResult GetBookWeekRankListByType(DragonAwardType awardType, int weekNumber = 0)
        {
            var originalIdentity = HttpContext.User.Identity as OriginalIdentity;
            var userId = originalIdentity != null ? originalIdentity.UserData.Id : 0;

            ViewBag.UserId = userId;
            //最小5月1开始
            var lastWeekNumber = VoteService.GetWeekNumberNow();

            var rankList = bookComicService.GetOriginalFinalists(lastWeekNumber, DragonAwardType.None, VoteRankType.WeekRank, 5);
            return PartialView(rankList);
        }


        //根据奖项类型获取本周排行榜
        [OutputCache(Duration = 10)]
        public ActionResult GetCurrentWeekRankListByType(DragonAwardType awardType)
        {
            var originalIdentity = HttpContext.User.Identity as OriginalIdentity;
            var userId = originalIdentity != null ? originalIdentity.UserData.Id : 0;
            ViewBag.UserId = userId;
            var currentWeekNumber = VoteService.GetWeekNumberNow() + 1;
            var rankList = bookComicService.GetOriginalFinalists(currentWeekNumber, awardType, VoteRankType.CurrentWeekRank, 10);

            return PartialView(rankList);
        }


        //获取月排行榜
        [OutputCache(Duration = 10)]
        public ActionResult GetBookRankListByType(DragonAwardType type, int monthNumber = 0)
        {
            var originalIdentity = HttpContext.User.Identity as OriginalIdentity;
            var userId = originalIdentity != null ? originalIdentity.UserData.Id : 0;

            //最小五月份开始
            //var month = DateTime.Now.Month > 5 ? DateTime.Now.Month : 5;
            ViewBag.UserId = userId;
            var rankList = bookComicService.GetOriginalFinalists(monthNumber, type, VoteRankType.MonthRank);
            return PartialView(rankList);
        }

        [OutputCache(Duration = 5)]
        public ActionResult GetBookListByType(DragonAwardType type, string keywords = "", bool isCurrentWeek = false, int pageNumber = 1, int start = 0, int count = 10)
        {
            var originalIdentity = HttpContext.User.Identity as OriginalIdentity;
            var userId = originalIdentity != null ? originalIdentity.UserData.Id : 0;
            ViewBag.UserId = userId;
            IList<VoteBookViewModel> bookList = new List<VoteBookViewModel>();
            if (keywords == string.Empty)
            {
                bookList = bookComicService.GetOriginalBookListByType(type, pageNumber, count, isCurrentWeek);
            }
            else
            {
                bookList = bookComicService.GetOriginalBookListByKeywords(keywords);
            }
            return PartialView(bookList);
        }

        [OutputCache(Duration = 5)]
        public ActionResult SearchBookList(DragonAwardType type, string keywords = "", bool isCurrentWeek = false, int pageNumber = 1, int start = 0, int count = 10)
        {
            if (HttpContext.Request.Browser.IsMobileDevice)
            {
                return RedirectToAction("M_SearchBookList", new { type = type });
            }
            var originalIdentity = HttpContext.User.Identity as OriginalIdentity;
            var userId = originalIdentity != null ? originalIdentity.UserData.Id : 0;

            IList<VoteBookViewModel> bookList = new List<VoteBookViewModel>();
            var bookCount = 0;
            if (keywords.Trim() == string.Empty)
            {
                bookCount = bookComicService.GetOriginalBookCountByType(type, isCurrentWeek);
                bookList = bookComicService.GetOriginalBookListByType(type, pageNumber, count, isCurrentWeek);
            }
            else
            {
                bookCount = bookComicService.GetOriginalBookCountByKeywords(keywords);
                bookList = bookComicService.GetOriginalBookListByKeywords(keywords.Trim(), pageNumber, count);
            }
            var pageCount = bookCount % count > 0 ? bookCount / count + 1 : bookCount / count;

            var searchBookList = new SearchBookListViewModel { CurrentUserId = userId, Keywords = keywords.Trim(), PageCount = pageCount, AwardType = type };
            searchBookList.VoteBookList = bookList.ToList();

            ViewBag.IsCurrentWeek = isCurrentWeek;
            ViewBag.UserId = userId;

            return View(searchBookList);
        }



        [OutputCache(Duration = 5)]
        public ActionResult SearchBookListChild(DragonAwardType type, string keywords = "", int pageNumber = 1, int start = 0, int count = 10)
        {
            var originalIdentity = HttpContext.User.Identity as OriginalIdentity;
            var userId = originalIdentity != null ? originalIdentity.UserData.Id : 0;

            IList<VoteBookViewModel> bookList = new List<VoteBookViewModel>();
            var bookCount = 0;
            if (keywords.Trim() == string.Empty)
            {
                bookCount = bookComicService.GetOriginalBookCountByType(type);
                bookList = bookComicService.GetOriginalBookListByType(type, pageNumber, count);
            }
            else
            {
                bookCount = bookComicService.GetOriginalBookCountByKeywords(keywords);
                bookList = bookComicService.GetOriginalBookListByKeywords(keywords.Trim(), pageNumber, count);
            }
            var pageCount = bookCount % count > 0 ? bookCount / count + 1 : bookCount / count;

            var searchBookList = new SearchBookListViewModel { CurrentUserId = userId, Keywords = keywords.Trim(), PageCount = pageCount, AwardType = type };
            searchBookList.VoteBookList = bookList.ToList();
            ViewBag.UserId = userId;

            return PartialView(searchBookList);
        }


        #region 旧的投票逻辑，需要登录逻辑
        //[HttpPost]
        //public ActionResult CreateVote(int bookId, int shareUserId = 0)
        //{
        //    var originalIdentity = HttpContext.User.Identity as OriginalIdentity;
        //    if (originalIdentity == null)
        //    {
        //        return Json(new { code = -2, description = "Have no access" });
        //    }
        //    var userId = originalIdentity.UserData.Id;



        //    if (!bookComicService.CreateVote(userId, bookId, shareUserId))
        //    {
        //        return Json(new { code = -1, description = "CreteVote failed" });
        //    }

        //    return Json(new { code = 0, description = "CreateVote Succeed" });
        //}
        #endregion

        [HttpPost]
        public ActionResult CreateVote(int bookId, int shareUserId = 0)
        {
            var originalIdentity = HttpContext.User.Identity as OriginalIdentity;
            var userVoteCountCookie = HttpContext.Request.Cookies["UVLC"];
            var userVoteBookIdsCookie = HttpContext.Request.Cookies["CBIDS"];

            var remainHourTime = 24 - DateTime.Now.Hour;
            var remainMinuteTime = 60 - DateTime.Now.Minute;
            TimeSpan ts = new TimeSpan(remainHourTime - 1, remainMinuteTime, 0);

            if (remainMinuteTime >= 60)
            {
                ts = new TimeSpan(remainHourTime, 0, 0);
            }

            if (userVoteCountCookie == null)
            {
                userVoteCountCookie = new HttpCookie("UVLC", "0");
            }
            var idList = new string[] { };
            if (userVoteBookIdsCookie == null)
            {
                userVoteBookIdsCookie = new HttpCookie("CBIDS", "0");
            }

            var userVoteNum = int.Parse(userVoteCountCookie.Value) + 1;
            if (userVoteNum > 5)
            {
                return Json(new { code = -1, description = "CreteVote failed" });
            }

            userVoteBookIdsCookie.Value = userVoteBookIdsCookie.Value + ',' + bookId.ToString();
            userVoteCountCookie.Value = userVoteNum.ToString();

            var userId = originalIdentity != null ? originalIdentity.UserData.Id : 0;

            string userHostAddress = HttpContext.Request.UserHostAddress;

            //if (originalIdentity == null)
            //{
            //    return Json(new { code = -2, description = "Have no access" });
            //}

            if (!bookComicService.CreateVote(userId, bookId, shareUserId, userHostAddress))
            {
                userVoteCountCookie.Expires = DateTime.Now.Add(ts);//设置过期时间
                userVoteBookIdsCookie.Expires = DateTime.Now.Add(ts);//设置过期时间
                Response.AppendCookie(userVoteCountCookie);
                Response.AppendCookie(userVoteBookIdsCookie);
                return Json(new { code = -1, description = "CreteVote failed" });
            }

            var voteStatus = bookComicService.CheckUserVoteStatus(bookId);
            var monthNum = bookComicService.GetUserVoteVaildNum(bookId);
            if (voteStatus == UserVoteStatus.Invaild)
            {
                userVoteCountCookie.Expires = DateTime.Now.Add(ts);//设置过期时间
                userVoteBookIdsCookie.Expires = DateTime.Now.Add(ts);//设置过期时间
                Response.AppendCookie(userVoteCountCookie);
                Response.AppendCookie(userVoteBookIdsCookie);
                return Json(new { code = 1, description = "CreateVote failed ,because it is invaild", month = monthNum });
            }

            userVoteCountCookie.Expires = DateTime.Now.Add(ts);//设置过期时间
            userVoteBookIdsCookie.Expires = DateTime.Now.Add(ts);//设置过期时间
            Response.AppendCookie(userVoteCountCookie);
            Response.AppendCookie(userVoteBookIdsCookie);

            return Json(new { code = 0, description = "CreateVote Succeed" });
        }

        [HttpPost]
        public ActionResult GetCurrentUserInfo()
        {
            var originalIdentity = HttpContext.User.Identity as OriginalIdentity;
            return Json(originalIdentity.UserData);
        }

        public ActionResult GetBookDetailById(int bookId, int userId = 0, int pagenumber = 1)
        {
            ViewBag.UserId = userId;
            var book = bookComicService.GetOriginalBookById(bookId);
            if (book.ChapterList != null)
            {
                ViewBag.PageCount = book.ChapterList.Count % 30 == 0 ? book.ChapterList.Count / 30 : book.ChapterList.Count / 30 + 1;
            }
            return View(book);
        }


        public ActionResult GetWeekRankList()
        {

            return View();
        }

        [OutputCache(Duration = 5)]
        public ActionResult GetChapter(int chapterId, int userId = 0)
        {
            var chapter = bookComicService.GetOriginalChapterById(chapterId);

            ViewBag.UserId = userId;
            return View(chapter);
        }


        #region 手机页面
        [OutputCache(Duration = 10)]
        public ActionResult M_Vote()
        {
            if (!HttpContext.Request.Browser.IsMobileDevice)
            {
                return RedirectToAction("Vote", "Home");
            }
            return View();
        }

        public ActionResult M_GetBookListByType(DragonAwardType type, int start = 0, int count = 9)
        {
            var originalIdentity = HttpContext.User.Identity as OriginalIdentity;
            var userId = originalIdentity != null ? originalIdentity.UserData.Id : 0;
            ViewBag.UserId = userId;
            IList<VoteBookViewModel> bookList = new List<VoteBookViewModel>();

            bookList = bookComicService.GetOriginalBookListByType(type, 1, count);

            return PartialView(bookList);
        }

        [OutputCache(Duration = 10)]
        public ActionResult M_SearchBookList(DragonAwardType type, string keywords = "", int pageNumber = 1, int start = 0, int count = 15)
        {
            if (!HttpContext.Request.Browser.IsMobileDevice)
            {
                return RedirectToAction("SearchBookList", new { type = type });
            }

            var originalIdentity = HttpContext.User.Identity as OriginalIdentity;
            var userId = originalIdentity != null ? originalIdentity.UserData.Id : 0;

            IList<VoteBookViewModel> bookList = new List<VoteBookViewModel>();
            var bookCount = 0;
            if (keywords.Trim() == string.Empty)
            {
                bookCount = bookComicService.GetOriginalBookCountByType(type);
                bookList = bookComicService.GetOriginalBookListByType(type, pageNumber, count);
            }
            else
            {
                bookCount = bookComicService.GetOriginalBookCountByKeywords(keywords);
                bookList = bookComicService.GetMBookListByKeywords(keywords.Trim(), pageNumber, count);
            }
            var pageCount = bookCount % count > 0 ? bookCount / count + 1 : bookCount / count;

            var searchBookList = new SearchBookListViewModel { CurrentUserId = userId, Keywords = keywords.Trim(), PageCount = pageCount, AwardType = type };
            searchBookList.VoteBookList = bookList.ToList();
            ViewBag.UserId = userId;

            return View(searchBookList);
        }

        public ActionResult M_BookListPartView(DragonAwardType type, string keywords = "", int pageNumber = 1, int start = 0, int count = 15)
        {
            var originalIdentity = HttpContext.User.Identity as OriginalIdentity;
            var userId = originalIdentity != null ? originalIdentity.UserData.Id : 0;

            IList<VoteBookViewModel> bookList = new List<VoteBookViewModel>();
            var bookCount = 0;
            if (keywords.Trim() == string.Empty)
            {
                bookCount = bookComicService.GetOriginalBookCountByType(type);
                bookList = bookComicService.GetOriginalBookListByType(type, pageNumber, count);
            }
            else
            {
                bookCount = bookComicService.GetOriginalBookCountByKeywords(keywords);
                bookList = bookComicService.GetMBookListByKeywords(keywords.Trim(), pageNumber, count);
            }
            var pageCount = bookCount % count > 0 ? bookCount / count + 1 : bookCount / count;

            var searchBookList = new SearchBookListViewModel { CurrentUserId = userId, Keywords = keywords.Trim(), PageCount = pageCount, AwardType = type };
            searchBookList.VoteBookList = bookList.ToList();
            ViewBag.UserId = userId;

            return PartialView(searchBookList);
        }

        [OutputCache(Duration = 5)]
        public ActionResult M_GetBookDetailById(int bookId, int userId = 0, int pagenumber = 1)
        {
            var book = bookComicService.GetOriginalBookById(bookId);
            if (book.ChapterList != null)
            {
                ViewBag.PageCount = book.ChapterList.Count % 30 == 0 ? book.ChapterList.Count / 30 : book.ChapterList.Count / 30 + 1;
            }
            ViewBag.UserId = userId;
            return View(book);
        }

        [OutputCache(Duration = 10)]
        public ActionResult M_GetChapter(int chapterId, int userId = 0)
        {
            var chapter = bookComicService.GetOriginalChapterById(chapterId);

            var originalIdentity = HttpContext.User.Identity as OriginalIdentity;
            userId = originalIdentity != null ? originalIdentity.UserData.Id : 0;

            ViewBag.UserId = userId;
            return View(chapter);
        }

        //现在是所有奖项要日榜
        public ActionResult M_GetLastDayRankList(DragonAwardType type, int count = 5)
        {
            var lastDayValue = (DateTime.Now - new DateTime(2016, 5, 1)).Days;//从五月一号开始
            var rankList = bookComicService.GetOriginalFinalists(lastDayValue, type, VoteRankType.DayRank, count);
            return PartialView(rankList);
        }

        public ActionResult M_MonthRankList(DragonAwardType type)
        {
            return PartialView(type);
        }


        //根据奖项类型获取周排行榜
        [OutputCache(Duration = 10)]
        public ActionResult M_GetBookWeekRankListByType(DragonAwardType awardType, int weekNumber = 0)
        {
            var originalIdentity = HttpContext.User.Identity as OriginalIdentity;
            var userId = originalIdentity != null ? originalIdentity.UserData.Id : 0;

            ViewBag.UserId = userId;
            //最小5月1开始
            var rankList = bookComicService.GetOriginalFinalists(weekNumber, awardType, VoteRankType.WeekRank, 5);
            return PartialView(rankList);
        }

        //根据奖项类型获取本周排行榜
        [OutputCache(Duration = 10)]
        public ActionResult M_GetCurrentWeekRankListByType(DragonAwardType awardType, int count = 5)
        {
            var originalIdentity = HttpContext.User.Identity as OriginalIdentity;
            var userId = originalIdentity != null ? originalIdentity.UserData.Id : 0;
            ViewBag.UserId = userId;
            var currentWeekNumber = VoteService.GetWeekNumberNow() + 1;
            var rankList = bookComicService.GetOriginalFinalists(currentWeekNumber, awardType, VoteRankType.CurrentWeekRank, count);

            return PartialView(rankList);
        }

        public ActionResult M_Index()
        {
            return View();
        }


        #endregion
    }
}
