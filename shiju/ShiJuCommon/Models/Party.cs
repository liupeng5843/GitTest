using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsServer.Json;

namespace ShiJu.Models
{
    public class Party
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CreatorUserId { get; set; }
        [StringLength(128)]
        public string Sponsor { get; set; }//for the cms add the sponsor
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        [Required(AllowEmptyStrings = true)]
        [StringLength(128)]
        public string Title { get; set; }
        [Required(AllowEmptyStrings = true)]
        [StringLength(1024)]
        public string Description { get; set; }
        [Required(AllowEmptyStrings = true)]
        [StringLength(128)]
        public string Address { get; set; }
        [Required(AllowEmptyStrings = true)]
        public string Images { get; set; }
        [Required(AllowEmptyStrings = true)]
        [StringLength(64)]
        public string Kind { get; set; }
        public long MaxUserCount { get; set; }
        public bool DirectFriendVisible { get; set; }
        public bool IsPublic { get; set; }
        public long LikeCount { get; set; }
        public long CommentCount { get; set; }
        [Required(AllowEmptyStrings = true)]
        [StringLength(256)]
        public string VoteTitle { get; set; }
        [Required(AllowEmptyStrings = true)]
        public string VoteChoicesJson { get; set; }
        public long VoteResult0Count { get; set; }
        public long VoteResult1Count { get; set; }
        public long VoteResult2Count { get; set; }
        public long VoteResult3Count { get; set; }
        public long VoteResult4Count { get; set; }
        public bool IsDisabled { get; set; }
        public DateTime CreatedTime { get; set; }
        [NotMapped]
        public long ParticipantCount { get; set; }
        [NotMapped]
        public bool IsUserLiked { get; set; }
        [NotMapped]
        public bool IsUserVoted { get; set; }
        [NotMapped]
        public Participant UserParticipant { get; set; }
        [NotMapped]
        public User CreatorUser { get; set; }
        [NotMapped]
        public List<Participant> Participants { get; set; }
        [NotMapped]
        public List<PartyComment> PartyComments { get; set; }

        public class Format
        {
            public bool Id { get; set; }
            public bool CreatorUserId { get; set; }
            public bool Sponsor { get; set; }
            public bool BeginTime { get; set; }
            public bool EndTime { get; set; }
            public bool Title { get; set; }
            public bool Description { get; set; }
            public bool Address { get; set; }
            public bool Images { get; set; }
            public bool Kind { get; set; }
            public bool MaxUserCount { get; set; }
            public bool DirectFriendVisible { get; set; }
            public bool IsPublic { get; set; }
            public bool LikeCount { get; set; }
            public bool CommentCount { get; set; }
            public bool VoteTitle { get; set; }
            public bool VoteChoicesJson { get; set; }
            public bool VoteResult0Count { get; set; }
            public bool VoteResult1Count { get; set; }
            public bool VoteResult2Count { get; set; }
            public bool VoteResult3Count { get; set; }
            public bool VoteResult4Count { get; set; }
            public bool IsDisabled { get; set; }
            public bool CreatedTime { get; set; }
            public bool IsUserLiked { get; set; }
            public bool IsUserVoted { get; set; }
            public bool ParticipantCount { get; set; }
            public Participant.Format UserParticipant { get; set; }
            public User.Format CreatorUser { get; set; }
            public Participant.Format Participants { get; set; }
            public PartyComment.Format PartyComments { get; set; }
        }

        public Party()
        {
        }

