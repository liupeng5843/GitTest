using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using EFDal.Context;
using EFDal.Dal;
using EFDal.Models;
using MHD.Original.Award.Services;
using MHD.Original.Award.ViewModels;
using Newtonsoft.Json;
using SimpleComm.Helper;

namespace MHD.Original.Award.Controllers
{
    public class CommentsController : Controller
    {
        private static CommentsService commentsService = new CommentsService();
        private static Object obj = new Object();
        private static Cache cache = new Cache();
        [HttpPost]
        public ActionResult CreateComment(original_discuss_info comment)
        {
            using (var db = new Original_comic_libraryContext(ContextType.originalCon))
            {
                var originalIdentity = HttpContext.User.Identity as OriginalIdentity;
                var userId = originalIdentity != null ? originalIdentity.UserData.Id : 0;
                comment.authorid = userId;
                comment.createtime = DateTime.Now;
                comment.isdelete = false;
                comment.ispause = false;
                //comment.pinnedtime = new DateTime(2016, 1, 1);
                var touristNumber = 0;
                touristNumber = db.original_config.FirstOrDefault(c => c.configtype == 1).configvalue;
                var touristCookie = HttpContext.Request.Cookies["tourist"];

                if (touristCookie == null)
                {
                    touristCookie = new HttpCookie("tourist", touristNumber.ToString());
                    comment.name = "游客" + touristNumber;
                    db.original_config.FirstOrDefault(c => c.configtype == 1).configvalue = (short)(touristNumber + 1);

                    Response.AppendCookie(touristCookie);
                }
                else
                {
                    comment.name = "游客" + touristCookie.Value;
                }
                db.SaveChanges();

                if (!commentsService.CreateComment(comment))
                {
                    return Json(new { code = -1, des = "create comment frequently" });
                }

                var jsonComment = JsonConvert.SerializeObject(comment);
                return Json(new { code = 0, comment = jsonComment });
            }
        }

        public ActionResult GetCommentList(int pageNumber = 1)
        {
            var commentList = commentsService.GetCommentList(pageNumber);
            var commentCount = commentsService.GetCommentPageCount();
            var pageCount = commentCount / 10 + 1;
            if (commentCount % 10 == 0)
            {
                pageCount--;
            }

            var originalIdentity = HttpContext.User.Identity as OriginalIdentity;
            var userId = originalIdentity != null ? originalIdentity.UserData.Id : 0;

            ViewBag.UserId = userId;
            ViewBag.PageCount = pageCount;
            ViewBag.CommentCount = commentCount;
            ViewBag.CurrentPage = pageNumber;

            return PartialView(commentList);
        }


        public ActionResult M_GetCommentList(int pageNumber = 1)
        {
            var commentList = commentsService.GetCommentList(pageNumber);
            var commentCount = commentsService.GetCommentPageCount();
            var pageCount = commentCount / 10 + 1;
            if (commentCount / 10 == 0)
            {
                pageCount--;
            }

            var originalIdentity = HttpContext.User.Identity as OriginalIdentity;
            var userId = originalIdentity != null ? originalIdentity.UserData.Id : 0;

            ViewBag.UserId = userId;
            ViewBag.PageCount = pageCount;
            ViewBag.CommentCount = commentCount;
            ViewBag.CurrentPage = pageNumber;

            return PartialView(commentList);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetVoteRecords()
        {
            using (var db = new Original_comic_libraryContext(ContextType.originalRead))
            {
                var records = HttpRuntime.Cache.Get("RecordList");
                if (records == null)
                {
                    lock (obj)
                    {
                        var comicNameList = db.original_book_info.Where(b => b.isonshelf && !b.isdelete & !b.ispause).Select(b => b.name).ToList();
                        var userNameList = db.original_author_info.Where(b => b.authorname != "" && !b.isdelete && !b.ispause).Select(a => a.authorname).ToList();
                        Random r = new Random();
                        var value = r.Next(0, comicNameList.Count());
                        var recordList = new List<string>();
                        var recordsHtml = "";
                        for (int i = 0; i < 15; i++)
                        {
                            var randomComicIndex = r.Next(0, comicNameList.Count() - 1);
                            var randomUserIndex = r.Next(0, userNameList.Count() - 1);

                            var record = "<span>《" + comicNameList[randomComicIndex] + "》得到了" + userNameList[randomUserIndex] + "的投票 </span>";
                            recordsHtml += record;
                            //recordList.Add(record);
                        }

                        //CacheItemRemovedCallback onRemove = new CacheItemRemovedCallback(this.RemovedCallback);

                        HttpRuntime.Cache.Insert("RecordList",
                            recordsHtml,
                            null,
                            DateTime.Now.AddSeconds(17),
                            Cache.NoSlidingExpiration
                            );

                        records = HttpRuntime.Cache.Get("RecordList");
                        //var jsonRecords = JsonConvert.SerializeObject(recordList);
                    }
                }

                return Json(new { code = 0, records = records.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        public void RemovedCallback(String k, Object v, CacheItemRemovedReason r)
        {
            
        }
    }
}
