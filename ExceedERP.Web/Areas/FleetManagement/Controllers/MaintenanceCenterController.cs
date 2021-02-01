using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using Kendo.Mvc.UI;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using Kendo.Mvc.Extensions;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
                 [AuthorizeRoles(FleetManagementRoles.FleetAdmin)]  
    public class MaintenanceCenterController : Controller
    {
        IMaintenanceCenterService _MaintenanceCenterService;

        public MaintenanceCenterController()
        {
            _MaintenanceCenterService = new MaintenanceCenterService();
        }
        //
        // GET: /FleetManagement/MaintenanceCenter/
        public ActionResult Index()
        {

            return View();
        }


        public JsonResult MaintenanceCenter_Read([DataSourceRequest] DataSourceRequest request)
        {
            _MaintenanceCenterService = new MaintenanceCenterService();
            var oper = _MaintenanceCenterService.GetAll();
            var Opers = oper.Select(o => new MaintenanceCenter
            {
                MaintenanceCenterID = o.MaintenanceCenterID,
                Name = o.Name,
                Type = o.Type,
                Description = o.Description

            });

            return this.Json(Opers.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult MaintenanceCenter_Create([DataSourceRequest] DataSourceRequest request, MaintenanceCenter MaintenanceCenter)
        {
            ModelState.Remove("MaintenanceCenterID");
            List<MaintenanceCenter> MaintenanceCenters = _MaintenanceCenterService.GetAll().Where(f => f.Name == MaintenanceCenter.Name).ToList();
            if (MaintenanceCenters.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", " Maintenance Center with Name Already Exists ");
            }
            else
            {
                MaintenanceCenter.MaintenanceCenterID = Guid.NewGuid();

                if (MaintenanceCenter != null)
                {
                    MaintenanceCenter.CreatedBy = User.Identity.Name;
                    MaintenanceCenter.DateCreated = DateTime.Now;
                    _MaintenanceCenterService.Insert(MaintenanceCenter);
                }
            }
            return this.Json(new[] { MaintenanceCenter }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult MaintenanceCenter_Update([DataSourceRequest] DataSourceRequest request, MaintenanceCenter MaintenanceCenter)
        {

            if (MaintenanceCenter != null)
            {
                MaintenanceCenter.ModifiedBy = User.Identity.Name;
                MaintenanceCenter.LastModified = DateTime.Now;
                _MaintenanceCenterService.Update(MaintenanceCenter);
            }


            return this.Json(new[] { MaintenanceCenter }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult MaintenanceCenter_Destroy([DataSourceRequest] DataSourceRequest request, MaintenanceCenter MaintenanceCenter)
        {
            var MaintenanceCenters = _MaintenanceCenterService.GetById(MaintenanceCenter.MaintenanceCenterID);
            if (MaintenanceCenters != null)
            {
                _MaintenanceCenterService.Delete(MaintenanceCenters);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Maintenance Center number " + MaintenanceCenter.MaintenanceCenterID);

            }

            return this.Json(new[] { MaintenanceCenter }.ToDataSourceResult(request));
        }
            [AllowAnonymous]
        public ActionResult MaintenanceCenterDDBind()
        {

            var fleets = _MaintenanceCenterService.GetAll().Select(s => new MaintenanceCenter
            {
                Name = s.Name,
                MaintenanceCenterID = s.MaintenanceCenterID
            });

            return Json(fleets, JsonRequestBehavior.AllowGet);
        }
            [AllowAnonymous]
        public ActionResult CenterTypeDDBind()
        {

            SelectListItem type = new SelectListItem();
            List<SelectListItem> typelist = new List<SelectListItem>();

            type.Text = "INTERNAL";
            type.Value = "INTERNAL";
            typelist.Add(type);
             type = new SelectListItem();
            type.Text = "EXTERNAL";
            type.Value = "EXTERNAL";
            typelist.Add(type);


            return Json(typelist, JsonRequestBehavior.AllowGet);
        }


                    [AllowAnonymous]
        public ActionResult MaintenanceCenterByTypeDDBind(string Type)
        {


            var fleets = _MaintenanceCenterService.GetAll().Where(x=>x.Type==Type).Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Name
            });

            return Json(fleets, JsonRequestBehavior.AllowGet);
        }


    }
}