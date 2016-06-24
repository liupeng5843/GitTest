using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using MHD.Original.Award.Models;
using Newtonsoft.Json;

namespace MHD.Original.Award
{
    public class OriginalIdentity : IIdentity
    {
        public UserDataModel UserData { get; set; }

        private FormsAuthenticationTicket ticket;

        public string AuthenticationType { get; set; }

        public bool IsAuthenticated { get; set; }

        public string Name { get; set; }

        public FormsAuthenticationTicket Ticket
        {
            get { return ticket; }
            set { ticket = value; }
        }
        public OriginalIdentity(string userData)
        {
            UserData = JsonConvert.DeserializeObject<UserDataModel>(userData);
        }
    }
}