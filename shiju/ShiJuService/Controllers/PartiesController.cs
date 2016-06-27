using ShiJu.Common;
using ShiJu.Models;
using ShiJu.Service.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Mvc = System.Web.Mvc;
using WindowsServer.Json;
using WindowsServer.Log;
using ThirdPartNotification.JPushNotification;
using WindowsServer.Configuration;
using System.Data.SqlClient;
using Collaboration.TaskForce.Service.Models;
using SACommon.Models;

namespace ShiJu.Service.Controllers
{
    public class PartiesController : ApiController
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [Route("Users/{userId}/ParticipatedParties/OneDay")]
        public HttpResponseMessage GetUserParticipatedParties(Guid userId, DateTime fromTime, string statusCombination, bool isPullDown = false, int start = 0, int count = 20)
        {
            var statusList = new List<ParticipantStatus>();
            if (!string.IsNullOrEmpty(statusCombination))
            {
                foreach (var statusString in statusCombination.Split('|'))
                {
                    statusList.Add((ParticipantStatus)Enum.Parse(typeof(ParticipantStatus), statusString, true));
                }
            }

            using (var db = Heart.CreateShiJuDbContext())
            {
                List<Party> parties, myParties;
                if (isPullDown)
                {
                    parties = (from p in db.Parties
                               join par in db.Participants on p.Id equals par.PartyId
                               where (par.UserId == userId) && (fromTime >= p.BeginTime) && statusList.Contains(par.Status) && (!p.IsDisabled)
                               orderby p.BeginTime ascending
                               select p).ToList();
                    myParties = db.Parties.Where(p => p.CreatorUserId == userId && fromTime >= p.BeginTime && !p.IsDisabled).ToList();

                }
                else
                {
                    parties = (from p in db.Parties
                               join par in db.Participants on p.Id equals par.PartyId
                               where (par.UserId == userId) && (fromTime <= p.BeginTime) && statusList.Contains(par.Status) && (!p.IsDisabled)
                               orderby p.BeginTime ascending
                               select p).ToList();
                    myParties = db.Parties.Where(p => p.CreatorUserId == userId && fromTime <= p.BeginTime && !p.IsDisabled).ToList();
                }

                parties.AddRange(myParties);


                var toTime = fromTime.AddDays(1);
                parties = parties.Where(p => p.BeginTime >= fromTime && p.BeginTime < toTime).OrderBy(p => p.BeginTime).ToList();

                foreach (var party in parties)
                {
                    party.CreatorUser = db.Users.Where(u => u.Id == party.CreatorUserId).FirstOrDefault();
                    party.UserParticipant = db.Participants.Where(p => p.PartyId == party.Id && p.UserId == userId).FirstOrDefault();
                }

                var writer = new JsonWriter();
                Party.Serialize(writer, parties, Formats.PartyDetail);

                return this.JsonResponseJson(0, "Parties", writer.ToString());
            }
        }


        [Route("Users/{userId}/ParticipatedParties")]
        public HttpResponseMessage GetUserParticipatedParties(Guid userId, DateTime fromTime, string statusCombination, bool isOneday = false, bool isPullDown = false, int start = 0, int count = 20)
        {
            var statusList = new List<ParticipantStatus>();
            if (!string.IsNullOrEmpty(statusCombination))
            {
                foreach (var statusString in statusCombination.Split('|'))
                {
                    statusList.Add((ParticipantStatus)Enum.Parse(typeof(ParticipantStatus), statusString, true));
                }
            }

            var localFromTime = fromTime.AddHours(-8);

            using (var db = Heart.CreateShiJuDbContext())
            {
                List<Party> parties, myParties;
                if (isPullDown)
                {
                    parties = (from p in db.Parties
                               join par in db.Participants on p.Id equals par.PartyId
                               where (par.UserId == userId) && (localFromTime >= p.BeginTime) && statusList.Contains(par.Status) && (!p.IsDisabled)
                               orderby p.BeginTime ascending
                               select p).ToList();
                    myParties = db.Parties.Where(p => p.CreatorUserId == userId && localFromTime > p.BeginTime && !p.IsDisabled).ToList();

                }
                else
                {
                    parties = (from p in db.Parties
                               join par in db.Participants on p.Id equals par.PartyId
                               where (par.UserId == userId) && (localFromTime <= p.BeginTime) && statusList.Contains(par.Status) && (!p.IsDisabled)
                               orderby p.BeginTime ascending
                               select p).ToList();
                    myParties = db.Parties.Where(p => p.CreatorUserId == userId && localFromTime <= p.BeginTime && !p.IsDisabled).ToList();
                }

                parties.AddRange(myParties);

                if (isOneday)
                {
                    var toTime = fromTime.AddDays(1);
                    parties = parties.Where(p => p.BeginTime >= localFromTime && p.BeginTime < toTime).OrderBy(p => p.BeginTime).ToList();
                }
                else
                {
                    if (isPullDown)
                    {
                        parties = parties.OrderByDescending(p => p.BeginTime).Skip(start).Take(count).OrderBy(p => p.BeginTime).ToList();
                    }
                    else
                    {
                        parties = parties.OrderBy(p => p.BeginTime).Skip(start).Take(count).ToList();
                    }
                }

                foreach (var party in parties)
                {
                    party.CreatorUser = db.Users.Where(u => u.Id == party.CreatorUserId).FirstOrDefault();
                    party.UserParticipant = db.Participants.Where(p => p.PartyId == party.Id && p.UserId == userId).FirstOrDefault();
                }

                var writer = new JsonWriter();
                Party.Serialize(writer, parties, Formats.PartyDetail);

                return this.JsonResponseJson(0, "Parties", writer.ToString());
            }
        }

