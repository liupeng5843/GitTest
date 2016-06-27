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
    public class UserGreeting
    {
        [Key]
        [Column(Order = 0)]
        public Guid SourceUserId { get; set; }
        [Key]
        [Column(Order = 1)]
        public Guid TargetUserId { get; set; }
        public long TotalCount { get; set; }
        public long AgreeCount { get; set; }
        public bool HasRead { get; set; }
        public bool HasNewGreeting { get; set; }
        public DateTime LastModifiedTime { get; set; }
        [NotMapped]
        public User SourceUser { get; set; }
        [NotMapped]
        public User TargetUser { get; set; }

        public class Format
        {
            public bool SourceUserId { get; set; }
            public bool TargetUserId { get; set; }
            public bool TotalCount { get; set; }
            public bool AgreeCount { get; set; }
            public bool HasRead { get; set; }
            public bool HasNewGreeting { get; set; }
            public bool LastModifiedTime { get; set; }
            public User.Format SourceUser { get; set; }
            public User.Format TargetUser { get; set; }
        }

        public UserGreeting()
        {
        }

        public UserGreeting(DbDataReader reader,
            int sourceUserIdIndex = -1,
            int targetUserIdIndex = -1,
            int totalCountIndex = -1,
            int agreeCountIndex = -1,
            int hasReadIndex = -1,
            int hasNewGreetingIndex = -1,
            int lastModifiedTimeIndex = -1,
            int sourceUserIndex = -1,
            int targetUserIndex = -1)
        {
            if (sourceUserIdIndex >= 0)
            {
                SourceUserId = (Guid)reader[sourceUserIdIndex];
            }

            if (targetUserIdIndex >= 0)
            {
                TargetUserId = (Guid)reader[targetUserIdIndex];
            }

            if (totalCountIndex >= 0)
            {
                TotalCount = (long)reader[totalCountIndex];
            }

            if (agreeCountIndex >= 0)
            {
                AgreeCount = (long)reader[agreeCountIndex];
            }

            if (hasReadIndex >= 0)
            {
                HasRead = (bool)reader[hasReadIndex];
            }

            if (hasNewGreetingIndex >= 0)
            {
                HasNewGreeting = (bool)reader[hasNewGreetingIndex];
            }

            if (lastModifiedTimeIndex >= 0)
            {
                LastModifiedTime = (DateTime)reader[lastModifiedTimeIndex];
            }

        }

        public static void Serialize(JsonWriter writer, UserGreeting item, Format format)
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

            if (format.TotalCount)
            {
                writer.Write("TotalCount", item.TotalCount);
            }

            if (format.AgreeCount)
            {
                writer.Write("AgreeCount", item.AgreeCount);
            }

            if (format.HasRead)
            {
                writer.Write("HasRead", item.HasRead);
            }

            if (format.HasNewGreeting)
            {
                writer.Write("HasNewGreeting", item.HasNewGreeting);
            }

            if (format.LastModifiedTime)
            {
                writer.Write("LastModifiedTime", item.LastModifiedTime);
            }

            if ((format.SourceUser != null) && (item.SourceUser != null))
            {
                writer.WriteName("SourceUser");
                User.Serialize(writer, item.SourceUser, format.SourceUser);
            }

            if ((format.TargetUser != null) && (item.TargetUser != null))
            {
                writer.WriteName("TargetUser");
                User.Serialize(writer, item.TargetUser, format.TargetUser);
            }

            writer.WriteEndObject();
        }

        public static void Serialize(JsonWriter writer, IList<UserGreeting> list, Format format)
        {
            writer.WriteStartArray();

            foreach (var item in list)
            {
                Serialize(writer, item, format);
            }

            writer.WriteEndArray();
        }

        public static UserGreeting Deserialize(JsonObject json)
        {
            var item = new UserGreeting()
            {
                SourceUserId = json.GetGuidValue("SourceUserId"),
                TargetUserId = json.GetGuidValue("TargetUserId"),
                TotalCount = json.GetLongValue("TotalCount"),
                AgreeCount = json.GetLongValue("AgreeCount"),
                HasRead = json.GetBooleanValue("HasRead"),
                HasNewGreeting = json.GetBooleanValue("HasNewGreeting"),
                LastModifiedTime = json.GetDateTimeValue("LastModifiedTime"),
            };

            var sourceUserJson = json.GetJsonObject("SourceUser");
            if (sourceUserJson != null)
            {
                item.SourceUser = User.Deserialize(sourceUserJson);
            }
            var targetUserJson = json.GetJsonObject("TargetUser");
            if (targetUserJson != null)
            {
                item.TargetUser = User.Deserialize(targetUserJson);
            }
            return item;
        }

        public static List<UserGreeting> Deserialize(JsonArray jsonArray)
        {
            var list = new List<UserGreeting>();

            foreach (JsonObject json in jsonArray)
            {
                list.Add(Deserialize(json));
            }

            return list;
        }
    }

}
