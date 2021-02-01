using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.FleetManagement;
 

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.FleetAdmin)]
    public class EquipmentTypeFuelConsumptionController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: FleetManagement/AlertLevel
        public ActionResult Index()
        {

            ViewData["EquipmentType"] = db.EquipmentType.ToList();
            ViewData["EquipmentIdentity"] = db.EquipmentIdentity.ToList();

            return View();
        }
        
        public ActionResult FleetIFuelConsumption_Read([DataSourceRequest] DataSourceRequest request)
        {
            var FleetIFuelConsumption = db.FleetIFuelConsumptions.Select(s => new   {
            
            EquipmentTypeId = s.EquipmentTypeId,
            EquipmentCategory = s.EquipmentType.EquipmentIdentityID.ToString(),
            MaxConsupmtion = s.MaxConsupmtion,
            MinConsupmtion = s.MinConsupmtion,
            }).ToList();

            return Json(FleetIFuelConsumption.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetIFuelConsumption_Create([DataSourceRequest] DataSourceRequest request, FleetIFuelConsumption model)
        {
            ModelState.Remove("FleetIFuelConsumptionId");
       
            var Level = new FleetIFuelConsumption
            {       
                 MinConsupmtion = model.MinConsupmtion,
                 MaxConsupmtion = model.MaxConsupmtion,
                 EquipmentTypeId = model.EquipmentTypeId,
                  DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name
        
            };
                   db.FleetIFuelConsumptions.Add(Level);
                    db.SaveChanges();
                    model.FleetIFuelConsumptionId = Level.FleetIFuelConsumptionId;
         
            return Json(new[] { model }.ToDataSourceResult(request,ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetIFuelConsumption_Update([DataSourceRequest] DataSourceRequest request, FleetIFuelConsumption FleetIFuelConsumption)
        {
          
            if (FleetIFuelConsumption != null)
            {
                FleetIFuelConsumption.MinConsupmtion = FleetIFuelConsumption.MinConsupmtion;
                FleetIFuelConsumption.MaxConsupmtion = FleetIFuelConsumption.MaxConsupmtion;
                FleetIFuelConsumption.EquipmentTypeId = FleetIFuelConsumption.EquipmentTypeId;
               
                FleetIFuelConsumption.ModifiedBy = User.Identity.Name;
                FleetIFuelConsumption.LastModified = DateTime.Now;

                db.FleetIFuelConsumptions.Attach(FleetIFuelConsumption);
                db.Entry(FleetIFuelConsumption).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { FleetIFuelConsumption }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetIFuelConsumption_Destroy([DataSourceRequest] DataSourceRequest request, FleetIFuelConsumption FleetIFuelConsumption)
        {
            var FleetIFuelConsumptions = db.FleetIFuelConsumptions.Where(x => x.FleetIFuelConsumptionId == FleetIFuelConsumption.FleetIFuelConsumptionId).FirstOrDefault();
            if (FleetIFuelConsumptions != null)
            {

                db.FleetIFuelConsumptions.Attach(FleetIFuelConsumptions);
                db.FleetIFuelConsumptions.Remove(FleetIFuelConsumptions);
                db.SaveChanges();
                  UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted FleetIFuelConsumption number " + FleetIFuelConsumption.FleetIFuelConsumptionId);

            }

            return Json(new[] { FleetIFuelConsumption }.ToDataSourceResult(request));
        }



    }
}
   