using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Inventory;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Construction.Controllers.Costing
{
    [AuthorizeRoles(ConstructionConstants.CostingUser)]
    public class ProjectExpensesController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            ViewData["ItemCategories"] = db.ItemCategorys.Select(p => new { p.ItemCategoryCode, p.Name }).ToList();

            ViewData["InventoryItems"] = db.InventoryItems.Select(i => new { i.Name, i.ItemCode}).ToList();

            ViewData["PeriodList"] = db.InventoryPeriods.Select(p => new { p.Name, p.InventoryPeriodId}).ToList();

            ViewData["Branchs"] = db.Branch.Select(b => new { b.Name, b.BranchId}).ToList();

            //this.PrepareProjectMaterialExpense();
            return View();
        }
        public ActionResult ProjectBranches_Read([DataSourceRequest] DataSourceRequest request)
        {
            var branches =
                db.Branch;
            var branchesToReturn = new List<MaterialExpenseProjectViewModel>();
            foreach (Branch branch in branches)
            {
                var record = new MaterialExpenseProjectViewModel
                {
                    Name = branch.Name,
                    BranchId = branch.BranchId,
                    Code = branch.Code
                };
                branchesToReturn.Add(record);
            }
            IQueryable<MaterialExpenseProjectViewModel> branchQuaryable = branchesToReturn.AsQueryable();

            return Json(branchQuaryable.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ProjectInformation_Read([DataSourceRequest]DataSourceRequest request)
        {
            var currentUser = User.Identity.Name;
            var assignments = db.ConstructionProjectUserAssignments.Where(assgn => assgn.UserName == currentUser).ToList();
            var ids = new HashSet<int>(assignments.Select(a => a.ProjectInformationId));

            IQueryable<ProjectInformation> projectinformation = db.ProjectInformation.Where(p => !p.IsDeleted && ids.Contains(p.ProjectInformationId));//.Where (x=> x.ProjectStatus == ProjectStatus .STARTED );
            DataSourceResult result = projectinformation.ToDataSourceResult(request, projectInformation => new
            {
                ProjectInformationId = projectInformation.ProjectInformationId,
                Name = projectInformation.Name,
                Code = projectInformation.Code,
                Category = projectInformation.Category,
                ProjectType = projectInformation.ProjectType,
                BranchId = projectInformation.BranchId,
                ContractAmount = projectInformation.ContractAmount,
                ProjectScope = projectInformation.ProjectScope,
                EmployeeId = projectInformation.EmployeeId,
                Client = projectInformation.Client,
                Consultant = projectInformation.Consultant,
                ProjectStatus = projectInformation.ProjectStatus,

                OrgBranchName = projectInformation.OrgBranchName,
                DateCreated = projectInformation.DateCreated,
                LastModified = projectInformation.LastModified,
                CreatedBy = projectInformation.CreatedBy,
                ModifiedBy = projectInformation.ModifiedBy,
            });

            return Json(result);
        }
        //
        //  Summary:
        //      prepare material expenses for each branch
        public ActionResult PrepareProjectMaterialExpense()
        {
            #region used when consumption was analyzed by branches instead of projects
            //var joinedBranches = (from b in db.Branch
            //                      join gr in db.GoodReceives
            //                      on b.BranchId equals gr.BranchId
            //                      select new
            //                      {
            //                          BranchId = b.BranchId
            //                      }).ToList(); 
            #endregion
            var joinedBranches = (from p in db.ProjectInformation
                                  join b in db.Branch
                                  on p.Code equals b.Code
                                  join gr in db.GoodReceives
                                  on b.BranchId equals gr.BranchId
                                  select new
                                  {
                                      BranchId = p.BranchId
                                  }).ToList();
            // Branches
            var branchIds = new HashSet<int>(joinedBranches.Select(p => p.BranchId));
            var consumptionBranches = db.Branch.Where(br => (branchIds.Contains(br.BranchId))).ToList();

            foreach (Branch currentBranch in consumptionBranches)
            {
                var periodsWithValues = (from p in db.InventoryPeriods
                                         join r in db.GoodReceives
                                         on p.InventoryPeriodId equals r.GLPeriodId
                                         where r.BranchId == currentBranch.BranchId
                                         select new
                                         {
                                             GLPeriodId = p.InventoryPeriodId
                                         }).ToList();
                var ids = new HashSet<int>(periodsWithValues.Select(p => p.GLPeriodId));
                var consumptionPeriods = db.InventoryPeriods.Where(p => (ids.Contains(p.InventoryPeriodId))).ToList();

                foreach (InventoryPeriod currentPeriod in consumptionPeriods)
                {
                    var materials = (from i in db.InventoryItems
                                     join g in db.GoodReceiveItems
                                     on i.ItemCode equals g.ItemCode
                                     where g.GoodReceive.GLPeriodId == currentPeriod.InventoryPeriodId && g.GoodReceive.BranchId == currentBranch.BranchId
                                     select new
                                     {
                                         ItemCategoryCode = i.ItemCategoryCode,
                                         ItemCode = i.ItemCode,
                                     }).ToList();

                    // consumption materials
                    var codes = new HashSet<string>(materials.Select(p => p.ItemCode));
                    var consumedMaterials = db.InventoryItems.Where(item => (codes.Contains(item.ItemCode))).ToList();

                    foreach (InventoryItem currentItem in consumedMaterials)
                    {
                        //var materials = db.GoodReceiveItems.Where(gri => gri.GoodReceive.GLPeriodId == currentPeriod.GLPeriodId && gri.GoodReceive.BranchId == currentBranch.BranchId).ToList();
                        //if (materials.Any())
                        //{
                        //foreach (var recivedItem in materials) {
                        ExtractMaterialConsumptionData(currentBranch.BranchId, currentPeriod.InventoryPeriodId, currentItem);
                        //}
                        //}
                        //if(currentItem.G)
                    }
                }
            }
            this.CreateDirectAndIndirectExpenses();

            return Redirect("Index");
        }
        private void ExtractMaterialConsumptionData(int branchId, int periodId, InventoryItem inventoryItem)
        {
            var existingItem = db.ProjectMaterialExpenses.Where(i => i.BranchId == branchId &&
                                                                i.GLPeriodId == periodId &&
                                                                i.ItemCode == inventoryItem.ItemCode
                                                            ).FirstOrDefault();
            if (existingItem == null)
            {
                var entity = new ProjectMaterialExpense
                {
                    BranchId = branchId,
                    GLPeriodId = periodId,
                    ItemCategoryCode = inventoryItem.ItemCategoryCode,
                    ItemCode = inventoryItem.ItemCode,
                    Unit = InventoryHelper.GetUnitOfMeasurment(inventoryItem.ItemCode),
                    UnitPrice = InventoryHelper.GetAverageUnitPrice(branchId, inventoryItem.ItemCode),
                    DeliveredQuantity = InventoryHelper.GetDeliveredQuantityForThisMonth(branchId, periodId, inventoryItem.ItemCode),
                    ConsumedQuantity = InventoryHelper.GetConsumedQuanityForThisMonth(branchId, periodId, inventoryItem.ItemCode),
                    TranIn = InventoryHelper.GetTransferedInForThisMonth(branchId, periodId, inventoryItem.ItemCode),
                    TranOut = InventoryHelper.GetTransferedOoutForThisMonth(branchId, periodId, inventoryItem.ItemCode),
                    DeliveredQuantityPrevMonth = InventoryHelper.GetDeliveredQuantityTillPrevMonth(branchId, periodId, inventoryItem.ItemCode),
                    ConsumedQuantityPrevMonth = InventoryHelper.GetConsumedQuanityTillPrevMonth(branchId, periodId, inventoryItem.ItemCode),
                    MaterilOnSitePrevMonth = 0,
                    DeliveredQuantityToDate = 0,
                    ConsumedQuantityToDate = 0,
                    ShipedQuantityToDate = 0,
                    StockBalance = 0,
                    MaterialOnSiteToDate = 0,

                    OrgBranchName = null,
                    DateCreated = DateTime.Now,
                    LastModified = null,
                    CreatedBy = null,
                    ModifiedBy = null
                };
                entity.DeliveredQuantityToDate = entity.DeliveredQuantity + entity.DeliveredQuantityPrevMonth;
                entity.ConsumedQuantityToDate = entity.ConsumedQuantity + entity.ConsumedQuantityPrevMonth;
                entity.StockBalance = entity.DeliveredQuantityToDate - entity.ConsumedQuantityToDate - entity.MaterialOnSiteToDate;
                entity.CostOfConsumedMaterialPrevMonth = entity.UnitPrice * entity.ConsumedQuantityPrevMonth;
                entity.CostOfConsumedMaterialThisMonth = entity.UnitPrice * entity.ConsumedQuantity;
                entity.CostOfConsumedMaterialToDate = entity.UnitPrice * entity.ConsumedQuantityToDate;
                entity.StockAmount = entity.StockBalance * entity.UnitPrice;
                entity.MaterialOnSiteAmount = entity.MaterialOnSiteToDate * entity.UnitPrice;

                db.ProjectMaterialExpenses.Add(entity);
                db.SaveChanges();
            }

        }
        private void CreateDirectAndIndirectExpenses()
        {

            var allExpenses = db.ProjectMaterialExpenses.ToList();
            if (allExpenses.Any())
            {
                foreach (ProjectMaterialExpense expense in allExpenses)
                {
                    if (MaterialIsUsedInEstimation(expense.ItemCode, expense.BranchId))
                    {
                        var existingMaterial = db.ProjectDirectMaterialExpenses
                            .FirstOrDefault(m => m.BranchId == expense.BranchId && 
                                            m.ItemCode == expense.ItemCode && 
                                            m.GLPeriodId == expense.GLPeriodId);
                        if(existingMaterial == null)
                        {
                            var expenseToAdd = new ProjectDirectMaterialExpense
                            {
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
                                DateCreated = DateTime.Now,
                                LastModified = null,
                                CreatedBy = User.Identity.Name,
                                ModifiedBy = null
                            };
                            db.ProjectDirectMaterialExpenses.Add(expenseToAdd);
                            db.SaveChanges();
                        }
                        
                    }
                    else
                    {
                        var existingMaterial = db.ProjectIndirectMaterialExpenses
                           .FirstOrDefault(m => m.BranchId == expense.BranchId &&
                                           m.ItemCode == expense.ItemCode &&
                                           m.GLPeriodId == expense.GLPeriodId);
                        if(existingMaterial == null)
                        {
                            var expenseToAdd = new ProjectIndirectMaterialExpense
                            {
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
                                DateCreated = DateTime.Now,
                                LastModified = null,
                                CreatedBy = User.Identity.Name,
                                ModifiedBy = null
                            };
                            db.ProjectIndirectMaterialExpenses.Add(expenseToAdd);
                            db.SaveChanges();
                        }
                        
                    }
                }
            }
        }
        private bool MaterialIsUsedInEstimation(string itemCode, int branchId)
        {
            var materialIsUsedInEstimation = false;
            var currentProject = db.ProjectInformation.FirstOrDefault(p => p.BranchId == branchId);
            if (currentProject != null)
            {
                var workItems = db.WorkItemPerProject.Where(wi => wi.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                if (workItems.Any())
                {
                    var ids = new HashSet<int>(workItems.Select(w => w.WorkItemPerProjectId));

                    var materialInEstimation = db.ProjectMaterial.Where(m => (m.ItemCode == itemCode && ids.Contains(m.WorkItemPerProjectId))).ToList();
                    if (materialInEstimation.Any())
                    {
                        materialIsUsedInEstimation = true;
                    }
                }
            }
            return materialIsUsedInEstimation;
        }
    }
}