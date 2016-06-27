using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACommon.Models
{
    [Table("AccessTokens")]
    public class AuthenticationAccessToken
    {
        [Key]
        [Required(AllowEmptyStrings = false)]
        [StringLength(100)]
        public string AccessToken
        {
            get;
            set;
        }

        public Guid UserId
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public string Content
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(100)]
        public string IPAddress
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(1000)]
        public string UserAgent
        {
            get;
            set;
        }

        public DateTime CreatedDate
        {
            get;
            set;
        }
    }
}
