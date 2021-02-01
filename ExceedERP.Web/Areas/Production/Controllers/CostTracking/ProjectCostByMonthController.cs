using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
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
    public class ProjectCostByMonthController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CostByMonth_Read([DataSourceRequest]DataSourceRequest request, int periodId)
        {
            IQueryable<ProjectCostByMonth> costs = db.ProjectCostByMonth.Where(a => a.CostTrackingPeriodId == periodId);
            DataSourceResult result = costs.ToDataSourceResult(request, cost => new
            {
                ProjectCostByMonthId = cost.ProjectCostByMonthId,
                CostTrackingPeriodId = cost.CostTrackingPeriodId,
                CropId = cost.CropId,
                Type = cost.Type,
                Generation = cost.Generation,

                MaterialCost = cost.MaterialCost,
                LaborCost = cost.LaborCost,
                EquipmentCost = cost.EquipmentCost,
                TotalCost = cost.TotalCost,

                OrgBranchName = cost.OrgBranchName,
                DateCreated = cost.DateCreated,
                LastModified = cost.LastModified,
                CreatedBy = cost.CreatedBy,
                ModifiedBy = cost.ModifiedBy,
            });

            return Json(result);
        }

        private bool CostByMonthValueExists(int periodId, ProjectCostByMonth record)
        {
            var valueExists = false;
            var temp = db.ProjectCostByMonth.FirstOrDefault(p => p.CostTrackingPeriodId == periodId && 
                                                            p.CropId == record.CropId && 
                                                            p.Generation == record.Generation &&
                                                            p.Type == record.Type );
            if (temp != null)
            {
                valueExists = true;
            }

            return valueExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CostByMonth_Create([DataSourceRequest]DataSourceRequest request, ProjectCostByMonth cost, int periodId)
        {
            if (ModelState.IsValid)
            {
                if(CostByMonthValueExists(periodId, cost))
                {
                    ModelState.AddModelError("", "Duplicate values should't be used");
                }
                else
                {
                    var entity = new ProjectCostByMonth
                    {
                        ProjectCostByMonthId = cost.ProjectCostByMonthId,
                        CostTrackingPeriodId = periodId,
                        CropId = cost.CropId,
                        Type = cost.Type,
                        Generation = cost.Generation,

                        MaterialCost = cost.MaterialCost,
                        LaborCost = cost.LaborCost,
                        EquipmentCost = cost.EquipmentCost,
                        TotalCost = cost.TotalCost,

                        OrgBranchName = cost.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = cost.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = cost.ModifiedBy,

                    };
                    entity.TotalCost = entity.MaterialCost + entity.LaborCost + entity.EquipmentCost;

                    db.ProjectCostByMonth.Add(entity);
                    db.SaveChanges();

                    cost.ProjectCostByMonthId = entity.ProjectCostByMonthId;
                }
            }

            return Json(new[] { cost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CostByMonth_Update([DataSourceRequest]DataSourceRequest request, ProjectCostByMonth cost)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectCostByMonth
                {
                    ProjectCostByMonthId = cost.ProjectCostByMonthId,
                    CostTrackingPeriodId = cost.CostTrackingPeriodId,
                    CropId = cost.CropId,
                    Type = cost.Type,
                    Generation = cost.Generation,

                    MaterialCost = cost.MaterialCost,
                    LaborCost = cost.LaborCost,
                    EquipmentCost = cost.EquipmentCost,
                    TotalCost = cost.TotalCost,

                    OrgBranchName = cost.OrgBranchName,
                    DateCreated = cost.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = cost.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };
                entity.TotalCost = entity.MaterialCost + entity.LaborCost + entity.EquipmentCost;

                db.ProjectCostByMonth.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { cost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CostByMonth_Destroy([DataSourceRequest]DataSourceRequest request, ProjectCostByMonth cost)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectCostByMonth
                {
                    ProjectCostByMonthId = cost.ProjectCostByMonthId,
                    CostTrackingPeriodId = cost.CostTrackingPeriodId,
                    CropId = cost.CropId,
                    Type = cost.Type,
                    Generation = cost.Generation,

                    MaterialCost = cost.MaterialCost,
                    LaborCost = cost.LaborCost,
                    EquipmentCost = cost.EquipmentCost,
                    TotalCost = cost.TotalCost,

                    OrgBranchName = cost.OrgBranchName,
                    DateCreated = cost.DateCreated,
                    LastModified = cost.LastModified,
                    CreatedBy = cost.CreatedBy,
                    ModifiedBy = cost.ModifiedBy,

                };

                db.ProjectCostByMonth.Attach(entity);
                db.ProjectCostByMonth.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { cost }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}