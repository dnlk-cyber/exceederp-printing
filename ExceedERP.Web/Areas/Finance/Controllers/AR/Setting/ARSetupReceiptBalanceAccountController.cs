using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.Setting
{
     [Authorize(Roles = AccountReceivableAdmin)]
    public class ArSetupReceiptBalanceAccountController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private const string AccountReceivableAdmin = "Account_Receivable_Admin";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ARSetupReceiptBalanceAccounts_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ARSetupReceiptBalanceAccount> arsetupreceiptbalanceaccounts = _db.ARSetupReceiptBalanceAccounts;
            DataSourceResult result = arsetupreceiptbalanceaccounts.ToDataSourceResult(request, aRSetupReceiptBalanceAccount => new {
                ARSetupReceiptBalanceAccountID = aRSetupReceiptBalanceAccount.ARSetupReceiptBalanceAccountID,
                UnidentifiedAccountCode = aRSetupReceiptBalanceAccount.UnidentifiedAccountCode,
                OnAccountAccountCode = aRSetupReceiptBalanceAccount.OnAccountAccountCode,
                UnappliedAccountCode = aRSetupReceiptBalanceAccount.UnappliedAccountCode,
                PrepaymentAccountCode = aRSetupReceiptBalanceAccount.PrepaymentAccountCode,
                Branch = aRSetupReceiptBalanceAccount.Branch
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARSetupReceiptBalanceAccounts_Create([DataSourceRequest]DataSourceRequest request, ARSetupReceiptBalanceAccount aRSetupReceiptBalanceAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new ARSetupReceiptBalanceAccount
                {
                    UnidentifiedAccountCode = aRSetupReceiptBalanceAccount.UnidentifiedAccountCode,
                    OnAccountAccountCode = aRSetupReceiptBalanceAccount.OnAccountAccountCode,
                    UnappliedAccountCode = aRSetupReceiptBalanceAccount.UnappliedAccountCode,
                    PrepaymentAccountCode = aRSetupReceiptBalanceAccount.PrepaymentAccountCode,
                    Branch = aRSetupReceiptBalanceAccount.Branch,
                    DateCreated = DateTime.Now,
                    LastModified = aRSetupReceiptBalanceAccount.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = aRSetupReceiptBalanceAccount.ModifiedBy
                };

                _db.ARSetupReceiptBalanceAccounts.Add(entity);
                _db.SaveChanges();
                aRSetupReceiptBalanceAccount.ARSetupReceiptBalanceAccountID = entity.ARSetupReceiptBalanceAccountID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR receipt balance account", "Success", User.Identity.Name, $"created  {aRSetupReceiptBalanceAccount.PrepaymentAccountCode}", Modules.Finance);
                #endregion
            }

            return Json(new[] { aRSetupReceiptBalanceAccount }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARSetupReceiptBalanceAccounts_Update([DataSourceRequest]DataSourceRequest request, ARSetupReceiptBalanceAccount aRSetupReceiptBalanceAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new ARSetupReceiptBalanceAccount
                {
                    ARSetupReceiptBalanceAccountID = aRSetupReceiptBalanceAccount.ARSetupReceiptBalanceAccountID,
                    UnidentifiedAccountCode = aRSetupReceiptBalanceAccount.UnidentifiedAccountCode,
                    OnAccountAccountCode = aRSetupReceiptBalanceAccount.OnAccountAccountCode,
                    UnappliedAccountCode = aRSetupReceiptBalanceAccount.UnappliedAccountCode,
                    PrepaymentAccountCode = aRSetupReceiptBalanceAccount.PrepaymentAccountCode,
                    Branch = aRSetupReceiptBalanceAccount.Branch,
                    DateCreated = aRSetupReceiptBalanceAccount.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = aRSetupReceiptBalanceAccount.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.ARSetupReceiptBalanceAccounts.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR receipt balance account", "Success", User.Identity.Name, $"updated  {aRSetupReceiptBalanceAccount.PrepaymentAccountCode}", Modules.Finance);
                #endregion
            }

            return Json(new[] { aRSetupReceiptBalanceAccount }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARSetupReceiptBalanceAccounts_Destroy([DataSourceRequest]DataSourceRequest request, ARSetupReceiptBalanceAccount aRSetupReceiptBalanceAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new ARSetupReceiptBalanceAccount
                {
                    ARSetupReceiptBalanceAccountID = aRSetupReceiptBalanceAccount.ARSetupReceiptBalanceAccountID,
                    UnidentifiedAccountCode = aRSetupReceiptBalanceAccount.UnidentifiedAccountCode,
                    OnAccountAccountCode = aRSetupReceiptBalanceAccount.OnAccountAccountCode,
                    UnappliedAccountCode = aRSetupReceiptBalanceAccount.UnappliedAccountCode,
                    PrepaymentAccountCode = aRSetupReceiptBalanceAccount.PrepaymentAccountCode,
                    Branch = aRSetupReceiptBalanceAccount.Branch
                };

                _db.ARSetupReceiptBalanceAccounts.Attach(entity);
                _db.ARSetupReceiptBalanceAccounts.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { aRSetupReceiptBalanceAccount }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
    
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
