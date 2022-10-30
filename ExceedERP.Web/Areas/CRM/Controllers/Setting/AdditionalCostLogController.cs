using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    public class AdditionalCostLogController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LineItems_Read([DataSourceRequest]DataSourceRequest request, int voucher, VoucherType type)
        {
            IQueryable<AdditionalCostLog> lineItems = db.AdditionalCostLogs.Where(a => a.VoucherType == type && a.VoucherId == voucher);
            DataSourceResult result = lineItems.ToDataSourceResult(request, additionalCostLog => new AdditionalCostLog
            {
                AdditionalCostLogId = additionalCostLog.AdditionalCostLogId,
                AdditionalCostId = additionalCostLog.AdditionalCostId,
                GLTaxRateID = additionalCostLog.GLTaxRateID,
                CostAmount = additionalCostLog.CostAmount,
                TaxAmount = additionalCostLog.TaxAmount,
                TotalAmount = additionalCostLog.TotalAmount,
                VoucherId = additionalCostLog.VoucherId,
                VoucherType = additionalCostLog.VoucherType,

                CreatedBy = additionalCostLog.CreatedBy,
                DateCreated = additionalCostLog.DateCreated,
                LastModified = additionalCostLog.LastModified,
                ModifiedBy = additionalCostLog.ModifiedBy,
                OrgBranchName = additionalCostLog.OrgBranchName,

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LineItem_Create([DataSourceRequest]DataSourceRequest request, AdditionalCostLog additionalCostLog, int voucher, VoucherType type)
        {
            if (ModelState.IsValid)
            {
                var entity = new AdditionalCostLog
                {

                    AdditionalCostLogId = additionalCostLog.AdditionalCostLogId,
                    AdditionalCostId = additionalCostLog.AdditionalCostId,
                    GLTaxRateID = additionalCostLog.GLTaxRateID,
                    CostAmount = additionalCostLog.CostAmount,
                    TaxAmount = this.CalculateTax(additionalCostLog.GLTaxRateID, additionalCostLog.CostAmount),
                    //TotalAmount = additionalCostLog.TotalAmount,
                    VoucherId = voucher,
                    VoucherType = type,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                };
                entity.TotalAmount = entity.CostAmount + entity.TaxAmount;

                db.AdditionalCostLogs.Add(entity);
                db.SaveChanges();
                additionalCostLog.AdditionalCostLogId = entity.AdditionalCostLogId;
                UserHelper.OperationLog(Request.UserHostAddress, "Create", "Success", User.Identity.Name, "Create "+ type.ToString() +" Additional Cost Line Item with id " + additionalCostLog.AdditionalCostLogId, Modules.CRM);

            }

            return Json(new[] { additionalCostLog }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LineItem_Update([DataSourceRequest]DataSourceRequest request, AdditionalCostLog additionalCostLog)
        {
            if (ModelState.IsValid)
            {
                var entity = new AdditionalCostLog
                {
                    AdditionalCostLogId = additionalCostLog.AdditionalCostLogId,
                    AdditionalCostId = additionalCostLog.AdditionalCostId,
                    GLTaxRateID = additionalCostLog.GLTaxRateID,
                    CostAmount = additionalCostLog.CostAmount,
                    TaxAmount = this.CalculateTax(additionalCostLog.GLTaxRateID, additionalCostLog.CostAmount),
                    TotalAmount = additionalCostLog.TotalAmount,
                    VoucherId = additionalCostLog.VoucherId,
                    VoucherType = additionalCostLog.VoucherType,

                    CreatedBy = additionalCostLog.CreatedBy,
                    DateCreated = additionalCostLog.DateCreated,
                    LastModified = additionalCostLog.LastModified,
                    ModifiedBy = additionalCostLog.ModifiedBy,
                    OrgBranchName = additionalCostLog.OrgBranchName
                };
                entity.TotalAmount = entity.CostAmount + entity.TaxAmount;

                db.AdditionalCostLogs.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                UserHelper.OperationLog(Request.UserHostAddress, "Update", "Success", User.Identity.Name, "Updated Addtional Cost Line Item with id " + additionalCostLog.AdditionalCostLogId, Modules.CRM);

            }

            return Json(new[] { additionalCostLog }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LineItem_Destroy([DataSourceRequest]DataSourceRequest request, AdditionalCostLog additionalCostLog)
        {
            if (ModelState.IsValid)
            {
                var entity = new AdditionalCostLog
                {
                    AdditionalCostLogId = additionalCostLog.AdditionalCostLogId,
                    AdditionalCostId = additionalCostLog.AdditionalCostId,
                    GLTaxRateID = additionalCostLog.GLTaxRateID,
                    CostAmount = additionalCostLog.CostAmount,
                    TaxAmount = additionalCostLog.TaxAmount,
                    TotalAmount = additionalCostLog.TotalAmount,
                    VoucherId = additionalCostLog.VoucherId,
                    VoucherType = additionalCostLog.VoucherType,

                    CreatedBy = additionalCostLog.CreatedBy,
                    DateCreated = additionalCostLog.DateCreated,
                    LastModified = additionalCostLog.LastModified,
                    ModifiedBy = additionalCostLog.ModifiedBy,
                    OrgBranchName = additionalCostLog.OrgBranchName

                };

                db.AdditionalCostLogs.Attach(entity);
                db.AdditionalCostLogs.Remove(entity);
                db.SaveChanges();
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Addtional Cost Line Item with id " + additionalCostLog.AdditionalCostLogId, Modules.CRM);

            }

            return Json(new[] { additionalCostLog }.ToDataSourceResult(request, ModelState));
        }

        private decimal CalculateTax(int taxId, decimal subTotal)
        {
            var amnt = decimal.Zero;
            var taxRate = db.GLTaxRates.Find(taxId);
            if(taxRate != null)
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