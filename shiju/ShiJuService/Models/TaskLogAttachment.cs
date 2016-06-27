using ShiJu.Models;
using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Web;
using WindowsServer.Json;

namespace ShiJu.Service.Models
{
    public class TaskLogAttachment
    {
        private static readonly PartyComment.Format _taskLogFormat = new PartyComment.Format()
        {
            Id = true,
            CreatedTime = true,
            UserId = true,
            TargetUserId = true,
        };

        public Guid Id { get; set; }
        public string Mime { get; set; }
        public long Size { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Properties { get; set; }

        public PartyComment Message { get; set; }

        public static List<TaskLogAttachment> DeserializeLogAttachments(string jsonString)
        {
            var attachments = new List<TaskLogAttachment>();

            var jsonObject = JsonValue.Parse(jsonString) as JsonObject;
            var json = jsonObject.GetJsonArray("Attachments");
            if (json != null)
            {
                foreach (JsonObject item in json)
                {
                    var attachment = new TaskLogAttachment()
                    {
                        Properties = new Dictionary<string, string>(),
                    };
                    foreach (var property in item)
                    {
                        switch (property.Key)
                        {
                            case "Id":
                                attachment.Id = (Guid)property.Value;
                                break;
                            case "Mime":
                                attachment.Mime = (string)property.Value;
                                break;
                            case "Size":
                                attachment.Size = (long)property.Value;
                                break;
                            case "Name":
                                attachment.Name = (string)property.Value;
                                break;
                            default:
                                attachment.Properties.Add(property.Key, property.Value.ToString());
                                break;
                        }
                    }
                    attachments.Add(attachment);
                }
            }
            return attachments;
        }

        public static void Serialize(JsonWriter writer, TaskLogAttachment attachment)
        {
            writer.WriteStartObject();

            writer.Write("Id", attachment.Id);
            if (attachment.Mime != null)
            {
                writer.Write("Mime", attachment.Mime);
            }
            writer.Write("Size", attachment.Size);
            if (attachment.Name != null)
            {
                writer.Write("Name", attachment.Name);
            }

            foreach (var pair in attachment.Properties)
            {
                writer.Write(pair.Key, pair.Value);
            }

            if (attachment.Message != null)
            {
                writer.WriteName("TaskLog");
                PartyComment.Serialize(writer, attachment.Message, _taskLogFormat);
            }

            writer.WriteEndObject();
        }

        public static void Serialize(JsonWriter writer, IList<TaskLogAttachment> attachments)
        {
            writer.WriteStartArray();

            foreach (var attachment in attachments)
            {
                Serialize(writer, attachment);
            }

            writer.WriteEndArray();
        }
    }
}