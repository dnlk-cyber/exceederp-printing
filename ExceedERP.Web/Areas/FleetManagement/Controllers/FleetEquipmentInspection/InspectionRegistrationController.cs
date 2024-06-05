//using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.FleetAdmin)]
    public class InspectionRegistrationController : Controller
    {
        IServiceService _ServiceService;
        IFleetService _FleetService;
        ExceedDbContext db = new ExceedDbContext();
        public InspectionRegistrationController()
        {
            _ServiceService = new ServiceService();
            _FleetService = new FleetService();
        }

        //
        // GET: /FleetManagement/Service/
        public ActionResult Index()
        {
            ViewData["Sections"] = db.WorkUnitSection.ToList();
            return View();
        }


        public ActionResult Service_Read([DataSourceRequest] DataSourceRequest request)
        {
            var Service = _ServiceService.GetAll().Select(s => new
                {
                    RepairTypeID = s.RepairTypeID,
                    ApprovedBy = s.ApprovedBy,
                    DateCreated = s.DateCreated,
                    DateInterval = s.DateInterval,
                    DateIntervalType = s.DateIntervalType,
                    IsPm = s.IsPm,
                    LabourCost = s.LabourCost,
                    STDTime = s.STDTime,
                    MileageInterval = s.MileageInterval,
                    PartCost = s.STDTime,
                    PlannerID = s.PlannerID,
                    Priority = s.Priority,
                    ServiceID = s.ServiceID,
                    ServiceName = s.RepairType.Name,
                    ServiceCode = s.RepairType.Code,
                    ServiceType = s.ServiceType,
                    SectionId = s.SectionId,
                    Type = s.Type

                });

            return Json(Service.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Service_Create([DataSourceRequest] DataSourceRequest request, Service Service)
        {
            ModelState.Remove("ServiceID");
            ModelState.Remove("ServiceName");
            List<Service> Services = _ServiceService.GetAll().Where(f => f.ServiceName == Service.ServiceName).ToList();
            if (Services.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", "  Service with Name Already Exists ");
            }
            else
            {
                var RepairType = db.RepairType.Where(x => x.RepairTypeID == Service.RepairTypeID).FirstOrDefault();
                if (Service != null && RepairType != null)
                {
                    Service.ServiceID = Guid.NewGuid();
                    Service.ServiceCode = RepairType.Code;
                    Service.ServiceName = RepairType.Name;
                    Service.CreatedBy = User.Identity.Name;
                    Service.DateCreated = DateTime.Now;
                    _ServiceService.Insert(Service);
                }
            }
            return Json(new[] { Service }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Service_Update([DataSourceRequest] DataSourceRequest request, Service Service)
        {
            ModelState.Remove("ServiceID");
            ModelState.Remove("RepairTypeID");
         
            var RepairType = db.RepairType.Where(x => x.RepairTypeID == Service.RepairTypeID).FirstOrDefault();
            if (Service != null && RepairType != null)
            {
                Service.ServiceCode = RepairType.Code;
                Service.ServiceName = RepairType.Name;
                Service.ModifiedBy = User.Identity.Name;
                Service.LastModified = DateTime.Now;
                _ServiceService.Update(Service);
            }
           

            return Json(new[] { Service }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Service_Destroy([DataSourceRequest] DataSourceRequest request, Service service)
        {
            var Service = _ServiceService.GetById(service.ServiceID);
            if (Service != null)
            {
                _ServiceService.Delete(Service);

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Equipment Type number " + service.ServiceID);

            }

            return Json(new[] { Service }.ToDataSourceResult(request));
        }
        [AllowAnonymous]
        public ActionResult ServiceDDBind(bool IsPm)
        {
            var Service = new List<Service>();
            if (IsPm)
            {
                Service = _ServiceService.GetAll().Where(x => x.IsPm == MaintenanceConst.Planned).Select(s => new Service
              {
                  ServiceName = s.ServiceName,
                  ServiceID = s.ServiceID
              }).ToList();
            }
            else
            {
                Service = _ServiceService.GetAll().Where(x => x.IsPm == MaintenanceConst.UnPlanned).Select(s => new Service
              {
                  ServiceName = s.ServiceName,
                  ServiceID = s.ServiceID
              }).ToList();
            }


            return Json(Service, JsonRequestBehavior.AllowGet);
        }




    }
}

