using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Costing
{
    public class ProjectEquipmentPerformanceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EquipmentPerformance_Read([DataSourceRequest]DataSourceRequest request, int projPerId)
        {
            IQueryable<ProjectEquipmentPerformance> equipmentperformance = db.EquipmentPerformance.Where(x => x.ProjectEquipmentUtilizationId == projPerId);
            DataSourceResult result = equipmentperformance.ToDataSourceResult(request, projectEquipmentPerformance => new {
                ProjectEquipmentPerformanceId = projectEquipmentPerformance.ProjectEquipmentPerformanceId,
                ProjectEquipmentUtilizationId = projectEquipmentPerformance.ProjectEquipmentUtilizationId,
                Date = projectEquipmentPerformance.Date,
                //EquipmentType = projectEquipmentPerformance.EquipmentType,
                PreviousPerformance = projectEquipmentPerformance.PreviousPerformance,
                CurrentPeriodPerformance = projectEquipmentPerformance.CurrentPeriodPerformance,

                ToDatePerformance = projectEquipmentPerformance.ToDatePerformance,
                OrgBranchName = projectEquipmentPerformance.OrgBranchName,
                DateCreated = projectEquipmentPerformance.DateCreated,
                LastModified = projectEquipmentPerformance.LastModified,
                CreatedBy = projectEquipmentPerformance.CreatedBy,
                ModifiedBy = projectEquipmentPerformance.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EquipmentPerformance_Create([DataSourceRequest]DataSourceRequest request, ProjectEquipmentPerformance projectEquipmentPerformance, int perId)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEquipmentPerformance
                {
                    ProjectEquipmentUtilizationId = perId,
                    Date = projectEquipmentPerformance.Date,
                    //EquipmentType = projectEquipmentPerformance.EquipmentType,
                    PreviousPerformance = projectEquipmentPerformance.PreviousPerformance,
                    CurrentPeriodPerformance = projectEquipmentPerformance.CurrentPeriodPerformance,
                    ToDatePerformance = projectEquipmentPerformance.ToDatePerformance,
                    OrgBranchName = projectEquipmentPerformance.OrgBranchName,
                    DateCreated = projectEquipmentPerformance.DateCreated,
                    LastModified = projectEquipmentPerformance.LastModified,
                    CreatedBy = projectEquipmentPerformance.CreatedBy,
                    ModifiedBy = projectEquipmentPerformance.ModifiedBy
                };

                db.EquipmentPerformance.Add(entity);
                db.SaveChanges();
                projectEquipmentPerformance.ProjectEquipmentPerformanceId = entity.ProjectEquipmentPerformanceId;
            }

            return Json(new[] { projectEquipmentPerformance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EquipmentPerformance_Update([DataSourceRequest]DataSourceRequest request, ProjectEquipmentPerformance projectEquipmentPerformance)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEquipmentPerformance
                {
                    ProjectEquipmentPerformanceId = projectEquipmentPerformance.ProjectEquipmentPerformanceId,
                    ProjectEquipmentUtilizationId = projectEquipmentPerformance.ProjectEquipmentUtilizationId,
                    Date = projectEquipmentPerformance.Date,
                    //EquipmentType = projectEquipmentPerformance.EquipmentType,
                    PreviousPerformance = projectEquipmentPerformance.PreviousPerformance,
                    CurrentPeriodPerformance = projectEquipmentPerformance.CurrentPeriodPerformance,
                    ToDatePerformance = projectEquipmentPerformance.ToDatePerformance,
                    OrgBranchName = projectEquipmentPerformance.OrgBranchName,
                    DateCreated = projectEquipmentPerformance.DateCreated,
                    LastModified = projectEquipmentPerformance.LastModified,
                    CreatedBy = projectEquipmentPerformance.CreatedBy,
                    ModifiedBy = projectEquipmentPerformance.ModifiedBy
                };

                db.EquipmentPerformance.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectEquipmentPerformance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EquipmentPerformance_Destroy([DataSourceRequest]DataSourceRequest request, ProjectEquipmentPerformance projectEquipmentPerformance)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEquipmentPerformance
                {
                    ProjectEquipmentPerformanceId = projectEquipmentPerformance.ProjectEquipmentPerformanceId,
                    ProjectEquipmentUtilizationId = projectEquipmentPerformance.ProjectEquipmentUtilizationId,
                    Date = projectEquipmentPerformance.Date,
                    //EquipmentType = projectEquipmentPerformance.EquipmentType,
                    PreviousPerformance = projectEquipmentPerformance.PreviousPerformance,
                    CurrentPeriodPerformance = projectEquipmentPerformance.CurrentPeriodPerformance,
                    ToDatePerformance = projectEquipmentPerformance.ToDatePerformance,
                    OrgBranchName = projectEquipmentPerformance.OrgBranchName,
                    DateCreated = projectEquipmentPerformance.DateCreated,
                    LastModified = projectEquipmentPerformance.LastModified,
                    CreatedBy = projectEquipmentPerformance.CreatedBy,
                    ModifiedBy = projectEquipmentPerformance.ModifiedBy
                };

                db.EquipmentPerformance.Attach(entity);
                db.EquipmentPerformance.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectEquipmentPerformance }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
