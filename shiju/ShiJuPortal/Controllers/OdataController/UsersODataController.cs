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
    builder.EntitySet<User>("Users");
    config.Routes.MapODataRoute("odata", "odata", builder.GetEdmModel());
    */
    public class UsersODataController : ODataController
    {
        private ShiJuDbContext db = Heart.CreateShiJuDbContext();
        private int hour = TimeZoneInfo.Local.BaseUtcOffset.Hours;
        // GET: odata/Users
        [Queryable]
        public IList<User> Get(int userStaus,int userGender)
        {
            var users = db.Users.AsNoTracking().ToList();
            if (userStaus != -1) 
            {
                users = users.Where(u => u.Status == (UserStatus)userStaus).ToList();
            }

            if (userGender != -1)
            {
                users = users.Where(u => u.Gender == (UserGender)userGender).ToList();
            }
           
            return users;
        }

        // GET: odata/Users(5)
        [Queryable]
        public SingleResult<User> GetUser(Guid key)
        {
            return SingleResult.Create(db.Users.Where(user => user.Id == key));
        }

        // PUT: odata/Users(5)
        public IHttpActionResult Put(Guid key, User patch)
        {
            patch.PhoneNumber=patch.PhoneNumber ?? "";
            patch.District = patch.District ?? "";
            patch.Signature = patch.Signature ?? "";
            var user = db.Users.FirstOrDefault(u => u.Id == key);
            patch.NeedNotification = user.NeedNotification;
            patch.CreatedTime = patch.CreatedTime.AddHours(-hour);
            db.Entry(patch).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(patch);
        }

        // POST: odata/Users
        public IHttpActionResult Post(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Users.Add(user);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (UserExists(user.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(user);
        }

        // PATCH: odata/Users(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch(Guid key)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User user = db.Users.Find(key);
            if (user == null)
            {
                return NotFound();
            }
            user.Status = UserStatus.Active;
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(user);
        }

        // DELETE: odata/Users(5)
        public IHttpActionResult Delete(Guid key)
        {
            User user = db.Users.Find(key);
            if (user == null)
            {
                return NotFound();
            }
            user.Status = UserStatus.Disabled;
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

        private bool UserExists(Guid key)
        {
            return db.Users.Count(e => e.Id == key) > 0;
        }
    }
}
