using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsServer.Json;

namespace ShiJu.Models
{
    public class Account
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public AccountType Type { get; set; }
        [Required(AllowEmptyStrings = true)]
        [StringLength(64)]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = true)]
        [StringLength(64)]
        public string Password { get; set; }
        public DateTime CreatedTime { get; set; }

        public class Format
        {
            public bool Id { get; set; }
            public bool UserId { get; set; }
            public bool Type { get; set; }
            public bool Name { get; set; }
            public bool Password { get; set; }
            public bool CreatedTime { get; set; }
        }

        public Account()
        {
        }

        public Account(DbDataReader reader,
            int idIndex = -1,
            int userIdIndex = -1,
            int typeIndex = -1,
            int nameIndex = -1,
            int passwordIndex = -1,
            int createdTimeIndex = -1)
        {
            if (idIndex >= 0)
            {
                Id = (Guid)reader[idIndex];
            }

            if (userIdIndex >= 0)
            {
                UserId = (Guid)reader[userIdIndex];
            }

            if (typeIndex >= 0)
            {
                Type = (AccountType)(long)reader[typeIndex];
            }

            if (nameIndex >= 0)
            {
                Name = (string)reader[nameIndex];
            }

            if (passwordIndex >= 0)
            {
                Password = (string)reader[passwordIndex];
            }

            if (createdTimeIndex >= 0)
            {
                CreatedTime = (DateTime)reader[createdTimeIndex];
            }

        }

        public static void Serialize(JsonWriter writer, Account item, Format format)
        {
            writer.WriteStartObject();

            if (format.Id)
            {
                writer.Write("Id", item.Id);
            }

            if (format.UserId)
            {
                writer.Write("UserId", item.UserId);
            }

            if (format.Type)
            {
                writer.Write("Type", (long)item.Type);
            }

            if (format.Name)
            {
                writer.Write("Name", item.Name);
            }

            if (format.Password)
            {
                writer.Write("Password", item.Password);
            }

            if (format.CreatedTime)
            {
                writer.Write("CreatedTime", item.CreatedTime);
            }

            writer.WriteEndObject();
        }

        public static void Serialize(JsonWriter writer, IList<Account> list, Format format)
        {
            writer.WriteStartArray();

            foreach (var item in list)
            {
                Serialize(writer, item, format);
            }

            writer.WriteEndArray();
        }

        public static Account Deserialize(JsonObject json)
        {
            var item = new Account()
            {
                Id = json.GetGuidValue("Id"),
                UserId = json.GetGuidValue("UserId"),
                Type = (AccountType)json.GetLongValue("Type", (long)AccountType.PhoneNumber),
                Name = json.GetStringValue("Name"),
                Password = json.GetStringValue("Password"),
                CreatedTime = json.GetDateTimeValue("CreatedTime"),
            };

            return item;
        }

        public static List<Account> Deserialize(JsonArray jsonArray)
        {
            var list = new List<Account>();

            foreach (JsonObject json in jsonArray)
            {
                list.Add(Deserialize(json));
            }

            return list;
        }
    }


}
