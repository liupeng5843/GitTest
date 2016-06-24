using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EFDal.Context;
using EFDal.Dal;
using EFDal.Models;
using EFDal.Models.Enums;
using MHD.Original.Award.Models;
using MHD.Original.Award.ViewModels;
using MySql.Data.MySqlClient;
using SimpleComm.Files;

namespace MHD.Original.Award.Services
{
    public class BookComicService
    {
        private Original_comic_libraryContext context = new Original_comic_libraryContext(ContextType.originalRead);
        private Comic_libraryContext comiccontext = new Comic_libraryContext(ContextType.libRead);

        public bool IsIP(string ip)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
        public IList<original_book_info> get_voted_book_infos()
        {
            return context.original_book_info.Where(x => x.awardtype == 1 && !x.isdelete && !x.ispause).ToList();
        }

        public original_book_vote_info get_book_vote_info(int bookid)
        {
            return context.original_book_vote_info.FirstOrDefault(x => x.bookid == bookid && !x.isdelete && !x.ispause);
        }

        public IList<original_user_vote_log> get_user_vote_logs(int userid, int usertype)
        {
            return new List<original_user_vote_log>();
        }

        public IList<original_book_info> GetOriginalBookList(int pageNumber = 1, int count = 10)
        {
            using (context)
            {
                var start = (pageNumber - 1) * count;
                return context.original_book_info.Where(b => !b.isdelete && !b.ispause && b.isonshelf).Skip(start).Take(count).ToList();
            }
        }

        public IList<VoteBookViewModel> GetOriginalFinalists(int typeValue, DragonAwardType awardType, VoteRankType type, int topCount = 10)
        {
            using (var context = new Original_comic_libraryContext(ContextType.originalRead))
            {
                if (context.original_book_vote_finalist.Any(f => f.type == (int)type))
                {
                    var finallist = (from f in context.original_book_vote_finalist
                                     join b in context.original_book_info on f.bookid equals b.id
                                     where f.type == (int)type && b.secondawardtype == (short)awardType && !f.isdelete
                                     select f).ToList();
                    var maxValue = finallist.Count != 0 ? finallist.Max(f => f.typevalue) : 1;
                    typeValue = typeValue > maxValue ? maxValue : typeValue;//显示最近数据
                }

                var resultRankList = new List<VoteBookViewModel>();
                var totalRankList = (from bf in context.original_book_vote_finalist
                                     join bi in context.original_book_info on bf.bookid equals bi.id
                                     join pr in context.original_author_pseudonym_relation on bi.pseudonymid equals pr.id
                                     where bf.type == (int)type && bf.typevalue == typeValue && bi.isonshelf && !bf.isdelete && bi.secondawardtype == (short)awardType
                                     select new VoteBookViewModel
                                     {
                                         AwardType = (DragonAwardType)bi.secondawardtype,
                                         BookId = bf.bookid,
                                         AuthorId = bi.authorid,
                                         BookCoverUrl = bi.coverpath,
                                         BookName = bi.name,
                                         WeekScore = bf.finalscore,
                                         MonthScore = bf.finalscore,
                                         TotalScore = bf.finalscore,
                                         Pseudonym = pr.pseudonym,
                                     }).ToList();

                resultRankList = totalRankList.OrderByDescending(r => r.TotalScore).Take(topCount).ToList();

                if (type == VoteRankType.WeekRank)
                {
                    //resultRankList = totalRankList.OrderByDescending(r => r.WeekScore).Take(topCount).ToList();
                    resultRankList.ForEach(l =>
                    {
                        l.FansList = (from uv in context.original_user_vote_log
                                      where uv.bookid == l.BookId && uv.votestatus == (int)UserVoteStatus.Vaild
                                      group uv by uv.shareuserid into uvlist
                                      select new { uvlist.Key, SupportNumber = uvlist.Count() } into temp
                                      join ai in context.original_author_info on temp.Key equals ai.id
                                      //orderby temp.SupportNumber descending, ai.authorname descending
                                      select new FansViewModel
                                      {
                                          Name = String.IsNullOrEmpty(ai.authorname) ? "佚名" : ai.authorname,
                                          PortraitUrl = ai.coverurl,
                                          SupportScore = temp.SupportNumber,
                                          UserId = temp.Key
                                      }).ToList().OrderByDescending(a => a.SupportScore).ThenByDescending(a => a.Name).Take(3).ToList();
                    });
                }

                return resultRankList;
            }
        }

