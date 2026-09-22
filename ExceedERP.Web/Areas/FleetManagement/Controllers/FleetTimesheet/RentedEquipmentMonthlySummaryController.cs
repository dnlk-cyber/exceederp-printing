using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetTimesheet
{
    public class RentedEquipmentMonthlySummaryController : Controller
    {
        // GET: FleetManagement/TimeSheetSetting
        ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();
        public ActionResult Index()
        {

            PopulateData();
            return View();
        }
        public ActionResult CostSheetIndex()
        {
            PopulateData();
            return View();
        }
        public ActionResult FinanceIndex()
        {
            PopulateData();
            return View();
        }
        private void PopulateData()
        {
            ViewData["branch"] = CacheData.GetBranchCaches().Select(
     i => new
     {
         Name = i.Name,
         BranchId = i.Id
     }).AsQueryable();
            ViewData["Period"] = db.GLPeriods.Select(
       i => new
       {
           Name = i.Name,
           Id = i.GLPeriodId
       }).AsQueryable();
           
        }
    
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
           // var lightVehicleTimesheet = db.LightVehicleTimeSheets.Where(x => mybraches.Contains(x.BranchId));
            IQueryable<RentedEquipmentMonthlyPaymentSummary> fetchSummary = db.RentedEquipmentMonthlyPaymentSummary.Where(x => mybraches.Contains(x.BranchId));

            var result = fetchSummary.ToDataSourceResult(request, x => new
            {
                DateCreated = x.DateCreated,
                CreatedBy = x.CreatedBy,
                LastModified = x.LastModified,
                ModifiedBy = x.ModifiedBy,

                RentedEquipmentMonthlyPaymentSummaryId = x.RentedEquipmentMonthlyPaymentSummaryId,
                GlPeriodId = x.GlPeriodId,
                EquipmentTypeId = x.EquipmentTypeId,
                EquipmentType = x.EquipmentType,
                FleetId = x.FleetId,
                PlateNo = x.PlateNo,
                OperatorName = x.OperatorName,
                OperatorId = x.OperatorId,
                BranchId = x.BranchId,
                BranchName = x.BranchName,
                AgreementNo = x.AgreementNo,
                RentalRateHour = x.RentalRateHour,
                RentalRateDay = x.RentalRateDay,
                AgreementDate = x.AgreementDate,
                TimeSheetPreparedBy = x.TimeSheetPreparedBy,
                TimeSheetPreparedDate = x.TimeSheetPreparedDate,
                TimeSheetCheckedBy = x.TimeSheetCheckedBy,
                TimeSheetCheckedDate = x.TimeSheetCheckedDate,
                TimeSheetIsChecked = x.TimeSheetIsChecked,
                IsWagonDrill = x.IsWagonDrill,
                IsRejected = x.IsRejected,
                CustomerName = x.CustomerName,
                SparePartCost=x.SparePartCost,
                OilLubCost=x.OilLubCost,
                MaintenanceCost = x.MaintenanceCost,
                FuelCost = x.FuelCost,
                TimeSheetApprovedBy = x.TimeSheetApprovedBy,
                TimeSheetApprovalDate = x.TimeSheetApprovalDate,
                TimeSheetIsApproved = x.TimeSheetIsApproved,
                TimeSheetAuthorizedBy = x.TimeSheetAuthorizedBy,
                TimeSheetAuthorizedDate = x.TimeSheetAuthorizedDate,
                TimeSheetIsAuthorized = x.TimeSheetIsAuthorized,
                TimeSheetDGMApprovedBy = x.TimeSheetDGMApprovedBy,
                TimeSheetDGMApprovalDate = x.TimeSheetDGMApprovalDate,
                TimeSheetIsDGMApproved = x.TimeSheetIsDGMApproved,
                TimeSheetEquipmentAdminOfficerPreparedBy = x.TimeSheetEquipmentAdminOfficerPreparedBy,
                TimeSheetEquipmentAdminOfficerPreparedDate = x.TimeSheetEquipmentAdminOfficerPreparedDate,
                TimeSheetEquipmentAdminOfficerPreparedBySign = x.TimeSheetEquipmentAdminOfficerPreparedBySign,

                TimeSheetEquipmentAdminCheckedBy = x.TimeSheetEquipmentAdminCheckedBy,
                TimeSheetEquipmentAdminCheckedDate = x.TimeSheetEquipmentAdminCheckedDate,
                TimeSheetEquipmentAdminCheckedBySign = x.TimeSheetEquipmentAdminCheckedBySign,

                TimeSheetOfficeEngineerCheckedBy = x.TimeSheetOfficeEngineerCheckedBy,
                TimeSheetOfficeEngineerCheckedDate = x.TimeSheetOfficeEngineerCheckedDate,
                TimeSheetOfficeEngineerCheckedBySign = x.TimeSheetOfficeEngineerCheckedBySign,

                TimeSheetConstructionEngineerApprovedBy = x.TimeSheetConstructionEngineerApprovedBy,
                TimeSheetConstructionEngineerApprovedDate = x.TimeSheetConstructionEngineerApprovedDate,
                TimeSheetConstructionEngineerApprovedBySign = x.TimeSheetConstructionEngineerApprovedBySign,

                TimeSheetDepartmentManagerApprovedBy = x.TimeSheetDepartmentManagerApprovedBy,
                TimeSheetDepartmentManagerApprovedDate = x.TimeSheetDepartmentManagerApprovedDate,
                TimeSheetDepartmentManagerApprovedBySign = x.TimeSheetDepartmentManagerApprovedBySign,
                TimeSheetIsEquipmentAdminChecked = x.TimeSheetIsEquipmentAdminChecked,
                TimeSheetIsOfficeEngineerChecked = x.TimeSheetIsOfficeEngineerChecked,
                TimeSheetIsConstructionEngineerApproved = x.TimeSheetIsConstructionEngineerApproved,
                TimeSheetIsDepartmentManagerApproved = x.TimeSheetIsDepartmentManagerApproved,
                GrossBeforeVat = x.GrossBeforeVat,
                VAT = x.VAT,
                //TOT = x.TOT,
                TotalWithVat = x.TotalWithVat,
                AdvancePayedWithVat = x.AdvancePayedWithVat,
                Deductable = x.Deductable,
                NetPayable = x.NetPayable,
                TotalWorkedHR = x.TotalWorkedHR
            });

            return Json(result);
        }
        public ActionResult CostSheet_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            IQueryable<RentedEquipmentMonthlyPaymentSummary> fetchSummary = db.RentedEquipmentMonthlyPaymentSummary.Where(x=>x.TimeSheetIsDepartmentManagerApproved).Where(x => mybraches.Contains(x.BranchId));
            var result = fetchSummary.ToDataSourceResult(request, x => new
            {
                DateCreated = x.DateCreated,
                CreatedBy = x.CreatedBy,
                LastModified = x.LastModified,
                ModifiedBy = x.ModifiedBy,

                RentedEquipmentMonthlyPaymentSummaryId = x.RentedEquipmentMonthlyPaymentSummaryId,
                GlPeriodId = x.GlPeriodId,
                EquipmentTypeId = x.EquipmentTypeId,
                TimeSheetIsPrepared = x.TimeSheetIsPrepared,
                EquipmentType = x.EquipmentType,
                FleetId = x.FleetId,
                PlateNo = x.PlateNo,
                CustomerName = x.CustomerName,
                OperatorName = x.OperatorName,
                OperatorId = x.OperatorId,
                BranchId = x.BranchId,
                BranchName = x.BranchName,
                SparePartCost = x.SparePartCost,
                OilLubCost = x.OilLubCost,
                MaintenanceCost = x.MaintenanceCost,
                FuelCost = x.FuelCost,
                AgreementNo = x.AgreementNo,
                RentalRateHour = x.RentalRateHour,
                RentalRateDay = x.RentalRateDay,
                AgreementDate = x.AgreementDate,
                TimeSheetPreparedBy = x.TimeSheetPreparedBy,
                TimeSheetPreparedDate = x.TimeSheetPreparedDate,
                TimeSheetCheckedBy = x.TimeSheetCheckedBy,
                TimeSheetCheckedDate = x.TimeSheetCheckedDate,
                TimeSheetIsChecked = x.TimeSheetIsChecked,
                IsRejected = x.IsRejected,
                IsWagonDrill = x.IsWagonDrill,
                TimeSheetApprovedBy = x.TimeSheetApprovedBy,
                TimeSheetApprovalDate = x.TimeSheetApprovalDate,
                TimeSheetIsApproved = x.TimeSheetIsApproved,
                TimeSheetAuthorizedBy = x.TimeSheetAuthorizedBy,
                TimeSheetAuthorizedDate = x.TimeSheetAuthorizedDate,
                TimeSheetIsAuthorized = x.TimeSheetIsAuthorized,
                TimeSheetDGMApprovedBy = x.TimeSheetDGMApprovedBy,
                TimeSheetDGMApprovalDate = x.TimeSheetDGMApprovalDate,
                TimeSheetIsDGMApproved = x.TimeSheetIsDGMApproved,
                TimeSheetEquipmentAdminOfficerPreparedBy = x.TimeSheetEquipmentAdminOfficerPreparedBy,
                TimeSheetEquipmentAdminOfficerPreparedDate = x.TimeSheetEquipmentAdminOfficerPreparedDate,
                TimeSheetEquipmentAdminOfficerPreparedBySign = x.TimeSheetEquipmentAdminOfficerPreparedBySign,

                TimeSheetEquipmentAdminCheckedBy = x.TimeSheetEquipmentAdminCheckedBy,
                TimeSheetEquipmentAdminCheckedDate = x.TimeSheetEquipmentAdminCheckedDate,
                TimeSheetEquipmentAdminCheckedBySign = x.TimeSheetEquipmentAdminCheckedBySign,

                TimeSheetOfficeEngineerCheckedBy = x.TimeSheetOfficeEngineerCheckedBy,
                TimeSheetOfficeEngineerCheckedDate = x.TimeSheetOfficeEngineerCheckedDate,
                TimeSheetOfficeEngineerCheckedBySign = x.TimeSheetOfficeEngineerCheckedBySign,

                TimeSheetConstructionEngineerApprovedBy = x.TimeSheetConstructionEngineerApprovedBy,
                TimeSheetConstructionEngineerApprovedDate = x.TimeSheetConstructionEngineerApprovedDate,
                TimeSheetConstructionEngineerApprovedBySign = x.TimeSheetConstructionEngineerApprovedBySign,

                TimeSheetDepartmentManagerApprovedBy = x.TimeSheetDepartmentManagerApprovedBy,
                TimeSheetDepartmentManagerApprovedDate = x.TimeSheetDepartmentManagerApprovedDate,
                TimeSheetDepartmentManagerApprovedBySign = x.TimeSheetDepartmentManagerApprovedBySign,
                TimeSheetIsEquipmentAdminChecked = x.TimeSheetIsEquipmentAdminChecked,
                TimeSheetIsOfficeEngineerChecked = x.TimeSheetIsOfficeEngineerChecked,
                TimeSheetIsConstructionEngineerApproved = x.TimeSheetIsConstructionEngineerApproved,
                TimeSheetIsDepartmentManagerApproved = x.TimeSheetIsDepartmentManagerApproved,
                GrossBeforeVat = x.GrossBeforeVat,
                VAT = x.VAT,
                //TOT = x.TOT,
                TotalWithVat = x.TotalWithVat,
                AdvancePayedWithVat = x.AdvancePayedWithVat,
                Deductable = x.Deductable,
                NetPayable = x.NetPayable,
                TotalWorkedHR = x.TotalWorkedHR
            });

            return Json(result);
        }
        public ActionResult Finance_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            IQueryable<RentedEquipmentMonthlyPaymentSummary> fetchSummary = db.RentedEquipmentMonthlyPaymentSummary.Where(x=>x.TimeSheetIsDGMApproved).Where(x => mybraches.Contains(x.BranchId));
            var result = fetchSummary.ToDataSourceResult(request, x => new
            {
                DateCreated = x.DateCreated,
                CreatedBy = x.CreatedBy,
                LastModified = x.LastModified,
                ModifiedBy = x.ModifiedBy,

                RentedEquipmentMonthlyPaymentSummaryId = x.RentedEquipmentMonthlyPaymentSummaryId,
                GlPeriodId = x.GlPeriodId,
                EquipmentTypeId = x.EquipmentTypeId,
                TimeSheetIsPrepared = x.TimeSheetIsPrepared,
                EquipmentType = x.EquipmentType,
                FleetId = x.FleetId,
                PlateNo = x.PlateNo,
                CustomerName = x.CustomerName,
                OperatorName = x.OperatorName,
                OperatorId = x.OperatorId,
                BranchId = x.BranchId,
                BranchName = x.BranchName,
                AgreementNo = x.AgreementNo,
                RentalRateHour = x.RentalRateHour,
                RentalRateDay = x.RentalRateDay,
                AgreementDate = x.AgreementDate,
                TimeSheetPreparedBy = x.TimeSheetPreparedBy,
                TimeSheetPreparedDate = x.TimeSheetPreparedDate,
                TimeSheetCheckedBy = x.TimeSheetCheckedBy,
                TimeSheetCheckedDate = x.TimeSheetCheckedDate,
                TimeSheetIsChecked = x.TimeSheetIsChecked,
                IsRejected = x.IsRejected,
                SparePartCost = x.SparePartCost,
                OilLubCost = x.OilLubCost,
                MaintenanceCost = x.MaintenanceCost,
                FuelCost = x.FuelCost,
                IsWagonDrill = x.IsWagonDrill,
                TimeSheetApprovedBy = x.TimeSheetApprovedBy,
                TimeSheetApprovalDate = x.TimeSheetApprovalDate,
                TimeSheetIsApproved = x.TimeSheetIsApproved,
                TimeSheetAuthorizedBy = x.TimeSheetAuthorizedBy,
                TimeSheetAuthorizedDate = x.TimeSheetAuthorizedDate,
                TimeSheetIsAuthorized = x.TimeSheetIsAuthorized,
                TimeSheetDGMApprovedBy = x.TimeSheetDGMApprovedBy,
                TimeSheetDGMApprovalDate = x.TimeSheetDGMApprovalDate,
                TimeSheetIsDGMApproved = x.TimeSheetIsDGMApproved,
                TimeSheetEquipmentAdminOfficerPreparedBy = x.TimeSheetEquipmentAdminOfficerPreparedBy,
                TimeSheetEquipmentAdminOfficerPreparedDate = x.TimeSheetEquipmentAdminOfficerPreparedDate,
                TimeSheetEquipmentAdminOfficerPreparedBySign = x.TimeSheetEquipmentAdminOfficerPreparedBySign,
                IsFinanceSendForApproval = x.IsFinanceSendForApproval,
                FinanceSendForApprovalBy = x.FinanceSendForApprovalBy,
                FinanceSendForApprovalTime = x.FinanceSendForApprovalTime,
                IsFinanceApproved = x.IsFinanceApproved,
                FinanceApprovedBy = x.FinanceApprovedBy,
                FinanceApprovedTime = x.FinanceApprovedTime,
                AmountDue = x.AmountDue,
                TotalAmount = x.TotalAmount,
                Paid = x.Paid,
                TimeSheetEquipmentAdminCheckedBy = x.TimeSheetEquipmentAdminCheckedBy,
                TimeSheetEquipmentAdminCheckedDate = x.TimeSheetEquipmentAdminCheckedDate,
                TimeSheetEquipmentAdminCheckedBySign = x.TimeSheetEquipmentAdminCheckedBySign,

                TimeSheetOfficeEngineerCheckedBy = x.TimeSheetOfficeEngineerCheckedBy,
                TimeSheetOfficeEngineerCheckedDate = x.TimeSheetOfficeEngineerCheckedDate,
                TimeSheetOfficeEngineerCheckedBySign = x.TimeSheetOfficeEngineerCheckedBySign,

                TimeSheetConstructionEngineerApprovedBy = x.TimeSheetConstructionEngineerApprovedBy,
                TimeSheetConstructionEngineerApprovedDate = x.TimeSheetConstructionEngineerApprovedDate,
                TimeSheetConstructionEngineerApprovedBySign = x.TimeSheetConstructionEngineerApprovedBySign,

                TimeSheetDepartmentManagerApprovedBy = x.TimeSheetDepartmentManagerApprovedBy,
                TimeSheetDepartmentManagerApprovedDate = x.TimeSheetDepartmentManagerApprovedDate,
                TimeSheetDepartmentManagerApprovedBySign = x.TimeSheetDepartmentManagerApprovedBySign,
                TimeSheetIsEquipmentAdminChecked = x.TimeSheetIsEquipmentAdminChecked,
                TimeSheetIsOfficeEngineerChecked = x.TimeSheetIsOfficeEngineerChecked,
                TimeSheetIsConstructionEngineerApproved = x.TimeSheetIsConstructionEngineerApproved,
                TimeSheetIsDepartmentManagerApproved = x.TimeSheetIsDepartmentManagerApproved,
                GrossBeforeVat = x.GrossBeforeVat,
                VAT = x.VAT,
                //TOT = x.TOT,
                TotalWithVat = x.TotalWithVat,
                AdvancePayedWithVat = x.AdvancePayedWithVat,
                Deductable = x.Deductable,
                NetPayable = x.NetPayable,
                TotalWorkedHR = x.TotalWorkedHR
            });

            return Json(result);
        }
        public ActionResult Detail_Read([DataSourceRequest] DataSourceRequest request,int id)
        {
            var timesheet = db.RentedEquipmentDailyOperation.Where(x => x.RentedEquipmentMonthlyPaymentSummaryId==id);
            DataSourceResult result = timesheet.ToDataSourceResult(request, model => new
            {

                RentedEquipmentDailyOperationId = model.RentedEquipmentDailyOperationId,
                RentedEquipmentMonthlyPaymentSummaryId = model.RentedEquipmentMonthlyPaymentSummaryId,
                Date = model.Date,
                MorningTimeStart = model.MorningTimeStart,
                MorningTimeEnd = model.MorningTimeEnd,
                AfternoonTimeStart = model.AfternoonTimeStart,
                AfternoonTimeEnd = model.AfternoonTimeEnd,
                NightTimeStart = model.NightTimeStart,
                NightTimeEnd = model.NightTimeEnd,
                PayedIdleHR = model.PayedIdleHR,
                TotalPayedHR = model.TotalPayedHR,
                WorkedHR = model.WorkedHR,
                InitialEngineHour = model.InitialEngineHour,
                FinalEngineHour = model.FinalEngineHour,
                DifferenceEngineHour = model.DifferenceEngineHour,
                DownHour = model.DownHour,
                Activity = model.Activity,
                Unit = model.Unit,
                Volume = model.Volume,
                IdleReasonList = model.IdleReasonList,
                DownReasonList = model.DownReasonList,
                SparePartCost = model.SparePartCost,
                OilLubCost = model.OilLubCost,
                MaintenanceCost = model.MaintenanceCost,
                FuelCost = model.FuelCost,
                FuelInLiter = model.FuelInLiter,
                FuelKMReading = model.FuelKMReading,

                OperatorId = model.OperatorId,
                OperatorName = model.OperatorName,

                SiteFormanId = model.SiteFormanId,
                SiteFormanName = model.SiteFormanName


            });
            return Json(result);
        }public ActionResult WagonDrill_Detail_Read([DataSourceRequest] DataSourceRequest request,int id)
        {
            var timesheet = db.WagonDrillDailyOperation.Where(x => x.RentedEquipmentMonthlyPaymentSummaryId==id);
            DataSourceResult result = timesheet.ToDataSourceResult(request, model => new
            {

                WagonDrillDailyOperationId = model.WagonDrillDailyOperationId,
                RentedEquipmentMonthlyPaymentSummaryId = model.RentedEquipmentMonthlyPaymentSummaryId,
                Date = model.Date,
                NumberOfHolesDrilled = model.NumberOfHolesDrilled,
                OperationDrilledHoleDepth = model.OperationDrilledHoleDepth,
                OperationRateForMeter = model.OperationRateForMeter,
                OperationAmount = model.OperationAmount,
                WorkedHR = model.WorkedHR,
                InitialEngineHour = model.InitialEngineHour,
                FinalEngineHour = model.FinalEngineHour,
                DifferenceEngineHour = model.DifferenceEngineHour,
                DownHour = model.DownHour,
                Activity = model.Activity,
                Unit = model.Unit,
                PayedIdleHour = model.PayedIdleHour,
                Volume = model.Volume,
                IdleReasonList = model.IdleReasonList,
                DownReasonList = model.DownReasonList,

                FuelInLiter = model.FuelInLiter,
                FuelKMReading = model.FuelKMReading,
                SparePartCost = model.SparePartCost,
                OilLubCost = model.OilLubCost,
                MaintenanceCost = model.MaintenanceCost,
                FuelCost = model.FuelCost,
                OperatorId = model.OperatorId,
                OperatorName = model.OperatorName,

                SiteFormanId = model.SiteFormanId,
                SiteFormanName = model.SiteFormanName


            });
            return Json(result);
        }
        public ActionResult RentedEquipmentMonthlyPaymentSummary_Create(
            [DataSourceRequest] DataSourceRequest request,
            RentedEquipmentMonthlyPaymentSummary rentedEqupmentSummary)
        {
            var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID.ToString() == rentedEqupmentSummary.FleetId);
            var GlPeriod = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == rentedEqupmentSummary.GlPeriodId);
            var equipmentRent = db.RentFromExternalEquipments.Where(x => x.FleetId == fleet.FleetsID&&x.IsOnlineApproved).OrderByDescending(x => x.RentFromExternalEquipmentId).FirstOrDefault();
            var rentFromExternal = db.RentFromExternal.FirstOrDefault(x => x.RentFromExternalID == equipmentRent.RentFromExternalId);
            if(rentFromExternal.StartDate>GlPeriod.DateTo)
            {
                ModelState.AddModelError("payment not allowed", "You can not create summary before agreement date");
            }
            var existingSummary = db.RentedEquipmentMonthlyPaymentSummary.FirstOrDefault(x => x.GlPeriodId == rentedEqupmentSummary.GlPeriodId && x.FleetId == rentedEqupmentSummary.FleetId);
            if(existingSummary!=null)
            {
                ModelState.AddModelError("payment not allowed", "Summary already exists");
            }
            decimal advancePayed = db.RentalEquipmentAdvancePayment.Where(x => x.RentFromExternalEquipmentId == equipmentRent.RentFromExternalEquipmentId).OrderByDescending(x => x.RentalEquipmentAdvancePaymentId).FirstOrDefault()?.AdvancePayed ?? 0;
            var equipmentType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == fleet.EquipmentTypeID);
            if (ModelState.IsValid)
            {
               
                if (equipmentType!=null)
                {
                    if (equipmentType.TimeSheetType == TimeSheetType.LightVehicle)
                    {
                        var timeSheets = db.LightVehicleTimeSheets.Where(x => x.FleetsID.ToString() == rentedEqupmentSummary.FleetId && x.Date >= GlPeriod.DateFrom && x.Date <= GlPeriod.DateTo).ToList();
                        var notApproved = timeSheets.FirstOrDefault(x => !x.CostSheetApproved);
                        if(notApproved!=null)
                        {
                            ModelState.AddModelError("notApproved", "Approve all costsheets before creating summary");
                        }
                        var groupedTimeSheets = timeSheets.GroupBy(x => x.ProcurementSupplierID);
                        foreach (var g in groupedTimeSheets)
                        {
                            var timesheetsByProject = g.GroupBy(x => x.BranchId);
                            foreach (var groupedTimeSheet in timesheetsByProject)
                            {
                                var lastTimeSheet = groupedTimeSheet.OrderByDescending(x => x.LightVehicleTimeSheetId).FirstOrDefault();

                                var first = groupedTimeSheet.First();
                                var entity = new RentedEquipmentMonthlyPaymentSummary();
                                entity.BranchId = lastTimeSheet.BranchId;
                                entity.BranchName = db.Branch.FirstOrDefault(x => x.BranchId == lastTimeSheet.BranchId)?.Name ?? "";
                                entity.CreatedBy = User.Identity.Name;
                                entity.DateCreated = DateTime.Now;
                                entity.GlPeriodId = rentedEqupmentSummary.GlPeriodId;
                                entity.EquipmentTypeId = fleet.EquipmentTypeID.ToString();
                                entity.EquipmentType = equipmentType.EquipmentTypeName;
                                entity.FleetId = fleet.FleetsID.ToString();
                                if (fleet.PlateNumber != null)
                                {
                                    entity.PlateNo = fleet.PlateNumber;
                                }
                                if (entity.PlateNo == null)
                                {
                                    entity.PlateNo = fleet.SerialNo;
                                }
                                if (entity.PlateNo == null)
                                {
                                    entity.PlateNo = fleet.Model;
                                }
                                entity.TimeSheetPreparedBy = User.Identity.Name;
                                entity.CustomerName = db.ProcurementSuppliers.FirstOrDefault(x => x.ProcurementSupplierID == first.ProcurementSupplierID)?.Name ?? "";
                                entity.OperatorName = groupedTimeSheet.Where(x => x.FleetsID.ToString() == rentedEqupmentSummary.FleetId && x.Date >= GlPeriod.DateFrom && x.Date <= GlPeriod.DateTo && x.ExternalOperatorName != null).OrderByDescending(x => x.LightVehicleTimeSheetId).FirstOrDefault()?.ExternalOperatorName;
                                //entity.GrossBeforeVat = timeSheets.Sum(x => (decimal?)x.TotalTime * equipmentRent.RentalRate) ?? 0;
                                foreach (var timesheet in groupedTimeSheet)
                                {
                                    entity.GrossBeforeVat += timesheet.TotalTime * timesheet.RentalRate;
                                    entity.MaintenanceCost += timesheet.MaintenanceCost;
                                    entity.SparePartCost += timesheet.SparePartCost;
                                    entity.OilLubCost += timesheet.OilCost+timesheet.GreaseCost;
                                }
                                entity.VAT = entity.GrossBeforeVat * (equipmentRent.Vat);
                                //entity.TOT = entity.GrossBeforeVat * (equipmentRent.Tot);
                                entity.TotalWithVat = entity.GrossBeforeVat + entity.VAT ;
                                //entity.AdvancePayedWithVat = advancePayed;
                                entity.Deductable = groupedTimeSheet.Sum(x => (decimal?)x.MaintenanceCost) ?? 0 + groupedTimeSheet.Sum(x => (decimal?)x.OilCost) ?? 0 + groupedTimeSheet.Sum(x => (decimal?)x.GreaseCost) ?? 0 + groupedTimeSheet.Sum(x => (decimal?)x.SparePartCost) ?? 0;
                                entity.NetPayable = entity.TotalWithVat - entity.Deductable;
                                entity.TotalWorkedHR = groupedTimeSheet.Sum(x => (decimal?)x.TotalTime) ?? 0;

                                entity.AgreementNo = rentFromExternal.Num.ToString();
                                entity.RentalRateHour = equipmentRent.RentalRate;
                                entity.TimeSheetEquipmentAdminOfficerPreparedBy = User.Identity.Name;
                                entity.TimeSheetEquipmentAdminOfficerPreparedDate = DateTime.Now;
                                entity.RentalRateHour = equipmentRent.RentalRate;
                                entity.RentalRateDay = equipmentRent.RentalRate * 11;
                                entity.AgreementDate = rentFromExternal?.StartDate ?? DateTime.Now;
                                entity.TimeSheetPreparedDate = DateTime.Now;
                                //entity.TimeSheetCheckedBy = summary.TimeSheetCheckedBy;
                                entity.TimeSheetCheckedDate = DateTime.Now;
                                // entity.TimeSheetIsChecked = summary.TimeSheetIsChecked;
                                //entity.IsRejected = summary.IsRejected;
                                // entity.TimeSheetApprovedBy = summary.TimeSheetApprovedBy;
                                entity.TimeSheetApprovalDate = DateTime.Now;
                                //entity.TimeSheetIsApproved = summary.TimeSheetIsApproved;
                                //entity.TimeSheetAuthorizedBy = summary.TimeSheetAuthorizedBy;
                                entity.TimeSheetAuthorizedDate = DateTime.Now;
                                // entity.TimeSheetIsAuthorized = summary.TimeSheetIsAuthorized;
                                // entity.TimeSheetDGMApprovedBy = summary.TimeSheetDGMApprovedBy;
                                entity.TimeSheetDGMApprovalDate = DateTime.Now;
                                //entity.TimeSheetIsDGMApproved = summary.TimeSheetIsDGMApproved;
                                //var 
                                //entity.GrossBeforeVat = summary.GrossBeforeVat;



                                db.RentedEquipmentMonthlyPaymentSummary.Add(entity);
                                db.SaveChanges();
                                //var timeSheets = db.LightVehicleTimeSheets.Where(x => x.FleetsID.ToString() == rentedEqupmentSummary.FleetId && x.Date >= GlPeriod.DateFrom && x.Date <= GlPeriod.DateTo).ToList();
                                foreach (var timesheet in groupedTimeSheet)
                                {
                                    var timeSheetDetail = new RentedEquipmentDailyOperation();
                                    timeSheetDetail.RentedEquipmentMonthlyPaymentSummaryId = entity.RentedEquipmentMonthlyPaymentSummaryId;
                                    timeSheetDetail.AfternoonTimeEnd = timesheet.AfternoonTimeEnd;
                                    timeSheetDetail.AfternoonTimeStart = timesheet.AfternoonTimeStart;
                                    timeSheetDetail.MorningTimeEnd = timesheet.MorningTimeEnd;
                                    timeSheetDetail.MorningTimeStart = timesheet.MorningTimeStart;
                                    timeSheetDetail.NightTimeEnd = timesheet.NightTimeEnd;
                                    timeSheetDetail.NightTimeStart = timesheet.NightTimeStart;
                                    timeSheetDetail.PayedIdleHR = timesheet.IdleHours;
                                    timeSheetDetail.TotalPayedHR = timesheet.TotalTime;
                                    timeSheetDetail.WorkedHR = timesheet.NetHour;
                                    timeSheetDetail.Date = timesheet.Date;
                                    timeSheetDetail.MaintenanceCost = timesheet.MaintenanceCost;
                                    timeSheetDetail.SparePartCost = timesheet.SparePartCost;
                                    timeSheetDetail.OilLubCost = timesheet.OilCost + timesheet.GreaseCost;
                                    timeSheetDetail.Activity = timesheet.WorkType;
                                    timeSheetDetail.Volume = fleet.Volume;
                                    timeSheetDetail.MaintenanceCost = timesheet.MaintenanceCost;
                                    timeSheetDetail.SparePartCost = timesheet.SparePartCost;
                                    timeSheetDetail.OilLubCost = timesheet.OilCost + timesheet.GreaseCost;
                                    timeSheetDetail.Unit = fleet.Unit.ToString();
                                    timeSheetDetail.FuelKMReading = timesheet.FuelKMR;
                                    timeSheetDetail.FuelInLiter = timesheet.FuelConsumption;
                                    timeSheetDetail.FinalEngineHour = timesheet.FinalKMR;
                                    timeSheetDetail.InitialEngineHour = timesheet.InitialKMR;
                                    timeSheetDetail.DifferenceEngineHour = timesheet.DifferenceKMR;
                                    //if (timesheet.TimeSheetApprovedBy != null)
                                    //{
                                    //    timeSheetDetail.SiteFormanName = UserInformationHelper.GetEmployeeFullName(timesheet.TimeSheetApprovedBy);
                                    //}
                                    timeSheetDetail.DownHour = timesheet.DownHours;
                                    // timeSheetDetail.DownReasonList = timesheet..Value;
                                    if (timesheet.ReasonForDown.HasValue&&timesheet.DownHours>0)
                                    {
                                        timeSheetDetail.DownReasonList = timesheet.ReasonForDown.Value;
                                    }
                                    if (timesheet.ReasonForIdle.HasValue&&timesheet.IdleHours>0)
                                    {
                                        timeSheetDetail.IdleReasonList = timesheet.ReasonForIdle.Value;
                                    }
                                    timeSheetDetail.OperatorName = timesheet.ExternalOperatorName;
                                    db.RentedEquipmentDailyOperation.Add(timeSheetDetail);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                    if (equipmentType.TimeSheetType== TimeSheetType.DryLoading)
                    {
                        var timeSheets = db.DryLoadingTimeSheets.Where(x => x.FleetsID.ToString() == rentedEqupmentSummary.FleetId && x.TripStartDate >= GlPeriod.DateFrom && x.TripStartDate <= GlPeriod.DateTo).ToList();
                        var notApproved = timeSheets.FirstOrDefault(x => !x.CostSheetApproved);
                        if (notApproved != null)
                        {
                            ModelState.AddModelError("notApproved", "Approve all costsheets before creating summary");
                        }
                        var entity = new RentedEquipmentMonthlyPaymentSummary();

                        entity.CreatedBy = User.Identity.Name;
                        entity.DateCreated = DateTime.Now;
                        entity.GlPeriodId = rentedEqupmentSummary.GlPeriodId;
                        entity.EquipmentTypeId = fleet.EquipmentTypeID.ToString();
                        entity.EquipmentType = equipmentType.EquipmentTypeName;
                        entity.FleetId = fleet.FleetsID.ToString();
                        if (fleet.PlateNumber != null)
                        {
                            entity.PlateNo = fleet.PlateNumber;
                        }
                        if (entity.PlateNo == null)
                        {
                            entity.PlateNo = fleet.SerialNo;
                        }
                        if (entity.PlateNo == null)
                        {
                            entity.PlateNo = fleet.Model;
                        }
                        entity.TimeSheetPreparedBy = User.Identity.Name;
                        entity.CustomerName = db.ProcurementSuppliers.FirstOrDefault(x => x.ProcurementSupplierID == rentFromExternal.ProcurementSupplierID)?.Name ?? "";
                        entity.OperatorName=db.DryLoadingTimeSheets.Where(x => x.FleetsID.ToString() == rentedEqupmentSummary.FleetId && x.TripStartDate >= GlPeriod.DateFrom && x.TripStartDate <= GlPeriod.DateTo).OrderByDescending(x => x.DryLoadingTimeSheetId).FirstOrDefault()?.ExternalOperatorName;

                        //entity.GrossBeforeVat = timeSheets.Sum(x => (decimal?)x.TotalTime * equipmentRent.RentalRate) ?? 0;
                        foreach (var timesheet in timeSheets)
                        {
                            entity.GrossBeforeVat += timesheet.TotalTime * timesheet.RentalRate;
                            entity.MaintenanceCost += timesheet.MaintenanceCost;
                            entity.SparePartCost += timesheet.SparePartCost;
                            entity.OilLubCost += timesheet.OilCost + timesheet.GreaseCost;
                        }
                        entity.VAT = entity.GrossBeforeVat * equipmentRent.Vat;
                        //entity.TOT = entity.GrossBeforeVat * equipmentRent.Tot;
                        entity.TotalWithVat = entity.GrossBeforeVat + entity.VAT ;
                        entity.AdvancePayedWithVat = advancePayed;
                        entity.Deductable = timeSheets.Sum(x=>(decimal?)x.MaintenanceCost)??0+timeSheets.Sum(x=>(decimal?)x.OilCost)??0+timeSheets.Sum(x=>(decimal?)x.GreaseCost)??0+timeSheets.Sum(x=>(decimal?)x.SparePartCost)??0;
                        entity.NetPayable = entity.TotalWithVat-entity.Deductable;
                        entity.TotalWorkedHR = timeSheets.Sum(x => (decimal?)x.TotalTime) ?? 0;
                        entity.BranchId = fleet.BranchId;
                        entity.BranchName = db.Branch.FirstOrDefault(x => x.BranchId == fleet.BranchId)?.Name ?? "";
                        entity.AgreementNo = rentFromExternal.Num.ToString();
                        entity.RentalRateHour = equipmentRent.RentalRate;
                        entity.TimeSheetEquipmentAdminOfficerPreparedBy = User.Identity.Name;
                        entity.TimeSheetEquipmentAdminOfficerPreparedDate = DateTime.Now;
                        entity.RentalRateHour = equipmentRent.RentalRate;
                        entity.RentalRateDay = equipmentRent.RentalRate * 11;
                        entity.AgreementDate = rentFromExternal?.StartDate ?? DateTime.Now;
                        entity.TimeSheetPreparedDate = DateTime.Now;
                        //entity.TimeSheetCheckedBy = summary.TimeSheetCheckedBy;
                        entity.TimeSheetCheckedDate = DateTime.Now;
                        // entity.TimeSheetIsChecked = summary.TimeSheetIsChecked;
                        //entity.IsRejected = summary.IsRejected;
                        // entity.TimeSheetApprovedBy = summary.TimeSheetApprovedBy;
                        entity.TimeSheetApprovalDate = DateTime.Now;
                        //entity.TimeSheetIsApproved = summary.TimeSheetIsApproved;
                        //entity.TimeSheetAuthorizedBy = summary.TimeSheetAuthorizedBy;
                        entity.TimeSheetAuthorizedDate = DateTime.Now;
                        // entity.TimeSheetIsAuthorized = summary.TimeSheetIsAuthorized;
                        // entity.TimeSheetDGMApprovedBy = summary.TimeSheetDGMApprovedBy;
                        entity.TimeSheetDGMApprovalDate = DateTime.Now;
                        //entity.TimeSheetIsDGMApproved = summary.TimeSheetIsDGMApproved;
                        //var 
                        //entity.GrossBeforeVat = summary.GrossBeforeVat;



                        db.RentedEquipmentMonthlyPaymentSummary.Add(entity);
                        db.SaveChanges();

                    }
                    if (equipmentType.TimeSheetType== TimeSheetType.DumpTruck)
                    {
                        var timeSheets = db.DumpTruckTimeSheets.Where(x => x.FleetsID.ToString() == rentedEqupmentSummary.FleetId && x.Date >= GlPeriod.DateFrom && x.Date <= GlPeriod.DateTo).ToList();
                        var notApproved = timeSheets.FirstOrDefault(x => !x.CostSheetApproved);
                        if (notApproved != null)
                        {
                            ModelState.AddModelError("notApproved", "Approve all costsheets before creating summary");
                        }
                        var groupedTimeSheets = timeSheets.GroupBy(x => x.ProcurementSupplierID);
                        foreach (var g in groupedTimeSheets)
                        {
                            var timesheetsByProject = g.GroupBy(x => x.BranchId);
                            foreach (var groupedTimeSheet in timesheetsByProject)
                            {
                                var lastTimeSheet = groupedTimeSheet.OrderByDescending(x => x.DumpTruckTimeSheetId).FirstOrDefault();

                                var first = groupedTimeSheet.First();
                                var entity = new RentedEquipmentMonthlyPaymentSummary();
                                entity.BranchId = lastTimeSheet.BranchId;
                                entity.BranchName = db.Branch.FirstOrDefault(x => x.BranchId == lastTimeSheet.BranchId)?.Name ?? "";
                                entity.CreatedBy = User.Identity.Name;
                                entity.DateCreated = DateTime.Now;
                                entity.GlPeriodId = rentedEqupmentSummary.GlPeriodId;
                                entity.EquipmentTypeId = fleet.EquipmentTypeID.ToString();
                                entity.EquipmentType = equipmentType.EquipmentTypeName;
                                entity.FleetId = fleet.FleetsID.ToString();
                                if (fleet.PlateNumber != null)
                                {
                                    entity.PlateNo = fleet.PlateNumber;
                                }
                                if (entity.PlateNo == null)
                                {
                                    entity.PlateNo = fleet.SerialNo;
                                }
                                if (entity.PlateNo == null)
                                {
                                    entity.PlateNo = fleet.Model;
                                }
                                entity.TimeSheetPreparedBy = User.Identity.Name;
                                entity.CustomerName = db.ProcurementSuppliers.FirstOrDefault(x => x.ProcurementSupplierID == first.ProcurementSupplierID)?.Name ?? "";
                                entity.OperatorName = groupedTimeSheet.Where(x => x.FleetsID.ToString() == rentedEqupmentSummary.FleetId && x.Date >= GlPeriod.DateFrom && x.Date <= GlPeriod.DateTo && x.ExternalOperatorName != null).OrderByDescending(x => x.DumpTruckTimeSheetId).FirstOrDefault()?.ExternalOperatorName;
                                //entity.GrossBeforeVat = timeSheets.Sum(x => (decimal?)x.TotalTime * equipmentRent.RentalRate) ?? 0;
                                foreach (var timesheet in groupedTimeSheet)
                                {
                                    entity.GrossBeforeVat += timesheet.TotalTime * timesheet.RentalRate;
                                    entity.MaintenanceCost += timesheet.MaintenanceCost;
                                    entity.SparePartCost += timesheet.SparePartCost;
                                    entity.OilLubCost += timesheet.OilCost + timesheet.GreaseCost;
                                }
                                entity.VAT = entity.GrossBeforeVat * (equipmentRent.Vat);
                               // entity.TOT = entity.GrossBeforeVat * (equipmentRent.Tot);
                                entity.TotalWithVat = entity.GrossBeforeVat + entity.VAT;
                                //entity.AdvancePayedWithVat = advancePayed;
                                entity.Deductable = groupedTimeSheet.Sum(x => (decimal?)x.MaintenanceCost) ?? 0 + groupedTimeSheet.Sum(x => (decimal?)x.OilCost) ?? 0 + groupedTimeSheet.Sum(x => (decimal?)x.GreaseCost) ?? 0 + groupedTimeSheet.Sum(x => (decimal?)x.SparePartCost) ?? 0;
                                entity.NetPayable = entity.TotalWithVat - entity.Deductable;
                                entity.TotalWorkedHR = groupedTimeSheet.Sum(x => (decimal?)x.TotalTime) ?? 0;

                                entity.AgreementNo = rentFromExternal.Num.ToString();
                                entity.RentalRateHour = equipmentRent.RentalRate;
                                entity.TimeSheetEquipmentAdminOfficerPreparedBy = User.Identity.Name;
                                entity.TimeSheetEquipmentAdminOfficerPreparedDate = DateTime.Now;
                                entity.RentalRateHour = equipmentRent.RentalRate;
                                entity.RentalRateDay = equipmentRent.RentalRate * 11;
                                entity.AgreementDate = rentFromExternal?.StartDate ?? DateTime.Now;
                                entity.TimeSheetPreparedDate = DateTime.Now;
                                //entity.TimeSheetCheckedBy = summary.TimeSheetCheckedBy;
                                entity.TimeSheetCheckedDate = DateTime.Now;
                                // entity.TimeSheetIsChecked = summary.TimeSheetIsChecked;
                                //entity.IsRejected = summary.IsRejected;
                                // entity.TimeSheetApprovedBy = summary.TimeSheetApprovedBy;
                                entity.TimeSheetApprovalDate = DateTime.Now;
                                //entity.TimeSheetIsApproved = summary.TimeSheetIsApproved;
                                //entity.TimeSheetAuthorizedBy = summary.TimeSheetAuthorizedBy;
                                entity.TimeSheetAuthorizedDate = DateTime.Now;
                                // entity.TimeSheetIsAuthorized = summary.TimeSheetIsAuthorized;
                                // entity.TimeSheetDGMApprovedBy = summary.TimeSheetDGMApprovedBy;
                                entity.TimeSheetDGMApprovalDate = DateTime.Now;
                                //entity.TimeSheetIsDGMApproved = summary.TimeSheetIsDGMApproved;
                                //var 
                                //entity.GrossBeforeVat = summary.GrossBeforeVat;



                                db.RentedEquipmentMonthlyPaymentSummary.Add(entity);
                                db.SaveChanges();
                                //var timeSheets = db.LightVehicleTimeSheets.Where(x => x.FleetsID.ToString() == rentedEqupmentSummary.FleetId && x.Date >= GlPeriod.DateFrom && x.Date <= GlPeriod.DateTo).ToList();
                                foreach (var timesheet in groupedTimeSheet)
                                {
                                    var timeSheetDetail = new RentedEquipmentDailyOperation();
                                    timeSheetDetail.RentedEquipmentMonthlyPaymentSummaryId = entity.RentedEquipmentMonthlyPaymentSummaryId;
                                    timeSheetDetail.AfternoonTimeEnd = timesheet.AfternoonTimeEnd;
                                    timeSheetDetail.AfternoonTimeStart = timesheet.AfternoonTimeStart;
                                    timeSheetDetail.MorningTimeEnd = timesheet.MorningTimeEnd;
                                    timeSheetDetail.MorningTimeStart = timesheet.MorningTimeStart;
                                    timeSheetDetail.NightTimeEnd = timesheet.NightTimeEnd;
                                    timeSheetDetail.NightTimeStart = timesheet.NightTimeStart;
                                    timeSheetDetail.PayedIdleHR = timesheet.IdleHour;
                                    timeSheetDetail.TotalPayedHR = timesheet.TotalTime;
                                    timeSheetDetail.WorkedHR = timesheet.NetworkingHour;
                                    timeSheetDetail.Date = timesheet.Date;
                                    timeSheetDetail.MaintenanceCost = timesheet.MaintenanceCost;
                                    timeSheetDetail.SparePartCost = timesheet.SparePartCost;
                                    timeSheetDetail.OilLubCost = timesheet.OilCost + timesheet.GreaseCost;
                                    timeSheetDetail.MaintenanceCost = timesheet.MaintenanceCost;
                                    timeSheetDetail.SparePartCost = timesheet.SparePartCost;
                                    timeSheetDetail.OilLubCost = timesheet.OilCost + timesheet.GreaseCost;
                                    timeSheetDetail.Activity = timesheet.TypeOfWorkExcuted;
                                    timeSheetDetail.Volume = fleet.Volume;
                                    timeSheetDetail.Unit = fleet.Unit.ToString();
                                    timeSheetDetail.FuelKMReading = timesheet.FuelKMR;
                                    timeSheetDetail.FuelInLiter = timesheet.FuelConsumption;
                                    timeSheetDetail.FinalEngineHour = timesheet.FinalKMR;
                                    timeSheetDetail.InitialEngineHour = timesheet.InitialKMR;
                                    timeSheetDetail.DifferenceEngineHour = timesheet.DifferenceKMR;
                                    //if (timesheet.TimeSheetApprovedBy != null)
                                    //{
                                    //    timeSheetDetail.SiteFormanName = UserInformationHelper.GetEmployeeFullName(timesheet.TimeSheetApprovedBy);
                                    //}
                                    timeSheetDetail.DownHour = timesheet.DownHour;
                                    // timeSheetDetail.DownReasonList = timesheet..Value;
                                    if (timesheet.DownReason.HasValue&&timesheet.DownHour>0)
                                    {
                                        timeSheetDetail.DownReasonList = timesheet.DownReason.Value;
                                    }
                                    if (timesheet.IdleReason.HasValue&&timesheet.IdleHour>0)
                                    {
                                        timeSheetDetail.IdleReasonList = timesheet.IdleReason.Value;
                                    }
                                    timeSheetDetail.OperatorName = timesheet.ExternalOperatorName;
                                    db.RentedEquipmentDailyOperation.Add(timeSheetDetail);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                    if (equipmentType.TimeSheetType== TimeSheetType.WagonDrill)
                    {
                        var timeSheets = db.WagonDrillTimesheets.Where(x => x.FleetsID.ToString() == rentedEqupmentSummary.FleetId && x.Date >= GlPeriod.DateFrom && x.Date <= GlPeriod.DateTo).ToList();
                        var notApproved = timeSheets.FirstOrDefault(x => !x.CostSheetApproved);
                        if (notApproved != null)
                        {
                            ModelState.AddModelError("notApproved", "Approve all costsheets before creating summary");
                        }
                        var groupedTimeSheets = timeSheets.GroupBy(x => x.ProcurementSupplierID);
                        foreach (var g in groupedTimeSheets)
                        {
                            var timesheetsByProject = g.GroupBy(x => x.BranchId);
                            foreach (var groupedTimeSheet in timesheetsByProject)
                            {
                                var lastTimeSheet = groupedTimeSheet.OrderByDescending(x => x.WagonDrillTimesheetId).FirstOrDefault();
                                var first = groupedTimeSheet.First();
                                var entity = new RentedEquipmentMonthlyPaymentSummary();
                                entity.BranchId = lastTimeSheet.BranchId;
                                entity.BranchName = db.Branch.FirstOrDefault(x => x.BranchId == lastTimeSheet.BranchId)?.Name ?? "";

                                entity.CreatedBy = User.Identity.Name;
                                entity.DateCreated = DateTime.Now;
                                entity.GlPeriodId = rentedEqupmentSummary.GlPeriodId;
                                entity.EquipmentTypeId = fleet.EquipmentTypeID.ToString();
                                entity.EquipmentType = equipmentType.EquipmentTypeName;
                                entity.FleetId = fleet.FleetsID.ToString();
                                if (fleet.PlateNumber != null)
                                {
                                    entity.PlateNo = fleet.PlateNumber;
                                }
                                if (entity.PlateNo == null)
                                {
                                    entity.PlateNo = fleet.SerialNo;
                                }
                                if (entity.PlateNo == null)
                                {
                                    entity.PlateNo = fleet.Model;
                                }
                                entity.TimeSheetPreparedBy = User.Identity.Name;
                                entity.CustomerName = db.ProcurementSuppliers.FirstOrDefault(x => x.ProcurementSupplierID == first.ProcurementSupplierID)?.Name ?? "";
                                entity.OperatorName = groupedTimeSheet.Where(x => x.FleetsID.ToString() == rentedEqupmentSummary.FleetId && x.Date >= GlPeriod.DateFrom && x.Date <= GlPeriod.DateTo && x.ExternalOperatorName != null).OrderByDescending(x => x.WagonDrillTimesheetId).FirstOrDefault()?.ExternalOperatorName;
                                //entity.GrossBeforeVat = timeSheets.Sum(x => (decimal?)x.TotalTime * equipmentRent.RentalRate) ?? 0;
                                foreach (var timesheet in groupedTimeSheet)
                                {
                                    entity.GrossBeforeVat += Math.Round(timesheet.OperationDrilledHoleDepth * timesheet.NumberOfHolesDrilled * timesheet.RateInMeter, 2);
                                    entity.MaintenanceCost += timesheet.MaintenanceCost;
                                    entity.SparePartCost += timesheet.SparePartCost;
                                    entity.OilLubCost += timesheet.OilCost + timesheet.GreaseCost;
                                    // vm.OperationAmount = 
                                }
                                entity.VAT = entity.GrossBeforeVat * (equipmentRent.Vat);
                               // entity.TOT = entity.GrossBeforeVat * (equipmentRent.Tot);
                                entity.TotalWithVat = entity.GrossBeforeVat + entity.VAT;
                                //entity.AdvancePayedWithVat = advancePayed;
                                entity.Deductable = groupedTimeSheet.Sum(x => (decimal?)x.MaintenanceCost) ?? 0 + groupedTimeSheet.Sum(x => (decimal?)x.OilCost) ?? 0 + groupedTimeSheet.Sum(x => (decimal?)x.GreaseCost) ?? 0 + groupedTimeSheet.Sum(x => (decimal?)x.SparePartCost) ?? 0;
                                entity.NetPayable = entity.TotalWithVat - entity.Deductable;
                                entity.TotalWorkedHR = groupedTimeSheet.Sum(x => (decimal?)x.OperationDrilledHoleDepth*x.NumberOfHolesDrilled) ?? 0;
                                entity.AgreementNo = rentFromExternal.Num.ToString();
                                entity.RentalRateHour = equipmentRent.RentalRate;
                                entity.IsWagonDrill = true;
                                entity.TimeSheetEquipmentAdminOfficerPreparedBy = User.Identity.Name;
                                entity.TimeSheetEquipmentAdminOfficerPreparedDate = DateTime.Now;
                                entity.RentalRateHour = equipmentRent.RentalRate;
                                entity.RentalRateDay = equipmentRent.RentalRate * 11;
                                entity.AgreementDate = rentFromExternal?.StartDate ?? DateTime.Now;
                                entity.TimeSheetPreparedDate = DateTime.Now;
                                //entity.TimeSheetCheckedBy = summary.TimeSheetCheckedBy;
                                entity.TimeSheetCheckedDate = DateTime.Now;
                                // entity.TimeSheetIsChecked = summary.TimeSheetIsChecked;
                                //entity.IsRejected = summary.IsRejected;
                                // entity.TimeSheetApprovedBy = summary.TimeSheetApprovedBy;
                                entity.TimeSheetApprovalDate = DateTime.Now;
                                //entity.TimeSheetIsApproved = summary.TimeSheetIsApproved;
                                //entity.TimeSheetAuthorizedBy = summary.TimeSheetAuthorizedBy;
                                entity.TimeSheetAuthorizedDate = DateTime.Now;
                                // entity.TimeSheetIsAuthorized = summary.TimeSheetIsAuthorized;
                                // entity.TimeSheetDGMApprovedBy = summary.TimeSheetDGMApprovedBy;
                                entity.TimeSheetDGMApprovalDate = DateTime.Now;
                                //entity.TimeSheetIsDGMApproved = summary.TimeSheetIsDGMApproved;
                                //var 
                                //entity.GrossBeforeVat = summary.GrossBeforeVat;



                                db.RentedEquipmentMonthlyPaymentSummary.Add(entity);
                                db.SaveChanges();
                                //var timeSheets = db.LightVehicleTimeSheets.Where(x => x.FleetsID.ToString() == rentedEqupmentSummary.FleetId && x.Date >= GlPeriod.DateFrom && x.Date <= GlPeriod.DateTo).ToList();
                                foreach (var timesheet in groupedTimeSheet)
                                {
                                    var timeSheetDetail = new WagonDrillDailyOperation();
                                    timeSheetDetail.RentedEquipmentMonthlyPaymentSummaryId = entity.RentedEquipmentMonthlyPaymentSummaryId;
                                    timeSheetDetail.NumberOfHolesDrilled = timesheet.NumberOfHolesDrilled;
                                    timeSheetDetail.OperationAmount = timesheet.OperationAmount;
                                    timeSheetDetail.OperationDrilledHoleDepth = timesheet.OperationDrilledHoleDepth;
                                    timeSheetDetail.OperationRateForMeter = timesheet.RateInMeter;
                                    timeSheetDetail.PayedIdleHour = 0;
                                    timeSheetDetail.TotalTime = timesheet.TotalTime;
                                    timeSheetDetail.WorkedHR = timesheet.OperationNetWorkingHour;
                                    timeSheetDetail.Date = timesheet.Date;
                                    timeSheetDetail.MaintenanceCost = timesheet.MaintenanceCost;
                                    timeSheetDetail.SparePartCost = timesheet.SparePartCost;
                                    timeSheetDetail.OilLubCost = timesheet.OilCost + timesheet.GreaseCost;
                                    timeSheetDetail.Activity = timesheet.WorkItem;
                                    timeSheetDetail.Volume = fleet.Volume;
                                    timeSheetDetail.Unit = fleet.Unit.ToString();
                                    timeSheetDetail.FuelKMReading = timesheet.FuelKMR;
                                    timeSheetDetail.FuelInLiter = timesheet.FuelInLiter;
                                    timeSheetDetail.FinalEngineHour = timesheet.FinalEHR;
                                    timeSheetDetail.InitialEngineHour = timesheet.InitialEHR;
                                    timeSheetDetail.DifferenceEngineHour = timesheet.DifferenceEHR;
                                    //if (timesheet.TimeSheetApprovedBy != null)
                                    //{
                                    //    timeSheetDetail.SiteFormanName = UserInformationHelper.GetEmployeeFullName(timesheet.TimeSheetApprovedBy);
                                    //}
                                    timeSheetDetail.DownHour = timesheet.DownTime;
                                    // timeSheetDetail.DownReasonList = timesheet..Value;
                                    if (timesheet.DownReason.HasValue&&timesheet.DownTime>0)
                                    {
                                        timeSheetDetail.DownReasonList = timesheet.DownReason.Value;
                                    }
                                    if (timesheet.IdleReason.HasValue&&timesheet.IdleHour>0)
                                    {
                                        timeSheetDetail.IdleReasonList = timesheet.IdleReason.Value;
                                    }
                                    timeSheetDetail.OperatorName = timesheet.ExternalOperatorName;
                                    db.WagonDrillDailyOperation.Add(timeSheetDetail);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                    if (equipmentType.TimeSheetType == TimeSheetType.Machinery)
                    {
                        var timeSheets = db.MachineryWorkingTimeSheets.Where(x => x.FleetsID.ToString() == rentedEqupmentSummary.FleetId && x.Date >= GlPeriod.DateFrom && x.Date <= GlPeriod.DateTo).ToList();
                        var notApproved = timeSheets.FirstOrDefault(x => !x.CostSheetApproved);
                        if (notApproved != null)
                        {
                            ModelState.AddModelError("notApproved", "Approve all costsheets before creating summary");
                        }
                        var groupedTimeSheets = timeSheets.GroupBy(x => x.ProcurementSupplierID);
                        foreach (var g in groupedTimeSheets)
                        {
                            var timesheetsByProject = g.GroupBy(x => x.BranchId);
                            foreach (var groupedTimeSheet in timesheetsByProject)
                            {
                                var lastTimeSheet = groupedTimeSheet.OrderByDescending(x => x.MachineryWorkingTimeSheetId).FirstOrDefault();
                                var first = groupedTimeSheet.First();
                                var entity = new RentedEquipmentMonthlyPaymentSummary();
                                entity.BranchId = lastTimeSheet.BranchId;
                                entity.BranchName = db.Branch.FirstOrDefault(x => x.BranchId == lastTimeSheet.BranchId)?.Name ?? "";

                                entity.CreatedBy = User.Identity.Name;
                                entity.DateCreated = DateTime.Now;
                                entity.GlPeriodId = rentedEqupmentSummary.GlPeriodId;
                                entity.EquipmentTypeId = fleet.EquipmentTypeID.ToString();
                                entity.EquipmentType = equipmentType.EquipmentTypeName;
                                entity.FleetId = fleet.FleetsID.ToString();
                                if (fleet.PlateNumber != null)
                                {
                                    entity.PlateNo = fleet.PlateNumber;
                                }
                                if (entity.PlateNo == null)
                                {
                                    entity.PlateNo = fleet.SerialNo;
                                }
                                if (entity.PlateNo == null)
                                {
                                    entity.PlateNo = fleet.Model;
                                }
                                entity.TimeSheetPreparedBy = User.Identity.Name;
                                entity.CustomerName = db.ProcurementSuppliers.FirstOrDefault(x => x.ProcurementSupplierID == first.ProcurementSupplierID)?.Name ?? "";
                                entity.OperatorName = groupedTimeSheet.Where(x => x.FleetsID.ToString() == rentedEqupmentSummary.FleetId && x.Date >= GlPeriod.DateFrom && x.Date <= GlPeriod.DateTo && x.ExternalOperatorName != null).OrderByDescending(x => x.MachineryWorkingTimeSheetId).FirstOrDefault()?.ExternalOperatorName;
                                //entity.GrossBeforeVat = timeSheets.Sum(x => (decimal?)x.TotalTime * equipmentRent.RentalRate) ?? 0;
                                foreach (var timesheet in groupedTimeSheet)
                                {
                                    entity.GrossBeforeVat += timesheet.TotalTime * timesheet.RentalRate;
                                    entity.MaintenanceCost += timesheet.MaintenanceCost;
                                    entity.SparePartCost += timesheet.SparePartCost;
                                    entity.OilLubCost += timesheet.OilCost + timesheet.GreaseCost;
                                }
                                entity.VAT = entity.GrossBeforeVat * (equipmentRent.Vat);
                                //entity.TOT = entity.GrossBeforeVat * (equipmentRent.Tot);
                                entity.TotalWithVat = entity.GrossBeforeVat + entity.VAT;
                                //entity.AdvancePayedWithVat = advancePayed;
                                entity.Deductable = groupedTimeSheet.Sum(x => (decimal?)x.MaintenanceCost) ?? 0 + groupedTimeSheet.Sum(x => (decimal?)x.OilCost) ?? 0 + groupedTimeSheet.Sum(x => (decimal?)x.GreaseCost) ?? 0 + groupedTimeSheet.Sum(x => (decimal?)x.SparePartCost) ?? 0;
                                entity.NetPayable = entity.TotalWithVat - entity.Deductable;
                                entity.TotalWorkedHR = groupedTimeSheet.Sum(x => (decimal?)x.TotalTime) ?? 0;
                                entity.AgreementNo = rentFromExternal.Num.ToString();
                                entity.RentalRateHour = equipmentRent.RentalRate;
                                entity.TimeSheetEquipmentAdminOfficerPreparedBy = User.Identity.Name;
                                entity.TimeSheetEquipmentAdminOfficerPreparedDate = DateTime.Now;
                                entity.RentalRateDay = equipmentRent.RentalRate * 11;
                                entity.AgreementDate = rentFromExternal?.StartDate ?? DateTime.Now;
                                entity.TimeSheetPreparedDate = DateTime.Now;
                                //entity.TimeSheetCheckedBy = summary.TimeSheetCheckedBy;
                                entity.TimeSheetCheckedDate = DateTime.Now;
                                // entity.TimeSheetIsChecked = summary.TimeSheetIsChecked;
                                //entity.IsRejected = summary.IsRejected;
                                // entity.TimeSheetApprovedBy = summary.TimeSheetApprovedBy;
                                entity.TimeSheetApprovalDate = DateTime.Now;
                                //entity.TimeSheetIsApproved = summary.TimeSheetIsApproved;
                                //entity.TimeSheetAuthorizedBy = summary.TimeSheetAuthorizedBy;
                                entity.TimeSheetAuthorizedDate = DateTime.Now;
                                // entity.TimeSheetIsAuthorized = summary.TimeSheetIsAuthorized;
                                // entity.TimeSheetDGMApprovedBy = summary.TimeSheetDGMApprovedBy;
                                entity.TimeSheetDGMApprovalDate = DateTime.Now;
                                //entity.TimeSheetIsDGMApproved = summary.TimeSheetIsDGMApproved;
                                //var 
                                //entity.GrossBeforeVat = summary.GrossBeforeVat;



                                db.RentedEquipmentMonthlyPaymentSummary.Add(entity);
                                db.SaveChanges();
                                //var timeSheets = db.LightVehicleTimeSheets.Where(x => x.FleetsID.ToString() == rentedEqupmentSummary.FleetId && x.Date >= GlPeriod.DateFrom && x.Date <= GlPeriod.DateTo).ToList();
                                foreach (var timesheet in groupedTimeSheet)
                                {
                                    var timeSheetDetail = new RentedEquipmentDailyOperation();
                                    timeSheetDetail.RentedEquipmentMonthlyPaymentSummaryId = entity.RentedEquipmentMonthlyPaymentSummaryId;
                                    timeSheetDetail.AfternoonTimeEnd = timesheet.AfternoonTimeEnd;
                                    timeSheetDetail.AfternoonTimeStart = timesheet.AfternoonTimeStart;
                                    timeSheetDetail.MorningTimeEnd = timesheet.MorningTimeEnd;
                                    timeSheetDetail.MorningTimeStart = timesheet.MorningTimeStart;
                                    timeSheetDetail.NightTimeEnd = timesheet.NightTimeEnd;
                                    timeSheetDetail.NightTimeStart = timesheet.NightTimeStart;
                                    timeSheetDetail.PayedIdleHR = timesheet.IdleHours;
                                    timeSheetDetail.TotalPayedHR = timesheet.TotalTime;
                                    timeSheetDetail.WorkedHR = timesheet.NetHour;
                                    timeSheetDetail.Date = timesheet.Date;
                                    timeSheetDetail.MaintenanceCost = timesheet.MaintenanceCost;
                                    timeSheetDetail.SparePartCost = timesheet.SparePartCost;
                                    timeSheetDetail.OilLubCost = timesheet.OilCost + timesheet.GreaseCost;
                                    timeSheetDetail.Activity = timesheet.WorkType;
                                    timeSheetDetail.Volume = fleet.Volume;
                                    timeSheetDetail.Unit = fleet.Unit.ToString();
                                    timeSheetDetail.FuelKMReading = timesheet.FuelKMR;
                                    timeSheetDetail.FuelInLiter = timesheet.FuelConsumption;
                                    timeSheetDetail.FinalEngineHour = timesheet.FinalEHR;
                                    timeSheetDetail.InitialEngineHour = timesheet.InitialEHR;
                                    timeSheetDetail.DifferenceEngineHour = timesheet.DifferenceEHR;
                                    //if (timesheet.TimeSheetApprovedBy != null)
                                    //{
                                    //    timeSheetDetail.SiteFormanName = UserInformationHelper.GetEmployeeFullName(timesheet.TimeSheetApprovedBy);
                                    //}
                                    timeSheetDetail.DownHour = timesheet.DownHours;
                                    // timeSheetDetail.DownReasonList = timesheet..Value;
                                    if (timesheet.ReasonForDown.HasValue)
                                    {
                                        timeSheetDetail.DownReasonList = timesheet.ReasonForDown.Value;
                                    }
                                    if (timesheet.ReasonForIdle.HasValue)
                                    {
                                        timeSheetDetail.IdleReasonList = timesheet.ReasonForIdle.Value;
                                    }
                                    timeSheetDetail.OperatorName = timesheet.ExternalOperatorName;
                                    db.RentedEquipmentDailyOperation.Add(timeSheetDetail);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            return Json(new[] { rentedEqupmentSummary }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Update([DataSourceRequest]DataSourceRequest request,RentedEquipmentMonthlyPaymentSummary paymentSummary)
        {
            ModelState.Remove("NumberOfHolesDrilled");
            ModelState.Remove("OperationDrilledHoleDepth");
            ModelState.Remove("AmountDue");
            ModelState.Remove("IsWagonDrill");
            ModelState.Remove("Paid");
            ModelState.Remove("TotalAmount");
            var existing = db.RentedEquipmentMonthlyPaymentSummary.FirstOrDefault(x => x.RentedEquipmentMonthlyPaymentSummaryId == paymentSummary.RentedEquipmentMonthlyPaymentSummaryId);
            var equipmentRent = db.RentFromExternalEquipments.Where(x => x.FleetId.ToString() == paymentSummary.FleetId&&x.IsOnlineApproved).OrderByDescending(x => x.RentFromExternalEquipmentId).FirstOrDefault();
            existing.RentalRateHour = paymentSummary.RentalRateHour;
            existing.AdvancePayedWithVat = paymentSummary.AdvancePayedWithVat;
            var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID.ToString() == paymentSummary.FleetId);
            var equipmentType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == fleet.EquipmentTypeID);
            if(equipmentType.TimeSheetType==TimeSheetType.WagonDrill)
            {
                var wagonDrillDetails = db.WagonDrillDailyOperation.Where(x => x.RentedEquipmentMonthlyPaymentSummaryId == paymentSummary.RentedEquipmentMonthlyPaymentSummaryId).ToList();
                foreach(var detail in wagonDrillDetails)
                {
                    if(paymentSummary.RentalRateHour!=0)
                        existing.GrossBeforeVat+= Math.Round(detail.NumberOfHolesDrilled * detail.OperationDrilledHoleDepth * paymentSummary.RentalRateHour, 2);
                    else
                    {
                        existing.GrossBeforeVat += Math.Round(detail.NumberOfHolesDrilled * detail.OperationDrilledHoleDepth * detail.OperationRateForMeter, 2);
                    }
                }
                }
            else
            {
                existing.GrossBeforeVat = paymentSummary.RentalRateHour * paymentSummary.TotalWorkedHR;
            }
          
            existing.VAT = existing.GrossBeforeVat * equipmentRent.Vat;
            //existing.TOT = existing.GrossBeforeVat * equipmentRent.Tot;
            if(paymentSummary.CustomerName!=null)
            {
                existing.CustomerName = paymentSummary.CustomerName;
            }
            //existing.TOT = existing.GrossBeforeVat * equipmentRent.Tot;
            existing.TotalWithVat = existing.GrossBeforeVat + existing.VAT;
            existing.OperatorName = paymentSummary.OperatorName;
            existing.Deductable = paymentSummary.MaintenanceCost + paymentSummary.SparePartCost + paymentSummary.OilLubCost + paymentSummary.FuelCost;
            existing.NetPayable = existing.TotalWithVat-existing.Deductable-paymentSummary.AdvancePayedWithVat;
            existing.MaintenanceCost = paymentSummary.MaintenanceCost;
            existing.SparePartCost = paymentSummary.SparePartCost;
            existing.AdvancePayedWithVat = paymentSummary.AdvancePayedWithVat;
            existing.OilLubCost = paymentSummary.OilLubCost;
            existing.FuelCost = paymentSummary.FuelCost;
            if (ModelState.IsValid)
            {
                db.Entry(existing).State = EntityState.Modified;
                db.SaveChanges();
            }
            var rentalPaymentLine = db.RentalPaymentRequestLines.FirstOrDefault(x => x.CostSheetSummaryId == existing.RentedEquipmentMonthlyPaymentSummaryId);
            if(rentalPaymentLine != null)
            {
                var rentalPaymentRequest = db.RentalPaymentRequests.FirstOrDefault(x => x.RentalPaymentRequestId == rentalPaymentLine.RentalPaymentRequestId);
                rentalPaymentLine.WithholdingAmount = existing.GrossBeforeVat * rentalPaymentRequest.WithHoldingTax;
                rentalPaymentLine.TotalAmount = existing.TotalWithVat;
                rentalPaymentLine.AdvancedPayment = existing.AdvancePayedWithVat;
                rentalPaymentLine.Amount = existing.NetPayable - rentalPaymentLine.WithholdingAmount;
                rentalPaymentLine.CostSheetSummaryId = paymentSummary.RentedEquipmentMonthlyPaymentSummaryId;
                rentalPaymentLine.GrossBeforeVat = existing.GrossBeforeVat;
                rentalPaymentLine.VAT = existing.VAT;
                rentalPaymentLine.SparePartCost = existing.SparePartCost;
                rentalPaymentLine.MaintenanceCost = existing.MaintenanceCost;
                rentalPaymentLine.OilLubCost = existing.OilLubCost;
                rentalPaymentLine.FuelCost = existing.FuelCost;
                rentalPaymentLine.SubTotalAmount = existing.NetPayable - rentalPaymentLine.WithholdingAmount;
                rentalPaymentLine.TotalCost = existing.Deductable;
                db.Entry(rentalPaymentLine).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { paymentSummary}.ToDataSourceResult(request,ModelState));
        }
        public ActionResult TimeSheetSummaryDestroy([DataSourceRequest] DataSourceRequest request, RentedEquipmentMonthlyPaymentSummary summary)
        {
            ModelState.Remove("TOT");
            ModelState.Remove("NumberOfHolesDrilled");
            ModelState.Remove("OperationDrilledHoleDepth");
            ModelState.Remove("AmountDue");
            ModelState.Remove("IsWagonDrill");
            ModelState.Remove("Paid");
            ModelState.Remove("TotalAmount");
            var existingSummary = db.RentedEquipmentMonthlyPaymentSummary.FirstOrDefault(x => x.RentedEquipmentMonthlyPaymentSummaryId == summary.RentedEquipmentMonthlyPaymentSummaryId);
            var details = db.RentedEquipmentDailyOperation.Where(x => x.RentedEquipmentMonthlyPaymentSummaryId == summary.RentedEquipmentMonthlyPaymentSummaryId).ToList();
            db.RentedEquipmentDailyOperation.RemoveRange(details);
            db.SaveChanges();
            db.RentedEquipmentMonthlyPaymentSummary.Remove(existingSummary);
            db.SaveChanges();
            return Json(new[] { summary }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult CheckTimeSheetSummery(int id)
        {

            //Guid Id = Guid.Parse(EId);
            var timeSheetSummary = db.RentedEquipmentMonthlyPaymentSummary.Where(x => x.RentedEquipmentMonthlyPaymentSummaryId == id).FirstOrDefault();
            if (timeSheetSummary != null)
            {
                timeSheetSummary.TimeSheetIsChecked = true;
                timeSheetSummary.TimeSheetCheckedBy = User.Identity.Name;
                timeSheetSummary.TimeSheetCheckedDate = DateTime.Now;
                //db.SitesVehicleTransfer.Attach(SitesVehicleTransfer);
                db.Entry(timeSheetSummary).State = EntityState.Modified;
                db.SaveChanges();
            }
            var formattedData = new
            {
                Message = "CostSheet Summery Checked",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);

        }public ActionResult PrepareTimeSheetSummery(int id)
        {

            //Guid Id = Guid.Parse(EId);
            var timeSheetSummary = db.RentedEquipmentMonthlyPaymentSummary.Where(x => x.RentedEquipmentMonthlyPaymentSummaryId == id).FirstOrDefault();
            if (timeSheetSummary != null)
            {
                timeSheetSummary.TimeSheetIsPrepared = true;
                timeSheetSummary.TimeSheetPreparedBy = User.Identity.Name;
                timeSheetSummary.TimeSheetPreparedDate = DateTime.Now;
                //db.SitesVehicleTransfer.Attach(SitesVehicleTransfer);
                db.Entry(timeSheetSummary).State = EntityState.Modified;
                db.SaveChanges();
            }
            var formattedData = new
            {
                Message = "CostSheet Summery Prepared",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);

        }
        public ActionResult CheckEquipmentAdminTimeSheetSummery(int id)
        {

            //Guid Id = Guid.Parse(EId);
            var timeSheetSummary = db.RentedEquipmentMonthlyPaymentSummary.Where(x => x.RentedEquipmentMonthlyPaymentSummaryId == id).FirstOrDefault();
            if (timeSheetSummary != null)
            {
                timeSheetSummary.TimeSheetIsEquipmentAdminChecked = true;
                timeSheetSummary.TimeSheetEquipmentAdminCheckedBy = User.Identity.Name;
                timeSheetSummary.TimeSheetEquipmentAdminCheckedDate = DateTime.Now;
                //db.SitesVehicleTransfer.Attach(SitesVehicleTransfer);
                db.Entry(timeSheetSummary).State = EntityState.Modified;
                db.SaveChanges();
            }
            var formattedData = new
            {
                Message = "TimeSheet Checked by equipment admin susscessfully",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);

        }  public ActionResult CheckOfficeEngineerTimeSheetSummery(int id)
        {

            //Guid Id = Guid.Parse(EId);
            var timeSheetSummary = db.RentedEquipmentMonthlyPaymentSummary.Where(x => x.RentedEquipmentMonthlyPaymentSummaryId == id).FirstOrDefault();
            if (timeSheetSummary != null)
            {
                timeSheetSummary.TimeSheetIsOfficeEngineerChecked = true;
                timeSheetSummary.TimeSheetOfficeEngineerCheckedBy = User.Identity.Name;
                timeSheetSummary.TimeSheetOfficeEngineerCheckedDate = DateTime.Now;
                //db.SitesVehicleTransfer.Attach(SitesVehicleTransfer);
                db.Entry(timeSheetSummary).State = EntityState.Modified;
                db.SaveChanges();
            }
            var formattedData = new
            {
                Message = "CostSheet Summery Checked",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);

        }  public ActionResult CheckConstructionEngineerTimeSheetSummery(int id)
        {

            //Guid Id = Guid.Parse(EId);
            var timeSheetSummary = db.RentedEquipmentMonthlyPaymentSummary.Where(x => x.RentedEquipmentMonthlyPaymentSummaryId == id).FirstOrDefault();
            if (timeSheetSummary != null)
            {
                timeSheetSummary.TimeSheetIsConstructionEngineerApproved = true;
                timeSheetSummary.TimeSheetConstructionEngineerApprovedBy = User.Identity.Name;
                timeSheetSummary.TimeSheetConstructionEngineerApprovedDate = DateTime.Now;
                //db.SitesVehicleTransfer.Attach(SitesVehicleTransfer);
                db.Entry(timeSheetSummary).State = EntityState.Modified;
                db.SaveChanges();
            }
            var formattedData = new
            {
                Message = "CostSheet Summery Checked",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);

        }  public ActionResult TimeSheetDepartmentManagerTimeSheetSummery(int id)
        {

            //Guid Id = Guid.Parse(EId);
            var timeSheetSummary = db.RentedEquipmentMonthlyPaymentSummary.Where(x => x.RentedEquipmentMonthlyPaymentSummaryId == id).FirstOrDefault();
            if (timeSheetSummary != null)
            {
                timeSheetSummary.TimeSheetIsDepartmentManagerApproved = true;
                timeSheetSummary.TimeSheetDepartmentManagerApprovedBy = User.Identity.Name;
                timeSheetSummary.TimeSheetDepartmentManagerApprovedDate = DateTime.Now;
                //db.SitesVehicleTransfer.Attach(SitesVehicleTransfer);
                db.Entry(timeSheetSummary).State = EntityState.Modified;
                db.SaveChanges();
            }
            var formattedData = new
            {
                Message = "CostSheet Summery Checked",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ApproveTimeSheetSummery(int id)
        {

            //Guid Id = Guid.Parse(EId);
            var timeSheetSummary = db.RentedEquipmentMonthlyPaymentSummary.Where(x => x.RentedEquipmentMonthlyPaymentSummaryId == id).FirstOrDefault();
            if (timeSheetSummary != null)
            {
                timeSheetSummary.TimeSheetIsApproved = true;
                timeSheetSummary.TimeSheetApprovedBy = User.Identity.Name;
                timeSheetSummary.TimeSheetApprovalDate = DateTime.Now;
                //db.SitesVehicleTransfer.Attach(SitesVehicleTransfer);
                db.Entry(timeSheetSummary).State = EntityState.Modified;
                db.SaveChanges();
            }
            var formattedData = new
            {
                Message = "CostSheet Summery Approved",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);

        }
        public ActionResult AuthorizeTimeSheetSummery(int id)
        {

            //Guid Id = Guid.Parse(EId);
            var timeSheetSummary = db.RentedEquipmentMonthlyPaymentSummary.Where(x => x.RentedEquipmentMonthlyPaymentSummaryId == id).FirstOrDefault();
            if (timeSheetSummary != null)
            {
                timeSheetSummary.TimeSheetIsAuthorized = true;
                timeSheetSummary.TimeSheetAuthorizedBy = User.Identity.Name;
                timeSheetSummary.TimeSheetAuthorizedDate = DateTime.Now;
                //db.SitesVehicleTransfer.Attach(SitesVehicleTransfer);
                db.Entry(timeSheetSummary).State = EntityState.Modified;
                db.SaveChanges();
            }
            var formattedData = new
            {
                Message = "CostSheet Summery Approved",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DGMApproveTimeSheetSummery(int id)
        {

            //Guid Id = Guid.Parse(EId);
            var timeSheetSummary = db.RentedEquipmentMonthlyPaymentSummary.Where(x => x.RentedEquipmentMonthlyPaymentSummaryId == id).FirstOrDefault();
            if (timeSheetSummary != null)
            {
                timeSheetSummary.TimeSheetIsDGMApproved = true;
                timeSheetSummary.TimeSheetDGMApprovedBy = User.Identity.Name;
                timeSheetSummary.TimeSheetDGMApprovalDate = DateTime.Now;
                //db.SitesVehicleTransfer.Attach(SitesVehicleTransfer);
                db.Entry(timeSheetSummary).State = EntityState.Modified;
                db.SaveChanges();
            }
            var formattedData = new
            {
                Message = "CostSheet Summery Authorized",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }


       
        public JsonResult TimeSheetSummaryReject(int id,string Text)
        {
            var timeSheetSummary = db.RentedEquipmentMonthlyPaymentSummary.Where(x => x.RentedEquipmentMonthlyPaymentSummaryId == id).FirstOrDefault();
            if (timeSheetSummary != null)
            {

                if (timeSheetSummary.TimeSheetIsAuthorized)
                {
                    timeSheetSummary.TimeSheetIsAuthorized = false;
                    timeSheetSummary.TimeSheetDGMApprovedBy = User.Identity.Name;
                    timeSheetSummary.TimeSheetDGMApprovalDate = DateTime.Now;
                    db.Entry(timeSheetSummary).State = EntityState.Modified;
                    db.SaveChanges();
                    var formattedDataa = new
                    {
                        Message = "CostSheet Rejected",
                        Text = "Authorizer",
                        Status = "OK"
                    };
                    return Json(formattedDataa, JsonRequestBehavior.AllowGet);
                }
                if (timeSheetSummary.TimeSheetIsApproved)
                {
                    timeSheetSummary.TimeSheetIsApproved = false;
                    timeSheetSummary.TimeSheetAuthorizedBy = User.Identity.Name;
                    timeSheetSummary.TimeSheetAuthorizedDate = DateTime.Now;
                    db.Entry(timeSheetSummary).State = EntityState.Modified;
                    db.SaveChanges();
                    var formattedDa = new
                    {
                        Message = "CostSheet Rejected",
                        Text = "Approver",
                        Status = "OK"
                    };
                    return Json(formattedDa, JsonRequestBehavior.AllowGet);
                }
                if (timeSheetSummary.TimeSheetIsChecked)
                {

                    timeSheetSummary.TimeSheetIsChecked = false;
                    timeSheetSummary.TimeSheetApprovedBy = User.Identity.Name;
                    timeSheetSummary.TimeSheetApprovalDate = DateTime.Now;
                    db.Entry(timeSheetSummary).State = EntityState.Modified;
                    db.SaveChanges();
                    var formattedDat = new
                    {
                        Message = "CostSheet Rejected",
                        Text = "checker",
                        Status = "OK"
                    };
                    return Json(formattedDat, JsonRequestBehavior.AllowGet);

                }
                if (timeSheetSummary.TimeSheetIsPrepared)
                {

                    timeSheetSummary.TimeSheetIsPrepared = false;
                    timeSheetSummary.TimeSheetCheckedBy = User.Identity.Name;
                    timeSheetSummary.TimeSheetCheckedDate = DateTime.Now;
                    db.Entry(timeSheetSummary).State = EntityState.Modified;
                    db.SaveChanges();
                    var formattedDat = new
                    {
                        Message = "CostSheet Rejected",
                        Text = "checker",
                        Status = "OK"
                    };
                    return Json(formattedDat, JsonRequestBehavior.AllowGet);

                }
                if (timeSheetSummary.TimeSheetIsDepartmentManagerApproved)
                {

                    timeSheetSummary.TimeSheetIsDepartmentManagerApproved = false;
                    timeSheetSummary.TimeSheetDepartmentManagerApprovedBy = " ";
                    timeSheetSummary.TimeSheetDepartmentManagerApprovedDate = DateTime.Now;
                    db.Entry(timeSheetSummary).State = EntityState.Modified;
                    db.SaveChanges();
                    var formattedDat = new
                    {
                        Message = "TimeSheet Summary Rejected",
                        Text = "DGM Approver",
                        Status = "OK"
                    };
                    return Json(formattedDat, JsonRequestBehavior.AllowGet);

                }

                if (timeSheetSummary.TimeSheetIsConstructionEngineerApproved)
                {
                    timeSheetSummary.TimeSheetIsConstructionEngineerApproved = false;
                    timeSheetSummary.TimeSheetDepartmentManagerApprovedBy = User.Identity.Name;
                    timeSheetSummary.TimeSheetDepartmentManagerApprovedDate = DateTime.Now;
                    db.Entry(timeSheetSummary).State = EntityState.Modified;
                    db.SaveChanges();
                    var formattedDat = new
                    {
                        Message = "Timesheet Rejected",
                        Text = "checker",
                        Status = "OK"
                    };
                    return Json(formattedDat, JsonRequestBehavior.AllowGet);

                }
                if (timeSheetSummary.TimeSheetIsOfficeEngineerChecked)
                {
                    timeSheetSummary.TimeSheetIsOfficeEngineerChecked = false;
                    timeSheetSummary.TimeSheetConstructionEngineerApprovedBy = User.Identity.Name;
                    timeSheetSummary.TimeSheetConstructionEngineerApprovedDate = DateTime.Now;
                    db.Entry(timeSheetSummary).State = EntityState.Modified;
                    db.SaveChanges();
                    var formattedDat = new
                    {
                        Message = "Timesheet Rejected",
                        Text = "checker",
                        Status = "OK"
                    };
                    return Json(formattedDat, JsonRequestBehavior.AllowGet);

                }
                if (timeSheetSummary.TimeSheetIsEquipmentAdminChecked)
                {
                    timeSheetSummary.TimeSheetIsEquipmentAdminChecked = false;
                    timeSheetSummary.TimeSheetOfficeEngineerCheckedBy = User.Identity.Name;
                    timeSheetSummary.TimeSheetOfficeEngineerCheckedDate = DateTime.Now;
                    db.Entry(timeSheetSummary).State = EntityState.Modified;
                    db.SaveChanges();
                    var formattedDat = new
                    {
                        Message = "TimeSheet Rejected",
                        Text = "checker",
                        Status = "OK"
                    };
                    return Json(formattedDat, JsonRequestBehavior.AllowGet);

                }
                
                //db.SitesVehicleTransfer.Attach(SitesVehicleTransfer);


            }
            var formattedData = new
            {
                Message = "Timesheet Rejected",
                Text = "",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);

        }
        public JsonResult ApproveRentedEquipments()
        {
            var externalFleets = db.Fleets.Where(x => x.IsExternal && x.RentFromExternalNum != null).Select(x => x.FleetsID).ToList();
            var checkLightVehicles = db.LightVehicleTimeSheets.Where(x => externalFleets.Contains(x.FleetsID));
            foreach (var lightVehicle in checkLightVehicles)
            {
                if (lightVehicle.ProcurementSupplierID == null)
                {
                    var Imediatevehilce = db.LightVehicleTimeSheets.Where(x => x.Date < lightVehicle.Date && x.ProcurementSupplierID != null).OrderByDescending(x => x.Date).FirstOrDefault();
                    if (Imediatevehilce != null)
                    {
                        lightVehicle.ProcurementSupplierID = Imediatevehilce.ProcurementSupplierID;
                        db.Entry(lightVehicle).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (lightVehicle.FinalKMR == 0 && lightVehicle.InitialKMR > 0)
                {
                    lightVehicle.FinalKMR = lightVehicle.InitialKMR;
                    lightVehicle.DifferenceKMR = 0;
                    db.Entry(lightVehicle).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                if (lightVehicle.CostSheetApproved == false)
                {
                    lightVehicle.TotalTime = lightVehicle.NetHour;
                    db.Entry(lightVehicle).State = System.Data.Entity.EntityState.Modified;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                    {
                        var sqlEx = ex.InnerException;

                        while (sqlEx.InnerException != null)
                        {
                            sqlEx = sqlEx.InnerException;
                        }

                        throw new Exception(sqlEx.Message);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                        throw;
                    }
                }
                if (lightVehicle.CostSheetApproved && lightVehicle.TotalTime == 0)
                {
                    lightVehicle.TotalTime = lightVehicle.IdleHours + lightVehicle.NetHour;
                    db.Entry(lightVehicle).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            var checkMachineryVehicles = db.MachineryWorkingTimeSheets.Where(x => externalFleets.Contains(x.FleetsID)); ;
            foreach (var Vehicle in checkMachineryVehicles)
            {
                var Imediatevehilce = db.MachineryWorkingTimeSheets.Where(x => x.Date < Vehicle.Date && x.ProcurementSupplierID != null).OrderByDescending(x => x.Date).FirstOrDefault();
                if (Imediatevehilce != null)
                {
                    Vehicle.ProcurementSupplierID = Imediatevehilce.ProcurementSupplierID;
                    db.Entry(Vehicle).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                if (Vehicle.FinalEHR == 0 && Vehicle.InitialEHR > 0)
                {
                    Vehicle.FinalEHR = Vehicle.InitialEHR;
                    Vehicle.DifferenceEHR = 0;
                    db.Entry(Vehicle).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                if (Vehicle.CostSheetApproved == false)
                {
                    Vehicle.TotalTime = Vehicle.NetHour;
                    db.Entry(Vehicle).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
                if (Vehicle.CostSheetApproved && Vehicle.TotalTime == 0)
                {
                    Vehicle.TotalTime = Vehicle.IdleHours + Vehicle.NetHour;
                    db.Entry(Vehicle).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            var checkDumpTruckVehicles = db.DumpTruckTimeSheets.Where(x => externalFleets.Contains(x.FleetsID));
            foreach (var Vehicle in checkDumpTruckVehicles)
            {
                var Imediatevehilce = db.DumpTruckTimeSheets.Where(x => x.Date < Vehicle.Date && x.ProcurementSupplierID != null).OrderByDescending(x => x.Date).FirstOrDefault();
                if (Imediatevehilce != null)
                {
                    Vehicle.ProcurementSupplierID = Imediatevehilce.ProcurementSupplierID;
                    db.Entry(Vehicle).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                if (Vehicle.FinalKMR == 0 && Vehicle.InitialKMR > 0)
                {
                    Vehicle.FinalKMR = Vehicle.InitialKMR;
                    Vehicle.DifferenceKMR = 0;
                    db.Entry(Vehicle).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                if (Vehicle.CostSheetApproved == false)
                {
                    Vehicle.TotalTime = Vehicle.NetworkingHour;
                    db.Entry(Vehicle).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                if (Vehicle.CostSheetApproved && Vehicle.TotalTime == 0)
                {
                    Vehicle.TotalTime = Vehicle.IdleHour + Vehicle.NetworkingHour;
                    db.Entry(Vehicle).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            var checkWagonDrillVehicles = db.WagonDrillTimesheets.Where(x => externalFleets.Contains(x.FleetsID));
            foreach (var Vehicle in checkWagonDrillVehicles)
            {
                var Imediatevehilce = db.WagonDrillTimesheets.Where(x => x.Date < Vehicle.Date && x.ProcurementSupplierID != null).OrderByDescending(x => x.Date).FirstOrDefault();
                if (Imediatevehilce != null)
                {
                    Vehicle.ProcurementSupplierID = Imediatevehilce.ProcurementSupplierID;
                    db.Entry(Vehicle).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                if (Vehicle.FinalEHR == 0 && Vehicle.InitialEHR > 0)
                {
                    Vehicle.FinalEHR = Vehicle.InitialEHR;
                    Vehicle.DifferenceEHR = 0;
                    db.Entry(Vehicle).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                if (Vehicle.CostSheetApproved == false)
                {
                    Vehicle.TotalTime = Vehicle.OperationNetWorkingHour;
                    db.Entry(Vehicle).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                if (Vehicle.CostSheetApproved && Vehicle.TotalTime == 0)
                {
                    Vehicle.TotalTime = Vehicle.IdleHour + Vehicle.OperationNetWorkingHour;
                    db.Entry(Vehicle).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            var rentfromExternalIds = db.RentFromExternal.Where(x => x.IsOnlineApproved && !x.IsExpired).Select(x => x.RentFromExternalID);
            var equipments = db.RentFromExternalEquipments.Where(x => rentfromExternalIds.Contains(x.RentFromExternalId)).ToList();
            foreach (var equipment in equipments)
            {
                equipment.IsOnlineApproved = true;
                db.Entry(equipment).State = System.Data.Entity.EntityState.Modified;

            }
            var fleets = db.Fleets.Where(x => x.IsExternal).ToList();
            var FleetsbyPlates = fleets.Where(x => x.PlateNumber != null).Select(x => x.PlateNumber).ToList();
            //var FleetsBySerials = fleets.Where(x=>x.PlateNumber==null&&x.SerialNo!=null).Select(x => x.SerialNo).ToList();
            //var FleetsByModels = fleets.Where(x=>x.PlateNumber==null&&x.SerialNo==null).Select(x => x.Model).ToList();
            List<string> DuplicatedPlates = FleetsbyPlates
                        .GroupBy(p => p)
                        .Where(g => g.Count() > 1)
                        .Select(g => g.Key)
                        .ToList();
            foreach (var plate in DuplicatedPlates)
            {
                var duplicatedFleets = db.Fleets.Where(x => x.IsExternal && x.PlateNumber == plate).ToList();
                if (duplicatedFleets.Count > 1)
                {
                    var first = duplicatedFleets.First();
                    var timesheetType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == first.EquipmentTypeID).TimeSheetType;
                    if (timesheetType == TimeSheetType.LightVehicle)
                    {
                        var approvedFleet = db.Fleets.FirstOrDefault(x => x.IsExternal && x.PlateNumber == plate && x.RentFromExternalNum != null);
                        var notApprovedFleet = db.Fleets.FirstOrDefault(x => x.IsExternal && x.PlateNumber == plate && x.RentFromExternalNum == null);
                        if (approvedFleet != null && notApprovedFleet != null)
                        {
                            var timeSheets = db.LightVehicleTimeSheets.Where(x => x.FleetsID == notApprovedFleet.FleetsID).ToList();
                            foreach (var timesheet in timeSheets)
                            {
                                timesheet.FleetsID = approvedFleet.FleetsID;
                                db.Entry(timesheet).State = System.Data.Entity.EntityState.Modified;
                            }
                            db.SaveChanges();
                        }
                    }
                    if (timesheetType == TimeSheetType.DumpTruck)
                    {
                        var approvedFleet = db.Fleets.FirstOrDefault(x => x.IsExternal && x.PlateNumber == plate && x.RentFromExternalNum != null);
                        var notApprovedFleet = db.Fleets.FirstOrDefault(x => x.IsExternal && x.PlateNumber == plate && x.RentFromExternalNum == null);
                        if (approvedFleet != null && notApprovedFleet != null)
                        {
                            var timeSheets = db.DumpTruckTimeSheets.Where(x => x.FleetsID == notApprovedFleet.FleetsID).ToList();
                            foreach (var timesheet in timeSheets)
                            {
                                timesheet.FleetsID = approvedFleet.FleetsID;
                                db.Entry(timesheet).State = System.Data.Entity.EntityState.Modified;
                            }
                            db.SaveChanges();
                        }
                    }
                    if (timesheetType == TimeSheetType.Machinery)
                    {
                        var approvedFleet = db.Fleets.FirstOrDefault(x => x.IsExternal && x.PlateNumber == plate && x.RentFromExternalNum != null);
                        var notApprovedFleet = db.Fleets.FirstOrDefault(x => x.IsExternal && x.PlateNumber == plate && x.RentFromExternalNum == null);
                        if (approvedFleet != null && notApprovedFleet != null)
                        {
                            var timeSheets = db.MachineryWorkingTimeSheets.Where(x => x.FleetsID == notApprovedFleet.FleetsID).ToList();
                            foreach (var timesheet in timeSheets)
                            {
                                timesheet.FleetsID = approvedFleet.FleetsID;
                                db.Entry(timesheet).State = System.Data.Entity.EntityState.Modified;
                            }
                            db.SaveChanges();
                        }
                    }
                    if (timesheetType == TimeSheetType.WagonDrill)
                    {
                        var approvedFleet = db.Fleets.FirstOrDefault(x => x.IsExternal && x.PlateNumber == plate && x.RentFromExternalNum != null);
                        var notApprovedFleet = db.Fleets.FirstOrDefault(x => x.IsExternal && x.PlateNumber == plate && x.RentFromExternalNum == null);
                        if (approvedFleet != null && notApprovedFleet != null)
                        {
                            var timeSheets = db.WagonDrillTimesheets.Where(x => x.FleetsID == notApprovedFleet.FleetsID).ToList();
                            foreach (var timesheet in timeSheets)
                            {
                                timesheet.FleetsID = approvedFleet.FleetsID;
                                db.Entry(timesheet).State = System.Data.Entity.EntityState.Modified;
                            }
                            db.SaveChanges();
                        }
                    }
                }
            }
            db.SaveChanges();
            var formattedData = new
            {
                Message = "Go to your timeSheet summary try to check if it is comming",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SummaryDoc_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var summaryDoc = db.RentalEquipmentPaymentSummaryDocument.Where(x => x.RentedEquipmentMonthlyPaymentSummaryId == id);
            return Json(summaryDoc.ToDataSourceResult(request));
        }
        public ActionResult SummaryDoc_Delete([DataSourceRequest]DataSourceRequest request, RentalEquipmentPaymentSummaryDocument model)
        {
            var summaryDoc = db.RentalEquipmentPaymentSummaryDocument.FirstOrDefault(x => x.RentedEquipmentMonthlyPaymentSummaryId == model.RentedEquipmentMonthlyPaymentSummaryId);
            db.RentalEquipmentPaymentSummaryDocument.Remove(summaryDoc);
            db.SaveChanges();
            var parent = db.RentedEquipmentMonthlyPaymentSummary.FirstOrDefault(x => x.RentedEquipmentMonthlyPaymentSummaryId == model.RentedEquipmentMonthlyPaymentSummaryId);
            parent.IsPaymentLetterAttached = false;
            db.Entry(parent).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SummaryAttachment_Create([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> attachments, int id)
        {
            if (attachments != null)
            {
                string[] AllowedExtensions = { ".jpg", ".png", ".pdf" };
                int MaxFileSize = 2 * 1024 * 1024;

                string subPath = "~/App_Data/RentalEquipmentPaymentSummary/";
                string shortPath = "/App_Data/RentalEquipmentPaymentSummary/";

                bool exists = Directory.Exists(Server.MapPath(subPath));

                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(subPath));

                foreach (var file in attachments)
                {
                    if (file != null)
                    {
                        #region file validation
                        var fileExtension = Path.GetExtension(file.FileName).ToLower();
                        if (!AllowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("File", "Invalid file type.");
                        }
                        if (file.ContentLength > MaxFileSize)
                        {
                            ModelState.AddModelError("File", $"File size exceeds the {MaxFileSize / (2024 * 2024)} MB limit.");
                        }
                        #endregion
                        if (ModelState.IsValid)
                        {
                            string fileName = Path.GetFileName(file.FileName);
                            string physicalPath = Server.MapPath(subPath + fileName);
                            //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                            file.SaveAs(physicalPath);
                            db.RentalEquipmentPaymentSummaryDocument.Add(
                               new RentalEquipmentPaymentSummaryDocument()
                               {
                                   FileName = Path.GetFileName(file.FileName),
                                   FileExtension = fileExtension,
                                   RentedEquipmentMonthlyPaymentSummaryId = id,
                                   ShortPath = shortPath,
                                   FilePath = physicalPath
                               }
                           );
                            db.SaveChanges();
                            var parent = db.RentedEquipmentMonthlyPaymentSummary.FirstOrDefault(x => x.RentedEquipmentMonthlyPaymentSummaryId == id);
                            parent.IsPaymentLetterAttached = true;
                            db.Entry(parent).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Rental Equipment Payment Summary attachment", "Success", User.Identity.Name, $"created attachement under po {id} ", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { attachments }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult RejectDGMApproval(int id)
        {
            var record = db.RentedEquipmentMonthlyPaymentSummary.FirstOrDefault(x=>x.RentedEquipmentMonthlyPaymentSummaryId==id);
            record.TimeSheetIsDGMApproved = false;
            record.TimeSheetDGMApprovedBy = null;
            db.Entry(record).State = EntityState.Modified;
            db.SaveChanges();
            var formattedData = new
            {
                Message = "Rejection Successfull",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
         public JsonResult VoidSummary(int id)
        {
            var record = db.RentedEquipmentMonthlyPaymentSummary.FirstOrDefault(x => x.RentedEquipmentMonthlyPaymentSummaryId == id);
            //var rentalPaymentRequestLine = db.RentalPaymentRequestLines.FirstOrDefault(x => x.CostSheetSummaryId == id);
            // if(rentalPaymentRequestLine!=null)
            //{
            //    var FormattedData = new
            //    {
            //        Message = "Summary is sent to on PR Void not allowed",
            //        Status = "OK"
            //    };
            //    return Json(FormattedData, JsonRequestBehavior.AllowGet);
            //}
      
            record.TimeSheetIsPrepared = false;
            record.TimeSheetPreparedBy = null;
            record.TimeSheetIsChecked = false;
            record.TimeSheetCheckedBy = null;
            record.TimeSheetIsApproved = false;
            record.TimeSheetApprovedBy = null;
            record.TimeSheetIsAuthorized = false;
            record.TimeSheetAuthorizedBy = null;
            record.TimeSheetIsDGMApproved = false;
            record.TimeSheetDGMApprovedBy = null;
            db.Entry(record).State = EntityState.Modified;
            db.SaveChanges();
            var formattedData = new
            {
                Message = "Void Success",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DownloadSummaryDocument(int CompanyDocumentId)
        {
            var record = db.RentalEquipmentPaymentSummaryDocument
                .Find(CompanyDocumentId);

            if (record == null)
                return HttpNotFound();

            var extension = Path.GetExtension(record.FileName).ToLower();

            string contentType;

            switch (extension)
            {
                case ".png":
                    contentType = "image/png";
                    break;

                case ".jpg":
                case ".jpeg":
                    contentType = "image/jpeg";
                    break;

                case ".pdf":
                    contentType = "application/pdf";
                    break;

                default:
                    contentType = "application/octet-stream";
                    break;
            }

            return File(record.FilePath, contentType);
        }
        //public ActionResult DownloadSummaryDocument(int CompanyDocumentId)
        //{
        //    var record = db.RentalEquipmentPaymentSummaryDocument.Find(CompanyDocumentId);

        //    return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        //}
    }
}

