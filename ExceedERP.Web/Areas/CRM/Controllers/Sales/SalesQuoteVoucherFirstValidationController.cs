using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.CRM.Models;
using ExceedERP.Web.CRM;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.SalesQuoteFirstApprover)]
    public class SalesQuoteVoucherFirstValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Validations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<SalesQuoteVoucherFirstValidation> validations = db.SalesQuoteVoucherFirstValidation.Where(v => v.SalesQuoteVoucherId == id);
            DataSourceResult result = validations.ToDataSourceResult(request, validation => new
            {
                validation.SalesQuoteVoucherFirstValidationId,
                validation.SalesQuoteVoucherId,

                validation.OrgBranchName,
                validation.DateCreated,
                validation.LastModified,
                validation.CreatedBy,
                validation.ModifiedBy,

                validation.Status,
                validation.Remark
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validation_Create([DataSourceRequest]DataSourceRequest request, SalesQuoteVoucherFirstValidation validation, int id)
        {
            var record = db.SalesQuoteVouchers.Find(id);

            #region Checking the  record

            if (record != null)
            {
                if (record.IsMiddleOnlineApproved)
                {
                    string message = "You cannot manipulate this record,it  is already approved";
                    ModelState.AddModelError("approved", message);
                }
                if (!record.IsMiddleSendForApproval)
                {
                    ModelState.AddModelError("notSentForApproval", "Record not sent for Approval. Transaction canceled.");
                }
            }
            else
            {
                ModelState.AddModelError("Not Found", "Quote record not found");
            }
            #endregion

            if (ModelState.IsValid)
            {
                bool quoteVoucherAndLineItemUpdateSuccessful = false;
                if (record != null)
                {
                    List<LineItem> lineItemList = db.LineItems.ToList().Where(Item => Item.Voucher == record.SalesQuoteVoucherID.ToString() && Item.VoucherDefinitionID == "0" && (Item.Status == CRMConstants.Pending || Item.Status == CRMConstants.Rejected)).ToList();

                    if (validation.Status == ApprovalStatuses.Approve)
                    {
                        #region update
                        // for 
                        if (record.Source == SalesQuoteSource.Maintenance)
                        {
                            record.LastObjectState = CRMVoucherStatusConstatns.FirstApproved;
                            record.IsMiddleOnlineApproved = true;
                            record.MiddleOnlineApprovedBy = User.Identity.Name;
                            record.MiddleOnlineApprovedTime = DateTime.Now;
                        }
                        else
                        {
                            // for manual sales quotes there is only 2 step validation (the second approval is need - thats why both flags are
                            // being modified in the first approval)
                            record.LastObjectState = CRMVoucherStatusConstatns.FirstApproved;
                            record.IsMiddleOnlineApproved = true;
                            record.IsSecondOnlineApproved = true; // important
                            record.MiddleOnlineApprovedBy = User.Identity.Name;
                            record.MiddleOnlineApprovedTime = DateTime.Now;
                        }
                        #endregion
                       
                        #region update line items status 
                        foreach (LineItem item in lineItemList)
                        {
                            item.Status = CRMConstants.Approved;
                            db.LineItems.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                            //db.SaveChanges();
                        }
                        #endregion
                    }
                    else if (validation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsMiddleSendForApproval = false;
                        record.LastObjectState = CRMVoucherStatusConstatns.FirstRejected;

                        #region update line items status 
                        foreach (LineItem item in lineItemList)
                        {
                            item.Status = CRMConstants.Rejected;
                            db.LineItems.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                            //db.SaveChanges();
                        }
                        #endregion
                    }
                    db.SalesQuoteVouchers.Attach(record);
                    db.Entry(record).State = EntityState.Modified;

                    // save changes
                    // Quote voucher changes
                    // Quote voucher line item changes
                    var result = db.SaveChanges();
                    if (result > 0)
                    {
                        quoteVoucherAndLineItemUpdateSuccessful = true;
                    }
                }

                if (quoteVoucherAndLineItemUpdateSuccessful)
                {
                    var entity = new SalesQuoteVoucherFirstValidation
                    {
                        //ReceivableInvoiceId = validation.ReceivableInvoiceId,
                        SalesQuoteVoucherId = id,
                        Status = validation.Status,
                        Remark = validation.Remark,

                        OrgBranchName = validation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = validation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = validation.ModifiedBy,
                    };

                    db.SalesQuoteVoucherFirstValidation.Add(entity);
                    db.SaveChanges();

                    validation.SalesQuoteVoucherId = entity.SalesQuoteVoucherId;
                }
            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validation_Update([DataSourceRequest]DataSourceRequest request, SalesQuoteVoucherFirstValidation validation)
        {
            if (ModelState.IsValid)
            {
                var entity = new SalesQuoteVoucherFirstValidation
                {
                    SalesQuoteVoucherFirstValidationId = validation.SalesQuoteVoucherFirstValidationId,
                    SalesQuoteVoucherId = validation.SalesQuoteVoucherId,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = validation.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = validation.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.SalesQuoteVoucherFirstValidation.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validation_Destroy([DataSourceRequest]DataSourceRequest request, SalesQuoteVoucherFirstValidation validation)
        {
            if (ModelState.IsValid)
            {
                var entity = new SalesQuoteVoucherFirstValidation
                {
                    SalesQuoteVoucherFirstValidationId = validation.SalesQuoteVoucherFirstValidationId,
                    SalesQuoteVoucherId = validation.SalesQuoteVoucherId,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = validation.DateCreated,
                    LastModified = validation.LastModified,
                    CreatedBy = validation.CreatedBy,
                    ModifiedBy = validation.ModifiedBy,
                };

                db.SalesQuoteVoucherFirstValidation.Attach(entity);
                db.SalesQuoteVoucherFirstValidation.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        private bool UpdateChangesOnQuoteApprove(SalesQuoteVoucher salesQuoteVoucher)
        {


            bool transactionCompletedSuccessfully = false;
            decimal Grandtot = 0;

            SalesQuoteVoucher sqv = db.SalesQuoteVouchers.Where(sq => sq.SalesQuoteVoucherID.ToString() == salesQuoteVoucher.SalesQuoteVoucherID.ToString()).SingleOrDefault();
            List<LineItem> items = db.LineItems.Where(l => l.Voucher == salesQuoteVoucher.SalesQuoteVoucherID.ToString() && l.VoucherDefinitionID == "0").ToList();
            SalesQuoteVoucher saleforwarded = db.SalesQuoteVouchers.Where(l => l.SalesQuoteVoucherID == salesQuoteVoucher.SalesQuoteVoucherID).FirstOrDefault();

            foreach (var item in items)
            {
                Grandtot = Grandtot + item.TotalAmount;
            }
            if (Grandtot < 1)
            {
                ModelState.AddModelError("DuplicateIndexEntry", " Grand Total  of Referenced Sales Quote Voucher is 0 !!!");

            }
            else if (saleforwarded.LastObjectState == CRMConstants.Forwarded)
            {
                ModelState.AddModelError("DuplicateForwarded", " Quote Voucher is Already Forwarded !!!");

            }
            else
            {
                int referedVoucherID;
                LineItem lineItem;
                //VoucherTerm _voucherTerm;
                //VoucherExtensionTransaction _voucherExtensionTransaction;
                if (sqv != null)
                {
                    var entity = new SalesOrderVoucher
                    {
                        OrganizationCustomerId = sqv.OrganizationCustomerId,
                        Source = SalesOrderSource.SalesQuote,
                        References = salesQuoteVoucher.SalesQuoteVoucherID.ToString(),
                        IssuedDate = sqv.IssuedDate,
                        IsIssued = sqv.IsIssued,
                        BranchId = sqv.BranchId,
                        CRMPeriodId = sqv.CRMPeriodId,
                        Date = sqv.Date,
                        IsVoid = sqv.IsVoid,
                        GrandTotal = Grandtot,
                        Period = sqv.Period,

                        LastObjectState = CRMConstants.UnForwarded,
                        PaymentMethod = sqv.PaymentMethod,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        Remark = sqv.Remark
                    };

                    db.SalesOrderVouchers.Add(entity);
                    db.SaveChanges();
                    referedVoucherID = entity.SalesOrderVoucherID;

                    List<LineItem> lineItemList = db.LineItems.ToList().Where(Item => Item.Voucher == salesQuoteVoucher.SalesQuoteVoucherID.ToString() && Item.VoucherDefinitionID == "0" && (Item.Status == CRMConstants.Pending || Item.Status == CRMConstants.Rejected)).ToList();
                    foreach (LineItem Item in lineItemList)
                    {
                        lineItem = new LineItem();
                        //This may be amended to be the uptodated value
                        lineItem.VoucherDefinitionID = SystemConstatnt.SALES_ORDER_VOUCHER;
                        lineItem.References = Item.LineItemID.ToString();
                        lineItem.CalculatedCost = Item.CalculatedCost;
                        lineItem.PriceTag = Item.PriceTag;
                        lineItem.Quantity = Item.Quantity;
                        lineItem.Remark = Item.Remark;
                        lineItem.Variety = Item.Variety;
                        lineItem.Class = Item.Class;
                        //This may be amended to be the uptodated value
                        lineItem.StockBalance = Item.StockBalance;
                        //This may be attached with StoreTransaction table
                        lineItem.StoreID = Item.StoreID;
                        lineItem.Discount = Item.Discount;
                        lineItem.Tax = Item.Tax;
                        lineItem.TaxAmount = Item.TaxAmount;
                        lineItem.ThingID = Item.ThingID;
                        lineItem.TotalAmount = Item.TotalAmount;
                        lineItem.UnitAmount = Item.UnitAmount;
                        lineItem.Vat = Item.Vat;
                        lineItem.WithholdingType = Item.WithholdingType;
                        lineItem.WithholdingAmount = Item.WithholdingAmount;
                        lineItem.WithholdTax = Item.WithholdTax;
                        lineItem.WithholdTaxAmount = Item.WithholdTaxAmount;
                        lineItem.Discount = Item.Discount;
                        lineItem.Voucher = referedVoucherID.ToString();
                        lineItem.VoucherDefinitionID = "1";
                        lineItem.Vat = Item.Vat;
                        lineItem.Status = CRMConstants.Pending;
                        lineItem.CreatedBy = User.Identity.Name;
                        lineItem.DateCreated = DateTime.Now;

                        db.LineItems.Add(lineItem);
                        db.SaveChanges();
                    }
                    foreach (LineItem Item in lineItemList)
                    {
                        Item.Status = CRMConstants.Forwarded;
                        db.LineItems.Attach(Item);
                        db.Entry(entity).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    //List<VoucherTerm> voucherTermList = db.VoucherTerms.Where(vt => vt.Voucher == salesQuoteVoucher.SalesQuoteVoucherID.ToString()).ToList();
                    //foreach (VoucherTerm voucherTerm in voucherTermList)
                    //{
                    //    _voucherTerm = new VoucherTerm();
                    //    _voucherTerm.Remark = voucherTerm.Remark;
                    //    _voucherTerm.Term = voucherTerm.Term;
                    //    _voucherTerm.Value = voucherTerm.Value;
                    //    _voucherTerm.Voucher = referedVoucherID.ToString();
                    //    db.VoucherTerms.Add(_voucherTerm);
                    //    db.SaveChanges();
                    //}
                    //List<VoucherExtensionTransaction> voucherExtensionTransactionList = db.VoucherExtensionTransactions.Where(ve => ve.Voucher == salesQuoteVoucher.SalesQuoteVoucherID.ToString()).ToList();
                    //foreach (VoucherExtensionTransaction voucherExtensionTrxn in voucherExtensionTransactionList)
                    //{
                    //    _voucherExtensionTransaction = new VoucherExtensionTransaction();
                    //    _voucherExtensionTransaction.Number = voucherExtensionTrxn.Number;
                    //    _voucherExtensionTransaction.Remark = voucherExtensionTrxn.Remark;
                    //    _voucherExtensionTransaction.Voucher = referedVoucherID.ToString();
                    //    _voucherExtensionTransaction.VoucherExtension = voucherExtensionTrxn.VoucherExtension;
                    //    db.VoucherExtensionTransactions.Add(_voucherExtensionTransaction);
                    //    db.SaveChanges();
                    //}
                }

                ExceedDbContext dbb = new ExceedDbContext();
                SalesQuoteVoucher sqvup = dbb.SalesQuoteVouchers.Where(so => so.SalesQuoteVoucherID == salesQuoteVoucher.SalesQuoteVoucherID).FirstOrDefault();
                sqvup.OnlineApprovedBy = User.Identity.Name;
                sqvup.SendForApprovalBy = User.Identity.Name;
                sqvup.OnlineApprovedTime = DateTime.Now;
                sqvup.IsOnlineApproved = true;
                sqvup.LastObjectState = CRMConstants.Forwarded;
                dbb.SalesQuoteVouchers.Attach(sqvup);
                dbb.Entry(sqvup).State = EntityState.Modified;
                var v = dbb.SaveChanges();
                if (v == 1)
                {
                    transactionCompletedSuccessfully = true;
                }
            }


            return transactionCompletedSuccessfully;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}