        public Party(DbDataReader reader,
            int idIndex = -1,
            int creatorUserIdIndex = -1,
            int sponsorIndex=-1,
            int beginTimeIndex = -1,
            int endTimeIndex = -1,
            int titleIndex = -1,
            int descriptionIndex = -1,
            int addressIndex = -1,
            int imagesIndex = -1,
            int kindIndex = -1,
            int maxUserCountIndex = -1,
            int directFriendVisibleIndex = -1,
            int isPublicIndex = -1,
            int likeCountIndex = -1,
            int commentCountIndex = -1,
            int participantCountIndex=-1,
            int voteTitleIndex = -1,
            int voteChoicesJsonIndex = -1,
            int voteResult0CountIndex = -1,
            int voteResult1CountIndex = -1,
            int voteResult2CountIndex = -1,
            int voteResult3CountIndex = -1,
            int voteResult4CountIndex = -1,
            int isDisabledIndex = -1,
            int isUserLikedIndex=-1,
            int isUserVotedIndex=-1,
            int userParticipantIndex=-1,
            int createdTimeIndex = -1,
            int creatorUserIndex = -1,
            int participantsIndex = -1,
            int partyComments=-1)
        {
            if (idIndex >= 0)
            {
                Id = (Guid)reader[idIndex];
            }

            if (creatorUserIdIndex >= 0)
            {
                CreatorUserId = (Guid)reader[creatorUserIdIndex];
            }

            if (sponsorIndex >= 0)
            {
                Sponsor = (string)reader[sponsorIndex];
            }

            if (beginTimeIndex >= 0)
            {
                BeginTime = (DateTime)reader[beginTimeIndex];
            }

            if (endTimeIndex >= 0)
            {
                EndTime = (DateTime)reader[endTimeIndex];
            }

            if (titleIndex >= 0)
            {
                Title = (string)reader[titleIndex];
            }

            if (descriptionIndex >= 0)
            {
                Description = (string)reader[descriptionIndex];
            }

            if (addressIndex >= 0)
            {
                Address = (string)reader[addressIndex];
            }

            if (imagesIndex >= 0)
            {
                Images = (string)reader[imagesIndex];
            }

            if (kindIndex >= 0)
            {
                Kind = (string)reader[kindIndex];
            }

            if (maxUserCountIndex >= 0)
            {
                MaxUserCount = (long)reader[maxUserCountIndex];
            }

            if (directFriendVisibleIndex >= 0)
            {
                DirectFriendVisible = (bool)reader[directFriendVisibleIndex];
            }

            if (isPublicIndex >= 0)
            {
                IsPublic = (bool)reader[isPublicIndex];
            }

            if (likeCountIndex >= 0)
            {
                LikeCount = (long)reader[likeCountIndex];
            }

            if (commentCountIndex >= 0)
            {
                CommentCount = (long)reader[commentCountIndex];
            }

            if (participantCountIndex >= 0)
            {
                ParticipantCount = (long)reader[participantCountIndex];
            }

            if (voteTitleIndex >= 0)
            {
                VoteTitle = (string)reader[voteTitleIndex];
            }

            if (voteChoicesJsonIndex >= 0)
            {
                VoteChoicesJson = (string)reader[voteChoicesJsonIndex];
            }

            if (voteResult0CountIndex >= 0)
            {
                VoteResult0Count = (long)reader[voteResult0CountIndex];
            }

            if (voteResult1CountIndex >= 0)
            {
                VoteResult1Count = (long)reader[voteResult1CountIndex];
            }

            if (voteResult2CountIndex >= 0)
            {
                VoteResult2Count = (long)reader[voteResult2CountIndex];
            }

            if (voteResult3CountIndex >= 0)
            {
                VoteResult3Count = (long)reader[voteResult3CountIndex];
            }

            if (voteResult4CountIndex >= 0)
            {
                VoteResult4Count = (long)reader[voteResult4CountIndex];
            }

            if (isDisabledIndex >= 0)
            {
                IsDisabled = (bool)reader[isDisabledIndex];
            }

            if (isUserLikedIndex >= 0)
            {
                IsUserLiked = (bool)reader[isUserLikedIndex];
            }

            if (isUserVotedIndex >= 0)
            {
                IsUserVoted = (bool)reader[isUserVotedIndex];
            }

            if (createdTimeIndex >= 0)
            {
                CreatedTime = (DateTime)reader[createdTimeIndex];
            }

        }

        public static void Serialize(JsonWriter writer, Party item, Format format)
        {
            writer.WriteStartObject();

            if (format.Id)
            {
                writer.Write("Id", item.Id);
            }

            if (format.CreatorUserId)
            {
                writer.Write("CreatorUserId", item.CreatorUserId);
            }

            if (format.Sponsor)
            {
                writer.Write("Sponsor", item.Sponsor);
            }

            if (format.BeginTime)
            {
                writer.Write("BeginTime", item.BeginTime);
            }

            if (format.EndTime)
            {
                writer.Write("EndTime", item.EndTime);
            }

            if (format.Title)
            {
                writer.Write("Title", item.Title);
            }

            if (format.Description)
            {
                writer.Write("Description", item.Description);
            }

            if (format.Address)
            {
                writer.Write("Address", item.Address);
            }

            if (format.Images)
            {
                writer.Write("Images", item.Images);
            }

            if (format.Kind)
            {
                writer.Write("Kind", item.Kind);
            }

            if (format.MaxUserCount)
            {
                writer.Write("MaxUserCount", item.MaxUserCount);
            }

            if (format.DirectFriendVisible)
            {
                writer.Write("DirectFriendVisible", item.DirectFriendVisible);
            }

            if (format.IsPublic)
            {
                writer.Write("IsPublic", item.IsPublic);
            }

            if (format.LikeCount)
            {
                writer.Write("LikeCount", item.LikeCount);
            }

            if (format.CommentCount)
            {
                writer.Write("CommentCount", item.CommentCount);
            }

            if (format.ParticipantCount)
            {
                writer.Write("ParticipantCount", item.ParticipantCount);
            }

            if (format.VoteTitle)
            {
                writer.Write("VoteTitle", item.VoteTitle);
            }

            if (format.VoteChoicesJson)
            {
                writer.Write("VoteChoicesJson", item.VoteChoicesJson);
            }

            if (format.VoteResult0Count)
            {
                writer.Write("VoteResult0Count", item.VoteResult0Count);
            }

            if (format.VoteResult1Count)
            {
                writer.Write("VoteResult1Count", item.VoteResult1Count);
            }

            if (format.VoteResult2Count)
            {
                writer.Write("VoteResult2Count", item.VoteResult2Count);
            }

            if (format.VoteResult3Count)
            {
                writer.Write("VoteResult3Count", item.VoteResult3Count);
            }

            if (format.VoteResult4Count)
            {
                writer.Write("VoteResult4Count", item.VoteResult4Count);
            }

            if (format.IsDisabled)
            {
                writer.Write("IsDisabled", item.IsDisabled);
            }

            if (format.CreatedTime)
            {
                writer.Write("CreatedTime", item.CreatedTime);
            }

            if (format.IsUserLiked)
            {
                writer.Write("IsUserLiked", item.IsUserLiked);
            }

            if (format.IsUserVoted)
            {
                writer.Write("IsUserVoted", item.IsUserVoted);
            }

            if (format.CreatedTime)
            {
                writer.Write("CreatedTime", item.CreatedTime);
            }

            if ((format.UserParticipant != null) && (item.UserParticipant != null))
            {
                writer.WriteName("UserParticipant");
                Participant.Serialize(writer, item.UserParticipant, format.UserParticipant);
            }

            if ((format.CreatorUser != null) && (item.CreatorUser != null))
            {
                writer.WriteName("CreatorUser");
                User.Serialize(writer, item.CreatorUser, format.CreatorUser);
            }

            if ((format.Participants != null) && (item.Participants != null))
            {
                writer.WriteName("Participants");
                Participant.Serialize(writer, item.Participants, format.Participants);
            }

            if ((format.PartyComments != null) && (item.PartyComments != null))
            {
                writer.WriteName("PartyComments");
                PartyComment.Serialize(writer, item.PartyComments, format.PartyComments);
            }

            writer.WriteEndObject();
        }

