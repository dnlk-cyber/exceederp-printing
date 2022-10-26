using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CashSalesApprover, CRMRoles.CashSalesUser)]
    public class CommissionLineItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LineItems_Read([DataSourceRequest]DataSourceRequest request, int voucher, VoucherType type)
        {
            IQueryable<CommissionLineItem> lineItems = db.CommissionLineItems.Where(c => c.VoucherId == voucher && c.VoucherType == type);
            DataSourceResult result = lineItems.ToDataSourceResult(request, commissionLineItem => new CommissionLineItem
            {
                CommissionLineItemId = commissionLineItem.CommissionLineItemId,
                Reason = commissionLineItem.Reason,
                GLTaxRateID = commissionLineItem.GLTaxRateID,
                CostAmount = commissionLineItem.CostAmount,
                TaxAmount = commissionLineItem.TaxAmount,
                TotalAmount = commissionLineItem.TotalAmount,
                VoucherId = commissionLineItem.VoucherId,
                VoucherType = commissionLineItem.VoucherType,
                
                CreatedBy = commissionLineItem.CreatedBy,
                DateCreated = commissionLineItem.DateCreated,
                LastModified = commissionLineItem.LastModified,
                ModifiedBy = commissionLineItem.ModifiedBy,
                OrgBranchName = commissionLineItem.OrgBranchName,

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LineItem_Create([DataSourceRequest]DataSourceRequest request, CommissionLineItem commissionLineItem, int voucher, VoucherType type)
        {
            if (ModelState.IsValid)
            {
                var entity = new CommissionLineItem
                {
                    CommissionLineItemId = commissionLineItem.CommissionLineItemId,
                    Reason = commissionLineItem.Reason,
                    GLTaxRateID = commissionLineItem.GLTaxRateID,
                    CostAmount = commissionLineItem.CostAmount,
                    TaxAmount = this.CalculateTax(commissionLineItem.GLTaxRateID, commissionLineItem.CostAmount),
                    TotalAmount = commissionLineItem.TotalAmount,
                    VoucherId = voucher,
                    VoucherType = type,
                    
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,

                };
                entity.TotalAmount = entity.CostAmount + entity.TaxAmount;

                db.CommissionLineItems.Add(entity);
                db.SaveChanges();
                commissionLineItem.CommissionLineItemId = entity.CommissionLineItemId;
                UserHelper.OperationLog(Request.UserHostAddress, "Create", "Success", User.Identity.Name, "Create Commission Line Item with id " + commissionLineItem.CommissionLineItemId, Modules.CRM);

            }

            return Json(new[] { commissionLineItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LineItem_Update([DataSourceRequest]DataSourceRequest request, CommissionLineItem commissionLineItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new CommissionLineItem
                {
                    CommissionLineItemId = commissionLineItem.CommissionLineItemId,
                    Reason = commissionLineItem.Reason,
                    GLTaxRateID = commissionLineItem.GLTaxRateID,
                    CostAmount = commissionLineItem.CostAmount,
                    TaxAmount = this.CalculateTax(commissionLineItem.GLTaxRateID, commissionLineItem.CostAmount),
                    TotalAmount = commissionLineItem.TotalAmount,
                    VoucherId = commissionLineItem.VoucherId,
                    VoucherType = commissionLineItem.VoucherType,

                    CreatedBy = commissionLineItem.CreatedBy,
                    DateCreated =commissionLineItem.DateCreated,
                    LastModified = commissionLineItem.LastModified,
                    ModifiedBy = commissionLineItem.ModifiedBy,
                    OrgBranchName = commissionLineItem.OrgBranchName
                };
                entity.TotalAmount = entity.CostAmount + entity.TaxAmount;

                db.CommissionLineItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                UserHelper.OperationLog(Request.UserHostAddress, "Update", "Success", User.Identity.Name, "Updated Commission Line Item with id " + commissionLineItem.CommissionLineItemId, Modules.CRM);

            }

            return Json(new[] { commissionLineItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LineItem_Destroy([DataSourceRequest]DataSourceRequest request, CommissionLineItem commissionLineItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new CommissionLineItem
                {
                    CommissionLineItemId = commissionLineItem.CommissionLineItemId,
                    Reason = commissionLineItem.Reason,
                    GLTaxRateID = commissionLineItem.GLTaxRateID,
                    CostAmount = commissionLineItem.CostAmount,
                    TaxAmount = commissionLineItem.TaxAmount,
                    TotalAmount = commissionLineItem.TotalAmount,
                    VoucherId = commissionLineItem.VoucherId,
                    VoucherType = commissionLineItem.VoucherType,

                    CreatedBy = commissionLineItem.CreatedBy,
                    DateCreated = commissionLineItem.DateCreated,
                    LastModified = commissionLineItem.LastModified,
                    ModifiedBy = commissionLineItem.ModifiedBy,
                    OrgBranchName = commissionLineItem.OrgBranchName

                };

                db.CommissionLineItems.Attach(entity);
                db.CommissionLineItems.Remove(entity);
                db.SaveChanges();
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Commission Line Item with id " + commissionLineItem.CommissionLineItemId, Modules.CRM);

            }

            return Json(new[] { commissionLineItem }.ToDataSourceResult(request, ModelState));
        }
        private decimal CalculateTax(int taxId, decimal subTotal)
        {
            var amnt = decimal.Zero;
            var taxRate = db.GLTaxRates.Find(taxId);
            if (taxRate != null)
            {
                amnt = taxRate.CalculatedRate * subTotal;
            }
            return amnt;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}