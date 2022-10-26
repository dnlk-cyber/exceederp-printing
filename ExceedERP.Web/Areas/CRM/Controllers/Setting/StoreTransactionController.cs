using System;
using ExceedERP.Web.Helpers;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.CRM.Controllers
{

    [AuthorizeRoles(CRMRoles.CRMAdmin, CRMRoles.CashSalesUser, CRMRoles.CashSalesApprover )] 
    public class StoreTransactionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StoreTransactions_Read([DataSourceRequest]DataSourceRequest request, int voucherID)
        {
            IQueryable<StoreTransaction> storetransactions = db.StoreTransactions.Where(st => st.Voucher == voucherID.ToString());
            DataSourceResult result = storetransactions.ToDataSourceResult(request, storeTransaction => new {
                StoreTransactionID = storeTransaction.StoreTransactionID,
                Voucher = storeTransaction.Voucher,
                Source = storeTransaction.Source,
                Destination = storeTransaction.Destination,
                HasEffect = storeTransaction.HasEffect,
                Remark = storeTransaction.Remark ,
                TransactionType=storeTransaction.TransactionType,
                beforeStock = storeTransaction.beforeStock,
                afterStock = storeTransaction.afterStock,
                Quantity = storeTransaction.Quantity,
                CreatedBy = storeTransaction.CreatedBy,
                DateCreated = storeTransaction.DateCreated,
                LastModified = storeTransaction.LastModified,
                ModifiedBy = storeTransaction.ModifiedBy,
                OrgBranchName = storeTransaction.OrgBranchName,
                Amount = storeTransaction.Amount
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreTransactions_Create([DataSourceRequest]DataSourceRequest request, StoreTransaction storeTransaction, int voucherID,bool hasEffect)
        {
            if (ModelState.IsValid)
            {
                var entity = new StoreTransaction
                {
                    Voucher = voucherID.ToString(),
                    Source = storeTransaction.Source,
                    Destination = storeTransaction.Destination,
                    HasEffect = hasEffect,
                    Amount =   storeTransaction.Amount,
                    beforeStock = storeTransaction.beforeStock,
                    afterStock = storeTransaction.afterStock,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = storeTransaction.Remark
                };

                db.StoreTransactions.Add(entity);
                db.SaveChanges();
                storeTransaction.StoreTransactionID = entity.StoreTransactionID;
            }

            return Json(new[] { storeTransaction }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreTransactions_Update([DataSourceRequest]DataSourceRequest request, StoreTransaction storeTransaction)
        {
            if (ModelState.IsValid)
            {
                var entity = new StoreTransaction
                {
                    StoreTransactionID = storeTransaction.StoreTransactionID,
                    Voucher = storeTransaction.Voucher,
                    Source = storeTransaction.Source,
                    Destination = storeTransaction.Destination,
                    HasEffect = storeTransaction.HasEffect,
                    Amount = storeTransaction.Amount,

                    Remark = storeTransaction.Remark
                };

                db.StoreTransactions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { storeTransaction }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreTransactions_Destroy([DataSourceRequest]DataSourceRequest request, StoreTransaction storeTransaction)
        {
            if (ModelState.IsValid)
            {
                var entity = new StoreTransaction
                {
                    StoreTransactionID = storeTransaction.StoreTransactionID,
                    Voucher = storeTransaction.Voucher,
                    Source = storeTransaction.Source,
                    Destination = storeTransaction.Destination,
                    HasEffect = storeTransaction.HasEffect,
                    Remark = storeTransaction.Remark
                };
                db.StoreTransactions.Attach(entity);
                db.StoreTransactions.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { storeTransaction }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
