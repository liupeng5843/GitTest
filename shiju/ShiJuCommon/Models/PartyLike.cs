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
    public class PartyLike
    {
        [Key]
        [Column(Order = 0)]
        public Guid PartyId { get; set; }
        [Key]
        [Column(Order = 1)]
        public Guid UserId { get; set; }
        public DateTime CreatedTime { get; set; }

        public class Format
        {
            public bool PartyId { get; set; }
            public bool UserId { get; set; }
            public bool CreatedTime { get; set; }
        }

        public PartyLike()
        {
        }

        public PartyLike(DbDataReader reader,
            int partyIdIndex = -1,
            int userIdIndex = -1,
            int createdTimeIndex = -1)
        {
            if (partyIdIndex >= 0)
            {
                PartyId = (Guid)reader[partyIdIndex];
            }

            if (userIdIndex >= 0)
            {
                UserId = (Guid)reader[userIdIndex];
            }

            if (createdTimeIndex >= 0)
            {
                CreatedTime = (DateTime)reader[createdTimeIndex];
            }

        }

        public static void Serialize(JsonWriter writer, PartyLike item, Format format)
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

            if (format.CreatedTime)
            {
                writer.Write("CreatedTime", item.CreatedTime);
            }

            writer.WriteEndObject();
        }

        public static void Serialize(JsonWriter writer, IList<PartyLike> list, Format format)
        {
            writer.WriteStartArray();

            foreach (var item in list)
            {
                Serialize(writer, item, format);
            }

            writer.WriteEndArray();
        }

        public static PartyLike Deserialize(JsonObject json)
        {
            var item = new PartyLike()
            {
                PartyId = json.GetGuidValue("PartyId"),
                UserId = json.GetGuidValue("UserId"),
                CreatedTime = json.GetDateTimeValue("CreatedTime"),
            };

            return item;
        }

        public static List<PartyLike> Deserialize(JsonArray jsonArray)
        {
            var list = new List<PartyLike>();

            foreach (JsonObject json in jsonArray)
            {
                list.Add(Deserialize(json));
            }

            return list;
        }
    }


}
