using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.ApplicationServices.FleetManagement.FleetEquipmentRent;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.Web.Areas.FleetManagement.Models;
using System.IO;

using ExceedERP.DataAccess.Context;



namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{

    [AuthorizeRoles(FleetManagementRoles.RentUser, FleetManagementRoles.RentApprover)]
    public class TimeSheetController : Controller
    {


        ITimeSheetService _TimeSheetService;
        IFleetService _FleetService;
        private IRentFromExternalService _RentFromExternalService;
        public TimeSheetController()
        {
            _TimeSheetService = new TimeSheetService();
            _FleetService = new FleetService();
            _RentFromExternalService = new RentFromExternalService();
        }

        //
        // GET: /FleetManagement/TimeSheet/
        public ActionResult Index()
        {
            return View();
        }
        public string GetFleetName(Guid Id)
        {


            var fleet = _FleetService.GetById(Id);
            if (fleet != null)
            {
                return fleet.UnitNum + " - " + fleet.PlateNumber;
            }
            else
            {
                return string.Empty;
            }
        }
        public string GetName(Guid Id)
        {


            var fleet = _FleetService.GetById(Id);
            if (fleet != null)
            {
                return fleet.EquipmentName;
            }
            else
            {
                return string.Empty;
            }
        }

       

        public ActionResult TimeSheet_Read([DataSourceRequest] DataSourceRequest request)
        {
            var TimeSheet = _TimeSheetService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).Where(x => x.Type == MaintenanceConst.RentalTimeSheet).Select(t => new TimeSheet

                {
                    Date = t.Date,
                    RentalRate = t.RentalRate,
                    IdleTime = t.IdleTime,
                    DownTime = t.DownTime,
                    CustomerName = t.CustomerName,
                    PlateSerialNo = GetFleetName(t.FleetsID),
                    EquipmentAdministratorName = t.EquipmentAdministratorName,
                    EquipmentName = GetName(t.FleetsID),
                    FleetsID = t.FleetsID,
                    KMHrReading = t.KMHrReading,
                    TimeSheetID = t.TimeSheetID,
                    WorkingHoursAfternoonFrom = t.WorkingHoursAfternoonFrom,
                    WorkingHoursAfternoonTo = t.WorkingHoursAfternoonTo,
                    WorkingHoursMorningFrom = t.WorkingHoursMorningFrom,
                    WorkingHoursMorningto = t.WorkingHoursMorningto,
                    TypeofWork = t.TypeofWork,
                    IsOnlineApproved = t.IsOnlineApproved,
                    IsSendForApproval = t.IsSendForApproval,
                    Status = t.Status,
                    DateCreated = t.DateCreated,
                    LastModified = t.LastModified,
                    CreatedBy = t.CreatedBy,
                    ModifiedBy = t.ModifiedBy

                });

            return Json(TimeSheet.ToDataSourceResult(request));
        }
        public ActionResult TimeSheetForExternal_Read([DataSourceRequest] DataSourceRequest request, string Id)
        {
            var TimeSheet = _TimeSheetService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).Where(x => x.IsExternal == true && x.FleetsID.ToString() == Id).Select(t => new TimeSheet

            {
                Date = t.Date,
                RentalRate = t.RentalRate,
                IdleTime = t.IdleTime,
                DownTime = t.DownTime,
                CustomerName = t.CustomerName,
                EquipmentAdministratorName = t.EquipmentAdministratorName,
                EquipmentName = t.EquipmentName,
                PlateSerialNo = t.PlateSerialNo,
                FleetsID = t.FleetsID,
                KMHrReading = t.KMHrReading,
                TimeSheetID = t.TimeSheetID,
                WorkingHoursAfternoonFrom = t.WorkingHoursAfternoonFrom,
                WorkingHoursAfternoonTo = t.WorkingHoursAfternoonTo,
                WorkingHoursMorningFrom = t.WorkingHoursMorningFrom,
                WorkingHoursMorningto = t.WorkingHoursMorningto,
                TypeofWork = t.TypeofWork,
                IsOnlineApproved = t.IsOnlineApproved,
                IsSendForApproval = t.IsSendForApproval,
                Status = t.Status,
                RentType = t.RentType,
                DateCreated = t.DateCreated,
                LastModified = t.LastModified,
                CreatedBy = t.CreatedBy,
                ModifiedBy = t.ModifiedBy

            });

            return Json(TimeSheet.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TimeSheetForExternal_Create([DataSourceRequest] DataSourceRequest request, TimeSheet TimeSheet, Guid Id)
        {

            ModelState.Remove("TimeSheetID");
            ModelState.Remove("CustomerName");
            ModelState.Remove("FleetsID");
            ModelState.Remove("FleetTypeID");
            ModelState.Remove("FleetCategoryID");
            IFleetService fleetService = new FleetService();

            if (TimeSheet.WorkingHoursAfternoonFrom > TimeSheet.WorkingHoursAfternoonTo)
            {
                ModelState.AddModelError("InvalidTime", "Starting Time Is Must Be Less Than Ending Time");
            }
            else if (TimeSheet.WorkingHoursMorningFrom > TimeSheet.WorkingHoursMorningto)
            {
                ModelState.AddModelError("InvalidTime", "Starting Time Is Must Be Less Than Ending Time");

            }
            else
            {


                var RentFromExternal = _RentFromExternalService.GetById(Id);
                Fleets fleet = fleetService.GetById(TimeSheet.FleetsID);

                if (TimeSheet != null)
                {
                    TimeSheet.TimeSheetID = Guid.NewGuid();
                    TimeSheet.CustomerName = RentFromExternal.CompanyName;
                    TimeSheet.Date = DateTime.Now;
                    TimeSheet.RentType = fleet.RentType;
                    TimeSheet.Type = MaintenanceConst.RentalTimeSheet;
                    TimeSheet.RentalRate = TimeSheet.RentalRate;
                    TimeSheet.FleetTypeID = Guid.Empty;
                    TimeSheet.FleetCategoryID = Guid.Empty;
                    TimeSheet.EquipmentName = TimeSheet.EquipmentName;
                    TimeSheet.Type = MaintenanceConst.RentalTimeSheet;
                    TimeSheet.CreatedBy = User.Identity.Name;
                    TimeSheet.DateCreated = DateTime.Now;
                    TimeSheet.Status = MaintenanceConst.Pending;
                    TimeSheet.IsExternal = true;
                    TimeSheet.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                    TimeSheet.FleetsID = Id;

                    _TimeSheetService.Insert(TimeSheet);
                }
            }

            return Json(new[] { TimeSheet }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TimeSheet_Create([DataSourceRequest] DataSourceRequest request, TimeSheet TimeSheet)
        {
            ModelState.Remove("TimeSheetID");
            ModelState.Remove("CustomerName");
            ModelState.Remove("FleetTypeID");
            ModelState.Remove("FleetCategoryID");

            IFleetService fleetService = new FleetService();
            Fleets fleet = fleetService.GetById(TimeSheet.FleetsID);

            if (fleet.RentalRate < 1)
            {
                ModelState.AddModelError("SetRental", "Set Proper Rental Rate");

            }

            else if (TimeSheet.WorkingHoursAfternoonFrom > TimeSheet.WorkingHoursAfternoonTo)
            {
                ModelState.AddModelError("InvalidTime", "Starting Time Is Must Be Less Than Ending Time");
            }
            else if (TimeSheet.WorkingHoursMorningFrom > TimeSheet.WorkingHoursMorningto)
            {
                ModelState.AddModelError("InvalidTime", "Starting Time Is Must Be Less Than Ending Time");

            }

            else
            {


                if (TimeSheet != null)
                {
                    TimeSheet.TimeSheetID = Guid.NewGuid();
                    TimeSheet.Date = DateTime.Now;
                    TimeSheet.RentalRate = TimeSheet.RentalRate;
                    TimeSheet.IdealRentalRate = TimeSheet.IdealRentalRate;
                    TimeSheet.RentType = fleet.RentType;
                    TimeSheet.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                    TimeSheet.CustomerName = fleet.Location;
                    TimeSheet.FleetTypeID = fleet.EquipmentIdentityID;
                    TimeSheet.FleetCategoryID = fleet.EquipmentTypeID;
                    TimeSheet.EquipmentName = fleet.EquipmentName;
                    TimeSheet.PlateSerialNo = fleet.PlateNumber + "-" + fleet.SerialNo;
                    TimeSheet.IsExternal = fleet.IsExternal;
                    TimeSheet.Type = MaintenanceConst.RentalTimeSheet;
                    TimeSheet.CreatedBy = User.Identity.Name;
                    TimeSheet.DateCreated = DateTime.Now;
                    TimeSheet.Status = MaintenanceConst.Pending;

                    _TimeSheetService.Insert(TimeSheet);
                }
            }

            return Json(new[] { TimeSheet }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TimeSheet_Update([DataSourceRequest] DataSourceRequest request, TimeSheet TimeSheet)
        {
            IFleetService fleetService = new FleetService();
            Fleets fleet = fleetService.GetById(TimeSheet.FleetsID);

            if (TimeSheet != null)
            {
                if (fleet.RentalRate < 1)
                {
                    ModelState.AddModelError("SetRental", "Set Proper Rental Rate");

                }

                else if (TimeSheet.WorkingHoursAfternoonFrom > TimeSheet.WorkingHoursAfternoonTo)
                {
                    ModelState.AddModelError("InvalidTime", "Starting Time Is Must Be Less Than Ending Time");
                }
                else if (TimeSheet.WorkingHoursMorningFrom > TimeSheet.WorkingHoursMorningto)
                {
                    ModelState.AddModelError("InvalidTime", "Starting Time Is Must Be Less Than Ending Time");

                }

                else
                {

                    TimeSheet.Date = DateTime.Now;
                    TimeSheet.RentalRate = TimeSheet.RentalRate;
                    TimeSheet.IdealRentalRate = TimeSheet.IdealRentalRate;
                    TimeSheet.RentType = fleet.RentType;
                    TimeSheet.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                    TimeSheet.CustomerName = fleet.Location;
                    TimeSheet.FleetTypeID = fleet.EquipmentIdentityID;
                    TimeSheet.FleetCategoryID = fleet.EquipmentTypeID;
                    TimeSheet.EquipmentName = fleet.EquipmentName;
                    TimeSheet.PlateSerialNo = fleet.PlateNumber + "-" + fleet.SerialNo;
                    TimeSheet.IsExternal = fleet.IsExternal;
                    TimeSheet.Type = MaintenanceConst.RentalTimeSheet;
                    TimeSheet.ModifiedBy = User.Identity.Name;
                    TimeSheet.LastModified = DateTime.Now;
                    TimeSheet.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                    _TimeSheetService.Update(TimeSheet);
                }
            }

            return Json(new[] { TimeSheet }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TimeSheet_Destroy([DataSourceRequest] DataSourceRequest request, TimeSheet timeSheet)
        {
            var TimeSheet = _TimeSheetService.GetById(timeSheet.TimeSheetID);
            if (TimeSheet != null)
            {
                _TimeSheetService.Delete(TimeSheet);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, " Delete Time Sheet number " + timeSheet.TimeSheetID);
            }

            return Json(new[] { TimeSheet }.ToDataSourceResult(request));
        }



    }
}

