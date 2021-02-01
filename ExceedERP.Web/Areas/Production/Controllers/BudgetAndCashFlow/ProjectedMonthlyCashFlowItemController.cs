using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Production;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.BudgetAndCashFlow
{
    public class ProjectedMonthlyCashFlowItemController : Controller
    {
        private ExceedDbContext db;
        public ProjectedMonthlyCashFlowItemController()
        {
            db = new ExceedDbContext();
        }

        public ActionResult CashFlow_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ProjectedMonthlyCashFlowItem> cashFlowItems = db.ProjectedMonthlyCashFlowItems.Where(i => i.ProjectedMonthlyCashFlowId == id);
            DataSourceResult result = cashFlowItems.ToDataSourceResult(request, cashFlow => new
            {
                ProjectedMonthlyCashFlowItemId = cashFlow.ProjectedMonthlyCashFlowItemId,
                ProjectedMonthlyCashFlowId = cashFlow.ProjectedMonthlyCashFlowId,
                ProductionPeriodId = cashFlow.ProductionPeriodId,

                CleanProductSales = cashFlow.CleanProductSales,
                CustomCleaningIncome = cashFlow.CustomCleaningIncome,
                ByproductSales = cashFlow.ByproductSales,
                TotalIncome = cashFlow.TotalIncome,
                RawProductPurchase = cashFlow.RawProductPurchase,
                OtherDirectCosts = cashFlow.OtherDirectCosts,
                ProcessingAndStorage = cashFlow.ProcessingAndStorage,
                Maintenance = cashFlow.Maintenance,
                Administration = cashFlow.Administration,
                HeadOfficeAllocation = cashFlow.HeadOfficeAllocation,
                TotalExpenditure = cashFlow.TotalExpenditure,
                Depreciation = cashFlow.Depreciation,
                TotalCashExpenditure = cashFlow.TotalCashExpenditure,
                NetCashIncome = cashFlow.NetCashIncome,
                CumulativeCashIncome = cashFlow.CumulativeCashIncome,
                AdditionalIncome = cashFlow.AdditionalIncome,
                RevisedNetCashIncome = cashFlow.RevisedNetCashIncome,
                RevisedCumulativeCashIncome = cashFlow.RevisedCumulativeCashIncome,

                OrgBranchName = cashFlow.OrgBranchName,
                DateCreated = cashFlow.DateCreated,
                LastModified = cashFlow.LastModified,
                CreatedBy = cashFlow.CreatedBy,
                ModifiedBy = cashFlow.ModifiedBy,
            });

            return Json(result);
        }
        private bool ItemExists(ProjectedMonthlyCashFlowItem record, int headerId)
        {
            return db.ProjectedMonthlyCashFlowItems
                .FirstOrDefault(i => 
                    i.ProjectedMonthlyCashFlowId == headerId && 
                    i.ProductionPeriodId == record.ProductionPeriodId && 
                    i.ProjectedMonthlyCashFlowItemId != record.ProjectedMonthlyCashFlowItemId) != null;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CashFlow_Create([DataSourceRequest]DataSourceRequest request, ProjectedMonthlyCashFlowItem cashFlow, int id)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(cashFlow, id))
                {
                    ModelState.AddModelError("", "Duplicate Values not Allowed");
                }
                else
                {
                    var entity = new ProjectedMonthlyCashFlowItem
                    {
                        ProjectedMonthlyCashFlowItemId = cashFlow.ProjectedMonthlyCashFlowItemId,
                        ProjectedMonthlyCashFlowId = id,
                        ProductionPeriodId = cashFlow.ProductionPeriodId,

                        CleanProductSales = cashFlow.CleanProductSales,
                        CustomCleaningIncome = cashFlow.CustomCleaningIncome,
                        ByproductSales = cashFlow.ByproductSales,
                        TotalIncome = cashFlow.TotalIncome,
                        RawProductPurchase = cashFlow.RawProductPurchase,
                        OtherDirectCosts = cashFlow.OtherDirectCosts,
                        ProcessingAndStorage = cashFlow.ProcessingAndStorage,
                        Maintenance = cashFlow.Maintenance,
                        Administration = cashFlow.Administration,
                        HeadOfficeAllocation = cashFlow.HeadOfficeAllocation,
                        TotalExpenditure = cashFlow.TotalExpenditure,
                        Depreciation = cashFlow.Depreciation,
                        TotalCashExpenditure = cashFlow.TotalCashExpenditure,
                        NetCashIncome = cashFlow.NetCashIncome,
                        CumulativeCashIncome = cashFlow.CumulativeCashIncome,
                        AdditionalIncome = cashFlow.AdditionalIncome,
                        RevisedNetCashIncome = cashFlow.RevisedNetCashIncome,
                        RevisedCumulativeCashIncome = cashFlow.RevisedCumulativeCashIncome,

                        OrgBranchName = cashFlow.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = cashFlow.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = cashFlow.ModifiedBy,

                    };
                    entity.TotalIncome = entity.CleanProductSales + entity.CustomCleaningIncome + entity.ByproductSales;

                    entity.TotalExpenditure = entity.RawProductPurchase +
                                                entity.OtherDirectCosts +
                                                entity.ProcessingAndStorage +
                                                entity.Maintenance +
                                                entity.Administration +
                                                entity.HeadOfficeAllocation;

                    entity.TotalCashExpenditure = entity.TotalExpenditure - entity.Depreciation;

                    entity.NetCashIncome = entity.TotalIncome - entity.TotalCashExpenditure;
                    entity.CumulativeCashIncome = entity.NetCashIncome;

                    entity.RevisedNetCashIncome = entity.NetCashIncome;
                    entity.RevisedCumulativeCashIncome = entity.CumulativeCashIncome;

                    db.ProjectedMonthlyCashFlowItems.Add(entity);
                    db.SaveChanges();

                    cashFlow.ProjectedMonthlyCashFlowId = entity.ProjectedMonthlyCashFlowId;
                }
            }

            return Json(new[] { cashFlow }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CashFlow_Update([DataSourceRequest]DataSourceRequest request, ProjectedMonthlyCashFlowItem cashFlow)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(cashFlow, cashFlow.ProjectedMonthlyCashFlowId))
                {
                    ModelState.AddModelError("", "Duplicate Values not Allowed");
                }
                else
                {
                    var entity = new ProjectedMonthlyCashFlowItem
                    {
                        ProjectedMonthlyCashFlowItemId = cashFlow.ProjectedMonthlyCashFlowItemId,
                        ProjectedMonthlyCashFlowId = cashFlow.ProjectedMonthlyCashFlowId,
                        ProductionPeriodId = cashFlow.ProductionPeriodId,

                        CleanProductSales = cashFlow.CleanProductSales,
                        CustomCleaningIncome = cashFlow.CustomCleaningIncome,
                        ByproductSales = cashFlow.ByproductSales,
                        TotalIncome = cashFlow.TotalIncome,
                        RawProductPurchase = cashFlow.RawProductPurchase,
                        OtherDirectCosts = cashFlow.OtherDirectCosts,
                        ProcessingAndStorage = cashFlow.ProcessingAndStorage,
                        Maintenance = cashFlow.Maintenance,
                        Administration = cashFlow.Administration,
                        HeadOfficeAllocation = cashFlow.HeadOfficeAllocation,
                        TotalExpenditure = cashFlow.TotalExpenditure,
                        Depreciation = cashFlow.Depreciation,
                        TotalCashExpenditure = cashFlow.TotalCashExpenditure,
                        NetCashIncome = cashFlow.NetCashIncome,
                        CumulativeCashIncome = cashFlow.CumulativeCashIncome,
                        AdditionalIncome = cashFlow.AdditionalIncome,
                        RevisedNetCashIncome = cashFlow.RevisedNetCashIncome,
                        RevisedCumulativeCashIncome = cashFlow.RevisedCumulativeCashIncome,

                        OrgBranchName = cashFlow.OrgBranchName,
                        DateCreated = cashFlow.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = cashFlow.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                    };
                    entity.TotalIncome = entity.CleanProductSales + entity.CustomCleaningIncome + entity.ByproductSales;

                    entity.TotalExpenditure = entity.RawProductPurchase +
                                                entity.OtherDirectCosts +
                                                entity.ProcessingAndStorage +
                                                entity.Maintenance +
                                                entity.Administration +
                                                entity.HeadOfficeAllocation;

                    entity.TotalCashExpenditure = entity.TotalExpenditure - entity.Depreciation;

                    entity.NetCashIncome = entity.TotalIncome - entity.TotalCashExpenditure;
                    entity.CumulativeCashIncome = entity.NetCashIncome;

                    db.ProjectedMonthlyCashFlowItems.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    ProductionBudgetAndCashFlowHelper.UpdateProjectedMonthlyCashflow(entity.ProductionPeriodId, GetBranchId(entity.ProjectedMonthlyCashFlowId));
                }
            }

            return Json(new[] { cashFlow }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CashFlow_Destroy([DataSourceRequest]DataSourceRequest request, ProjectedMonthlyCashFlowItem cashFlow)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectedMonthlyCashFlowItem
                {
                    ProjectedMonthlyCashFlowItemId = cashFlow.ProjectedMonthlyCashFlowItemId,
                    ProjectedMonthlyCashFlowId = cashFlow.ProjectedMonthlyCashFlowId,
                    ProductionPeriodId = cashFlow.ProductionPeriodId,

                    CleanProductSales = cashFlow.CleanProductSales,
                    CustomCleaningIncome = cashFlow.CustomCleaningIncome,
                    ByproductSales = cashFlow.ByproductSales,
                    TotalIncome = cashFlow.TotalIncome,
                    RawProductPurchase = cashFlow.RawProductPurchase,
                    OtherDirectCosts = cashFlow.OtherDirectCosts,
                    ProcessingAndStorage = cashFlow.ProcessingAndStorage,
                    Maintenance = cashFlow.Maintenance,
                    Administration = cashFlow.Administration,
                    HeadOfficeAllocation = cashFlow.HeadOfficeAllocation,
                    TotalExpenditure = cashFlow.TotalExpenditure,
                    Depreciation = cashFlow.Depreciation,
                    TotalCashExpenditure = cashFlow.TotalCashExpenditure,
                    NetCashIncome = cashFlow.NetCashIncome,
                    CumulativeCashIncome = cashFlow.CumulativeCashIncome,
                    AdditionalIncome = cashFlow.AdditionalIncome,
                    RevisedNetCashIncome = cashFlow.RevisedNetCashIncome,
                    RevisedCumulativeCashIncome = cashFlow.RevisedCumulativeCashIncome,

                    OrgBranchName = cashFlow.OrgBranchName,
                    DateCreated = cashFlow.DateCreated,
                    LastModified = cashFlow.LastModified,
                    CreatedBy = cashFlow.CreatedBy,
                    ModifiedBy = cashFlow.ModifiedBy,

                };

                db.ProjectedMonthlyCashFlowItems.Attach(entity);
                db.ProjectedMonthlyCashFlowItems.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { cashFlow }.ToDataSourceResult(request, ModelState));
        }
        private int GetBranchId(int headerId)
        {
            var id = -1;
            var header = db.ProjectedMonthlyCashFlow.Find(headerId);
            if(header != null)
            {
                id = header.BranchId;
            }
            return id;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}