using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Returns;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Return
{
    
    public class FixedAssetReturnItemController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult FixedAssetReturnItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FixedAssetReturnItem> fixedAssetReturnItems = _db.FixedAssetReturnItems.Where(i => i.FixedAssetReturnId == id);
            DataSourceResult result = fixedAssetReturnItems.ToDataSourceResult(request, fixedAssetReturnItem => new {
                FixedAssetReturnItemId = fixedAssetReturnItem.FixedAssetReturnItemId,
                FixedAssetReturnId = fixedAssetReturnItem.FixedAssetReturnId,
                FixedAssetCategoryId = fixedAssetReturnItem.FixedAssetCategoryId,
                FixedAssetSubCategoryId = fixedAssetReturnItem.FixedAssetSubCategoryId,
                AssetRegistrationId = fixedAssetReturnItem.AssetRegistrationId,
                TagNo = fixedAssetReturnItem.TagNo,
                ItemName = fixedAssetReturnItem.ItemName,
                Quantity = fixedAssetReturnItem.Quantity,
                UnitPrice = fixedAssetReturnItem.UnitPrice,
                Total = fixedAssetReturnItem.Total,
                OrgBranchName = fixedAssetReturnItem.OrgBranchName,
                DateCreated = fixedAssetReturnItem.DateCreated,
                LastModified = fixedAssetReturnItem.LastModified,
                CreatedBy = fixedAssetReturnItem.CreatedBy,
                ModifiedBy = fixedAssetReturnItem.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReturnItems_Create(int id,[DataSourceRequest]DataSourceRequest request, FixedAssetReturnItem fixedAssetReturnItem)
        {

            var asset = _db.AssetRegistrations.Find(fixedAssetReturnItem.AssetRegistrationId);

            if (asset == null)
                ModelState.AddModelError("doesnotexist", "Please select an asset.");
            else
            {
                fixedAssetReturnItem.ItemName = asset.Name;
                fixedAssetReturnItem.TagNo = asset.Tag;
                fixedAssetReturnItem.UnitPrice = asset.BookValue;
                fixedAssetReturnItem.Total = fixedAssetReturnItem.Quantity * asset.BookValue;

            }

            if (ModelState.IsValid)
            {
                var entity = new FixedAssetReturnItem
                {
                    FixedAssetReturnId = id,
                    FixedAssetCategoryId = fixedAssetReturnItem.FixedAssetCategoryId,
                    FixedAssetSubCategoryId = fixedAssetReturnItem.FixedAssetSubCategoryId,
                    AssetRegistrationId = fixedAssetReturnItem.AssetRegistrationId,
                    ItemName = fixedAssetReturnItem.ItemName,
                    TagNo = fixedAssetReturnItem.TagNo,
                    Quantity = fixedAssetReturnItem.Quantity,
                    UnitPrice = fixedAssetReturnItem.UnitPrice,
                    Total = fixedAssetReturnItem.Total,
                    OrgBranchName = fixedAssetReturnItem.OrgBranchName,
                    DateCreated = fixedAssetReturnItem.DateCreated,
                    LastModified = fixedAssetReturnItem.LastModified,
                    CreatedBy = fixedAssetReturnItem.CreatedBy,
                    ModifiedBy = fixedAssetReturnItem.ModifiedBy
                };

                _db.FixedAssetReturnItems.Add(entity);
                _db.SaveChanges();
                fixedAssetReturnItem.FixedAssetReturnItemId = entity.FixedAssetReturnItemId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetReturnItem", "Success", User.Identity.Name, $"created FixedAssetReturnItem {entity.AssetRegistrationId} TagNo {entity.TagNo}", Modules.FixedAsset);
                #endregion

            }

            return Json(new[] { fixedAssetReturnItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReturnItems_Update([DataSourceRequest]DataSourceRequest request, FixedAssetReturnItem fixedAssetReturnItem, int id)
        {
            var asset = _db.AssetRegistrations.Find(fixedAssetReturnItem.AssetRegistrationId);

            if (asset == null)
                ModelState.AddModelError("doesnotexist", "Please select an asset.");
            else
            {
                fixedAssetReturnItem.ItemName = asset.Name;
                fixedAssetReturnItem.TagNo = asset.Tag;
                fixedAssetReturnItem.UnitPrice = asset.BookValue;
                fixedAssetReturnItem.Total = fixedAssetReturnItem.Quantity * asset.BookValue;

            }

            if (ModelState.IsValid)
            {
                var entity = new FixedAssetReturnItem
                {
                    FixedAssetReturnItemId = fixedAssetReturnItem.FixedAssetReturnItemId,
                    FixedAssetReturnId = fixedAssetReturnItem.FixedAssetReturnId,
                    FixedAssetCategoryId = fixedAssetReturnItem.FixedAssetCategoryId,
                    FixedAssetSubCategoryId = fixedAssetReturnItem.FixedAssetSubCategoryId,
                    AssetRegistrationId = fixedAssetReturnItem.AssetRegistrationId,
                    ItemName = fixedAssetReturnItem.ItemName,
                    TagNo = fixedAssetReturnItem.TagNo,
                    Quantity = fixedAssetReturnItem.Quantity,
                    UnitPrice = fixedAssetReturnItem.UnitPrice,
                    Total = fixedAssetReturnItem.Total,
                    OrgBranchName = fixedAssetReturnItem.OrgBranchName,
                };

                _db.FixedAssetReturnItems.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetReturnItem", "Success", User.Identity.Name, $"updated FixedAssetReturnItem {entity.AssetRegistrationId} TagNo {entity.TagNo}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { fixedAssetReturnItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReturnItems_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetReturnItem fixedAssetReturnItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetReturnItem
                {
                    FixedAssetReturnItemId = fixedAssetReturnItem.FixedAssetReturnItemId,
                    FixedAssetCategoryId = fixedAssetReturnItem.FixedAssetCategoryId,
                    FixedAssetSubCategoryId = fixedAssetReturnItem.FixedAssetSubCategoryId,
                    AssetRegistrationId = fixedAssetReturnItem.AssetRegistrationId,
                    TagNo = fixedAssetReturnItem.TagNo,
                    Quantity = fixedAssetReturnItem.Quantity,
                    UnitPrice = fixedAssetReturnItem.UnitPrice,
                    Total = fixedAssetReturnItem.Total,
                    OrgBranchName = fixedAssetReturnItem.OrgBranchName,
                    DateCreated = fixedAssetReturnItem.DateCreated,
                    LastModified = fixedAssetReturnItem.LastModified,
                    CreatedBy = fixedAssetReturnItem.CreatedBy,
                    ModifiedBy = fixedAssetReturnItem.ModifiedBy
                };

                _db.FixedAssetReturnItems.Attach(entity);
                _db.FixedAssetReturnItems.Remove(entity);
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"delete FixedAssetReturnItem {fixedAssetReturnItem.AssetRegistrationId} TagNo {fixedAssetReturnItem.TagNo}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { fixedAssetReturnItem }.ToDataSourceResult(request, ModelState));
        }
       
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