        public bool CreateVote(int userId, int bookId, int shareUserId = 0, string userHostAddress = "")
        {
            using (var context = new Original_comic_libraryContext(ContextType.originalCon))
            {
                try
                {
                    var startTime = DateTime.Now.AddHours(-DateTime.Now.Hour).AddMinutes(-DateTime.Now.Minute);
                    var voteStatus = CheckUserVoteStatus(bookId);

                    //此逻辑废除
                    //var voteCountToday = context.original_user_vote_log.Count(l => l.userid == userId && l.createtime > startTime);
                    //if (voteCountToday >= 5)
                    //{
                    //    return false;
                    //}
                    var todayStartTime = DateTime.Now.Date;
                    var voteCountByIP = context.original_user_vote_log.Count(l => l.ip == userHostAddress && l.createtime > todayStartTime);
                    if (voteCountByIP > 100)
                    {
                        return false;
                    }

                    var bookVoteInfo = context.original_book_vote_info.FirstOrDefault(b => b.bookid == bookId);
                    if (bookVoteInfo == null)
                    {
                        bookVoteInfo = new original_book_vote_info
                        {
                            bookid = bookId,
                            isdelete = false,
                            ispause = false,
                            weekscore = 0,
                            finalweekscore = 0,
                            totalscore = 0,
                            finaltotalscore = 0,
                            monthscore = 0,
                            finalmonthscore = 0,
                            praisescore = 0,
                            pinnedlevel = 100,
                            updatetime = DateTime.Now,
                            createtime = DateTime.Now,
                        };
                        context.original_book_vote_info.Add(bookVoteInfo);
                    }
                    //bookVoteInfo.weekscore++;
                    //bookVoteInfo.finalweekscore++;
                    //bookVoteInfo.monthscore++;
                    //bookVoteInfo.finalmonthscore++;
                    var totlaScore = context.original_user_vote_log.Count(v => v.bookid == bookId && !v.isdelete);
                    bookVoteInfo.finaltotalscore = bookVoteInfo.finaltotalscore >= totlaScore ? bookVoteInfo.finaltotalscore + 1 : totlaScore + 1;
                    //bookVoteInfo.finaltotalscore++;

                    context.SaveChanges();
                    var newBookVoteInfoId = context.original_book_vote_info.FirstOrDefault(b => b.bookid == bookId).id;

                    var userVote = new original_user_vote_log
                    {
                        bookid = bookId,
                        ip = userHostAddress,
                        shareuserid = shareUserId,
                        bookvoteinfoid = newBookVoteInfoId,
                        userid = userId,
                        createtime = DateTime.Now,
                        isdelete = false,
                        ispause = false,
                        type = (int)VoteLogType.Vote,
                        votestatus = (int)voteStatus,
                        usertype = 0
                    };
                    context.original_user_vote_log.Add(userVote);
                    context.SaveChanges();
                    

                    var autoAddLog = new original_user_vote_log_auto_add_log
                    {
                        ouvlid = userVote.id,
                        addnum = 3
                    };
                    if (userVote.userid > 0)
                    {
                        autoAddLog.addnum = 5;
                    }
                    context.original_user_vote_log_auto_add_log.Add(autoAddLog);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    LogHelper.WriteCustomLog("CreateVoteError", string.Format("errorInfo:{0}", ex.ToString()));
                    return false;
                }
            }
            return true;
        }

