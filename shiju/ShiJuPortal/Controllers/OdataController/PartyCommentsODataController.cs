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
using System.Web.Http.OData.Query;
using System.Json;

namespace ShiJu.Portal.Controllers.OdataController
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using ShiJu.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<PartyComment>("PartyComments");
    config.Routes.MapODataRoute("odata", "odata", builder.GetEdmModel());
    */
    public class PartyCommentsODataController : ODataController
    {
        private ShiJuDbContext db = Heart.CreateShiJuDbContext();

        // GET: odata/PartyComments
        [Queryable]
        public IList<PartyCommentDto> Get(ODataQueryOptions options, Guid partyId)
        {
            var comments=db.PartyComments.Where(c => c.PartyId == partyId).OrderByDescending(c=>c.CreatedTime).ToList();
            List<PartyCommentDto> partyCommentsDto=new List<PartyCommentDto>();
            var audiotext = @"<a href='{0}/Messages/{1}/Logs/Attachments/{2}?messageId={3}'>下载语音</a>";
           // var audioUrlText = @"{0}/Messages/{1}/Logs/Attachments/{2}";
            var audioText = "<button class='btn btn-sm btn-default audiobtn' onclick=\"Test('"+Heart.AudioText+"')\">Play</button>";
            foreach (var c in comments)
            {
                var partyCommentDto = new PartyCommentDto
                {
                    Id = c.Id,
                    AudioJson = c.AudioJson,
                    CreatedTime = c.CreatedTime,
                    PartyId = c.PartyId,
                    TargetUserId = c.TargetUserId,
                    Text = c.Text,
                    UserId = c.UserId,
                    UserName = db.Users.Find(c.UserId).NickName,
                    VoteResult = c.VoteResult
                };

                //this wrong , wait client design
                var audioUrl = "";
                if (partyCommentDto.AudioJson != "")
                {
                    var audioJson = JsonValue.Parse(partyCommentDto.AudioJson) as JsonObject;
                    var audioId = new Guid(audioJson["AudioId"]);
                   // audioUrl = String.Format(audiotext, Heart.ShijuServiceBaseUrl, partyCommentDto.PartyId, audioId, Guid.Empty);
                    audioUrl = String.Format(audioText,partyCommentDto.PartyId, audioId);
                }
                
                if (partyCommentDto.Text == "")
                {
                    partyCommentDto.Text = audioUrl;
                }
                partyCommentsDto.Add(partyCommentDto);
            }
            var queryComments = ((IQueryable<PartyCommentDto>)(options.ApplyTo(partyCommentsDto.AsQueryable()))).ToList();
            var temp = queryComments.OrderByDescending(c => c.CreatedTime).ToList();
            return temp;
        }

        // GET: odata/PartyComments(5)
        [Queryable]
        public SingleResult<PartyComment> GetPartyComment([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.PartyComments.Where(partyComment => partyComment.Id == key));
        }

        // PUT: odata/PartyComments(5)
        public IHttpActionResult Put([FromODataUri] Guid key, Delta<PartyComment> patch)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            PartyComment partyComment = db.PartyComments.Find(key);
            if (partyComment == null)
            {
                return NotFound();
            }

            patch.Put(partyComment);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PartyCommentExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(partyComment);
        }

        // POST: odata/PartyComments
        public IHttpActionResult Post(PartyComment partyComment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.PartyComments.Add(partyComment);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (PartyCommentExists(partyComment.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(partyComment);
        }

        // PATCH: odata/PartyComments(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] Guid key, Delta<PartyComment> patch)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            PartyComment partyComment = db.PartyComments.Find(key);
            if (partyComment == null)
            {
                return NotFound();
            }

            patch.Patch(partyComment);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PartyCommentExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(partyComment);
        }

        // DELETE: odata/PartyComments(5)
        public IHttpActionResult Delete(Guid key)
        {
            PartyComment partyComment = db.PartyComments.Find(key);
            if (partyComment == null)
            {
                return NotFound();
            }

            db.PartyComments.Remove(partyComment);
            db.Parties.FirstOrDefault(p => p.Id == partyComment.PartyId).CommentCount -= 1;
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

        private bool PartyCommentExists(Guid key)
        {
            return db.PartyComments.Count(e => e.Id == key) > 0;
        }
    }
}
