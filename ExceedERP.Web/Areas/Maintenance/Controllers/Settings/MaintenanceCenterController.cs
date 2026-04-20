using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    //[AuthorizeRoles(FleetManagementRoles.FleetAdmin)]
    public class MaintenanceCenterController : Controller
    {
        private IMaintenanceCenterService _MaintenanceCenterService;
        ExceedDbContext db = new ExceedDbContext();

        public MaintenanceCenterController()
        {
            _MaintenanceCenterService = new MaintenanceCenterService();
        }

        public ActionResult Index()
        {
            ViewData["BusinessUnit"] = db.BusinessUnits.Select(s => new
            {
                Value = s.BusinessUnitId,
                Text = s.Name
            });
            ViewData["Branch"] = db.Branch.Select(s => new
            {
                Value = s.BranchId,
                Text = s.Name
            });
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult MaintenanceCenter_Read([DataSourceRequest] DataSourceRequest request)
        {
            var oper = db.MaintenanceCenter.ToList();
            var Opers = oper.Select(o => new MaintenanceCenter
            {
                MaintenanceCenterID = o.MaintenanceCenterID,
                Name = o.Name,
                Type = o.Type,
                Description = o.Description,
                BusinessUnitId = o.BusinessUnitId,
                BranchId = o.BranchId,
                IsExternal = o.IsExternal
            });

            return this.Json(Opers.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult MaintenanceCenter_Create([DataSourceRequest] DataSourceRequest request, MaintenanceCenter MaintenanceCenter)
        {
            ModelState.Remove("MaintenanceCenterID");
            var existing = _MaintenanceCenterService.GetAll().Where(f => f.Name == MaintenanceCenter.Name).ToList();

            if (existing.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", "Maintenance Center with this Name already exists.");
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
            var existing = _MaintenanceCenterService.GetById(MaintenanceCenter.MaintenanceCenterID);
            if (existing != null)
            {
                _MaintenanceCenterService.Delete(existing);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Maintenance Center " + MaintenanceCenter.MaintenanceCenterID);
            }

            return this.Json(new[] { MaintenanceCenter }.ToDataSourceResult(request));
        }

        public ActionResult MaintenanceCenterDDBind()
        {
            var data = _MaintenanceCenterService.GetAll().Select(s => new MaintenanceCenter
            {
                Name = s.Name,
                MaintenanceCenterID = s.MaintenanceCenterID
            });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CenterTypeDDBind()
        {
            var types = new List<SelectListItem>
            {
                new SelectListItem { Text = "INTERNAL", Value = "INTERNAL" },
                new SelectListItem { Text = "EXTERNAL", Value = "EXTERNAL" }
            };

            return Json(types, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllMaintenanceCenters()
        {
            var data = _MaintenanceCenterService.GetAll().Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Name
            });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllMaintenanceCentersById()
        {
            var data = _MaintenanceCenterService.GetAll().Select(s => new
            {
                Name = s.Name,
                Id = s.MaintenanceCenterID
            });

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}