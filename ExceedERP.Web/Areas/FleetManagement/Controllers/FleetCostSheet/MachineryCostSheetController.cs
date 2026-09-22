using System.Data.Entity;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Helpers.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.FleetManagement;
using System.Collections.Generic;
using ExceedERP.Reporting.Fleet.ViewModel;
//using ExceedERP.Reporting.Fleet.ViewModel;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetCostSheet
{
    [AuthorizeRoles(FleetManagementRoles.FleetCostSheetUser, FleetManagementRoles.FleetCostSheetApprover)]
    public class MachineryCostSheetController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();
        public ActionResult Index()
        {
            ViewData["RepairTypes"] = db.RepairType.Select(s => new
            {
                Code = s.RepairTypeID,
                Name = s.Name
            }).AsQueryable();
            var fetchSetting = db.RequestToInspections.FirstOrDefault();
            string isECWC = (fetchSetting != null && fetchSetting.Remark == "ECWC") ? "ECWC" : "Not";
            ViewBag.setting = (fetchSetting != null) ? fetchSetting.Remark : "";
            // Store in ViewBag (or use a strongly typed ViewModel if preferred)
            ViewData["IsECWC"] = isECWC;
            //ViewData["category"] = db.EquipmentIdentity.Select(s => new
            //{
            //    Text = s.EquipmentCategoryName,
            //    Value = s.EquipmentIdentityID
            //}).ToList();
            //ViewData["type"] = db.EquipmentType.Select(s => new
            //{
            //    Text = s.EquipmentTypeName,
            //    Value = s.EquipmentTypeID
            //}).ToList();
            //ViewData["name"] = db.EquipmentName.Select(s => new
            //{
            //    Text = s.Name,
            //    Value = s.EquipmentNameID
            //}).ToList();
            ViewData["category"] = CacheData.GetEquipmentIdentityCaches().Select(
      i => new
      {
          Text = i.Name,
          Value = i.Id
      }).AsQueryable();
            ViewData["type"] = CacheData.GetEquipmentTypeCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();
            ViewData["name"] = CacheData.GetEquipmentNameCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();
            ViewData["fleet"] = db.Fleets.Select(x => new
            {
                Value = x.FleetsID,
                Text = x.PlateNumber+" "+x.SerialNo+" "+x.Model
            }).AsQueryable();
            ViewData["operator"] = db.Operator.Select(s => new
            {
                Value = s.OperatorID,
                Text = s.FullName
            }).AsQueryable();
            ViewData["branch"] = CacheData.GetBranchCaches().Select(
     i => new
     {
         Name = i.Name,
         BranchId = i.Id
     }).AsQueryable();
            ViewData["businessunit"] = CacheData.GetBusinessUnitCaches().Select(
         i => new
         {
             Name = i.Name,
             Id = i.Id
         }).AsQueryable();

            ViewData["ProcurementSupplier"] = db.ProcurementSuppliers.Select(x => new { x.ProcurementSupplierID, x.Name }).AsQueryable();

            return View();
        }
       
        public ActionResult MachineryCostSheet_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            var machineryWorkingTimeSheets = db.MachineryWorkingTimeSheets.Where(x => x.TimeSheetApproved)
                .Where(x => mybraches.Contains(x.BranchId));
            DataSourceResult result = machineryWorkingTimeSheets.ToDataSourceResult(request, machineryWorkingTimeSheet => new MachineryWorkingTimeSheet
            {
                
                FleetsID = machineryWorkingTimeSheet.FleetsID,
                MachineryWorkingTimeSheetId = machineryWorkingTimeSheet.MachineryWorkingTimeSheetId,
                Date= machineryWorkingTimeSheet.Date,
                MorningTimeStart= machineryWorkingTimeSheet.MorningTimeStart,
                MorningTimeEnd= machineryWorkingTimeSheet.MorningTimeEnd,
                AfternoonTimeStart= machineryWorkingTimeSheet.AfternoonTimeStart,
                AfternoonTimeEnd = machineryWorkingTimeSheet.AfternoonTimeEnd,
                NightTimeStart= machineryWorkingTimeSheet.NightTimeStart,
                NightTimeEnd= machineryWorkingTimeSheet.NightTimeEnd,
                NumberOfHoles= machineryWorkingTimeSheet.NumberOfHoles,
                HoleDepth= machineryWorkingTimeSheet.HoleDepth,
                FuelConsumption= machineryWorkingTimeSheet.FuelConsumption,
                FuelRate = machineryWorkingTimeSheet.FuelRate,
                IdleHours= machineryWorkingTimeSheet.IdleHours,
                IdleReason= machineryWorkingTimeSheet.IdleReason,
                DownHours= machineryWorkingTimeSheet.DownHours,
                DownReason= machineryWorkingTimeSheet.DownReason,
                NetHour= machineryWorkingTimeSheet.NetHour,
                ExternalOperatorName = machineryWorkingTimeSheet.ExternalOperatorName,
                WorkType= machineryWorkingTimeSheet.WorkType,
                TotalTime = machineryWorkingTimeSheet.TotalTime,
                Remark = machineryWorkingTimeSheet.Remark,
                RentalRate = machineryWorkingTimeSheet.RentalRate,
                IdealRentalRate = machineryWorkingTimeSheet.IdealRentalRate,
                ProcurementSupplierID = machineryWorkingTimeSheet.ProcurementSupplierID,
                OperationAmount = machineryWorkingTimeSheet.OperationAmount,
                IdleRate = machineryWorkingTimeSheet.IdleRate,
                IdleAmount = machineryWorkingTimeSheet.IdleAmount,
                TotalAmountBeforeVAT = machineryWorkingTimeSheet.TotalAmountBeforeVAT,
                VAT = machineryWorkingTimeSheet.VAT,
                InitialEHR = machineryWorkingTimeSheet.InitialEHR,
                FinalEHR = machineryWorkingTimeSheet.FinalEHR,
                DifferenceEHR = machineryWorkingTimeSheet.DifferenceEHR,
                FuelType = machineryWorkingTimeSheet.FuelType,
                OilType = machineryWorkingTimeSheet.OilType,
                ReasonForDown = machineryWorkingTimeSheet.ReasonForDown,
                ReasonForIdle = machineryWorkingTimeSheet.ReasonForIdle,
                TotalAmount = machineryWorkingTimeSheet.TotalAmount,
                FuelCost = machineryWorkingTimeSheet.FuelCost,
                ServiceRecharge = machineryWorkingTimeSheet.ServiceRecharge,


                SparePartCost = machineryWorkingTimeSheet.SparePartCost,
                OilCost = machineryWorkingTimeSheet.OilCost,
                GreaseConsumption = machineryWorkingTimeSheet.GreaseConsumption,
                GreaseCost = machineryWorkingTimeSheet.GreaseCost,
                MaintenanceCost = machineryWorkingTimeSheet.MaintenanceCost,
                OilConsumption = machineryWorkingTimeSheet.OilConsumption,

                TotalCost = machineryWorkingTimeSheet.TotalCost,
                NetAmount = machineryWorkingTimeSheet.NetAmount,
                LabourCost = machineryWorkingTimeSheet.LabourCost,
                BatteryCost = machineryWorkingTimeSheet.BatteryCost,
                OverTime = machineryWorkingTimeSheet.OverTime,
                BasicSalary = machineryWorkingTimeSheet.BasicSalary,
                DownHourRate = machineryWorkingTimeSheet.DownHourRate,
                OperationRevenue = machineryWorkingTimeSheet.OperationRevenue,
                Perdium = machineryWorkingTimeSheet.Perdium,
                Salary = machineryWorkingTimeSheet.Salary,
                TireCost = machineryWorkingTimeSheet.TireCost,  
                BranchId = machineryWorkingTimeSheet.BranchId,
                BusinessUnitId = machineryWorkingTimeSheet.BusinessUnitId,
                CostSheetApproved = machineryWorkingTimeSheet.CostSheetApproved,
                CostSheetApprovedBy = machineryWorkingTimeSheet.CostSheetApprovedBy,
                CostSheetApprovedDate = machineryWorkingTimeSheet.CostSheetApprovedDate,
                CostSheetSendForAproval = machineryWorkingTimeSheet.CostSheetSendForAproval,
                CreatedBy = machineryWorkingTimeSheet.CreatedBy,
                DateCreated = machineryWorkingTimeSheet.DateCreated,
                LastModified = machineryWorkingTimeSheet.LastModified,
               
                ModifiedBy = machineryWorkingTimeSheet.ModifiedBy,
                TimeSheetApproved = machineryWorkingTimeSheet.TimeSheetApproved,
                TimeSheetApprovedBy = machineryWorkingTimeSheet.TimeSheetApprovedBy,
                TimeSheetApprovedDate = machineryWorkingTimeSheet.TimeSheetApprovedDate,
                TimeSheetSendForAproval = machineryWorkingTimeSheet.TimeSheetSendForAproval,
                TimeSheetStatus = machineryWorkingTimeSheet.TimeSheetStatus,
                CostSheetStatus = machineryWorkingTimeSheet.CostSheetStatus,
                EquipmentCategoryId = machineryWorkingTimeSheet.EquipmentCategoryId,
                EquipmentNameId = machineryWorkingTimeSheet.EquipmentNameId,
                EquipmentTypeId = machineryWorkingTimeSheet.EquipmentTypeId,
                TimeSheetSendForApprovalTime = machineryWorkingTimeSheet.TimeSheetSendForApprovalTime,
                TimeSheetSendForApprovalBy = machineryWorkingTimeSheet.TimeSheetSendForApprovalBy,
                OperatorId = machineryWorkingTimeSheet.OperatorId,
                GreaseRate = machineryWorkingTimeSheet.GreaseRate,
                ExternalOperator = machineryWorkingTimeSheet.ExternalOperator,
                CostSheetSendForApprovalBy = machineryWorkingTimeSheet.CostSheetSendForApprovalBy,
                CostSheetSendForApprovalTime = machineryWorkingTimeSheet.CostSheetSendForApprovalTime,
                IsECWC=checkIsECWC()

            });
            return Json(result,JsonRequestBehavior.AllowGet);
        }
        private bool checkIsECWC()
        {
            bool x = false;
            var fetchSetting = db.RequestToInspections.FirstOrDefault();
            if (fetchSetting!=null&&fetchSetting.Remark == "ECWC")
            {
                x = true;
            }
            return (x);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MachineryCostSheet_Update([DataSourceRequest] DataSourceRequest request, MachineryWorkingTimeSheet machineryWorkingTimeSheet)
        {
            var fetchSetting = db.RequestToInspections.FirstOrDefault();

            decimal MCost = 0;
            if (fetchSetting != null && fetchSetting.Remark != "ECWC")
            {


                var MaintenanceJob = db.JobOrder.FirstOrDefault(x => x.FleetsID == machineryWorkingTimeSheet.FleetsID && x.SecondOnlineApprovedTime == DateTime.Now && x.Status == MaintenanceConst.Delivered);
                if (MaintenanceJob != null)
                {
                    var WorkShopJobs = db.ServiceCenterJob.Where(x => x.JobOrderID == MaintenanceJob.JobOrderID).ToList();
                    foreach (var WJ in WorkShopJobs)
                    {
                        var StoreReq = db.StoreRequisitions.FirstOrDefault(x => x.ServiceJobOrderNo == WJ.ServiceCenterJobId.ToString());
                        if (StoreReq != null)
                        {
                            var Issue = db.Issues.Where(x => x.StoreRequisitionID == StoreReq.StoreRequisitionID).ToList();
                            foreach (var issue in Issue)
                            {
                                var issueItems = db.IssueItems.Where(x => x.IssueID == issue.IssueID);
                                foreach (var item in issueItems)
                                {
                                    MCost += item.IssuedQuantity * item.UnitPrice;
                                }
                            }
                        }
                        var HumanCost = db.ServiceCrewWorkLogs.Where(x => x.JobId == WJ.ServiceCenterJobId).ToList();
                        foreach (var HC in HumanCost)
                        {
                            MCost += HC.TotalLabourCost;
                        }
                    }

                }
                if (machineryWorkingTimeSheet.MaintenanceCost == 0)
                {
                    machineryWorkingTimeSheet.MaintenanceCost = MCost;
                }
            }
            var companySetting = db.RequestToInspections.FirstOrDefault();
            if (companySetting != null && companySetting.Remark == "MCG")
            {
                machineryWorkingTimeSheet.TotalTime = machineryWorkingTimeSheet.NetHour+machineryWorkingTimeSheet.IdleHours * (machineryWorkingTimeSheet.IdealRentalRate*0.01m);
                machineryWorkingTimeSheet.IdealRentalRate = machineryWorkingTimeSheet.IdleHours * (machineryWorkingTimeSheet.IdealRentalRate*0.01m);
            }
            if (ModelState.IsValid)
            {
                var entity = new MachineryWorkingTimeSheet
                {

                    FleetsID = machineryWorkingTimeSheet.FleetsID,
                    MachineryWorkingTimeSheetId = machineryWorkingTimeSheet.MachineryWorkingTimeSheetId,
                    Date = machineryWorkingTimeSheet.Date,
                    MorningTimeStart = machineryWorkingTimeSheet.MorningTimeStart,
                    MorningTimeEnd = machineryWorkingTimeSheet.MorningTimeEnd,
                    AfternoonTimeStart = machineryWorkingTimeSheet.AfternoonTimeStart,
                    AfternoonTimeEnd = machineryWorkingTimeSheet.AfternoonTimeEnd,
                    NightTimeStart = machineryWorkingTimeSheet.NightTimeStart,
                    NightTimeEnd = machineryWorkingTimeSheet.NightTimeEnd,
                    NumberOfHoles = machineryWorkingTimeSheet.NumberOfHoles,
                    HoleDepth = machineryWorkingTimeSheet.HoleDepth,
                    FuelConsumption = machineryWorkingTimeSheet.FuelConsumption,
                    FuelRate = machineryWorkingTimeSheet.FuelRate,
                    IdleHours = machineryWorkingTimeSheet.IdleHours,
                    IdleReason = machineryWorkingTimeSheet.IdleReason,
                    DownHours = machineryWorkingTimeSheet.DownHours,
                    DownReason = machineryWorkingTimeSheet.DownReason,
                    NetHour = machineryWorkingTimeSheet.NetHour,
                    TotalTime = machineryWorkingTimeSheet.TotalTime,
                    WorkType = machineryWorkingTimeSheet.WorkType,
                    Remark = machineryWorkingTimeSheet.Remark,
                    RentalRate = machineryWorkingTimeSheet.RentalRate,
                    IdealRentalRate = machineryWorkingTimeSheet.IdealRentalRate,
                    ProcurementSupplierID = machineryWorkingTimeSheet.ProcurementSupplierID,
                    OperationAmount = machineryWorkingTimeSheet.OperationAmount,
                    IdleRate = machineryWorkingTimeSheet.IdleRate,
                    IdleAmount = machineryWorkingTimeSheet.IdleAmount,
                    TotalAmountBeforeVAT = machineryWorkingTimeSheet.TotalAmountBeforeVAT,
                    VAT = machineryWorkingTimeSheet.VAT,
                    TotalAmount = machineryWorkingTimeSheet.TotalAmount,
                    FuelCost = machineryWorkingTimeSheet.FuelCost,
                    ServiceRecharge = machineryWorkingTimeSheet.ServiceRecharge,

                    SparePartCost = machineryWorkingTimeSheet.SparePartCost,
                    OilCost = machineryWorkingTimeSheet.OilCost,
                    GreaseConsumption = machineryWorkingTimeSheet.GreaseConsumption,
                    GreaseCost = machineryWorkingTimeSheet.GreaseCost,
                    MaintenanceCost = machineryWorkingTimeSheet.MaintenanceCost,
                    OilConsumption = machineryWorkingTimeSheet.OilConsumption,

                    TotalCost = machineryWorkingTimeSheet.TotalCost,
                    NetAmount = machineryWorkingTimeSheet.NetAmount,
                  
                    DownHourRate = machineryWorkingTimeSheet.DownHourRate,
                    OperationRevenue = machineryWorkingTimeSheet.OperationRevenue,
                    InitialEHR = machineryWorkingTimeSheet.InitialEHR,
                    FinalEHR = machineryWorkingTimeSheet.FinalEHR,
                    DifferenceEHR = machineryWorkingTimeSheet.DifferenceEHR,
                    FuelType = machineryWorkingTimeSheet.FuelType,
                    OilType = machineryWorkingTimeSheet.OilType,
                    ReasonForDown = machineryWorkingTimeSheet.ReasonForDown,
                    ReasonForIdle = machineryWorkingTimeSheet.ReasonForIdle,
                    Perdium = machineryWorkingTimeSheet.Perdium,
                    Salary = machineryWorkingTimeSheet.Salary,
                    TireCost = machineryWorkingTimeSheet.TireCost,
                    BranchId = machineryWorkingTimeSheet.BranchId,
                    BusinessUnitId = machineryWorkingTimeSheet.BusinessUnitId,
                    CostSheetApproved = machineryWorkingTimeSheet.CostSheetApproved,
                    CostSheetApprovedBy = machineryWorkingTimeSheet.CostSheetApprovedBy,
                    CostSheetApprovedDate = machineryWorkingTimeSheet.CostSheetApprovedDate,
                    CostSheetSendForAproval = machineryWorkingTimeSheet.CostSheetSendForAproval,
                    CreatedBy = machineryWorkingTimeSheet.CreatedBy,
                    DateCreated = machineryWorkingTimeSheet.DateCreated,
                    LastModified = machineryWorkingTimeSheet.LastModified,
                    ModifiedBy = machineryWorkingTimeSheet.ModifiedBy,
                    TimeSheetApproved = machineryWorkingTimeSheet.TimeSheetApproved,
                    TimeSheetApprovedBy = machineryWorkingTimeSheet.TimeSheetApprovedBy,
                    TimeSheetApprovedDate = machineryWorkingTimeSheet.TimeSheetApprovedDate,
                    TimeSheetSendForAproval = machineryWorkingTimeSheet.TimeSheetSendForAproval,
                    TimeSheetStatus = machineryWorkingTimeSheet.TimeSheetStatus,
                    CostSheetStatus = machineryWorkingTimeSheet.CostSheetStatus,
                    CostSheetSendForApprovalBy = machineryWorkingTimeSheet.CostSheetSendForApprovalBy,
                    CostSheetSendForApprovalTime = machineryWorkingTimeSheet.CostSheetSendForApprovalTime,
                    EquipmentCategoryId = machineryWorkingTimeSheet.EquipmentCategoryId,
                    EquipmentNameId = machineryWorkingTimeSheet.EquipmentNameId,
                    EquipmentTypeId = machineryWorkingTimeSheet.EquipmentTypeId,
                    ExternalOperator = machineryWorkingTimeSheet.ExternalOperator,
                    GreaseRate = machineryWorkingTimeSheet.GreaseRate,
                    OperatorId = machineryWorkingTimeSheet.OperatorId,
                    LabourCost = machineryWorkingTimeSheet.LabourCost,
                    BatteryCost = machineryWorkingTimeSheet.BatteryCost,
                    OverTime = machineryWorkingTimeSheet.OverTime,
                    BasicSalary = machineryWorkingTimeSheet.BasicSalary,
                    TimeSheetSendForApprovalBy = machineryWorkingTimeSheet.TimeSheetSendForApprovalBy,
                    TimeSheetSendForApprovalTime = machineryWorkingTimeSheet.TimeSheetSendForApprovalTime
                };

                
                //db.MachineryWorkingTimeSheets.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Working machinery registration", "Success", User.Identity.Name, $"updated machinery working timesheet {entity.MachineryWorkingTimeSheetId}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { machineryWorkingTimeSheet }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult MachineryCostSheet_Destroy([DataSourceRequest] DataSourceRequest request, MachineryWorkingTimeSheet machineryWorkingTimeSheet)
        {
            if (ModelState.IsValid)
            {
                var entity = new MachineryWorkingTimeSheet
                {
                    FleetsID = machineryWorkingTimeSheet.FleetsID,
                    MachineryWorkingTimeSheetId = machineryWorkingTimeSheet.MachineryWorkingTimeSheetId,
                    Date = machineryWorkingTimeSheet.Date,
                    MorningTimeStart = machineryWorkingTimeSheet.MorningTimeStart,
                    MorningTimeEnd = machineryWorkingTimeSheet.MorningTimeEnd,
                    AfternoonTimeStart = machineryWorkingTimeSheet.AfternoonTimeStart,
                    AfternoonTimeEnd = machineryWorkingTimeSheet.AfternoonTimeEnd,
                    NightTimeStart = machineryWorkingTimeSheet.NightTimeStart,
                    NightTimeEnd = machineryWorkingTimeSheet.NightTimeEnd,
                    NumberOfHoles = machineryWorkingTimeSheet.NumberOfHoles,
                    HoleDepth = machineryWorkingTimeSheet.HoleDepth,
                    FuelConsumption = machineryWorkingTimeSheet.FuelConsumption,
                    FuelRate = machineryWorkingTimeSheet.FuelRate,
                    GreaseConsumption = machineryWorkingTimeSheet.GreaseConsumption,
                    IdleHours = machineryWorkingTimeSheet.IdleHours,
                    IdleReason = machineryWorkingTimeSheet.IdleReason,
                    DownHours = machineryWorkingTimeSheet.DownHours,
                    DownReason = machineryWorkingTimeSheet.DownReason,
                    NetHour = machineryWorkingTimeSheet.NetHour,
                    WorkType = machineryWorkingTimeSheet.WorkType,
                    Remark = machineryWorkingTimeSheet.Remark,
                    RentalRate = machineryWorkingTimeSheet.RentalRate,
                    IdealRentalRate = machineryWorkingTimeSheet.IdealRentalRate,
                    ProcurementSupplierID = machineryWorkingTimeSheet.ProcurementSupplierID,
                    OperationAmount = machineryWorkingTimeSheet.OperationAmount,
                    IdleRate = machineryWorkingTimeSheet.IdleRate,
                    IdleAmount = machineryWorkingTimeSheet.IdleAmount,
                    TotalAmountBeforeVAT = machineryWorkingTimeSheet.TotalAmountBeforeVAT,
                    VAT = machineryWorkingTimeSheet.VAT,
                    TotalAmount = machineryWorkingTimeSheet.TotalAmount,
                    FuelCost = machineryWorkingTimeSheet.FuelCost,
                    ServiceRecharge = machineryWorkingTimeSheet.ServiceRecharge,
                    MaintenanceCost = machineryWorkingTimeSheet.MaintenanceCost,
                    TotalCost = machineryWorkingTimeSheet.TotalCost,
                    NetAmount = machineryWorkingTimeSheet.NetAmount,

                    DownHourRate = machineryWorkingTimeSheet.DownHourRate,
                    OperationRevenue = machineryWorkingTimeSheet.OperationRevenue,
                    Perdium = machineryWorkingTimeSheet.Perdium,
                    Salary = machineryWorkingTimeSheet.Salary,
                    TireCost = machineryWorkingTimeSheet.TireCost,
                    BranchId = machineryWorkingTimeSheet.BranchId,
                    BusinessUnitId = machineryWorkingTimeSheet.BusinessUnitId,
                    CostSheetApproved = machineryWorkingTimeSheet.CostSheetApproved,
                    CostSheetApprovedBy = machineryWorkingTimeSheet.CostSheetApprovedBy,
                    CostSheetApprovedDate = machineryWorkingTimeSheet.CostSheetApprovedDate,
                    CostSheetSendForAproval = machineryWorkingTimeSheet.CostSheetSendForAproval,
                    CreatedBy = machineryWorkingTimeSheet.CreatedBy,
                    DateCreated = machineryWorkingTimeSheet.DateCreated,
                    LastModified = machineryWorkingTimeSheet.LastModified,
                    ModifiedBy = machineryWorkingTimeSheet.ModifiedBy,
                    TimeSheetApproved = machineryWorkingTimeSheet.TimeSheetApproved,
                    TimeSheetApprovedBy = machineryWorkingTimeSheet.TimeSheetApprovedBy,
                    TimeSheetApprovedDate = machineryWorkingTimeSheet.TimeSheetApprovedDate,
                    TimeSheetSendForAproval = machineryWorkingTimeSheet.TimeSheetSendForAproval,
                    TimeSheetStatus = machineryWorkingTimeSheet.TimeSheetStatus,
                    CostSheetStatus = machineryWorkingTimeSheet.CostSheetStatus


                };

                db.MachineryWorkingTimeSheets.Attach(entity);
                db.MachineryWorkingTimeSheets.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted working machinery timesheet {entity.MachineryWorkingTimeSheetId }");
                #endregion
            }

            return Json(new[] { machineryWorkingTimeSheet }.ToDataSourceResult(request, ModelState));
        }



        //-----------------------------------------------------------Approval------------------------------------------------------------------//

        #region Approval
        public JsonResult CostSheetSendForApproval(int machineryWorkingTimeSheetId)
        {

            var machineryCostSheet = db.MachineryWorkingTimeSheets.Find(machineryWorkingTimeSheetId);

            if (machineryCostSheet != null)
            {
                machineryCostSheet.CostSheetSendForAproval = true;
                machineryCostSheet.CostSheetSendForApprovalTime = DateTime.Now;
                machineryCostSheet.CostSheetSendForApprovalBy = User.Identity.Name;
                machineryCostSheet.CostSheetStatus = MaintenanceConst.WaitingApproval;

                db.MachineryWorkingTimeSheets.Attach(machineryCostSheet);
                db.Entry(machineryCostSheet).State = EntityState.Modified;
                db.SaveChanges();
                var returnMessage = new
                {
                    Message = "Successfully Sent!",

                };
                return Json(returnMessage.Message, JsonRequestBehavior.AllowGet);

            }

            var jsonData = new
            {
                Message = "Error Sending!",

            };
            return Json(jsonData.Message, JsonRequestBehavior.AllowGet);

        }
        public JsonResult VoidCostSheet(int machineryWorkingTimeSheetId)
        {

            var machineryCostSheet = db.MachineryWorkingTimeSheets.Find(machineryWorkingTimeSheetId);

            if (machineryCostSheet != null)
            {
                machineryCostSheet.CostSheetSendForAproval = false;
                machineryCostSheet.CostSheetApproved = false;
                machineryCostSheet.CostSheetApprovedDate = DateTime.Now;
                machineryCostSheet.CostSheetApprovedBy = User.Identity.Name;
                machineryCostSheet.CostSheetStatus = MaintenanceConst.Rejected;

                db.MachineryWorkingTimeSheets.Attach(machineryCostSheet);
                db.Entry(machineryCostSheet).State = EntityState.Modified;
                db.SaveChanges();
                var returnMessage = new
                {
                    Message = "Successfully Sent!",

                };
                return Json(returnMessage.Message, JsonRequestBehavior.AllowGet);

            }

            var jsonData = new
            {
                Message = "Error Sending!",

            };
            return Json(jsonData.Message, JsonRequestBehavior.AllowGet);

        }
        public ActionResult MachineryCostSheetApproval_Read([DataSourceRequest] DataSourceRequest request, int id)
        {

            var approval = db.CostSheetApprovals.Where(x => x.CostSheetId == id && x.TimeSheetType == TimeSheetType.Machinery);
            DataSourceResult result = approval.ToDataSourceResult(request, model => new CostSheetApproval
            {
                CostSheetId = model.CostSheetId,
                CreatedBy = model.CreatedBy,
                DateCreated = model.DateCreated,
                IsApproved = model.IsApproved,
                LastModified = model.LastModified,
                ModifiedBy = model.ModifiedBy,
                Remark = model.Remark,
                Status = model.Status,
                CostSheetApprovalId = model.CostSheetApprovalId,
                TimeSheetType = model.TimeSheetType
            });
            return Json(result);
        }
        [AuthorizeRoles(FleetManagementRoles.FleetCostSheetApprover)]
        public ActionResult MachineryCostSheetApproval_Create([DataSourceRequest] DataSourceRequest request, CostSheetApproval model, int id)
        {
            if (ModelState.IsValid)
            {
                if (model.Status == MaintenanceConst.Approved)
                {
                    Approved(id);
                    model.IsApproved = true;

                }
                else
                {
                    Rejected(id);
                    model.IsApproved = false;
                }
                model.CreatedBy = User.Identity.Name;
                model.DateCreated = DateTime.Now;
                model.CostSheetId = id;
                model.TimeSheetType = TimeSheetType.Machinery;
                db.CostSheetApprovals.Add(model);
                db.SaveChanges();

            }


            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        private void Approved(int id)
        {
            var machineryCostSheet = db.MachineryWorkingTimeSheets.Find(id);
            if (machineryCostSheet != null)
            {
                machineryCostSheet.CostSheetApproved = true;
                machineryCostSheet.CostSheetStatus = MaintenanceConst.Approved;
                machineryCostSheet.CostSheetApprovedBy = User.Identity.Name;
                machineryCostSheet.CostSheetApprovedDate = DateTime.Now;
                db.Entry(machineryCostSheet).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        private void Rejected(int id)
        {
            var machineryCostSheet = db.MachineryWorkingTimeSheets.Find(id);
            if (machineryCostSheet != null)
            {
                machineryCostSheet.CostSheetApproved = false;
                machineryCostSheet.CostSheetStatus = MaintenanceConst.Rejected;
                machineryCostSheet.CostSheetApprovedBy = User.Identity.Name;
                machineryCostSheet.CostSheetApprovedDate = DateTime.Now;
                db.Entry(machineryCostSheet).State = EntityState.Modified;
                db.SaveChanges();

            }
        }

        #endregion



        public JsonResult GetFleetValues(Guid fleetId)
        {
            var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == fleetId);
            if (fleet != null)
            {
                return Json(fleet, JsonRequestBehavior.AllowGet);
            }
            return Json(null);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}