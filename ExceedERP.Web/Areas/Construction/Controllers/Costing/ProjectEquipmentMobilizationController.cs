using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Costing
{
    public class ProjectEquipmentMobilizationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectEquipmentMobilization_Read([DataSourceRequest]DataSourceRequest request, int projmoId)
        {
            IQueryable<ProjectEquipmentMobilization> projectequipmentmobilization = db.ProjectEquipmentMobilization.Where(x => x.ProjectEquipmentUtilizationId == projmoId);
            DataSourceResult result = projectequipmentmobilization.ToDataSourceResult(request, projectEquipmentMobilization => new {
                ProjectEquipmentMobilizationId = projectEquipmentMobilization.ProjectEquipmentMobilizationId,
                ProjectEquipmentUtilizationId = projectEquipmentMobilization.ProjectEquipmentUtilizationId,
                //EquipmentType = projectEquipmentMobilization.EquipmentType,
                From = projectEquipmentMobilization.From,
                To = projectEquipmentMobilization.To,
                Date = projectEquipmentMobilization.Date,
                Time = projectEquipmentMobilization.Time,
                OrgBranchName = projectEquipmentMobilization.OrgBranchName,
                DateCreated = projectEquipmentMobilization.DateCreated,
                LastModified = projectEquipmentMobilization.LastModified,
                CreatedBy = projectEquipmentMobilization.CreatedBy,
                ModifiedBy = projectEquipmentMobilization.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEquipmentMobilization_Create([DataSourceRequest]DataSourceRequest request, ProjectEquipmentMobilization projectEquipmentMobilization, int projmobId)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEquipmentMobilization
                {
                    ProjectEquipmentUtilizationId = projmobId,
                    //EquipmentType = projectEquipmentMobilization.EquipmentType,
                    From = projectEquipmentMobilization.From,
                    To = projectEquipmentMobilization.To,
                    Date = projectEquipmentMobilization.Date,
                    Time = projectEquipmentMobilization.Time,
                    OrgBranchName = projectEquipmentMobilization.OrgBranchName,
                    DateCreated = projectEquipmentMobilization.DateCreated,
                    LastModified = projectEquipmentMobilization.LastModified,
                    CreatedBy = projectEquipmentMobilization.CreatedBy,
                    ModifiedBy = projectEquipmentMobilization.ModifiedBy
                };

                db.ProjectEquipmentMobilization.Add(entity);
                db.SaveChanges();
                projectEquipmentMobilization.ProjectEquipmentMobilizationId = entity.ProjectEquipmentMobilizationId;
            }

            return Json(new[] { projectEquipmentMobilization }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEquipmentMobilization_Update([DataSourceRequest]DataSourceRequest request, ProjectEquipmentMobilization projectEquipmentMobilization)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEquipmentMobilization
                {
                    ProjectEquipmentMobilizationId = projectEquipmentMobilization.ProjectEquipmentMobilizationId,
                    ProjectEquipmentUtilizationId = projectEquipmentMobilization.ProjectEquipmentUtilizationId,
                   // EquipmentType = projectEquipmentMobilization.EquipmentType,
                    From = projectEquipmentMobilization.From,
                    To = projectEquipmentMobilization.To,
                    Date = projectEquipmentMobilization.Date,
                    Time = projectEquipmentMobilization.Time,
                    OrgBranchName = projectEquipmentMobilization.OrgBranchName,
                    DateCreated = projectEquipmentMobilization.DateCreated,
                    LastModified = projectEquipmentMobilization.LastModified,
                    CreatedBy = projectEquipmentMobilization.CreatedBy,
                    ModifiedBy = projectEquipmentMobilization.ModifiedBy
                };

                db.ProjectEquipmentMobilization.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectEquipmentMobilization }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEquipmentMobilization_Destroy([DataSourceRequest]DataSourceRequest request, ProjectEquipmentMobilization projectEquipmentMobilization)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEquipmentMobilization
                {
                    ProjectEquipmentMobilizationId = projectEquipmentMobilization.ProjectEquipmentMobilizationId,
                    ProjectEquipmentUtilizationId = projectEquipmentMobilization.ProjectEquipmentUtilizationId,
                    //EquipmentType = projectEquipmentMobilization.EquipmentType,
                    From = projectEquipmentMobilization.From,
                    To = projectEquipmentMobilization.To,
                    Date = projectEquipmentMobilization.Date,
                    Time = projectEquipmentMobilization.Time,
                    OrgBranchName = projectEquipmentMobilization.OrgBranchName,
                    DateCreated = projectEquipmentMobilization.DateCreated,
                    LastModified = projectEquipmentMobilization.LastModified,
                    CreatedBy = projectEquipmentMobilization.CreatedBy,
                    ModifiedBy = projectEquipmentMobilization.ModifiedBy
                };

                db.ProjectEquipmentMobilization.Attach(entity);
                db.ProjectEquipmentMobilization.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectEquipmentMobilization }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
