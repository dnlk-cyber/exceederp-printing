using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.AP.PaymentRequest;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.CRM;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Web.Areas.Maintenance.Models;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using ExceedERP.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    [AuthorizeRoles(MaintenanceRoles.MaintenanceOrderUser)]
    public class JobOrderController : Controller
    {
        private readonly CacheData CacheData = new CacheData();
        IJobOrderDetailService _JobOrderDetailService;
        private IJobOrderDetailService JobOrderDetailService;
        private IJobOrderService _JobOrderService;
        private readonly IFleetService _FleetService;
        private readonly IAdministrativeApprovalService _AdministrativeApprovalService;
        private ExceedDbContext db = new ExceedDbContext();

        public const string Finished = "Finished";
        public const string Pending = "Pending";
        public const string Active = "Active";

        public JobOrderController()
        {
            _JobOrderService = new JobOrderService();
            _FleetService = new FleetService();
            JobOrderDetailService = new JobOrderDetailService();
            _AdministrativeApprovalService = new AdministrativeJobApprovalService();
            _JobOrderDetailService = new JobOrderDetailService();
        }
        public async Task<ActionResult> ClosingJobs()
        {
            ViewData["EquipmentsList"] = db.EquipmentInformation.Select(e => new { e.EquipmentInformationId, e.LicensePlate, e.DeviceNumber }).ToList();
            await GetMaintainaceDue();
            ViewBag.ServiceType = db.MaintenanceType.ToList();
            ViewBag.setting = (db.RequestToInspections.FirstOrDefault() != null) ? db.RequestToInspections.FirstOrDefault().Remark : "";
            //UpdateJobOrderStatus();
            PopulateViewData();
            return View();
        }
        //
        // GET: /FleetManagement/JobOrder/
        public async Task<ActionResult> Index()
        {
            ViewData["ServiceGroup"] = db.ServiceGroupSettings.Select(s => new
            {
                Code = s.ServiceGroupSettingId,
                Name = s.ServiceGroupName
            }).ToList();
            ViewData["EquipmentsList"] = db.EquipmentInformation.Select(e => new { e.EquipmentInformationId, e.LicensePlate, e.DeviceNumber }).AsQueryable();
            await GetMaintainaceDue();
            ViewBag.ServiceType = db.MaintenanceType.ToList();
            ViewData["MaintenanceCenter"] = db.MaintenanceCenter.Select(x => new
            {
                Id = x.MaintenanceCenterID,
                Name = x.Name
            }

            ).ToList();
            //UpdateJobOrderStatus();
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            ViewData["Service"] = db.RepairType.Select(s => new
            {
                Code = s.RepairTypeID,
                Name = s.Code + "-" + s.Name
            }).AsQueryable();
            var inventoryPeriods = db.InventoryPeriods;
            ViewData["inventoryPeriods"] = inventoryPeriods.Select(i => new {
                Name = i.Name,
                GLPeriodId = i.InventoryPeriodId
            }).AsQueryable();
            ViewData["storesInCompany"] = db.InventoryStores.AsQueryable();
            var mechanic = db.MechanicCrew;
            var selectedCostCenter = CacheData.GetWorkUnitCaches().Select(i => new {
                Name = i.Name,
                Code = i.Id
            }).AsQueryable();
            ViewData["costCenters"] = selectedCostCenter;
            ViewData["Mechanic"] = mechanic.Select(s => new {
                Code = s.MechanicCrewID,
                Name = s.Person_Employee.FirstName + " " + s.Person_Employee.MiddleName + " " + s.Person_Employee.LastName
            }).AsQueryable();
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

            ViewData["Branches"] = CacheData.GetBranchCaches().Select(
         i => new
         {
             Name = i.Name,
             BranchId = i.Id
         }).AsQueryable();

            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                var businessUnits = CacheData.GetBusinessUnitCaches().Select(
         i => new
         {
             Name = i.Name,
             Id = i.Id
         }).AsQueryable();

                ViewData["BusinessUnits"] = businessUnits;
            }

            //var items = db.ApSetupItems.AsQueryable();

            var items = db.InventoryItems;
            ViewData["Items"] = items.Select(i => new {
                Name = i.ItemCode + "_" + i.PartNo + "_" + i.Name,
                ItemCode = i.ItemCode
            }).ToList();

            ViewData["SubCategories"] = db.ItemCategorys.Select(s => new
            {
                Code = s.ItemCategoryCode,
                Name = s.Name
            }).AsQueryable();
            ViewData["JobCauses"] = db.JobCauses.ToList().Select(s => new
            {
                Code = s.JobCausesId,
                Name = XAPI.Common.RemoveSpecialChars(s.Description)
            }).AsQueryable();
            ViewData["Customers"] = db.OrganizationCustomers.Select(s => new
            {
                Code = s.OrganizationCustomerID,
                Name = s.TradeName
            }).AsQueryable();


            ViewData["MaintenanceCenter"] = CacheData.GetMaintenanceCenterCaches().Select(
        i => new
        {
            Name = i.Name,
            Id = i.Id
        }).AsQueryable();


            ViewData["ServiceCenter"] = db.ServiceCenters.Select(s => new
            {
                Name = s.Name,
                Id = s.ServiceCenterId
            }).AsQueryable();
            ViewBag.FailureReason = db.FailureReasons.ToList();

            // Job Order Detail
            ViewData["JobOrderDetail"] = db.JobOrderServiceCenterDetails.Select(s => new {
                Code = s.JobOrderServiceCenterDetailId,
                Name = s.RepairType.Name + "-" + s.JobCauses.Name
            }).AsQueryable();

            // Service Group
            ViewData["ServiceGroup"] = db.ServiceGroupSettings.Select(s => new {
                Code = s.ServiceGroupSettingId,
                Name = s.ServiceGroupName
            }).ToList();

            // Service Type
            ViewBag.ServiceType = db.MaintenanceType.ToList();

            // Inventory Items (formatted version - since your current Items is different)
            ViewData["ItemsFormatted"] = db.InventoryItems.Select(i => new {
                Name = i.ItemCode + "_" + i.PartNo + "_" + i.Name,
                ItemCode = i.ItemCode
            }).ToList();

            // Category + Default Category
            var categories = db.ItemCategorys;
            ViewData["Category"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();

            // Stores (additional reference)
            ViewData["stores"] = db.InventoryStores;

            // Cannibalize Plates
            ViewData["CanibalizePlates"] = db.Fleets.Select(x => new {
                Name = x.PlateNumber,
                PlateNumber = x.PlateNumber
            }).AsQueryable();

            // Vehicle Parts
            ViewData["VehicleParts"] = db.VehiclePart.Select(x => new {
                Name = x.Name,
                VehiclePartId = x.VehiclePartId
            }).AsQueryable();

            // Equipment List
            ViewData["EquipmentsList"] = db.EquipmentInformation.Select(e => new {
                e.EquipmentInformationId,
                e.LicensePlate,
                e.DeviceNumber
            }).ToList();
        }

        //public JsonResult getOrder(string Num)
        //{
        //    var jobOrder = db.JobOrder.Where(q => q.Num.ToString() == Num).FirstOrDefault();

        //    return Json(jobOrder, JsonRequestBehavior.AllowGet);

        //}

        public ActionResult UpdateJobOrderSourcesToMuchJobRequest()
        {
            var jobs = db.JobOrder.ToList();
            foreach (JobOrder job in jobs)
            {
                var req = db.JobRequest.FirstOrDefault(r => r.Num == job.Referance);
                if (req != null)
                {
                    job.IsExternal = req.Source == JobRequestSource.ExternalVehicle ? true : false;

                    db.JobOrder.Attach(job);
                    db.Entry(job).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return Redirect("Index");
        }
        
        public ActionResult JobOrder_Read([DataSourceRequest] DataSourceRequest request)
        {
            // ─────────────────────────────
            // 1️⃣ Guard: Employee branch
            // ─────────────────────────────
            // var fetchSetting = db.RequestToInspections.FirstOrDefault();

            int employeeBranch = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name);
            //if (employeeBranch == null&&((fetchSetting != null && fetchSetting.Remark != "PAGRI")||(fetchSetting == null)))
            if (employeeBranch == 0)
            {
                return Json(
                    new List<object>().ToDataSourceResult(request),
                    JsonRequestBehavior.AllowGet
                );
            }

            // ─────────────────────────────
            // 2️⃣ Maintenance centers
            // ─────────────────────────────
            var maintenanceCenterIds = db.MaintenanceCenter
                .AsNoTracking()
                .Where(x => x.BranchId == employeeBranch)
                .Select(x => x.MaintenanceCenterID)
                .ToList();
            //var fetchSetting = db.RequestToInspections.FirstOrDefault();
            //if(fetchSetting!=null&&fetchSetting.Remark=="PAGRI")
            //{
            //    maintenanceCenterIds = db.MaintenanceCenter
            //    .AsNoTracking()
            //    .Select(x => x.MaintenanceCenterID)
            //    .ToList();
            //}
            // ─────────────────────────────
            // 3️⃣ Job orders (SQL-safe)
            // ─────────────────────────────
            var rawJobOrders = db.JobOrder
                .AsNoTracking()
                //.Where(j => j.MaintenanceCenterId.HasValue &&
                //            maintenanceCenterIds.Contains(j.MaintenanceCenterId.Value))
                .Select(j => new
                {
                    j.JobOrderID,
                    j.JobOrderNum,
                    j.Num,
                    j.EquipmentName,
                    j.LicensePlate,
                    j.AssetNum,
                    j.MaintenanceCenterId,
                    j.BranchId,
                    j.BusinessUnitId,
                    j.OrgStructureId,
                    j.IsPM,
                    j.IsRework,
                    j.IsInsured,
                    j.IsExternal,
                    j.Type,
                    j.CurrentFuel,
                    j.ProjectName,
                    j.RejectionReason,
                    j.FailureReason,
                    j.AdvanceMeasure,
                    j.Referance,
                    j.Odometer,
                    j.Status,
                    j.FleetsID,
                    j.IsOnlineApproved,
                    j.SecondOnlineApprovedTime,
                    j.OnlineApprovedTime,
                    j.DateCreated,
                    j.IsSubmittedToServiceCenter,
                    j.IsSendForApproval,
                    j.IsSecondSendForApproval,
                    j.IsSecondOnlineApproved,
                    j.IsOnlineTransferred,
                    j.CreatedAtBranchId,
                    j.CreatedAtBusinessUnitId,
                    j.KMReading,
                    j.GPSInfo,
                    j.Description,
                    j.RequesterId
                })
                .ToList();
            //if (fetchSetting != null && fetchSetting.Remark == "PAGRI")
            //{

            //    rawJobOrders = db.JobOrder
            //    .AsNoTracking()
            //    .Select(j => new
            //    {
            //        j.JobOrderID,
            //        j.JobOrderNum,
            //        j.Num,
            //        j.EquipmentName,
            //        j.LicensePlate,
            //        j.AssetNum,
            //        j.MaintenanceCenterId,
            //        j.BranchId,
            //        j.BusinessUnitId,
            //        j.OrgStructureId,
            //        j.IsPM,
            //        j.IsRework,
            //        j.IsInsured,
            //        j.IsExternal,
            //        j.Type,
            //        j.CurrentFuel,
            //        j.ProjectName,
            //        j.RejectionReason,
            //        j.FailureReason,
            //        j.AdvanceMeasure,
            //        j.Referance,
            //        j.Odometer,
            //        j.Status,
            //        j.FleetsID,
            //        j.IsOnlineApproved,
            //        j.SecondOnlineApprovedTime,
            //        j.OnlineApprovedTime,
            //        j.DateCreated,
            //        j.IsSubmittedToServiceCenter,
            //        j.IsSendForApproval,
            //        j.IsSecondSendForApproval,
            //        j.IsSecondOnlineApproved,
            //        j.IsOnlineTransferred,
            //        j.CreatedAtBranchId,
            //        j.CreatedAtBusinessUnitId,
            //        j.KMReading,
            //        j.GPSInfo,
            //        j.Description,
            //        j.RequesterId
            //    })
            //    .ToList();
            //}

            // ─────────────────────────────
            // 4️⃣ Reference data (in-memory)
            // ─────────────────────────────
            var jobRequests = db.JobRequest.AsNoTracking()
                .Select(r => new { r.Num, r.CustomerId })
                .ToList();

            var customers = db.OrganizationCustomers.AsNoTracking()
                .Select(c => new { c.OrganizationCustomerID, c.TradeName })
                .ToList();

            var employees = db.Person_Employee.AsNoTracking()
                .Select(e => new
                {
                    e.EmployeeId,
                    e.FirstNameEnglish,
                    e.MiddleNameEnglish,
                    e.LastNameEnglish
                })
                .ToList();

            var maintenanceCenters = db.MaintenanceCenter.AsNoTracking()
                .Select(m => new { m.MaintenanceCenterID, m.Name })
                .ToList();

            var branches = db.Branch.AsNoTracking()
                .Select(b => new { b.BranchId, b.Name })
                .ToList();

            var businessUnits = db.BusinessUnits.AsNoTracking()
                .Select(bu => new { bu.BusinessUnitId, bu.Name })
                .ToList();

            var orgStructures = db.MainCompanyStructures.AsNoTracking()
                .Select(org => new { org.OrgStructureId, org.Name })
                .ToList();

            // ─────────────────────────────
            // 5️⃣ In-memory joins (NULL SAFE)
            // ─────────────────────────────
            var resultData =
                (from j in rawJobOrders
                 join r in jobRequests on j.Referance equals r.Num into jr
                 from r in jr.DefaultIfEmpty()

                 join c in customers on r?.CustomerId equals c.OrganizationCustomerID into jc
                 from c in jc.DefaultIfEmpty()

                 join e in employees on j.RequesterId equals e.EmployeeId into je
                 from e in je.DefaultIfEmpty()

                 join m in maintenanceCenters on j.MaintenanceCenterId equals m.MaintenanceCenterID into jm
                 from m in jm.DefaultIfEmpty()

                 join b in branches on j.BranchId equals b.BranchId into jb
                 from b in jb.DefaultIfEmpty()

                 join bu in businessUnits on j.BusinessUnitId equals bu.BusinessUnitId into jbu
                 from bu in jbu.DefaultIfEmpty()

                 join org in orgStructures on j.OrgStructureId equals org.OrgStructureId into jo
                 from org in jo.DefaultIfEmpty()

                 select new
                 {
                     j.JobOrderID,

                     j.Num,
                     j.EquipmentName,
                     j.LicensePlate,

                     CustomerId = r?.CustomerId,
                     CustomerName = c?.TradeName ?? "",

                     RequesterName = e == null
                         ? ""
                         : string.Join(" ",
                             new[]
                             {
                         e.FirstNameEnglish,
                         e.MiddleNameEnglish,
                         e.LastNameEnglish
                             }.Where(x => !string.IsNullOrWhiteSpace(x))),

                     PercentCompleted = $"{j.AssetNum ?? ""}--{j.EquipmentName ?? ""}",

                     j.MaintenanceCenterId,
                     MaintenanceCenterName = m?.Name ?? "",

                     j.BranchId,
                     BranchName = b?.Name ?? "",

                     j.BusinessUnitId,
                     BusinessUnitName = bu?.Name ?? "",

                     j.OrgStructureId,
                     OrgStructureName = org?.Name ?? "",

                     j.IsPM,
                     j.IsRework,
                     j.IsInsured,
                     j.IsExternal,
                     j.Type,
                     j.CurrentFuel,
                     j.ProjectName,
                     j.RejectionReason,
                     j.FailureReason,
                     j.AdvanceMeasure,
                     j.Referance,
                     j.Odometer,
                     j.FleetsID,
                     j.Status,
                     j.IsOnlineApproved,
                     j.SecondOnlineApprovedTime,
                     j.OnlineApprovedTime,
                     j.DateCreated,
                     j.IsSubmittedToServiceCenter,
                     j.IsSendForApproval,
                     j.IsSecondSendForApproval,
                     j.IsSecondOnlineApproved,
                     j.IsOnlineTransferred,
                     j.CreatedAtBranchId,
                     j.CreatedAtBusinessUnitId,
                     j.KMReading,
                     j.GPSInfo,
                     j.Description,
                     j.RequesterId
                 }).ToList();

            // ─────────────────────────────
            // 6️⃣ Kendo result
            // ─────────────────────────────
            var result = resultData.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JobOrderClosed_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            IQueryable<JobOrder> jobOrders =
                db.JobOrder
                //.AsNoTracking()
                //.ToList()
                // .Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name))
                .Where(x => mybraches.Contains(x.CreatedAtBranchId) || x.CreatedAtBranchId == 0)
                .Where(x => x.Status == "Closed")
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
        public ActionResult JobOrderClosing_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            IQueryable<JobOrder> jobOrders =
                db.JobOrder
                 //.AsNoTracking()
                 //.ToList()
                 //.Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name))
                 .Where(x => mybraches.Contains(x.CreatedAtBranchId) || x.CreatedAtBranchId == 0)
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
                // Progress = GetProgressPercent(j.JobOrderID),
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

        public ActionResult JobOrderExtra_Read([DataSourceRequest] DataSourceRequest request, Guid Id)
        {

            FleetCostHelper fleetCostHelper = new FleetCostHelper();
            var inventoryMapping = db.InventorySparePartMapping.ToList();

            IQueryable<JobOrder> jobOrders = db.JobOrder.AsQueryable().Where(x => x.JobOrderID == Id);
            DataSourceResult result = jobOrders.ToDataSourceResult(request, j => new
            {
                IsPM = j.IsPM,
                EquipmentName = j.EquipmentName,
                CurrentFuel = j.CurrentFuel,
                ProjectName = j.ProjectName,
                Progress = GetProgressPercent(j.JobOrderID),
                Status = j.Status,
                PercentCompleted = j.AssetNum + "--" + j.EquipmentName,
                MaintenanceCenter = j.MaintenanceCenter,
                Num = j.Num,
                JobOrderNum = j.JobOrderNum,
                JobOrderID = j.JobOrderID,
                Type = j.Type,
                IsExternal = j.IsExternal,
                EquipmentInformationId = j.EquipmentInformationId,
                FailureReason = j.FailureReason,
                AdvanceMeasure = j.AdvanceMeasure,
                Referance = j.Referance,
                Odometer = j.Odometer,
                InspectorComments = GetMaintenaceDuration(j.JobOrderID),
                IsOnlineApproved = j.IsOnlineApproved,
                LabourCost = fleetCostHelper.GetLabourCost(j),
                SpareCost = fleetCostHelper.GetJobOrderSpareCost(j),
                //TyreCost = fleetCostHelper.GetJobOrderSpareCost(j, inventoryMapping, SpareType.Tyre),
                //OilLubricantCost = fleetCostHelper.GetJobOrderSpareCost(j, inventoryMapping, SpareType.Oil_Lubricant),
                //OtherCost = fleetCostHelper.GetJobOrderSpareCost(j, inventoryMapping, SpareType.Other),
                //SalvageCost = fleetCostHelper.GetSalvageSpareCost(j.JobOrderID),
                //BattreyCost = fleetCostHelper.GetJobOrderSpareCost(j, inventoryMapping, SpareType.Battery),
                Efficiency = fleetCostHelper.GetEfficiency(j.JobOrderID),
                ComponentCost = fleetCostHelper.GetComponentCost(j.JobOrderID),
                IsSendForApproval = j.IsSendForApproval,
                IsSecondSendForApproval = j.IsSecondSendForApproval,
                IsSecondOnlineApproved = j.IsSecondOnlineApproved,
                IsOnlineTransferred = j.IsOnlineTransferred


            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ServiceCenter_Read([DataSourceRequest] DataSourceRequest request)
        {
            var JobOrder = db.JobOrder.ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name))
                .Where(
                    x =>
                        x.IsSecondOnlineApproved == true && x.Type == MaintenanceConst.Internal &&
                        (x.Status == MaintenanceConst.Open || x.Status == MaintenanceConst.Closed))
                .Select(j => new JobOrder
                {
                    IsPM = j.IsPM,
                    ProjectName = j.ProjectName,
                    Status = GetProgressPercent(j.JobOrderID),
                    PercentCompleted = j.AssetNum + "--" + j.EquipmentName,
                    MaintenanceCenter = j.MaintenanceCenter,
                    JobOrderNum = j.JobOrderNum,
                    JobOrderID = j.JobOrderID,
                    Type = j.Type,
                    LicensePlate = j.LicensePlate,
                    EquipmentInformationId = j.EquipmentInformationId,
                    InspectorComments = GetMaintenaceDuration(j.JobOrderID),
                    IsOnlineApproved = j.IsOnlineApproved,
                    IsSendForApproval = j.IsSendForApproval,
                    IsSecondSendForApproval = j.IsSecondSendForApproval,
                    SpareCost = j.SpareCost,
                    LabourCost = j.LabourCost,
                    IsSecondOnlineApproved = j.IsSecondOnlineApproved



                }).OrderByDescending(x => x.DateIssued);

            return Json(JobOrder.ToDataSourceResult(request));
        }

        public ActionResult OutSource_Read([DataSourceRequest] DataSourceRequest request)
        {
            var JobOrder = db.JobOrder.Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name))
                .Where(
                    x =>
                        x.IsOnlineApproved == true && x.IsExternal == true)
                .Select(j => new JobOrder
                {
                    Num = j.Num,
                    ProjectName = j.ProjectName,
                    Status = j.Status,
                    IsPM = j.IsPM,
                    EquipmentName = j.EquipmentName,
                    MaintenanceCenter = j.MaintenanceCenter,
                    JobOrderNum = j.JobOrderNum,
                    JobOrderID = j.JobOrderID,
                    Type = j.Type,
                    EquipmentInformationId = j.EquipmentInformationId,
                    InspectorComments = j.InspectorComments,
                    IsOnlineApproved = j.IsOnlineApproved,
                    IsSendForApproval = j.IsSendForApproval,
                    IsSecondSendForApproval = j.IsSecondSendForApproval,
                    SpareCost = j.SpareCost,
                    LabourCost = j.LabourCost,
                    IsSecondOnlineApproved = j.IsSecondOnlineApproved,
                    IsOnlineTransferred = j.IsOnlineTransferred



                }).OrderByDescending(x => x.DateIssued);

            return Json(JobOrder.ToDataSourceResult(request));
        }


        public ActionResult Request_Read([DataSourceRequest] DataSourceRequest request)
        {
            var JobOrder = db.JobOrder.Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).Select(j => new JobOrder
            {
                ProjectName = j.ProjectName,
                Status = j.Status,
                PercentCompleted = j.AssetNum,
                MaintenanceCenter = j.MaintenanceCenter,
                JobOrderNum = j.JobOrderNum,
                JobOrderID = j.JobOrderID,
                EquipmentInformationId = j.EquipmentInformationId,
                Type = j.Type,
                SpareCost = j.SpareCost,
                LabourCost = j.LabourCost,
                InspectorComments = j.InspectorComments


            }).OrderByDescending(x => x.DateIssued);

            return Json(JobOrder.ToDataSourceResult(request));
        }


        public ActionResult Technical_Read([DataSourceRequest] DataSourceRequest request)
        {
            var JobOrder = db.JobOrder.ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name))
                .Where(x => x.Status == MaintenanceConst.WaitingTechnical)
                .Select(j => new JobOrder
                {
                    ProjectName = j.ProjectName,
                    Status = j.Status,
                    PercentCompleted = j.AssetNum,
                    MaintenanceCenter = j.MaintenanceCenter,
                    JobOrderNum = j.JobOrderNum,
                    JobOrderID = j.JobOrderID,
                    EquipmentInformationId = j.EquipmentInformationId,
                    Type = j.Type,
                    SpareCost = j.SpareCost,
                    LabourCost = j.LabourCost,
                    InspectorComments = j.InspectorComments


                }).OrderByDescending(x => x.DateIssued);

            return Json(JobOrder.ToDataSourceResult(request));
        }

        public ActionResult Adminstrative_Read([DataSourceRequest] DataSourceRequest request, string JobID)
        {
            var JobOrder = db.JobOrder.ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name))
                .Where(x => x.Status == MaintenanceConst.WaitingAdminstrative && x.JobOrderID == Guid.Parse(JobID))
                .Select(j => new JobOrder
                {
                    ProjectName = j.ProjectName,
                    Status = j.Status,
                    PercentCompleted = j.AssetNum,
                    MaintenanceCenter = j.MaintenanceCenter,
                    JobOrderNum = j.JobOrderNum,
                    JobOrderID = j.JobOrderID,
                    Type = j.Type,
                    EquipmentInformationId = j.EquipmentInformationId,
                    InspectorComments = j.InspectorComments


                }).OrderByDescending(x => x.DateIssued);

            return Json(JobOrder.ToDataSourceResult(request));
        }

        public ActionResult PropertyAdmin_Read([DataSourceRequest] DataSourceRequest request, string JobID)
        {
            var JobOrder = db.JobOrder.ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name))
                .Where(
                    x =>
                        x.IsExternal == true &&
                        x.JobOrderID == Guid.Parse(JobID))
                .Select(j => new JobOrder
                {
                    ProjectName = j.ProjectName,
                    Status = j.Status,
                    PercentCompleted = j.AssetNum,
                    MaintenanceCenter = j.MaintenanceCenter,
                    JobOrderNum = j.JobOrderNum,
                    JobOrderID = j.JobOrderID,
                    Type = j.Type,
                    EquipmentInformationId = j.EquipmentInformationId,
                    InspectorComments = j.InspectorComments
                    // Add approval status

                }).OrderByDescending(x => x.DateIssued);

            return Json(JobOrder.ToDataSourceResult(request));
        }



        public ActionResult OnHand_Read([DataSourceRequest] DataSourceRequest request, string Id)
        {
            List<StoreReturnItems> Returns = new List<StoreReturnItems>();
            FleetReportHelper fleetReportHelper = new FleetReportHelper();
            var JobOrder = db.JobOrder.Where(x => x.JobOrderID.ToString() == Id).FirstOrDefault();
            if (JobOrder != null)
            {
                var StoreIssues = fleetReportHelper.GetInventoryIssueItem(x => x.Issue.JobOrderNo == JobOrder.Num.ToString() || x.Issue.JobOrderNo == JobOrder.JobOrderID.ToString()).Select(s => new { Id = s.IssueID }).ToList();
                foreach (var Issue in StoreIssues)
                {
                    var StoreReturns = db.StoreReturns.Where(x =>
                        x.Reference == Issue.Id.ToString()).ToList();

                    foreach (var StoreReturn in StoreReturns)
                    {
                        var ReturnItems = StoreReturn.StoreReturnItems.Select(
                      s => new StoreReturnItems
                      {
                          CreatedBy = s.CreatedBy,
                          Quantity = s.Quantity,
                          UnitPrice = s.UnitPrice,
                          ItemCode = s.ItemCode,
                          DateCreated = s.DateCreated,
                          OrgBranchName = s.StoreReturn.Reference,
                      }).ToList();
                        Returns.AddRange(ReturnItems);

                    }

                }

            }

            return Json(Returns.ToDataSourceResult(request));
        }


        public ActionResult JobOrder_Create([DataSourceRequest] DataSourceRequest request, JobOrder JobOrder)
        {
            ModelState.Remove("JobOrderID");
            IFleetService _FleetService = new FleetService();
            _JobOrderService = new JobOrderService();
            #region data 
            if (JobOrder.RequesterId == 0)
            {
                ModelState.AddModelError("requester", "Please select requestor");
            }
            if (JobOrder.RequesterId != 0)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == JobOrder.RequesterId && x.isCurrent);
                if (placement != null)
                {
                    JobOrder.OrgStructureId = placement.OrgStructureId;
                    JobOrder.BranchId = placement.BranchId;
                    JobOrder.BusinessUnitId = placement.BusinessUnitId;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }

            JobOrder.CreatedAtBranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name);
            JobOrder.CreatedAtBusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
            #endregion

            var Orders = db.JobOrder;

            List<JobOrder> joborders =
                db.JobOrder.Where(j => j.JobOrderNum == JobOrder.JobOrderNum).ToList();
            if (joborders.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingJobOrder", " JobOrder Number Already Exists ");
            }
            else
            {
                //EquipmentInformation fleet = db.EquipmentInformation.Find(JobOrder.EquipmentInformationId);
                //fleet.Status = MaintenanceConst.OnMaintenance;
                //db.EquipmentInformation.Attach(fleet);
                //db.Entry(fleet).State = System.Data.Entity.EntityState.Modified;
                //db.SaveChanges();

                int Num = 0;

                if (Orders != null && Orders.Any())
                {
                    var LastJob = Orders.OrderByDescending(x => x.DateCreated).FirstOrDefault();
                    Num = int.Parse(LastJob.JobOrderNum.ToString()) + 1;
                }

                JobOrder.JobOrderID = Guid.NewGuid();
                //JobOrder.AssetNum = fleet.UnitNum;
                //JobOrder.EquipmentName = fleet.EquipmentName;
                JobOrder.JobOrderNum = Num;
                JobOrder.Status = MaintenanceConst.Pending;
                JobOrder.MaintenanceCenter = "";
                JobOrder.Type = "";
                JobOrder.DateIssued = DateTime.Now;
                JobOrder.CreatedBy = User.Identity.Name;
                JobOrder.DateCreated = DateTime.Now;
                JobOrder.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);


                if (JobOrder != null)
                {
                    _JobOrderService.Insert(JobOrder);
                }
            }

            return Json(new[] { JobOrder }.ToDataSourceResult(request));
        }


        public ActionResult JobOrder_Update([DataSourceRequest] DataSourceRequest request, JobOrder JobOrder)
        {
            #region data 
            if (JobOrder.RequesterId == 0)
            {
                ModelState.AddModelError("requester", "Please select requestor");
            }
            if (JobOrder.RequesterId != 0)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == JobOrder.RequesterId && x.isCurrent);
                if (placement != null)
                {
                    JobOrder.OrgStructureId = placement.OrgStructureId;
                    JobOrder.BranchId = placement.BranchId;
                    JobOrder.BusinessUnitId = placement.BusinessUnitId;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }
            #endregion
            List<JobOrder> Order =
                db.JobOrder
                    .Where(j => j.JobOrderNum == JobOrder.JobOrderNum && j.JobOrderID != JobOrder.JobOrderID)
                    .ToList();
            if (Order.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingJobOrder", " JobOrder Number Not Exists ");
            }
            else
            {
                if (JobOrder != null && Order != null)
                {
                    JobOrder jOrder = _JobOrderService.GetById(JobOrder.JobOrderID);

                    jOrder.ModifiedBy = User.Identity.Name;
                    jOrder.LastModified = DateTime.Now;
                    jOrder.LabourCost = JobOrder.LabourCost;
                    jOrder.SpareCost = JobOrder.SpareCost;
                    _JobOrderService.Update(jOrder);
                }
            }

            return Json(new[] { JobOrder }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult ReworkJobOrder(Guid id)

        {
            var request = new DataSourceRequest();
            var jobOrder = db.JobOrder.Find(id);
            if (jobOrder != null)
            {
                var reJobOrder = new JobOrder();
                reJobOrder.JobOrderID = Guid.NewGuid();
                reJobOrder.LicensePlate = jobOrder.LicensePlate;
                reJobOrder.IsRework = true;
                reJobOrder.Referance = jobOrder.Referance;
                reJobOrder.JobOrderNum = jobOrder.Num;
                reJobOrder.CreatedBy = User.Identity.Name;
                reJobOrder.DateCreated = DateTime.Now;
                reJobOrder.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                reJobOrder.Status = MaintenanceConst.Pending;
                db.JobOrder.Add(reJobOrder);
                db.SaveChanges();
                return Json(new[] { reJobOrder }.ToDataSourceResult(request, ModelState));

            }
            else
            {
                var formattedData = new
                {
                    Message = "Job Order is not available.",
                    Status = "Record_Not_Found"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult DeliverJobOrder(Guid id)

        {
            var request = new DataSourceRequest();
            var jobOrder = db.JobOrder.Find(id);
            if (jobOrder != null && jobOrder.Status == MaintenanceConst.Closed)
            {
                jobOrder.Status = MaintenanceConst.Delivered;
                jobOrder.SecondOnlineApprovedBy = User.Identity.Name;
                jobOrder.SecondOnlineApprovedTime = DateTime.Now;
                db.JobOrder.Attach(jobOrder);
                db.Entry(jobOrder).State = EntityState.Modified;
                db.SaveChanges();
                List<UserVM> list = new List<UserVM>();
                list = UserInformationHelper.GetUsersInRole(MaintenanceRoles.ServiceUser);
                foreach (var user in list)
                {
                    NotifModels jobRequesterNotif = new NotifModels
                    {
                        UserId = user.UserId,
                        Title = "Delivered Maintenance",
                        Message = $"There is delivered maintenance",
                        URL = "/Maintenance/JobOrder",

                    };
                    Notify.NotifyUser(jobRequesterNotif);
                }
                list = UserInformationHelper.GetUsersInRole(MaintenanceRoles.MaintenanceOrderUser);
                foreach (var user in list)
                {
                    NotifModels jobApproverNotif = new NotifModels
                    {
                        //UserId = UserInformationHelper.GetEmployeeUserId(empId.),
                        UserId = user.UserId,
                        Title = "Delivered Maintenance",
                        Message = $"There is delivered maintenance",
                        URL = "/Maintenance/JobOrder"

                    };
                    Notify.NotifyUser(jobApproverNotif);
                }

                return Json(new[] { jobOrder }.ToDataSourceResult(request, ModelState));


            }
            else
            {
                var formattedData = new
                {
                    Message = "Job Order is not available.",
                    Status = "Record_Not_Found"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult JobOrder_Destroy([DataSourceRequest] DataSourceRequest request, JobOrder jobOrder)
        {

            IJobOrderDetailService _JobOrderDetailService = new JobOrderDetailService();
            IServiceSpareService _ServiceSpareService = new ServiceSpareService();
            var JobOrder = _JobOrderService.GetById(jobOrder.JobOrderID);
            JobOrder.IsSecondOnlineApproved = true;
            JobOrder.SecondOnlineApprovedBy = User.Identity.Name;
            JobOrder.SecondOnlineApprovedTime = DateTime.Now;
            JobOrder.Status = MaintenanceConst.Closed;
            var updateStatus = _JobOrderService.Update(JobOrder);
            var maintenanceStage = db.MaintenanceStage.FirstOrDefault(x => x.JobRequestId == JobOrder.JobOrderID.ToString());
            maintenanceStage.ModifiedBy = User.Identity.Name;
            maintenanceStage.LastModified = DateTime.Now;
            maintenanceStage.Description = "Vehicle repaired and Closed you can take your vehicle";

            db.Entry(maintenanceStage).State = EntityState.Modified;
            db.SaveChanges();
            var jobOrderDetail = db.JobOrderDetail.Where(x => x.JobOrderID == JobOrder.JobOrderID).ToList();
            var jobRequests = db.JobRequest.Where(x => x.Num == JobOrder.Referance).ToList();
            foreach (var item in jobOrderDetail)
            {
                item.Status = MaintenanceConst.Closed;
                db.JobOrderDetail.Attach(item);
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                //_JobOrderDetailService.Update(item);
            }

            foreach (var jobRequest in jobRequests)
            {
                jobRequest.JobOrderStatus = JobOrderStatus.Delivered;
                jobRequest.DeliveredDate = DateTime.Now;
                db.JobRequest.Attach(jobRequest);
                db.Entry(jobRequest).State = EntityState.Modified;
                db.SaveChanges();
            }
            if (updateStatus)
            {
                //var Fleet = db.EquipmentInformation.FirstOrDefault(j => j.LicensePlate == jobOrder.LicensePlate);
                //if(Fleet != null)
                //{
                //    Fleet.Status = MaintenanceConst.ACTIVE;
                //    Fleet.OdometerOnLastMaintenance = Fleet.CurrentOdometer;
                //    Fleet.FutureMaintenanceAlert = FutureMaintenanceAlert.Off;
                //    Fleet.OdometerOnNextRegularMaintenance = Fleet.OdometerOnLastMaintenance + Fleet.RegularMaintenanceInterval;
                //    Fleet.RemainingKmForUpComingMaintenance = Fleet.OdometerOnNextRegularMaintenance - Fleet.CurrentOdometer;
                //    Fleet.LastMaintained = DateTime.Now;

                //    db.EquipmentInformation.Attach(Fleet);
                //    db.Entry(Fleet).State = System.Data.Entity.EntityState.Modified;
                //    db.SaveChanges();
                //    Notify.NotifyUser(new NotifModels
                //    {
                //        DeadLine = DateTime.Now,
                //        Message = "Equipment with " + Fleet.LicensePlate + " Finished JobOrder ",
                //        Title = " Finished JobOrder ",
                //        URL = Url.Action("Index", "JobOrder", new { area = "FleetManagment" }),
                //    });
                //}
                CRMMaintenanceIntegrationHelper.CreateSalesQuoteVoucherFromMaintenance(JobOrder, User.Identity.Name);

                var maintenanceCenter = db.MaintenanceCenter.FirstOrDefault(x => x.MaintenanceCenterID == JobOrder.MaintenanceCenterId);
                if (maintenanceCenter != null)
                {
                    if (maintenanceCenter.IsExternal)
                    {
                        GenerateExternalPaymentRequest(jobOrder.Num);
                    }
                }
            }

            // return Json(new[] { JobOrder }.ToDataSourceResult(request));
            return Redirect("Index");

            //return Json(new[] { jobOrder }.ToDataSourceResult(request));



        }
        #region ExternalMaintenancePaymentRequest
        public void GenerateExternalPaymentRequest(int num)
        {
            var jobOrder = db.JobOrder.FirstOrDefault(x => x.Num == num);
            if (jobOrder != null)
            {
                var serviceCenterJob = db.ServiceCenterJob.FirstOrDefault(x => x.Num == num);
                if (serviceCenterJob != null)
                {
                    ExternalMaintenancePaymentRequest externalMaintenancePaymentRequest = new ExternalMaintenancePaymentRequest
                    {
                        BranchId = jobOrder.BranchId,
                        BusinessUnitId = jobOrder.OrgStructureId,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Today,
                        EnteranceDate = jobOrder.DateCreated,
                        EquipmentName = jobOrder.EquipmentName,
                        ExitDate = jobOrder.DateCompleted,
                        FleetId = jobOrder.FleetsID,
                        MaintenanceCenterId = serviceCenterJob.MaintenanceCenterId,
                        JobNum = (int)serviceCenterJob.Num
                    };
                    db.ExternalMaintenancePaymentRequests.Add(externalMaintenancePaymentRequest);
                    db.SaveChanges();
                    var payments = db.ExternalServiceCenterPayments.Where(x => x.ServiceCenterJobId == serviceCenterJob.ServiceCenterJobId);
                    if (payments.Any())
                    {
                        foreach (var payment in payments)
                        {
                            ExternalMaintenancePaymentRequestDetail detail = new ExternalMaintenancePaymentRequestDetail
                            {
                                CostType = payment.CostType,
                                Date = payment.Date,
                                ExternalMaintenancePaymenntrequestId = externalMaintenancePaymentRequest.ExternalMaintenancePaymenntrequestId,
                                GlTaxRateId = payment.GlTaxRateId,
                                SubTotal = payment.SubTotal,
                                UnitPrice = payment.UnitPrice,
                                RepairDescribtion = "",
                                Total = payment.Total,
                                Quantity = payment.Quantity,
                                CreatedBy = User.Identity.Name,
                                DateCreated = DateTime.Now.Date,
                                ModifiedBy = User.Identity.Name,
                                LastModified = DateTime.Now.Date,
                            };
                            db.ExternalMaintenancePaymentRequestDetails.Add(detail);
                        }

                        db.SaveChanges();
                    }
                }


            }

        }
        #endregion
        public string GetJobStatus(string Id)
        {

            JobOrder JobRequests = db.JobOrder.Where(j => j.JobOrderID.ToString() == Id && (j.IsOnlineTransferred == true || j.IsOnlineApproved == true || j.IsSendForApproval == true)).FirstOrDefault();
            if (JobRequests != null)
            {
                return JobRequests.Status;
            }

            return "Pending";


        }


        public ActionResult TechnicalApproval_Read([DataSourceRequest] DataSourceRequest request, string Id)
        {
            var Admin = new List<TechnicalApprovalViewModel>();
            Guid JId = Guid.Parse(Id);
            var Request = db.JobOrder.Where(x => x.JobOrderID.ToString() == Id).FirstOrDefault();
            if (Request != null)
            {
                if (Request.IsOnlineApproved)
                {
                    Admin =
                        db.TechnicalJobApproval
                            .Where(x => x.TechnicalJobId.ToString() == Id).Select(s => new TechnicalApprovalViewModel
                            {
                                JobOrderID = JId,
                                DateCreated = s.DateCreated,
                                CreatedBy = s.CreatedBy,
                                ApproveStatus = ApprovalStatuses.Approve,
                                //Type = Request.IsExternal ? "EXTERNAL" : "INTERNAL",
                                MaintenanceCenter = Request.MaintenanceCenter

                            })
                            .ToList();


                }

                if (Request.IsSendForApproval && !Request.IsOnlineApproved)
                {



                    var adminApproval = new TechnicalApprovalViewModel
                    {
                        JobOrderID = JId,

                        Status = ApprovalStatuses.Pending,
                        Remark = string.Empty,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now
                    };

                    Admin.Add(adminApproval);

                }
            }
            return Json(Admin.ToDataSourceResult(request));
        }


        public ActionResult Administrative_Approval([DataSourceRequest] DataSourceRequest request, JobOrder jobOrder)
        {

            var JobOrder = _JobOrderService.GetById(jobOrder.JobOrderID);
            JobOrder.Status = MaintenanceConst.Approved;

            if (JobOrder != null)
            {

                _JobOrderService.Update(JobOrder);

            }
            List<UserVM> list = new List<UserVM>();
            list = UserInformationHelper.GetUsersInRole(MaintenanceRoles.ServiceUser);
            foreach (var user in list)
            {
                NotifModels jobRequesterNotif = new NotifModels
                {
                    UserId = user.UserId,
                    Title = "Delivered Maintenance",
                    Message = $"There is delivered maintenance",
                    URL = "/Maintenance/JobOrder",

                };
                Notify.NotifyUser(jobRequesterNotif);
            }
            list = UserInformationHelper.GetUsersInRole(MaintenanceRoles.MaintenanceOrderUser);
            foreach (var user in list)
            {
                NotifModels jobApproverNotif = new NotifModels
                {
                    //UserId = UserInformationHelper.GetEmployeeUserId(empId.),
                    UserId = user.UserId,
                    Title = "Delivered Maintenance",
                    Message = $"There is delivered maintenance",
                    URL = "/Maintenance/JobOrder"

                };
                Notify.NotifyUser(jobApproverNotif);
            }

            return Json(new[] { JobOrder }.ToDataSourceResult(request));
        }


        public ActionResult Operator_SendForApproval(string JobOrderID)
        {

            var JobOrder = _JobOrderService.GetById(Guid.Parse(JobOrderID));
            JobOrder.Status = MaintenanceConst.WaitingTechnical;
            JobOrder.SendForApprovalBy = User.Identity.Name;
            JobOrder.SendForApprovalTime = DateTime.Now;
            if (JobOrder != null)
            {

                _JobOrderService.Update(JobOrder);

            }
            ViewBag.ServiceType = db.MaintenanceType.ToList();

            return View("Index");
        }

        public JsonResult Technical_SendForApproval(string Id)
        {
            DataSourceRequest request = new DataSourceRequest();
            var JobOrder = _JobOrderService.GetById(Guid.Parse(Id));
            var jobOrderDetail = db.JobOrderDetail.Where(q => q.JobOrderID.ToString() == Id).Count();
            if (jobOrderDetail == 0)
            {
                var FormattedData = new
                {
                    Message = "Job Services Should be added first",
                    Status = "NOTOK"
                };
                return Json(FormattedData, JsonRequestBehavior.AllowGet);
            }
            JobOrder.Status = MaintenanceConst.WaitingApproval;
            JobOrder.SendForApprovalBy = User.Identity.Name;
            JobOrder.IsSendForApproval = true;
            JobOrder.SendForApprovalTime = DateTime.Now;
            
            if (JobOrder != null && jobOrderDetail != 0)
            {

                _JobOrderService.Update(JobOrder);
                // MaintenanceRoles.ServiceUser, MaintenanceRoles.MaintenanceOrderUser
                List<UserVM> list = new List<UserVM>();
                //MaintenanceRoles.MaintenanceOrderUser MaintenanceRoles.MaintenanceRequestor, MaintenanceRoles.MaintenanceRequestApprover, MaintenanceRoles.MaintenanceServiceApprover
                list = UserInformationHelper.GetUsersInRole(MaintenanceRoles.MaintenanceOrderUser);
                foreach (var user in list)
                {
                    NotifModels jobRequesterNotif = new NotifModels
                    {
                        UserId = user.UserId,
                        Title = "Technical to be approved",
                        Message = "There is new technical document to be approved",
                        DeadLine = DateTime.Now,
                        URL = Url.Action("Index", "JobOrder", new { area = "Maintenance" }),
                    };


                    Notify.NotifyUser(jobRequesterNotif);
                }
                List<UserVM> maintenanceServiceUser = new List<UserVM>();
                //MaintenanceRoles.MaintenanceOrderUser MaintenanceRoles.MaintenanceRequestor, MaintenanceRoles.MaintenanceRequestApprover, MaintenanceRoles.MaintenanceServiceApprover
                list = UserInformationHelper.GetUsersInRole(MaintenanceRoles.ServiceUser);
                foreach (var user in list)
                {
                    NotifModels jobRequesterNotif = new NotifModels
                    {
                        UserId = user.UserId,
                        Title = "Technical to be approved",
                        Message = "There is new technical document to be approved",
                        DeadLine = DateTime.Now,
                        URL = Url.Action("Index", "JobOrder", new { area = "Maintenance" }),

                    };


                    Notify.NotifyUser(jobRequesterNotif);
                }


            }
            ViewBag.ServiceType = db.MaintenanceType.ToList();
            var formatedData = new
            {
                Message = "Record send for approval",
                Status = "OK"
            };
            return Json(formatedData, JsonRequestBehavior.AllowGet);
        }
        [AuthorizeRoles(MaintenanceRoles.MaintenanceVoid)]
        public ActionResult MakeVoid(string Id)
        {
            Guid.TryParse(Id, out Guid jobId);
            var jobOrder = db.JobOrder.Find(jobId);
            var jobOrderDetail = db.JobOrderDetail.Where(q => q.JobOrderID == jobOrder.JobOrderID);
            var serviceCenter = db.ServiceCenterJob.FirstOrDefault(x => x.JobOrderID == jobOrder.JobOrderID);
            db.ServiceCenterJob.Remove(serviceCenter);
            db.SaveChanges();
            foreach (var item in jobOrderDetail)
            {
                item.ApprovalStatus = ApprovalStatuses.Pending;

                db.Entry(item).State = EntityState.Modified;

            }
            db.SaveChanges();

            jobOrder.IsOnlineApproved = false;
            jobOrder.IsSendForApproval = false;
            db.Entry(jobOrder).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult UpdateType(string Id, string[] JobType)
        {
            string Type = string.Empty;
            try
            {
                foreach (var type in JobType)
                {
                    Type = Type + " - " + type;
                }

                var JobOrder = _JobOrderService.GetById(Guid.Parse(Id));
                if (JobOrder != null)
                {
                    JobOrder.Type = Type;
                    JobOrder.ModifiedBy = User.Identity.Name;
                    JobOrder.LastModified = DateTime.Now;

                    _JobOrderService.Update(JobOrder);
                }
                ViewBag.ServiceType = db.MaintenanceType.ToList();
            }
            catch (Exception)
            { }

            return RedirectToAction("Index", "JobOrder");
        }



        public ActionResult UpdateAdvanceMeasure(string Id, string JobType)
        {
            try
            {


                var JobOrder = _JobOrderService.GetById(Guid.Parse(Id));
                if (JobOrder != null)
                {
                    JobOrder.AdvanceMeasure = JobType;
                    JobOrder.ModifiedBy = User.Identity.Name;
                    JobOrder.LastModified = DateTime.Now;

                    _JobOrderService.Update(JobOrder);
                }
                ViewBag.ServiceType = db.MaintenanceType.ToList();
            }
            catch (Exception)
            { }

            return RedirectToAction("Index", "ServiceCenter");
        }


        public ActionResult UpdateFailureReason(string Id, string JobType)
        {
            try
            {


                var JobOrder = _JobOrderService.GetById(Guid.Parse(Id));
                if (JobOrder != null)
                {
                    JobOrder.FailureReason = JobType;
                    JobOrder.ModifiedBy = User.Identity.Name;
                    JobOrder.LastModified = DateTime.Now;

                    _JobOrderService.Update(JobOrder);
                }
                ViewBag.ServiceType = db.MaintenanceType.ToList();
            }
            catch (Exception)
            { }

            return RedirectToAction("Index", "WorkShop");
        }


        public ActionResult Admin_SendForApproval(string JobOrderID)
        {

            var JobOrder = _JobOrderService.GetById(Guid.Parse(JobOrderID));
            JobOrder.Status = MaintenanceConst.Approved;
            JobOrder.SendForApprovalBy = User.Identity.Name;
            JobOrder.SendForApprovalTime = DateTime.Now;
            if (JobOrder != null)
            {

                _JobOrderService.Update(JobOrder);

            }
            ViewBag.ServiceType = db.MaintenanceType.ToList();

            return View("Index");
        }

        public void CleanJobOrder()
        {
            var jobOrders = db.JobOrder.ToList();
            foreach (var jobOrder in jobOrders)
            {
                var fleet = db.Fleets.FirstOrDefault(x => x.PlateNumber == jobOrder.LicensePlate || x.SerialNo == jobOrder.LicensePlate);
                if (fleet != null)
                {
                    jobOrder.FleetsID = fleet.FleetsID;
                    db.Entry(jobOrder).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
        }

        public void UpdateJobOrderStatus()
        {
            var jobOrders = db.JobOrder.Where(x => x.Status == MaintenanceConst.Open).ToList();

            foreach (var job in jobOrders)
            {
                var services = db.ServiceCenterJob
                    .Where(x => x.JobOrderID == job.JobOrderID).ToList();

                if (services.Any())
                {
                    var uncompleted = services.Where(x => x.Status != MaintenanceConst.Closed).ToList();
                    if (!uncompleted.Any())
                    {
                        job.IsSubmittedToServiceCenter = true;
                        db.JobOrder.Attach(job);
                        db.Entry(job).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

            }
        }
        #region Support Method
        public string GetProgressPercent(Guid Id)
        {
            try
            {

                decimal RequestOrderLine = 0;
                decimal RequestOrderLineFinished = 0;
                var Order = _JobOrderService.GetById(Id);
                var Requests = db.JobRequest.Where(x => x.Num == Order.Referance).FirstOrDefault();
                if (Requests != null)
                {
                    RequestOrderLine = db.JobOrderDetail.Where(x => x.JobOrderID == Requests.JobRequestId).ToList().Count;
                    RequestOrderLineFinished = db.JobOrderDetail.Where(x => x.JobOrderID == Requests.JobRequestId && x.Status == MaintenanceConst.Completed).ToList().Count;
                }
                decimal OrderLine = db.JobOrderDetail.Where(x => x.JobOrderID == Id).ToList().Count;
                decimal OrderLineFinished = db.JobOrderDetail.Where(x => x.JobOrderID == Id && x.Status == MaintenanceConst.Completed).ToList().Count;
                var progress = (OrderLineFinished + RequestOrderLineFinished) / (OrderLine + RequestOrderLine);

                progress = progress * 100;
                return ((int)progress).ToString();

            }
            catch (Exception)
            {

                return "0";
            }
        }
        public string GetMaintenaceDuration(Guid Id)
        {
            try
            {
                var OrderLine = db.JobOrder.Where(x => x.JobOrderID == Id && x.IsOnlineApproved == true).FirstOrDefault();

                if (!OrderLine.IsOnlineTransferred)
                {
                    TimeSpan Duration = DateTime.Now - DateTime.Parse(OrderLine.OnlineApprovedTime.ToString());
                    return Duration.Days + " Days and " + Duration.Hours + " Hours";
                }

                TimeSpan FinishedDuration = DateTime.Parse(OrderLine.OnlineTransferredTime.ToString()) - DateTime.Parse(OrderLine.OnlineApprovedTime.ToString());
                return FinishedDuration.Days + " Days and " + FinishedDuration.Hours + " Hours";
            }
            catch (Exception)
            {

                return "0";
            }

        }
        public double GetMaintenaceDurationNum(Guid Id)
        {
            try
            {


                var OrderLine = db.JobOrder.Where(x => x.JobOrderID == Id).FirstOrDefault();

                if (!OrderLine.IsOnlineTransferred)
                {
                    TimeSpan Duration = DateTime.Now - DateTime.Parse(OrderLine.OnlineApprovedTime.ToString());
                    return Duration.TotalHours;
                }

                TimeSpan FinishedDuration = DateTime.Parse(OrderLine.OnlineTransferredTime.ToString()) - DateTime.Parse(OrderLine.OnlineApprovedTime.ToString());
                return FinishedDuration.TotalHours;
            }
            catch (Exception)
            {

                return 0;
            }
        }
        public decimal GetLabourCost(Guid Id)
        {
            ExceedDbContext db = new ExceedDbContext();
            IServiceCrewService _ServiceCrewService = new ServiceCrewService();
            try
            {
                decimal Cost = 0;

                var JobOrder = db.JobOrder.Where(x => x.JobOrderID == Id).FirstOrDefault();

                var Request = db.JobRequest.Where(x => x.Num == JobOrder.Referance).FirstOrDefault();

                var Services = db.JobOrderDetail.Where(x => x.JobOrderID == JobOrder.JobOrderID || x.JobOrderID == Request.JobRequestId).ToList();
                foreach (var Service in Services)
                {

                    var Crews = _ServiceCrewService.GetAll().Where(x => x.ServiceID == Service.ServiceID).ToList();



                    foreach (var Crew in Crews)
                    {

                        var WorkLogs = db.ServiceCrewWorkLogs.Where(x => x.MecanicId == Crew.MechanicCrewID && x.IsOnlineApproved == true && x.JobOrderDetailId == Service.JobOrderDetailID).ToList();

                        foreach (var WorkLog in WorkLogs)
                        {
                            var Duration = ((DateTime.Parse(WorkLog.WorkingHoursMorningto.ToString()) - DateTime.Parse(WorkLog.WorkingHoursMorningFrom.ToString())).TotalHours) + ((DateTime.Parse(WorkLog.WorkingHoursAfternoonTo.ToString()) - DateTime.Parse(WorkLog.WorkingHoursAfternoonFrom.ToString())).TotalHours);
                            Cost = Cost + (decimal.Parse(Duration.ToString()) * Crew.MechanicCrews.HourlyWage);

                        }
                    }
                }

                return Cost;
            }

            catch (Exception)
            {

                return 0;
            }
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

        #endregion


        public ActionResult Unique(string field)
        {
            var result = db.JobOrder.ToList().Distinct(new JobOrderComparer(field));
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
    public class JobOrderComparer : IEqualityComparer<JobOrder>
    {
        private string field;

        private PropertyInfo prop;

        public JobOrderComparer(string field)
        {
            this.field = field;
            prop = typeof(JobOrder).GetProperty(field);
        }
        public bool Equals(JobOrder x, JobOrder y)
        {
            var valueX = prop.GetValue(x, null);
            var valueY = prop.GetValue(y, null);
            if (valueX == null)
            {
                return valueY == null;
            }
            return valueX.Equals(valueY);
        }

        public int GetHashCode(JobOrder obj)
        {
            var value = prop.GetValue(obj, null);
            if (value == null)
            {
                return 0;
            }
            return value.GetHashCode();
        }

    }
}

