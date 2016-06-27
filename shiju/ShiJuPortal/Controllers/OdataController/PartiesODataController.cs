using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using ShiJu.Models;
using Shiju.Portal;

namespace ShiJu.Portal.Controllers.OdataController
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using ShiJu.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Party>("Parties");
    config.Routes.MapODataRoute("odata", "odata", builder.GetEdmModel());
    */
    public class PartiesODataController : ODataController
    {
        private ShiJuDbContext db = Heart.CreateShiJuDbContext();
        private int hour = TimeZoneInfo.Local.BaseUtcOffset.Hours;
        // GET: odata/Parties
        [Queryable]
        public IList<Party> Get(int partyStaus)
        {
            var partyList = db.Parties.AsNoTracking().ToList();
            if (partyStaus == 0)
            {
                partyList = partyList.Where(p => !p.IsDisabled).ToList();
            }
            else if (partyStaus == 1)
            {
                partyList = partyList.Where(p => p.IsDisabled).ToList();
            }
            return partyList;
        }

        // GET: odata/Parties(5)
        [Queryable]
        public SingleResult<Party> GetParty(Guid key)
        {
            var _party = db.Parties.Where(party => party.Id == key);
            foreach (var p in _party)
            {
                p.EndTime = p.EndTime.ToLocalTime();
                p.BeginTime = p.BeginTime.ToLocalTime();
            }

            return SingleResult.Create(db.Parties.Where(party => party.Id == key));
        }

        // PUT: odata/Parties(5)
        public IHttpActionResult Put(Guid key, Party patch)
        {
            var party = db.Parties.Where(p => p.Id == key).FirstOrDefault();
            party.Images = patch.Images ?? "[]";
            party.BeginTime = patch.BeginTime.ToUniversalTime();
            party.EndTime = patch.EndTime.ToUniversalTime();
            party.Title = patch.Title;
            party.Description = patch.Description ?? string.Empty;
            party.Address = patch.Address ?? string.Empty;
            party.IsPublic = patch.IsPublic;
            party.DirectFriendVisible = patch.DirectFriendVisible;
            party.MaxUserCount = patch.MaxUserCount;
            party.LikeCount = patch.LikeCount;
            party.CommentCount = patch.CommentCount;
            party.IsDisabled = patch.IsDisabled;

            //db.Entry(patch).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PartyExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(party);
        }

        // POST: odata/Parties
        public IHttpActionResult Post(Party party)
        {
            party.Id = Guid.NewGuid();
            party.CreatedTime = DateTime.UtcNow;
            party.BeginTime = party.BeginTime.AddHours(-hour);
            party.EndTime = party.EndTime.AddHours(-hour);
            party.Kind = "活动";
            if (party.VoteTitle == null)
            {
                party.VoteTitle = "";
            }
            db.Parties.Add(party);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (PartyExists(party.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(party);
        }

        // PATCH: odata/Parties(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] Guid key, Delta<Party> patch)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Party party = db.Parties.Find(key);
            if (party == null)
            {
                return NotFound();
            }

            patch.Patch(party);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PartyExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(party);
        }

        // DELETE: odata/Parties(5)
        public IHttpActionResult Delete([FromODataUri] Guid key)
        {
            Party party = db.Parties.Find(key);
            if (party == null)
            {
                return NotFound();
            }

            db.Parties.Remove(party);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PartyExists(Guid key)
        {
            return db.Parties.Count(e => e.Id == key) > 0;
        }
    }
}
