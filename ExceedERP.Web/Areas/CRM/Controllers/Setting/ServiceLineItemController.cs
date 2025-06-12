using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.CRM;
using ExceedERP.Web.Areas.CRM.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    //[AuthorizeRoles(CRMRoles.CRMAdmin, CRMRoles.ServiceSupportUser)] 
    public class ServiceLineItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();



        public ActionResult ServiceLineItems_Read([DataSourceRequest]DataSourceRequest request, int voucherID, CRMVoucherType voucherType)
        {
            IQueryable<ServiceLineItem> ServiceLineItems = db.ServiceLineItems.Where(x => x.VoucherId == voucherID && x.VoucherType == voucherType);
            DataSourceResult result = ServiceLineItems.ToDataSourceResult(request, ServiceLineItem => new
            {
                ServiceLineItemId = ServiceLineItem.ServiceLineItemId,
                VoucherId = ServiceLineItem.VoucherId,
                VoucherType = ServiceLineItem.VoucherType,
                ServiceTypeCode = ServiceLineItem.ServiceTypeCode,
                ServiceTypeId = ServiceLineItem.ServiceTypeId,
                Quantity = ServiceLineItem.Quantity,
                UnitAmount = ServiceLineItem.UnitAmount,
                SubTotal = ServiceLineItem.SubTotal,
                TaxTypeId = ServiceLineItem.TaxTypeId,
                TaxAmount = ServiceLineItem.TaxAmount,
                WithholdingTypeId = ServiceLineItem.WithholdingTypeId,
                WithholdingAmount = ServiceLineItem.WithholdingAmount,
                WithholdTaxAmount = ServiceLineItem.WithholdTaxAmount,
                WithholdTax = ServiceLineItem.WithholdTax,
                TotalAmount = ServiceLineItem.TotalAmount,


                CreatedBy = ServiceLineItem.CreatedBy,
                DateCreated = ServiceLineItem.DateCreated,
                LastModified = ServiceLineItem.LastModified,
                ModifiedBy = ServiceLineItem.ModifiedBy,

            });

            return Json(result);
        }

        public ActionResult ServiceLineItemApproval_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            List<ServiceUsageApproval> usageApproval = db.ServiceUsageApprovals.Where(x => x.ServiceUsageId == Id).ToList();


            DataSourceResult result = usageApproval.ToDataSourceResult(request, approval => new ServiceUsageApproval()
            {
                ServiceUsageApprovalId = approval.ServiceUsageApprovalId,
                ServiceUsageId = approval.ServiceUsageId,
                Remark = approval.Remark,
                Status = approval.Status,

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceLineItems_Create([DataSourceRequest]DataSourceRequest request, ServiceLineItem ServiceLineItem, int Id)
        {

            //if (ServiceLineItem.TotalAmount == 0)
            //{
            //    ModelState.AddModelError("InvalidQuantity", " Total Amount Can't Be 0 ! ");

            //}
            //else if (ServiceLineItem.UnitPrice == 0)
            //{
            //    ModelState.AddModelError("InvalidQuantity", "Unit Price Value Can't Be 0 ! ");

            //}
            //else
            //{
            if (ModelState.IsValid)
            {
                var entity = new ServiceLineItem
                {

                    ServiceLineItemId = ServiceLineItem.ServiceLineItemId,
                    VoucherId = ServiceLineItem.VoucherId,
                    VoucherType = ServiceLineItem.VoucherType,
                    ServiceTypeCode = ServiceLineItem.ServiceTypeCode,
                    ServiceTypeId = ServiceLineItem.ServiceTypeId,
                    Quantity = ServiceLineItem.Quantity,
                    UnitAmount = ServiceLineItem.UnitAmount,
                    SubTotal = ServiceLineItem.SubTotal,
                    TaxTypeId = ServiceLineItem.TaxTypeId,
                    TaxAmount = ServiceLineItem.TaxAmount,
                    WithholdingTypeId = ServiceLineItem.WithholdingTypeId,
                    WithholdingAmount = ServiceLineItem.WithholdingAmount,
                    WithholdTaxAmount = ServiceLineItem.WithholdTaxAmount,
                    WithholdTax = ServiceLineItem.WithholdTax,
                    TotalAmount = ServiceLineItem.TotalAmount,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = ServiceLineItem.ModifiedBy,
                    LastModified = ServiceLineItem.LastModified,

                };
                entity.SubTotal = entity.Quantity * entity.UnitAmount;
                entity.TaxAmount = CRMSalesHelper.CalculateTaxAmount(entity.TaxTypeId, entity.SubTotal);
                if (entity.WithholdTax)
                {
                    entity.WithholdTaxAmount = CRMSalesHelper.CalculateVatWithholdingAmount(entity.SubTotal, entity.TaxAmount);
                }
                else
                {
                    entity.WithholdTaxAmount = 0;
                }
                entity.WithholdingAmount = CRMSalesHelper.CalculateTaxAmount(entity.WithholdingTypeId, entity.SubTotal);
                entity.TotalAmount = entity.SubTotal + entity.TaxAmount;

                db.ServiceLineItems.Add(entity);
                db.SaveChanges();
                ServiceLineItem.ServiceLineItemId = entity.ServiceLineItemId;

                if (entity.VoucherType == CRMVoucherType.SalesInvoice)
                {
                    var invoiceHeader = db.CashSalesInvoices
                        .AsNoTracking()
                        .FirstOrDefault(inv => inv.CashSalesInvoiceID == entity.VoucherId);
                    if (invoiceHeader != null)
                    {
                        new CRMSalesHelper().UpdateSalesInvoiceRecordOnLineEdit(invoiceHeader);
                        new CRMPaymentDistibutionHelper().UpdateCashSalesInvoiceDistribution(invoiceHeader);
                    }
                }
            }


            return Json(new[] { ServiceLineItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceLineItems_Update([DataSourceRequest]DataSourceRequest request, ServiceLineItem ServiceLineItem, int voucherID, CRMVoucherType voucherType)
        {
            //if (ServiceLineItem.TotalAmount == 0)
            //{
            //    ModelState.AddModelError("InvalidQuantity", " Total Amount Can't Be 0 ! ");

            //}
            //else if (ServiceLineItem.UnitPrice == 0)
            //{
            //    ModelState.AddModelError("InvalidQuantity", "Unit Price Value Can't Be 0 ! ");

            //}
            //else
            //{
            var entity = new ServiceLineItem
            {
                ServiceLineItemId = ServiceLineItem.ServiceLineItemId,
                VoucherId = ServiceLineItem.VoucherId,
                VoucherType = ServiceLineItem.VoucherType,
                ServiceTypeCode = ServiceLineItem.ServiceTypeCode,
                ServiceTypeId = ServiceLineItem.ServiceTypeId,
                Quantity = ServiceLineItem.Quantity,
                UnitAmount = ServiceLineItem.UnitAmount,
                SubTotal = ServiceLineItem.SubTotal,
                TaxTypeId = ServiceLineItem.TaxTypeId,
                TaxAmount = ServiceLineItem.TaxAmount,
                WithholdingTypeId = ServiceLineItem.WithholdingTypeId,
                WithholdingAmount = ServiceLineItem.WithholdingAmount,
                WithholdTaxAmount = ServiceLineItem.WithholdTaxAmount,
                WithholdTax = ServiceLineItem.WithholdTax,
                TotalAmount = ServiceLineItem.TotalAmount,


                CreatedBy = ServiceLineItem.CreatedBy,
                DateCreated = ServiceLineItem.DateCreated,
                LastModified = DateTime.Now,
                ModifiedBy = User.Identity.Name
            };
            entity.SubTotal = entity.Quantity * entity.UnitAmount;
            entity.TaxAmount = CRMSalesHelper.CalculateTaxAmount(entity.TaxTypeId, entity.SubTotal);
            if (entity.WithholdTax)
            {
                entity.WithholdTaxAmount = CRMSalesHelper.CalculateVatWithholdingAmount(entity.SubTotal, entity.TaxAmount);
            }
            else
            {
                entity.WithholdTaxAmount = 0;
            }
            entity.WithholdingAmount = CRMSalesHelper.CalculateTaxAmount(entity.WithholdingTypeId, entity.SubTotal);
            entity.TotalAmount = entity.SubTotal + entity.TaxAmount;

            db.ServiceLineItems.Attach(entity);
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();

            if (entity.VoucherType == CRMVoucherType.SalesInvoice)
            {
                var invoiceHeader = db.CashSalesInvoices
                    .AsNoTracking()
                    .FirstOrDefault(inv => inv.CashSalesInvoiceID == entity.VoucherId);
                if (invoiceHeader != null)
                {
                    new CRMSalesHelper().UpdateSalesInvoiceRecordOnLineEdit(invoiceHeader);
                    new CRMPaymentDistibutionHelper().UpdateCashSalesInvoiceDistribution(invoiceHeader);
                }
            }

            return Json(new[] { ServiceLineItem }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceLineItems_Destroy([DataSourceRequest]DataSourceRequest request, ServiceLineItem ServiceLineItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new ServiceLineItem
                {
                    ServiceLineItemId = ServiceLineItem.ServiceLineItemId,
                    VoucherId = ServiceLineItem.VoucherId,
                    VoucherType = ServiceLineItem.VoucherType,
                    ServiceTypeCode = ServiceLineItem.ServiceTypeCode,
                    ServiceTypeId = ServiceLineItem.ServiceTypeId,
                    Quantity = ServiceLineItem.Quantity,
                    UnitAmount = ServiceLineItem.UnitAmount,
                    SubTotal = ServiceLineItem.SubTotal,
                    TaxTypeId = ServiceLineItem.TaxTypeId,
                    TaxAmount = ServiceLineItem.TaxAmount,
                    WithholdingTypeId = ServiceLineItem.WithholdingTypeId,
                    WithholdingAmount = ServiceLineItem.WithholdingAmount,
                    WithholdTaxAmount = ServiceLineItem.WithholdTaxAmount,
                    WithholdTax = ServiceLineItem.WithholdTax,
                    TotalAmount = ServiceLineItem.TotalAmount,
                    

                    CreatedBy = ServiceLineItem.CreatedBy,
                    DateCreated = ServiceLineItem.DateCreated,
                    LastModified = ServiceLineItem.LastModified,
                    ModifiedBy = ServiceLineItem.ModifiedBy,
                };

                db.ServiceLineItems.Attach(entity);
                db.ServiceLineItems.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  Service LineItem number " + ServiceLineItem.ServiceLineItemId);

                if (entity.VoucherType == CRMVoucherType.SalesInvoice)
                {
                    var invoiceHeader = db.CashSalesInvoices
                        .AsNoTracking()
                        .FirstOrDefault(inv => inv.CashSalesInvoiceID == entity.VoucherId);
                    if (invoiceHeader != null)
                    {
                        new CRMSalesHelper().UpdateSalesInvoiceRecordOnLineEdit(invoiceHeader);
                        new CRMPaymentDistibutionHelper().UpdateCashSalesInvoiceDistribution(invoiceHeader);
                    }
                }
            }

            return Json(new[] { ServiceLineItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceLineItems_Approve([DataSourceRequest]DataSourceRequest request, ServiceUsageApproval ServiceUsageApproval, int Id)
        {
            if (ServiceUsageApproval.Status == CRMConstants.Approved)
            {
                var entity = db.ServiceUsages.Where(x => x.ServiceUsageId == Id).FirstOrDefault();

                entity.IsOnlineApproved = true;
                entity.OnlineApprovedBy = User.Identity.Name;
                entity.OnlineApprovedTime = DateTime.Now;

                db.ServiceUsages.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            var approval = new ServiceUsageApproval()
            {
                Status = ServiceUsageApproval.Status,
                ServiceUsageId = Id,
                Remark = ServiceUsageApproval.Remark,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name

            };

            db.ServiceUsageApprovals.Add(approval);
            db.SaveChanges();
            ServiceUsageApproval.ServiceUsageApprovalId = approval.ServiceUsageApprovalId;
            return Json(new[] { approval }.ToDataSourceResult(request));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
