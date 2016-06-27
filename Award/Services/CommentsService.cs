using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EFDal.Context;
using EFDal.Dal;
using EFDal.Models;
using MHD.Original.Award.ViewModels;

namespace MHD.Original.Award.Services
{
    public class CommentsService
    {
        public bool CreateComment(original_discuss_info comment)
        {
            using (var db = new Original_comic_libraryContext(ContextType.originalCon))
            {
                if (comment.authorid > 0)
                {
                    comment.name = db.original_author_info.Find(comment.authorid).authorname;
                }

                var lastComment = db.original_discuss_info.Where(d => d.name == comment.name && d.authorid == comment.authorid).OrderByDescending(d => d.createtime).FirstOrDefault();
                if (lastComment != null)
                {
                    if (comment.createtime.Ticks - lastComment.createtime.Ticks < 30 * 10000000)
                    {
                        return false;
                    }
                }

                db.original_discuss_info.Add(comment);
                return db.SaveChanges() >= 1;
            }
        }

        public List<CommentViewModel> GetCommentList(int pageNumber = 1, int count = 10)
        {
            using (var db = new Original_comic_libraryContext(ContextType.originalRead))
            {
                var anonymityCommentList = (from d in db.original_discuss_info
                                            where !d.isdelete && !d.ispause && d.authorid == 0
                                            select new CommentViewModel
                                            {
                                                id = d.id,
                                                authorid = d.authorid,
                                                content = d.content,
                                                name = d.name,
                                                parentid = d.parentid,
                                                PortraitUrl = "../Content/img/user_head.png",
                                                UserName = d.name,
                                                createtime = d.createtime,
                                                pinnedtime = d.pinnedtime,
                                                isdelete = d.isdelete,
                                                ispause = d.ispause,
                                            }).ToList();

                var autonymCommentList = (from d in db.original_discuss_info
                                          join a in db.original_author_info on d.authorid equals a.id
                                          select new CommentViewModel
                                          {
                                              id = d.id,
                                              authorid = d.authorid,
                                              content = d.content,
                                              name = d.name,
                                              parentid = d.parentid,
                                              PortraitUrl = a.coverurl.Contains("http://")? a.coverurl:"http://www.manhuadao.cn"+a.coverurl.Replace(@"\",@"/"),
                                              UserName = a.authorname,
                                              createtime = d.createtime,
                                              pinnedtime=d.pinnedtime,
                                              isdelete=d.isdelete,
                                              ispause=d.ispause,
                                          }).ToList();
                anonymityCommentList.AddRange(autonymCommentList);

                var parentCommentList = anonymityCommentList.Where(c => c.parentid == 0).OrderByDescending(c => c.pinnedtime).ThenByDescending(c => c.createtime).Skip((pageNumber - 1) * count).Take(count).ToList();

                parentCommentList.ForEach(c =>
                {
                    c.ChildrenComment = anonymityCommentList.Where(ac => ac.parentid == c.id && !ac.isdelete && !ac.ispause).OrderByDescending(ac => ac.createtime).Take(100).ToList(); 
                });

                return parentCommentList;
            }
        }

        public int GetCommentPageCount(int count = 10)
        {
            using (var db = new Original_comic_libraryContext(ContextType.originalRead))
            {
                var commentCount = db.original_discuss_info.Count(d => d.parentid == 0&&!d.isdelete &&!d.ispause);
                
                return commentCount;
            }
        }
    }
}