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
    public class PartyComment
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PartyId { get; set; }
        public Guid UserId { get; set; }
        public Guid TargetUserId { get; set; }
        [Required(AllowEmptyStrings = true)]
        [StringLength(1024)]
        public string Text { get; set; }
        public string AudioJson { get; set; }
        [Required(AllowEmptyStrings = true)]
        [StringLength(128)]
        public string VoteResult { get; set; }
        public DateTime CreatedTime { get; set; }
        [NotMapped]
        public User User { get; set; }
        [NotMapped]
        public User TargetUser { get; set; }

        public class Format
        {
            public bool Id { get; set; }
            public bool PartyId { get; set; }
            public bool UserId { get; set; }
            public bool TargetUserId { get; set; }
            public bool Text { get; set; }
            public bool AudioJson { get; set; }
            public bool VoteResult { get; set; }
            public bool CreatedTime { get; set; }
            public User.Format User { get; set; }
            public User.Format TargetUser { get; set; }
        }

        public PartyComment()
        {
        }

        public PartyComment(DbDataReader reader,
            int idIndex = -1,
            int partyIdIndex = -1,
            int userIdIndex = -1,
            int targetUserIdIndex = -1,
            int textIndex = -1,
            int audioJsonIndex = -1,
            int voteResultIndex = -1,
            int createdTimeIndex = -1,
            int userIndex = -1,
            int targetUserIndex = -1)
        {
            if (idIndex >= 0)
            {
                Id = (Guid)reader[idIndex];
            }

            if (partyIdIndex >= 0)
            {
                PartyId = (Guid)reader[partyIdIndex];
            }

            if (userIdIndex >= 0)
            {
                UserId = (Guid)reader[userIdIndex];
            }

            if (targetUserIdIndex >= 0)
            {
                TargetUserId = (Guid)reader[targetUserIdIndex];
            }

            if (textIndex >= 0)
            {
                Text = (string)reader[textIndex];
            }

            if (audioJsonIndex >= 0)
            {
                AudioJson = (string)reader[audioJsonIndex];
            }

            if (voteResultIndex >= 0)
            {
                VoteResult = (string)reader[voteResultIndex];
            }

            if (createdTimeIndex >= 0)
            {
                CreatedTime = (DateTime)reader[createdTimeIndex];
            }

        }

        public static void Serialize(JsonWriter writer, PartyComment item, Format format)
        {
            writer.WriteStartObject();

            if (format.Id)
            {
                writer.Write("Id", item.Id);
            }

            if (format.PartyId)
            {
                writer.Write("PartyId", item.PartyId);
            }

            if (format.UserId)
            {
                writer.Write("UserId", item.UserId);
            }

            if (format.TargetUserId)
            {
                writer.Write("TargetUserId", item.TargetUserId);
            }

            if (format.Text)
            {
                writer.Write("Text", item.Text);
            }

            if (format.AudioJson)
            {
                writer.Write("AudioJson", item.AudioJson);
            }

            if (format.VoteResult)
            {
                writer.Write("VoteResult", item.VoteResult);
            }

            if (format.CreatedTime)
            {
                writer.Write("CreatedTime", item.CreatedTime);
            }

            if ((format.User != null) && (item.User != null))
            {
                writer.WriteName("User");
                User.Serialize(writer, item.User, format.User);
            }

            if ((format.TargetUser != null) && (item.TargetUser != null))
            {
                writer.WriteName("TargetUser");
                User.Serialize(writer, item.TargetUser, format.TargetUser);
            }

            writer.WriteEndObject();
        }

        public static void Serialize(JsonWriter writer, IList<PartyComment> list, Format format)
        {
            writer.WriteStartArray();

            foreach (var item in list)
            {
                Serialize(writer, item, format);
            }

            writer.WriteEndArray();
        }

        public static PartyComment Deserialize(JsonObject json)
        {
            var item = new PartyComment()
            {
                Id = json.GetGuidValue("Id"),
                PartyId = json.GetGuidValue("PartyId"),
                UserId = json.GetGuidValue("UserId"),
                TargetUserId = json.GetGuidValue("TargetUserId"),
                Text = json.GetStringValue("Text"),
                AudioJson = json.GetStringValue("AudioJson"),
                VoteResult = json.GetStringValue("VoteResult"),
                CreatedTime = json.GetDateTimeValue("CreatedTime"),
            };

            var userJson = json.GetJsonObject("User");
            if (userJson != null)
            {
                item.User = User.Deserialize(userJson);
            }
            var targetUserJson = json.GetJsonObject("TargetUser");
            if (targetUserJson != null)
            {
                item.TargetUser = User.Deserialize(targetUserJson);
            }
            return item;
        }

        public static List<PartyComment> Deserialize(JsonArray jsonArray)
        {
            var list = new List<PartyComment>();

            foreach (JsonObject json in jsonArray)
            {
                list.Add(Deserialize(json));
            }

            return list;
        }
    }



}
