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
    public class Greeting
    {
        [Key]
        public int Id { get; set; }
        public Guid SourceUserId { get; set; }
        public Guid TargetUserId { get; set; }
        [Required(AllowEmptyStrings = true)]
        [StringLength(1024)]
        public string TargetPhoneNumber { get; set; }
        public bool IsAgreed { get; set; }
        public DateTime CreatedTime { get; set; }
        [NotMapped]
        public User SourceUser { get; set; }
        [NotMapped]
        public User TargetUser { get; set; }

        public class Format
        {
            public bool Id { get; set; }
            public bool SourceUserId { get; set; }
            public bool TargetUserId { get; set; }
            public bool TargetPhoneNumber { get; set; }
            public bool IsAgreed { get; set; }
            public bool CreatedTime { get; set; }
            public User.Format SourceUser { get; set; }
            public User.Format TargetUser { get; set; }
        }

        public Greeting()
        {
        }

        public Greeting(DbDataReader reader,
            int idIndex = -1,
            int sourceUserIdIndex = -1,
            int targetUserIdIndex = -1,
            int targetPhoneNumberIndex = -1,
            int isAgreedIndex = -1,
            int createdTimeIndex = -1,
            int sourceUserIndex = -1,
            int targetUserIndex = -1)
        {
            if (idIndex >= 0)
            {
                Id = (int)reader[idIndex];
            }

            if (sourceUserIdIndex >= 0)
            {
                SourceUserId = (Guid)reader[sourceUserIdIndex];
            }

            if (targetUserIdIndex >= 0)
            {
                TargetUserId = (Guid)reader[targetUserIdIndex];
            }

            if (targetPhoneNumberIndex >= 0)
            {
                TargetPhoneNumber = (string)reader[targetPhoneNumberIndex];
            }

            if (isAgreedIndex >= 0)
            {
                IsAgreed = (bool)reader[isAgreedIndex];
            }

            if (createdTimeIndex >= 0)
            {
                CreatedTime = (DateTime)reader[createdTimeIndex];
            }

        }

        public static void Serialize(JsonWriter writer, Greeting item, Format format)
        {
            writer.WriteStartObject();

            if (format.Id)
            {
                writer.Write("Id", item.Id);
            }

            if (format.SourceUserId)
            {
                writer.Write("SourceUserId", item.SourceUserId);
            }

            if (format.TargetUserId)
            {
                writer.Write("TargetUserId", item.TargetUserId);
            }

            if (format.TargetPhoneNumber)
            {
                writer.Write("TargetPhoneNumber", item.TargetPhoneNumber);
            }

            if (format.IsAgreed)
            {
                writer.Write("IsAgreed", item.IsAgreed);
            }

            if (format.CreatedTime)
            {
                writer.Write("CreatedTime", item.CreatedTime);
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

        public static void Serialize(JsonWriter writer, IList<Greeting> list, Format format)
        {
            writer.WriteStartArray();

            foreach (var item in list)
            {
                Serialize(writer, item, format);
            }

            writer.WriteEndArray();
        }

        public static Greeting Deserialize(JsonObject json)
        {
            var item = new Greeting()
            {
                Id = json.GetIntValue("Id"),
                SourceUserId = json.GetGuidValue("SourceUserId"),
                TargetUserId = json.GetGuidValue("TargetUserId"),
                TargetPhoneNumber = json.GetStringValue("TargetPhoneNumber"),
                IsAgreed = json.GetBooleanValue("IsAgreed"),
                CreatedTime = json.GetDateTimeValue("CreatedTime"),
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

        public static List<Greeting> Deserialize(JsonArray jsonArray)
        {
            var list = new List<Greeting>();

            foreach (JsonObject json in jsonArray)
            {
                list.Add(Deserialize(json));
            }

            return list;
        }
    }



}
