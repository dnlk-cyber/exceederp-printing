using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    public class CartTransactionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CartTransactions_Read([DataSourceRequest]DataSourceRequest request, int voucherID)
        {
            IQueryable<CartTransaction> carttransactions = db.CartTransactions.Where(ct => ct.Voucher == voucherID.ToString());
            DataSourceResult result = carttransactions.ToDataSourceResult(request, cartTransaction => new {
                CartTransactionID = cartTransaction.CartTransactionID,
                Cart = cartTransaction.Cart,
                Voucher = cartTransaction.Voucher,
                Description = cartTransaction.Description,
                Value = cartTransaction.Value,
                CreatedBy = cartTransaction.CreatedBy,
                DateCreated = cartTransaction.DateCreated,
                LastModified = cartTransaction.LastModified,
                ModifiedBy = cartTransaction.ModifiedBy,
                OrgBranchName = cartTransaction.OrgBranchName,
                Remark = cartTransaction.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CartTransactions_Create([DataSourceRequest]DataSourceRequest request, CartTransaction cartTransaction, int voucherID)
        {
            if (ModelState.IsValid)
            {
                var entity = new CartTransaction
                {
                    Cart = cartTransaction.Cart,
                    Voucher = voucherID.ToString(),
                    Description = cartTransaction.Description,
                    Value = cartTransaction.Value,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = cartTransaction.Remark
                };

                db.CartTransactions.Add(entity);
                db.SaveChanges();
                cartTransaction.CartTransactionID = entity.CartTransactionID;
            }

            return Json(new[] { cartTransaction }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CartTransactions_Update([DataSourceRequest]DataSourceRequest request, CartTransaction cartTransaction)
        {
            if (ModelState.IsValid)
            {
                var entity = new CartTransaction
                {
                    CartTransactionID = cartTransaction.CartTransactionID,
                    Cart = cartTransaction.Cart,
                    Voucher = cartTransaction.Voucher,
                    Description = cartTransaction.Description,
                    Value = cartTransaction.Value,
                    Remark = cartTransaction.Remark
                };

                db.CartTransactions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { cartTransaction }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CartTransactions_Destroy([DataSourceRequest]DataSourceRequest request, CartTransaction cartTransaction)
        {
            if (ModelState.IsValid)
            {
                var entity = new CartTransaction
                {
                    CartTransactionID = cartTransaction.CartTransactionID,
                    Cart = cartTransaction.Cart,
                    Voucher = cartTransaction.Voucher,
                    Description = cartTransaction.Description,
                    Value = cartTransaction.Value,
                    Remark = cartTransaction.Remark
                };

                db.CartTransactions.Attach(entity);
                db.CartTransactions.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { cartTransaction }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
