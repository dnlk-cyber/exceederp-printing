using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Settings
{
    public class ProjectTypeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var types = db.Configurations.Where(x => x.Reference == "STRUCTURE_TYPE").ToList();
            ViewData["Types"] = types;
            return View();
        }

        //public ActionResult ProjectTypes_Read([DataSourceRequest]DataSourceRequest request)
        //{
        //    IQueryable<ProjectType> ProjectTypes = db.ProjectTypes.Where(x => x.ProjectTypes == Core.Domain.Construction.ProjectTypes.MAIN_PROJECT);
        //    DataSourceResult result = ProjectTypes.ToDataSourceResult(request, ProjectType => new
        //    {
        //        ProjectTypeId = ProjectType.ProjectTypeId,
        //        Name = ProjectType.Name,
        //        ProjectTypes = ProjectType.ProjectTypes,
        //        Remark = ProjectType.Remark
        //    });

        //    return Json(result);
        //}

        //private bool CheckExistance(string description)
        //{
        //    var ProjectType = db.ProjectTypes.Where(x => x.Name == description).FirstOrDefault();
        //    if (ProjectType != null)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult ProjectTypes_Create([DataSourceRequest]DataSourceRequest request, ProjectType ProjectType)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (CheckExistance(ProjectType.Name))
        //        {
        //            ModelState.AddModelError("", "This project type is already registered!!");
        //        }
        //        else
        //        {
        //            var entity = new ProjectType
        //            {
        //                ProjectTypeId = ProjectType.ProjectTypeId,
        //                Name = ProjectType.Name,
        //                ProjectTypes = ProjectTypes.MAIN_PROJECT,
        //                Remark = ProjectType.Remark
        //            };
        //            db.ProjectTypes.Add(entity);
        //            db.SaveChanges();
        //            ProjectType.ProjectTypeId = entity.ProjectTypeId;
        //        }
        //    }

        //    return Json(new[] { ProjectType }.ToDataSourceResult(request, ModelState));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult ProjectTypes_Update([DataSourceRequest]DataSourceRequest request, ProjectType ProjectType)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new ProjectType
        //        {
        //            ProjectTypeId = ProjectType.ProjectTypeId,
        //            Name = ProjectType.Name,
        //            ProjectTypes = ProjectTypes.MAIN_PROJECT,
        //            Remark = ProjectType.Remark
        //        };
        //        db.ProjectTypes.Attach(entity);
        //        db.Entry(entity).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //    return Json(new[] { ProjectType }.ToDataSourceResult(request, ModelState));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult ProjectTypes_Destroy([DataSourceRequest]DataSourceRequest request, ProjectType ProjectType)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new ProjectType
        //        {
        //            ProjectTypeId = ProjectType.ProjectTypeId,
        //            Name = ProjectType.Name,
        //            ProjectTypes = ProjectType.ProjectTypes,
        //            Remark = ProjectType.Remark
        //        };

        //        db.ProjectTypes.Attach(entity);
        //        db.ProjectTypes.Remove(entity);
        //        db.SaveChanges();
        //    }

        //    return Json(new[] { ProjectType }.ToDataSourceResult(request, ModelState));
        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
