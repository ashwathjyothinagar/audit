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
    public class CRNsController : ApiController
    {
        private FntEntities db = new FntEntities();

        // GET: api/CRNs
        public dynamic GetCRNs()
        {
            return db.CRNs.Where(x => x.IsVisibleInMainApplication == true).Select(x=> new { x.CRNID, x.CRNNAME }).ToList();
        }

        // GET: api/CRNs/5
        [ResponseType(typeof(CRN))]
        public IHttpActionResult GetCRN(int id)
        {
            CRN cRN = db.CRNs.Find(id);
            if (cRN == null)
            {
                return NotFound();
            }

            return Ok(cRN);
        }

        // PUT: api/CRNs/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCRN(int id, CRN cRN)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cRN.CRNID)
            {
                return BadRequest();
            }

            db.Entry(cRN).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CRNExists(id))
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

        // POST: api/CRNs
        [ResponseType(typeof(CRN))]
        public IHttpActionResult PostCRN(CRN cRN)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CRNs.Add(cRN);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (CRNExists(cRN.CRNID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = cRN.CRNID }, cRN);
        }

        // DELETE: api/CRNs/5
        [ResponseType(typeof(CRN))]
        public IHttpActionResult DeleteCRN(int id)
        {
            CRN cRN = db.CRNs.Find(id);
            if (cRN == null)
            {
                return NotFound();
            }

            db.CRNs.Remove(cRN);
            db.SaveChanges();

            return Ok(cRN);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CRNExists(int id)
        {
            return db.CRNs.Count(e => e.CRNID == id) > 0;
        }
    }
}