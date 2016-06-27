using ShiJu.Common;
using ShiJu.Models;
using ShiJu.Utils;
using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ThirdPartNotification.JPushNotification;
using WindowsServer.Json;
using WindowsServer.Log;

namespace ShiJu.Service.Controllers
{
    public class UserGreetingsController : Controller
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        [HttpPost]
        [Route("UserGreeting/User/{sourceUserId}/Greet")]   //？targetPhoneNumber
        public ActionResult Greet(Guid sourceUserId, Guid? targetUserId, string targetPhoneNumber = "")
        {
            var utcNow = DateTime.UtcNow;
            using (var db = Heart.CreateShiJuDbContext())
            {
                var sourUser = db.Users.Find(sourceUserId);
                if (string.IsNullOrEmpty(targetPhoneNumber))
                {
                    var userGreeting = db.UserGreetings.FirstOrDefault(u => u.SourceUserId == sourceUserId && u.TargetUserId == targetUserId.Value);
                    if (userGreeting == null)
                    {
                        userGreeting = new UserGreeting()
                        {
                            SourceUserId = sourceUserId,
                            TargetUserId = targetUserId.Value,
                            TotalCount = 0,
                            AgreeCount = 0,
                            LastModifiedTime = utcNow,
                            HasNewGreeting = false,
                            HasRead = false
                        };
                        db.UserGreetings.Add(userGreeting);
                    }
                    userGreeting.TotalCount += 1;
                    userGreeting.HasNewGreeting = true;
                    userGreeting.HasRead = false;
                    userGreeting.LastModifiedTime = DateTime.UtcNow;

                    var newGreeting = new Greeting()
                    {
                        SourceUserId = sourceUserId,
                        TargetPhoneNumber = targetPhoneNumber ?? "",
                        TargetUserId = targetUserId.Value == Guid.Empty ? Guid.Empty : targetUserId.Value,
                        IsAgreed = false,
                        CreatedTime = utcNow
                    };
                    db.Greetings.Add(newGreeting);
                    db.SaveChanges();
                }
                else
                {
                    var account = db.Accounts.FirstOrDefault(a => a.Name == targetPhoneNumber);
                    if (account != null && account.UserId == sourceUserId)
                    {
                        return this.JsonResult(-1, "can't greet yourself");
                    }

                    var user = db.Users.FirstOrDefault(u => u.PhoneNumber == targetPhoneNumber);
                    if (user == null)
                    {
                        user = new User()
                        {
                            Id = Guid.NewGuid(),
                            PhoneNumber = targetPhoneNumber,
                            CreatedTime = utcNow,
                            Status = UserStatus.Inactive,
                            NickName = "",
                            Signature = "",
                            Portrait = Guid.Empty,
                            BackgroundImage = Guid.Empty,
                            District = "",
                            SignUpTime = utcNow
                        };
                        db.Users.Add(user);
                        //notification
                    }

                    targetUserId = user.Id;

                    //？？ 是否加好友
                    var userGreeting = db.UserGreetings.FirstOrDefault(u => u.SourceUserId == sourceUserId && u.TargetUserId == targetUserId.Value);
                    if (userGreeting == null)
                    {
                        userGreeting = new UserGreeting()
                        {
                            SourceUserId = sourceUserId,
                            TargetUserId = targetUserId.Value,
                            TotalCount = 0,
                            AgreeCount = 0,
                            LastModifiedTime = utcNow,
                            HasNewGreeting = false,
                            HasRead = false
                        };
                        db.UserGreetings.Add(userGreeting);
                    }
                    userGreeting.TotalCount += 1;
                    userGreeting.HasNewGreeting = true;
                    userGreeting.HasRead = false;
                    userGreeting.LastModifiedTime = DateTime.UtcNow;

                    var newGreeting = new Greeting()
                    {
                        SourceUserId = sourceUserId,
                        TargetPhoneNumber = targetPhoneNumber ?? "",
                        TargetUserId = targetUserId.Value == Guid.Empty ? Guid.Empty : targetUserId.Value,
                        IsAgreed = false,
                        CreatedTime = utcNow
                    };
                    db.Greetings.Add(newGreeting);
                    db.SaveChanges();

                    var outGreeting = db.Greetings.OrderByDescending(g => g.CreatedTime).FirstOrDefault(g => g.SourceUserId == sourceUserId && g.TargetUserId == targetUserId.Value && g.TargetPhoneNumber == targetPhoneNumber);

                    if (user.Status == UserStatus.Inactive && outGreeting != null)
                    {
                        try
                        {
                            var balance = SmsUtility.getBalance();//debug
                            var smsContent = string.Format(Heart.SmsGreetTemplate, sourUser.NickName,outGreeting.Id);
                            var respCode = SmsUtility.sendOnce(targetPhoneNumber, smsContent);
                            _logger.Info("Send message return code:" + respCode);
                        }
                        catch (Exception ex)
                        {
                            _logger.ErrorException("Error on sending message to " + targetPhoneNumber, ex);
                        }
                    }
                }


                var extraKeyValues = new Dictionary<string, string>();
                extraKeyValues.Add("Type", ((long)NotificationType.Greet).ToString());
                extraKeyValues.Add("Description", "Greeting");

                var targetUser = db.Users.FirstOrDefault(u => u.Id == targetUserId.Value && u.Status == UserStatus.Active);
                if (targetUser != null && targetUser.NeedNotification)
                {
                    JPushNotificationManager.PushNotification(ShiJuApp.AppModuleId, new List<Guid>() { targetUser.Id }, 1, string.Format("{0}找你玩", sourUser.NickName), extraKeyValues);
                }
            }

            return this.JsonResult(0, "Greet succeed");
        }

