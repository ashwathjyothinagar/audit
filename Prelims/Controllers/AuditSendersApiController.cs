using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Prelims;

namespace Prelims.Controllers
{
    public class AuditSendersApiController : ApiController
    {
        private FntEntities db = new FntEntities();

        // GET: api/AuditSendersApi
        public dynamic GetAuditSenders()
        {
            return db.AuditSenders.Select(x => new { x.Id, x.Name, x.EmailAddress, x.CrnId }).ToList();
        }

        // GET: api/AuditSendersApi/5
        [ResponseType(typeof(AuditSender))]
        public IHttpActionResult GetAuditSender(int id)
        {
            AuditSender AuditSender = db.AuditSenders.Find(id);
            if (AuditSender == null)
            {
                return NotFound();
            }

            return Ok(AuditSender);
        }

        // PUT: api/AuditSendersApi/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAuditSender(int id, AuditSender AuditSender)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != AuditSender.Id)
            {
                return BadRequest();
            }

            db.Entry(AuditSender).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuditSenderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/AuditSendersApi
        [ResponseType(typeof(AuditSender))]
        public IHttpActionResult PostAuditSender(AuditSender AuditSender)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AuditSenders.Add(AuditSender);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = AuditSender.Id }, AuditSender);
        }

        // DELETE: api/AuditSendersApi/5
        [ResponseType(typeof(AuditSender))]
        public IHttpActionResult DeleteAuditSender(int id)
        {
            AuditSender AuditSender = db.AuditSenders.Find(id);
            if (AuditSender == null)
            {
                return NotFound();
            }

            db.AuditSenders.Remove(AuditSender);
            db.SaveChanges();

            return Ok(AuditSender);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AuditSenderExists(int id)
        {
            return db.AuditSenders.Count(e => e.Id == id) > 0;
        }
    }
}