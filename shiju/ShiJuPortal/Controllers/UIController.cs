using Portal.Common.Models;
using Shiju.Portal;
using ShiJu.Models;
using ShiJu.Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShiJu.Portal.Controllers
{
    [CheckLogin]
    [ImageHubAttribute]
    public class UIController : Controller
    {
        // GET: UI
        public ActionResult Parties()
        {
            return View();
        }

        // GET: UI
        public ActionResult Users()
        {
            return View();
        }

        // GET: UI
        public ActionResult AboutUs()
        {
            return View();
        }


        [Route("GetParticipantCount")]
        public ActionResult GetParticipantCountByPartyId(Guid partyId)
        {
            using(var db=Heart.CreateShiJuDbContext())
            {
                var count = db.Participants.Where(p => p.PartyId == partyId && p.Status == ParticipantStatus.Accepted).Count();
                return this.JsonResult(0,count.ToString());
            }
        }
    }
}