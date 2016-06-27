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
    public class FriendRequest
    {
        [Key]
        [Column(Order = 0)]
        public Guid SourceUserId { get; set; }
        [Key]
        [Column(Order = 1)]
        public Guid TargetUserId { get; set; }
        [Required(AllowEmptyStrings = true)]
        [StringLength(1024)]
        public string Text { get; set; }
        public DateTime CreatedTime { get; set; }
        public FriendRequestStatus Status { get; set; }
        public bool TargetUnread { get; set; }
        public bool SourceUnread { get; set; }
        [NotMapped]
        public User SourceUser { get; set; }

        public class Format
        {
            public bool SourceUserId { get; set; }
            public bool TargetUserId { get; set; }
            public bool Text { get; set; }
            public bool CreatedTime { get; set; }
            public bool Status { get; set; }
            public bool TargetUnread { get; set; }
            public bool SourceUnread { get; set; }
            public User.Format SourceUser { get; set; }
        }

        public FriendRequest()
        {
        }

        public FriendRequest(DbDataReader reader,
            int sourceUserIdIndex = -1,
            int targetUserIdIndex = -1,
            int textIndex = -1,
            int createdTimeIndex = -1,
            int statusIndex = -1,
            int targetUnreadIndex = -1,
            int sourceUnreadIndex = -1,
            int sourceUserIndex=-1)
        {
            if (sourceUserIdIndex >= 0)
            {
                SourceUserId = (Guid)reader[sourceUserIdIndex];
            }

            if (targetUserIdIndex >= 0)
            {
                TargetUserId = (Guid)reader[targetUserIdIndex];
            }

            if (textIndex >= 0)
            {
                Text = (string)reader[textIndex];
            }

            if (createdTimeIndex >= 0)
            {
                CreatedTime = (DateTime)reader[createdTimeIndex];
            }

            if (statusIndex >= 0)
            {
                Status = (FriendRequestStatus)(long)reader[statusIndex];
            }

            if (targetUnreadIndex >= 0)
            {
                TargetUnread = (bool)reader[targetUnreadIndex];
            }

            if (sourceUnreadIndex >= 0)
            {
                SourceUnread = (bool)reader[sourceUnreadIndex];
            }
        }

        public static void Serialize(JsonWriter writer, FriendRequest item, Format format)
        {
            writer.WriteStartObject();

            if (format.SourceUserId)
            {
                writer.Write("SourceUserId", item.SourceUserId);
            }

            if (format.TargetUserId)
            {
                writer.Write("TargetUserId", item.TargetUserId);
            }

            if (format.Text)
            {
                writer.Write("Text", item.Text);
            }

            if (format.CreatedTime)
            {
                writer.Write("CreatedTime", item.CreatedTime);
            }

            if (format.Status)
            {
                writer.Write("Status", (long)item.Status);
            }

            if (format.TargetUnread)
            {
                writer.Write("TargetUnread", item.TargetUnread);
            }

            if (format.SourceUnread)
            {
                writer.Write("SourceUnread", item.SourceUnread);
            }

            if (format.SourceUser != null && item.SourceUser != null)
            {
                writer.WriteName("SourceUser");
                User.Serialize(writer, item.SourceUser, format.SourceUser);
            }

            writer.WriteEndObject();
        }

        public static void Serialize(JsonWriter writer, IList<FriendRequest> list, Format format)
        {
            writer.WriteStartArray();

            foreach (var item in list)
            {
                Serialize(writer, item, format);
            }

            writer.WriteEndArray();
        }

        public static FriendRequest Deserialize(JsonObject json)
        {
            var item = new FriendRequest()
            {
                SourceUserId = json.GetGuidValue("SourceUserId"),
                TargetUserId = json.GetGuidValue("TargetUserId"),
                Text = json.GetStringValue("Text"),
                CreatedTime = json.GetDateTimeValue("CreatedTime"),
                Status = (FriendRequestStatus)json.GetLongValue("Status", (long)FriendRequestStatus.Created),
                TargetUnread = json.GetBooleanValue("TargetUnread"),
                SourceUnread = json.GetBooleanValue("SourceUnread"),
            };

            var userJson = json.GetJsonObject("SourceUser");
            if (userJson != null)
            {
                item.SourceUser = User.Deserialize(userJson);
            }
            return item;
        }

        public static List<FriendRequest> Deserialize(JsonArray jsonArray)
        {
            var list = new List<FriendRequest>();

            foreach (JsonObject json in jsonArray)
            {
                list.Add(Deserialize(json));
            }
            return list;
        }
    }


}
