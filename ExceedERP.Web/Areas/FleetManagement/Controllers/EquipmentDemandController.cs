using ExceedERP.ApplicationServices.FleetManagement.FleetResource;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
       [AuthorizeRoles(FleetManagementRoles.ResourceRequestor, FleetManagementRoles.ResourceRequestApprover)] 
    public class EquipmentDemandController : Controller
    {
        IVehicleDemandService _VehicleDemandService;

        public EquipmentDemandController()
        {
            _VehicleDemandService = new VehicleDemandService();
        }
        //
        // GET: /FleetManagement/VehicleDemand/
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult VehicleDemand_Read([DataSourceRequest] DataSourceRequest request)
        {
            var VehicleDemand = _VehicleDemandService.GetAll();

            return Json(VehicleDemand.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleDemand_Create([DataSourceRequest] DataSourceRequest request, VehicleDemand VehicleDemand)
        {
            ModelState.Remove("VehicleDemandId");
            if (VehicleDemand.StartingDate > VehicleDemand.ToDate)
            {
                ModelState.Clear();
                ModelState.AddModelError("", "Start Date is Above End Date");
            }
            else
            {
                int RequestNumber = 1;
                VehicleDemand vehicleDemand = _VehicleDemandService.GetAll().OrderByDescending(x => x.RequestNumber).FirstOrDefault();
                if (vehicleDemand != null)
                {
                    RequestNumber = int.Parse(vehicleDemand.RequestNumber.ToString()) + 1;
                }
                VehicleDemand.Status = MaintenanceConst.Pending;
                VehicleDemand.VehicleDemandId = Guid.NewGuid();
                VehicleDemand.RequestNumber = RequestNumber;
                VehicleDemand.DateCreated = DateTime.Now;
                VehicleDemand.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                
                VehicleDemand.CreatedBy = User.Identity.Name;
                if (VehicleDemand != null)
                {
                    _VehicleDemandService.Insert(VehicleDemand);
                }
            }
            return Json(new[] { VehicleDemand }.ToDataSourceResult(request,ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleDemand_Update([DataSourceRequest] DataSourceRequest request, VehicleDemand VehicleDemand)
        {
            if (VehicleDemand.StartingDate > VehicleDemand.ToDate)
            {
                ModelState.Clear();
                ModelState.AddModelError("", "Start Date is Above End Date");
            }
            else
            {
                if (VehicleDemand != null)
                {
                    VehicleDemand.ModifiedBy = User.Identity.Name;
                    VehicleDemand.LastModified = DateTime.Now;
                    VehicleDemand.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                
                    _VehicleDemandService.Update(VehicleDemand);
                }
            }
            return Json(new[] { VehicleDemand }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleDemand_Destroy([DataSourceRequest] DataSourceRequest request, VehicleDemand vehicleDemand)
        {
            var VehicleDemand = _VehicleDemandService.GetById(vehicleDemand.VehicleDemandId);
            if (VehicleDemand != null)
            {
                _VehicleDemandService.Delete(VehicleDemand);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Equipment Demand number " + vehicleDemand.VehicleDemandId);
            }

            return Json(new[] { VehicleDemand }.ToDataSourceResult(request));
        }



    }
}