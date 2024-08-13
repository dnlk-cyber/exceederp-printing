using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.Maintenance.Models;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers.WorkFlows
{
    [AuthorizeRoles(MaintenanceRoles.MaintenanceOrderUser)]
    public class JobOrderClosingController : Controller
    {
        IJobOrderDetailService _JobOrderDetailService;
        private IJobOrderDetailService JobOrderDetailService;
        private IJobOrderService _JobOrderService;
        private readonly IFleetService _FleetService;
        private readonly IAdministrativeApprovalService _AdministrativeApprovalService;
        private ExceedDbContext db = new ExceedDbContext();

        public const string Finished = "Finished";
        public const string Pending = "Pending";
        public const string Active = "Active";
       
        public JobOrderClosingController()
        {
            _JobOrderService = new JobOrderService();
            _FleetService = new FleetService();
            JobOrderDetailService = new JobOrderDetailService();
            _AdministrativeApprovalService = new AdministrativeJobApprovalService();
            _JobOrderDetailService = new JobOrderDetailService();
        }
        // GET: Maintenance/JobOrderClosing
        public async Task<ActionResult> Index()
        {
            ViewData["Service"] = db.RepairType.Select(s => new
            {
                Code = s.RepairTypeID,
                Name = s.Code + "-" + s.Name
            }).ToList();
            ViewData["SubCategories"] = db.ItemCategorys.Select(s => new
            {
                Code = s.ItemCategoryCode,
                Name = s.Name
            }).ToList();
            ViewData["JobCauses"] = db.JobCauses.ToList().Select(s => new
            {
                Code = s.JobCausesId,
                Name = XAPI.Common.RemoveSpecialChars(s.Description)
            }).ToList();
            ViewData["Customers"] = db.OrganizationCustomers.Select(s => new
            {
                Code = s.OrganizationCustomerID,
                Name = s.TradeName
            }).ToList();


            ViewData["MaintenanceCenter"] = db.MaintenanceCenter.Select(s => new
            {
                Id = s.MaintenanceCenterID,
                Name = s.Name
            }).ToList();


            ViewData["ServiceCenter"] = db.ServiceCenters.Select(s => new
            {
                Name = s.Name,
                Id = s.ServiceCenterId
            }).ToList();


            ViewData["EquipmentsList"] = db.EquipmentInformation.Select(e => new { e.EquipmentInformationId, e.LicensePlate, e.DeviceNumber }).ToList();
            await GetMaintainaceDue();
            ViewBag.ServiceType = db.MaintenanceType.ToList();

            //UpdateJobOrderStatus();
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var employee = db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;
            var branches = db.Branch;
            var structure = db.MainCompanyStructures;


            ViewData["Structure"] = structure;

            ViewData["Branches"] = branches;

            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                var businessUnits = db.BusinessUnits.ToList();
                ViewData["BusinessUnits"] = businessUnits;
            }

            var items = db.ApSetupItems.ToList();
            ViewData["Items"] = items;
        }
        public ActionResult JobOrderClosing_Read([DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<JobOrder> jobOrders =
                db.JobOrder
                .AsNoTracking()
                .ToList()
                .Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name))
                .Where(x => x.Status != "Closed")
                .Where(x => x.IsSubmittedToServiceCenter)
                .AsQueryable();
            DataSourceResult result = jobOrders.ToDataSourceResult(request, j => new JobOrder
            {
                IsPM = j.IsPM,
                IsRework = j.IsRework,
                CustomerId = db.JobRequest.Where(q => q.Num == j.Referance).FirstOrDefault()?.CustomerId,
                EquipmentName = j.EquipmentName,
                CurrentFuel = j.CurrentFuel,
                ProjectName = j.ProjectName,
                RejectionReason = j.RejectionReason,
                //Progress = GetProgressPercent(j.JobOrderID),
                Status = j.Status,
                PercentCompleted = j.AssetNum + "--" + j.EquipmentName,
                MaintenanceCenter = j.MaintenanceCenter,
                Num = j.JobOrderNum ?? j.Num,
                JobOrderNum = j.JobOrderNum,
                JobOrderID = j.JobOrderID,
                Type = j.Type,
                LicensePlate = j.LicensePlate,
                IsExternal = j.IsExternal,
                EquipmentInformationId = j.EquipmentInformationId,
                FailureReason = j.FailureReason,
                AdvanceMeasure = j.AdvanceMeasure,
                Referance = j.Referance,
                Odometer = j.Odometer,
                //InspectorComments = GetMaintenaceDuration(j.JobOrderID),
                IsOnlineApproved = j.IsOnlineApproved,
                SecondOnlineApprovedTime = j.SecondOnlineApprovedTime,
                OnlineApprovedTime = j.OnlineApprovedTime,
                DateCreated = j.DateCreated,

                IsSubmittedToServiceCenter = j.IsSubmittedToServiceCenter,
                IsSendForApproval = j.IsSendForApproval,
                IsSecondSendForApproval = j.IsSecondSendForApproval,
                IsSecondOnlineApproved = j.IsSecondOnlineApproved,
                IsOnlineTransferred = j.IsOnlineTransferred,
                MaintenanceCenterId = j.MaintenanceCenterId,
                BranchId = j.BranchId,
                BusinessUnitId = j.BusinessUnitId,
                OrgStructureId = j.OrgStructureId,
                CreatedAtBranchId = j.CreatedAtBranchId,
                CreatedAtBusinessUnitId = j.CreatedAtBusinessUnitId,
                IsInsured = j.IsInsured,
                Description = j.Description
            });

            return Json(result);

        }
        public async Task GetMaintainaceDue()
        {
            IFleetService _FleetService = new FleetService();
            IServiceService _ServiceService = new ServiceService();
            IPlannerService _PlannerService = new PlannerService();
            List<MaintainceDueViewModel> FinalAssetsDues = new List<MaintainceDueViewModel>();
            Guid AssetsId = Guid.Empty;
            Guid AssetID = Guid.Empty;
            List<Fleets> AssetsDue = _FleetService.GetAll().Where(x => x.Status != MaintenanceConst.OnMaintenance).ToList();
            string AssetName = "";

            int CountDue = 0;
            string IsMileageOrDate = null;
            try
            {

                foreach (var AssetsDues in AssetsDue)
                {
                    AssetID = Guid.Empty;
                    AssetName = AssetsDues.EquipmentName + "--" + AssetsDues.PlateNumber;
                    List<Service> AssetsDuess = _ServiceService.GetAll().Where(x => x.PlannerID == AssetsDues.PlannerID).ToList();// MDueMgt.getMaintenaceDueInforamation().Where(x => x.Status != "OnMaintenance" && x.AssetId == AssetsDues).ToList();
                    int TotDues = AssetsDuess.Count;

                    foreach (var Services in AssetsDuess)
                    {
                        CountDue = CountDue + 1;
                        IsMileageOrDate = null;

                        Service PMM = Services;

                        if (PMM.IsPm == MaintenanceConst.Planned)
                        {
                            if (PMM.Planner.TrackByDate)
                            {

                                if (Services.DateIntervalType == MaintenanceConst.Day)
                                {

                                    DateTime newDay = DateTime.Parse(AssetsDues.LastMaintained.ToString()).AddDays(double.Parse(Services.DateInterval.ToString()));
                                    if (DateTime.Now > newDay)
                                    {
                                        IsMileageOrDate = IsMileageOrDate + " By Date";
                                        Notify.NotifyUser(new NotifModels { DeadLine = DateTime.Now, Message = "Equipment with Asset No " + AssetsDues.UnitNum + " For Service " + Services.ServiceName + "  ", Title = " Equipment Due By Date at " + newDay.Day + "/" + newDay.Month + "/" + newDay.Year, URL = Url.Action("Index", "JobRequest", new { area = "FleetManagment" }) });

                                    }

                                }
                                if (Services.DateIntervalType == MaintenanceConst.Week)
                                {

                                    DateTime newDay = DateTime.Parse(AssetsDues.LastMaintained.ToString()).AddDays(double.Parse(Services.DateInterval.ToString()) * 7);
                                    if (DateTime.Now > newDay)
                                    {
                                        IsMileageOrDate = IsMileageOrDate + " By Date";
                                        Notify.NotifyUser(new NotifModels { DeadLine = DateTime.Now, Message = "Equipment with Asset No " + AssetsDues.UnitNum + " For Service " + Services.ServiceName + "  ", Title = " Equipment Due By Date at " + newDay.Day + "/" + newDay.Month + "/" + newDay.Year, URL = Url.Action("Index", "JobRequest", new { area = "FleetManagment" }) });

                                    }

                                }
                                else if (Services.DateIntervalType == MaintenanceConst.Month)
                                {
                                    DateTime newDay = DateTime.Parse(AssetsDues.LastMaintained.ToString()).AddMonths(int.Parse(Services.DateInterval.ToString()));
                                    if (DateTime.Now > newDay)
                                    {
                                        IsMileageOrDate = IsMileageOrDate + " By Date";
                                        Notify.NotifyUser(new NotifModels { DeadLine = DateTime.Now, Message = "Equipment with Asset No " + AssetsDues.UnitNum + " For Service " + Services.ServiceName + "  ", Title = " Equipment Due By Date at " + newDay.Day + "/" + newDay.Month + "/" + newDay.Year, URL = Url.Action("Index", "JobRequest", new { area = "FleetManagment" }) });

                                    }

                                }
                                else if (Services.DateIntervalType == MaintenanceConst.Year)
                                {
                                    DateTime newDay = DateTime.Parse(AssetsDues.LastMaintained.ToString()).AddYears(int.Parse(Services.DateInterval.ToString()));
                                    if (DateTime.Now > newDay)
                                    {
                                        IsMileageOrDate = IsMileageOrDate + " By Date";
                                        Notify.NotifyUser(new NotifModels { DeadLine = DateTime.Now, Message = "Equipment with Asset No " + AssetsDues.UnitNum + " For Service " + Services.ServiceName + "  ", Title = " Equipment Due By Date at " + newDay.Day + "/" + newDay.Month + "/" + newDay.Year, URL = Url.Action("Index", "JobRequest", new { area = "FleetManagment" }) });

                                    }

                                }
                            }

                            if (PMM.Planner.TrackByMileage)
                            {

                                int OdoMeterMeasure = int.Parse(AssetsDues.CurentOdoMeter.ToString()) - int.Parse(AssetsDues.LastMaintenanceOdometer.ToString());
                                if (OdoMeterMeasure > Services.MileageInterval)
                                {
                                    IsMileageOrDate = IsMileageOrDate + " By Mileage";
                                    Notify.NotifyUser(new NotifModels { DeadLine = DateTime.Now, Message = "Equipment with Asset No " + AssetsDues.UnitNum + " For Service " + Services.ServiceName + "  ", Title = " Equipment Due By Mileage " + OdoMeterMeasure + " KM", URL = Url.Action("Index", "JobRequest", new { area = "FleetManagment" }) });


                                }

                            }
                        }



                    }


                }
            }
            catch (Exception)
            {

            }

        }
    }
}