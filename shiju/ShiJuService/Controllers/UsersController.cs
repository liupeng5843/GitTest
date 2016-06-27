using ShiJu.Models;
using ShiJu.Service.Models;
using ShiJu.Utils;
using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using WindowsServer.Caching;
using WindowsServer.Json;
using WindowsServer.Log;

namespace ShiJu.Service.Controllers
{
    public class UsersController : Controller
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        [Route("Users/{userId}")]
        public ActionResult GetUserDetail(Guid userId)
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                var user = db.Users.Find(userId);
                if (user == null)
                {
                    return this.JsonResult(-1, "The user does not exist.");
                }

                var writer = new JsonWriter();
                ShiJu.Models.User.Serialize(writer, user, Formats.UserDetail);
                return this.JsonResponseJson(0, "User", writer.ToString());
            }
        }

        [Route("Users/{userId}/Friendship/{friendUserId}")]
        public ActionResult GetUserDetail(Guid userId, Guid friendUserId)
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                var user = db.Users.Find(userId);
                var friend = db.Users.Find(friendUserId);

                if (user == null || friend == null)
                {
                    return this.JsonResult(-1, "The user does not exist.");
                }

                friend.IsFriend = db.Friends.Where(f => (f.UserId == userId && f.FriendUserId == friendUserId) || (f.UserId == friendUserId && f.FriendUserId == userId)).Any();

                var writer = new JsonWriter();
                ShiJu.Models.User.Serialize(writer, friend, Formats.UserDetail);
                return this.JsonResponseJson(0, "User", writer.ToString());
            }
        }

        [Route("Users/{userId}/Update")]
        [HttpPost]
        public ActionResult UpdateUser(Guid userId)
        {
            var json = this.GetRequestContentJson() as JsonObject;
            var newUser = ShiJu.Models.User.Deserialize(json.GetJsonObject("User"));

            using (var db = Heart.CreateShiJuDbContext())
            {
                var user = db.Users.Find(userId);
                if (user == null)
                {
                    return this.JsonResult(-1, "The user does not exist.");
                }

                // Update
                user.NickName = newUser.NickName;
                user.Gender = newUser.Gender;
                user.Signature = newUser.Signature;
                user.Portrait = newUser.Portrait;
                user.PhoneNumber = newUser.PhoneNumber;
                user.District = newUser.District;
                user.BackgroundImage = newUser.BackgroundImage;
                db.SaveChanges();

                var writer = new JsonWriter();
                ShiJu.Models.User.Serialize(writer, user, Formats.UserDetail);
                return this.JsonResponseJson(0, "User", writer.ToString());
            }
        }

        [Route("Users/Search")]
        public ActionResult SearchUsers(string keywords, int start = 0, int count = 20)
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                var search = (from u in db.Users
                              where (u.PhoneNumber.Contains(keywords) || u.NickName.Contains(keywords)) && (u.Status == ShiJu.Models.UserStatus.Active)
                              orderby u.CreatedTime descending
                              select u);
                var userCount = search.Count();
                var users = search.Skip(start).Take(count).ToList();

                var writer = new JsonWriter();
                ShiJu.Models.User.Serialize(writer, users, Formats.UserBrief);

                var extraJsons = new Dictionary<string, string>();
                extraJsons["Count"] = userCount.ToString();
                extraJsons["Users"] = writer.ToString();
                return this.JsonResponseJson(0, "Succeeded", extraJsons);
            }

        }

        //the new add by archer
        [Route("Users/{userId}/{phoneNumber}/Invite")]
        public ActionResult InviteUserByPhoneNumber(Guid userId, string phoneNumber)
        {
            var text = string.Empty;
            using (var db = Heart.CreateShiJuDbContext())
            {
                var sourceUser = db.Users.FirstOrDefault(u => u.Id == userId);
                text = string.Format(Heart.SmsInviteFriendTemplate, sourceUser.NickName);
                var targetUser = db.Users.Where(u => u.PhoneNumber == phoneNumber).FirstOrDefault();
                //var targetAccount = db.Accounts.FirstOrDefault(a => a.Name == phoneNumber);
                if (targetUser != null && userId == targetUser.Id)
                {
                    return this.JsonResult(-1, "you can not invite yourself.");
                }
                var targetUserId = Guid.Empty;
                if (targetUser == null)
                {
                    var newUser = new User()
                     {
                         Id = Guid.NewGuid(),
                         PhoneNumber = phoneNumber,
                         CreatedTime = DateTime.UtcNow,
                         Status = UserStatus.Inactive,
                         NickName = "",
                         Signature = "",
                         Portrait = Guid.Empty,
                         BackgroundImage = Guid.Empty,
                         District = "",
                         SignUpTime = DateTime.UtcNow
                     };
                    targetUserId = newUser.Id;
                    db.Users.Add(newUser);
                }
                else
                {
                    targetUserId = targetUser.Id;
                }

                var friendRequestExist = db.FriendRequests.Where(f => f.SourceUserId == userId && f.TargetUserId == targetUserId).Any();
                if (!friendRequestExist)
                {
                    var friendRequest = new FriendRequest
                    {
                        CreatedTime = DateTime.UtcNow,
                        SourceUnread = true,
                        SourceUserId = userId,
                        TargetUserId = targetUserId,
                        Text = "Hello",
                        Status = FriendRequestStatus.Created
                    };
                    db.FriendRequests.Add(friendRequest);
                }

                db.SaveChanges();
            }

            try
            {
                var respCode = SmsUtility.sendOnce(phoneNumber, text);
                _logger.Info("Send message return code:" + respCode);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error on sending message to " + phoneNumber, ex);
            }

            return this.JsonResult(0, "invite friend successfully.");
        }

        [HttpPost]
        [Route("Users/{userId}/PhoneNumberList/Status")]
        public ActionResult GetUsersStatus(Guid userId)
        {
            var jsonArray = this.GetRequestContentJson() as JsonArray;
            var users = ShiJu.Models.User.Deserialize(jsonArray);
            var userList = new List<User>() { };
            var phoneNumberList = jsonArray.ToList();
            using (var db = Heart.CreateShiJuDbContext())
            {
                foreach (var virtualUser in users)
                {
                    if (db.Users.Where(u => u.PhoneNumber == virtualUser.PhoneNumber).Any())
                    {
                        var user = new User();
                        user.Id = db.Users.Where(u => u.PhoneNumber == virtualUser.PhoneNumber).Select(u => u.Id).FirstOrDefault();
                        user.PhoneNumber = virtualUser.PhoneNumber;
                        user.Status = db.Users.Where(u => u.PhoneNumber == virtualUser.PhoneNumber).Select(u => u.Status).FirstOrDefault();
                        user.NickName = virtualUser.NickName;
                        userList.Add(user);
                    }
                    else
                    {
                        var user = new User();
                        user.Id = new Guid();
                        user.PhoneNumber = virtualUser.PhoneNumber;
                        user.Status = UserStatus.Inexsitence;
                        user.NickName = virtualUser.NickName;
                        userList.Add(user);
                    }
                }
            }
            var writer = new JsonWriter();
            ShiJu.Models.User.Serialize(writer, userList, Formats.UserBrief);

            var extraJsons = new Dictionary<string, string>();
            extraJsons["Users"] = writer.ToString();
            return this.JsonResponseJson(0, "Succeeded", extraJsons);
        }



        [Route("Users/{userId}/Participants")]
        public ActionResult getUserParticipants(Guid userId, ParticipantStatus participantStatus, int start = 0, int count = 20, bool isApplaction = false)
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                //var participants = db.Participants.Where(p => p.UserId == userId && p.Status == participantStatus).ToList();
                List<Participant> participantList = new List<Participant>();

                if (isApplaction)//真实用户创建活动
                {
                    var utcNow = DateTime.UtcNow;
                    var parties = db.Parties.AsNoTracking().Where(p => p.CreatorUserId == userId && p.EndTime > utcNow).ToList();
                    foreach (var party in parties)
                    {
                        var list = db.Participants.AsNoTracking().Where(p => p.PartyId == party.Id && p.Status == ParticipantStatus.Enrolled).ToList();
                        participantList.AddRange(list);
                    }
                }
                else
                {
                    participantList = db.Participants.AsNoTracking().Where(p => p.UserId == userId && p.Status == participantStatus).ToList();
                }

                foreach (var participant in participantList)
                {
                    participant.User = db.Users.AsNoTracking().Where(u => u.Id == participant.UserId).FirstOrDefault();
                    participant.Party = db.Parties.AsNoTracking().Where(p => p.Id == participant.PartyId).FirstOrDefault();
                    participant.Party.CreatorUser = db.Users.AsNoTracking().Where(u => u.Id == participant.Party.CreatorUserId).FirstOrDefault();
                }

                participantList = participantList.OrderByDescending(p => p.CreatedTime).Skip(start).Take(count).ToList();

                var writer = new JsonWriter();
                ShiJu.Models.Participant.Serialize(writer, participantList, Formats.ParticipantDetial);

                var extraJsons = new Dictionary<string, string>();
                extraJsons["Participants"] = writer.ToString();
                //return this.JsonResponseJson(0, "Succeeded", extraJsons);
                return this.JsonResponseJson(0, "Succeeded", extraJsons);
            }
        }

        [Route("Users/{userId}/NeedNotification")]
        public ActionResult UpdateNeedNotification(Guid userId, Boolean needNotification)
        {
            User user;
            using (var db = Heart.CreateShiJuDbContext())
            {
                user = db.Users.Where(u => u.Id == userId).FirstOrDefault();
                if (user == null)
                {
                    return this.JsonResult(-1, "The user cannot be found.");
                }

                if (needNotification)
                {
                    user.NeedNotification = true;
                }
                else
                {
                    user.NeedNotification = false;
                }

                db.SaveChanges();
            }

            var writer = new JsonWriter();
            ShiJu.Models.User.Serialize(writer, user, Formats.UserDetail);

            return this.JsonResponseJson(0, "NeedNotification Updated", "User", writer.ToString());
        }

    }
}