using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.PurchaseRequisitions
{

    public class PurchaseReturnController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var categories = db.ItemCategorys;
            var items = db.InventoryItems;
            var branches = db.Branch;
            var tax = db.GLTaxRates;
            var stores = db.InventoryStores;
            var receivetype = db.ReceiveTypes;

            ViewData["stores"] = stores;
            ViewData["tax"] = tax;
            ViewData["ReeceiveType"] = receivetype;
            ViewData["Branches"] = branches;
            ViewData["Items"] = items;
            ViewData["Category"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult PurchaseReturns_Read([DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<PurchaseReturn> purchasereturns = db.PurchaseReturns;
            var result = purchasereturns.ToDataSourceResult(request, purchaseReturn => new
                {
                    purchaseReturn.PurchaseReturnID,
                    purchaseReturn.Reference,
                    purchaseReturn.StoreCode,
                    purchaseReturn.Reason,
                    purchaseReturn.GoodReceiveId,
                    purchaseReturn.Status,
                    purchaseReturn.Remark,
                    purchaseReturn.IsVoid,
                    purchaseReturn.VoidBy,
                    purchaseReturn.VoidTime,
                    purchaseReturn.IsOnlineApproved,
                    purchaseReturn.OnlineApprovedBy,
                    purchaseReturn.OnlineApprovedTime,

                    purchaseReturn.IsSendForApproval,
                    purchaseReturn.SendForApprovalBy,
                    purchaseReturn.SendForApprovalTime,

                    purchaseReturn.IsOnlineTransferred,
                    purchaseReturn.IsOnlineTransferredBy,
                    purchaseReturn.CreatedBy,
                    purchaseReturn.DateCreated,
                    purchaseReturn.OnlineTransferredTime
                });

            return Json(result);
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseReturns_Create([DataSourceRequest] DataSourceRequest request, PurchaseReturn purchaseReturn)
        {
            var record = db.GoodReceives.FirstOrDefault(p => p.GoodReceiveId == purchaseReturn.GoodReceiveId);
            ModelState.Remove("StoreCode");

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    var entity = new PurchaseReturn
                    {
                        Reference = record.Reference,
                        StoreCode = record.StoreCode,
                        Reason = purchaseReturn.Reason,
                        GoodReceiveId = purchaseReturn.GoodReceiveId,
                        Status = Enum.GetName(typeof(OperationStatuses), 5),
                        Remark = purchaseReturn.Remark,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now
                    };

                    db.PurchaseReturns.Add(entity);
                    db.SaveChanges();
                    purchaseReturn.PurchaseReturnID = entity.PurchaseReturnID;
                    GoodReceiveItems(entity);
                    // UpdateDistribution(entity);
                }
            }
            return Json(new[] { purchaseReturn }.ToDataSourceResult(request, ModelState));
        }

        private void GoodReceiveItems(PurchaseReturn record)
        {
            var items = db.GoodReceiveItems.Where(p => p.GoodReceiveId == record.GoodReceiveId);
            foreach (var item in items)
            {
                var purchaseReturnItems = new PurchaseReturnItems
                {
                    ItemCategoryCode = item.ItemCategoryCode,
                    ItemCode = item.ItemCode,
                    PurchaseReturnID = record.PurchaseReturnID,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    SubTotal = item.SubTotal,
                    GLTaxRateID = item.GLTaxRateID,
                    TaxAmount = item.TaxAmount,
                    TotalAmount = item.Total,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                };

                db.PurchaseReturnItemss.Add(purchaseReturnItems);
            }
            db.SaveChanges();
        }



        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseReturns_Update([DataSourceRequest] DataSourceRequest request, PurchaseReturn purchaseReturn)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseReturn
                    {
                        PurchaseReturnID = purchaseReturn.PurchaseReturnID,
                        Reference = purchaseReturn.Reference,
                        StoreCode = purchaseReturn.StoreCode,
                        Reason = purchaseReturn.Reason,
                        GoodReceiveId = purchaseReturn.GoodReceiveId,
                        Status = purchaseReturn.Status,
                        Remark = purchaseReturn.Remark,
                        IsVoid = purchaseReturn.IsVoid,
                        VoidBy = purchaseReturn.VoidBy,
                        VoidTime = purchaseReturn.VoidTime,
                        IsOnlineApproved = purchaseReturn.IsOnlineApproved,
                        OnlineApprovedBy = purchaseReturn.OnlineApprovedBy,
                        OnlineApprovedTime = purchaseReturn.OnlineApprovedTime,
                        IsOnlineTransferred = purchaseReturn.IsOnlineTransferred,
                        IsOnlineTransferredBy = purchaseReturn.IsOnlineTransferredBy,
                        OnlineTransferredTime = purchaseReturn.OnlineTransferredTime,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Now.Date
                    };

                db.PurchaseReturns.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { purchaseReturn }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseReturns_Destroy([DataSourceRequest] DataSourceRequest request, PurchaseReturn purchaseReturn)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseReturn
                    {
                        PurchaseReturnID = purchaseReturn.PurchaseReturnID,
                        Reference = purchaseReturn.Reference,
                        StoreCode = purchaseReturn.StoreCode,
                        Reason = purchaseReturn.Reason,
                        GoodReceiveId = purchaseReturn.GoodReceiveId,
                        Status = purchaseReturn.Status,
                        Remark = purchaseReturn.Remark,
                        IsVoid = purchaseReturn.IsVoid,
                        VoidBy = purchaseReturn.VoidBy,
                        VoidTime = purchaseReturn.VoidTime,
                        IsOnlineApproved = purchaseReturn.IsOnlineApproved,
                        OnlineApprovedBy = purchaseReturn.OnlineApprovedBy,
                        OnlineApprovedTime = purchaseReturn.OnlineApprovedTime,
                        IsOnlineTransferred = purchaseReturn.IsOnlineTransferred,
                        IsOnlineTransferredBy = purchaseReturn.IsOnlineTransferredBy,
                        OnlineTransferredTime = purchaseReturn.OnlineTransferredTime
                    };

                db.PurchaseReturns.Attach(entity);
                db.PurchaseReturns.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted purchase return number " + purchaseReturn.PurchaseReturnID + " " + purchaseReturn.Reference);
                #endregion
            }

            return Json(new[] { purchaseReturn }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        public ActionResult PurchaseReturn_OnlineTransfer(string id)
        {
            int myid = int.Parse(id);
            PurchaseReturn record = db.PurchaseReturns.Find(myid);
            if (!record.IsOnlineApproved)
            {
                //ViewBag.Note = "The record cannot be transffred before validation.";
                //return View("Error");
            }
            else
            {
                if (!record.IsOnlineTransferred)
                {
                    #region check if debit and credit are equal
                    decimal TDebit = 0;
                    decimal TCredit = 0;
                    var distribution = record.PurchaseReturnDistribution.ToList();
                    if (distribution != null)
                    {
                        TDebit = distribution.Sum(x => x.Debit);
                        TCredit = distribution.Sum(x => x.Credit);
                    }
                    if (TDebit != TCredit)
                    {
                        //ViewBag.Note = "Total debit and total credit is not balanced.";
                        // return View("Error");
                    }
                    if (TDebit == 0)
                    {
                        //ViewBag.Note = "Total debit and total credit is zero.";
                        //return View("Error");
                    }
                    #endregion
                    #region send to GL
                    string glperiod = "";
                    string glyear = "";
                    #region get period
                    var period = db.GLPeriods.Where(x => x.DateFrom <= record.GoodReceive.ReceiveDate)
                                           .Where(x => x.DateTo >= record.GoodReceive.ReceiveDate).ToList();
                    if (period != null)
                    {

                        if (period.Count() == 1)
                        {
                            var myPeriod = period.SingleOrDefault();
                            glperiod = myPeriod.Name;
                            glyear = myPeriod.GlFiscalYear.Name.ToString();
                            if (myPeriod.Status == FiscalYearStatus.Close)
                            {
                                //ViewBag.Note = "The period " + glperiod + " is closed,the record cannot be transffred to closed period.";
                                //return View("Error");
                            }
                        }
                        else if (!period.Any())
                        {
                            //ViewBag.Note = "There are no periods defined that contain the Invoice date.";
                            //return View("Error");
                        }
                        else if (period.Any())
                        {
                            //ViewBag.Note = "There are multiple periods defined that contain the Invoice date.";
                            //return View("Error");
                        }
                        else
                        {

                        }
                    }

                    #endregion
                    if (record.PurchaseReturnDistribution.Any())
                    {
                        GlRecordJournal journal = new GlRecordJournal();
                        journal.DateCreated = DateTime.Now;
                        journal.CreatedBy = User.Identity.Name;
                        journal.JournalName = "IssueFuel";
                        journal.Description = "IssueFuel";
                        journal.FiscalYear = glyear;
                        journal.PeriodName = glperiod;
                        journal.CategoryName = "IssueFuel";
                        journal.SourcesName = "IF";
                        journal.JournalNumber = "IssueFuel." + record.PurchaseReturnID.ToString();
                        journal.EffectiveDate = record.GoodReceive.ReceiveDate;
                        journal.Status = "Payment";
                        journal.IsOnlineApproved = true;
                        journal.OnlineApprovedBy = User.Identity.Name;
                        journal.OnlineApprovedTime = DateTime.Now;
                        db.GLRecordJournals.Add(journal);
                        db.SaveChanges();
                        GLRecordJournalDetail journalDetail;
                        foreach (var i in record.PurchaseReturnDistribution)
                        {
                            #region debit and credit
                            journalDetail = new GLRecordJournalDetail();
                            journalDetail.DateCreated = DateTime.Now;
                            journalDetail.CreatedBy = User.Identity.Name;
                            journalDetail.GLRecordJournal = journal;
                            journalDetail.AccountCode = i.AccountCode;
                            journalDetail.Debit = i.Debit;
                            journalDetail.Credit = i.Credit;
                            db.GLRecordJournalDetails.Add(journalDetail);
                            db.SaveChanges();
                            #endregion
                        }
                    }
                    #endregion
                    record.IsOnlineTransferred = true;
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    //ViewBag.Note = "You cannot transfer this transaction, it is already transffered.";
                    //return View("Error");
                }
            }
            return RedirectToAction("Index");
        }


        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        public ActionResult PurchaseReturn_Approval(int id)
        {
            var record = db.PurchaseReturns.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.PurchaseReturnItems.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.PurchaseReturns.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        public ActionResult PurchaseReturn_Approval1([DataSourceRequest] DataSourceRequest request, int id)
        {
            var purchaseReturn = db.PurchaseReturns.Find(id);
            if (purchaseReturn != null && !purchaseReturn.IsOnlineApproved)
            {
                purchaseReturn.Status = Enum.GetName(typeof(OperationStatuses), 4);
                db.PurchaseReturns.Attach(purchaseReturn);
                db.Entry(purchaseReturn).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}