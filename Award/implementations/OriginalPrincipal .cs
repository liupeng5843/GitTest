using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace MHD.Original.Award
{
    public class OriginalPrincipal : IPrincipal
    {
        private IIdentity identity { get; set; }
        public IIdentity Identity
        {
            get { return identity; }
            set { identity = value; }
        }

        public bool IsInRole(string role)
        {
            throw new NotImplementedException();
        }
    }
}