        [Route("Users/{userId}/Month/ParticipatedParties")]
        public HttpResponseMessage GetPartiesDate(Guid userId, string statusCombination, DateTime firstDayInMonth)
        {
            var statusList = new List<ParticipantStatus>();
            if (!string.IsNullOrEmpty(statusCombination))
            {
                foreach (var statusString in statusCombination.Split('|'))
                {
                    statusList.Add((ParticipantStatus)Enum.Parse(typeof(ParticipantStatus), statusString, true));
                }
            }

            var utcFirstDayInMonth = firstDayInMonth.AddHours(-8);

            using (var db = Heart.CreateShiJuDbContext())
            {
                var endDayInMonth = new DateTime(firstDayInMonth.Year, firstDayInMonth.Month, DateTime.DaysInMonth(firstDayInMonth.Year, firstDayInMonth.Month), 16, 0, 0);

                var parties = (from p in db.Parties
                               join par in db.Participants on p.Id equals par.PartyId
                               where (par.UserId == userId) && (utcFirstDayInMonth <= p.BeginTime) && (endDayInMonth > p.BeginTime) && statusList.Contains(par.Status) && (!p.IsDisabled)
                               orderby p.BeginTime ascending
                               select p).ToList();
                var myParties = db.Parties.Where(p => p.CreatorUserId == userId && utcFirstDayInMonth <= p.BeginTime && endDayInMonth > p.BeginTime && !p.IsDisabled).ToList();

                parties.AddRange(myParties);

                parties = parties.OrderBy(p => p.BeginTime).ToList();

                var writer = new JsonWriter();
                Party.Serialize(writer, parties, Formats.PartySmall);

                return this.JsonResponseJson(0, "Parties", writer.ToString());
            }
        }

        [Route("HotParties")]
        public HttpResponseMessage GetHotParties()
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                var parties = (from p in db.Parties
                               where (p.IsPublic) && (!p.IsDisabled)
                               orderby p.BeginTime ascending
                               select p).ToList();

                var writer = new JsonWriter();
                Party.Serialize(writer, parties, Formats.PartyBrief);

