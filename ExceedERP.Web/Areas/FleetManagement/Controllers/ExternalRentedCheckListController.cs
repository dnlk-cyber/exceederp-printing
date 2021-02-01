using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Helpers.Common;  
using System.Data.Entity;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class ExternalRentedCheckListController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExternalRentedEquipmentCheckList_Read([DataSourceRequest] DataSourceRequest request , Guid Id)
        {
            var ExternalRentedEquipmentCheckList = db.ExternalRentedEquipmentCheckList.Where(x=>x.ExternalRentedEquipmentId == Id).ToList();

            return Json(ExternalRentedEquipmentCheckList.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExternalRentedEquipmentCheckList_Create([DataSourceRequest] DataSourceRequest request, ExternalRentedEquipmentCheckList ExternalRentedEquipmentCheckList, Guid Id)
        {
            ModelState.Remove("ExternalRentedEquipmentCheckListId");
            ModelState.Remove("ExternalComponentId");

            ExternalRentedEquipmentCheckList.ExternalRentedEquipmentCheckListId = Guid.NewGuid();
            ExternalRentedEquipmentCheckList.ExternalRentedEquipmentId = Id;
            ExternalRentedEquipmentCheckList.DateCreated = DateTime.Now;
            ExternalRentedEquipmentCheckList.CreatedBy = User.Identity.Name;

            db.ExternalRentedEquipmentCheckList.Add(ExternalRentedEquipmentCheckList);
            db.SaveChanges();


            return Json(new[] { ExternalRentedEquipmentCheckList }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExternalRentedEquipmentCheckList_Update([DataSourceRequest] DataSourceRequest request, ExternalRentedEquipmentCheckList ExternalRentedEquipmentCheckList)
        {
            if (ExternalRentedEquipmentCheckList != null)
            {
                ExternalRentedEquipmentCheckList.ModifiedBy = User.Identity.Name;
                ExternalRentedEquipmentCheckList.LastModified = DateTime.Now;

                db.ExternalRentedEquipmentCheckList.Attach(ExternalRentedEquipmentCheckList);
                db.Entry(ExternalRentedEquipmentCheckList).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { ExternalRentedEquipmentCheckList }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExternalRentedEquipmentCheckList_Destroy([DataSourceRequest] DataSourceRequest request, ExternalRentedEquipmentCheckList ExternalRentedEquipmentCheckList)
        {
            var ExternalRentedEquipmentCheckLists = db.ExternalRentedEquipmentCheckList.Where(x => x.ExternalRentedEquipmentCheckListId == ExternalRentedEquipmentCheckList.ExternalRentedEquipmentCheckListId).FirstOrDefault();
            if (ExternalRentedEquipmentCheckLists != null)
            {

                db.ExternalRentedEquipmentCheckList.Attach(ExternalRentedEquipmentCheckLists);
                db.ExternalRentedEquipmentCheckList.Remove(ExternalRentedEquipmentCheckLists);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted External Component Detail number " + ExternalRentedEquipmentCheckList.ExternalRentedEquipmentCheckListId);

            }

            return Json(new[] { ExternalRentedEquipmentCheckList }.ToDataSourceResult(request));
        }



    }
}
