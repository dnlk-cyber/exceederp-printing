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
    public class ProjectCostByMonthMaterialController : Controller
    {
        ExceedDbContext db;

        public ProjectCostByMonthMaterialController()
        {
            db = new ExceedDbContext();
        }

        public ActionResult Materials_Read([DataSourceRequest]DataSourceRequest request, int ctId)
        {
            IQueryable<ProjectCostByMonthMaterial> costingMaterials = db.ProjectCostByMonthMaterials.Where(a => a.ProjectCostByMonthId == ctId);
            DataSourceResult result = costingMaterials.ToDataSourceResult(request, costingMaterial => new
            {
                ProjectCostByMonthMaterialId = costingMaterial.ProjectCostByMonthMaterialId,
                ProjectCostByMonthId = costingMaterial.ProjectCostByMonthId,
                MaterialName = costingMaterial.MaterialName,
                Unit = costingMaterial.Unit,

                UnitPrice = costingMaterial.UnitPrice,
                Quantity = costingMaterial.Quantity,
                TotalCost = costingMaterial.TotalCost,

                OrgBranchName = costingMaterial.OrgBranchName,
                DateCreated = costingMaterial.DateCreated,
                LastModified = costingMaterial.LastModified,
                CreatedBy = costingMaterial.CreatedBy,
                ModifiedBy = costingMaterial.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Material_Create([DataSourceRequest]DataSourceRequest request, ProjectCostByMonthMaterial costingMaterial, int ctId)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectCostByMonthMaterial
                {
                    ProjectCostByMonthMaterialId = costingMaterial.ProjectCostByMonthMaterialId,
                    ProjectCostByMonthId = ctId,
                    MaterialName = costingMaterial.MaterialName,
                    Unit = costingMaterial.Unit,

                    UnitPrice = costingMaterial.UnitPrice,
                    Quantity = costingMaterial.Quantity,
                    TotalCost = costingMaterial.TotalCost,

                    OrgBranchName = costingMaterial.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = costingMaterial.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = costingMaterial.ModifiedBy,

                };
                entity.TotalCost = entity.UnitPrice != null && entity.Quantity != null ? (decimal)entity.UnitPrice * (decimal)entity.Quantity : 0;

                db.ProjectCostByMonthMaterials.Add(entity);
                db.SaveChanges();

                costingMaterial.ProjectCostByMonthMaterialId = entity.ProjectCostByMonthMaterialId;

                CostTrackingHelper.UpdateCostTrackingRecord(entity.ProjectCostByMonthId);
            }

            return Json(new[] { costingMaterial }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Material_Update([DataSourceRequest]DataSourceRequest request, ProjectCostByMonthMaterial costingMaterial)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectCostByMonthMaterial
                {
                    ProjectCostByMonthMaterialId = costingMaterial.ProjectCostByMonthMaterialId,
                    ProjectCostByMonthId = costingMaterial.ProjectCostByMonthId,
                    MaterialName = costingMaterial.MaterialName,
                    Unit = costingMaterial.Unit,

                    UnitPrice = costingMaterial.UnitPrice,
                    Quantity = costingMaterial.Quantity,
                    TotalCost = costingMaterial.TotalCost,

                    OrgBranchName = costingMaterial.OrgBranchName,
                    DateCreated = costingMaterial.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = costingMaterial.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };
                entity.TotalCost = entity.UnitPrice != null && entity.Quantity != null ? (decimal)entity.UnitPrice * (decimal)entity.Quantity : 0;

                db.ProjectCostByMonthMaterials.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                CostTrackingHelper.UpdateCostTrackingRecord(entity.ProjectCostByMonthId);
            }

            return Json(new[] { costingMaterial }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Material_Destroy([DataSourceRequest]DataSourceRequest request, ProjectCostByMonthMaterial costingMaterial)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectCostByMonthMaterial
                {
                    ProjectCostByMonthMaterialId = costingMaterial.ProjectCostByMonthMaterialId,
                    ProjectCostByMonthId = costingMaterial.ProjectCostByMonthId,
                    MaterialName = costingMaterial.MaterialName,
                    Unit = costingMaterial.Unit,

                    UnitPrice = costingMaterial.UnitPrice,
                    Quantity = costingMaterial.Quantity,
                    TotalCost = costingMaterial.TotalCost,

                    OrgBranchName = costingMaterial.OrgBranchName,
                    DateCreated = costingMaterial.DateCreated,
                    LastModified = costingMaterial.LastModified,
                    CreatedBy = costingMaterial.CreatedBy,
                    ModifiedBy = costingMaterial.ModifiedBy,

                };

                db.ProjectCostByMonthMaterials.Attach(entity);
                db.ProjectCostByMonthMaterials.Remove(entity);
                db.SaveChanges();

                //CotTrackingHelper.UpdateCostTrackingRecord(entity.ProjectCostByMonthId);
            }

            return Json(new[] { costingMaterial }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}