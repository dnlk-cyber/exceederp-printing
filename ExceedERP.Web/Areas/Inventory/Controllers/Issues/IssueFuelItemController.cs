using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Issues
{
    
    public class IssueFuelItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private DistributionHelper distHelper = new DistributionHelper();

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            return View();
        }

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover, FinanceConstants.GeneralSubLedgerTransfer)]
        public ActionResult IssueFuelItems_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<IssueItem> issuefuelItems = db.IssueItems.Where(isf => isf.IssueID == id);
            DataSourceResult result = issuefuelItems.ToDataSourceResult(request, issueFuelItem => new {
                IssueItemID = issueFuelItem.IssueItemID,
                IssueID = issueFuelItem.IssueID,
                ItemCategoryCode = issueFuelItem.ItemCategoryCode,
                ItemCode = issueFuelItem.ItemCode,
                RequestedQuantity = issueFuelItem.RequestedQuantity,
                ApprovedQuantity = issueFuelItem.ApprovedQuantity,
                IssuedQuantity = issueFuelItem.IssuedQuantity,
                UnitPrice = issueFuelItem.UnitPrice,
                AdditionalCost = issueFuelItem.AdditionalCost,
                AgreementPrice = issueFuelItem.AgreementPrice,
                Total = issueFuelItem.Total,
               
                EmployeeOfFuelCompany = issueFuelItem.EmployeeOfFuelCompany,
               
                EmployeeWhoGotItRefuelled = issueFuelItem.EmployeeWhoGotItRefuelled,
              
                Name = issueFuelItem.Name,
                OrgBranchName = issueFuelItem.OrgBranchName,
            });

            return Json(result);
        }

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IssueFuelItems_Create([DataSourceRequest]DataSourceRequest request, IssueItem issueFuelItem , int id )
        {
            var fuelissue = db.Issues.Find(id);

            var exist = db.IssueItems.Where(x => x.IssueID == id)
            .Where(x => x.ItemCode == issueFuelItem.ItemCode)
            .AsQueryable();
            if (exist.Any())
            {
                ModelState.AddModelError("exist", "Item " + issueFuelItem.ItemCode + " already exists");
            }


            //if (fuelissue != null && (fuelissue.Owner == IssueFuelVehicleOwner.External && issueFuelItem.AdditionalCost == 0))
            //{
            //    ModelState.AddModelError("additional cost","Please fill additional cost");
            //}
            decimal avgPrice = 0;
            var issue = db.Issues.Find(id);
            if (issue != null)
                avgPrice = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(issueFuelItem.ItemCode, issue.StoreCode);

            //if (avgPrice == 0)
            //{
            //    ModelState.AddModelError("invalidValue", "Item Price of " + issueFuelItem.InventoryItem.Name + " is " + avgPrice);
            //}
            var storeItem = db.StoreItemAssignments.FirstOrDefault(x => x.ItemCode == issueFuelItem.ItemCode && x.StoreCode == issueFuelItem.Issue.StoreCode);
            if (storeItem == null)
                ModelState.AddModelError("not defined", "The item " + issueFuelItem.InventoryItem.Name + " is not available in store " + issueFuelItem.Issue.StoreCode);

            if (storeItem != null && storeItem.Balance < issueFuelItem.IssuedQuantity)
                ModelState.AddModelError("minBalance", "The remaining balance in Store" + storeItem.StoreCode + " is only " + storeItem.Balance);


            if (ModelState.IsValid)
            {
                var entity = new IssueItem
                {

                    IssueID = id,
                    ItemCategoryCode = issueFuelItem.ItemCategoryCode,
                    ItemCode = issueFuelItem.ItemCode,
                    RequestedQuantity = issueFuelItem.RequestedQuantity,
                    ApprovedQuantity = issueFuelItem.ApprovedQuantity,
                    IssuedQuantity = issueFuelItem.IssuedQuantity,
                    UnitPrice = avgPrice,
                    AdditionalCost = issueFuelItem.AdditionalCost,
                    AgreementPrice = issueFuelItem.AgreementPrice,
                    Total = issueFuelItem.IssuedQuantity * issueFuelItem.UnitPrice + issueFuelItem.AdditionalCost,
                   
                    EmployeeOfFuelCompany = issueFuelItem.EmployeeOfFuelCompany,
                    EmployeeWhoGotItRefuelled = issueFuelItem.EmployeeWhoGotItRefuelled,
                  
                    Name = issueFuelItem.Name,
                    OrgBranchName = issueFuelItem.OrgBranchName,
                };

                db.IssueItems.Add(entity);
                db.SaveChanges();
                issueFuelItem.IssueItemID = entity.IssueItemID;
              
                #region distribution
                var record = db.Issues.Find(id);
                if (record != null)
                   distHelper.UpdateIssueDistribution(record);
                #endregion
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "issue fuel item", "Success", User.Identity.Name, $"created issue fuel item {entity.ItemCode} under record {entity.IssueID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { issueFuelItem }.ToDataSourceResult(request, ModelState));
        }

          [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IssueFuelItems_Update([DataSourceRequest]DataSourceRequest request, IssueItem issueFuelItem , int id)
        {
            var fuelissue = db.Issues.Find(id);

            var exist = db.IssueItems.Where(x => x.IssueID == id)
              .Where(x => x.IssueItemID != issueFuelItem.IssueItemID)
            .Where(x => x.ItemCode == issueFuelItem.ItemCode)
            .AsQueryable();
            if (exist.Any())
            {
                ModelState.AddModelError("exist", "Item " + issueFuelItem.ItemCode + " already exists");
            }

            //if (fuelissue != null && (fuelissue.Owner == IssueFuelVehicleOwner.External && issueFuelItem.AdditionalCost == 0))
            //{
            //    ModelState.AddModelError("additionalcost", "Please fill additional cost");
            //}
            decimal avgPrice = 0;
            var issue = db.Issues.Find(id);
            if (issue != null)
            avgPrice = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(issueFuelItem.ItemCode, issue.StoreCode);

            //if (avgPrice == 0)
            //{
            //    ModelState.AddModelError("invalidValue", "Item Price of " + issueFuelItem.InventoryItem.Name + " is " + avgPrice);
            //}

            var storeItem = db.StoreItemAssignments.FirstOrDefault(x => x.ItemCode == issueFuelItem.ItemCode && x.StoreCode == issue.StoreCode);
            if (storeItem == null)
                ModelState.AddModelError("not defined", "The item " + issueFuelItem.InventoryItem.Name + " is not available in store " + issue.StoreCode);

            if (storeItem != null && storeItem.Balance < issueFuelItem.IssuedQuantity)
                ModelState.AddModelError("minBalance", "The remaining balance in Store" + storeItem.StoreCode + " is only " + storeItem.Balance);


            if (ModelState.IsValid)
            {
                var entity = new IssueItem
                {
                    IssueItemID = issueFuelItem.IssueItemID,
                    IssueID = issueFuelItem.IssueID,
                    ItemCategoryCode = issueFuelItem.ItemCategoryCode,
                    ItemCode = issueFuelItem.ItemCode,
                    RequestedQuantity = issueFuelItem.RequestedQuantity,
                    ApprovedQuantity = issueFuelItem.ApprovedQuantity,
                    IssuedQuantity = issueFuelItem.IssuedQuantity,
                    UnitPrice = avgPrice,
                    AdditionalCost = issueFuelItem.AdditionalCost,
                    AgreementPrice = issueFuelItem.AgreementPrice,
                    Total = issueFuelItem.IssuedQuantity * issueFuelItem.UnitPrice+issueFuelItem.AdditionalCost,
                   
                    EmployeeOfFuelCompany = issueFuelItem.EmployeeOfFuelCompany,
                    EmployeeWhoGotItRefuelled = issueFuelItem.EmployeeWhoGotItRefuelled,
                  
                    Name = issueFuelItem.Name,
                    OrgBranchName = issueFuelItem.OrgBranchName,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                };

                db.IssueItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region distribution
                var record = db.Issues.Find(id);
                if (record != null)
                   distHelper.UpdateIssueDistribution(record);
                #endregion

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "issue fuel item", "Success", User.Identity.Name, $"updated issue fuel item {entity.ItemCode} under record {entity.IssueID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { issueFuelItem }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IssueFuelItems_Destroy([DataSourceRequest]DataSourceRequest request,  IssueItem issueFuelItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new IssueItem
                {
                    IssueItemID = issueFuelItem.IssueItemID,
                    IssueID = issueFuelItem.IssueID,
                    ItemCode = issueFuelItem.ItemCode,
                    RequestedQuantity = issueFuelItem.RequestedQuantity,
                    ApprovedQuantity = issueFuelItem.ApprovedQuantity,
                    IssuedQuantity = issueFuelItem.IssuedQuantity,
                    UnitPrice = issueFuelItem.UnitPrice,
                    AdditionalCost = issueFuelItem.AdditionalCost,
                    AgreementPrice = issueFuelItem.AgreementPrice,
                    Total = issueFuelItem.Total,
                   
                    EmployeeOfFuelCompany = issueFuelItem.EmployeeOfFuelCompany,
                    EmployeeWhoGotItRefuelled = issueFuelItem.EmployeeWhoGotItRefuelled,
                  
                    Name = issueFuelItem.Name,
                    OrgBranchName = issueFuelItem.OrgBranchName,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                db.IssueItems.Attach(entity);
                db.IssueItems.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted issue fuel item " + issueFuelItem.IssueID + " " + issueFuelItem.InventoryItem.Name);
                #endregion

                #region distribution
                var record = db.Issues.Find(issueFuelItem.IssueID);
                if (record != null)
                   distHelper.UpdateIssueDistribution(record);
                #endregion
            }

            return Json(new[] { issueFuelItem }.ToDataSourceResult(request, ModelState));
        }

       
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
