using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    public class SparePartInventoryController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: FleetManagement/AlertLevel
        public ActionResult Index()
        {
            ViewData["ItemCategorys"] = db.ItemCategorys.ToList();
            return View();
        }

        public ActionResult SparePartInventory_Read([DataSourceRequest] DataSourceRequest request)
        {
            var InventorySparePartMapping = db.InventorySparePartMapping.ToList();

            return Json(InventorySparePartMapping.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SparePartInventory_Create([DataSourceRequest] DataSourceRequest request, InventorySparePartMapping InventorySparePartMapping)
        {
            ModelState.Remove("InventorySparePartMappingId");


            InventorySparePartMapping.InventorySparePartMappingId = Guid.NewGuid();
         
                InventorySparePartMapping.DateCreated = DateTime.Now;
                InventorySparePartMapping.CreatedBy = User.Identity.Name;

                db.InventorySparePartMapping.Add(InventorySparePartMapping);
            db.SaveChanges();
          
            return Json(new[] { InventorySparePartMapping }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SparePartInventory_Update([DataSourceRequest] DataSourceRequest request, InventorySparePartMapping InventorySparePartMapping)
        {
            if (InventorySparePartMapping != null)
            {
                InventorySparePartMapping.ModifiedBy = User.Identity.Name;
                InventorySparePartMapping.LastModified = DateTime.Now;

                db.InventorySparePartMapping.Attach(InventorySparePartMapping);
                db.Entry(InventorySparePartMapping).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { InventorySparePartMapping }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SparePartInventory_Destroy([DataSourceRequest] DataSourceRequest request, InventorySparePartMapping InventorySparePartMapping)
        {
            var InventorySparePartMappings = db.InventorySparePartMapping.Where(x => x.InventorySparePartMappingId == InventorySparePartMapping.InventorySparePartMappingId).FirstOrDefault();
            if (InventorySparePartMappings != null)
            {

                db.InventorySparePartMapping.Attach(InventorySparePartMappings);
                db.InventorySparePartMapping.Remove(InventorySparePartMappings);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted InventorySparePartMapping number " + InventorySparePartMapping.InventorySparePartMappingId);

            }

            return Json(new[] { InventorySparePartMapping }.ToDataSourceResult(request));
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