        [Route("UserGreeting/User/{userId}/GreetingList")]
        public ActionResult GetGreetingList(Guid userId, int start = 0, int count = 20)
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                var greetingList = db.UserGreetings.Where(u => u.TargetUserId == userId).OrderByDescending(u => u.LastModifiedTime).Skip(start).Take(count).ToList();
                foreach (var greeting in greetingList)
                {
                    greeting.SourceUser = db.Users.Find(greeting.SourceUserId);
                }

                var writer = new JsonWriter();
                UserGreeting.Serialize(writer, greetingList, Formats.UesrGreetingDetail);
                return this.JsonResponseJson(0, "Succeed", "GreetingList", writer.ToString());
            }
        }

        [Route("UserGreeting/SourceUser/{sourceUserId}/TargetUser/{targetUserId}/AcceptGreeting")]
        public ActionResult AcceptGreeting(Guid sourceUserId, Guid targetUserId, int greetingId = -1)
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                var nowTime = DateTime.UtcNow;
                var sourceUser = db.Users.Find(sourceUserId);
                var userGreetring = db.UserGreetings.FirstOrDefault(u => u.SourceUserId == sourceUserId && u.TargetUserId == targetUserId);
                userGreetring.HasNewGreeting = false;
                userGreetring.AgreeCount += 1;
                if (greetingId > 0)
                {
                    var greeting = db.Greetings.Find(greetingId);
                    greeting.IsAgreed = true;
                }

                var beginTime = nowTime;
                for (int i = 0; i < 4; i++)
                {
                    if (15 * i <= beginTime.Minute && beginTime.Minute < 15 * (i + 1))
                    {
                        beginTime = beginTime.AddMinutes(15 * (i + 1) - nowTime.Minute);
                        break;
                    }
                }

                var party = new Party()
                {
                    Address = string.Empty,
                    BeginTime = beginTime,
                    CommentCount = 0,
                    CreatedTime = nowTime,
                    CreatorUserId = sourceUserId,
                    Description = "",
                    EndTime = beginTime.AddHours(1),
                    Id = Guid.NewGuid(),
                    Images = Heart.GreetingPartyImages,
                    IsPublic = false,
                    IsDisabled = false,
                    Kind = ((long)PartyKind.Greeting).ToString(),
                    DirectFriendVisible = false,
                    IsUserLiked = false,
                    IsUserVoted = false,
                    LikeCount = 0,
                    MaxUserCount = 2,
                    ParticipantCount = 2,
                    Title = Heart.GreetingPartyTitle,
                    VoteChoicesJson = "[]",
                    Sponsor = sourceUser.NickName,
                    VoteResult0Count = 0,
                    VoteResult1Count = 0,
                    VoteResult2Count = 0,
                    VoteResult3Count = 0,
                    VoteResult4Count = 0,
                    VoteTitle = string.Empty
                };
                db.Parties.Add(party);

                var participant = new Participant()
                {
                    CreatedTime = nowTime,
                    PartyId = party.Id,
                    ProposedBeginTime = beginTime,
                    ProposedEndTime = beginTime.AddHours(1),
                    Status = ParticipantStatus.Accepted,
                    Unread = true,
                    UserId = targetUserId
                };
                db.Participants.Add(participant);

                if (greetingId < 0)
                {
                    var friend = db.Friends.FirstOrDefault(f => (f.UserId == sourceUserId && f.FriendUserId == targetUserId) || (f.UserId == targetUserId && f.FriendUserId == sourceUserId));
                    if (friend == null)
                    {
                        friend = new Friend()
                        {
                            UserId = sourceUserId,
                            FriendUserId = targetUserId,
                            CreatedTime = DateTime.UtcNow
                        };
                        db.Friends.Add(friend);
                    }
                }
               
                db.SaveChanges();

                return this.JsonResult(0, "Succeed");
            }
        }

        [HttpPost]
        [Route("UserGreeting/User/{userId}/MarkRead")]
        public ActionResult MarkReadUserGreeting(Guid userId)
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                var greetingList = db.UserGreetings.Where(u => u.TargetUserId == userId).ToList();
                foreach (var greeting in greetingList)
                {
                    greeting.HasRead = true;
                }
                db.SaveChanges();
                return this.JsonResult(0, "Succeed");
            }
        }

        [Route("UserGreeting/Greeting/{greetingId}/Invitation")]
        public ActionResult Invitation(int greetingId)
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                ViewData["ServiceBaseUrl"] = Heart.ServiceBaseUrl;
                ViewData["ImageBaseUrl"] = Heart.ImageBaseUrl;

                var greeting=db.Greetings.AsNoTracking().FirstOrDefault(g=>g.Id==greetingId);

                var HasSignup = db.Accounts.Any(a => a.Name == greeting.TargetPhoneNumber);
                if (HasSignup)
                {
                    ViewData["HasSignup"] = 1;
                }
                else
                {
                    ViewData["HasSignup"] = 0;
                }

                greeting.SourceUser = db.Users.Find(greeting.SourceUserId);
                return View(greeting);
            }
        }

        [Route("UserGreeting/Greeting/{greetingId}")]
        public ActionResult GetGreeting(int greetingId)
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                var greeting = db.Greetings.Find(greetingId);
                var writer = new JsonWriter();
                Greeting.Serialize(writer, greeting, Formats.GreetingDetail);
                return this.JsonResponseJson(0, "Succeed", "Greeting", writer.ToString());
            }
        }


        public ActionResult Register()
        {
            return View();
        }

        public ActionResult RegisterSuccess()
        {
            return View();
        }
    }
}