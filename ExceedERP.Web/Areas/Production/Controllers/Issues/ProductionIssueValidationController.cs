using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Production.Controllers.Issues
{
    //[AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyApprover, InventoryConstants.PropertyUser)]
    public class ProductionIssueValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        //[AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            return View();
        }

        //[AuthorizeRoles(InventoryConstants.PropertyApprover)]
        public ActionResult IssueValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ProductionIssueValidation> issuevalidations = db.ProductionIssueValidations.Where(ii => ii.ProductionIssueId == id);
            DataSourceResult result = issuevalidations.ToDataSourceResult(request, issueValidation => new
            {
                ProductionIssueValidationId = issueValidation.ProductionIssueValidationId,
                ProductionIssueId = issueValidation.ProductionIssueId,
                Status = issueValidation.Status,
                Remark = issueValidation.Remark,
            });

            return Json(result);
        }

        //[AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IssueValidations_Create([DataSourceRequest] DataSourceRequest request, ProductionIssueValidation issueValidation, int id)
        {

            var record = db.ProductionIssues.Find(id);
            var issueItems = db.ProductionIssueItems.Where(x => x.ProductionIssueId == id).AsNoTracking().ToList();
            var pricesetting = db.AveragePriceSettings.FirstOrDefault();
            #region validation 

            if (record != null)
            {
                if (record.IsOnlineApproved)
                {
                    string message = "You cannot manipulate this record,it  is already approved";
                    ModelState.AddModelError("approved", message);
                }
                {
                    if (issueValidation.Status == ApprovalStatuses.Approve)
                    {
                        foreach (var i in issueItems)
                        {

                            #region stock

                            var storeItem = db.StoreItemAssignments.Where(x => x.ItemCode == i.ItemCode).FirstOrDefault(x => x.StoreCode == record.StoreCode);
                            if (storeItem == null)
                            {
                                string message = "Item: " + i.InventoryItem.Name + " is not available in store: " + record.StoreCode;
                                ModelState.AddModelError("store", message);

                            }

                            if (storeItem != null && storeItem.Balance - i.IssuedQuantity < 0)
                            {
                                ModelState.AddModelError("enoughQuant", "No enough quantity To Issue for item " + i.ItemCode);
                            }

                            if (storeItem != null && storeItem.Balance - i.IssuedQuantity >= 0)
                            {
                                decimal avgPrice = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(i.ItemCode, record.StoreCode);
                                if (avgPrice == 0)
                                {
                                    ModelState.AddModelError("invalidValue", " Item Price of " + i.ItemCode + " is " + 0);
                                }
                            }

                            #endregion
                            
                        }
                    }
                    if (issueValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }
                }
            }
            else
            {
                ModelState.AddModelError("No Found", "Issue record not found");
            }


            #endregion

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    //validate 
                    if (issueValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.ProductionIssueId = id;
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();
                        #region update stock based on the issue
                        string storecode = record.StoreCode;
                        foreach (var i in issueItems)
                        {
                            if (i.IssuedQuantity > 0)
                            {
                                string itemCode = i.ItemCode;
                                decimal avgPrice = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(i.ItemCode, storecode);
                                i.UnitPrice = avgPrice;
                                #region stock

                                var storeItem =db.StoreItemAssignments.FirstOrDefault(
                                            x => x.StoreCode == storecode && x.ItemCode == itemCode);

                                if (storeItem != null)
                                {
                                    if (i.IssuedQuantity > storeItem.Balance)
                                    {
                                        i.IssuedQuantity = storeItem.Balance;
                                        storeItem.Balance = 0;
                                               
                                    }
                                    else if (i.IssuedQuantity <= storeItem.Balance)
                                    {
                                        storeItem.Balance = storeItem.Balance - i.IssuedQuantity;
                                    }

                                    i.Total = i.IssuedQuantity * i.UnitPrice;
                                    db.Entry(i).State = EntityState.Modified;
                                    db.SaveChanges();

                                    db.Entry(storeItem).State = EntityState.Modified;
                                    db.SaveChanges();
                                    #region update bin card

                                    #region bin card line

                                    BinCardLine binCardLine = new BinCardLine
                                    {
                                        StoreItemAssignment = storeItem,
                                        ReferenceDoc = "Production Issue " + record.ProductionIssueId.ToString(),
                                        Reference = record.Remark,
                                        Date = record.IssueDate,
                                        IssuedQTY = i.IssuedQuantity,
                                        IssuedUPrice = i.UnitPrice,
                                        IssuedTPrice = i.Total,
                                        BalanceQTY = storeItem.Balance,
                                        BalanceUPrice = i.UnitPrice,
                                        BalanceTPrice = i.UnitPrice * storeItem.Balance,
                                        //IssuedTo = record.OrgStructureId.ToString()

                                    };
                                    db.BinCardLines.Add(binCardLine);
                                    db.SaveChanges();

                                    #endregion

                                    #endregion
                                }

                                if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerBranch)
                                {
                                    int storeBranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(storecode);
                                    var branchItem = db.BranchItemAssignments.Where(x => x.BranchId == storeBranchId)
                                        .FirstOrDefault(x => x.ItemCode == i.ItemCode);
                                    if (branchItem != null)
                                    {
                                        branchItem.Balance = branchItem.Balance - i.IssuedQuantity;
                                        db.Entry(branchItem).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }

                                if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerCompany)
                                {
                                    var stockItem = db.InventoryItems.Find(itemCode);
                                    if (stockItem != null)
                                    {
                                        stockItem.Balance = stockItem.Balance - i.IssuedQuantity;
                                        db.Entry(stockItem).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }

                                #endregion
                            }
                        }
                        #endregion
                    }
                    if (issueValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    var entity = new ProductionIssueValidation
                    {
                        ProductionIssueId = id,
                        Status = issueValidation.Status,
                        Remark = issueValidation.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = issueValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = issueValidation.ModifiedBy
                    };

                    db.ProductionIssueValidations.Add(entity);
                    db.SaveChanges();
                    issueValidation.ProductionIssueValidationId = entity.ProductionIssueValidationId;
                }

            }

            return Json(new[] { issueValidation }.ToDataSourceResult(request, ModelState));

        }

        //[AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IssueValidations_Update([DataSourceRequest]DataSourceRequest request, ProductionIssueValidation issueValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionIssueValidation
                {
                    ProductionIssueValidationId = issueValidation.ProductionIssueValidationId,
                    ProductionIssueId = issueValidation.ProductionIssueId,
                    Status = issueValidation.Status,
                    Remark = issueValidation.Remark,
                };

                db.ProductionIssueValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { issueValidation }.ToDataSourceResult(request, ModelState));
        }

        //[AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IssueValidations_Destroy([DataSourceRequest]DataSourceRequest request, ProductionIssueValidation issueValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionIssueValidation
                {
                    ProductionIssueValidationId = issueValidation.ProductionIssueValidationId,
                    ProductionIssueId = issueValidation.ProductionIssueId,
                    Status = issueValidation.Status,
                    Remark = issueValidation.Remark,
                };

                db.ProductionIssueValidations.Attach(entity);
                db.ProductionIssueValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { issueValidation }.ToDataSourceResult(request, ModelState));
        }

       

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
