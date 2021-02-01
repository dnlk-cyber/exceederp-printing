using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Requisitions;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.StoreRequisitions
{
    [Authorize]

    public class FixedAssetRequsitionItemPropertyReviewController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }


        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult FixedAssetRequsitionItemPropertyReviews_Read([DataSourceRequest]DataSourceRequest request , int id )
        {
            IQueryable<FixedAssetRequisitionItem> storerequisitionItems = _db.FixedAssetRequisitionItems.Where(p => p.FixedAssetRequisitionId == id);
            DataSourceResult result = storerequisitionItems.ToDataSourceResult(request, storeRequisitionItem => new {

                FixedAssetRequisitionItemId = storeRequisitionItem.FixedAssetRequisitionItemId,
                FixedAssetRequisitionId = storeRequisitionItem.FixedAssetRequisitionId,
                FixedAssetCategoryID = storeRequisitionItem.FixedAssetCategoryID,
                FixedAssetSubCategoryID = storeRequisitionItem.FixedAssetSubCategoryID,
                Name = storeRequisitionItem.Name,
                PartNo = storeRequisitionItem.PartNo,
                ModelNo = storeRequisitionItem.ModelNo,
                SerialNo = storeRequisitionItem.SerialNo,
                ChasisNo = storeRequisitionItem.ChasisNo,
                //AssetRegistrationId = storeRequisitionItem.AssetRegistrationId,
                UoM = storeRequisitionItem.UoM,
                RequestedQuantity = storeRequisitionItem.RequestedQuantity,
                ApprovedQuantity = storeRequisitionItem.ApprovedQuantity,
                IssuedQuantity = storeRequisitionItem.IssuedQuantity,
                PurchaseQuantity = storeRequisitionItem.PurchaseQuantity,
                GeneratePurchaseReq = storeRequisitionItem.GeneratePurchaseReq,
                IsApproved = storeRequisitionItem.IsApproved,
                ItemSpecification = storeRequisitionItem.ItemSpecification
            });

            return Json(result);
        }


        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
         public ActionResult FixedAssetRequsitionItemPropertyReviews_Create([DataSourceRequest]DataSourceRequest request, FixedAssetRequisitionItem storeRequisitionItem, int id)
        {
            if (storeRequisitionItem.PurchaseQuantity + storeRequisitionItem.IssuedQuantity >storeRequisitionItem.ApprovedQuantity)
            {
                ModelState.AddModelError("above","The sum of issue and purchase quantity cannot be more than the approved quantity");
            }

            if (storeRequisitionItem.PurchaseQuantity + storeRequisitionItem.IssuedQuantity <storeRequisitionItem.ApprovedQuantity)
            {
               // ModelState.AddModelError("below", "The sum of issue and purchase quantity are less than the approved quantity");
            }
            if (storeRequisitionItem.GeneratePurchaseReq && storeRequisitionItem.PurchaseQuantity==0)
            {
                ModelState.AddModelError("below", "Purchase quantity is not set");
            }
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetRequisitionItem
                {
                    FixedAssetRequisitionId = id,
                    FixedAssetCategoryID = storeRequisitionItem.FixedAssetCategoryID,
                    FixedAssetSubCategoryID = storeRequisitionItem.FixedAssetSubCategoryID,
                    Name = storeRequisitionItem.Name,
                    PartNo = storeRequisitionItem.PartNo,
                    ModelNo = storeRequisitionItem.ModelNo,
                    SerialNo = storeRequisitionItem.SerialNo,
                    ChasisNo = storeRequisitionItem.ChasisNo,
                    //AssetRegistrationId = storeRequisitionItem.AssetRegistrationId,
                    UoM = storeRequisitionItem.UoM,
                    RequestedQuantity = storeRequisitionItem.RequestedQuantity,
                    ApprovedQuantity = storeRequisitionItem.RequestedQuantity,
                    IssuedQuantity = storeRequisitionItem.RequestedQuantity,
                    PurchaseQuantity = storeRequisitionItem.PurchaseQuantity,
                    IsApproved = storeRequisitionItem.IsApproved,
                    ItemSpecification = storeRequisitionItem.ItemSpecification,
                    GeneratePurchaseReq = storeRequisitionItem.GeneratePurchaseReq,

                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Today,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Today,
                };

                _db.FixedAssetRequisitionItems.Add(entity);
                _db.SaveChanges();
                storeRequisitionItem.FixedAssetRequisitionItemId = entity.FixedAssetRequisitionItemId;
            }

            return Json(new[] { storeRequisitionItem }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
         public ActionResult FixedAssetRequsitionItemPropertyReviews_Update([DataSourceRequest]DataSourceRequest request, FixedAssetRequisitionItem storeRequisitionItem)
        {
            if (storeRequisitionItem.PurchaseQuantity + storeRequisitionItem.IssuedQuantity > storeRequisitionItem.ApprovedQuantity)
            {
                ModelState.AddModelError("above", "The sum of issue and purchase quantity cannot be more than the approved quantity");
            }

            //if (storeRequisitionItem.PurchaseQuantity + storeRequisitionItem.IssuedQuantity < storeRequisitionItem.ApprovedQuantity)
            //{
            //    ModelState.AddModelError("below", "The sum of issue and purchase quantity are less than the approved quantity");
            //}
            if (storeRequisitionItem.GeneratePurchaseReq && storeRequisitionItem.PurchaseQuantity == 0)
            {
                ModelState.AddModelError("below", "Purchase quantity is not set");
            }
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetRequisitionItem
                {

                    FixedAssetRequisitionId = storeRequisitionItem.FixedAssetRequisitionId,
                    FixedAssetRequisitionItemId = storeRequisitionItem.FixedAssetRequisitionItemId,
                    FixedAssetCategoryID = storeRequisitionItem.FixedAssetCategoryID,
                    FixedAssetSubCategoryID = storeRequisitionItem.FixedAssetSubCategoryID,
                    Name = storeRequisitionItem.Name,
                    PartNo = storeRequisitionItem.PartNo,
                    ModelNo = storeRequisitionItem.ModelNo,
                    SerialNo = storeRequisitionItem.SerialNo,
                    ChasisNo = storeRequisitionItem.ChasisNo,
                    //AssetRegistrationId = storeRequisitionItem.AssetRegistrationId,
                    UoM = storeRequisitionItem.UoM,
                    RequestedQuantity = storeRequisitionItem.RequestedQuantity,
                    ApprovedQuantity = storeRequisitionItem.ApprovedQuantity,
                    IssuedQuantity = storeRequisitionItem.IssuedQuantity,
                    PurchaseQuantity = storeRequisitionItem.PurchaseQuantity,
                    IsApproved = storeRequisitionItem.IsApproved,
                    ItemSpecification = storeRequisitionItem.ItemSpecification,
                    GeneratePurchaseReq = storeRequisitionItem.GeneratePurchaseReq,

                    DateCreated = storeRequisitionItem.DateCreated,
                    CreatedBy = storeRequisitionItem.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Today,
                };

                _db.FixedAssetRequisitionItems.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { storeRequisitionItem }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
         public ActionResult FixedAssetRequsitionItemPropertyReviews_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetRequisitionItem storeRequisitionItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetRequisitionItem
                {
                    FixedAssetRequisitionId = storeRequisitionItem.FixedAssetRequisitionId,
                    FixedAssetRequisitionItemId = storeRequisitionItem.FixedAssetRequisitionItemId,
                    FixedAssetCategoryID = storeRequisitionItem.FixedAssetCategoryID,
                    FixedAssetSubCategoryID = storeRequisitionItem.FixedAssetSubCategoryID,
                    Name = storeRequisitionItem.Name,
                    //AssetRegistrationId = storeRequisitionItem.AssetRegistrationId,
                    RequestedQuantity = storeRequisitionItem.RequestedQuantity,
                    ApprovedQuantity = storeRequisitionItem.ApprovedQuantity,
                    IssuedQuantity = storeRequisitionItem.IssuedQuantity,
                    PurchaseQuantity = storeRequisitionItem.PurchaseQuantity,
                    IsApproved = storeRequisitionItem.IsApproved,
                    ItemSpecification = storeRequisitionItem.ItemSpecification,
                    GeneratePurchaseReq = storeRequisitionItem.GeneratePurchaseReq,
                };

                _db.FixedAssetRequisitionItems.Attach(entity);
                _db.FixedAssetRequisitionItems.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { storeRequisitionItem }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
