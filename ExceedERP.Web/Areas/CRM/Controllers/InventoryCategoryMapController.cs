using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    public class InventoryCategoryMapController : Controller
    {
          ExceedDbContext db = new ExceedDbContext();
        // GET: FleetManagement/AlertLevel
        public ActionResult Index()
        {
            ViewData["ItemCategorys"] = db.ItemCategorys.ToList();
            return View();
        }

        public ActionResult InventoryCateoryMap_Read([DataSourceRequest] DataSourceRequest request)
        {
            var InventoryCateoryMap = db.InventoryCateoryMaps.ToList();

            return Json(InventoryCateoryMap.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InventoryCateoryMap_Create([DataSourceRequest] DataSourceRequest request, InventoryCateoryMap InventoryCateoryMap)
        {
            ModelState.Remove("InventoryCateoryMapId");

            List<InventoryCateoryMap> InventoryCateoryMapList = db.InventoryCateoryMaps.Where(ii => ii.InventoryCategoryId == InventoryCateoryMap.InventoryCategoryId && ii.Category == InventoryCateoryMap.Category).ToList();
            if (InventoryCateoryMapList.Any())
                {
                    ModelState.AddModelError("InvalidIndex", "Please enter Different Inventory Category Duplicate value exist");
                    return Json(new[] { InventoryCateoryMap }.ToDataSourceResult(request));
            }
                else
                {
                    InventoryCateoryMap.InventoryCateoryMapId = Guid.NewGuid();

                    InventoryCateoryMap.DateCreated = DateTime.Now;
                    InventoryCateoryMap.CreatedBy = User.Identity.Name;

                    db.InventoryCateoryMaps.Add(InventoryCateoryMap);
                    db.SaveChanges();
                }
            return Json(new[] { InventoryCateoryMap }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InventoryCateoryMap_Update([DataSourceRequest] DataSourceRequest request, InventoryCateoryMap InventoryCateoryMap)
        {

             List<InventoryCateoryMap> InventoryCateoryMapList = db.InventoryCateoryMaps.Where(ii => ii.InventoryCategoryId == InventoryCateoryMap.InventoryCategoryId && ii.Category == InventoryCateoryMap.Category && ii.InventoryCateoryMapId != InventoryCateoryMap.InventoryCateoryMapId).ToList();
             if (InventoryCateoryMapList.Any())
             {
                 ModelState.AddModelError("InvalidIndex", "Please enter Different Inventory Category Duplicate value exist");
                 return Json(new[] { InventoryCateoryMap }.ToDataSourceResult(request));
             }
             else
             {
                 if (InventoryCateoryMap != null)
                 {
                     InventoryCateoryMap.ModifiedBy = User.Identity.Name;
                     InventoryCateoryMap.LastModified = DateTime.Now;

                     db.InventoryCateoryMaps.Attach(InventoryCateoryMap);
                     db.Entry(InventoryCateoryMap).State = EntityState.Modified;
                     db.SaveChanges();
                 }
             }
            return Json(new[] { InventoryCateoryMap }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InventoryCateoryMap_Destroy([DataSourceRequest] DataSourceRequest request, InventoryCateoryMap InventoryCateoryMap)
        {
            var InventoryCateoryMaps = db.InventoryCateoryMaps.Where(x => x.InventoryCateoryMapId == InventoryCateoryMap.InventoryCateoryMapId).FirstOrDefault();
            if (InventoryCateoryMaps != null)
            {

                db.InventoryCateoryMaps.Attach(InventoryCateoryMaps);
                db.InventoryCateoryMaps.Remove(InventoryCateoryMaps);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted InventoryCateoryMap number " + InventoryCateoryMap.InventoryCateoryMapId);

            }

            return Json(new[] { InventoryCateoryMap }.ToDataSourceResult(request));
        }


        public ActionResult ItemCategoryDDBind()
        {

            var ItemCategory = db.ItemCategorys.Select(s => new
            {
                Text = s.Name,
                Value = s.ItemCategoryCode
            });

            return Json(ItemCategory, JsonRequestBehavior.AllowGet);
        }
    }
    
}