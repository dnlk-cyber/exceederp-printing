using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Assetregistration
{
    public class AssetDisposalItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult DisposalItems_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<DisposalItem> disposalitems = db.DisposalItems.Where(x=>x.AssetDisposalId==id);
            DataSourceResult result = disposalitems.ToDataSourceResult(request, disposalItem => new {
                AssetDisposalId = disposalItem.AssetDisposalId,
                DisposalItemId = disposalItem.DisposalItemId,
                FixedAssetCategoryID = disposalItem.FixedAssetCategoryID,
                FixedAssetSubCategoryID = disposalItem.FixedAssetSubCategoryID,
                AssetRegistrationId = disposalItem.AssetRegistrationId,
                BookValue = disposalItem.BookValue,
                SalesPrice = disposalItem.SalesPrice,
                GlTaxRateId = disposalItem.GlTaxRateId,
                TaxAmount = disposalItem.TaxAmount,
                Total = disposalItem.Total,
                OrgBranchName = disposalItem.OrgBranchName,
                DateCreated = disposalItem.DateCreated,
                LastModified = disposalItem.LastModified,
                CreatedBy = disposalItem.CreatedBy,
                ModifiedBy = disposalItem.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DisposalItems_Create(int id,[DataSourceRequest]DataSourceRequest request, DisposalItem disposalItem)
        {
            //if(disposalItem.SalesPrice==0)
            //    ModelState.AddModelError("zero","Sales price can't be zero.");

            var tax = db.GLTaxRates.Find(disposalItem.GlTaxRateId);
            if (tax != null)
            {
                disposalItem.TaxAmount = disposalItem.SalesPrice * tax.CalculatedRate;
                disposalItem.Total = disposalItem.SalesPrice + disposalItem.TaxAmount;
            }

            if (ModelState.IsValid)
            {
                var entity = new DisposalItem
                {
                    AssetDisposalId = id,
                    FixedAssetCategoryID = disposalItem.FixedAssetCategoryID,
                    FixedAssetSubCategoryID = disposalItem.FixedAssetSubCategoryID,
                    AssetRegistrationId = disposalItem.AssetRegistrationId,
                    BookValue = disposalItem.BookValue,
                    SalesPrice = disposalItem.SalesPrice,
                    GlTaxRateId = disposalItem.GlTaxRateId,
                    TaxAmount = disposalItem.TaxAmount,
                    Total = disposalItem.Total,
                    OrgBranchName = disposalItem.OrgBranchName,
                    DateCreated = disposalItem.DateCreated,
                    LastModified = disposalItem.LastModified,
                    CreatedBy = disposalItem.CreatedBy,
                    ModifiedBy = disposalItem.ModifiedBy
                };

                db.DisposalItems.Add(entity);
                db.SaveChanges();
                disposalItem.DisposalItemId = entity.DisposalItemId;
            }

            return Json(new[] { disposalItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DisposalItems_Update([DataSourceRequest]DataSourceRequest request, DisposalItem disposalItem)
        {
            //if (disposalItem.SalesPrice == 0)
            //    ModelState.AddModelError("zero", "Sales price can't be zero.");
            var tax = db.GLTaxRates.Find(disposalItem.GlTaxRateId);
            if (tax != null)
            {
                disposalItem.TaxAmount = disposalItem.SalesPrice * tax.CalculatedRate;
                disposalItem.Total = disposalItem.SalesPrice + disposalItem.TaxAmount;
            }
                      

            if (ModelState.IsValid)
            {
                var entity = new DisposalItem
                {
                    AssetDisposalId = disposalItem.AssetDisposalId,
                    DisposalItemId = disposalItem.DisposalItemId,
                    FixedAssetCategoryID = disposalItem.FixedAssetCategoryID,
                    FixedAssetSubCategoryID = disposalItem.FixedAssetSubCategoryID,
                    AssetRegistrationId = disposalItem.AssetRegistrationId,
                    BookValue = disposalItem.BookValue,
                    SalesPrice = disposalItem.SalesPrice,
                    GlTaxRateId = disposalItem.GlTaxRateId,
                    TaxAmount = disposalItem.TaxAmount,
                    Total = disposalItem.Total,
                    OrgBranchName = disposalItem.OrgBranchName,
                    DateCreated = disposalItem.DateCreated,
                    LastModified = disposalItem.LastModified,
                    CreatedBy = disposalItem.CreatedBy,
                    ModifiedBy = disposalItem.ModifiedBy
                };

                db.DisposalItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { disposalItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DisposalItems_Destroy([DataSourceRequest]DataSourceRequest request, DisposalItem disposalItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new DisposalItem
                {
                    AssetDisposalId = disposalItem.AssetDisposalId,
                    DisposalItemId = disposalItem.DisposalItemId,
                    FixedAssetCategoryID = disposalItem.FixedAssetCategoryID,
                    FixedAssetSubCategoryID = disposalItem.FixedAssetSubCategoryID,
                    AssetRegistrationId = disposalItem.AssetRegistrationId,
                    BookValue = disposalItem.BookValue,
                    SalesPrice = disposalItem.SalesPrice,
                    GlTaxRateId = disposalItem.GlTaxRateId,
                    TaxAmount = disposalItem.TaxAmount,
                    Total = disposalItem.Total,
                    OrgBranchName = disposalItem.OrgBranchName,
                    DateCreated = disposalItem.DateCreated,
                    LastModified = disposalItem.LastModified,
                    CreatedBy = disposalItem.CreatedBy,
                    ModifiedBy = disposalItem.ModifiedBy
                };

                db.DisposalItems.Attach(entity);
                db.DisposalItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { disposalItem }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
