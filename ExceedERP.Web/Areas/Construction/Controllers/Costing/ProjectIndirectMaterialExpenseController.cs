using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Inventory;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Costing
{
    public class ProjectIndirectMaterialExpenseController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MaterialExpense_Read([DataSourceRequest]DataSourceRequest request, int branchId)
        {
            IQueryable<ProjectIndirectMaterialExpense> expenses = db.ProjectIndirectMaterialExpenses.Where(me => me.BranchId == branchId);
            DataSourceResult result = expenses.ToDataSourceResult(request, expense => new
            {
                ProjectIndirectMaterialExpenseId = expense.ProjectIndirectMaterialExpenseId,
                BranchId = expense.BranchId,
                GLPeriodId = expense.GLPeriodId,
                ItemCategoryCode = expense.ItemCategoryCode,
                ItemCode = expense.ItemCode,
                Unit = expense.Unit,
                UnitPrice = expense.UnitPrice,
                DeliveredQuantity = expense.DeliveredQuantity,
                ConsumedQuantity = expense.ConsumedQuantity,
                TranIn = expense.TranIn,
                TranOut = expense.TranOut,
                DeliveredQuantityPrevMonth = expense.DeliveredQuantityPrevMonth,
                ConsumedQuantityPrevMonth = expense.ConsumedQuantityPrevMonth,
                MaterilOnSitePrevMonth = expense.MaterilOnSitePrevMonth,
                DeliveredQuantityToDate = expense.DeliveredQuantityToDate,
                ConsumedQuantityToDate = expense.ConsumedQuantityToDate,
                ShipedQuantityToDate = expense.ShipedQuantityToDate,
                MaterialOnSiteToDate = expense.MaterialOnSiteToDate,
                StockBalance = expense.StockBalance,
                CostOfConsumedMaterialThisMonth = expense.CostOfConsumedMaterialThisMonth,
                CostOfConsumedMaterialPrevMonth = expense.CostOfConsumedMaterialPrevMonth,
                CostOfConsumedMaterialToDate = expense.CostOfConsumedMaterialToDate,
                StockAmount = expense.StockAmount,
                MaterialOnSiteAmount = expense.MaterialOnSiteAmount,

                OrgBranchName = expense.OrgBranchName,
                DateCreated = expense.DateCreated,
                LastModified = expense.LastModified,
                CreatedBy = expense.CreatedBy,
                ModifiedBy = expense.ModifiedBy
            });

            return Json(result);
        }
        private bool ExpenseDataExists(int periodId, int branchId, string itemCode)
        {
            var materialIsRegistered = false;
            var temp = db.ProjectIndirectMaterialExpenses.FirstOrDefault(p => p.BranchId == branchId && p.GLPeriodId == periodId && p.ItemCode == itemCode);
            if (temp != null)
            {
                materialIsRegistered = true;
            }
            return materialIsRegistered;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaterialExpense_Create([DataSourceRequest]DataSourceRequest request, ProjectIndirectMaterialExpense expense, int branchId)
        {
            if (ModelState.IsValid)
            {
                if (ExpenseDataExists(expense.GLPeriodId, branchId, expense.ItemCode))
                {
                    ModelState.AddModelError("", "This material exists");
                }
                else
                {
                    var entity = new ProjectIndirectMaterialExpense
                    {
                        ProjectIndirectMaterialExpenseId = expense.ProjectIndirectMaterialExpenseId,
                        BranchId = branchId,
                        GLPeriodId = expense.GLPeriodId,
                        ItemCategoryCode = expense.ItemCategoryCode,
                        ItemCode = expense.ItemCode,
                        Unit = InventoryHelper.GetUnitOfMeasurment(expense.ItemCode),
                        UnitPrice = InventoryHelper.GetAverageUnitPrice(branchId, expense.ItemCode),
                        DeliveredQuantity = InventoryHelper.GetDeliveredQuantityForThisMonth(branchId, expense.GLPeriodId, expense.ItemCode),
                        ConsumedQuantity = InventoryHelper.GetConsumedQuanityForThisMonth(branchId, expense.GLPeriodId, expense.ItemCode),
                        TranIn = InventoryHelper.GetTransferedInForThisMonth(branchId, expense.GLPeriodId, expense.ItemCode),
                        TranOut = InventoryHelper.GetTransferedOoutForThisMonth(branchId, expense.GLPeriodId, expense.ItemCode),
                        DeliveredQuantityPrevMonth = InventoryHelper.GetDeliveredQuantityTillPrevMonth(branchId, expense.GLPeriodId, expense.ItemCode),
                        ConsumedQuantityPrevMonth = InventoryHelper.GetConsumedQuanityTillPrevMonth(branchId, expense.GLPeriodId, expense.ItemCode),
                        MaterilOnSitePrevMonth = expense.MaterilOnSitePrevMonth,
                        DeliveredQuantityToDate = expense.DeliveredQuantityToDate,
                        ConsumedQuantityToDate = expense.ConsumedQuantityToDate,
                        ShipedQuantityToDate = expense.ShipedQuantityToDate,
                        StockBalance = expense.StockBalance,
                        MaterialOnSiteToDate = expense.MaterialOnSiteToDate,
                        CostOfConsumedMaterialThisMonth = expense.CostOfConsumedMaterialThisMonth,
                        CostOfConsumedMaterialPrevMonth = expense.CostOfConsumedMaterialPrevMonth,
                        CostOfConsumedMaterialToDate = expense.CostOfConsumedMaterialToDate,
                        StockAmount = expense.StockAmount,
                        MaterialOnSiteAmount = expense.MaterialOnSiteAmount,

                        OrgBranchName = expense.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = expense.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = expense.ModifiedBy
                    };
                    entity.DeliveredQuantityToDate = entity.DeliveredQuantity + entity.DeliveredQuantityPrevMonth;
                    entity.ConsumedQuantityToDate = entity.ConsumedQuantity + entity.ConsumedQuantityPrevMonth;
                    entity.StockBalance = entity.DeliveredQuantityToDate - entity.ConsumedQuantityToDate - entity.MaterialOnSiteToDate;
                    entity.CostOfConsumedMaterialPrevMonth = entity.UnitPrice * entity.ConsumedQuantityPrevMonth;
                    entity.CostOfConsumedMaterialThisMonth = entity.UnitPrice * entity.ConsumedQuantity;
                    entity.CostOfConsumedMaterialToDate = entity.UnitPrice * entity.ConsumedQuantityToDate;
                    entity.StockAmount = entity.StockBalance * entity.UnitPrice;
                    entity.MaterialOnSiteAmount = entity.MaterialOnSiteToDate * entity.UnitPrice;

                    db.ProjectIndirectMaterialExpenses.Add(entity);
                    db.SaveChanges();
                    expense.ProjectIndirectMaterialExpenseId = entity.ProjectIndirectMaterialExpenseId;

                    expense.Unit = entity.Unit;
                    expense.UnitPrice = entity.UnitPrice;
                    expense.DeliveredQuantity = entity.DeliveredQuantity;
                    expense.ConsumedQuantity = entity.ConsumedQuantity;
                    expense.DeliveredQuantityPrevMonth = entity.DeliveredQuantityPrevMonth;
                    expense.ConsumedQuantityPrevMonth = entity.ConsumedQuantityPrevMonth;
                    expense.DeliveredQuantityToDate = entity.DeliveredQuantityToDate;
                    expense.ConsumedQuantityToDate = entity.ConsumedQuantityToDate;
                    expense.MaterialOnSiteToDate = entity.MaterialOnSiteToDate;
                    expense.CostOfConsumedMaterialPrevMonth = entity.CostOfConsumedMaterialPrevMonth;
                    expense.CostOfConsumedMaterialThisMonth = entity.CostOfConsumedMaterialThisMonth;
                    expense.CostOfConsumedMaterialToDate = entity.CostOfConsumedMaterialToDate;
                    expense.StockAmount = entity.StockAmount;
                    expense.MaterialOnSiteAmount = entity.MaterialOnSiteAmount;
                }

            }
            return Json(new[] { expense }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaterialExpense_Update([DataSourceRequest]DataSourceRequest request, ProjectIndirectMaterialExpense expense)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectIndirectMaterialExpense
                {
                    ProjectIndirectMaterialExpenseId = expense.ProjectIndirectMaterialExpenseId,
                    BranchId = expense.BranchId,
                    GLPeriodId = expense.GLPeriodId,
                    ItemCategoryCode = expense.ItemCategoryCode,
                    ItemCode = expense.ItemCode,
                    Unit = InventoryHelper.GetUnitOfMeasurment(expense.ItemCode),
                    //UnitPrice = InventoryHelper.GetAverageUnitPrice(expense.ProjectInformationId, expense.ItemCode),
                    //DeliveredQuantity = InventoryHelper.GetDeliveredQuantityForThisMonth(expense.ProjectInformationId, expense.GLPeriodId, expense.ItemCode),
                    //ConsumedQuantity = InventoryHelper.GetConsumedQuanityForThisMonth(expense.ProjectInformationId, expense.GLPeriodId, expense.ItemCode),
                    TranIn = expense.TranIn,
                    TranOut = expense.TranOut,
                    //DeliveredQuantityPrevMonth = InventoryHelper.GetDeliveredQuantityTillPrevMonth(expense.ProjectInformationId, expense.GLPeriodId, expense.ItemCode),
                    //ConsumedQuantityPrevMonth = InventoryHelper.GetConsumedQuanityTillPrevMonth(expense.ProjectInformationId, expense.GLPeriodId, expense.ItemCode),
                    MaterilOnSitePrevMonth = expense.MaterilOnSitePrevMonth,
                    DeliveredQuantityToDate = expense.DeliveredQuantityToDate,
                    ConsumedQuantityToDate = expense.ConsumedQuantityToDate,
                    ShipedQuantityToDate = expense.ShipedQuantityToDate,
                    StockBalance = expense.StockBalance,
                    MaterialOnSiteToDate = expense.MaterialOnSiteToDate,
                    CostOfConsumedMaterialThisMonth = expense.CostOfConsumedMaterialThisMonth,
                    CostOfConsumedMaterialPrevMonth = expense.CostOfConsumedMaterialPrevMonth,
                    CostOfConsumedMaterialToDate = expense.CostOfConsumedMaterialToDate,
                    StockAmount = expense.StockAmount,
                    MaterialOnSiteAmount = expense.MaterialOnSiteAmount,

                    OrgBranchName = expense.OrgBranchName,
                    DateCreated = expense.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = expense.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                entity.DeliveredQuantityToDate = entity.DeliveredQuantity + entity.DeliveredQuantityPrevMonth;
                entity.ConsumedQuantityToDate = entity.ConsumedQuantity + entity.ConsumedQuantityPrevMonth;
                entity.StockBalance = entity.DeliveredQuantityToDate - entity.ConsumedQuantityToDate - entity.MaterialOnSiteToDate;
                entity.CostOfConsumedMaterialPrevMonth = entity.UnitPrice * entity.ConsumedQuantityPrevMonth;
                entity.CostOfConsumedMaterialThisMonth = entity.UnitPrice * entity.ConsumedQuantity;
                entity.CostOfConsumedMaterialToDate = entity.UnitPrice * entity.ConsumedQuantityToDate;
                entity.StockAmount = entity.StockBalance * entity.UnitPrice;
                entity.MaterialOnSiteAmount = entity.MaterialOnSiteToDate * entity.UnitPrice;

                db.ProjectIndirectMaterialExpenses.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                expense.Unit = entity.Unit;
                expense.UnitPrice = entity.UnitPrice;
                expense.DeliveredQuantity = entity.DeliveredQuantity;
                expense.ConsumedQuantity = entity.ConsumedQuantity;
                expense.DeliveredQuantityPrevMonth = entity.DeliveredQuantityPrevMonth;
                expense.ConsumedQuantityPrevMonth = entity.ConsumedQuantityPrevMonth;
                expense.DeliveredQuantityToDate = entity.DeliveredQuantityToDate;
                expense.ConsumedQuantityToDate = entity.ConsumedQuantityToDate;
                expense.StockBalance = entity.StockBalance;
                expense.CostOfConsumedMaterialPrevMonth = entity.CostOfConsumedMaterialPrevMonth;
                expense.CostOfConsumedMaterialThisMonth = entity.CostOfConsumedMaterialThisMonth;
                expense.CostOfConsumedMaterialToDate = entity.CostOfConsumedMaterialToDate;
                expense.StockAmount = entity.StockAmount;
                expense.MaterialOnSiteAmount = entity.MaterialOnSiteAmount;
            }

            return Json(new[] { expense }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaterialExpense_Destroy([DataSourceRequest]DataSourceRequest request, ProjectIndirectMaterialExpense expense)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectIndirectMaterialExpense
                {
                    ProjectIndirectMaterialExpenseId = expense.ProjectIndirectMaterialExpenseId,
                    BranchId = expense.BranchId,
                    GLPeriodId = expense.GLPeriodId,
                    ItemCategoryCode = expense.ItemCategoryCode,
                    ItemCode = expense.ItemCode,
                    Unit = expense.Unit,
                    UnitPrice = expense.UnitPrice,
                    DeliveredQuantity = expense.DeliveredQuantity,
                    ConsumedQuantity = expense.ConsumedQuantity,
                    TranIn = expense.TranIn,
                    TranOut = expense.TranOut,
                    DeliveredQuantityPrevMonth = expense.DeliveredQuantityPrevMonth,
                    ConsumedQuantityPrevMonth = expense.ConsumedQuantityPrevMonth,
                    MaterilOnSitePrevMonth = expense.MaterilOnSitePrevMonth,
                    DeliveredQuantityToDate = expense.DeliveredQuantityToDate,
                    ConsumedQuantityToDate = expense.ConsumedQuantityToDate,
                    ShipedQuantityToDate = expense.ShipedQuantityToDate,
                    MaterialOnSiteToDate = expense.MaterialOnSiteToDate,
                    StockBalance = expense.StockBalance,
                    CostOfConsumedMaterialThisMonth = expense.CostOfConsumedMaterialThisMonth,
                    CostOfConsumedMaterialPrevMonth = expense.CostOfConsumedMaterialPrevMonth,
                    CostOfConsumedMaterialToDate = expense.CostOfConsumedMaterialToDate,
                    StockAmount = expense.StockAmount,
                    MaterialOnSiteAmount = expense.MaterialOnSiteAmount,

                    OrgBranchName = expense.OrgBranchName,
                    DateCreated = expense.DateCreated,
                    LastModified = expense.LastModified,
                    CreatedBy = expense.CreatedBy,
                    ModifiedBy = expense.ModifiedBy
                };

                db.ProjectIndirectMaterialExpenses.Attach(entity);
                db.ProjectIndirectMaterialExpenses.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { expense }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}