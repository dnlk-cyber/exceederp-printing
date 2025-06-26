using Kendo.Mvc.UI;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.ApplicationServices.FleetManagement.FleetResource;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Maintenance;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.ResourceReturnUser)] 
    public class EquipmentReturnController : Controller
    {
        IVehicleReturnService _VehicleReturnService;

        ExceedDbContext db = new ExceedDbContext();


        public EquipmentReturnController()
        {
            _VehicleReturnService = new VehicleReturnService();
        }
        //
        // GET: /FleetManagement/VehicleReturn/
        public ActionResult Index()
        {
           
            ViewData["Fleets"] = db.Fleets.ToList().Select(s => new {
                Id = s.FleetsID,
                s.OrgBranchName,
                Name = s.EquipmentName + "-" + s.PlateNumber + "-" + s.ChasisNumber
            });
            ViewData["EquipmentNames"] = db.EquipmentName.ToList().Select(s => new {
                Id = s.EquipmentNameID,
                Name = s.Name
            });
            return View();
        }


        public ActionResult VehicleReturn_Read([DataSourceRequest] DataSourceRequest request)
        {
            var VehicleReturn = _VehicleReturnService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).Select(v => new VehicleReturn
                {
                    FleetsID = v.FleetsID,
                    Description = v.Description,
                    ReasonforReturn = v.ReasonforReturn,
                    ReturnDate =v.ReturnDate,
                    ReturnedBy = v.ReturnedBy,
                    TotalTimeElapsed = v.TotalTimeElapsed,
                    ReturnStatus =v.ReturnStatus,
                    VehicleReturnID = v.VehicleReturnID ,
                    IsOnlineApproved =  v.IsOnlineApproved,
                    IsSendForApproval =  v.IsSendForApproval

                }
                );

            return Json(VehicleReturn.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleReturn_Create([DataSourceRequest] DataSourceRequest request, VehicleReturn VehicleReturn)
        {
            VehicleReturn.VehicleReturnID = Guid.NewGuid();
            VehicleReturn.ReturnDate = DateTime.Now;

            if (VehicleReturn != null)
            {
                VehicleReturn.CreatedBy = User.Identity.Name;
                VehicleReturn.DateCreated = DateTime.Now;
                VehicleReturn.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                VehicleReturn.ReturnStatus = MaintenanceConst.Pending;
                _VehicleReturnService.Insert(VehicleReturn);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Returned equipment registration", "Success", User.Identity.Name, $"created returnde vehicle {VehicleReturn.VehicleReturnID}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { VehicleReturn }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleReturn_Update([DataSourceRequest] DataSourceRequest request, VehicleReturn VehicleReturn)
        {
            if (VehicleReturn != null)
            {
                VehicleReturn.ModifiedBy = User.Identity.Name;
                VehicleReturn.LastModified = DateTime.Now;
                VehicleReturn.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                _VehicleReturnService.Update(VehicleReturn);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Returned equipment registration", "Success", User.Identity.Name, $"updated returnde vehicle {VehicleReturn.VehicleReturnID}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { VehicleReturn }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleReturn_Destroy([DataSourceRequest] DataSourceRequest request, VehicleReturn vehicleReturn)
        {
            var VehicleReturn = _VehicleReturnService.GetById(vehicleReturn.VehicleReturnID);
            if (VehicleReturn != null)
            {
                VehicleReturn.ReturnStatus = MaintenanceConst.Finished;
                _VehicleReturnService.Update(VehicleReturn);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted returned vehicle with registration number {vehicleReturn.VehicleReturnID }");
                #endregion

            }

            return Json(new[] { VehicleReturn }.ToDataSourceResult(request));
        }



    }
}

