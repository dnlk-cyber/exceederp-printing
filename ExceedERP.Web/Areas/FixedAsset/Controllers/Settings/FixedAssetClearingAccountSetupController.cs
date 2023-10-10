using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Settings
{


     [AuthorizeRoles(FixedAssetConstants.FixedAssetAdmin)]
    public class FixedAssetClearingAccountSetupController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FixedAssetClearingAccountSetups_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FixedAssetClearingAccountSetup> fixedassetclearingaccountsetups = db.FixedAssetClearingAccountSetups;
            DataSourceResult result = fixedassetclearingaccountsetups.ToDataSourceResult(request, fixedAssetClearingAccountSetup => new {
                FixedAssetClearingAccountSetupID = fixedAssetClearingAccountSetup.FixedAssetClearingAccountSetupID,
                AccountCode = fixedAssetClearingAccountSetup.AccountCode,
                DateCreated = fixedAssetClearingAccountSetup.DateCreated,
                LastModified = fixedAssetClearingAccountSetup.LastModified,
                CreatedBy = fixedAssetClearingAccountSetup.CreatedBy,
                ModifiedBy = fixedAssetClearingAccountSetup.ModifiedBy
            });

            return Json(result);
        }

         [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetClearingAccountSetups_Create([DataSourceRequest]DataSourceRequest request, FixedAssetClearingAccountSetup fixedAssetClearingAccountSetup)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetClearingAccountSetup
                {
                    AccountCode = fixedAssetClearingAccountSetup.AccountCode,
                    DateCreated = fixedAssetClearingAccountSetup.DateCreated,
                    LastModified = fixedAssetClearingAccountSetup.LastModified,
                    CreatedBy = fixedAssetClearingAccountSetup.CreatedBy,
                    ModifiedBy = fixedAssetClearingAccountSetup.ModifiedBy
                };

                db.FixedAssetClearingAccountSetups.Add(entity);
                db.SaveChanges();
                fixedAssetClearingAccountSetup.FixedAssetClearingAccountSetupID = entity.FixedAssetClearingAccountSetupID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetClearingAccountSetup", "Success", User.Identity.Name, $"created FixedAssetClearingAccountSetup {entity.FixedAssetClearingAccountSetupID} AccountCode {entity.AccountCode}", Modules.FixedAsset);
                #endregion

            }

            return Json(new[] { fixedAssetClearingAccountSetup }.ToDataSourceResult(request, ModelState));
        }


       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetClearingAccountSetups_Update([DataSourceRequest]DataSourceRequest request, FixedAssetClearingAccountSetup fixedAssetClearingAccountSetup)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetClearingAccountSetup
                {
                    FixedAssetClearingAccountSetupID = fixedAssetClearingAccountSetup.FixedAssetClearingAccountSetupID,
                    AccountCode = fixedAssetClearingAccountSetup.AccountCode,
                    DateCreated = fixedAssetClearingAccountSetup.DateCreated,
                    LastModified = fixedAssetClearingAccountSetup.LastModified,
                    CreatedBy = fixedAssetClearingAccountSetup.CreatedBy,
                    ModifiedBy = fixedAssetClearingAccountSetup.ModifiedBy
                };

                db.FixedAssetClearingAccountSetups.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetClearingAccountSetup", "Success", User.Identity.Name, $"updated FixedAssetClearingAccountSetup {entity.FixedAssetClearingAccountSetupID} AccountCode {entity.AccountCode}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { fixedAssetClearingAccountSetup }.ToDataSourceResult(request, ModelState));
        }

       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetClearingAccountSetups_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetClearingAccountSetup fixedAssetClearingAccountSetup)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetClearingAccountSetup
                {
                    FixedAssetClearingAccountSetupID = fixedAssetClearingAccountSetup.FixedAssetClearingAccountSetupID,
                    AccountCode = fixedAssetClearingAccountSetup.AccountCode,
                    DateCreated = fixedAssetClearingAccountSetup.DateCreated,
                    LastModified = fixedAssetClearingAccountSetup.LastModified,
                    CreatedBy = fixedAssetClearingAccountSetup.CreatedBy,
                    ModifiedBy = fixedAssetClearingAccountSetup.ModifiedBy
                };

                db.FixedAssetClearingAccountSetups.Attach(entity);
                db.FixedAssetClearingAccountSetups.Remove(entity);
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Delete FixedAssetClearingAccountSetup {fixedAssetClearingAccountSetup.FixedAssetClearingAccountSetupID} AccountCode {fixedAssetClearingAccountSetup.AccountCode}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { fixedAssetClearingAccountSetup }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
