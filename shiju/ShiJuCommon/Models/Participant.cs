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
    public class Participant
    {
        [Key]
        [Column(Order = 0)]
        public Guid PartyId { get; set; }
        [Key]
        [Column(Order = 1)]
        public Guid UserId { get; set; }
        public ParticipantStatus Status { get; set; }
        public DateTime ProposedBeginTime { get; set; }
        public DateTime ProposedEndTime { get; set; }
        public bool Unread { get; set; }
        public DateTime CreatedTime { get; set; }
        [NotMapped]
        public User User { get; set; }
        public Party Party { get; set; }

        public class Format
        {
            public bool PartyId { get; set; }
            public bool UserId { get; set; }
            public bool Status { get; set; }
            public bool ProposedBeginTime { get; set; }
            public bool ProposedEndTime { get; set; }
            public bool Unread { get; set; }
            public bool CreatedTime { get; set; }
            public User.Format User { get; set; }
            public Party.Format Party { get; set; }
        }

        public Participant()
        {
        }

        public Participant(DbDataReader reader,
            int partyIdIndex = -1,
            int userIdIndex = -1,
            int statusIndex = -1,
            int proposedBeginTimeIndex = -1,
            int proposedEndTimeIndex = -1,
            int unreadIndex = -1,
            int createdTimeIndex = -1,
            int userIndex = -1)
        {
            if (partyIdIndex >= 0)
            {
                PartyId = (Guid)reader[partyIdIndex];
            }

            if (userIdIndex >= 0)
            {
                UserId = (Guid)reader[userIdIndex];
            }

            if (statusIndex >= 0)
            {
                Status = (ParticipantStatus)(long)reader[statusIndex];
            }

            if (proposedBeginTimeIndex >= 0)
            {
                ProposedBeginTime = (DateTime)reader[proposedBeginTimeIndex];
            }

            if (proposedEndTimeIndex >= 0)
            {
                ProposedEndTime = (DateTime)reader[proposedEndTimeIndex];
            }

            if (unreadIndex >= 0)
            {
                Unread = (bool)reader[unreadIndex];
            }

            if (createdTimeIndex >= 0)
            {
                CreatedTime = (DateTime)reader[createdTimeIndex];
            }

        }

        public static void Serialize(JsonWriter writer, Participant item, Format format)
        {
            writer.WriteStartObject();

            if (format.PartyId)
            {
                writer.Write("PartyId", item.PartyId);
            }

            if (format.UserId)
            {
                writer.Write("UserId", item.UserId);
            }

            if (format.Status)
            {
                writer.Write("Status", (long)item.Status);
            }

            if (format.ProposedBeginTime)
            {
                writer.Write("ProposedBeginTime", item.ProposedBeginTime);
            }

            if (format.ProposedEndTime)
            {
                writer.Write("ProposedEndTime", item.ProposedEndTime);
            }

            if (format.Unread)
            {
                writer.Write("Unread", item.Unread);
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

            if ((format.Party != null) && (item.Party != null))
            {
                writer.WriteName("Party");
                Party.Serialize(writer, item.Party, format.Party);
            }

            writer.WriteEndObject();
        }

        public static void Serialize(JsonWriter writer, IList<Participant> list, Format format)
        {
            writer.WriteStartArray();

            foreach (var item in list)
            {
                Serialize(writer, item, format);
            }

            writer.WriteEndArray();
        }

        public static Participant Deserialize(JsonObject json)
        {
            var item = new Participant()
            {
                PartyId = json.GetGuidValue("PartyId"),
                UserId = json.GetGuidValue("UserId"),
                Status = (ParticipantStatus)json.GetLongValue("Status", (long)ParticipantStatus.Created),
                ProposedBeginTime = json.GetDateTimeValue("ProposedBeginTime"),
                ProposedEndTime = json.GetDateTimeValue("ProposedEndTime"),
                Unread = json.GetBooleanValue("Unread"),
                CreatedTime = json.GetDateTimeValue("CreatedTime"),
            };

            var userJson = json.GetJsonObject("User");
            if (userJson != null)
            {
                item.User = User.Deserialize(userJson);
            }
            return item;
        }

        public static List<Participant> Deserialize(JsonArray jsonArray)
        {
            var list = new List<Participant>();

            foreach (JsonObject json in jsonArray)
            {
                list.Add(Deserialize(json));
            }

            return list;
        }
    }


}
