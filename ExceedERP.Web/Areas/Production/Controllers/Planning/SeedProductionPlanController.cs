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

namespace ExceedERP.Web.Areas.Production.Controllers.Planning
{
    public class SeedProductionPlanController : Controller
    {
        // GET: Production/SeedProductionPlan
        ExceedDbContext db;
        public SeedProductionPlanController()
        {
            db = new ExceedDbContext();
        }
        public ActionResult Index()
        {
            var years = db.GLFiscalYears.ToList();
            ViewData["FiscalYears"] = years;

            var crops = db.Crops.ToList();
            ViewData["Crops"] = crops;
            return View();
        }

        public ActionResult ProductionPlan_Read([DataSourceRequest]DataSourceRequest request, int yearId)
        {
            IQueryable<SeedProductionPlan> plans = db.SeedProductionPlans.Where(p => p.SeedProductionPlanYearId == yearId);
            DataSourceResult result = plans.ToDataSourceResult(request, plan => new
            {
                SeedProductionPlanId = plan.SeedProductionPlanId,
                SeedProductionPlanYearId = plan.SeedProductionPlanYearId,
                CropId = plan.CropId,
                Type = plan.Type,
                Generation = plan.Generation,
                Area = plan.Area,
                YieldQuantity = plan.YieldQuantity,
                TotalQuantity = plan.TotalQuantity,

                OrgBranchName = plan.OrgBranchName,
                DateCreated = plan.DateCreated,
                LastModified = plan.LastModified,
                CreatedBy = plan.CreatedBy,
                ModifiedBy = plan.ModifiedBy,
            });

            return Json(result);
        }
        private bool PlanForSeedExists(int yearId, SeedProductionPlan model, ControllerFunctionType action)
        {
            var valueExists = false;
            if (action == ControllerFunctionType.Create)
            {
                var temp = db.SeedProductionPlans.FirstOrDefault(p =>
                                            p.SeedProductionPlanYearId == yearId &&
                                            p.Generation == model.Generation &&
                                            p.Type == model.Type &&
                                            p.CropId == model.CropId);
                if (temp != null)
                {
                    valueExists = true;
                }
            }
            else if (action == ControllerFunctionType.Update)
            {
                var temp = db.SeedProductionPlans.FirstOrDefault(p =>
                                            p.SeedProductionPlanYearId == yearId &&
                                            p.Generation == model.Generation &&
                                            p.Type == model.Type &&
                                            p.CropId == model.CropId &&
                                            p.SeedProductionPlanId != model.SeedProductionPlanId);
                if (temp != null)
                {
                    valueExists = true;
                }
            }

            //if(temp != null)
            //{
            //    valueExists = true;
            //}
            return valueExists;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionPlan_Create([DataSourceRequest]DataSourceRequest request, SeedProductionPlan plan, int yearId)
        {
            if (ModelState.IsValid)
            {
                if (PlanForSeedExists(yearId, plan, ControllerFunctionType.Create))
                {
                    ModelState.AddModelError("", "Duplicate Values are not allowed");
                }
                else
                {
                    var entity = new SeedProductionPlan
                    {
                        SeedProductionPlanId = plan.SeedProductionPlanId,
                        SeedProductionPlanYearId = yearId,
                        CropId = plan.CropId,
                        Type = plan.Type,
                        Generation = plan.Generation,
                        Area = plan.Area,
                        YieldQuantity = plan.YieldQuantity,
                        TotalQuantity = plan.TotalQuantity,

                        OrgBranchName = plan.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = plan.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = plan.ModifiedBy,

                    };
                    entity.TotalQuantity = entity.YieldQuantity * entity.Area;

                    db.SeedProductionPlans.Add(entity);
                    db.SaveChanges();

                    plan.SeedProductionPlanId = entity.SeedProductionPlanId;
                }
            }

            return Json(new[] { plan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionPlan_Update([DataSourceRequest]DataSourceRequest request, SeedProductionPlan plan)
        {
            if (ModelState.IsValid)
            {
                if (PlanForSeedExists(plan.SeedProductionPlanYearId, plan, ControllerFunctionType.Update))
                {
                    ModelState.AddModelError("", "Duplicate Values are not allowed");
                }
                else
                {
                    var entity = new SeedProductionPlan
                    {
                        SeedProductionPlanId = plan.SeedProductionPlanId,
                        SeedProductionPlanYearId = plan.SeedProductionPlanYearId,
                        CropId = plan.CropId,
                        Type = plan.Type,
                        Generation = plan.Generation,
                        Area = plan.Area,
                        YieldQuantity = plan.YieldQuantity,
                        TotalQuantity = plan.TotalQuantity,

                        OrgBranchName = plan.OrgBranchName,
                        DateCreated = plan.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = plan.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                    };
                    entity.TotalQuantity = entity.YieldQuantity * entity.Area;

                    db.SeedProductionPlans.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { plan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionPlan_Destroy([DataSourceRequest]DataSourceRequest request, SeedProductionPlan plan)
        {
            if (ModelState.IsValid)
            {
                var entity = new SeedProductionPlan
                {
                    SeedProductionPlanId = plan.SeedProductionPlanId,
                    SeedProductionPlanYearId = plan.SeedProductionPlanYearId,
                    CropId = plan.CropId,
                    Type = plan.Type,
                    Generation = plan.Generation,
                    Area = plan.Area,
                    YieldQuantity = plan.YieldQuantity,
                    TotalQuantity = plan.TotalQuantity,

                    OrgBranchName = plan.OrgBranchName,
                    DateCreated = plan.DateCreated,
                    LastModified = plan.LastModified,
                    CreatedBy = plan.CreatedBy,
                    ModifiedBy = plan.ModifiedBy,

                };

                db.SeedProductionPlans.Attach(entity);
                db.SeedProductionPlans.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { plan }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}