        public UserVoteStatus CheckUserVoteStatus(int bookId)
        {
            using (var db = new Original_comic_libraryContext(ContextType.originalRead))
            {
                //此处存在脏数据，因为onshelftime可能为空
                var bookTime = db.original_book_info.Find(bookId).onshelftime;
                if (bookTime == null)
                {
                    return UserVoteStatus.Invaild;
                }
                var bookPutawayTime = db.original_book_info.Find(bookId).onshelftime.Value;
                if (bookPutawayTime.Year == DateTime.Now.Year && bookPutawayTime.Month == DateTime.Now.Month)
                {
                    var onShelfTimeBucket = (double)bookPutawayTime.Day / 7;
                    var nowTimeBuket = (double)DateTime.Now.Day / 7;
                    var onShelfTimeRemainder = bookPutawayTime.Day % 7;
                    var nowTimeRemainder = DateTime.Now.Day % 7;

                    if ((onShelfTimeRemainder != 0 && nowTimeRemainder == 0 && (int)onShelfTimeBucket + 1 == (int)nowTimeBuket))
                    {
                        return UserVoteStatus.Vaild;
                    }
                    else if (onShelfTimeBucket == nowTimeBuket)
                    {
                        return UserVoteStatus.Vaild;
                    }
                    else if ((onShelfTimeRemainder != 0 && (int)onShelfTimeBucket == (int)nowTimeBuket) || nowTimeBuket > 3)
                    {
                        return UserVoteStatus.Vaild;
                    }
                }
                //if(bookPutawayTime!=null&& bookPutawayTime)
                return UserVoteStatus.Invaild;
            }
        }


        public int GetUserVoteVaildNum(int bookId)
        {
            using (var db = new Original_comic_libraryContext(ContextType.originalRead))
            {
                //此处存在脏数据，因为onshelftime可能为空
                var bookTime = db.original_book_info.Find(bookId).onshelftime;
                if (bookTime == null)
                {
                    return -1;
                }
                var bookPutawayTime = db.original_book_info.Find(bookId).onshelftime.Value;
                if (bookPutawayTime.Year == DateTime.Now.Year && bookPutawayTime.Month == DateTime.Now.Month)
                {
                    return DateTime.Now.Month;
                }
                //if(bookPutawayTime!=null&& bookPutawayTime)
                return -1;
            }
        }

