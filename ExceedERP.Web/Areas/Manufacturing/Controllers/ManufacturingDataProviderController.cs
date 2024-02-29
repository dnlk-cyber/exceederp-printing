using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers
{
    public class ManufacturingDataProviderController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: Manufacturing/ManufacturingDataProvider
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetInventoryItemCategory(string text)
        {
            // only Category
            var itemCategory = db.ItemCategorys.Where(x => !x.HaveParent);
            if (!string.IsNullOrWhiteSpace(text))
            {
                itemCategory = itemCategory.Where(x => x.Name.Contains(text));
            }
            var result = itemCategory.Select(s => new
            {
                Value = s.ItemCategoryCode,
                Text = s.Name + "-" + s.ItemCategoryCode
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInventoryItemSubCategory(string text, string categoryCode)
        {
            // Only sub-Category
            var itemCategory = db.ItemCategorys.Where(x => x.HaveParent && x.ParentName == categoryCode);

            if (!string.IsNullOrWhiteSpace(text))
            {
                itemCategory = itemCategory.Where(x => x.Name.Contains(text));
            }
            var result = itemCategory.Select(s => new
            {
                Value = s.ItemCategoryCode,
                Text = s.Name + "-" + s.ItemCategoryCode
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetManufacturingMaterialCategory()
        {
            var categories = db.ManufacturingMaterialCategories;
            var result = categories.Select(s => new
            {
                Value = s.ManufacturingMaterialCategoryId,
                Text = s.ManufacturingCategory
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUoM()
        {
            var unitOfm = db.UoMs.AsQueryable();
            return Json(unitOfm.Select(i => new
            {
                UoMName = i.UoMName,
                Value = i.UoMName
            })
            , JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTaskCategory()
        {
            var taskCategory = db.ManifucturingTaskCategories.AsQueryable();
            var result = taskCategory.Select(s => new
            {
                Text = s.Name,
                Value = s.ManifucturingTaskCategoryId
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}