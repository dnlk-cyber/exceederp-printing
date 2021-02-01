using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Production;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.CostTracking
{
    public class ProjectCostByMonthDetailController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();

        public ActionResult CostDetails_Read([DataSourceRequest]DataSourceRequest request, int costId)
        {
            IQueryable<ProjectCostByMonthDetail> details = db.ProjectCostByMonthDetails.Where(a => a.ProjectCostByMonthId == costId);
            DataSourceResult result = details.ToDataSourceResult(request, detail => new
            {
                ProjectCostByMonthDetailId = detail.ProjectCostByMonthDetailId,
                ProjectCostByMonthId = detail.ProjectCostByMonthId,
                ActivityName = detail.ActivityName,
                OperationType = detail.OperationType,

                LaborCost = detail.LaborCost,
                EquipmentCost = detail.EquipmentCost,
                TotalCost = detail.TotalCost,

                OrgBranchName = detail.OrgBranchName,
                DateCreated = detail.DateCreated,
                LastModified = detail.LastModified,
                CreatedBy = detail.CreatedBy,
                ModifiedBy = detail.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CostDetail_Create([DataSourceRequest]DataSourceRequest request, ProjectCostByMonthDetail detail, int costId)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectCostByMonthDetail
                {
                    ProjectCostByMonthDetailId = detail.ProjectCostByMonthDetailId,
                    ProjectCostByMonthId = costId,
                    ActivityName = detail.ActivityName,
                    OperationType = detail.OperationType,

                    LaborCost = detail.LaborCost,
                    EquipmentCost = detail.EquipmentCost,
                    TotalCost = detail.TotalCost,

                    OrgBranchName = detail.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = detail.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = detail.ModifiedBy,

                };
                entity.TotalCost = entity.LaborCost + entity.EquipmentCost;

                db.ProjectCostByMonthDetails.Add(entity);
                db.SaveChanges();

                detail.ProjectCostByMonthId = entity.ProjectCostByMonthId;

                CostTrackingHelper.UpdateCostTrackingRecord(entity.ProjectCostByMonthId);
            }

            return Json(new[] { detail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CostDetail_Update([DataSourceRequest]DataSourceRequest request, ProjectCostByMonthDetail detail)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectCostByMonthDetail
                {
                    ProjectCostByMonthDetailId = detail.ProjectCostByMonthDetailId,
                    ProjectCostByMonthId = detail.ProjectCostByMonthId,
                    ActivityName = detail.ActivityName,
                    OperationType = detail.OperationType,

                    LaborCost = detail.LaborCost,
                    EquipmentCost = detail.EquipmentCost,
                    TotalCost = detail.TotalCost,

                    OrgBranchName = detail.OrgBranchName,
                    DateCreated = detail.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = detail.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };
                entity.TotalCost = entity.LaborCost + entity.EquipmentCost;

                db.ProjectCostByMonthDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                CostTrackingHelper.UpdateCostTrackingRecord(entity.ProjectCostByMonthId);
            }

            return Json(new[] { detail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CostDetail_Destroy([DataSourceRequest]DataSourceRequest request, ProjectCostByMonthDetail detail)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectCostByMonthDetail
                {
                    ProjectCostByMonthDetailId = detail.ProjectCostByMonthDetailId,
                    ProjectCostByMonthId = detail.ProjectCostByMonthId,
                    ActivityName = detail.ActivityName,
                    OperationType = detail.OperationType,

                    LaborCost = detail.LaborCost,
                    EquipmentCost = detail.EquipmentCost,
                    TotalCost = detail.TotalCost,

                    OrgBranchName = detail.OrgBranchName,
                    DateCreated = detail.DateCreated,
                    LastModified = detail.LastModified,
                    CreatedBy = detail.CreatedBy,
                    ModifiedBy = detail.ModifiedBy,

                };

                db.ProjectCostByMonthDetails.Attach(entity);
                db.ProjectCostByMonthDetails.Remove(entity);
                db.SaveChanges();

                CostTrackingHelper.UpdateCostTrackingRecord(entity.ProjectCostByMonthId);
            }

            return Json(new[] { detail }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}