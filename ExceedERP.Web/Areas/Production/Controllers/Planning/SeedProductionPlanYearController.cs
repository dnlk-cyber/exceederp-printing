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
    public class SeedProductionPlanYearController : Controller
    {
        ExceedDbContext db;
        // GET: Production/SeedProductionPlanYear
        public SeedProductionPlanYearController()
        {
            db = new ExceedDbContext();
        }

        public ActionResult Years_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<SeedProductionPlanYear> planYears = db.SeedProductionPlanYear.Where(a => a.ProductionProjectId == id);
            DataSourceResult result = planYears.ToDataSourceResult(request, year => new
            {
                SeedProductionPlanYearId = year.SeedProductionPlanYearId,
                GlFiscalYearId = year.GlFiscalYearId,
                ProductionProjectId = year.ProductionProjectId,
                Remark = year.Remark,

                OrgBranchName = year.OrgBranchName,
                DateCreated = year.DateCreated,
                LastModified = year.LastModified,
                CreatedBy = year.CreatedBy,
                ModifiedBy = year.ModifiedBy,
            });

            return Json(result);
        }

        private bool PeriodExists(int projectId, SeedProductionPlanYear model, ControllerFunctionType action)
        {
            var valueExists = false;
            if (action == ControllerFunctionType.Create)
            {
                var temp = db.SeedProductionPlanYear
                    .FirstOrDefault(y => y.ProductionProjectId == projectId && 
                                    y.GlFiscalYearId == model.GlFiscalYearId);
                if(temp != null)
                {
                    valueExists = true;
                }
            }
            else if (action == ControllerFunctionType.Update)
            {
                var temp = db.SeedProductionPlanYear
                    .FirstOrDefault(y => y.ProductionProjectId == projectId &&
                                    y.GlFiscalYearId == model.GlFiscalYearId &&
                                    y.SeedProductionPlanYearId != model.SeedProductionPlanYearId);
                if(temp != null)
                {
                    valueExists = true;
                }
            }
            
            return valueExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Year_Create([DataSourceRequest]DataSourceRequest request, SeedProductionPlanYear year, int id)
        {
            if (ModelState.IsValid)
            {
                if (PeriodExists(id, year, ControllerFunctionType.Create))
                {
                    ModelState.AddModelError("", "This Year is Registered already");
                }
                else
                {
                    var entity = new SeedProductionPlanYear
                    {
                        GlFiscalYearId = year.GlFiscalYearId,
                        ProductionProjectId = id,
                        Remark = year.Remark,

                        OrgBranchName = year.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = year.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = year.ModifiedBy,

                    };

                    db.SeedProductionPlanYear.Add(entity);
                    db.SaveChanges();

                    year.SeedProductionPlanYearId = entity.SeedProductionPlanYearId;

                }

            }

            return Json(new[] { year }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Year_Update([DataSourceRequest]DataSourceRequest request, SeedProductionPlanYear year)
        {
            if (ModelState.IsValid)
            {
                if (PeriodExists(year.ProductionProjectId, year, ControllerFunctionType.Update))
                {
                    ModelState.AddModelError("", "This Year is Registered already");
                }
                else
                {
                    var entity = new SeedProductionPlanYear
                    {
                        SeedProductionPlanYearId = year.SeedProductionPlanYearId,
                        GlFiscalYearId = year.GlFiscalYearId,
                        ProductionProjectId = year.ProductionProjectId,
                        Remark = year.Remark,

                        OrgBranchName = year.OrgBranchName,
                        DateCreated = year.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = year.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                    };

                    db.SeedProductionPlanYear.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                

            }

            return Json(new[] { year }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Year_Destroy([DataSourceRequest]DataSourceRequest request, SeedProductionPlanYear year)
        {
            if (ModelState.IsValid)
            {


                var entity = new SeedProductionPlanYear
                {
                    SeedProductionPlanYearId = year.SeedProductionPlanYearId,
                    GlFiscalYearId = year.GlFiscalYearId,
                    ProductionProjectId = year.ProductionProjectId,
                    Remark = year.Remark,

                    OrgBranchName = year.OrgBranchName,
                    DateCreated = year.DateCreated,
                    LastModified = year.LastModified,
                    CreatedBy = year.CreatedBy,
                    ModifiedBy = year.ModifiedBy,

                };

                db.SeedProductionPlanYear.Attach(entity);
                db.SeedProductionPlanYear.Remove(entity);
                db.SaveChanges();
            }


            return Json(new[] { year }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}