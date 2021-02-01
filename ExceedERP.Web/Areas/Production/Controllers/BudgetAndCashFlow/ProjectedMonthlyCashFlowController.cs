using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Helpers.Production;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.BudgetAndCashFlow
{
    public class ProjectedMonthlyCashFlowController : Controller
    {
        // GET: Production/Crop
        ExceedDbContext db;
        public ProjectedMonthlyCashFlowController()
        {
            db = new ExceedDbContext();
        }
        public ActionResult Index()
        {
            return View();
        }
       
        public ActionResult CashFlow_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectedMonthlyCashFlow> cashFlowItems = db.ProjectedMonthlyCashFlow;
            DataSourceResult result = cashFlowItems.ToDataSourceResult(request, cashFlow => new 
            {
                ProjectedMonthlyCashFlowId = cashFlow.ProjectedMonthlyCashFlowId,
                BranchId = cashFlow.BranchId,
                GlFiscalYearId = cashFlow.GlFiscalYearId,                
                Remark = cashFlow.Remark,                

                OrgBranchName = cashFlow.OrgBranchName,
                DateCreated = cashFlow.DateCreated,
                LastModified = cashFlow.LastModified,
                CreatedBy = cashFlow.CreatedBy,
                ModifiedBy = cashFlow.ModifiedBy,
            });

            return Json(result);
        }
        private bool ItemExists(ProjectedMonthlyCashFlow record)
        {
            return db.ProjectedMonthlyCashFlow
                .FirstOrDefault(i => i.BranchId == record.BranchId && i.GlFiscalYearId == record.GlFiscalYearId && i.ProjectedMonthlyCashFlowId != record.ProjectedMonthlyCashFlowId) != null;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CashFlow_Create([DataSourceRequest]DataSourceRequest request, ProjectedMonthlyCashFlow cashFlow)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(cashFlow))
                {
                    ModelState.AddModelError("", "Duplicate Values not Allowed");
                }
                else
                {
                    var entity = new ProjectedMonthlyCashFlow
                    {
                        ProjectedMonthlyCashFlowId = cashFlow.ProjectedMonthlyCashFlowId,
                        BranchId = cashFlow.BranchId,
                        GlFiscalYearId = cashFlow.GlFiscalYearId,
                        Remark = cashFlow.Remark,

                        OrgBranchName = cashFlow.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = cashFlow.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = cashFlow.ModifiedBy,

                    };

                    db.ProjectedMonthlyCashFlow.Add(entity);
                    db.SaveChanges();

                    cashFlow.ProjectedMonthlyCashFlowId = entity.ProjectedMonthlyCashFlowId;

                    this.CreateMonthlyCashFlowItems(entity);
                    this.CreateCleanedProductSaleHeader(entity);
                    this.CreateRawProductPurchaseHeader(entity);
                    this.CreateCustomCleaningHeder(entity);
                }
            }

            return Json(new[] { cashFlow }.ToDataSourceResult(request, ModelState));
        }

        

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CashFlow_Update([DataSourceRequest]DataSourceRequest request, ProjectedMonthlyCashFlow cashFlow)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(cashFlow))
                {
                    ModelState.AddModelError("", "Duplicate Values not Allowed");
                }
                else
                {
                    var entity = new ProjectedMonthlyCashFlow
                    {
                        ProjectedMonthlyCashFlowId = cashFlow.ProjectedMonthlyCashFlowId,
                        BranchId = cashFlow.BranchId,
                        GlFiscalYearId = cashFlow.GlFiscalYearId,
                        Remark = cashFlow.Remark,

                        OrgBranchName = cashFlow.OrgBranchName,
                        DateCreated = cashFlow.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = cashFlow.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                    };

                    db.ProjectedMonthlyCashFlow.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { cashFlow }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CashFlow_Destroy([DataSourceRequest]DataSourceRequest request, ProjectedMonthlyCashFlow cashFlow)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectedMonthlyCashFlow
                {
                    ProjectedMonthlyCashFlowId = cashFlow.ProjectedMonthlyCashFlowId,
                    BranchId = cashFlow.BranchId,
                    GlFiscalYearId = cashFlow.GlFiscalYearId,
                    Remark = cashFlow.Remark,

                    OrgBranchName = cashFlow.OrgBranchName,
                    DateCreated = cashFlow.DateCreated,
                    LastModified = cashFlow.LastModified,
                    CreatedBy = cashFlow.CreatedBy,
                    ModifiedBy = cashFlow.ModifiedBy,

                };

                db.ProjectedMonthlyCashFlow.Attach(entity);
                db.ProjectedMonthlyCashFlow.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { cashFlow }.ToDataSourceResult(request, ModelState));
        }

        private void CreateMonthlyCashFlowItems(ProjectedMonthlyCashFlow header)
        {
            var year = header.GlFiscalYear;
            if(header.GlFiscalYear == null)
            {
                year = db.GLFiscalYears.Find(header.GlFiscalYearId);
            }
            if(year != null)
            {
                var periods = db.ProductionPeriods
                .Where(p => p.Status == Core.Domain.Finance.GL.FiscalYearStatus.Open &&
                        p.DateFrom >= year.DateFrom &&
                        p.DateTo <= year.DateTo).ToList();

                if (periods.Any())
                {
                    foreach(ProductionPeriod period in periods)
                    {
                        var entity = new ProjectedMonthlyCashFlowItem
                        {
                            ProductionPeriodId = period.ProductionPeriodId,
                            ProjectedMonthlyCashFlowId = header.ProjectedMonthlyCashFlowId
                        };
                        db.ProjectedMonthlyCashFlowItems.Add(entity);
                        db.SaveChanges();
                    }
                }

            }

        }
        private void CreateCustomCleaningHeder(ProjectedMonthlyCashFlow entity)
        {
            var existingRecord = db.CustomCleaningAndStorage.FirstOrDefault(cc => cc.BranchId == entity.BranchId && cc.GlFiscalYearId == entity.GlFiscalYearId);
            if(existingRecord == null)
            {
                var record = new CustomCleaningAndStorage
                {
                    BranchId = entity.BranchId,
                    GlFiscalYearId = entity.GlFiscalYearId
                };
                db.CustomCleaningAndStorage.Add(record);
                db.SaveChanges();
            }
        }

        private void CreateRawProductPurchaseHeader(ProjectedMonthlyCashFlow entity)
        {
            var existingRecord = db.RawProductPurchases.FirstOrDefault(cc => cc.BranchId == entity.BranchId && cc.GlFiscalYearId == entity.GlFiscalYearId);
            if (existingRecord == null)
            {
                var record = new RawProductPurchase
                {
                    BranchId = entity.BranchId,
                    GlFiscalYearId = entity.GlFiscalYearId
                };
                db.RawProductPurchases.Add(record);
                db.SaveChanges();
            }
        }

        private void CreateCleanedProductSaleHeader(ProjectedMonthlyCashFlow entity)
        {
            var existingRecord = db.CleanedProductSales.FirstOrDefault(cc => cc.BranchId == entity.BranchId && cc.GlFiscalYearId == entity.GlFiscalYearId);
            if (existingRecord == null)
            {
                var record = new CleanedProductSale
                {
                    BranchId = entity.BranchId,
                    GlFiscalYearId = entity.GlFiscalYearId
                };
                db.CleanedProductSales.Add(record);
                db.SaveChanges();
            }
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}