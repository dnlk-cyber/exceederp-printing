using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.CRM;
using ExceedERP.Web.Areas.CRM.Models;
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
    [AuthorizeRoles(CRMRoles.SalesOrderApprover, CRMRoles.SalesOrderUser)]
    public class SalesOrderVoucherValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Validations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<SalesOrderVoucherValidation> validations = db.SalesOrderVoucherValidation.Where(v => v.SalesOrderVoucherId == id);
            DataSourceResult result = validations.ToDataSourceResult(request, validation => new
            {
                validation.SalesOrderVoucherValidationId,
                validation.SalesOrderVoucherId,

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
        public ActionResult Validation_Create([DataSourceRequest]DataSourceRequest request, SalesOrderVoucherValidation validation, int id)
        {
            var record = db.SalesOrderVouchers.Find(id);

            #region Checking the  record

            if (record != null)
            {
                if (record.IsOnlineApproved)
                {
                    string message = "You cannot manipulate this record,it  is already approved";
                    ModelState.AddModelError("approved", message);
                }
                if (!record.IsSendForApproval)
                {
                    string message = "This record is not sent for approval. Operation Canceled";
                    ModelState.AddModelError("Not_Sent", message);
                }

                if (validation.Status == ApprovalStatuses.Reject)
                {
                    record.IsSendForApproval = false;
                    record.Status = "Rejected";
                }

            }
            else
            {
                ModelState.AddModelError("Not Found", "Sales order voucher record not found");
            }
            #endregion

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (validation.Status == ApprovalStatuses.Approve)
                    {
                        //var success = UpdateChangesOnSalesOrderApprove(record);
                        //if (success)
                        //{
                        #region update
                        record.LastObjectState = CRMVoucherStatusConstatns.Approved;
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        #endregion
                        //}
                        //else
                        //{
                        //    ModelState.AddModelError("", "Something Went Wrong in Processing the Transaction!");
                        //}
                    }
                    if (validation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        record.LastObjectState = CRMConstants.Rejected;

                    }
                    db.SalesOrderVouchers.Attach(record);
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                }

                var entity = new SalesOrderVoucherValidation
                {
                    //ReceivableInvoiceId = validation.ReceivableInvoiceId,
                    SalesOrderVoucherId = id,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = validation.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = validation.ModifiedBy,
                };

                db.SalesOrderVoucherValidation.Add(entity);
                db.SaveChanges();

                validation.SalesOrderVoucherValidationId = entity.SalesOrderVoucherValidationId;
            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validation_Update([DataSourceRequest]DataSourceRequest request, SalesOrderVoucherValidation validation)
        {
            if (ModelState.IsValid)
            {
                var entity = new SalesOrderVoucherValidation
                {
                    SalesOrderVoucherValidationId = validation.SalesOrderVoucherValidationId,
                    SalesOrderVoucherId = validation.SalesOrderVoucherId,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = validation.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = validation.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.SalesOrderVoucherValidation.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validation_Destroy([DataSourceRequest]DataSourceRequest request, SalesOrderVoucherValidation validation)
        {
            if (ModelState.IsValid)
            {
                var entity = new SalesOrderVoucherValidation
                {
                    SalesOrderVoucherValidationId = validation.SalesOrderVoucherValidationId,
                    SalesOrderVoucherId = validation.SalesOrderVoucherId,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = validation.DateCreated,
                    LastModified = validation.LastModified,
                    CreatedBy = validation.CreatedBy,
                    ModifiedBy = validation.ModifiedBy,
                };

                db.SalesOrderVoucherValidation.Attach(entity);
                db.SalesOrderVoucherValidation.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        private bool UpdateChangesOnSalesOrderApprove(SalesOrderVoucher salesOrderVoucher)
        {


            bool transactionCompletedSuccessfully = false;

            //SalesOrderVoucher sov = db.SalesOrderVouchers.Where(so => so.SalesOrderVoucherID == salesOrderVoucherID).FirstOrDefault();
            //SalesOrderVoucher saleforwarded = db.SalesOrderVouchers.Where(l => l.SalesOrderVoucherID.ToString() == salesOrderVoucherID).FirstOrDefault();

            decimal GrandTotal = db.LineItems.Where(t => t.Voucher == salesOrderVoucher.SalesOrderVoucherID.ToString() && t.VoucherDefinitionID == "1" && (t.Status == CRMConstants.Pending || t.Status == CRMConstants.Rejected)).Sum(x => x.TotalAmount);
            if (GrandTotal < 1)
            {
                ModelState.AddModelError("DuplicateIndexEntry", " Grand Total balance Must not be 0 !!!");
            }
            else if (salesOrderVoucher.LastObjectState == CRMConstants.Forwarded)
            {
                ModelState.AddModelError("DuplicateForwarded", " Order Voucher is Already Forwarded !!!");

            }
            else
            {
                //db = new ExceedDbContext();
                //int referedVoucherID;
                string Receive = "CASH";
                //LineItem lineItem;
                //VoucherTerm _voucherTerm;
                //VoucherExtensionTransaction _voucherExtensionTransaction;
                var PaymentMethod = db.PaymentMethods.FirstOrDefault();
                if (PaymentMethod != null)
                {

                    Receive = PaymentMethod.PaymentMethodID.ToString();
                }
                if (salesOrderVoucher != null)
                {
                    var entity = new CashSalesInvoice
                    {
                        OrganizationCustomerId = salesOrderVoucher.OrganizationCustomerId,
                        References = salesOrderVoucher.SalesOrderVoucherID.ToString(),
                        IssuedDate = salesOrderVoucher.IssuedDate,
                        BranchId = salesOrderVoucher.BranchId,
                        CRMPeriodId = salesOrderVoucher.CRMPeriodId,
                        IsIssued = salesOrderVoucher.IsIssued,
                        Date = salesOrderVoucher.Date,
                        IsVoid = salesOrderVoucher.IsVoid,
                        GrandTotal = GrandTotal,
                        Period = salesOrderVoucher.Period,
                        Source = SalesInvoiceSource.SalesOrder,
                        LastObjectState = CRMConstants.UnForwarded,
                        PaymentMethod = salesOrderVoucher.PaymentMethod,
                        RecieveMethod = Receive,
                        Remark = salesOrderVoucher.Remark,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                    };

                    db.CashSalesInvoices.Add(entity);
                    db.SaveChanges();
                    //referedVoucherID = entity.CashSalesInvoiceID;

                    List<LineItem> lineItemList = db.LineItems.ToList().Where(Item => Item.Voucher == salesOrderVoucher.SalesOrderVoucherID.ToString() && Item.VoucherDefinitionID == "1" && (Item.Status == CRMConstants.Pending || Item.Status == CRMConstants.Rejected)).ToList();
                    foreach (LineItem Item in lineItemList)
                    {
                        var lineItem = new LineItem
                        {
                            //This may be amended to be the uptodated value
                            CalculatedCost = Item.CalculatedCost,
                            PriceTag = Item.PriceTag,
                            Quantity = Item.Quantity,
                            Remark = Item.Remark,
                            //This may be amended to be the uptodated value
                            StockBalance = Item.StockBalance,
                            Variety = Item.Variety,
                            Class = Item.Class,
                            Discount = Item.Discount,
                            //This may be attached with StoreTransaction table
                            StoreID = Item.StoreID,
                            Tax = Item.Tax,
                            TaxAmount = Item.TaxAmount,
                            ThingID = Item.ThingID,
                            TotalAmount = Item.TotalAmount,
                            UnitAmount = Item.UnitAmount,
                            Voucher = entity.CashSalesInvoiceID.ToString(),
                            VoucherDefinitionID = "2",
                            Vat = Item.Vat,
                            References = salesOrderVoucher.SalesOrderVoucherID.ToString(),
                            WithholdingType = Item.WithholdingType,
                            WithholdingAmount = Item.WithholdingAmount,
                            WithholdTax = Item.WithholdTax,
                            WithholdTaxAmount = Item.WithholdTaxAmount,
                            Status = CRMConstants.Pending,
                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Now
                        };

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
                    //List<VoucherTerm> voucherTermList = db.VoucherTerms.Where(vt => vt.Voucher == salesOrderVoucher..References).ToList();
                    //foreach (VoucherTerm voucherTerm in voucherTermList)
                    //{
                    //    var _voucherTerm = new VoucherTerm
                    //    {
                    //        Remark = voucherTerm.Remark,
                    //        Term = voucherTerm.Term,
                    //        Value = voucherTerm.Value,
                    //        Voucher = entity.CashSalesInvoiceID.ToString()
                    //    };
                    //    db.VoucherTerms.Add(_voucherTerm);
                    //    db.SaveChanges();
                    //}
                    //List<VoucherExtensionTransaction> voucherExtensionTransactionList = db.VoucherExtensionTransactions.Where(ve => ve.Voucher == sov.References).ToList();
                    //foreach (VoucherExtensionTransaction voucherExtensionTrxn in voucherExtensionTransactionList)
                    //{
                    //    _voucherExtensionTransaction = new VoucherExtensionTransaction
                    //    {
                    //        Number = voucherExtensionTrxn.Number,
                    //        Remark = voucherExtensionTrxn.Remark,
                    //        Voucher = referedVoucherID.ToString(),
                    //        VoucherExtension = voucherExtensionTrxn.VoucherExtension
                    //    };
                    //    db.VoucherExtensionTransactions.Add(_voucherExtensionTransaction);
                    //    db.SaveChanges();
                    //}


                    //ExceedDbContext dbb = new ExceedDbContext();
                    //SalesOrderVoucher sovup = dbb.SalesOrderVouchers.Where(so => so.SalesOrderVoucherID.ToString() == salesOrderVoucherID).FirstOrDefault();
                    salesOrderVoucher.LastObjectState = CRMConstants.Forwarded;
                    salesOrderVoucher.SendForApprovalBy = User.Identity.Name;
                    salesOrderVoucher.OnlineApprovedBy = User.Identity.Name;
                    salesOrderVoucher.OnlineApprovedTime = DateTime.Now;
                    salesOrderVoucher.IsOnlineApproved = true;
                    db.SalesOrderVouchers.Attach(salesOrderVoucher);
                    db.Entry(salesOrderVoucher).State = EntityState.Modified;
                    var success = db.SaveChanges();
                    if (success == 1)
                    {
                        transactionCompletedSuccessfully = true;
                        if (entity.PaymentMethod == CRMConstants.CreditPaymentMethod)
                        {
                            new CRMPaymentDistibutionHelper().DistributeCreditInvoice(entity);
                        }
                        else if (entity.PaymentMethod == CRMConstants.CashPaymentMethod)
                        {
                            new CRMPaymentDistibutionHelper().UpdateCashSalesInvoiceDistribution(entity);
                        }
                    }
                }

            }
            //lineItems = db.LineItems.ToList();
            //List<SalesOrderVoucher> voulist = new List<SalesOrderVoucher>();
            //List<SalesOrderVoucher> quoteVouchers = new List<SalesOrderVoucher>();
            //quoteVouchers = dbbb.SalesOrderVouchers.ToList();
            //foreach (var vou in quoteVouchers)
            //{
            //    SalesQuoteVoucherView vsqvView = new SalesQuoteVoucherView();
            //    decimal tottax = lineItems.Where(t => t.Voucher == vou.SalesOrderVoucherID.ToString() && t.VoucherDefinitionID == "1").Sum(x => x.TaxAmount);
            //    decimal totsub = lineItems.Where(t => t.Voucher == vou.SalesOrderVoucherID.ToString() && t.VoucherDefinitionID == "1").Sum(x => x.CalculatedCost) - lineItems.Where(t => t.Voucher == vou.SalesOrderVoucherID.ToString()).Sum(x => x.TaxAmount);
            //    decimal totgrand = lineItems.Where(t => t.Voucher == vou.SalesOrderVoucherID.ToString() && t.VoucherDefinitionID == "1").Sum(x => x.TotalAmount);
            //    voulist.Add(new SalesOrderVoucher { SalesOrderVoucherID = vou.SalesOrderVoucherID, Period = vou.Period, OrganizationCustomerId = vou.OrganizationCustomerId, Date = vou.Date, GrandTotal = totgrand, IsIssued = vou.IsIssued, IssuedDate = vou.IssuedDate, IsVoid = vou.IsVoid });
            //}



            return transactionCompletedSuccessfully;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}