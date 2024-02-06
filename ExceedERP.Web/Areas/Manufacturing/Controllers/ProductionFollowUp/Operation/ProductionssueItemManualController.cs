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
namespace ExceedERP.Web.Areas.Manufacturing.Controllers.ProductionFollowUp.Operation
{

    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpDeliveryUser)]
    public class ProductionssueItemManualController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        //private DistributionHelper distHelper = new DistributionHelper();

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            return View();
        }


      //  [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover, FinanceConstants.GeneralSubLedgerTransfer)]
        public ActionResult IssueItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<IssueItem> issueitems = db.IssueItems.Where(i => i.IssueID == id);
            DataSourceResult result = issueitems.ToDataSourceResult(request, issueItem => new
            {
                IssueID = issueItem.IssueID,
                IssueItemID = issueItem.IssueItemID,
                ItemCategoryCode = issueItem.ItemCategoryCode,
                ItemCode = issueItem.ItemCode,
                RequestedQuantity = issueItem.RequestedQuantity,
                ApprovedQuantity = issueItem.ApprovedQuantity,
                IssuedQuantity = issueItem.IssuedQuantity,
                UnitPrice = issueItem.UnitPrice,
                Total = issueItem.Total,
                AdditionalCost = issueItem.AdditionalCost,
                WorkItemCode = issueItem.WorkItemCode,


                OrgBranchName = issueItem.OrgBranchName,
                DateCreated = issueItem.DateCreated,
                LastModified = issueItem.LastModified,
                CreatedBy = issueItem.CreatedBy,
                ModifiedBy = issueItem.ModifiedBy
            });

            return Json(result,JsonRequestBehavior.AllowGet);
        }

        //[AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IssueItems_Create([DataSourceRequest]DataSourceRequest request, IssueItem issueItem, int id)
        {

            var exist = db.IssueItems.Where(x => x.IssueID == id)
             .Where(x => x.ItemCode == issueItem.ItemCode)
             .AsQueryable();
            if (exist.Any())
            {
                ModelState.AddModelError("exist", "Item " + issueItem.ItemCode + " already exists");
            }

            var issue = db.Issues.Find(id);
            decimal avgPrice = 0;
            if (issue != null)
                avgPrice = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(issueItem.ItemCode, issue.StoreCode);

            //if (avgPrice == 0)
            //{
            //    ModelState.AddModelError("invalidValue", "Item Price of " + issueItem.InventoryItem.Name + " is " + avgPrice);
            //}

            var storeItem = db.StoreItemAssignments.FirstOrDefault(x => x.ItemCode == issueItem.ItemCode && x.StoreCode == issueItem.Issue.StoreCode);
            if (storeItem == null)
                ModelState.AddModelError("not defined", "The item " + issueItem.InventoryItem.Name + " is not available in store " + issueItem.Issue.StoreCode);

            if (storeItem != null && storeItem.Balance < issueItem.IssuedQuantity)
                ModelState.AddModelError("minBalance", "The remaining balance in Store" + storeItem.StoreCode + " is only " + storeItem.Balance);

            if (ModelState.IsValid)
            {
                var entity = new IssueItem
                {
                    IssueID = id,
                    ItemCategoryCode = issueItem.ItemCategoryCode,
                    ItemCode = issueItem.ItemCode,
                    RequestedQuantity = issueItem.RequestedQuantity,
                    ApprovedQuantity = issueItem.ApprovedQuantity,
                    IssuedQuantity = issueItem.IssuedQuantity,
                    WorkItemCode = issueItem.WorkItemCode,
                    AdditionalCost = issueItem.AdditionalCost,
                    UnitPrice = avgPrice,
                    Total = issueItem.IssuedQuantity*avgPrice,
                    OrgBranchName = issueItem.OrgBranchName,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                };

                db.IssueItems.Add(entity);
                db.SaveChanges();
                issueItem.IssueItemID = entity.IssueItemID;                

                var record = db.Issues.Find(id);
                if (record != null)
           //    distHelper.UpdateIssueDistribution(record);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "issue item", "Success", User.Identity.Name, $"created issue item {entity.ItemCode} under record {entity.IssueID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { issueItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IssueItems_Update([DataSourceRequest]DataSourceRequest request, IssueItem issueItem, int id)
        {

            var exist = db.IssueItems.Where(x => x.IssueID == id)
                .Where(x => x.IssueItemID != issueItem.IssueItemID)
          .Where(x => x.ItemCode == issueItem.ItemCode)
          .AsQueryable();
            if (exist.Any())
            {
                ModelState.AddModelError("exist", "Item " + issueItem.ItemCode + " already exists");
            }

            decimal avgPrice = 0;
            var issue=db.Issues.Find(id);
            if (issue != null)
            avgPrice = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(issueItem.ItemCode, issue.StoreCode);

            //if (avgPrice == 0)
            //{
            //    ModelState.AddModelError("invalidValue", "Item Price of " + issueItem.InventoryItem.Name + " is " + avgPrice);
            //}



            var storeItem = db.StoreItemAssignments.FirstOrDefault(x => x.ItemCode == issueItem.ItemCode && x.StoreCode == issue.StoreCode);
            if (storeItem == null)
                ModelState.AddModelError("not defined", "The item " + issueItem.InventoryItem.Name + " is not available in store " + issue.StoreCode);

            if (storeItem != null && storeItem.Balance < issueItem.IssuedQuantity)
                ModelState.AddModelError("minBalance", "The remaining balance in Store" + storeItem.StoreCode + " is only " + storeItem.Balance);


            if (ModelState.IsValid)
            {
                var entity = new IssueItem
                {
                    IssueItemID = issueItem.IssueItemID,
                    IssueID = issueItem.IssueID,
                    ItemCategoryCode = issueItem.ItemCategoryCode,
                    ItemCode = issueItem.ItemCode,
                    WorkItemCode = issueItem.WorkItemCode,
                    AdditionalCost = issueItem.AdditionalCost,
                    RequestedQuantity = issueItem.RequestedQuantity,
                    ApprovedQuantity = issueItem.ApprovedQuantity,
                    IssuedQuantity = issueItem.IssuedQuantity,
                    UnitPrice = avgPrice,
                    Total = issueItem.IssuedQuantity * avgPrice,
                    OrgBranchName = issueItem.OrgBranchName,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                db.IssueItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "issue item", "Success", User.Identity.Name, $"updated issue item {entity.ItemCode} under record {entity.IssueID} ", Modules.Inventory);
                #endregion

                var record = db.Issues.Find(id);
                //if (record != null)
                //   distHelper.UpdateIssueDistribution(record);
            }

            return Json(new[] { issueItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IssueItems_Destroy([DataSourceRequest]DataSourceRequest request, IssueItem issueItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new IssueItem
                {
                    IssueItemID = issueItem.IssueItemID,
                    ItemCategoryCode = issueItem.ItemCategoryCode,
                    ItemCode = issueItem.ItemCode,
                    RequestedQuantity = issueItem.RequestedQuantity,
                    ApprovedQuantity = issueItem.ApprovedQuantity,
                    IssuedQuantity = issueItem.IssuedQuantity,
                    UnitPrice = issueItem.UnitPrice,

                    Total = issueItem.Total,
                   

                    OrgBranchName = issueItem.OrgBranchName,
                    DateCreated = issueItem.DateCreated,
                    LastModified = issueItem.LastModified,
                    CreatedBy = issueItem.CreatedBy,
                    ModifiedBy = issueItem.ModifiedBy
                };

                db.IssueItems.Attach(entity);
                db.IssueItems.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted issue item " + issueItem.IssueID + " " + issueItem.InventoryItem.Name);
                #endregion
                #region distribution
                var record = db.Issues.Find(issueItem.IssueID);
                //if (record != null)
                //   distHelper.UpdateIssueDistribution(record);
                #endregion
            }

            return Json(new[] { issueItem }.ToDataSourceResult(request, ModelState));
        }

       
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
