using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Settings
{
    [AuthorizeRoles(FixedAssetConstants.FixedAssetAdmin)]
    public class FixedAssetDepreciationSetupController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }
    
        public ActionResult FixedAssetDepreciationSetups_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FixedAssetDepreciationSetup> fixedassetdepreciationsetups = db.FixedAssetDepreciationSetups;
            DataSourceResult result = fixedassetdepreciationsetups.ToDataSourceResult(request, fixedAssetDepreciationSetup => new {
                FixedAssetDepreciationSetupID = fixedAssetDepreciationSetup.FixedAssetDepreciationSetupID,
                FixedAssetAccountCode = fixedAssetDepreciationSetup.FixedAssetAccountCode,
                FixedAssetAccountCodeDescription = fixedAssetDepreciationSetup.FixedAssetAccountCodeDescription,
                DepreciationAccountCode = fixedAssetDepreciationSetup.DepreciationAccountCode,
                AccumulatedDepreciationAccountCode = fixedAssetDepreciationSetup.AccumulatedDepreciationAccountCode,
                DateCreated = fixedAssetDepreciationSetup.DateCreated,
                LastModified = fixedAssetDepreciationSetup.LastModified,
                CreatedBy = fixedAssetDepreciationSetup.CreatedBy,
                ModifiedBy = fixedAssetDepreciationSetup.ModifiedBy
            });

            return Json(result);
        }
   
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetDepreciationSetups_Create([DataSourceRequest]DataSourceRequest request, FixedAssetDepreciationSetup fixedAssetDepreciationSetup)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetDepreciationSetup
                {
                    FixedAssetAccountCode = fixedAssetDepreciationSetup.FixedAssetAccountCode,
                    FixedAssetAccountCodeDescription = fixedAssetDepreciationSetup.FixedAssetAccountCodeDescription,
                    DepreciationAccountCode = fixedAssetDepreciationSetup.DepreciationAccountCode,
                    AccumulatedDepreciationAccountCode = fixedAssetDepreciationSetup.AccumulatedDepreciationAccountCode,
                    DateCreated = fixedAssetDepreciationSetup.DateCreated,
                    LastModified = fixedAssetDepreciationSetup.LastModified,
                    CreatedBy = fixedAssetDepreciationSetup.CreatedBy,
                    ModifiedBy = fixedAssetDepreciationSetup.ModifiedBy
                };

                db.FixedAssetDepreciationSetups.Add(entity);
                db.SaveChanges();
                fixedAssetDepreciationSetup.FixedAssetDepreciationSetupID = entity.FixedAssetDepreciationSetupID;
            }

            return Json(new[] { fixedAssetDepreciationSetup }.ToDataSourceResult(request, ModelState));
        }
    
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetDepreciationSetups_Update([DataSourceRequest]DataSourceRequest request, FixedAssetDepreciationSetup fixedAssetDepreciationSetup)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetDepreciationSetup
                {
                    FixedAssetDepreciationSetupID = fixedAssetDepreciationSetup.FixedAssetDepreciationSetupID,
                    FixedAssetAccountCode = fixedAssetDepreciationSetup.FixedAssetAccountCode,
                    FixedAssetAccountCodeDescription = fixedAssetDepreciationSetup.FixedAssetAccountCodeDescription,
                    DepreciationAccountCode = fixedAssetDepreciationSetup.DepreciationAccountCode,
                    AccumulatedDepreciationAccountCode = fixedAssetDepreciationSetup.AccumulatedDepreciationAccountCode,
                    DateCreated = fixedAssetDepreciationSetup.DateCreated,
                    LastModified = fixedAssetDepreciationSetup.LastModified,
                    CreatedBy = fixedAssetDepreciationSetup.CreatedBy,
                    ModifiedBy = fixedAssetDepreciationSetup.ModifiedBy
                };

                db.FixedAssetDepreciationSetups.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { fixedAssetDepreciationSetup }.ToDataSourceResult(request, ModelState));
        }
    
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetDepreciationSetups_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetDepreciationSetup fixedAssetDepreciationSetup)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetDepreciationSetup
                {
                    FixedAssetDepreciationSetupID = fixedAssetDepreciationSetup.FixedAssetDepreciationSetupID,
                    FixedAssetAccountCode = fixedAssetDepreciationSetup.FixedAssetAccountCode,
                    FixedAssetAccountCodeDescription = fixedAssetDepreciationSetup.FixedAssetAccountCodeDescription,
                    DepreciationAccountCode = fixedAssetDepreciationSetup.DepreciationAccountCode,
                    AccumulatedDepreciationAccountCode = fixedAssetDepreciationSetup.AccumulatedDepreciationAccountCode,
                    DateCreated = fixedAssetDepreciationSetup.DateCreated,
                    LastModified = fixedAssetDepreciationSetup.LastModified,
                    CreatedBy = fixedAssetDepreciationSetup.CreatedBy,
                    ModifiedBy = fixedAssetDepreciationSetup.ModifiedBy
                };

                db.FixedAssetDepreciationSetups.Attach(entity);
                db.FixedAssetDepreciationSetups.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { fixedAssetDepreciationSetup }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
