using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset.Requisitions;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.StoreRequisitions
{
    [Authorize]

    public class FixedAssetRequisitionItemController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetRequistion, FixedAssetConstants.FixedAssetRequistionApprover, FixedAssetConstants.FixedAssetRequistionSecondApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetRequistion, FixedAssetConstants.FixedAssetRequistionApprover, FixedAssetConstants.FixedAssetRequistionSecondApprover)]
        public ActionResult FixedAssetRequisitionItems_Read([DataSourceRequest]DataSourceRequest request , int id )
        {
            IQueryable<FixedAssetRequisitionItem> storerequisitionItems = _db.FixedAssetRequisitionItems.Where(p => p.FixedAssetRequisitionId == id);
            DataSourceResult result = storerequisitionItems.ToDataSourceResult(request, storeRequisitionItem => new {
                
                FixedAssetRequisitionItemId = storeRequisitionItem.FixedAssetRequisitionItemId,
                FixedAssetRequisitionId = storeRequisitionItem.FixedAssetRequisitionId,
                FixedAssetCategoryID = storeRequisitionItem.FixedAssetCategoryID,
                FixedAssetSubCategoryID = storeRequisitionItem.FixedAssetSubCategoryID,
                Name = storeRequisitionItem.Name,
                FixedAssetNameID = storeRequisitionItem.FixedAssetNameID,
                PartNo = storeRequisitionItem.PartNo,
                ModelNo = storeRequisitionItem.ModelNo,
                SerialNo = storeRequisitionItem.SerialNo,
                ChasisNo = storeRequisitionItem.ChasisNo,
                EngineNo = storeRequisitionItem.EngineNo,
                TagNo = storeRequisitionItem.TagNo,
                UoM = storeRequisitionItem.UoM,
                RequestedQuantity = storeRequisitionItem.RequestedQuantity,
                ApprovedQuantity = storeRequisitionItem.ApprovedQuantity,
                IssuedQuantity = storeRequisitionItem.IssuedQuantity,
                PurchaseQuantity = storeRequisitionItem.PurchaseQuantity,
                IsApproved = storeRequisitionItem.IsApproved,
                ItemSpecification = storeRequisitionItem.ItemSpecification
            });

            return Json(result);
        }


        [AuthorizeRoles(FixedAssetConstants.FixedAssetRequistion)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetRequisitionItems_Create([DataSourceRequest]DataSourceRequest request, FixedAssetRequisitionItem storeRequisitionItem, int id)
        {
            storeRequisitionItem.IssuedQuantity = 0;
            storeRequisitionItem.PurchaseQuantity = 0;
            //Added to validate same entry
            var item = _db.FixedAssetRequisitionItems.Where(x => x.FixedAssetNameID == storeRequisitionItem.FixedAssetNameID && x.ItemSpecification == storeRequisitionItem.ItemSpecification && x.FixedAssetRequisitionId == id).ToList();
            if (item.Any())
            {
                ModelState.AddModelError("error", "Item with the same specification exists");
            }
;           if (storeRequisitionItem.ApprovedQuantity > storeRequisitionItem.RequestedQuantity)
            {
                ModelState.AddModelError("above","Approved quantity cannot be more than requested.");
            }
           
            //Added to validate 0 or negative quantity 
            if(storeRequisitionItem.RequestedQuantity <= 0)
            {
                ModelState.AddModelError("Error", "Requested QTY can't be 0 or negative value");
            }
            
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetRequisitionItem
                {
                    FixedAssetRequisitionId = id,
                    FixedAssetCategoryID = storeRequisitionItem.FixedAssetCategoryID,
                    FixedAssetSubCategoryID = storeRequisitionItem.FixedAssetSubCategoryID,
                    FixedAssetNameID = storeRequisitionItem.FixedAssetNameID,
                    Name =_db.FixedAssetSubCategoryNames.Find(storeRequisitionItem.FixedAssetNameID).Name,
                    PartNo = storeRequisitionItem.PartNo,
                    ModelNo = storeRequisitionItem.ModelNo,
                    SerialNo = storeRequisitionItem.SerialNo,
                    ChasisNo = storeRequisitionItem.ChasisNo,
                    EngineNo = storeRequisitionItem.EngineNo,
                    TagNo = storeRequisitionItem.TagNo,
                    UoM = storeRequisitionItem.UoM,
                    RequestedQuantity = storeRequisitionItem.RequestedQuantity,
                    ApprovedQuantity = storeRequisitionItem.RequestedQuantity,
                    IssuedQuantity = storeRequisitionItem.RequestedQuantity,
                    PurchaseQuantity = storeRequisitionItem.PurchaseQuantity,
                    IsApproved = storeRequisitionItem.IsApproved,
                    ItemSpecification = storeRequisitionItem.ItemSpecification,

                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Today,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Today,
                };

                _db.FixedAssetRequisitionItems.Add(entity);
                _db.SaveChanges();
                storeRequisitionItem.FixedAssetRequisitionItemId = entity.FixedAssetRequisitionItemId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetRequisitionItem", "Success", User.Identity.Name, $"created FixedAssetRequisitionItem {entity.FixedAssetRequisitionItemId} PartNo {entity.PartNo}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { storeRequisitionItem }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(FixedAssetConstants.FixedAssetRequistion)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetRequisitionItems_Update([DataSourceRequest]DataSourceRequest request, FixedAssetRequisitionItem storeRequisitionItem)
        {
            storeRequisitionItem.IssuedQuantity = 0;
            storeRequisitionItem.PurchaseQuantity = 0;
            if (storeRequisitionItem.ApprovedQuantity > storeRequisitionItem.RequestedQuantity)
            {
                ModelState.AddModelError("above", "Approved quantity cannot be more than requested.");
            }
            //Added to validate same entry
            var item = _db.FixedAssetRequisitionItems.Where(x => x.FixedAssetNameID == storeRequisitionItem.FixedAssetNameID && x.ItemSpecification == storeRequisitionItem.ItemSpecification && x.FixedAssetRequisitionId == storeRequisitionItem.FixedAssetRequisitionId && x.FixedAssetRequisitionItemId != storeRequisitionItem.FixedAssetRequisitionItemId).ToList();
            if (item.Any())
            {
                ModelState.AddModelError("error", "Item with the same specification exists");
            }
            //Added to validate 0 or negative quantity 
            if (storeRequisitionItem.RequestedQuantity <= 0)
            {
                ModelState.AddModelError("Error", "Requested QTY can't be 0 or negative value");
            }
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetRequisitionItem
                {

                    FixedAssetRequisitionId = storeRequisitionItem.FixedAssetRequisitionId,
                    FixedAssetRequisitionItemId = storeRequisitionItem.FixedAssetRequisitionItemId,
                    FixedAssetCategoryID = storeRequisitionItem.FixedAssetCategoryID,
                    FixedAssetSubCategoryID = storeRequisitionItem.FixedAssetSubCategoryID,
                    FixedAssetNameID = storeRequisitionItem.FixedAssetNameID,
                    Name = _db.FixedAssetSubCategoryNames.Find(storeRequisitionItem.FixedAssetNameID).Name,
                    PartNo = storeRequisitionItem.PartNo,
                    ModelNo = storeRequisitionItem.ModelNo,
                    SerialNo = storeRequisitionItem.SerialNo,
                    ChasisNo = storeRequisitionItem.ChasisNo,
                    EngineNo = storeRequisitionItem.EngineNo,
                    TagNo = storeRequisitionItem.TagNo,
                    UoM = storeRequisitionItem.UoM,
                    RequestedQuantity = storeRequisitionItem.RequestedQuantity,
                    ApprovedQuantity = storeRequisitionItem.RequestedQuantity,
                    IssuedQuantity = storeRequisitionItem.RequestedQuantity,
                    PurchaseQuantity = storeRequisitionItem.PurchaseQuantity,
                    ItemSpecification = storeRequisitionItem.ItemSpecification,

                    DateCreated = storeRequisitionItem.DateCreated,
                    CreatedBy = storeRequisitionItem.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Today,
                };

                _db.FixedAssetRequisitionItems.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetRequisitionItem", "Success", User.Identity.Name, $"Updated FixedAssetRequisitionItem {entity.FixedAssetRequisitionItemId} PartNo {entity.PartNo}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { storeRequisitionItem }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(FixedAssetConstants.FixedAssetRequistion)]
        [AcceptVerbs(HttpVerbs.Post)]
       public ActionResult FixedAssetRequisitionItems_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetRequisitionItem storeRequisitionItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetRequisitionItem
                {
                    FixedAssetRequisitionId = storeRequisitionItem.FixedAssetRequisitionId,
                    FixedAssetRequisitionItemId = storeRequisitionItem.FixedAssetRequisitionItemId,
                    FixedAssetCategoryID = storeRequisitionItem.FixedAssetCategoryID,
                    FixedAssetSubCategoryID = storeRequisitionItem.FixedAssetSubCategoryID,
                    FixedAssetNameID = storeRequisitionItem.FixedAssetNameID,
                    Name = storeRequisitionItem.Name,
                    RequestedQuantity = storeRequisitionItem.RequestedQuantity,
                    ApprovedQuantity = storeRequisitionItem.ApprovedQuantity,
                    IssuedQuantity = storeRequisitionItem.IssuedQuantity,
                    PurchaseQuantity = storeRequisitionItem.PurchaseQuantity,
                    IsApproved = storeRequisitionItem.IsApproved,
                    ItemSpecification = storeRequisitionItem.ItemSpecification
                };

                _db.FixedAssetRequisitionItems.Attach(entity);
                _db.FixedAssetRequisitionItems.Remove(entity);
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"delete FixedAssetRequisitionItem {storeRequisitionItem.FixedAssetRequisitionItemId} PartNo {storeRequisitionItem.PartNo}", Modules.FixedAsset);
                #endregion
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
