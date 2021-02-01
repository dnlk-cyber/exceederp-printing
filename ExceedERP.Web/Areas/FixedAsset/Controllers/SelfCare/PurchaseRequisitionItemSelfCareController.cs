using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.SelfCare
{

    public class PurchaseRequisitionItemSelfCareController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetRequistionApprover, FixedAssetConstants.FixedAssetRequistion)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetRequistionApprover, FixedAssetConstants.FixedAssetRequistion)]
        public ActionResult PurchaseRequisitionItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<PurchaseRequisitionItem> purchaserequisitionItems = db.PurchaseRequisitionItems.Where(pri => pri.PurchaseRequisitionID == id);
            DataSourceResult result = purchaserequisitionItems.ToDataSourceResult(request, purchaseRequisitionItem => new
            {
                PurchaseRequisitionItemID = purchaseRequisitionItem.PurchaseRequisitionItemID,
                PurchaseRequisitionID = purchaseRequisitionItem.PurchaseRequisitionID,
                CatCode = purchaseRequisitionItem.CatCode,// + "-" + purchaseRequisitionItem.CategoryName,
                CategoryName = purchaseRequisitionItem.CategoryName,
                ItmCode = purchaseRequisitionItem.ItmCode,// + "-" + purchaseRequisitionItem.ItemName,
                ItemName = purchaseRequisitionItem.ItemName,
                PartNo = purchaseRequisitionItem.PartNo,
                ModelNo = purchaseRequisitionItem.ModelNo,
                SerialNo = purchaseRequisitionItem.SerialNo,
                ChasisNo = purchaseRequisitionItem.ChasisNo,
                RequestedQuantity = purchaseRequisitionItem.RequestedQuantity,
                Quantity = purchaseRequisitionItem.Quantity,
                IsApproved = purchaseRequisitionItem.IsApproved,
                ItemSpecification = purchaseRequisitionItem.ItemSpecification,
                Name = purchaseRequisitionItem.Name,
                UoM = purchaseRequisitionItem.UoM,
                ItemType = purchaseRequisitionItem.ItemType,
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetRequistion)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionItems_Create([DataSourceRequest]DataSourceRequest request, PurchaseRequisitionItem purchaseRequisitionItem, int id)
        {
            purchaseRequisitionItem.Quantity = purchaseRequisitionItem.RequestedQuantity;

            #region data 

            var assetCategory = db.FixedAssetCategories
                .FirstOrDefault(x => x.FixedAssetCategoryID.ToString() == purchaseRequisitionItem.CatCode);
            if (assetCategory != null)
            {
                purchaseRequisitionItem.CategoryName = assetCategory.CategoryName;
            }

            var assetSubCategory = db.FixedAssetSubCategories
                .FirstOrDefault(x => x.FixedAssetSubCategoryID.ToString() == purchaseRequisitionItem.ItmCode);
            if (assetSubCategory != null)
            {
                purchaseRequisitionItem.ItemName = assetSubCategory.SubCategoryName;
            }

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisitionItem
                {
                    PurchaseRequisitionID = id,
                    CatCode = purchaseRequisitionItem.CatCode,
                    CategoryName = purchaseRequisitionItem.CategoryName,
                    ItmCode = purchaseRequisitionItem.ItmCode,
                    ItemName = purchaseRequisitionItem.ItemName,
                    PartNo = purchaseRequisitionItem.PartNo,
                    ModelNo = purchaseRequisitionItem.ModelNo,
                    SerialNo = purchaseRequisitionItem.SerialNo,
                    ChasisNo = purchaseRequisitionItem.ChasisNo,
                    RequestedQuantity = purchaseRequisitionItem.RequestedQuantity,
                    Quantity = purchaseRequisitionItem.Quantity,
                    IsApproved = purchaseRequisitionItem.IsApproved,
                    ItemSpecification = purchaseRequisitionItem.ItemSpecification,
                    Name = purchaseRequisitionItem.Name,
                    UoM = purchaseRequisitionItem.UoM,
                    ItemType = PurchaseItemType.FixedAsset,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                db.PurchaseRequisitionItems.Add(entity);
                db.SaveChanges();
                purchaseRequisitionItem.PurchaseRequisitionItemID = entity.PurchaseRequisitionItemID;
            }

            return Json(new[] { purchaseRequisitionItem }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(FixedAssetConstants.FixedAssetRequistion)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionItems_Update([DataSourceRequest]DataSourceRequest request, PurchaseRequisitionItem purchaseRequisitionItem)
        {
            if (purchaseRequisitionItem.Quantity > purchaseRequisitionItem.RequestedQuantity)
                ModelState.AddModelError("qty", "Final quantity can not be more than requested quanity");

            #region data 

            var assetCategory = db.FixedAssetCategories
                .FirstOrDefault(x => x.FixedAssetCategoryID.ToString() == purchaseRequisitionItem.CatCode);
            if (assetCategory != null)
            {
                purchaseRequisitionItem.CategoryName = assetCategory.CategoryName;
            }

            var assetSubCategory = db.FixedAssetSubCategories
                .FirstOrDefault(x => x.FixedAssetSubCategoryID.ToString() == purchaseRequisitionItem.ItmCode);
            if (assetSubCategory != null)
            {
                purchaseRequisitionItem.ItemName = assetSubCategory.SubCategoryName;
            }

            #endregion

            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisitionItem
                {
                    PurchaseRequisitionID = purchaseRequisitionItem.PurchaseRequisitionID,
                    PurchaseRequisitionItemID = purchaseRequisitionItem.PurchaseRequisitionItemID,
                    CatCode = purchaseRequisitionItem.CatCode,
                    CategoryName = purchaseRequisitionItem.CategoryName,
                    ItmCode = purchaseRequisitionItem.ItmCode,
                    ItemName = purchaseRequisitionItem.ItemName,
                    PartNo = purchaseRequisitionItem.PartNo,
                    ModelNo = purchaseRequisitionItem.ModelNo,
                    SerialNo = purchaseRequisitionItem.SerialNo,
                    ChasisNo = purchaseRequisitionItem.ChasisNo,
                    RequestedQuantity = purchaseRequisitionItem.RequestedQuantity,
                    Quantity = purchaseRequisitionItem.Quantity,
                    IsApproved = purchaseRequisitionItem.IsApproved,
                    ItemSpecification = purchaseRequisitionItem.ItemSpecification,
                    Name = purchaseRequisitionItem.Name,
                    UoM = purchaseRequisitionItem.UoM,
                    ItemType = purchaseRequisitionItem.ItemType,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Today,
                };

                db.PurchaseRequisitionItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { purchaseRequisitionItem }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(FixedAssetConstants.FixedAssetRequistion)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionItems_Destroy([DataSourceRequest]DataSourceRequest request, PurchaseRequisitionItem purchaseRequisitionItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisitionItem
                {
                    PurchaseRequisitionItemID = purchaseRequisitionItem.PurchaseRequisitionItemID,
                    CatCode = purchaseRequisitionItem.CatCode,
                    CategoryName = purchaseRequisitionItem.CategoryName,
                    ItmCode = purchaseRequisitionItem.ItmCode,
                    ItemName = purchaseRequisitionItem.ItemName,
                    PartNo = purchaseRequisitionItem.PartNo,
                    RequestedQuantity = purchaseRequisitionItem.RequestedQuantity,
                    Quantity = purchaseRequisitionItem.Quantity,
                    IsApproved = purchaseRequisitionItem.IsApproved,
                    ItemSpecification = purchaseRequisitionItem.ItemSpecification,
                    UoM = purchaseRequisitionItem.UoM,
                    ItemType = purchaseRequisitionItem.ItemType,
                };

                db.PurchaseRequisitionItems.Attach(entity);
                db.PurchaseRequisitionItems.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted purchase requisition item " + purchaseRequisitionItem.PurchaseRequisitionID + " " + purchaseRequisitionItem.ItemName);
                #endregion
            }

            return Json(new[] { purchaseRequisitionItem }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
