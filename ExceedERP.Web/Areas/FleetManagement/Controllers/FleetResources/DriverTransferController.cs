using ExceedERP.ApplicationServices.FleetManagement.FleetResource;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.ResourceRequestor, FleetManagementRoles.ResourceApprover)]
    public class DriverTransferController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        IDriverVehicleTransferService _DriverVehicleTransferService;
        IFleetService _FleetService;
        public DriverTransferController()
        {
            _FleetService = new FleetService();
            _DriverVehicleTransferService = new DriverVehicleTransferService();
        }
        //
        // GET: /FleetManagement/DriverVehicleTransfer/
        public ActionResult Index()
        {
            ViewData["Operators"] = db.Operator.ToList().Select(s => new {
                Id = s.OperatorID,
                Name = s.FullName
            });
            return View();
        }


        public ActionResult DriverVehicleTransfer_Read([DataSourceRequest] DataSourceRequest request)
        {
            var DriverVehicleTransfer = _DriverVehicleTransferService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).ToList();

            return Json(DriverVehicleTransfer.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DriverVehicleTransfer_Create([DataSourceRequest] DataSourceRequest request, DriverVehicleTransfer DriverVehicleTransfer)
        {

            ExceedDbContext db = new ExceedDbContext();
            ModelState.Remove("DriverVehicleTransferID");
            ModelState.Remove("FromEquipment");
            ModelState.Remove("ToEquipment");


            if (DriverVehicleTransfer.FromOperatorId == DriverVehicleTransfer.ToOperatorId)
            {
                ModelState.Clear();
                ModelState.AddModelError("", " Driver must be Diffrent ! ");
            }
            else if (DriverVehicleTransfer.StartingDate > DriverVehicleTransfer.ToDate)
            {
                ModelState.Clear();
                ModelState.AddModelError("", "Start Date is Above End Date !");
            }
            else
            {
                var FromFleet =
                    _FleetService.GetAll()
                        .Where(x => x.OperatorName == DriverVehicleTransfer.FromOperatorId)
                        .FirstOrDefault();
                var ToFleet =
                _FleetService.GetAll()
                    .Where(x => x.OperatorName == DriverVehicleTransfer.ToOperatorId)
                    .FirstOrDefault();

                if (DriverVehicleTransfer != null)
                {
                    DriverVehicleTransfer.DriverVehicleTransferID = Guid.NewGuid();

                    DriverVehicleTransfer.FromEquipment = FromFleet.FleetsID;
                    DriverVehicleTransfer.ToEquipment = ToFleet.FleetsID;
                    DriverVehicleTransfer.DateCreated = DateTime.Now;
                    DriverVehicleTransfer.CreatedBy = User.Identity.Name;
                    DriverVehicleTransfer.Status = MaintenanceConst.Pending;
                    DriverVehicleTransfer.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
              
                    _DriverVehicleTransferService.Insert(DriverVehicleTransfer);

                }

                var EquipmentTyres = db.EquipmentTyres.Where(x => (x.fleetId == DriverVehicleTransfer.FromEquipment || x.fleetId == DriverVehicleTransfer.ToEquipment) && x.Isattached).ToList();

                foreach (var EquipmentTyre in EquipmentTyres)
                {

                    var TyreDetail = new SiteVechicleTransferTyreDetail()
                    {
                        SiteVechicleTransferId = DriverVehicleTransfer.DriverVehicleTransferID,
                        SerialNo = EquipmentTyre.SerialNo,
                        Type = EquipmentTyre.Type,
                        Description = EquipmentTyre.Make + " " + EquipmentTyre.Condition,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now

                    };
                    db.SiteVechicleTransferTyreDetailes.Add(TyreDetail);
                    db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Operator transfer", "Success", User.Identity.Name, $"created driver transfer   {TyreDetail.SiteVechicleTransferTyreDetailId}", Modules.FleetManagement);
                    #endregion

                }
            }

                return Json(new[] { DriverVehicleTransfer }.ToDataSourceResult(request, ModelState));
            
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DriverVehicleTransfer_Update([DataSourceRequest] DataSourceRequest request, DriverVehicleTransfer DriverVehicleTransfer)
        {
            if (DriverVehicleTransfer.StartingDate > DriverVehicleTransfer.ToDate)
            {
                ModelState.Clear();
                ModelState.AddModelError("", "Start Date is Above End Date");
            }
            else
            {

                if (DriverVehicleTransfer != null)
                {
                    DriverVehicleTransfer.DateCreated = DateTime.Now;
                    DriverVehicleTransfer.CreatedBy = User.Identity.Name;
                    DriverVehicleTransfer.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
              
                    _DriverVehicleTransferService.Update(DriverVehicleTransfer);
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Operator transfer", "Success", User.Identity.Name, $"updated driver transfer   {DriverVehicleTransfer.DriverVehicleTransferID}", Modules.FleetManagement);
                    #endregion
                }

            }
            return Json(new[] { DriverVehicleTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DriverVehicleTransfer_Destroy([DataSourceRequest] DataSourceRequest request, DriverVehicleTransfer driverVehicleTransfer)
        {
            var DriverVehicleTransfer = _DriverVehicleTransferService.GetById(driverVehicleTransfer.DriverVehicleTransferID);
            
            if (DriverVehicleTransfer != null)
            {
                _DriverVehicleTransferService.Delete(DriverVehicleTransfer);

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Diver Transfer" + DriverVehicleTransfer.DriverVehicleTransferID);

            }

            return Json(new[] { DriverVehicleTransfer }.ToDataSourceResult(request));
        }



    }
}