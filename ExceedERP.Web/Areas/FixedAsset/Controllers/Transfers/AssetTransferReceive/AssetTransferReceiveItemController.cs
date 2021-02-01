using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.FixedAsset.Transfers;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Transfers.AssetTransferReceive
{
    
    public class AssetTransferReceiveItemController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult AssetTransferReceiveItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<AssetTransferReceiveItem> assetTransferReceiveItems = _db.AssetTransferReceiveItems.Where(i => i.AssetTransferReceiveId == id);
            DataSourceResult result = assetTransferReceiveItems.ToDataSourceResult(request, assetTransferReceiveItem => new {
                AssetTransferReceiveItemId = assetTransferReceiveItem.AssetTransferReceiveItemId,
                AssetTransferReceiveId = assetTransferReceiveItem.AssetTransferReceiveId,
                FixedAssetCategoryId = assetTransferReceiveItem.FixedAssetCategoryId,
                FixedAssetSubCategoryId = assetTransferReceiveItem.FixedAssetSubCategoryId,
                AssetRegistrationId = assetTransferReceiveItem.AssetRegistrationId,
                ItemName = assetTransferReceiveItem.ItemName,
                Unit = assetTransferReceiveItem.Unit,
                UnitPrice = assetTransferReceiveItem.UnitPrice,
                Total = assetTransferReceiveItem.Total,
                Name = assetTransferReceiveItem.Name,
                Size = assetTransferReceiveItem.Size,
                GrnNo = assetTransferReceiveItem.GrnNo,
                ModelNo = assetTransferReceiveItem.ModelNo,
                SerialNo = assetTransferReceiveItem.SerialNo,
                TagNo = assetTransferReceiveItem.TagNo,
                Quantity = assetTransferReceiveItem.Quantity,
                Remark = assetTransferReceiveItem.Remark,
                DateCreated = assetTransferReceiveItem.DateCreated,
                LastModified = assetTransferReceiveItem.LastModified,
                CreatedBy = assetTransferReceiveItem.CreatedBy,
                ModifiedBy = assetTransferReceiveItem.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssetTransferReceiveItems_Create(int id,[DataSourceRequest]DataSourceRequest request, AssetTransferReceiveItem assetTransferReceiveItem)
        {
            var asset = _db.AssetRegistrations.Find(assetTransferReceiveItem.AssetRegistrationId);
            if (asset != null)
            {
                assetTransferReceiveItem.TagNo = asset.Tag;
                assetTransferReceiveItem.ModelNo = asset.ModelNo;
                assetTransferReceiveItem.Size = asset.Size;
                assetTransferReceiveItem.SerialNo = asset.SerialNo;
                assetTransferReceiveItem.Unit = asset.Unit;
                assetTransferReceiveItem.Name = asset.Name;
                assetTransferReceiveItem.ItemName = asset.Name;
                assetTransferReceiveItem.GrnNo = asset.GRN;
                assetTransferReceiveItem.UnitPrice = asset.BookValue;
            }

            if (assetTransferReceiveItem.Quantity == 0)
                assetTransferReceiveItem.Quantity = 1;



            assetTransferReceiveItem.Total = assetTransferReceiveItem.Quantity * assetTransferReceiveItem.UnitPrice;

            //if (!FixedAssetHelper.IsAssetCurrentLocationStore(AssetTransferReceiveItem.AssetRegistrationId, AssetTransferReceiveItem.FixedAssetIssue.StoreCode))
            //{
            //    string message = "Asset with tag number " + AssetTransferReceiveItem.TagNo + " does not exist in store " + AssetTransferReceiveItem.FixedAssetIssue.StoreCode;
            //    ModelState.AddModelError("doesnotexist", message);

            //}
            if (ModelState.IsValid)
            {
                var entity = new AssetTransferReceiveItem
                {
                    AssetTransferReceiveId = id,
                    AssetTransferReceiveItemId = assetTransferReceiveItem.AssetTransferReceiveItemId,
                    FixedAssetCategoryId = assetTransferReceiveItem.FixedAssetCategoryId,
                    FixedAssetSubCategoryId = assetTransferReceiveItem.FixedAssetSubCategoryId,
                    AssetRegistrationId = assetTransferReceiveItem.AssetRegistrationId,
                    ItemName = assetTransferReceiveItem.ItemName,
                    Unit = assetTransferReceiveItem.Unit,
                    UnitPrice = assetTransferReceiveItem.UnitPrice,
                    Total = assetTransferReceiveItem.Total,
                    Name = assetTransferReceiveItem.Name,
                    Size = assetTransferReceiveItem.Size,
                    GrnNo = assetTransferReceiveItem.GrnNo,
                    ModelNo = assetTransferReceiveItem.ModelNo,
                    SerialNo = assetTransferReceiveItem.SerialNo,
                    TagNo = assetTransferReceiveItem.TagNo,
                    Quantity = assetTransferReceiveItem.Quantity,
                    Remark = assetTransferReceiveItem.Remark,
                    DateCreated = assetTransferReceiveItem.DateCreated,
                    LastModified = assetTransferReceiveItem.LastModified,
                    CreatedBy = assetTransferReceiveItem.CreatedBy,
                    ModifiedBy = assetTransferReceiveItem.ModifiedBy
                };

                _db.AssetTransferReceiveItems.Add(entity);
                _db.SaveChanges();
                assetTransferReceiveItem.AssetTransferReceiveItemId = entity.AssetTransferReceiveItemId;
            }

            return Json(new[] { assetTransferReceiveItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssetTransferReceiveItems_Update([DataSourceRequest]DataSourceRequest request, AssetTransferReceiveItem assetTransferReceiveItem, int id)
        {

            var asset = _db.AssetRegistrations.Find(assetTransferReceiveItem.AssetRegistrationId);
            if (asset != null)
            {
                assetTransferReceiveItem.TagNo = asset.Tag;
                assetTransferReceiveItem.ModelNo = asset.ModelNo;
                assetTransferReceiveItem.Size = asset.Size;
                assetTransferReceiveItem.SerialNo = asset.SerialNo;
                assetTransferReceiveItem.Unit = asset.Unit;
                assetTransferReceiveItem.Name = asset.Name;
                assetTransferReceiveItem.ItemName = asset.Name;
                assetTransferReceiveItem.GrnNo = asset.GRN;
                assetTransferReceiveItem.UnitPrice = asset.BookValue;
            }

            if (assetTransferReceiveItem.Quantity == 0)
                assetTransferReceiveItem.Quantity = 1;



            assetTransferReceiveItem.Total = assetTransferReceiveItem.Quantity * assetTransferReceiveItem.UnitPrice;
            //var asset = _db.AssetRegistrations.Find(AssetTransferReceiveItem.AssetRegistrationId);
            //if (asset != null)
            //{
            //    AssetTransferReceiveItem.TagNo = asset.Tag;
            //    AssetTransferReceiveItem.UnitPrice = asset.BookValue;
            //    AssetTransferReceiveItem.Total = asset.BookValue;
            //}

            //if (!FixedAssetHelper.IsAssetCurrentLocationStore(AssetTransferReceiveItem.AssetRegistrationId, AssetTransferReceiveItem.FixedAssetIssue.StoreCode))
            //{
            //    string message = "Asset with tag number " + AssetTransferReceiveItem.TagNo + " does not exist in store " + AssetTransferReceiveItem.FixedAssetIssue.StoreCode;
            //    ModelState.AddModelError("doesnotexist", message);

            //}

            if (ModelState.IsValid)
            {
                var entity = new AssetTransferReceiveItem
                {
                    AssetTransferReceiveId = assetTransferReceiveItem.AssetTransferReceiveId,
                    AssetTransferReceiveItemId = assetTransferReceiveItem.AssetTransferReceiveItemId,
                    FixedAssetCategoryId = assetTransferReceiveItem.FixedAssetCategoryId,
                    FixedAssetSubCategoryId = assetTransferReceiveItem.FixedAssetSubCategoryId,
                    AssetRegistrationId = assetTransferReceiveItem.AssetRegistrationId,
                    ItemName = assetTransferReceiveItem.ItemName,
                    Unit = assetTransferReceiveItem.Unit,
                    UnitPrice = assetTransferReceiveItem.UnitPrice,
                    Total = assetTransferReceiveItem.Total,
                    Name = assetTransferReceiveItem.Name,
                    Size = assetTransferReceiveItem.Size,
                    GrnNo = assetTransferReceiveItem.GrnNo,
                    ModelNo = assetTransferReceiveItem.ModelNo,
                    SerialNo = assetTransferReceiveItem.SerialNo,
                    TagNo = assetTransferReceiveItem.TagNo,
                    Quantity = assetTransferReceiveItem.Quantity,
                    Remark = assetTransferReceiveItem.Remark,
                    DateCreated = assetTransferReceiveItem.DateCreated,
                    LastModified = assetTransferReceiveItem.LastModified,
                    CreatedBy = assetTransferReceiveItem.CreatedBy,
                    ModifiedBy = assetTransferReceiveItem.ModifiedBy
                };

                _db.AssetTransferReceiveItems.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                
            }
        
            return Json(new[] { assetTransferReceiveItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssetTransferReceiveItems_Destroy([DataSourceRequest]DataSourceRequest request, AssetTransferReceiveItem assetTransferReceiveItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new AssetTransferReceiveItem
                {
                    AssetTransferReceiveId = assetTransferReceiveItem.AssetTransferReceiveId,
                    AssetTransferReceiveItemId = assetTransferReceiveItem.AssetTransferReceiveItemId,
                    FixedAssetCategoryId = assetTransferReceiveItem.FixedAssetCategoryId,
                    FixedAssetSubCategoryId = assetTransferReceiveItem.FixedAssetSubCategoryId,
                    AssetRegistrationId = assetTransferReceiveItem.AssetRegistrationId,
                    Size = assetTransferReceiveItem.Size,
                    ModelNo = assetTransferReceiveItem.ModelNo,
                    SerialNo = assetTransferReceiveItem.SerialNo,
                    TagNo = assetTransferReceiveItem.TagNo,
                    Quantity = assetTransferReceiveItem.Quantity,
                    Remark = assetTransferReceiveItem.Remark,
                    DateCreated = assetTransferReceiveItem.DateCreated,
                    LastModified = assetTransferReceiveItem.LastModified,
                    CreatedBy = assetTransferReceiveItem.CreatedBy,
                    ModifiedBy = assetTransferReceiveItem.ModifiedBy
                };

                _db.AssetTransferReceiveItems.Attach(entity);
                _db.AssetTransferReceiveItems.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { assetTransferReceiveItem }.ToDataSourceResult(request, ModelState));
        }
       
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