        public static void Serialize(JsonWriter writer, IList<Party> list, Format format)
        {
            writer.WriteStartArray();

            foreach (var item in list)
            {
                Serialize(writer, item, format);
            }

            writer.WriteEndArray();
        }

        public static Party Deserialize(JsonObject json)
        {
            var item = new Party()
            {
                Id = json.GetGuidValue("Id"),
                CreatorUserId = json.GetGuidValue("CreatorUserId"),
                Sponsor = json.GetStringValue("Sponsor"),
                BeginTime = json.GetDateTimeValue("BeginTime"),
                EndTime = json.GetDateTimeValue("EndTime"),
                Title = json.GetStringValue("Title"),
                Description = json.GetStringValue("Description"),
                Address = json.GetStringValue("Address"),
                Images = json.GetStringValue("Images"),
                Kind = json.GetStringValue("Kind"),
                MaxUserCount = json.GetLongValue("MaxUserCount"),
                DirectFriendVisible = json.GetBooleanValue("DirectFriendVisible"),
                IsPublic = json.GetBooleanValue("IsPublic"),
                LikeCount = json.GetLongValue("LikeCount"),
                CommentCount = json.GetLongValue("CommentCount"),
                VoteTitle = json.GetStringValue("VoteTitle"),
                VoteChoicesJson = json.GetStringValue("VoteChoicesJson"),
                VoteResult0Count = json.GetLongValue("VoteResult0Count"),
                VoteResult1Count = json.GetLongValue("VoteResult1Count"),
                VoteResult2Count = json.GetLongValue("VoteResult2Count"),
                VoteResult3Count = json.GetLongValue("VoteResult3Count"),
                VoteResult4Count = json.GetLongValue("VoteResult4Count"),
                IsDisabled = json.GetBooleanValue("IsDisabled"),
                CreatedTime = json.GetDateTimeValue("CreatedTime"),
                ParticipantCount = json.GetLongValue("ParticipantCount"),
            };

            var userParticipantsJson = json.GetJsonObject("UserParticipant");
            if (userParticipantsJson != null)
            {
                item.UserParticipant = Participant.Deserialize(userParticipantsJson);
            }

            var creatorUserJson = json.GetJsonObject("CreatorUser");
            if (creatorUserJson != null)
            {
                item.CreatorUser = User.Deserialize(creatorUserJson);
            }
            var participantsJson = json.GetJsonArray("Participants");
            if (participantsJson != null)
            {
                item.Participants = Participant.Deserialize(participantsJson);
            }
            var partyCommentsJson = json.GetJsonArray("PartyComments");
            if (partyCommentsJson != null)
            {
                item.PartyComments = PartyComment.Deserialize(partyCommentsJson);
            }
            return item;
        }

        public static List<Party> Deserialize(JsonArray jsonArray)
        {
            var list = new List<Party>();

            foreach (JsonObject json in jsonArray)
            {
                list.Add(Deserialize(json));
            }

            return list;
        }
    }


}
