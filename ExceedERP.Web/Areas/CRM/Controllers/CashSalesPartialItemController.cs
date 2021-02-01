﻿using System;
using System.Collections.Generic;

using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.CRM;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    public class CashSalesPartialItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CashSalesPartialItems_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            IQueryable<CashSalesPartialItem> cashsalespartialitems = db.CashSalesPartialItems.Where(x => x.CashSalesInvoiceId == Id);
            DataSourceResult result = cashsalespartialitems.ToDataSourceResult(request, cashSalesPartialItem => new
            {
                CashSalesPartialItemId = cashSalesPartialItem.CashSalesPartialItemId,
                Date = cashSalesPartialItem.Date,
                CashSalesInvoiceId = cashSalesPartialItem.CashSalesInvoiceId,
                IsReceiptPrinted = cashSalesPartialItem.IsReceiptPrinted,
                IsPaymentProcessed = cashSalesPartialItem.IsPaymentProcessed,
                PaymentStatus = cashSalesPartialItem.IsPaymentProcessed ? "Collected" : "Not Collected",
                Remark = cashSalesPartialItem.Remark,
                Status = cashSalesPartialItem.Status,
                IsVoid = cashSalesPartialItem.IsVoid,
                VoidBy = cashSalesPartialItem.VoidBy,
                VoidTime = cashSalesPartialItem.VoidTime,
                IsOnlineApproved = cashSalesPartialItem.IsOnlineApproved,
                OnlineApprovedBy = cashSalesPartialItem.OnlineApprovedBy,
                OnlineApprovedTime = cashSalesPartialItem.OnlineApprovedTime,
                IsOnlineTransferred = cashSalesPartialItem.IsOnlineTransferred,
                IsOnlineTransferredBy = cashSalesPartialItem.IsOnlineTransferredBy,
                OnlineTransferredTime = cashSalesPartialItem.OnlineTransferredTime,
                IsSendForApproval = cashSalesPartialItem.IsSendForApproval,
                OrgBranchName = cashSalesPartialItem.OrgBranchName,
                SendForApprovalBy = cashSalesPartialItem.SendForApprovalBy,
                SendForApprovalTime = cashSalesPartialItem.SendForApprovalTime,
                DateCreated = cashSalesPartialItem.DateCreated,
                LastModified = cashSalesPartialItem.LastModified,
                CreatedBy = cashSalesPartialItem.CreatedBy,
                ModifiedBy = cashSalesPartialItem.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CashSalesPartialItems_Create([DataSourceRequest]DataSourceRequest request, CashSalesPartialItem cashSalesPartialItem, int Id)
        {
            CRMSalesHelper CRMSalesHelper = new CRMSalesHelper();
            if (ModelState.IsValid)
            {
                var entity = new CashSalesPartialItem
                {
                    CashSalesInvoiceId = Id,
                    Date = DateTime.Now,
                    IsReceiptPrinted = false,
                    IsPaymentProcessed = false,
                    Remark = cashSalesPartialItem.Remark,
                    Status = cashSalesPartialItem.Status,
                    IsVoid = cashSalesPartialItem.IsVoid,
                    VoidBy = cashSalesPartialItem.VoidBy,
                    VoidTime = cashSalesPartialItem.VoidTime,
                    IsOnlineApproved = cashSalesPartialItem.IsOnlineApproved,
                    OnlineApprovedBy = cashSalesPartialItem.OnlineApprovedBy,
                    OnlineApprovedTime = cashSalesPartialItem.OnlineApprovedTime,
                    IsOnlineTransferred = cashSalesPartialItem.IsOnlineTransferred,
                    IsOnlineTransferredBy = cashSalesPartialItem.IsOnlineTransferredBy,
                    OnlineTransferredTime = cashSalesPartialItem.OnlineTransferredTime,
                    IsSendForApproval = cashSalesPartialItem.IsSendForApproval,
                    OrgBranchName = cashSalesPartialItem.OrgBranchName,
                    SendForApprovalBy = cashSalesPartialItem.SendForApprovalBy,
                    SendForApprovalTime = cashSalesPartialItem.SendForApprovalTime,
                    DateCreated = DateTime.Now,
                    LastModified = cashSalesPartialItem.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = cashSalesPartialItem.ModifiedBy
                };

                db.CashSalesPartialItems.Add(entity);
                db.SaveChanges();
                var PartialCashSalesItem = db.CashSalesPartialItems.ToList();
                var PartialCashSalesLineItem = db.CashSalesPartialLineItems.ToList();
                   
                var LineItems = db.LineItems.Where(x => x.Voucher == entity.CashSalesInvoiceId.ToString() && x.VoucherDefinitionID == "2").ToList();
                decimal canClose = decimal.Zero;

                foreach (var Line in LineItems)
                {
                    var PartialQuantity = decimal.Zero;
                    var CashSalesPartialItems = PartialCashSalesItem.Where(x => x.CashSalesInvoiceId == entity.CashSalesInvoiceId).ToList();
                    foreach (var PartialItem in CashSalesPartialItems)
                    {
                        var CashSalesPartialLineItems = PartialCashSalesLineItem.Where(x => x.CashSalesPartialItemId == PartialItem.CashSalesPartialItemId).ToList();

                        PartialQuantity +=  CRMSalesHelper.GetPartialLineQuantityByPartialItem(Line, PartialCashSalesLineItem, PartialItem); //CashSalesPartialILinetems.Where(x => x.LineItemID == Item.LineItemID).Sum(x => x.PartialPayedAmount);
        
                    }
                    PartialQuantity = (Line.CalculatedCost / Line.UnitAmount) - PartialQuantity;

                    canClose += PartialQuantity;
                        if (PartialQuantity > 0)
                        {
                          
                            var CashSalesPartialLine = new CashSalesPartialLineItem()
                            {
                                CashSalesPartialItemId = entity.CashSalesPartialItemId,
                                LineItemID = Line.LineItemID,
                                PartialPayedAmount = PartialQuantity,
                            };
                            db.CashSalesPartialLineItems.Add(CashSalesPartialLine);
                            db.SaveChanges();

                        Line.Quantity = Line.Quantity + (Line.CalculatedCost / Line.UnitAmount);
                        db.LineItems.Attach(Line);
                        db.Entry(Line).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                        else
                    {
                        Line.Quantity = Line.CalculatedCost / Line.UnitAmount;
                        db.LineItems.Attach(Line);
                        db.Entry(Line).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    

                    cashSalesPartialItem.CashSalesPartialItemId = entity.CashSalesPartialItemId;
                }
                if (canClose == 0)
                {
                    var cashInvoice = db.CashSalesInvoices.Where(x => x.CashSalesInvoiceID == Id).FirstOrDefault();
                    if (cashInvoice != null)
                    {
                        cashInvoice.LastObjectState = "PaymentReceived";
                        db.CashSalesInvoices.Attach(cashInvoice);
                        db.Entry(cashInvoice).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            return Json(new[] { cashSalesPartialItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CashSalesPartialItems_Update([DataSourceRequest]DataSourceRequest request, CashSalesPartialItem cashSalesPartialItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new CashSalesPartialItem
                {
                    CashSalesInvoiceId = cashSalesPartialItem.CashSalesInvoiceId,
                    CashSalesPartialItemId = cashSalesPartialItem.CashSalesPartialItemId,
                    Date = cashSalesPartialItem.Date,
                    IsReceiptPrinted = cashSalesPartialItem.IsReceiptPrinted,
                    IsPaymentProcessed = cashSalesPartialItem.IsPaymentProcessed,
                    Remark = cashSalesPartialItem.Remark,
                    Status = cashSalesPartialItem.Status,
                    IsVoid = cashSalesPartialItem.IsVoid,
                    VoidBy = cashSalesPartialItem.VoidBy,
                    VoidTime = cashSalesPartialItem.VoidTime,
                    IsOnlineApproved = cashSalesPartialItem.IsOnlineApproved,
                    OnlineApprovedBy = cashSalesPartialItem.OnlineApprovedBy,
                    OnlineApprovedTime = cashSalesPartialItem.OnlineApprovedTime,
                    IsOnlineTransferred = cashSalesPartialItem.IsOnlineTransferred,
                    IsOnlineTransferredBy = cashSalesPartialItem.IsOnlineTransferredBy,
                    OnlineTransferredTime = cashSalesPartialItem.OnlineTransferredTime,
                    IsSendForApproval = cashSalesPartialItem.IsSendForApproval,
                    OrgBranchName = cashSalesPartialItem.OrgBranchName,
                    SendForApprovalBy = cashSalesPartialItem.SendForApprovalBy,
                    SendForApprovalTime = cashSalesPartialItem.SendForApprovalTime,
                    LastModified = DateTime.Now,
                    DateCreated = cashSalesPartialItem.DateCreated,
                    ModifiedBy = User.Identity.Name,
                    CreatedBy = cashSalesPartialItem.CreatedBy
                };

                db.CashSalesPartialItems.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { cashSalesPartialItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CashSalesPartialItems_Destroy([DataSourceRequest]DataSourceRequest request, CashSalesPartialItem cashSalesPartialItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new CashSalesPartialItem
                {
                    CashSalesPartialItemId = cashSalesPartialItem.CashSalesPartialItemId,
                    Date = cashSalesPartialItem.Date,
                    IsReceiptPrinted = cashSalesPartialItem.IsReceiptPrinted,
                    Remark = cashSalesPartialItem.Remark,
                    Status = cashSalesPartialItem.Status,
                    IsVoid = cashSalesPartialItem.IsVoid,
                    VoidBy = cashSalesPartialItem.VoidBy,
                    VoidTime = cashSalesPartialItem.VoidTime,
                    IsOnlineApproved = cashSalesPartialItem.IsOnlineApproved,
                    OnlineApprovedBy = cashSalesPartialItem.OnlineApprovedBy,
                    OnlineApprovedTime = cashSalesPartialItem.OnlineApprovedTime,
                    IsOnlineTransferred = cashSalesPartialItem.IsOnlineTransferred,
                    IsOnlineTransferredBy = cashSalesPartialItem.IsOnlineTransferredBy,
                    OnlineTransferredTime = cashSalesPartialItem.OnlineTransferredTime,
                    IsSendForApproval = cashSalesPartialItem.IsSendForApproval,
                    OrgBranchName = cashSalesPartialItem.OrgBranchName,
                    SendForApprovalBy = cashSalesPartialItem.SendForApprovalBy,
                    SendForApprovalTime = cashSalesPartialItem.SendForApprovalTime,
                    DateCreated = cashSalesPartialItem.DateCreated,
                    LastModified = cashSalesPartialItem.LastModified,
                    CreatedBy = cashSalesPartialItem.CreatedBy,
                    ModifiedBy = cashSalesPartialItem.ModifiedBy
                };

                db.CashSalesPartialItems.Attach(entity);
                db.CashSalesPartialItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { cashSalesPartialItem }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
