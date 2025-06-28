using System;
using ExceedERP.Web.Helpers;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.CRM.TenderManagement;

namespace ExceedERP.Web.Areas.CRM.Controllers.TenderManagement
{
	[AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class TenderFollowupLineItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
       
        public ActionResult Data_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<TenderFollowupLineItem> data = db.TenderFollowupLineItem.Where(l => l.TenderFollowupId == id);
            DataSourceResult result = data.ToDataSourceResult(request, tenderFollowupLineItem => new TenderFollowupLineItem{
                TenderFollowupLineItemId = tenderFollowupLineItem.TenderFollowupLineItemId,
                TenderFollowupId = tenderFollowupLineItem.TenderFollowupId,
                ItemCategoryCode = tenderFollowupLineItem.ItemCategoryCode,
                ItemCode = tenderFollowupLineItem.ItemCode,
                Quantity = tenderFollowupLineItem.Quantity,
                IsExternalProduct = tenderFollowupLineItem.IsExternalProduct,
                Category = tenderFollowupLineItem.Category,
                Product = tenderFollowupLineItem.Product,

                CategoryName = tenderFollowupLineItem.IsExternalProduct? tenderFollowupLineItem.Category : GetCategoryName(tenderFollowupLineItem.ItemCategoryCode),
                ItemName = tenderFollowupLineItem.IsExternalProduct?tenderFollowupLineItem.Product: GetItemName(tenderFollowupLineItem.ItemCode),

                CreatedBy = tenderFollowupLineItem.CreatedBy,
                DateCreated = tenderFollowupLineItem.DateCreated,
                LastModified = tenderFollowupLineItem.LastModified,
                ModifiedBy = tenderFollowupLineItem.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Data_Create([DataSourceRequest]DataSourceRequest request, TenderFollowupLineItem tenderFollowupLineItem, int id)
        {
 
            if (ModelState.IsValid)
            {
                var entity = new TenderFollowupLineItem
                {
                    TenderFollowupLineItemId = tenderFollowupLineItem.TenderFollowupLineItemId,
                    TenderFollowupId = id,
                    ItemCategoryCode = tenderFollowupLineItem.ItemCategoryCode,
                    ItemCode = tenderFollowupLineItem.ItemCode,
                    Quantity = tenderFollowupLineItem.Quantity,
                    IsExternalProduct = tenderFollowupLineItem.IsExternalProduct,
                    Category = tenderFollowupLineItem.Category,
                    Product = tenderFollowupLineItem.Product,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                };

                db.TenderFollowupLineItem.Add(entity);
                db.SaveChanges();
                tenderFollowupLineItem.TenderFollowupLineItemId = entity.TenderFollowupLineItemId;
            }

            return Json(new[] { tenderFollowupLineItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Data_Update([DataSourceRequest]DataSourceRequest request, TenderFollowupLineItem tenderFollowupLineItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new TenderFollowupLineItem
                {
                    TenderFollowupLineItemId = tenderFollowupLineItem.TenderFollowupLineItemId,
                    TenderFollowupId = tenderFollowupLineItem.TenderFollowupId,
                    ItemCategoryCode = tenderFollowupLineItem.ItemCategoryCode,
                    ItemCode = tenderFollowupLineItem.ItemCode,
                    Quantity = tenderFollowupLineItem.Quantity,
                    IsExternalProduct = tenderFollowupLineItem.IsExternalProduct,
                    Category = tenderFollowupLineItem.Category,
                    Product = tenderFollowupLineItem.Product,

                    CreatedBy = tenderFollowupLineItem.CreatedBy,
                    DateCreated = tenderFollowupLineItem.DateCreated,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                db.TenderFollowupLineItem.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { tenderFollowupLineItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Data_Destroy([DataSourceRequest]DataSourceRequest request, TenderFollowupLineItem tenderFollowupLineItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new TenderFollowupLineItem
                {
                    TenderFollowupLineItemId = tenderFollowupLineItem.TenderFollowupLineItemId,
                    TenderFollowupId = tenderFollowupLineItem.TenderFollowupId,
                    ItemCategoryCode = tenderFollowupLineItem.ItemCategoryCode,
                    ItemCode = tenderFollowupLineItem.ItemCode,
                    Quantity = tenderFollowupLineItem.Quantity,
                    IsExternalProduct = tenderFollowupLineItem.IsExternalProduct,
                    Category = tenderFollowupLineItem.Category,
                    Product = tenderFollowupLineItem.Product,

                    CreatedBy = tenderFollowupLineItem.CreatedBy,
                    DateCreated = tenderFollowupLineItem.DateCreated,
                    LastModified = tenderFollowupLineItem.LastModified,
                    ModifiedBy = tenderFollowupLineItem.ModifiedBy,
                };

                db.TenderFollowupLineItem.Attach(entity);
                db.TenderFollowupLineItem.Remove(entity);
                db.SaveChanges();
		
            }

            return Json(new[] { tenderFollowupLineItem }.ToDataSourceResult(request, ModelState));
        }

        private string GetCategoryName(string code)
        {
            return db.ItemCategorys.Find(code)?.Name;
        }
        private string GetItemName(string code)
        {
            return db.InventoryItems.Find(code)?.Name;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