                return this.JsonResponseJson(0, "Parties", writer.ToString());
            }
        }

        [Route("Parties/{partyId}")]
        public HttpResponseMessage GetParty(Guid partyId, Guid userId)
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                var party = db.Parties.Find(partyId);
                party.ParticipantCount = (from p in db.Participants.AsNoTracking() where p.PartyId == partyId && p.Status == ParticipantStatus.Accepted select p).Count();

                var participantCount = 5;
                party.Participants = (from p in db.Participants.AsNoTracking() where p.PartyId == partyId && p.Status == ParticipantStatus.Accepted select p).Take(participantCount).ToList();

                foreach (var participant in party.Participants)
                {
                    participant.User = UserDAO.GetUserWithCahce(db, participant.UserId);
                }

                var commentCounts = 5;
                var partyComments = db.PartyComments.Where(pc => pc.PartyId == partyId && string.IsNullOrEmpty(pc.VoteResult.Trim())).OrderByDescending(pc => pc.CreatedTime).Take(commentCounts).ToList();
                foreach (var pc in partyComments)
                {
                    pc.User = UserDAO.GetUserWithCahce(db, pc.UserId);
                    if (!pc.TargetUserId.IsEmpty())
                    {
                        pc.TargetUser = UserDAO.GetUserWithCahce(db, pc.TargetUserId);
                    }

                }
                party.PartyComments = partyComments;
                party.CreatorUser = db.Users.Where(u => u.Id == party.CreatorUserId).FirstOrDefault();

                if (userId != Guid.Empty || userId != null)
                {
                    party.IsUserLiked = db.PartyLikes.Where(pl => pl.UserId == userId && pl.PartyId == partyId).Any();
                    var voteResult = (from pc in db.PartyComments
                                      where pc.UserId == userId && pc.PartyId == partyId
                                      select pc.VoteResult).FirstOrDefault();
                    if (voteResult == null || voteResult == "" || voteResult == "[]")
                    {
                        party.IsUserVoted = false;
                    }
                    else
                    {
                        party.IsUserVoted = true;
                    }

                    party.UserParticipant = db.Participants.Where(p => p.UserId == userId && p.PartyId == partyId).FirstOrDefault();
                }

                var writer = new JsonWriter();
                Party.Serialize(writer, party, Formats.PartyDetail);
                return this.JsonResponseJson(0, "Party", writer.ToString());
            }
        }

        [Route("Parties/Create")]
        public async Task<HttpResponseMessage> CreateParty()
        {
            var utcNow = DateTime.UtcNow;
            var json = await this.GetRequestContentJson() as JsonObject;
            var party = Party.Deserialize(json.GetJsonObject("Party"));
            party.Id = Guid.NewGuid();
            party.CreatedTime = DateTime.UtcNow;
            party.IsPublic = false;
            party.IsDisabled = false;

            var needNotificationUserIdList = new List<Guid>();
            using (var db = Heart.CreateShiJuDbContext())
            {
                db.Parties.Add(party);
                if (party.Participants != null)
                {
                    foreach (var participant in party.Participants)
                    {
                        var userExist = db.Users.Where(u => u.Id == participant.UserId).Any();
                        if (!userExist)
                        {
                            return this.JsonResult(-1, "this user is not exist in user table DB" + participant.UserId);
                        }

                        participant.PartyId = party.Id;
                        participant.CreatedTime = utcNow;
                        participant.Status = ParticipantStatus.Created;
                        participant.Unread = true;
                        participant.ProposedBeginTime = party.BeginTime;
                        participant.ProposedEndTime = party.EndTime;

                        db.Participants.Add(participant);
                    }
                }
                db.SaveChanges();
                party.CreatorUser = db.Users.Where(u => u.Id == party.CreatorUserId).FirstOrDefault();
                var paticipantIdList = party.Participants.Select(p => p.UserId).ToList();

                foreach (var userid in paticipantIdList)
                {
                    if (db.Users.Find(userid).NeedNotification)
                    {
                        needNotificationUserIdList.Add(userid);
                    }
                }
            }


            var extraKeyValues = new Dictionary<string, string>();
            extraKeyValues.Add("Type", ((long)NotificationType.InviteUser).ToString());
            extraKeyValues.Add("Description", "invite user");
            //extraKeyValues.Add("CreateUser", party.CreatorUser.NickName);
            //var text = String.Format("{0}邀请您参加他的活动", party.CreatorUser.NickName);

            JPushNotificationManager.PushNotification(ShiJuApp.AppModuleId, needNotificationUserIdList, 1, "你有新的活动邀请", extraKeyValues);

            var writer = new JsonWriter();
            Party.Serialize(writer, party, Formats.PartyDetail);
            return this.JsonResponseJson(0, "Party", writer.ToString());
        }

        [HttpPost]
        [Route("Parties/{partyId}/Update")]//the new added by archer
        public async Task<HttpResponseMessage> UpdateParty(Guid partyId)
        {
            var utcNow = DateTime.UtcNow;
            var json = await this.GetRequestContentJson() as JsonObject;
            var partyJson = Party.Deserialize(json.GetJsonObject("Party"));
            Party party;
            var needNotificationUserIdList = new List<Guid>();
            using (var db = Heart.CreateShiJuDbContext())
            {
                party = db.Parties.Find(partyId);
                if (party != null)
                {
                    party.Images = partyJson.Images;
                    party.EndTime = partyJson.EndTime;
                    party.BeginTime = partyJson.BeginTime;
                    party.Address = partyJson.Address;
                    party.Description = partyJson.Description;
                    party.DirectFriendVisible = partyJson.DirectFriendVisible;
                    party.Sponsor = partyJson.Sponsor;
                    party.Title = partyJson.Title;
                    party.VoteChoicesJson = partyJson.VoteChoicesJson;
                    party.VoteTitle = partyJson.VoteTitle;
                    party.MaxUserCount = partyJson.MaxUserCount;
                }
                db.SaveChanges();
                var partyName = db.Parties.Where(p => p.Id == partyId).Select(p => p.Title).FirstOrDefault();
                var userIdList = db.Participants.Where(p => p.PartyId == partyId).Select(p => p.UserId).ToList();

                foreach (var userid in userIdList)
                {
                    if (db.Users.Find(userid).NeedNotification)
                    {
                        needNotificationUserIdList.Add(userid);
                    }
                }

                var extraKeyValues = new Dictionary<string, string>();
                extraKeyValues.Add("Type", ((long)NotificationType.UpdateParty).ToString());
                extraKeyValues.Add("Description", "update  party");
                JPushNotificationManager.PushNotification(ShiJuApp.AppModuleId, userIdList, 1, string.Format("您参加的活动 {0} 已经被修改,请注意查看", partyName));
                party.CreatorUser = db.Users.Where(u => u.Id == party.CreatorUserId).FirstOrDefault();
            }

            var writer = new JsonWriter();
            Party.Serialize(writer, party, Formats.PartyDetail);
            return this.JsonResponseJson(0, "Party", writer.ToString());
        }

        [Route("Parties/{partyId}/Participants")]
        public HttpResponseMessage GetPartyParticipants(Guid partyId)
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                var participants = (from p in db.Participants
                                    join u in db.Users on p.UserId equals u.Id
                                    where (p.PartyId == partyId) && (u.Status == UserStatus.Active)
                                    orderby p.Status
                                    select p).ToList();

                foreach (var participant in participants)
                {
                    participant.User = UserDAO.GetUserWithCahce(db, participant.UserId);
                }

                var writer = new JsonWriter();
                Participant.Serialize(writer, participants, Formats.ParticipantBrief);

                return this.JsonResponseJson(0, "Participants", writer.ToString());
            }
        }

        [Route("Users/{userId}/ParticipantUnreadCount")]
        public HttpResponseMessage GetParticipantUnreadCount(Guid userId)
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                var utcNow = DateTime.UtcNow;
                var participants = db.Participants.AsNoTracking().Where(p => p.UserId == userId && p.Unread).ToList();
                var createdCount = participants.Where(p => p.Status == ParticipantStatus.Created).Count();
                var acceptedCount = participants.Where(p => p.Status == ParticipantStatus.Accepted).Count();

                var enrolledCount = (from participant in db.Participants
                                     join party in db.Parties on participant.PartyId equals party.Id
                                     where party.CreatorUserId == userId && participant.Status == ParticipantStatus.Enrolled && participant.Unread
                                     select participant).Count();

                var greetingCount = db.UserGreetings.Where(u => u.TargetUserId == userId && u.HasRead == false).Count();
                //var enrolledCount = participants.Where(p => p.Status == ParticipantStatus.Enrolled).Count();
                //var totalCount = createdCount + acceptedCount + enrolledCount;

                var json = new JsonObject();
                json.Add("CreatedCount", createdCount);
                json.Add("EnrolledCount", enrolledCount);
                json.Add("GreetingCount", greetingCount);

                return this.JsonResponseJson(0, "ParticipantCount", json.ToString());
            }
        }

        [HttpPost]
        [Route("Users/{userId}/MarkReadForParticipants")]
        public HttpResponseMessage MarkReadForParticipants(Guid userId)
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                var utcNow = DateTime.UtcNow;

                var participants = (from participant in db.Participants
                                    join party in db.Parties on participant.PartyId equals party.Id
                                    where participant.Unread
                                    &&
                                    (party.CreatorUserId == userId && participant.Status == ParticipantStatus.Enrolled)
                                    || (participant.UserId == userId
                                    && (participant.Status == ParticipantStatus.Created
                                    || participant.Status == ParticipantStatus.Accepted
                                    || participant.Status == ParticipantStatus.QuitAfterAccepting
                                    || participant.Status == ParticipantStatus.Enrolled)
                                    )
                                    select participant).ToList();

                foreach (var participant in participants)
                {
                    participant.Unread = false;
                }
                db.SaveChanges();

                return this.JsonResult(0, "Marked", "Count", participants.Count);
            }
        }

        [HttpPost]
        [Route("Users/{userId}/MarkReadForOwnParticipants")]
        public HttpResponseMessage MarkReadForOwnParticipants(Guid userId)
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                var utcNow = DateTime.UtcNow;

                var ownParticipants = (from participant in db.Participants
                                    join party in db.Parties on participant.PartyId equals party.Id
                                    where participant.Unread
                                    &&participant.UserId == userId
                                    && participant.Status != ParticipantStatus.Refused
                                    select participant).ToList();

                foreach (var participant in ownParticipants)
                {
                    participant.Unread = false;
                }
                db.SaveChanges();

                return this.JsonResult(0, "Marked", "OwnParticipantCount", ownParticipants.Count);
            }
        }

        [HttpPost]
        [Route("Users/{userId}/MarkReadForEnrolledParticipants")]
        public HttpResponseMessage MarkReadForEnrolledParticipants(Guid userId)
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                //var participants = (from p in db.Participants
                //                    where p.Unread && (p.UserId == userId) && (p.Status == ParticipantStatus.Created || p.Status == ParticipantStatus.Accepted || p.Status == ParticipantStatus.QuitAfterAccepting || p.Status == ParticipantStatus.Enrolled)
                //                    select p).ToList();
                var utcNow = DateTime.UtcNow;

                var enrolledParticipants = (from participant in db.Participants
                                            join party in db.Parties on participant.PartyId equals party.Id
                                            where participant.Unread
                                            && party.CreatorUserId == userId
                                            && participant.Status == ParticipantStatus.Enrolled
                                            select participant).ToList();

                foreach (var participant in enrolledParticipants)
                {
                    participant.Unread = false;
                }
                db.SaveChanges();

                return this.JsonResult(0, "Marked", "EnrolledParticipantCount", enrolledParticipants.Count);
            }
        }

        [HttpPost]
        [Route("Parties/Participants/Create")]
        public async Task<HttpResponseMessage> CreateParticipant()
        {
            var json = await this.GetRequestContentJson() as JsonObject;
            var newParticipant = Participant.Deserialize(json.GetJsonObject("Participant"));

            newParticipant.CreatedTime = DateTime.UtcNow;
            newParticipant.Unread = true;

            using (var db = Heart.CreateShiJuDbContext())
            {
                var participantExist = (from p in db.Participants
                                        where (p.PartyId == newParticipant.PartyId) && (p.UserId == newParticipant.UserId)
                                        select p).Any();
                if (participantExist)
                {
                    return this.JsonResult(-1, "this participant has exsit.");
                }

                db.Participants.Add(newParticipant);
                db.SaveChanges();

                var party = db.Parties.AsNoTracking().FirstOrDefault(p => p.Id == newParticipant.PartyId);
                var creatorUser = db.Users.AsNoTracking().FirstOrDefault(u => u.Id == party.CreatorUserId);

                var extraKeyValues = new Dictionary<string, string>();

                extraKeyValues.Add("Type", ((long)NotificationType.EnrollParty).ToString());
                extraKeyValues.Add("Description", "enroll party");
                //extraKeyValues.Add("ParticipantUser", creatorUser.NickName);
                //var text = String.Format("{0}申请参加您的活动", creatorUser.NickName);
                var needNotification = db.Users.Find(party.CreatorUserId).NeedNotification;
                if (needNotification)
                {
                    JPushNotificationManager.PushNotification(ShiJuApp.AppModuleId, new List<Guid>() { party.CreatorUserId }, 1, "你收到新的活动加入申请", extraKeyValues);
                }

                var writer = new JsonWriter();
                Participant.Serialize(writer, newParticipant, Formats.ParticipantBrief);
                return this.JsonResponseJson(0, "Participant", writer.ToString());
            }
        }

        [HttpPost]
        [Route("Parties/{partyId}/Participants/{userId}/Update")]
        public async Task<HttpResponseMessage> UpdateParticipant(Guid partyId, Guid userId)
        {
            var json = await this.GetRequestContentJson() as JsonObject;
            var newParticipant = Participant.Deserialize(json.GetJsonObject("Participant"));

            using (var db = Heart.CreateShiJuDbContext())
            {
                var participant = (from p in db.Participants
                                   where (p.PartyId == partyId) && (p.UserId == userId)
                                   select p).FirstOrDefault();

                if (participant == null)
                {
                    return this.JsonResult(-1, "The participant does not exist.");
                }

                participant.Status = newParticipant.Status;
                participant.ProposedBeginTime = newParticipant.ProposedBeginTime;
                participant.ProposedEndTime = newParticipant.ProposedEndTime;

                if (newParticipant.Status == ParticipantStatus.Enrolled)
                {
                    participant.Unread = true;
                    var party = db.Parties.Find(participant.PartyId);
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

                var writer = new JsonWriter();
                Participant.Serialize(writer, participant, Formats.ParticipantBrief);
                return this.JsonResponseJson(0, "Participant", writer.ToString());
            }

        }

        #region Attachment audio

        [HttpPost]
        [Route("Messages/{partyId}/Logs/Attachments/Add")]
        public async Task<HttpResponseMessage> AddMessageAttachment(Guid partyId, string mimeType)
        {
            try
            {
                var rootPath = HttpContext.Current.Server.MapPath("~");
                var provider = new MultipartFormDataStreamProvider(rootPath);
                await Request.Content.ReadAsMultipartAsync(provider);

                Guid attachmentId;
                var filePath = provider.FileData[0].LocalFileName;
                using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    fileStream.Seek(0, SeekOrigin.Begin);
                    using (var md5 = MD5.Create())
                    {
                        attachmentId = new Guid(md5.ComputeHash(fileStream));
                    }
                    fileStream.Seek(0, SeekOrigin.Begin);

                    AttachmentStorage.AddAttachment(ShiJuApp.CorporationId, partyId, attachmentId, mimeType, fileStream);
                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                return this.JsonResult(true, "AttachmentId", attachmentId);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("error occurs:", ex);
                throw ex;
            }
        }

        // GET Messages/{parentId}/Logs/Attachments/{attachmentId}
        [Route("Messages/{partyId}/Logs/Attachments/{attachmentId}")]
        public HttpResponseMessage GetMessageAttachment(Guid partyId, Guid attachmentId)
        {
            string mimeType = "audio/collaboration.amr";//hard code for current app cuz just only this type of attachment exists;
            //var filePath = AttachmentStorage.GetAttachmentFilePath(ZigeApp.CorporationId, parentId, attachmentId, out mimeType);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            using (Stream stream = AttachmentStorage.GetAttachmentStream(ShiJuApp.CorporationId, partyId, attachmentId, mimeType))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                response.Content = new ByteArrayContent(bytes);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = attachmentId.ToString();
            }

            return response;
        }
        #endregion

        [Route("Parties/{partyId}/Comments/Create")]
        public async Task<HttpResponseMessage> CreatePartyComment(Guid partyId)
        {
            var json = await this.GetRequestContentJson() as JsonObject;
            var comment = PartyComment.Deserialize(json.GetJsonObject("PartyComment"));
            comment.Id = Guid.NewGuid();
            comment.PartyId = partyId;
            comment.CreatedTime = DateTime.UtcNow;

            using (var db = Heart.CreateShiJuDbContext())
            {
                db.PartyComments.Add(comment);
                db.SaveChanges();

                comment.User = db.Users.Where(u => u.Id == comment.UserId).FirstOrDefault();
                comment.TargetUser = db.Users.Where(u => u.Id == comment.TargetUserId).FirstOrDefault();

                if (comment.VoteResult != "")
                {
                    var voteCount = (from pc in db.PartyComments
                                     where pc.VoteResult == comment.VoteResult && pc.PartyId == partyId
                                     select pc).Count();

                    try
                    {
                        var voteResult = int.Parse(comment.VoteResult);
                        switch (voteResult)
                        {
                            case 0:
                                db.Parties.Where(p => p.Id == partyId).FirstOrDefault().VoteResult0Count = voteCount;
                                break;
                            case 1:
                                db.Parties.Where(p => p.Id == partyId).FirstOrDefault().VoteResult1Count = voteCount;
                                break;
                            case 2:
                                db.Parties.Where(p => p.Id == partyId).FirstOrDefault().VoteResult2Count = voteCount;
                                break;
                            case 3:
                                db.Parties.Where(p => p.Id == partyId).FirstOrDefault().VoteResult3Count = voteCount;
                                break;
                            case 4:
                                db.Parties.Where(p => p.Id == partyId).FirstOrDefault().VoteResult4Count = voteCount;
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }

                if (comment.Text != "" || (string.IsNullOrEmpty(comment.VoteResult) && string.IsNullOrEmpty(comment.Text)))
                {
                    var commentCount = (from pc in db.PartyComments
                                        where pc.VoteResult == "" && pc.PartyId == partyId
                                        select pc).Count();
                    db.Parties.Where(p => p.Id == partyId).FirstOrDefault().CommentCount = commentCount;
                }
                db.SaveChanges();
            }

            var writer = new JsonWriter();
            PartyComment.Serialize(writer, comment, Formats.PartyCommentBrief);

            return this.JsonResponseJson(0, "PartyComment", writer.ToString());
        }

        [Route("Parties/{partyId}/Comments")]
        public HttpResponseMessage GetPartyComments(Guid partyId, int start = 0, int count = 20)
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                var commentsQuery = (from c in db.PartyComments
                                     where c.PartyId == partyId && string.IsNullOrEmpty(c.VoteResult.Trim())
                                     orderby c.CreatedTime descending
                                     select c);
                var commentCount = commentsQuery.Count();
                var comments = commentsQuery.Skip(start).Take(count).ToList();

                foreach (var comment in comments)
                {
                    comment.User = UserDAO.GetUserWithCahce(db, comment.UserId);
                    if (!comment.TargetUserId.IsEmpty())
                    {
                        comment.TargetUser = UserDAO.GetUserWithCahce(db, comment.TargetUserId);
                    }
                }

                var writer = new JsonWriter();
                PartyComment.Serialize(writer, comments, Formats.PartyCommentBrief);

                var extraJsons = new Dictionary<string, string>();
                extraJsons["PartyCommentCount"] = commentCount.ToString();
                extraJsons["PartyComments"] = writer.ToString();
                return this.JsonResponseJson(0, "Succeeded", extraJsons);
            }
        }

        [Route("Users/{userId}/Friends/Parties")]//include systemparty
        public HttpResponseMessage GetFriendsParties(Guid userId, DateTime fromTime, bool includeSystemParty = true, int start = 0, int count = 20)
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                var utcNow = DateTime.UtcNow;
                var friends = db.Friends.AsNoTracking().ToList();
                var friendIdList = (friends.Where(f => f.UserId == userId).Select(f => f.FriendUserId)).Union(
                    friends.Where(f => f.FriendUserId == userId).Select(f => f.UserId)
                    ).Distinct().ToList();

                if (includeSystemParty)
                {
                    friendIdList.Add(Guid.Empty);
                }

                var parties = (from p in db.Parties.AsNoTracking()
                               where friendIdList.Contains(p.CreatorUserId) && p.DirectFriendVisible && (p.BeginTime > fromTime) && (!p.IsDisabled)
                               select p).OrderBy(p => p.CreatedTime).ToList();

                var paryIdList = parties.Select(p => p.Id).ToList();

                var partyListInvited = (from party in db.Parties.AsNoTracking()
                                        join participant in db.Participants.AsNoTracking() on party.Id equals participant.PartyId
                                        where participant.UserId == userId && !paryIdList.Contains(party.Id) && party.BeginTime > fromTime
                                        select party).ToList();

                parties.AddRange(partyListInvited);
                //var distinctParties = parties.Distinct(new PartyComparer()).ToList();
                //var distinctIds = distinctParties.Select(p => p.Id).Distinct();

                foreach (var party in parties)
                {
                    party.ParticipantCount = db.Participants.AsNoTracking().Where(p => p.PartyId == party.Id && p.Status == ParticipantStatus.Accepted).Count();
                }
                var hotCount = 3;
                var hotParties = parties.OrderByDescending(p => p.ParticipantCount).Skip(0).Take(hotCount).OrderBy(p => p.BeginTime).ToList();

                parties.RemoveRange(hotParties);

                var normalParties = new List<Party>();

                if (start == 0)
                {
                    normalParties = hotParties.Concat(parties.OrderBy(p => p.BeginTime).Skip(start).Take(count - hotParties.Count).ToList()).ToList();
                }
                else
                {
                    normalParties = parties.OrderBy(p => p.BeginTime).Skip(start - hotParties.Count).Take(count).ToList();
                }

                foreach (var party in normalParties)
                {
                    party.CreatorUser = db.Users.AsNoTracking().Where(u => u.Id == party.CreatorUserId).FirstOrDefault();
                }

                var writer = new JsonWriter();
                ShiJu.Models.Party.Serialize(writer, normalParties, Formats.PartyBrief);

                var extraJsons = new Dictionary<string, string>();
                extraJsons["Parties"] = writer.ToString();
                return this.JsonResponseJson(0, "Succeeded", extraJsons);
            }
        }


        [Route("Users/{userId}/AllFriends/Parties")]//include systemparty
        public HttpResponseMessage GetAllFriendsParties(Guid userId, DateTime fromTime, bool isPullDown = false, bool includeSystemParty = true, int start = 0, int count = 20)
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                var sqlstr = @"SELECT F1.FriendUserId
                          FROM [ShiJu].[dbo].[Friends] F1
                          WHERE F1.UserId = @userId
                          UNION
                        SELECT F1.UserId
                          FROM [ShiJu].[dbo].[Friends] F1
                          WHERE F1.FriendUserId = @userId
                          UNION
                        SELECT F2.FriendUserId
                          FROM [ShiJu].[dbo].[Friends] F1
                          JOIN [ShiJu].[dbo].[Friends] F2 ON F1.FriendUserId = F2.UserId
                          WHERE F1.UserId = @userId
                          UNION
                        SELECT F2.UserId
                          FROM [ShiJu].[dbo].[Friends] F1
                          JOIN [ShiJu].[dbo].[Friends] F2 ON F1.UserId = F2.FriendUserId
                          WHERE F1.FriendUserId = @userId
                        ";

                var friendListSql = @"Select f.FriendUserId 
                                        From Friends f 
                                        where f.UserId=@userId
                                        union
                                        Select f.UserId
                                        From Friends f 
                                        where f.FriendUserId=@userId";

                var allFriendList = db.Database.SqlQuery<Guid>(friendListSql, new SqlParameter("@userId", userId)).Distinct().ToList();
                //var friendIdList = db.Friends.Where(f => f.UserId == userId).Select(f => f.FriendUserId).ToList();
                var startTime = fromTime;
                // var localFromTime = fromTime.AddHours(-8);

                if (fromTime.Hour >= 16)
                {
                    startTime = fromTime.AddHours(-fromTime.Hour + 16).AddMinutes(-fromTime.Minute).AddSeconds(-fromTime.Second);
                }
                else
                {
                    startTime = fromTime.AddDays(-1).AddHours(-fromTime.Hour + 16).AddMinutes(-fromTime.Minute).AddSeconds(-fromTime.Second);
                }
                var endTime = startTime.AddDays(1);
                if (includeSystemParty)
                {
                    allFriendList.Add(Guid.Empty);
                }
                var partyList = new List<Party>();
                var parties = new List<Party>();
                if (isPullDown)
                {
                    parties = db.Parties.AsNoTracking().Where(p => allFriendList.Contains(p.CreatorUserId)
                    && p.BeginTime < fromTime
                    && p.BeginTime >= startTime
                    && (!p.IsDisabled)
                    && p.DirectFriendVisible
                    ).OrderBy(p => p.BeginTime).Skip(start).Take(count).ToList();
                }
                else
                {
                    parties = db.Parties.AsNoTracking().Where(p => allFriendList.Contains(p.CreatorUserId)
                    && p.BeginTime >= fromTime
                    && p.BeginTime < endTime
                    && (!p.IsDisabled)
                    && p.DirectFriendVisible
                    ).OrderByDescending(p => p.BeginTime).Skip(start).Take(count).OrderBy(p => p.BeginTime).ToList();
                }

                foreach (var party in parties)
                {
                    party.CreatorUser = db.Users.AsNoTracking().Where(u => u.Id == party.CreatorUserId).FirstOrDefault();
                }

                var writer = new JsonWriter();
                ShiJu.Models.Party.Serialize(writer, parties, Formats.PartyBrief);

                var extraJsons = new Dictionary<string, string>();
                extraJsons["Parties"] = writer.ToString();
                return this.JsonResponseJson(0, "Succeeded", extraJsons);
            }
        }

        [HttpPost]
        [Route("Parties/{partyId}/Users/{userId}/PartyLikes/Create")]
        public HttpResponseMessage CreatePartyLike(Guid partyId, Guid userId)
        {
            var partyLike = new PartyLike() { UserId = userId, CreatedTime = DateTime.UtcNow, PartyId = partyId };

            using (var db = Heart.CreateShiJuDbContext())
            {
                var partyLikeExist = db.PartyLikes.Where(pl => pl.PartyId == partyId && pl.UserId == userId).Any();
                if (partyLikeExist)
                {
                    return this.JsonResult(-1, "this partylike has been exist.");
                }

                db.PartyLikes.Add(partyLike);
                db.SaveChanges();
                db.Parties.Where(p => p.Id == partyId).FirstOrDefault().LikeCount = db.PartyLikes.Where(p => p.PartyId == partyId).Count();
                db.SaveChanges();
                return this.JsonResult(0, "Succeeded");
            }
        }

        [HttpPost]
        [Route("Parties/{partyId}/Users/{userId}/PartyLikes/Delete")]
        public HttpResponseMessage DeletePartyLike(Guid partyId, Guid userId)
        {
            var partyLike = new PartyLike() { UserId = userId, CreatedTime = DateTime.UtcNow, PartyId = partyId };

            using (var db = Heart.CreateShiJuDbContext())
            {
                var partyLikeExist = db.PartyLikes.Where(pl => pl.PartyId == partyId && pl.UserId == userId).FirstOrDefault();
                if (partyLikeExist == null)
                {
                    return this.JsonResult(-1, "this partylike is not exist.");
                }

                db.PartyLikes.Remove(partyLikeExist);
                db.SaveChanges();

                db.Parties.Where(p => p.Id == partyId).FirstOrDefault().LikeCount = db.PartyLikes.Where(p => p.PartyId == partyId).Count();
                db.SaveChanges();

                return this.JsonResult(0, "Succeeded");
            }
        }
    }

    class PartyComparer : IEqualityComparer<Party>
    {
        public bool Equals(Party p1, Party p2)
        {
            if (p2 == null && p1 == null)
                return true;
            else if (p1.Id == p2.Id)
            {
                return true;
            }
            else
                return false;
        }

        public int GetHashCode(Party party)
        {
            return party.GetHashCode();
        }
    }
}