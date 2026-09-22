using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class DistributionTimeSheetController : Controller
    {

        ITimeSheetService _TimeSheetService;
        IFleetService _FleetService;
        ExceedDbContext db = new ExceedDbContext();
        public DistributionTimeSheetController()
        {
            _TimeSheetService = new TimeSheetService();
            _FleetService = new FleetService();
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
                return fleet.UnitNum + fleet.PlateNumber;
            }
            return string.Empty;
        }

        public ActionResult TimeSheet_Read([DataSourceRequest] DataSourceRequest request)
        {
            var TimeSheet = db.TimeSheet.Where(x => x.Type == MaintenanceConst.DistributionTimeSheet && x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).Select(t => new TimeSheet

                {
                    Date = t.Date,
                    RentalRate = t.RentalRate,
                    IdleTime = t.IdleTime,
                    DownTime = t.DownTime,
                    CustomerName = t.CustomerName,
                    EquipmentAdministratorName = t.EquipmentAdministratorName,
                    EquipmentName = t.EquipmentName ,
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
                    DateCreated = t.DateCreated,
                    LastModified = t.LastModified,
                    CreatedBy = t.CreatedBy,
                    ModifiedBy = t.ModifiedBy

                });

            return Json(TimeSheet.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TimeSheet_Create([DataSourceRequest] DataSourceRequest request, TimeSheet TimeSheet)
        {
            ModelState.Remove("TimeSheetID");
            ModelState.Remove("CustomerName");
            ModelState.Remove("FleetsID");
            ModelState.Remove("FleetTypeID");
            ModelState.Remove("FleetCategoryID");
            try
            {
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

                        if (TimeSheet.DateCreated == null)
                        {
                            TimeSheet.DateCreated = DateTime.Now;
                        }


                        TimeSheet.TimeSheetID = Guid.NewGuid();
                        TimeSheet.Date = DateTime.Now;
                        TimeSheet.RentalRate = fleet.RentalRate;
                        TimeSheet.IdealRentalRate = fleet.IdealRentalRate;
                        TimeSheet.EquipmentName = fleet.EquipmentName;
                        TimeSheet.PlateSerialNo = fleet.PlateNumber + "-" + fleet.SerialNo;
                        TimeSheet.CustomerName = fleet.Location;
                        TimeSheet.Type = MaintenanceConst.DistributionTimeSheet;
                        TimeSheet.RentType = fleet.RentType;
                        TimeSheet.FleetTypeID = fleet.EquipmentIdentityID;
                        TimeSheet.FleetCategoryID = fleet.EquipmentTypeID;
                        TimeSheet.CreatedBy = User.Identity.Name;
                        TimeSheet.DateCreated = TimeSheet.DateCreated;
                        TimeSheet.Status = MaintenanceConst.Pending;
                        TimeSheet.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

                        _TimeSheetService.Insert(TimeSheet);
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "Distribution timesheet registration", "Success", User.Identity.Name, $"created distributed timesheet {TimeSheet.TimeSheetID}", Modules.FleetManagement);
                        #endregion

                    }
                }
            }
            catch (DbEntityValidationException dbEx)
            {

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
                TimeSheet.EquipmentName = fleet.EquipmentName;
                TimeSheet.PlateSerialNo = fleet.PlateNumber + "-" + fleet.SerialNo;
                TimeSheet.RentalRate = fleet.RentalRate;
                TimeSheet.ModifiedBy = User.Identity.Name;
                TimeSheet.LastModified = DateTime.Now;
                TimeSheet.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

                _TimeSheetService.Update(TimeSheet);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Distributed timesheet registration", "Success", User.Identity.Name, $"updated distributed timesheet {TimeSheet.TimeSheetID}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { TimeSheet }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TimeSheet_Destroy([DataSourceRequest] DataSourceRequest request, TimeSheet timeSheet)
        {
            var TimeSheet = _TimeSheetService.GetById(timeSheet.TimeSheetID);
            if (TimeSheet != null)
            {
                _TimeSheetService.Delete(TimeSheet);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, " Delete timesheet number " + timeSheet.TimeSheetID);
            }

            return Json(new[] { TimeSheet }.ToDataSourceResult(request));
        }
        
    }
}

