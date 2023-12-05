using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Counts;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Counts
{
    
    public class FixedAssetCountItemController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        //private List<FixedAssetCountItem> _propertyItems = new List<FixedAssetCountItem>();

         [AuthorizeRoles(FixedAssetConstants.FixedAssetCount, FixedAssetConstants.FixedAssetCountApprover)]
        public ActionResult Index()
        {
            return View();
        }

        

         [AuthorizeRoles(FixedAssetConstants.FixedAssetCount, FixedAssetConstants.FixedAssetCountApprover)]
        public ActionResult FixedAssetCountItems_Read([DataSourceRequest]DataSourceRequest request , int id)
            {
                IQueryable<FixedAssetCountItem> fixedAssetCountItems = _db.FixedAssetCountItems.Where(x=>x.FixedAssetCountId==id).AsQueryable();
                DataSourceResult result = fixedAssetCountItems.ToDataSourceResult(request, fixedAssetCountItem => new {
                 
                    FixedAssetCountItemId = fixedAssetCountItem.FixedAssetCountItemId,
                    FixedAssetCountId = fixedAssetCountItem.FixedAssetCountId,
                    FixedAssetCategoryId = fixedAssetCountItem.FixedAssetCategoryId,
                    FixedAssetSubCategoryId = fixedAssetCountItem.FixedAssetSubCategoryId,
                    AssetRegistrationId = fixedAssetCountItem.AssetRegistrationId,
                    Count = fixedAssetCountItem.Count,
                    PreviousBalance = fixedAssetCountItem.PreviousBalance,
                    Value = fixedAssetCountItem.Value,
                    AssetStatus = fixedAssetCountItem.AssetStatus,
                    Status = fixedAssetCountItem.Status
                });

                return Json(result);
            }

         [AuthorizeRoles(FixedAssetConstants.FixedAssetCount)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetCountItems_Create([DataSourceRequest]DataSourceRequest request, FixedAssetCountItem fixedAssetCountItem,int id)
         {
             var record = _db.FixedAssetCounts.Find(id);
             var exist =
                 _db.FixedAssetCountItems
                     .Where(x => x.FixedAssetSubCategoryId == fixedAssetCountItem.FixedAssetSubCategoryId)
                     .FirstOrDefault(x => x.FixedAssetCountId == id);
             if (exist != null)
             {
                 ModelState.AddModelError("exist","Item: "+fixedAssetCountItem.FixedAssetSubCategoryId+ " already exists in this count sheet.");
             }

             var item = _db.AssetRegistrations.Find(fixedAssetCountItem.AssetRegistrationId);

            if (ModelState.IsValid)
            {
                if (item != null)
                {
                    var entity = new FixedAssetCountItem
                    {
                        FixedAssetCountId = id,
                        FixedAssetCategoryId = fixedAssetCountItem.FixedAssetCategoryId,
                        FixedAssetSubCategoryId = fixedAssetCountItem.FixedAssetSubCategoryId,
                        AssetRegistrationId = fixedAssetCountItem.AssetRegistrationId,
                        Count = fixedAssetCountItem.Count,
                        Value = fixedAssetCountItem.Value,
                        AssetStatus = fixedAssetCountItem.AssetStatus,
                        CreatedBy = User.Identity.Name,
                        Status = true,
                     
                        DateCreated = DateTime.Now
                    };

                    _db.FixedAssetCountItems.Add(entity);
                    _db.SaveChanges();
                    fixedAssetCountItem.FixedAssetCountItemId = entity.FixedAssetCountItemId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Asset Count Item", "Success", User.Identity.Name, $"created asset count item {entity.FixedAssetCountId} Tag {entity.AssetRegistrationId}", Modules.FixedAsset);
                    #endregion
                }

            }

            return Json(new[] { fixedAssetCountItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetCount)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetCountItems_Update([DataSourceRequest]DataSourceRequest request, FixedAssetCountItem fixedAssetCountItem)
        {
            var count = _db.FixedAssetCounts.Find(fixedAssetCountItem.FixedAssetCountId);
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetCountItem
                {
                    FixedAssetCountItemId = fixedAssetCountItem.FixedAssetCountItemId,
                    FixedAssetCategoryId = fixedAssetCountItem.FixedAssetCategoryId,
                    FixedAssetSubCategoryId = fixedAssetCountItem.FixedAssetSubCategoryId,
                    AssetRegistrationId = fixedAssetCountItem.AssetRegistrationId,
                    PreviousBalance = fixedAssetCountItem.PreviousBalance,
                    Count = fixedAssetCountItem.Count,
                    FixedAssetCountId = fixedAssetCountItem.FixedAssetCountId,
                    Status = true,
                    AssetStatus = fixedAssetCountItem.AssetStatus,

                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                };
                _db.FixedAssetCountItems.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Asset Count Item", "Success", User.Identity.Name, $"updated asset count item {entity.FixedAssetCountId} Tag {entity.AssetRegistrationId}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { fixedAssetCountItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetCount)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetCountItems_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetCountItem fixedAssetCountItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetCountItem
                {
                    FixedAssetCountItemId = fixedAssetCountItem.FixedAssetCountItemId,
                    FixedAssetCategoryId = fixedAssetCountItem.FixedAssetCategoryId,
                    FixedAssetSubCategoryId = fixedAssetCountItem.FixedAssetSubCategoryId,
                    Count = fixedAssetCountItem.Count,
                    Status = fixedAssetCountItem.Status,
                    AssetStatus = fixedAssetCountItem.AssetStatus,
                };

                _db.FixedAssetCountItems.Attach(entity);
                _db.FixedAssetCountItems.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted asset count item {fixedAssetCountItem.FixedAssetCountItemId } and Tag {fixedAssetCountItem.AssetRegistrationId}");
                #endregion
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted property count number " + fixedAssetCountItem.FixedAssetCountId + " " + fixedAssetCountItem.FixedAssetCount.Remark);
                #endregion
            }

            return Json(new[] { fixedAssetCountItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetCount)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetCount)]
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        public ActionResult Change(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var fixedAssetCountItem = _db.FixedAssetCountItems.Find(id);
            if (fixedAssetCountItem == null)
            {
                return HttpNotFound();
            }
            fixedAssetCountItem.Status = true;
            _db.Entry(fixedAssetCountItem).State = EntityState.Modified;
            _db.SaveChanges();
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
