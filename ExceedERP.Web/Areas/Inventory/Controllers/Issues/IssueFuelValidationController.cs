using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Hangfire;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Issues
{

    public class IssueFuelValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult IssueFuelValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<IssueValidation> issuefuelvalidations = db.IssueValidations.Where(isf => isf.IssueID == id);
            DataSourceResult result = issuefuelvalidations.ToDataSourceResult(request, issueFuelValidation => new
            {
                IssueValidationID = issueFuelValidation.IssueValidationID,
                IssueID = issueFuelValidation.IssueID,
                Status = issueFuelValidation.Status,
                Remark = issueFuelValidation.Remark,
            });

            return Json(result);
        }

        [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IssueFuelValidations_Create([DataSourceRequest]DataSourceRequest request, IssueValidation issueFuelValidation, int id)
        {
            var record = db.Issues.Find(id);
            var items = db.IssueItems.Where(x => x.IssueID == id).AsNoTracking().ToList();
            var pricesetting = db.AveragePriceSettings.FirstOrDefault();
            #region validation 

            if (!items.Any())
            {
                string message = "The Record Does not Contain Any Item";
                ModelState.AddModelError(String.Empty, message);
            }
            if (record != null)
            {
                if (record.IsOnlineApproved)
                {
                    string message = "You cannot manipulate this record,it  is already approved";
                    ModelState.AddModelError("approved", message);
                }
                {
                    if (issueFuelValidation.Status == ApprovalStatuses.Approve)
                    {
                        foreach (var i in items)
                        {

                            #region stock

                            var storeItem = db.StoreItemAssignments.Where(x => x.ItemCode == i.ItemCode).FirstOrDefault(x => x.StoreCode == record.StoreCode);
                            if (storeItem == null)
                            {
                                string message = "Item: " + i.ItemCode + " is not available in store: " + record.StoreCode;
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
                    if (issueFuelValidation.Status == ApprovalStatuses.Reject)
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
                    if (issueFuelValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.IssueID = id;
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        db.Entry(record).State = EntityState.Modified;
                        //db.SaveChanges();
                        #region update stock based on the issue
                        string storecode = record.StoreCode;
                        foreach (var i in items)
                        {
                            if (i.IssuedQuantity > 0)
                            {
                                string itemCode = i.ItemCode;
                                decimal avgPrice = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(i.ItemCode, storecode);
                                i.UnitPrice = avgPrice;
                                #region stock

                                var storeItem = db.StoreItemAssignments.FirstOrDefault(
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
                                   // db.SaveChanges();

                                    db.Entry(storeItem).State = EntityState.Modified;
                                    //db.SaveChanges();
                                    #region update bin card

                                    #region bin card line

                                    BinCardLine binCardLine = new BinCardLine
                                    {
                                        StoreItemAssignment = storeItem,
                                        ReferenceDoc = "Issue " + record.IssueID.ToString(),
                                        Reference = record.Reference + " " + record.Remark,
                                        Date = record.IssueDate,
                                        IssuedQTY = i.IssuedQuantity,
                                        IssuedUPrice = i.UnitPrice,
                                        IssuedTPrice = i.Total,
                                        BalanceQTY = storeItem.Balance,
                                        BalanceUPrice = i.UnitPrice,
                                        BalanceTPrice = i.UnitPrice * storeItem.Balance,
                                        IssuedTo = record.OrgStructureId.ToString(),
                                        //new    
                                        StoreCode = record.StoreCode,
                                        GLPeriodId = record.GLPeriodId,
                                        TransactionType = BinCardTransactionTypes.Issue,
                                        StatusType = BinCardTransactionStatus.Regular,
                                        TransactionOrder = BinCardTransactionOrder.Issue

                                    };
                                    db.BinCardLines.Add(binCardLine);
                                    //db.SaveChanges();

                                    #endregion
                                    #endregion
                                    var jobId = BackgroundJob.Enqueue(() => InventoryBalanceHelper.UpdateSigleItemBinCardTransactions(storeItem.StoreItemAssignmentID, record.IssueDate));
                                    
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
                                        //db.SaveChanges();
                                    }
                                }

                                if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerCompany)
                                {
                                    var stockItem = db.InventoryItems.Find(itemCode);
                                    if (stockItem != null)
                                    {
                                        stockItem.Balance = stockItem.Balance - i.IssuedQuantity;
                                        db.Entry(stockItem).State = EntityState.Modified;
                                       // db.SaveChanges();
                                    }
                                }

                                #endregion
                            }
                        }
                        #endregion

                    }
                    if (issueFuelValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        db.Entry(record).State = EntityState.Modified;
                       // db.SaveChanges();
                    }

                    var entity = new IssueValidation
                    {
                        IssueID = id,
                        Status = issueFuelValidation.Status,
                        Remark = issueFuelValidation.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = issueFuelValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = issueFuelValidation.ModifiedBy
                    };

                    db.IssueValidations.Add(entity);
                    db.SaveChanges();
                    issueFuelValidation.IssueValidationID = entity.IssueValidationID;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "inventory issue fuel approval", "Success", User.Identity.Name, $"approved issue {entity.IssueID} ", Modules.Inventory);
                    #endregion
                }

            }
            return Json(new[] { issueFuelValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IssueFuelValidations_Update([DataSourceRequest]DataSourceRequest request, IssueValidation issueFuelValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new IssueValidation
                {
                    IssueValidationID = issueFuelValidation.IssueValidationID,
                    IssueID = issueFuelValidation.IssueID,
                    Status = issueFuelValidation.Status,
                    Remark = issueFuelValidation.Remark,
                };

                db.IssueValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { issueFuelValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IssueFuelValidations_Destroy([DataSourceRequest]DataSourceRequest request, IssueValidation issueFuelValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new IssueValidation
                {
                    IssueValidationID = issueFuelValidation.IssueValidationID,
                    IssueID = issueFuelValidation.IssueID,
                    Status = issueFuelValidation.Status,
                    Remark = issueFuelValidation.Remark,
                };

                db.IssueValidations.Attach(entity);
                db.IssueValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { issueFuelValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
