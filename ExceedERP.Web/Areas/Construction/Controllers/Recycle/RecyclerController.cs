using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Recycle
{
    public class RecyclerController : Controller
    {
        // GET: Construction/Recycler
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Projects()
        {
            return View();
        }
        public ActionResult SubProjectTypes()
        {
            return View();
        }

        public void RestoreProject(int id = 0) {
            var db = new ExceedDbContext();

            var project = db.ProjectInformation.Find(id);
            if(project != null)
            {
                project.IsDeleted = false;
                project.DeletedBy = null;
                project.DeletedOn = null;

                db.ProjectInformation.Attach(project);
                db.Entry(project).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }
        public void RestoreSubProjectType(int id = 0) {
            var db = new ExceedDbContext();

            var project = db.SubProjectType.Find(id);
            if(project != null)
            {
                project.IsDeleted = false;
                project.DeletedBy = null;
                project.DeletedOn = null;

                db.SubProjectType.Attach(project);
                db.Entry(project).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}