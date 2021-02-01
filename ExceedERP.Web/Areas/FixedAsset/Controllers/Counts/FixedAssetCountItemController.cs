using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Counts;
using ExceedERP.Core.Domain.FixedAsset.Setting;
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
                    Price = fixedAssetCountItem.Price,
                    Location = fixedAssetCountItem.Location,
                    ItemName = fixedAssetCountItem.ItemName,
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
                        Price = fixedAssetCountItem.Price,
                        CreatedBy = User.Identity.Name,
                        Status = true,
                        Location = fixedAssetCountItem.Location,
                        DateCreated = DateTime.Now
                    };

                    _db.FixedAssetCountItems.Add(entity);
                    _db.SaveChanges();
                    fixedAssetCountItem.FixedAssetCountItemId = entity.FixedAssetCountItemId;
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
                    Location = fixedAssetCountItem.Location,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                };
                _db.FixedAssetCountItems.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
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
                    Status = fixedAssetCountItem.Status
                };

                _db.FixedAssetCountItems.Attach(entity);
                _db.FixedAssetCountItems.Remove(entity);
                _db.SaveChanges();
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
