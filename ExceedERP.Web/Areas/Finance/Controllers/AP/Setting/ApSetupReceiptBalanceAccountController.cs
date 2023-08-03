using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP
{
   [AuthorizeRoles(FinanceConstants.AccountPayableAdmin)]
    public class ApSetupReceiptBalanceAccountController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApSetupReceiptBalanceAccounts_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApSetupReceiptBalanceAccount> apsetupreceiptbalanceaccounts = _db.ApSetupReceiptBalanceAccounts;
            DataSourceResult result = apsetupreceiptbalanceaccounts.ToDataSourceResult(request, apSetupReceiptBalanceAccount => new {
                ApSetupReceiptBalanceAccountId = apSetupReceiptBalanceAccount.ApSetupReceiptBalanceAccountId,
                PrepaymentAccountCode = apSetupReceiptBalanceAccount.PrepaymentAccountCode,
                Type = apSetupReceiptBalanceAccount.Type,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApSetupReceiptBalanceAccounts_Create([DataSourceRequest]DataSourceRequest request, ApSetupReceiptBalanceAccount apSetupReceiptBalanceAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApSetupReceiptBalanceAccount
                {
                    PrepaymentAccountCode = apSetupReceiptBalanceAccount.PrepaymentAccountCode,
                    Type = apSetupReceiptBalanceAccount.Type,
                    DateCreated = DateTime.Now,
                    LastModified = apSetupReceiptBalanceAccount.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = apSetupReceiptBalanceAccount.ModifiedBy
                };

                _db.ApSetupReceiptBalanceAccounts.Add(entity);
                _db.SaveChanges();
                apSetupReceiptBalanceAccount.ApSetupReceiptBalanceAccountId = entity.ApSetupReceiptBalanceAccountId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP receipt balance account setup", "Success", User.Identity.Name, $"created  {apSetupReceiptBalanceAccount.PrepaymentAccountCode}", Modules.Finance);
                #endregion
            }

            return Json(new[] { apSetupReceiptBalanceAccount }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApSetupReceiptBalanceAccounts_Update([DataSourceRequest]DataSourceRequest request, ApSetupReceiptBalanceAccount apSetupReceiptBalanceAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApSetupReceiptBalanceAccount
                {
                    ApSetupReceiptBalanceAccountId = apSetupReceiptBalanceAccount.ApSetupReceiptBalanceAccountId,
                    PrepaymentAccountCode = apSetupReceiptBalanceAccount.PrepaymentAccountCode,
                    Type = apSetupReceiptBalanceAccount.Type,
                    DateCreated = apSetupReceiptBalanceAccount.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = apSetupReceiptBalanceAccount.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.ApSetupReceiptBalanceAccounts.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP receipt balance account setup", "Success", User.Identity.Name, $"updated  {apSetupReceiptBalanceAccount.PrepaymentAccountCode}", Modules.Finance);
                #endregion
            }

            return Json(new[] { apSetupReceiptBalanceAccount }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApSetupReceiptBalanceAccounts_Destroy([DataSourceRequest]DataSourceRequest request, ApSetupReceiptBalanceAccount apSetupReceiptBalanceAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApSetupReceiptBalanceAccount
                {
                    ApSetupReceiptBalanceAccountId = apSetupReceiptBalanceAccount.ApSetupReceiptBalanceAccountId,
                    Type = apSetupReceiptBalanceAccount.Type,
                    PrepaymentAccountCode = apSetupReceiptBalanceAccount.PrepaymentAccountCode
                };

                _db.ApSetupReceiptBalanceAccounts.Attach(entity);
                _db.ApSetupReceiptBalanceAccounts.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted AP setup account " + apSetupReceiptBalanceAccount.ApSetupReceiptBalanceAccountId + " " + apSetupReceiptBalanceAccount.PrepaymentAccountCode, Modules.Finance);
                #endregion
            }

            return Json(new[] { apSetupReceiptBalanceAccount }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
