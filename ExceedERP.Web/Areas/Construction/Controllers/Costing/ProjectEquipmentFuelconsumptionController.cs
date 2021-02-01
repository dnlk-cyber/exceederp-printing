using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Costing
{
    public class ProjectEquipmentFuelconsumptionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EquipmentFuelconsumption_Read([DataSourceRequest]DataSourceRequest request, int projfuId)
        {
            IQueryable<ProjectEquipmentFuelconsumption> equipmentfuelconsumption = db.EquipmentFuelconsumption.Where(x => x.ProjectEquipmentUtilizationId == projfuId);
            DataSourceResult result = equipmentfuelconsumption.ToDataSourceResult(request, projectEquipmentFuelconsumption => new {
                ProjectEquipmentFuelconsumptionId = projectEquipmentFuelconsumption.ProjectEquipmentFuelconsumptionId,
                ProjectEquipmentUtilizationId = projectEquipmentFuelconsumption.ProjectEquipmentUtilizationId,
                PreviousDelivered = projectEquipmentFuelconsumption.PreviousDelivered,
                ToDateDelivered = projectEquipmentFuelconsumption.ToDateDelivered,
                CurrentPeriodDelivered = projectEquipmentFuelconsumption.CurrentPeriodDelivered,
                TotalPreviousConsumed = projectEquipmentFuelconsumption.TotalPreviousConsumed,
                TotalCurrentConsumed = projectEquipmentFuelconsumption.TotalCurrentConsumed,
                TotalToDateConsumed = projectEquipmentFuelconsumption.TotalToDateConsumed,
                ToDateBalance = projectEquipmentFuelconsumption.ToDateBalance,
                OrgBranchName = projectEquipmentFuelconsumption.OrgBranchName,
                DateCreated = projectEquipmentFuelconsumption.DateCreated,
                LastModified = projectEquipmentFuelconsumption.LastModified,
                CreatedBy = projectEquipmentFuelconsumption.CreatedBy,
                ModifiedBy = projectEquipmentFuelconsumption.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EquipmentFuelconsumption_Create([DataSourceRequest]DataSourceRequest request, ProjectEquipmentFuelconsumption projectEquipmentFuelconsumption, int profueId)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEquipmentFuelconsumption
                {
                    ProjectEquipmentUtilizationId = profueId,
                    PreviousDelivered = projectEquipmentFuelconsumption.PreviousDelivered,
                    ToDateDelivered = projectEquipmentFuelconsumption.ToDateDelivered,
                    CurrentPeriodDelivered = projectEquipmentFuelconsumption.CurrentPeriodDelivered,
                    TotalPreviousConsumed = projectEquipmentFuelconsumption.TotalPreviousConsumed,
                    TotalCurrentConsumed = projectEquipmentFuelconsumption.TotalCurrentConsumed,
                    TotalToDateConsumed = projectEquipmentFuelconsumption.TotalToDateConsumed,
                    ToDateBalance = projectEquipmentFuelconsumption.ToDateBalance,
                    OrgBranchName = projectEquipmentFuelconsumption.OrgBranchName,
                    DateCreated = projectEquipmentFuelconsumption.DateCreated,
                    LastModified = projectEquipmentFuelconsumption.LastModified,
                    CreatedBy = projectEquipmentFuelconsumption.CreatedBy,
                    ModifiedBy = projectEquipmentFuelconsumption.ModifiedBy
                };

                db.EquipmentFuelconsumption.Add(entity);
                db.SaveChanges();
                projectEquipmentFuelconsumption.ProjectEquipmentFuelconsumptionId = entity.ProjectEquipmentFuelconsumptionId;
            }

            return Json(new[] { projectEquipmentFuelconsumption }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EquipmentFuelconsumption_Update([DataSourceRequest]DataSourceRequest request, ProjectEquipmentFuelconsumption projectEquipmentFuelconsumption)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEquipmentFuelconsumption
                {
                    ProjectEquipmentFuelconsumptionId = projectEquipmentFuelconsumption.ProjectEquipmentFuelconsumptionId,
                    ProjectEquipmentUtilizationId = projectEquipmentFuelconsumption.ProjectEquipmentUtilizationId,
                    PreviousDelivered = projectEquipmentFuelconsumption.PreviousDelivered,
                    ToDateDelivered = projectEquipmentFuelconsumption.ToDateDelivered,
                    CurrentPeriodDelivered = projectEquipmentFuelconsumption.CurrentPeriodDelivered,
                    TotalPreviousConsumed = projectEquipmentFuelconsumption.TotalPreviousConsumed,
                    TotalCurrentConsumed = projectEquipmentFuelconsumption.TotalCurrentConsumed,
                    TotalToDateConsumed = projectEquipmentFuelconsumption.TotalToDateConsumed,
                    ToDateBalance = projectEquipmentFuelconsumption.ToDateBalance,
                    OrgBranchName = projectEquipmentFuelconsumption.OrgBranchName,
                    DateCreated = projectEquipmentFuelconsumption.DateCreated,
                    LastModified = projectEquipmentFuelconsumption.LastModified,
                    CreatedBy = projectEquipmentFuelconsumption.CreatedBy,
                    ModifiedBy = projectEquipmentFuelconsumption.ModifiedBy
                };

                db.EquipmentFuelconsumption.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectEquipmentFuelconsumption }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EquipmentFuelconsumption_Destroy([DataSourceRequest]DataSourceRequest request, ProjectEquipmentFuelconsumption projectEquipmentFuelconsumption)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEquipmentFuelconsumption
                {
                    ProjectEquipmentFuelconsumptionId = projectEquipmentFuelconsumption.ProjectEquipmentFuelconsumptionId,
                    ProjectEquipmentUtilizationId = projectEquipmentFuelconsumption.ProjectEquipmentUtilizationId,
                    PreviousDelivered = projectEquipmentFuelconsumption.PreviousDelivered,
                    ToDateDelivered = projectEquipmentFuelconsumption.ToDateDelivered,
                    CurrentPeriodDelivered = projectEquipmentFuelconsumption.CurrentPeriodDelivered,
                    TotalPreviousConsumed = projectEquipmentFuelconsumption.TotalPreviousConsumed,
                    TotalCurrentConsumed = projectEquipmentFuelconsumption.TotalCurrentConsumed,
                    TotalToDateConsumed = projectEquipmentFuelconsumption.TotalToDateConsumed,
                    ToDateBalance = projectEquipmentFuelconsumption.ToDateBalance,
                    OrgBranchName = projectEquipmentFuelconsumption.OrgBranchName,
                    DateCreated = projectEquipmentFuelconsumption.DateCreated,
                    LastModified = projectEquipmentFuelconsumption.LastModified,
                    CreatedBy = projectEquipmentFuelconsumption.CreatedBy,
                    ModifiedBy = projectEquipmentFuelconsumption.ModifiedBy
                };

                db.EquipmentFuelconsumption.Attach(entity);
                db.EquipmentFuelconsumption.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectEquipmentFuelconsumption }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
