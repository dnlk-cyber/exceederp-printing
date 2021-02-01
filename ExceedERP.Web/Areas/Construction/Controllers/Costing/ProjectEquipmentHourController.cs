using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Costing
{
    public class ProjectEquipmentHourController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EquipmentHour_Read([DataSourceRequest]DataSourceRequest request, int projewId)
        {
            IQueryable<ProjectEquipmentHour> equipmenthour = db.EquipmentHour.Where(x => x.ProjectEquipmentUtilizationId == projewId);
            DataSourceResult result = equipmenthour.ToDataSourceResult(request, projectEquipmentHour => new {
                ProjectEquipmentHourId = projectEquipmentHour.ProjectEquipmentHourId,
                ProjectEquipmentUtilizationId = projectEquipmentHour.ProjectEquipmentUtilizationId,
                PlannedHours = projectEquipmentHour.PlannedHours,
                AvailableHours = projectEquipmentHour.AvailableHours,
                UtilizedHours = projectEquipmentHour.UtilizedHours,
                AvailableEfficiency = projectEquipmentHour.AvailableEfficiency,
                UtilizedEfficiency = projectEquipmentHour.UtilizedEfficiency,
                OverallEfficiency = projectEquipmentHour.OverallEfficiency,
                Remark = projectEquipmentHour.Remark,
                OrgBranchName = projectEquipmentHour.OrgBranchName,
                DateCreated = projectEquipmentHour.DateCreated,
                LastModified = projectEquipmentHour.LastModified,
                CreatedBy = projectEquipmentHour.CreatedBy,
                ModifiedBy = projectEquipmentHour.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EquipmentHour_Create([DataSourceRequest]DataSourceRequest request, ProjectEquipmentHour projectEquipmentHour, int projeqId)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEquipmentHour
                {
                    ProjectEquipmentUtilizationId = projeqId,
                    PlannedHours = projectEquipmentHour.PlannedHours,
                    AvailableHours = projectEquipmentHour.AvailableHours,
                    UtilizedHours = projectEquipmentHour.UtilizedHours,
                    AvailableEfficiency = projectEquipmentHour.AvailableEfficiency,
                    UtilizedEfficiency = projectEquipmentHour.UtilizedEfficiency,
                    OverallEfficiency = projectEquipmentHour.OverallEfficiency,
                    Remark = projectEquipmentHour.Remark,
                    OrgBranchName = projectEquipmentHour.OrgBranchName,
                    DateCreated = projectEquipmentHour.DateCreated,
                    LastModified = projectEquipmentHour.LastModified,
                    CreatedBy = projectEquipmentHour.CreatedBy,
                    ModifiedBy = projectEquipmentHour.ModifiedBy
                };

                db.EquipmentHour.Add(entity);
                db.SaveChanges();
                projectEquipmentHour.ProjectEquipmentHourId = entity.ProjectEquipmentHourId;
            }

            return Json(new[] { projectEquipmentHour }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EquipmentHour_Update([DataSourceRequest]DataSourceRequest request, ProjectEquipmentHour projectEquipmentHour)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEquipmentHour
                {
                    ProjectEquipmentHourId = projectEquipmentHour.ProjectEquipmentHourId,
                    ProjectEquipmentUtilizationId = projectEquipmentHour.ProjectEquipmentUtilizationId,
                    PlannedHours = projectEquipmentHour.PlannedHours,
                    AvailableHours = projectEquipmentHour.AvailableHours,
                    UtilizedHours = projectEquipmentHour.UtilizedHours,
                    AvailableEfficiency = projectEquipmentHour.AvailableEfficiency,
                    UtilizedEfficiency = projectEquipmentHour.UtilizedEfficiency,
                    OverallEfficiency = projectEquipmentHour.OverallEfficiency,
                    Remark = projectEquipmentHour.Remark,
                    OrgBranchName = projectEquipmentHour.OrgBranchName,
                    DateCreated = projectEquipmentHour.DateCreated,
                    LastModified = projectEquipmentHour.LastModified,
                    CreatedBy = projectEquipmentHour.CreatedBy,
                    ModifiedBy = projectEquipmentHour.ModifiedBy
                };

                db.EquipmentHour.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectEquipmentHour }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EquipmentHour_Destroy([DataSourceRequest]DataSourceRequest request, ProjectEquipmentHour projectEquipmentHour)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEquipmentHour
                {
                    ProjectEquipmentHourId = projectEquipmentHour.ProjectEquipmentHourId,
                    ProjectEquipmentUtilizationId = projectEquipmentHour.ProjectEquipmentUtilizationId,
                    PlannedHours = projectEquipmentHour.PlannedHours,
                    AvailableHours = projectEquipmentHour.AvailableHours,
                    UtilizedHours = projectEquipmentHour.UtilizedHours,
                    AvailableEfficiency = projectEquipmentHour.AvailableEfficiency,
                    UtilizedEfficiency = projectEquipmentHour.UtilizedEfficiency,
                    OverallEfficiency = projectEquipmentHour.OverallEfficiency,
                    Remark = projectEquipmentHour.Remark,
                    OrgBranchName = projectEquipmentHour.OrgBranchName,
                    DateCreated = projectEquipmentHour.DateCreated,
                    LastModified = projectEquipmentHour.LastModified,
                    CreatedBy = projectEquipmentHour.CreatedBy,
                    ModifiedBy = projectEquipmentHour.ModifiedBy
                };

                db.EquipmentHour.Attach(entity);
                db.EquipmentHour.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectEquipmentHour }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
