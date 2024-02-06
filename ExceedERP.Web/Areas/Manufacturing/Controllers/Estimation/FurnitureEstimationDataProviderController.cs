using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.Estimation
{
    public class FurnitureEstimationDataProviderController : Controller
    {
        ExceedDbContext _db = new ExceedDbContext();
        // GET: PrintingEstimation/PrintingEstimationDataProvider
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetItemCategory()
        {
            var itemCategory = _db.ItemCategorys
                .Where(x => x.HaveParent == false)
                .ToList();//exclude sub categories
            return Json(itemCategory.Select(p => new
            {
                Name = p.ItemCategoryCode + "-" + p.Name,
                ItemCategoryCode = p.ItemCategoryCode
            }),
                JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetItems1(string ItemCategoryCode, string text)
        {
            var items = _db.InventoryItems.Where(x => x.Enable).Where(i => i.ItemCategoryCode == ItemCategoryCode).OrderBy(x => x.Name).AsQueryable();
            if (!string.IsNullOrEmpty(text))
            {
                items = items.Where(p => p.Name.ToUpper().Contains(text.ToUpper()) || p.ItemCode.Contains(text.ToUpper()));
            }
            return Json(items.Select(i => new
            {
                Name = i.ItemCode + "-" + i.Name,
                ItemCode = i.ItemCode

            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllStores()
        {
            var stores = _db.InventoryStores.ToList().Select(s => new
            {

                s.StoreCode,
                s.Name

            });
            return Json(stores, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFurnitureCategory()
        {
            var category = _db.FurnitureCategories.ToList().Select(s => new
            {

                s.FurnitureCategoryId,
                s.Name

            });
            return Json(category, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetItemDetail(string itemCode, string storeCode)
        {
            var item = _db.StoreItemAssignments.FirstOrDefault(q => q.StoreCode == storeCode && q.ItemCode == itemCode);
            if(item!=null)
            {
                return Json(new { Balance = item.Balance,AvgPrice= item.AvgPrice }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Balance = 0, AvgPrice = 0 }, JsonRequestBehavior.AllowGet);
        }
    }
}