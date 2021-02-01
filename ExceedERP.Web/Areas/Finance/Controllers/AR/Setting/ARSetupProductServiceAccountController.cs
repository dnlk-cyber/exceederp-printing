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
    public class ArSetupProductServiceAccountController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private const string AccountReceivableAdmin = "Account_Receivable_Admin";

        public ActionResult Index()
        {
            PopulateCategories();
            return View();
        }
        private void PopulateCategories()
        {

            var uoMs = _db.UoMs;

            ViewData["Units"] = uoMs;
        }

        public ActionResult ARSetupProductServiceAccounts_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ARSetupProductServiceAccount> arsetupproductserviceaccounts = _db.ARSetupProductServiceAccounts;
            DataSourceResult result = arsetupproductserviceaccounts.ToDataSourceResult(request, aRSetupProductServiceAccount => new {
                ARSetupProductServiceAccountID = aRSetupProductServiceAccount.ARSetupProductServiceAccountID,
                Product = aRSetupProductServiceAccount.Product,
                Description = aRSetupProductServiceAccount.Description,
                AccountCode = aRSetupProductServiceAccount.AccountCode,
                OrgBranchName = aRSetupProductServiceAccount.OrgBranchName,
                DateCreated = aRSetupProductServiceAccount.DateCreated,
                LastModified = aRSetupProductServiceAccount.LastModified,
                CreatedBy = aRSetupProductServiceAccount.CreatedBy,
                ModifiedBy = aRSetupProductServiceAccount.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARSetupProductServiceAccounts_Create([DataSourceRequest]DataSourceRequest request, ARSetupProductServiceAccount aRSetupProductServiceAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new ARSetupProductServiceAccount
                {
                    Product = aRSetupProductServiceAccount.Product,
                    Description = aRSetupProductServiceAccount.Description,
                    AccountCode = aRSetupProductServiceAccount.AccountCode,
                    OrgBranchName = aRSetupProductServiceAccount.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = aRSetupProductServiceAccount.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = aRSetupProductServiceAccount.ModifiedBy
                };

                _db.ARSetupProductServiceAccounts.Add(entity);
                _db.SaveChanges();
                aRSetupProductServiceAccount.ARSetupProductServiceAccountID = entity.ARSetupProductServiceAccountID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR product or service account", "Success", User.Identity.Name, $"created  {aRSetupProductServiceAccount.AccountCode}", Modules.Finance);
                #endregion
            }

            return Json(new[] { aRSetupProductServiceAccount }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARSetupProductServiceAccounts_Update([DataSourceRequest]DataSourceRequest request, ARSetupProductServiceAccount aRSetupProductServiceAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new ARSetupProductServiceAccount
                {
                    ARSetupProductServiceAccountID = aRSetupProductServiceAccount.ARSetupProductServiceAccountID,
                    Product = aRSetupProductServiceAccount.Product,
                    Description = aRSetupProductServiceAccount.Description,
                    AccountCode = aRSetupProductServiceAccount.AccountCode,
                    OrgBranchName = aRSetupProductServiceAccount.OrgBranchName,
                    DateCreated = aRSetupProductServiceAccount.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = aRSetupProductServiceAccount.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.ARSetupProductServiceAccounts.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR product or service account", "Success", User.Identity.Name, $"updated {aRSetupProductServiceAccount.AccountCode}", Modules.Finance);
                #endregion
            }

            return Json(new[] { aRSetupProductServiceAccount }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARSetupProductServiceAccounts_Destroy([DataSourceRequest]DataSourceRequest request, ARSetupProductServiceAccount aRSetupProductServiceAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new ARSetupProductServiceAccount
                {
                    ARSetupProductServiceAccountID = aRSetupProductServiceAccount.ARSetupProductServiceAccountID,
                    Product = aRSetupProductServiceAccount.Product,
                    Description = aRSetupProductServiceAccount.Description,
                    AccountCode = aRSetupProductServiceAccount.AccountCode,
                    OrgBranchName = aRSetupProductServiceAccount.OrgBranchName,
                    DateCreated = aRSetupProductServiceAccount.DateCreated,
                    LastModified = aRSetupProductServiceAccount.LastModified,
                    CreatedBy = aRSetupProductServiceAccount.CreatedBy,
                    ModifiedBy = aRSetupProductServiceAccount.ModifiedBy
                };

                _db.ARSetupProductServiceAccounts.Attach(entity);
                _db.ARSetupProductServiceAccounts.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { aRSetupProductServiceAccount }.ToDataSourceResult(request, ModelState));
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
