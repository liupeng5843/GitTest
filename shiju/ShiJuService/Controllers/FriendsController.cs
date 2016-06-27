using ShiJu.Common;
using ShiJu.Models;
using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThirdPartNotification.JPushNotification;
using WindowsServer.Json;

namespace ShiJu.Service.Controllers
{
    public class FriendsController : Controller
    {
        [Route("Users/{userId}/Friends")]
        public ActionResult GetUserFriends(Guid userId)
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                var friendUsers = (from u in db.Users.AsNoTracking()
                                   join f in db.Friends on u.Id equals f.FriendUserId 
                                   where f.UserId == userId
                                   select u).Union(from u in db.Users
                                                   join f in db.Friends on u.Id equals f.UserId
                                                   where f.FriendUserId == userId
                                                   select u).OrderBy(u => u.NickName).Distinct().ToList();

                var writer = new JsonWriter();
                ShiJu.Models.User.Serialize(writer, friendUsers, Formats.UserBrief);
                return this.JsonResponseJson(0, "FriendUsers", writer.ToString());
            }
        }

        [Route("Users/{userId}/FriendRequests/UnreadCount")]
        public ActionResult GetUserFriendRequestsUnreadCount(Guid userId)
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                var count = (from fr in db.FriendRequests
                             where (fr.TargetUserId == userId) && fr.TargetUnread
                             select fr).Count();

                //count += (from fr in db.FriendRequests
                //          where (fr.TargetUserId == userId) && fr.SourceUnread
                //          select fr).Count();

                return this.JsonResult(0, "Succeeded", "UnreadCount", count);
            }
        }


        [HttpPost]
        [Route("Users/{sourceUserId}/FriendRequests/{targetUserId}/UpdateStatus")]
        public ActionResult UpdateUserFriendRequestsUnread(Guid sourceUserId, Guid targetUserId, FriendRequestStatus friendRequestStatus)
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                var friendRequest = db.FriendRequests.Where(fr => fr.SourceUserId == sourceUserId && fr.TargetUserId == targetUserId).OrderByDescending(fr => fr.CreatedTime).FirstOrDefault();
                if (friendRequest != null)
                {
                    friendRequest.Status = friendRequestStatus;
                    if (friendRequestStatus == FriendRequestStatus.Approved)
                    {
                        var exist = db.Friends.Where(f => (f.UserId == sourceUserId && f.FriendUserId == targetUserId)).Any();
                        if (!exist)
                        {
                            var friend = new Friend() { UserId = sourceUserId, FriendUserId = targetUserId, CreatedTime = DateTime.UtcNow };
                            db.Friends.Add(friend);
                        }
                        else
                        {
                            return this.JsonResult(-2, "this record has been exist!");
                        }
                    }
                }
                else
                {
                    return this.JsonResult(-1, "this friendRequeset is not exist!");
                }
                db.SaveChanges();
                return this.JsonResult(0, "Succeeded");
            }
        }

        [HttpPost]
        [Route("Users/{userId}/FriendRequests/MarkRead")]
        public ActionResult MarkReadForUserFriendRequests(Guid userId)
        {
            int count = 0;
            using (var db = Heart.CreateShiJuDbContext())
            {
                var friendRequests = (from fr in db.FriendRequests
                                      where ((fr.SourceUserId == userId) && fr.SourceUnread) || ((fr.TargetUserId == userId) && fr.TargetUnread)
                                      select fr);
                foreach (var request in friendRequests)
                {
                    if (request.SourceUserId == userId)
                    {
                        request.SourceUnread = false;
                    }

                    if (request.TargetUserId == userId)
                    {
                        request.TargetUnread = false;
                    }

                    count++;
                }

                db.SaveChanges();
            }

            return this.JsonResult(0, "Succeeded", "Count", count);
        }

        [HttpPost]
        [Route("Users/{userId}/FriendRequests/Create")]
        public ActionResult CreateUserFriendRequest(Guid userId)
        {
            var json = this.GetRequestContentJson() as JsonObject;
            var friendRequest = FriendRequest.Deserialize(json.GetJsonObject("FriendRequest"));
            if (friendRequest.SourceUserId != userId)
            {
                return this.JsonResult(-1, "You must send a friend request as yourself");
            }

            friendRequest.Status = FriendRequestStatus.Created;
            friendRequest.SourceUnread = false;
            friendRequest.TargetUnread = true;
            friendRequest.CreatedTime = DateTime.UtcNow;

            using (var db = Heart.CreateShiJuDbContext())
            {
                // If the FriendRequest exist, delete it first
                var existingFriendRequest = (from fr in db.FriendRequests
                                             where (fr.SourceUserId == friendRequest.SourceUserId) && (fr.TargetUserId == friendRequest.TargetUserId)
                                             select fr).FirstOrDefault();
                if (existingFriendRequest != null)
                {
                    existingFriendRequest.Text = friendRequest.Text;
                    existingFriendRequest.Status = FriendRequestStatus.Created;
                    existingFriendRequest.SourceUnread = false;
                    existingFriendRequest.TargetUnread = true;
                }
                else 
                {
                    db.FriendRequests.Add(friendRequest);
                }

                db.SaveChanges();
                
                var extraKeyValues = new Dictionary<string, string>();
                extraKeyValues.Add("Type", ((long)NotificationType.FriendRequest).ToString());
                extraKeyValues.Add("Description", "FriendRequest");

                var needNotification = db.Users.Find(friendRequest.TargetUserId).NeedNotification;
                if (needNotification)
                {
                    JPushNotificationManager.PushNotification(ShiJuApp.AppModuleId, new List<Guid>() { friendRequest.TargetUserId }, 1, "你有新的好友请求", extraKeyValues);
                }
            }
          
            var writer = new JsonWriter();
            FriendRequest.Serialize(writer, friendRequest, Formats.FriendRequestDetail);

            return this.JsonResponseJson(0, "FriendRequest", writer.ToString());
        }

        [HttpPost]
        [Route("User/{userId}/Friend/{friendUserId}/Delete")]
        public ActionResult DelteFriend(Guid userId, Guid friendUserId)
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                var friends = db.Friends.Where(f => (f.UserId == userId && f.FriendUserId == friendUserId) || (f.UserId == friendUserId && f.FriendUserId == userId)).ToList();
                if (friends.Count != 0)
                {
                    db.Friends.RemoveRange(friends);
                    db.SaveChanges();
                }

                return this.JsonResult(0, "Delete Succeeded");
            }
        }

        [Route("Users/{userId}/FriendRequests")]
        public ActionResult GetUserFriendRequests(Guid userId, int start = 0, int count = 20)
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                var friendRequestsQuery = (from fr in db.FriendRequests.AsNoTracking()
                                           where (fr.TargetUserId == userId) && fr.Status==FriendRequestStatus.Created
                                           orderby fr.CreatedTime descending
                                           select fr);
                var friendRequestCount = friendRequestsQuery.Count();
                var friendRequests = friendRequestsQuery.Skip(start).Take(count).ToList();
                foreach (var fr in friendRequests)
                {
                    fr.SourceUser = db.Users.AsNoTracking().Where(u => u.Id == fr.SourceUserId).FirstOrDefault();
                }

                var writer = new JsonWriter();
                FriendRequest.Serialize(writer, friendRequests, Formats.FriendRequestDetail);

                var extraJsons = new Dictionary<string, string>();
                extraJsons["FriendRequestCount"] = friendRequestCount.ToString();
                extraJsons["FriendRequests"] = writer.ToString();

                return this.JsonResponseJson(0, "Succeeded", extraJsons);
            }
        }
    }
}
