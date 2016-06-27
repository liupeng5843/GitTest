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
    public class Friend
    {
        [Key]
        [Column(Order = 0)]
        public Guid UserId { get; set; }
        [Key]
        [Column(Order = 1)]
        public Guid FriendUserId { get; set; }
        public DateTime CreatedTime { get; set; }
        [NotMapped]
        public User FriendUser { get; set; }

        public class Format
        {
            public bool UserId { get; set; }
            public bool FriendUserId { get; set; }
            public bool CreatedTime { get; set; }
            public User.Format FriendUser { get; set; }
        }

        public Friend()
        {
        }

        public Friend(DbDataReader reader,
            int userIdIndex = -1,
            int friendUserIdIndex = -1,
            int createdTimeIndex = -1,
            int friendUserIndex = -1)
        {
            if (userIdIndex >= 0)
            {
                UserId = (Guid)reader[userIdIndex];
            }

            if (friendUserIdIndex >= 0)
            {
                FriendUserId = (Guid)reader[friendUserIdIndex];
            }

            if (createdTimeIndex >= 0)
            {
                CreatedTime = (DateTime)reader[createdTimeIndex];
            }

        }

        public static void Serialize(JsonWriter writer, Friend item, Format format)
        {
            writer.WriteStartObject();

            if (format.UserId)
            {
                writer.Write("UserId", item.UserId);
            }

            if (format.FriendUserId)
            {
                writer.Write("FriendUserId", item.FriendUserId);
            }

            if (format.CreatedTime)
            {
                writer.Write("CreatedTime", item.CreatedTime);
            }

            if ((format.FriendUser != null) && (item.FriendUser != null))
            {
                writer.WriteName("FriendUser");
                User.Serialize(writer, item.FriendUser, format.FriendUser);
            }

            writer.WriteEndObject();
        }

        public static void Serialize(JsonWriter writer, IList<Friend> list, Format format)
        {
            writer.WriteStartArray();

            foreach (var item in list)
            {
                Serialize(writer, item, format);
            }

            writer.WriteEndArray();
        }

        public static Friend Deserialize(JsonObject json)
        {
            var item = new Friend()
            {
                UserId = json.GetGuidValue("UserId"),
                FriendUserId = json.GetGuidValue("FriendUserId"),
                CreatedTime = json.GetDateTimeValue("CreatedTime"),
            };

            var friendUserJson = json.GetJsonObject("FriendUser");
            if (friendUserJson != null)
            {
                item.FriendUser = User.Deserialize(friendUserJson);
            }
            return item;
        }

        public static List<Friend> Deserialize(JsonArray jsonArray)
        {
            var list = new List<Friend>();

            foreach (JsonObject json in jsonArray)
            {
                list.Add(Deserialize(json));
            }

            return list;
        }
    }


}
