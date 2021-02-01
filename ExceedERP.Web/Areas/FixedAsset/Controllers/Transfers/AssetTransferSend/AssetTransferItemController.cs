using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.FixedAsset.Transfers;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Transfers.AssetTransferSend
{
    
    public class AssetTransferItemController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult AssetTransferItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<AssetTransferItem> assetTransferItems = _db.AssetTransferItems.Where(i => i.AssetTransferId == id);
            DataSourceResult result = assetTransferItems.ToDataSourceResult(request, assetTransferItem => new {
                AssetTransferItemId = assetTransferItem.AssetTransferItemId,
                AssetTransferId = assetTransferItem.AssetTransferId,
                FixedAssetCategoryId = assetTransferItem.FixedAssetCategoryId,
                FixedAssetSubCategoryId = assetTransferItem.FixedAssetSubCategoryId,
                AssetRegistrationId = assetTransferItem.AssetRegistrationId,
                ItemName = assetTransferItem.ItemName,
                Unit = assetTransferItem.Unit,
                UnitPrice = assetTransferItem.UnitPrice,
                Total = assetTransferItem.Total,
                Name = assetTransferItem.Name,
                Size = assetTransferItem.Size,
                GrnNo = assetTransferItem.GrnNo,
                ModelNo = assetTransferItem.ModelNo,
                SerialNo = assetTransferItem.SerialNo,
                TagNo = assetTransferItem.TagNo,
                Quantity = assetTransferItem.Quantity,
                Remark = assetTransferItem.Remark,
                DateCreated = assetTransferItem.DateCreated,
                LastModified = assetTransferItem.LastModified,
                CreatedBy = assetTransferItem.CreatedBy,
                ModifiedBy = assetTransferItem.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssetTransferItems_Create(int id,[DataSourceRequest]DataSourceRequest request, AssetTransferItem assetTransferItem)
        {
            var asset = _db.AssetRegistrations.Find(assetTransferItem.AssetRegistrationId);
            if (asset != null)
            {
                assetTransferItem.TagNo = asset.Tag;
                assetTransferItem.ModelNo = asset.ModelNo;
                assetTransferItem.Size = asset.Size;
                assetTransferItem.SerialNo = asset.SerialNo;
                assetTransferItem.Unit = asset.Unit;
                assetTransferItem.Name = asset.Name;
                assetTransferItem.ItemName = asset.Name;
                assetTransferItem.GrnNo = asset.GRN;
                assetTransferItem.UnitPrice = asset.BookValue;

            }

            if (assetTransferItem.Quantity == 0)
                assetTransferItem.Quantity = 1;



            assetTransferItem.Total = assetTransferItem.Quantity * assetTransferItem.UnitPrice;

            //if (!FixedAssetHelper.IsAssetCurrentLocationStore(assetTransferItem.AssetRegistrationId, assetTransferItem.FixedAssetIssue.StoreCode))
            //{
            //    string message = "Asset with tag number " + assetTransferItem.TagNo + " does not exist in store " + assetTransferItem.FixedAssetIssue.StoreCode;
            //    ModelState.AddModelError("doesnotexist", message);

            //}
            if (ModelState.IsValid)
            {
                var entity = new AssetTransferItem
                {
                    AssetTransferId = id,
                    AssetTransferItemId = assetTransferItem.AssetTransferItemId,
                    FixedAssetCategoryId = assetTransferItem.FixedAssetCategoryId,
                    FixedAssetSubCategoryId = assetTransferItem.FixedAssetSubCategoryId,
                    AssetRegistrationId = assetTransferItem.AssetRegistrationId,
                    ItemName = assetTransferItem.ItemName,
                    Unit = assetTransferItem.Unit,
                    UnitPrice = assetTransferItem.UnitPrice,
                    Total = assetTransferItem.Total,
                    Name = assetTransferItem.Name,
                    Size = assetTransferItem.Size,
                    GrnNo = assetTransferItem.GrnNo,
                    ModelNo = assetTransferItem.ModelNo,
                    SerialNo = assetTransferItem.SerialNo,
                    TagNo = assetTransferItem.TagNo,
                    Quantity = assetTransferItem.Quantity,
                    Remark = assetTransferItem.Remark,
                    DateCreated = assetTransferItem.DateCreated,
                    LastModified = assetTransferItem.LastModified,
                    CreatedBy = assetTransferItem.CreatedBy,
                    ModifiedBy = assetTransferItem.ModifiedBy
                };

                _db.AssetTransferItems.Add(entity);
                _db.SaveChanges();
                assetTransferItem.AssetTransferItemId = entity.AssetTransferItemId;
            }

            return Json(new[] { assetTransferItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssetTransferItems_Update([DataSourceRequest]DataSourceRequest request, AssetTransferItem assetTransferItem, int id)
        {

            var asset = _db.AssetRegistrations.Find(assetTransferItem.AssetRegistrationId);
            if (asset != null)
            {
                assetTransferItem.TagNo = asset.Tag;
                assetTransferItem.ModelNo = asset.ModelNo;
                assetTransferItem.Size = asset.Size;
                assetTransferItem.SerialNo = asset.SerialNo;
                assetTransferItem.Unit = asset.Unit;
                assetTransferItem.Name = asset.Name;
                assetTransferItem.ItemName = asset.Name;
                assetTransferItem.GrnNo = asset.GRN;
                assetTransferItem.UnitPrice = asset.BookValue;
            }

            if (assetTransferItem.Quantity == 0)
                assetTransferItem.Quantity = 1;



            assetTransferItem.Total = assetTransferItem.Quantity * assetTransferItem.UnitPrice;
            //var asset = _db.AssetRegistrations.Find(assetTransferItem.AssetRegistrationId);
            //if (asset != null)
            //{
            //    assetTransferItem.TagNo = asset.Tag;
            //    assetTransferItem.UnitPrice = asset.BookValue;
            //    assetTransferItem.Total = asset.BookValue;
            //}

            //if (!FixedAssetHelper.IsAssetCurrentLocationStore(assetTransferItem.AssetRegistrationId, assetTransferItem.FixedAssetIssue.StoreCode))
            //{
            //    string message = "Asset with tag number " + assetTransferItem.TagNo + " does not exist in store " + assetTransferItem.FixedAssetIssue.StoreCode;
            //    ModelState.AddModelError("doesnotexist", message);

            //}

            if (ModelState.IsValid)
            {
                var entity = new AssetTransferItem
                {
                    AssetTransferId = assetTransferItem.AssetTransferId,
                    AssetTransferItemId = assetTransferItem.AssetTransferItemId,
                    FixedAssetCategoryId = assetTransferItem.FixedAssetCategoryId,
                    FixedAssetSubCategoryId = assetTransferItem.FixedAssetSubCategoryId,
                    AssetRegistrationId = assetTransferItem.AssetRegistrationId,
                    ItemName = assetTransferItem.ItemName,
                    Unit = assetTransferItem.Unit,
                    UnitPrice = assetTransferItem.UnitPrice,
                    Total = assetTransferItem.Total,
                    Name = assetTransferItem.Name,
                    Size = assetTransferItem.Size,
                    GrnNo = assetTransferItem.GrnNo,
                    ModelNo = assetTransferItem.ModelNo,
                    SerialNo = assetTransferItem.SerialNo,
                    TagNo = assetTransferItem.TagNo,
                    Quantity = assetTransferItem.Quantity,
                    Remark = assetTransferItem.Remark,
                    DateCreated = assetTransferItem.DateCreated,
                    LastModified = assetTransferItem.LastModified,
                    CreatedBy = assetTransferItem.CreatedBy,
                    ModifiedBy = assetTransferItem.ModifiedBy
                };

                _db.AssetTransferItems.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                
            }
        
            return Json(new[] { assetTransferItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssetTransferItems_Destroy([DataSourceRequest]DataSourceRequest request, AssetTransferItem assetTransferItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new AssetTransferItem
                {
                    AssetTransferId = assetTransferItem.AssetTransferId,
                    AssetTransferItemId = assetTransferItem.AssetTransferItemId,
                    FixedAssetCategoryId = assetTransferItem.FixedAssetCategoryId,
                    FixedAssetSubCategoryId = assetTransferItem.FixedAssetSubCategoryId,
                    AssetRegistrationId = assetTransferItem.AssetRegistrationId,
                    Size = assetTransferItem.Size,
                    ModelNo = assetTransferItem.ModelNo,
                    SerialNo = assetTransferItem.SerialNo,
                    TagNo = assetTransferItem.TagNo,
                    Quantity = assetTransferItem.Quantity,
                    DateCreated = assetTransferItem.DateCreated,
                    LastModified = assetTransferItem.LastModified,
                    CreatedBy = assetTransferItem.CreatedBy,
                    ModifiedBy = assetTransferItem.ModifiedBy
                };

                _db.AssetTransferItems.Attach(entity);
                _db.AssetTransferItems.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { assetTransferItem }.ToDataSourceResult(request, ModelState));
        }
       
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
