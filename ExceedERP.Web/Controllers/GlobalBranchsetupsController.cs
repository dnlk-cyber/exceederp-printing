using System;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using XAPI;
using System.Linq;
using PagedList;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;

namespace ExceedERP.Web.Controllers
{
     [Authorize(Roles = "Admin")]
    public class GlobalBranchsetupsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            if (searchString != null)
            {
                Common.pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var cpos = from s in db.GlobalBranchsetups
                       select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                cpos = cpos.Where(s => s.OrgBranchName.ToString().ToUpper().Contains(searchString.ToUpper())
                                       || s.BranchAccountCode.ToString().ToUpper().Contains(searchString.ToUpper()));
            }

            Common.pageNumber = (page ?? 1);
            return View(cpos.OrderBy(x => x.OrgBranchName).ToPagedList(Common.pageNumber, Common.pageSize));
        }

        // GET: GlobalBranchsetups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GlobalBranchsetup globalBranchsetup = db.GlobalBranchsetups.Find(id);
            if (globalBranchsetup == null)
            {
                return HttpNotFound();
            }
            return View(globalBranchsetup);
        }

        // GET: GlobalBranchsetups/Create
        public ActionResult Create()
        {
            var locationSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.Location);
            var locations = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == locationSeg.GLSegmentSetupId);
            ViewBag.AccountCodes = new SelectList(locations, "Values", "Description");
            return View();
        }

        // POST: GlobalBranchsetups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GlobalBranchsetupID,OrgBranchName,OrgBranchAbbreviation,BranchAccountCode,Description,CreatedOn,CreatedBy")] GlobalBranchsetup globalBranchsetup)
        {
            globalBranchsetup.CreatedOn = DateTime.Now;
            globalBranchsetup.CreatedBy = User.Identity.Name;
            if (ModelState.IsValid)
            {
                db.GlobalBranchsetups.Add(globalBranchsetup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var locationSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.Location);
            var locations = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == locationSeg.GLSegmentSetupId);
            ViewBag.AccountCodes = new SelectList(locations, "Values", "Description", globalBranchsetup.BranchAccountCode);
            return View(globalBranchsetup);
        }

        // GET: GlobalBranchsetups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GlobalBranchsetup globalBranchsetup = db.GlobalBranchsetups.Find(id);
            if (globalBranchsetup == null)
            {
                return HttpNotFound();
            }
            var locationSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.Location);
            var locations = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == locationSeg.GLSegmentSetupId);
            ViewBag.AccountCodes = new SelectList(locations, "Values", "Description", globalBranchsetup.BranchAccountCode);
            return View(globalBranchsetup);
        }

        // POST: GlobalBranchsetups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GlobalBranchsetupID,OrgBranchName,OrgBranchAbbreviation,BranchAccountCode,Description,CreatedOn,CreatedBy")] GlobalBranchsetup globalBranchsetup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(globalBranchsetup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var locationSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.Location);
            var locations = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == locationSeg.GLSegmentSetupId);
            ViewBag.AccountCodes = new SelectList(locations, "Values", "Description", globalBranchsetup.BranchAccountCode);
            return View(globalBranchsetup);
        }

        // GET: GlobalBranchsetups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GlobalBranchsetup globalBranchsetup = db.GlobalBranchsetups.Find(id);
            if (globalBranchsetup == null)
            {
                return HttpNotFound();
            }
            return View(globalBranchsetup);
        }

        // POST: GlobalBranchsetups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GlobalBranchsetup globalBranchsetup = db.GlobalBranchsetups.Find(id);
            db.GlobalBranchsetups.Remove(globalBranchsetup);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
