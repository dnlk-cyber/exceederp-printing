using ExceedERP.Core.Domain.FleetManagement.FuelManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Web.Helpers;     

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class FuelInventoryController : Controller
    {
        //
        ExceedDbContext db = new ExceedDbContext();
        //
        // GET: /FleetManagement/FuelInventory/
        public ActionResult Index()
        {

            return View();
        }


        public ActionResult FuelInventory_Read([DataSourceRequest] DataSourceRequest request)
        {

            var FuelInventory = db.FuelInventorys.ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).Select(e => new

            {
                FuelInventoryId = e.FuelInventoryId,
                SiteName = e.SiteName,
                DateCreated = e.DateCreated,
                Remark = e.Remark
            }
        ).ToList();

            return Json(FuelInventory.ToDataSourceResult(request));

        }


        public ActionResult FuelInventoryDistinctSite_Read([DataSourceRequest] DataSourceRequest request)
        {

            var Allinventory = db.FuelInventorys.ToList();
            List<FuelInventory> Inventory = new List<FuelInventory>();

            string Site = string.Empty;

            foreach ( var inventory in Allinventory)
            {
                if(Site != inventory.SiteName)
                {
                    Site = inventory.SiteName;

                    var InventoryBySite = Allinventory.Where(x=>x.SiteName == Site).ToList();
                    var FuelInventory = new FuelInventory

        {
            FuelInventoryId = inventory.FuelInventoryId,
            SiteName = inventory.SiteName,

            Quantity = InventoryBySite.Sum(x => x.Quantity),
          
            TotalPrice = InventoryBySite.Sum(x => x.Quantity) * InventoryBySite.Sum(x => x.Quantity),
            DateCreated = inventory.DateCreated,
             CreatedBy = inventory.CreatedBy,
            ModifiedBy = inventory.ModifiedBy,
            LastModified = inventory.LastModified
        };
                    Inventory.Add(FuelInventory);

                }


            }



            return Json(Inventory.ToDataSourceResult(request));

        }

        public ActionResult FuelInventoryBySite_Read(string Site, [DataSourceRequest] DataSourceRequest request)
        {

            var FuelInventory = db.FuelInventorys.Where(x=>x.SiteName == Site).Select(e => new

            {
                FuelInventoryId = e.FuelInventoryId,
                SiteName = e.SiteName,
                FuelType = e.FuelType,
                Quantity = e.Quantity,
                UnitPrice = e.UnitPrice,
                TotalPrice = e.UnitPrice * e.Quantity,
                DateCreated = e.DateCreated,
                Remark = e.Remark
            }
        ).ToList();

            return Json(FuelInventory.ToDataSourceResult(request));

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FuelInventory_Create([DataSourceRequest] DataSourceRequest request, FuelInventory FuelInventorys)
        {
         

            var entity = new FuelInventory
            {

                FuelInventoryId = Guid.NewGuid(),
                SiteName = FuelInventorys.SiteName,
                FuelType = FuelInventorys.FuelType,
                Quantity = FuelInventorys.Quantity,
                UnitPrice = FuelInventorys.UnitPrice,
                TotalPrice = FuelInventorys.UnitPrice * FuelInventorys.Quantity,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name,
                Remark = FuelInventorys.Remark
            };

            db.FuelInventorys.Add(entity);
            db.SaveChanges();

            FuelInventorys.FuelInventoryId = entity.FuelInventoryId;

            return Json(new[] { FuelInventorys }.ToDataSourceResult(request));

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FuelInventory_Update([DataSourceRequest] DataSourceRequest request, FuelInventory FuelInventory)
        {


            return Json(new[] { FuelInventory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FuelInventory_Destroy([DataSourceRequest] DataSourceRequest request, FuelInventory FuelInventory)
        {
            var FuelInventoryObj = db.FuelInventorys.Where(x => x.FuelInventoryId == FuelInventory.FuelInventoryId).FirstOrDefault();
            if (FuelInventory != null)
            {

                db.FuelInventorys.Attach(FuelInventoryObj);
                db.FuelInventorys.Remove(FuelInventoryObj);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted FuelInventory number " + FuelInventory.FuelInventoryId);

            }

            return Json(new[] { FuelInventory }.ToDataSourceResult(request));
        }

    }
}