        public IList<VoteBookViewModel> GetOriginalBookListByType(DragonAwardType awardType, int pageNumber = 1, int count = 10, bool isCurrentWeek = false)
        {
            using (var context = new Original_comic_libraryContext(ContextType.originalRead))
            {
                var originalBookList = (from bi in context.original_book_info
                                        join pr in context.original_author_pseudonym_relation on bi.pseudonymid equals pr.id
                                        join bv in context.original_book_vote_info on bi.id equals bv.bookid into temp
                                        from tt in temp.DefaultIfEmpty()
                                        where bi.secondawardtype == (short)awardType && bi.isonshelf
                                        orderby bi.createtime descending
                                        select new VoteBookViewModel
                                        {
                                            AuthorId = bi.authorid,
                                            BookCoverUrl = bi.coverpath,
                                            BookId = bi.id,
                                            BookName = bi.name,
                                            TotalScore = tt == null ? 0 : tt.finaltotalscore,
                                            PinnedLevel = tt == null ? 100 : tt.pinnedlevel,
                                            Pseudonym = pr.pseudonym,
                                            Brief = bi.brief,
                                            OnShelfTime = bi.onshelftime != null ? bi.onshelftime.Value : DateTime.MinValue,
                                        }).ToList();

                if (isCurrentWeek)
                {
                    var currentDay = DateTime.Now.Day;
                    while (currentDay > 0)
                    {
                        if (currentDay > 21)
                        {
                            var startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                            originalBookList = originalBookList.Where(b => b.OnShelfTime > startTime).ToList();
                        }
                        else if (currentDay > 14 && currentDay <= 21)
                        {
                            var startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 15);
                            var entTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 22);
                            originalBookList = originalBookList.Where(b => b.OnShelfTime > startTime && b.OnShelfTime < entTime).ToList();
                        }
                        else if (currentDay > 7 && currentDay <= 14)
                        {
                            var startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 8);
                            var entTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 15);
                            originalBookList = originalBookList.Where(b => b.OnShelfTime > startTime && b.OnShelfTime < entTime).ToList();
                        }
                        else if (currentDay >= 1 && currentDay <= 7)
                        {
                            var startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                            var entTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 8);
                            originalBookList = originalBookList.Where(b => b.OnShelfTime > startTime && b.OnShelfTime < entTime).ToList();
                        }
                        //保证获取到本月数据
                        if (originalBookList.Count > 0)
                        {
                            break;
                        }
                        else
                        {
                            currentDay = currentDay - 7;
                        }
                    }
                    originalBookList = originalBookList.OrderBy(b => b.PinnedLevel).Skip((pageNumber - 1) * count).Take(count).ToList();
                }
                else
                {
                    originalBookList = originalBookList.OrderByDescending(b => b.TotalScore).Skip((pageNumber - 1) * count).Take(count).ToList();
                }

                return originalBookList;
            }
        }
        public int GetOriginalBookCountByType(DragonAwardType awardType, bool isCurrentWeek = false)
        {
            using (var context = new Original_comic_libraryContext(ContextType.originalRead))
            {
                var originalBookList = context.original_book_info.Where(b => b.secondawardtype == (short)awardType && !b.isdelete && !b.ispause && b.isonshelf).ToList();
                var count = originalBookList.Count();

                if (isCurrentWeek)
                {
                    var currentDay = DateTime.Now.Day;
                    if (currentDay > 21)
                    {
                        var startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                        count = originalBookList.Where(b => b.onshelftime > startTime).Count();
                    }
                    else if (currentDay > 14 && currentDay <= 21)
                    {
                        var startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 15);
                        var entTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 22);
                        count = originalBookList.Where(b => b.onshelftime > startTime && b.onshelftime < entTime).Count();
                    }
                    else if (currentDay > 7 && currentDay <= 14)
                    {
                        var startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 8);
                        var entTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 15);
                        count = originalBookList.Where(b => b.onshelftime > startTime && b.onshelftime < entTime).Count();
                    }
                    else if (currentDay >= 1 && currentDay <= 7)
                    {
                        var startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                        var entTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 8);
                        count = originalBookList.Where(b => b.onshelftime > startTime && b.onshelftime < entTime).Count();
                    }
                }

                return count;
            }
        }

        public int GetOriginalBookCountByKeywords(string keywords = "")
        {

            using (var context = new Original_comic_libraryContext(ContextType.originalRead))
            {
                var count = (from bi in context.original_book_info
                             join pr in context.original_author_pseudonym_relation on bi.pseudonymid equals pr.id
                             where (bi.name.Contains(keywords) || pr.pseudonym.Contains(keywords)) && !bi.isdelete && !bi.ispause && bi.isonshelf && bi.secondawardtype != (short)DragonAwardType.None
                             select bi
                             ).Count();
                return count;
            }
        }


        public IList<VoteBookViewModel> GetOriginalBookListByKeywords(string keywords, int pageNumber = 1, int count = 10)
        {
            using (var context = new Original_comic_libraryContext(ContextType.originalRead))
            {

                var originalBookList = (from bi in context.original_book_info
                                        join pr in context.original_author_pseudonym_relation on bi.pseudonymid equals pr.id
                                        join bv in context.original_book_vote_info on bi.id equals bv.bookid into temp
                                        from tt in temp.DefaultIfEmpty()
                                        where (bi.name.Contains(keywords) || pr.pseudonym.Contains(keywords)) && bi.isonshelf && !bi.isdelete && !bi.ispause && bi.secondawardtype != (short)DragonAwardType.None
                                        select new VoteBookViewModel
                                        {
                                            AuthorId = bi.authorid,
                                            BookCoverUrl = bi.coverpath,
                                            BookId = bi.id,
                                            BookName = bi.name,
                                            TotalScore = tt == null ? 0 : tt.finaltotalscore,
                                            PinnedLevel = tt == null ? 0 : tt.pinnedlevel,
                                            Pseudonym = pr.pseudonym,
                                            Brief = bi.brief,
                                        }).ToList();

                return originalBookList.OrderByDescending(b => b.PinnedLevel).Skip((pageNumber - 1) * count).Take(count).ToList();

            }
        }


        public IList<VoteBookViewModel> GetMBookListByKeywords(string keywords, int pageNumber = 1, int count = 10)
        {
            using (var context = new Original_comic_libraryContext(ContextType.originalRead))
            {
                var originalBookList = (from bi in context.original_book_info
                                        join pr in context.original_author_pseudonym_relation on bi.pseudonymid equals pr.id
                                        join bv in context.original_book_vote_info on bi.id equals bv.bookid into temp
                                        from tt in temp.DefaultIfEmpty()
                                        where bi.name.Contains(keywords) && bi.isonshelf && !bi.isdelete && !bi.ispause && bi.secondawardtype != (short)DragonAwardType.None
                                        select new VoteBookViewModel
                                        {
                                            AuthorId = bi.authorid,
                                            BookCoverUrl = bi.coverpath,
                                            BookId = bi.id,
                                            BookName = bi.name,
                                            TotalScore = tt == null ? 0 : tt.finaltotalscore,
                                            PinnedLevel = tt == null ? 0 : tt.pinnedlevel,
                                            Pseudonym = pr.pseudonym,
                                            Brief = bi.brief,
                                        }).ToList();

                return originalBookList.OrderByDescending(b => b.PinnedLevel).Skip((pageNumber - 1) * count).Take(count).ToList();

            }
        }


        public VoteBookViewModel GetOriginalBookById(int bookId)
        {
            using (var context = new Original_comic_libraryContext(ContextType.originalRead))
            {
                var bigBookThirdPartyUrl = context.Database.SqlQuery<string>(@"select cbb.thirdpartyurl
                                                                            from comic_library.book_info cb 
                                                                            join original_comic_library.original_originalbook_book_relation oob on oob.bookid = cb.id
                                                                            join comic_library.bigbook_book_relation bbr on  bbr.bookid =cb.id
                                                                            join comic_library.bigbook_info cbb on cbb.id = bbr.bigbookid
                                                                            where oob.originalbookid=@bookid",
                                                                            new MySqlParameter[] { new MySqlParameter("@bookid", bookId) }).FirstOrDefault();
                var originalBook = (from bi in context.original_book_info
                                    join pr in context.original_author_pseudonym_relation on bi.pseudonymid equals pr.id
                                    join bv in context.original_book_vote_info on bi.id equals bv.bookid into temp
                                    from tt in temp.DefaultIfEmpty()
                                    where bi.id == bookId
                                    select new VoteBookViewModel
                                     {
                                         AuthorId = bi.authorid,
                                         BookCoverUrl = bi.coverpath,
                                         BookId = bi.id,
                                         BookName = bi.name,
                                         TotalScore = tt == null ? 0 : tt.finaltotalscore,
                                         LastChapterName = bi.lastpartname,
                                         Brief = bi.brief,
                                         Pseudonym = pr.pseudonym,
                                         ThirdPartyUrl = bigBookThirdPartyUrl ?? string.Empty,
                                     }).FirstOrDefault();

                if (string.IsNullOrEmpty(originalBook.ThirdPartyUrl))
                {
                    originalBook.ChapterList = (from p in context.original_part_info
                                                where p.bookid == originalBook.BookId && !p.ispause && !p.isdelete && p.isonshelf
                                                orderby p.partnumber ascending
                                                select new ChapterViewModel
                                                {
                                                    Id = p.id,
                                                    Name = p.name,
                                                    PartNumber = p.partnumber,
                                                    TotalNumber = p.totalpage
                                                }).ToList();
                }

                return originalBook;
            }
        }

        public ChapterViewModel GetOriginalChapterById(int chapterId)
        {
            using (var context = new Original_comic_libraryContext(ContextType.originalRead))
            {
                var chapter = context.original_part_info.Find(chapterId);
                if (chapter != null)
                {
                    var chapterViewModel = new ChapterViewModel { PartNumber = chapter.partnumber, Name = chapter.name, TotalNumber = chapter.totalpage, Id = chapter.id, BookId = chapter.bookid };
                    chapterViewModel.PageList = (from p in context.original_page_info
                                                 where p.partid == chapterId && !p.isdelete && !p.ispause
                                                 orderby p.pagenumber ascending
                                                 select new PageViewModel
                                                 {
                                                     Id = p.id,
                                                     PageUrl = p.pageurl,
                                                     PageNumber = p.pagenumber
                                                 }).ToList();
                    var prevChapter = context.original_part_info.FirstOrDefault(p => p.partnumber == chapter.partnumber - 1 && !p.isdelete && !p.ispause && p.bookid == chapter.bookid);
                    if (prevChapter != null)
                    {
                        chapterViewModel.PrevPartId = prevChapter.id;
                    }
                    var nextChapter = context.original_part_info.FirstOrDefault(p => p.partnumber == chapter.partnumber + 1 && !p.isdelete && !p.ispause && p.bookid == chapter.bookid);
                    if (nextChapter != null)
                    {
                        chapterViewModel.NextPartId = nextChapter.id;
                    }
                    return chapterViewModel;
                }
                return null;
            }
        }

        public int InputComicBooks(string comicBookIdListStr, DragonAwardType awardType)
        {
            var comicBookIdList = comicBookIdListStr.Split(',');
            using (var orgcontext = new Original_comic_libraryContext(ContextType.originalCon))
            {
                using (var libcontext = new Comic_libraryContext(ContextType.libRead))
                {
                    using (var tran = libcontext.Database.BeginTransaction())
                    {
                        try
                        {
                            foreach (var comicBookIdStr in comicBookIdList)
                            {
                                var comicBookId = int.Parse(comicBookIdStr);
                                var comicBook = libcontext.book_info.Find(comicBookId);

                                if (comicBook == null)
                                {
                                    LogHelper.WriteCustomLog("导入漫画岛数据TranError", "该漫画不存在");
                                    return -1;
                                }

                                var originalBookRelation = orgcontext.original_originalbook_book_relation.Where(r => r.bookid == comicBookId).FirstOrDefault();

                                var originalAuthor = new original_author_info
                                {
                                    address = string.Empty,
                                    authorname = comicBook.author,
                                    backupcoverurl = string.Empty,
                                    contactemail = "id",
                                    coverurl = "http://mhd.ufile.ucloud.cn/upload/original/book_267/bookcover_267_dc8cd6ee-66d1-49bd-b738-3d116562023b.jpg",
                                    gender = false,
                                    email = Guid.NewGuid().ToString(),
                                    idnumber = string.Empty,
                                    state = 0,
                                    isdelete = false,
                                    ispause = false,
                                    createtime = DateTime.Now,
                                    issign = false,
                                    lastupdateip = string.Empty,
                                    password = "123456",
                                    qq = string.Empty,
                                    platform = 0,
                                    userid = 0,
                                    phonenumber = string.Empty,
                                    updatetime = DateTime.Now,
                                };

                                orgcontext.original_author_info.Add(originalAuthor);
                                orgcontext.SaveChanges();

                                var originalPseudonym = new original_author_pseudonym_relation
                                {
                                    authorid = originalAuthor.id,
                                    createtime = DateTime.Now,
                                    isdelete = false,
                                    pseudonym = originalAuthor.authorname,
                                    ispause = false,
                                };

                                orgcontext.original_author_pseudonym_relation.Add(originalPseudonym);
                                orgcontext.SaveChanges();

                                var originalComicBook = new original_book_info();
                                if (originalBookRelation == null)
                                {
                                    Random ra = new Random();
                                    originalComicBook = new original_book_info
                                    {
                                        author = comicBook.author,
                                        authorid = originalAuthor.id,//这个要考虑加
                                        awardtype = 1,
                                        secondawardtype = (short)awardType,
                                        brief = comicBook.brief,
                                        coverpath = comicBook.coverurl,
                                        createtime = DateTime.Now,
                                        directoryname = string.Empty,
                                        isdelete = false,
                                        isonshelf = true,
                                        ispause = false,
                                        keywords = comicBook.keyword,
                                        subjectid = (int)comicBook.subjectid,
                                        name = comicBook.name,
                                        totalpart = (short)comicBook.totalpart,
                                        lastpartname = comicBook.lastpartname,
                                        progresstype = (short)comicBook.progresstype,
                                        onshelftime = DateTime.Now,
                                        publishstate = 0,
                                        state = 6,
                                        updateplanid = 0,
                                        sizetype = (short)comicBook.sizetype,
                                        pseudonymid = originalPseudonym.id,//笔名要不要创建
                                        issignbook = true,
                                        thirdpartyurl = string.Empty
                                    };
                                    //添加原创书籍
                                    orgcontext.original_book_info.Add(originalComicBook);
                                    orgcontext.SaveChanges();

                                    //var comicBigBookId=libcontext.bigbook_book_relation.Where(r=>r.bookid==comicBookId).Select(r=>r.bigbookid).FirstOrDefault();

                                    originalBookRelation = new original_originalbook_book_relation
                                    {
                                        bookid = comicBookId,
                                        originalbookid = originalComicBook.id,
                                        createtime = DateTime.Now,
                                        isdelete = false,
                                        ispause = false,
                                        updatetime = DateTime.Now,
                                    };

                                    orgcontext.original_originalbook_book_relation.Add(originalBookRelation);
                                    orgcontext.SaveChanges();
                                }
                                else
                                {
                                    originalComicBook = orgcontext.original_book_info.Find(originalBookRelation.originalbookid);
                                }
                                //var isInputed= orgcontext.original_part_info.Any(p => p.bookid == originalComicBook.id);
                                //存在章节即继续循环
                                if (orgcontext.original_part_info.Any(p => p.bookid == originalComicBook.id))
                                {
                                    continue;
                                }
                                var comicChapterList = libcontext.part_info.Where(p => p.bookid == comicBookId && !p.isdelete && !p.ispause).ToList();
                                foreach (var chapter in comicChapterList)
                                {
                                    var originalChapter = new original_part_info
                                    {
                                        bookid = originalComicBook.id,
                                        name = chapter.name,
                                        partnumber = (short)(chapter.partnumber - 1),
                                        state = 6,
                                        replacepartid = 0,
                                        totalpage = (short)chapter.totalpage,
                                        partsize = chapter.partsize,
                                        isdelete = false,
                                        isonshelf = true,
                                        ispause = false,
                                        islastpart = false,
                                        createtime = DateTime.Now,
                                    };
                                    orgcontext.original_part_info.Add(originalChapter);
                                    orgcontext.SaveChanges();

                                    var pageList = libcontext.page_info.Where(p => p.partid == chapter.id).ToList();
                                    foreach (var page in pageList)
                                    {
                                        var domain = libcontext.domainconfig.Find(page.domainid).domain;
                                        var originalPageInfo = new original_page_info
                                        {
                                            bookid = originalComicBook.id,
                                            pagenumber = page.pagenumber,
                                            pageurl = string.Format(@"{0}{1}", domain, page.pageurl),
                                            partid = originalChapter.id,
                                            isdelete = false,
                                            ispause = false,
                                            createtime = DateTime.Now,
                                        };
                                        orgcontext.original_page_info.Add(originalPageInfo);
                                    }
                                    orgcontext.SaveChanges();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.WriteCustomLog("导入漫画岛数据TranError", ex.ToString());
                            tran.Rollback();
                            throw;
                        }
                        tran.Commit();
                    }
                }
            }
            return comicBookIdList.Count();
        }
    }
}