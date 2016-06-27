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
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public UserStatus Status { get; set; }
        [Required(AllowEmptyStrings = true)]
        [StringLength(64)]
        public string PhoneNumber { get; set; }
        [Required(AllowEmptyStrings = true)]
        [StringLength(128)]
        public string NickName { get; set; }
        [Required(AllowEmptyStrings = true)]
        [StringLength(512)]
        public string Signature { get; set; }
        public Guid Portrait { get; set; }
        public UserGender Gender { get; set; }
        [Required(AllowEmptyStrings = true)]
        [StringLength(128)]
        public string District { get; set; }
        public Guid BackgroundImage { get; set; }
        public bool NeedNotification { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime SignUpTime { get; set; }
        [NotMapped]
        public bool IsFriend { get; set; }

        public class Format
        {
            public bool Id { get; set; }
            public bool Status { get; set; }
            public bool PhoneNumber { get; set; }
            public bool NickName { get; set; }
            public bool Signature { get; set; }
            public bool Portrait { get; set; }
            public bool Gender { get; set; }
            public bool District { get; set; }
            public bool BackgroundImage { get; set; }
            public bool NeedNotification { get; set; }
            public bool CreatedTime { get; set; }
            public bool SignUpTime { get; set; }
            public bool IsFriend { get; set; }
        }

        public User()
        {
        }

        public User(DbDataReader reader,
            int idIndex = -1,
            int statusIndex = -1,
            int phoneNumberIndex = -1,
            int nickNameIndex = -1,
            int signatureIndex = -1,
            int portraitIndex = -1,
            int genderIndex = -1,
            int districtIndex = -1,
            int backgroundImageIndex = -1,
            int needNotificationIndex=-1,
            int createdTimeIndex = -1,
            int signUpTimeIndex = -1,
            int isFriendIndex=-1)
        {
            if (idIndex >= 0)
            {
                Id = (Guid)reader[idIndex];
            }

            if (statusIndex >= 0)
            {
                Status = (UserStatus)(long)reader[statusIndex];
            }

            if (phoneNumberIndex >= 0)
            {
                PhoneNumber = (string)reader[phoneNumberIndex];
            }

            if (nickNameIndex >= 0)
            {
                NickName = (string)reader[nickNameIndex];
            }

            if (signatureIndex >= 0)
            {
                Signature = (string)reader[signatureIndex];
            }

            if (portraitIndex >= 0)
            {
                Portrait = (Guid)reader[portraitIndex];
            }

            if (genderIndex >= 0)
            {
                Gender = (UserGender)(long)reader[genderIndex];
            }

            if (districtIndex >= 0)
            {
                District = (string)reader[districtIndex];
            }

            if (backgroundImageIndex >= 0)
            {
                BackgroundImage = (Guid)reader[backgroundImageIndex];
            }

            if (needNotificationIndex >= 0)
            {
                NeedNotification = (bool)reader[needNotificationIndex];
            }

            if (createdTimeIndex >= 0)
            {
                CreatedTime = (DateTime)reader[createdTimeIndex];
            }

            if (signUpTimeIndex >= 0)
            {
                SignUpTime = (DateTime)reader[signUpTimeIndex];
            }

            if (isFriendIndex >= 0)
            {
                IsFriend = (bool)reader[isFriendIndex];
            }
        }

        public static void Serialize(JsonWriter writer, User item, Format format)
        {
            writer.WriteStartObject();

            if (format.Id)
            {
                writer.Write("Id", item.Id);
            }

            if (format.Status)
            {
                writer.Write("Status", (long)item.Status);
            }

            if (format.PhoneNumber)
            {
                writer.Write("PhoneNumber", item.PhoneNumber);
            }

            if (format.NickName)
            {
                writer.Write("NickName", item.NickName);
            }

            if (format.Signature)
            {
                writer.Write("Signature", item.Signature);
            }

            if (format.Portrait)
            {
                writer.Write("Portrait", item.Portrait);
            }

            if (format.Gender)
            {
                writer.Write("Gender", (long)item.Gender);
            }

            if (format.District)
            {
                writer.Write("District", item.District);
            }

            if (format.BackgroundImage)
            {
                writer.Write("BackgroundImage", item.BackgroundImage);
            }

            if (format.NeedNotification)
            {
                writer.Write("NeedNotification", item.NeedNotification);
            }

            if (format.CreatedTime)
            {
                writer.Write("CreatedTime", item.CreatedTime);
            }

            if (format.SignUpTime)
            {
                writer.Write("SignUpTime", item.SignUpTime);
            }

            if (format.IsFriend)
            {
                writer.Write("IsFriend", item.IsFriend);
            }

            writer.WriteEndObject();
        }

        public static void Serialize(JsonWriter writer, IList<User> list, Format format)
        {
            writer.WriteStartArray();

            foreach (var item in list)
            {
                Serialize(writer, item, format);
            }

            writer.WriteEndArray();
        }

        public static User Deserialize(JsonObject json)
        {
            var item = new User()
            {
                Id = json.GetGuidValue("Id"),
                Status = (UserStatus)json.GetLongValue("Status", (long)UserStatus.Active),
                PhoneNumber = json.GetStringValue("PhoneNumber"),
                NickName = json.GetStringValue("NickName"),
                Signature = json.GetStringValue("Signature"),
                Portrait = json.GetGuidValue("Portrait"),
                Gender = (UserGender)json.GetLongValue("Gender", (long)UserGender.Unknown),
                District = json.GetStringValue("District"),
                BackgroundImage = json.GetGuidValue("BackgroundImage"),
                NeedNotification = json.GetBooleanValue("NeedNotification"),
                CreatedTime = json.GetDateTimeValue("CreatedTime"),
                SignUpTime = json.GetDateTimeValue("SignUpTime"),
            };

            return item;
        }

        public static List<User> Deserialize(JsonArray jsonArray)
        {
            var list = new List<User>();

            foreach (JsonObject json in jsonArray)
            {
                list.Add(Deserialize(json));
            }

            return list;
        }
    }


}
