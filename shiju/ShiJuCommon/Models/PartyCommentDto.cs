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
    public class PartyCommentDto
    {
        public Guid Id { get; set; }
        public Guid PartyId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid TargetUserId { get; set; }
        public string Text { get; set; }
        public string AudioJson { get; set; }
        public string VoteResult { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
