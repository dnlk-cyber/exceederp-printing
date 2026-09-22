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
using System.Data.Entity;
using ExceedERP.Web.Areas.FleetManagement.Models;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
    public class FuelTypeController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        //
        // GET: /FleetManagement/FuelType/
        public ActionResult Index()
        {

            return View();
        }


        public ActionResult FuelType_Read([DataSourceRequest] DataSourceRequest request)
        {

            var FuelType = db.FuelItemSettings.Select(e => new 

            {
                FuelTypeId = e.FuelItemSettingId,
                Name = e.ItemCategory.Name,
                Remark = e.InventoryItem.Name,
                DateCreated = e.DateCreated
               
            }
        ).ToList();

            return Json(FuelType.ToDataSourceResult(request));

        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FuelType_Create([DataSourceRequest] DataSourceRequest request, FuelType FuelTypes)
        {
            var entity = new FuelType
                  {

                      FuelTypeId = Guid.NewGuid(),
                      Name = FuelTypes.Name,
                      DateCreated = DateTime.Now,
                      Remark = FuelTypes.Remark
                  };
            db.FuelTypes.Add(entity);

            db.SaveChanges();
            FuelTypes.FuelTypeId = entity.FuelTypeId;
            return Json(new[] { FuelTypes }.ToDataSourceResult(request));

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FuelType_Update([DataSourceRequest] DataSourceRequest request, FuelType FuelType)
        {


            return Json(new[] { FuelType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FuelType_Destroy([DataSourceRequest] DataSourceRequest request, FuelType FuelType)
        {
            var FuelTypeObj = db.FuelTypes.Where(x => x.FuelTypeId == FuelType.FuelTypeId).FirstOrDefault();
            if (FuelType != null)
            {

                db.FuelTypes.Attach(FuelTypeObj);
                db.FuelTypes.Remove(FuelTypeObj);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted FuelType number " + FuelType.FuelTypeId);

            }

            return Json(new[] { FuelType }.ToDataSourceResult(request));
        }

        
        public ActionResult FuelTypeDDBind()
        {

            var fleets = db.FuelTypes.Select(s => new FleetViewModel
            {
                PlateNumber = s.Name ,
                FleetsID = Guid.Empty
            });

            return Json(fleets, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }

}

