using ShiJu.Common;
using ShiJu.Models;
using ShiJu.Service.Models;
using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ThirdPartNotification.JPushNotification;
using WindowsServer.Log;

namespace ShiJu.Service.Controllers
{
    public class WeChatHelperController : Controller
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        public ActionResult JoinWay(string unionId, string partyId, string beginTime, string endTime, string nickName)
        {
            ViewData["UnionId"] = unionId;
            ViewData["NickName"] = nickName;
            ViewData["PartyBeginTime"] = beginTime;
            ViewData["PartyEndTime"] = endTime;
            ViewData["PartyId"] = partyId;
            ViewData["ServiceBaseUrl"] = Heart.ServiceBaseUrl;

            return View();
        }

        public ActionResult Bangding()
        {
            ViewData["ServiceBaseUrl"] = Heart.ServiceBaseUrl;
            return View();
        }

        public ActionResult Reminder()
        {
            return View();
        }

        public ActionResult JoinSuccess()
        {
            return View();
        }
        [Route("Parties/{partyId}/Display")]
        public ActionResult DisplayParty(Guid partyId)
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                var party = db.Parties.Find(partyId);
                party.Participants = (from p in db.Participants where p.PartyId == partyId select p).ToList();
                foreach (var participant in party.Participants)
                {
                    participant.User = UserDAO.GetUserWithCahce(db, participant.UserId);
                }

                party.PartyComments = (from c in db.PartyComments where c.PartyId == partyId select c).ToList();
                party.CreatorUser = db.Users.Find(party.CreatorUserId);

                foreach (var comment in party.PartyComments)
                {
                    comment.User = UserDAO.GetUserWithCahce(db, comment.UserId);
                }
                ViewData["ImageBaseUrl"] = Heart.ImageBaseUrl;
                ViewData["GetPartyUrl"] = String.Format(Heart.GetPartyUrl, partyId);
                ViewData["ServiceBaseUrl"] = Heart.ServiceBaseUrl;
                return View(party);
            }
        }

        [Route("Wechat/Redirect/Parties/{partyId}/Participant")]
        public async Task<ActionResult> WechatRedirect(Guid partyId, string code, string state)
        {
            using (var client = new HttpClient())
            {
                var unionId = string.Empty;
                var nickname = string.Empty;
                var w_userNickname = string.Empty;
                var w_userSex = long.MinValue;
                var w_userHeadImgUrl = string.Empty;

                if (code == null)
                {
                    return this.JsonResult(-1, "the wechat user refuse authorization");
                }
                var result = string.Empty;
                var openId = string.Empty;
                JsonObject json;
                var accessToken = string.Empty;
                var refresh_token = string.Empty;
                result = await GetWechatUserInfo(code);
                json = JsonValue.Parse(result) as JsonObject;
                openId = json.GetStringValue("openid");
                accessToken = json.GetStringValue("access_token");
                refresh_token = json.GetStringValue("refresh_token");

                if (string.IsNullOrEmpty(accessToken))
                {
                    return this.JsonResult(-2, "获取用户授权失败");
                }

                accessToken = await RefreshToken(refresh_token);
                var getWechateUserUrl = string.Format("https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN"
                , accessToken, openId);
                var wechatUserInfo = await client.GetStringAsync(getWechateUserUrl);
                //s_logger.Debug("wechatUserInfo" + wechatUserInfo);
                var wechatUserJson = JsonValue.Parse(wechatUserInfo) as JsonObject;
                unionId = wechatUserJson.GetStringValue("unionid");
                w_userNickname = wechatUserJson.GetStringValue("nickname");
                w_userSex = wechatUserJson.GetLongValue("sex");
                w_userHeadImgUrl = wechatUserJson.GetStringValue("headimgurl");

                if (string.IsNullOrEmpty(w_userNickname))
                {
                    return this.JsonResult(-3, "获取用户信息失败");
                }

                using (var db = Heart.CreateShiJuDbContext())
                {
                    var account = (from a in db.Accounts
                                   where (a.Type == AccountType.Wechat) && (a.Name == unionId)
                                   select a).FirstOrDefault();
                    var party = db.Parties.Find(partyId);

                    if (account == null)
                    {
                        return RedirectToAction("JoinWay", "WeChatHelper", new { unionId = unionId, partyId = partyId.ToString(), beginTime = party.BeginTime.ToString(), endTime = party.EndTime.ToString(), nickName = w_userNickname });
                    }

                    if (party.CreatorUserId == account.UserId)
                    {
                        return RedirectToAction("Reminder", "WeChatHelper");
                    }

                    var utcNow = DateTime.UtcNow;
                    var user = db.Users.Find(account.UserId);

                    // var party = db.Parties.Find(partyId);
                    var participant = db.Participants.FirstOrDefault(p => p.PartyId == partyId && p.UserId == user.Id);
                    if (participant == null)
                    {
                        participant = new Participant()
                        {
                            CreatedTime = utcNow,
                            PartyId = partyId,
                            ProposedBeginTime = party.BeginTime,
                            ProposedEndTime = party.EndTime,
                            UserId = user.Id,
                            Unread = true,
                            Status = ParticipantStatus.Enrolled
                        };
                        db.Participants.Add(participant);
                    }

                    if (participant.Status != ParticipantStatus.Accepted && participant.Status != ParticipantStatus.Enrolled)
                    {
                        participant.Status = ParticipantStatus.Enrolled;
                        var extraKeyValues = new Dictionary<string, string>();
                        extraKeyValues.Add("Type", ((long)NotificationType.EnrollParty).ToString());
                        extraKeyValues.Add("Description", "enroll party");
                        var needNotification = db.Users.Find(party.CreatorUserId).NeedNotification;
                        if (needNotification)
                        {
                            JPushNotificationManager.PushNotification(ShiJuApp.AppModuleId, new List<Guid>() { party.CreatorUserId }, 1, "你收到新的活动加入申请", extraKeyValues);
                        }
                    }
                    db.SaveChanges();
                }
            }
            return RedirectToAction("JoinSuccess", "WeChatHelper");
        }

        public async Task<string> GetWechatUserInfo(string code)
        {
            using (var client = new HttpClient())
            {
                string grant_type = "authorization_code";
                var getTokenUrl = string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type={3}",
                    Heart.WechatAppId, Heart.WechatSecret, code, grant_type);
                var result = await client.GetStringAsync(getTokenUrl);
                return result.ToString();
            }
        }

        public async Task<string> RefreshToken(string refresh_token)
        {
            using (var client = new HttpClient())
            {
                var url = string.Format("https://api.weixin.qq.com/sns/oauth2/refresh_token?appid={0}&grant_type=refresh_token&refresh_token={1}",
                    Heart.WechatAppId, refresh_token);

                var jsonStirng = await client.GetStringAsync(url);
                var json = JsonValue.Parse(jsonStirng) as JsonObject;
                var accessToken = json.GetStringValue("access_token");
                return accessToken;
            }
        }
    }
}