using ExceedERP.Core.Domain.FleetManagement.BatteryManagement;
using ExceedERP.Core.Domain.FleetManagement.EquipmentInspection;
using ExceedERP.Core.Domain.FleetManagement.EquipmentPlanandEvaluation;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.Core.Domain.FleetManagement.FleetSalvageStore;
using ExceedERP.Core.Domain.FleetManagement.FleetTireManagement;
using ExceedERP.Core.Domain.FleetManagement.FuelManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Core.Domain.FleetManagement.FleetInsurances;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.FleetManagement.RentalPayment;
using ExceedERP.Core.Domain.FleetManagement;

namespace ExceedERP.DataAccess.Context
{
    
    public class FleetManagementEntityConfiguration
    {
        public class TimeSheetExceptionEntityTypeConfiguration : EntityTypeConfiguration<TimeSheetException>
        {
            public TimeSheetExceptionEntityTypeConfiguration()
            {
                this.ToTable("TimeSheetException", "FleetManagement");
                this.HasKey<int>(p => p.TimeSheetExceptionId);
            }
        }  public class RentalPaymentRequestFinanceValidationEntityTypeConfiguration : EntityTypeConfiguration<RentalPaymentRequestValidation>
        {
            public RentalPaymentRequestFinanceValidationEntityTypeConfiguration()
            {
                this.ToTable("RentalPaymentRequestFinanceValidation", "FleetManagement");
                this.HasKey<int>(p => p.RentalPaymentRequestValidationId);
            }
        }
        
        public class TimeSheetSettingEntityTypeConfiguration : EntityTypeConfiguration<TimeSheetSetting>
        {
            public TimeSheetSettingEntityTypeConfiguration()
            {
                this.ToTable("TimeSheetSetting", "FleetManagement");
                this.HasKey<int>(p => p.TimeSheetSettingId);
            }
        }
        public class InspectionStatusKeyEntityTypeConfiguration : EntityTypeConfiguration<InspectionStatusKey>
        {
            public InspectionStatusKeyEntityTypeConfiguration()
            {
                this.ToTable("InspectionStatusKeys", "FleetManagement");
                this.HasKey<int>(p => p.InspectionStatusKeyId);
            }
        }
        public class RentFromExternalEquipmentRateEntityTypeConfiguration : EntityTypeConfiguration<RentFromExternalEquipmentRate>
        {
            public RentFromExternalEquipmentRateEntityTypeConfiguration()
            {
                this.ToTable("RentFromExternalEquipmentRate", "FleetManagement");
                this.HasKey<int>(p => p.RentFromExternalEquipmentRateId);
            }
        }
        public class RequestToInspectionEntityTypeConfiguration : EntityTypeConfiguration<RequestToInspection>
        {
            public RequestToInspectionEntityTypeConfiguration()
            {
                this.ToTable("RequestToInspection", "FleetManagement");
                this.HasKey<int>(p => p.RequestToInspectionId);
            }
        }
        public class FleetInsuranceDocumentEntityTypeConfiguration : EntityTypeConfiguration<FleetInsuranceDocument>
        {
            public FleetInsuranceDocumentEntityTypeConfiguration()
            {
                this.ToTable("FleetInsuranceDocument", "FleetManagement");
                this.HasKey<int>(p => p.FleetInsuranceDocumentId);
            }
        }
        public class TireIssueDetailEntityTypeConfiguration : EntityTypeConfiguration<TireIssueDetail>
        {
            public TireIssueDetailEntityTypeConfiguration()
            {
                this.ToTable("TireIssueDetails", "FleetManagement");
                this.HasKey<int>(p => p.TireIssueDetailId);
            }
        }
        public class FleetRentalRateEntityTypeConfiguration : EntityTypeConfiguration<FleetRentalRate>
        {
            public FleetRentalRateEntityTypeConfiguration()
            {
                this.ToTable("FleetRentalRate", "FleetManagement");
                this.HasKey<int>(p => p.FleetRentalRateId);
            }
        }
        public class EquipmentRateEntityTypeConfiguration : EntityTypeConfiguration<EquipmentRate>
        {
            public EquipmentRateEntityTypeConfiguration()
            {
                this.ToTable("EquipmentRate", "FleetManagement");
                this.HasKey<int>(p => p.EquipmentRateId);
            }
        }
        public class FleetAnnualPlanEntitytypeConfiguration : EntityTypeConfiguration<FleetAnnualPlan>
        {
            public FleetAnnualPlanEntitytypeConfiguration()
            {
                this.ToTable("FleetAnnualPlan", "FleetManagement");
                this.HasKey<int>(p => p.FleetAnnualPlanId);
            }
        } public class RoadORLoadEntitytypeConfiguration : EntityTypeConfiguration<RoadORLoad>
        {
            public RoadORLoadEntitytypeConfiguration()
            {
                this.ToTable("RoadORLoad", "FleetManagement");
                this.HasKey<int>(p => p.RoadOrLoadId);
            }
        }
        public class HeavyORDumpTruckEntitytypeConfiguration : EntityTypeConfiguration<HeavyORDumpTruck>
        {
            public HeavyORDumpTruckEntitytypeConfiguration()
            {
                this.ToTable("HeavyORDumpTruck", "FleetManagement");
                this.HasKey<int>(p => p.HeavyORDumpTruckId);
            }
        }
        public class RateOfRoadORLoadEntitytypeConfiguration : EntityTypeConfiguration<RateOfRoadORLoad>
        {
            public RateOfRoadORLoadEntitytypeConfiguration()
            {
                this.ToTable("RateOfRoadORLoad", "FleetManagement");
                this.HasKey<int>(p => p.RateOfRoadORLoadId);
            }
        }
        public class FleetAnnualPlanDetailEntityTypeConfiguration : EntityTypeConfiguration<FleetAnnualPlanDetail>
        {
            public FleetAnnualPlanDetailEntityTypeConfiguration()
            {
                this.ToTable("FleetAnnualPlanDetail", "FleetManagement");
                this.HasKey<int>(p => p.FleetAnnualPlanDetailId);
            }
        }
        public class FleetBatteryAndTireEntityTypeConfiguration : EntityTypeConfiguration<FleetBatteryAndTire>
        {
            public FleetBatteryAndTireEntityTypeConfiguration()
            {
                this.ToTable("FleetBatteryAndTire", "FleetManagement");
                this.HasKey<int>(p => p.FleetBatteryAndTireId);
            }
        }
        public class FleetBoloRegistrationEntityTypeConfiguration : EntityTypeConfiguration<FleetBoloRegistration>
        {
            public FleetBoloRegistrationEntityTypeConfiguration()
            {
                this.ToTable("FleetBoloRegistration", "FleetManagement");
                this.HasKey<int>(p => p.FleetBoloRegistrationId);
            }
        }
        public class RentFromExternalEquipmentEntityTypeConfiguration : EntityTypeConfiguration<RentFromExternalEquipment>
        {
            public RentFromExternalEquipmentEntityTypeConfiguration()
            {
                this.ToTable("RentFromExternalEquipment", "FleetManagement");
                this.HasKey<int>(k => k.RentFromExternalEquipmentId);
            }
        }
        public class AssignedEquipmentAdditionalInfoEntityTypeConfiguration : EntityTypeConfiguration<AssignedEquipmentAdditionalInfo>
        {

            public AssignedEquipmentAdditionalInfoEntityTypeConfiguration()
            {
                this.ToTable("AssignedEquipmentAdditionalInfo", "FleetManagement");
                this.HasKey<int>(t => t.AssignedEquipmentAdditionalInfoId);

            }
        }
        public class DryLoadingEntityTypeConfiguration : EntityTypeConfiguration<DryLoadingTimeSheet>
        {

            public DryLoadingEntityTypeConfiguration()
            {
                this.ToTable("DryLoadingTimeSheet", "FleetManagement");
                this.HasKey<int>(t => t.DryLoadingTimeSheetId);
                 
            }
        }
        public class LightVehicleEntityTypeConfiguration : EntityTypeConfiguration<LightVehicleTimeSheet>
        {
            public LightVehicleEntityTypeConfiguration()
            {
                this.ToTable("LightVehicleTimesheet", "FleetManagement");
                this.HasKey<int>(t => t.LightVehicleTimeSheetId);
            }
        }
        public class WagonDrillTimesheetEntityTypeConfiguration : EntityTypeConfiguration<WagonDrillTimesheet>
        {

            public WagonDrillTimesheetEntityTypeConfiguration()
            {
                this.ToTable("WagonDrillTimesheet", "FleetManagement");
                this.HasKey<int>(t => t.WagonDrillTimesheetId);

            }
        }
        public class DumpTruckTimeSheetEntityTypeConfiguration : EntityTypeConfiguration<DumpTruckTimeSheet>
        {

            public DumpTruckTimeSheetEntityTypeConfiguration()
            {
                this.ToTable("DumpTruckTimeSheet", "FleetManagement");
                this.HasKey<int>(t => t.DumpTruckTimeSheetId);

            }
        }

        public class DumpTruckTimeSheetDetailEntityTypeConfiguration : EntityTypeConfiguration<DumpTruckTimeSheetDetail>
        {

            public DumpTruckTimeSheetDetailEntityTypeConfiguration()
            {
                this.ToTable("DumpTruckTimeSheetDetail", "FleetManagement");
                this.HasKey<int>(t => t.DumpTruckTimeSheetDetailId);

            }
        }
        public class RentalInspectionEntityTypeConfiguration : EntityTypeConfiguration<RentalInspection>
        {

            public RentalInspectionEntityTypeConfiguration()
            {
                this.ToTable("RentalInspection", "FleetManagement");
                this.HasKey<int>(t => t.RentalInspectionId);

            }
        }
        public class RentalPaymentRequestEntityTypeConfiguration : EntityTypeConfiguration<RentalPaymentRequest>
        {

            public RentalPaymentRequestEntityTypeConfiguration()
            {
                this.ToTable("RentalPaymentRequest", "FleetManagement");
                this.HasKey<int>(t => t.RentalPaymentRequestId);

            }
        }
        public class RentalPaymentRequestValidationEntityTypeConfiguration : EntityTypeConfiguration<RentalPaymentRequestValidation>
        {

            public RentalPaymentRequestValidationEntityTypeConfiguration()
            {
                this.ToTable("RentalPaymentRequestValidation", "FleetManagement");
                this.HasKey<int>(t => t.RentalPaymentRequestValidationId);

            }
        }
        public class RentalPaymentRequestDepartmentValidationEntityTypeConfiguration : EntityTypeConfiguration<RentalPaymentRequestDepartmentValidation>
        {

            public RentalPaymentRequestDepartmentValidationEntityTypeConfiguration()
            {
                this.ToTable("RentalPaymentRequestDepartmentValidation", "FleetManagement");
                this.HasKey<int>(t => t.RentalPaymentRequestDepartmentValidationId);

            }
        }
        public class RentalPaymentRequestBudgetValidationEntityTypeConfiguration : EntityTypeConfiguration<RentalPaymentRequestBudgetValidation>
        {

            public RentalPaymentRequestBudgetValidationEntityTypeConfiguration()
            {
                this.ToTable("RentalPaymentRequestBudgetValidation", "FleetManagement");
                this.HasKey<int>(t => t.RentalPaymentRequestBudgetValidationId);

            }
        }
        public class RentalPaymentRequestLineEntityTypeConfiguration : EntityTypeConfiguration<RentalPaymentRequestLine>
        {

            public RentalPaymentRequestLineEntityTypeConfiguration()
            {
                this.ToTable("RentalPaymentRequestLine", "FleetManagement");
                this.HasKey<int>(t => t.RentalPaymentRequestLineId);

            }
        }
        public class RentalInspectionValidationEntityTypeConfiguration : EntityTypeConfiguration<RentalInspectionValidation>
        {

            public RentalInspectionValidationEntityTypeConfiguration()
            {
                this.ToTable("RentalInspectionValidation", "FleetManagement");
                this.HasKey<int>(t => t.RentalInspectionValidationId);

            }
        }
        public class RentalInspectionCriteriaEntityTypeConfiguration : EntityTypeConfiguration<RentalInspectionCriteria>
        {

            public RentalInspectionCriteriaEntityTypeConfiguration()
            {
                this.ToTable("RentalInspectionCriteria", "FleetManagement");
                this.HasKey<int>(t => t.RentalInspectionCriteriaId);

            }
        }
        public class RentalInspectionCriteriaDetailEntityTypeConfiguration : EntityTypeConfiguration<RentalInspectionCriteriaDetail>
        {

            public RentalInspectionCriteriaDetailEntityTypeConfiguration()
            {
                this.ToTable("RentalInspectionCriteriaDetail", "FleetManagement");
                this.HasKey<int>(t => t.RentalInspectionCriteriaDetailId);

            }
        }
        public class EquipmentCheckListEntityTypeConfiguration : EntityTypeConfiguration<EquipmentCheckList>
        {

            public EquipmentCheckListEntityTypeConfiguration()
            {
                this.ToTable("EquipmentCheckList", "FleetManagement");
                this.HasKey<int>(t => t.EquipmentCheckListId);

            }
        }
        public class RentalMVInspectionEntityTypeConfiguration : EntityTypeConfiguration<RentalMVInspection>
        {

            public RentalMVInspectionEntityTypeConfiguration()
            {
                this.ToTable("RentalMVInspection", "FleetManagement");
                this.HasKey<int>(t => t.RentalMVInspectionId);

            }
        }
        public class MachineryWorkingTimeSheetEntityTypeConfiguration : EntityTypeConfiguration<MachineryWorkingTimeSheet>
        {

            public MachineryWorkingTimeSheetEntityTypeConfiguration()
            {
                this.ToTable("MachineryWorkingTimeSheet", "FleetManagement");
                this.HasKey<int>(t => t.MachineryWorkingTimeSheetId);

            }
        }

        public class FleetReturnApprovalEntityTypeConfiguration : EntityTypeConfiguration<FleetReturnApproval>
        {

            public FleetReturnApprovalEntityTypeConfiguration()
            {
                this.ToTable("FleetReturnApproval", "FleetManagement");
                this.HasKey<int>(t => t.FleetReturnApprovalId);

            }
        }

        public class FleetReturnItemEntityTypeConfiguration : EntityTypeConfiguration<FleetReturnItem>
        {

            public FleetReturnItemEntityTypeConfiguration()
            {
                this.ToTable("FleetReturnItem", "FleetManagement");
                this.HasKey<int>(t => t.FleetReturnItemId);

            }
        }
        public class FleetReturnEntityTypeConfiguration : EntityTypeConfiguration<FleetReturn>
        {

            public FleetReturnEntityTypeConfiguration()
            {
                this.ToTable("FleetReturn", "FleetManagement");
                this.HasKey<int>(t => t.FleetReturnId);

            }
        }
        public class ExternalRentedEquipmentCheckListEntityTypeConfiguration : EntityTypeConfiguration<ExternalRentedEquipmentCheckList>
        {

            public ExternalRentedEquipmentCheckListEntityTypeConfiguration()
            {
                this.ToTable("ExternalRentedEquipmentCheckList", "FleetManagement");
                this.HasKey<Guid>(t => t.ExternalRentedEquipmentCheckListId);

            }
        }

        public class LicenseTypeEntityTypeConfiguration : EntityTypeConfiguration<LicenseType>
        {

            public LicenseTypeEntityTypeConfiguration()
            {
                this.ToTable("LicenseType", "FleetManagement");
                this.HasKey<int>(t => t.LicenseTypeId);

            }
        }
        public class LicenseLevelEntityTypeConfiguration : EntityTypeConfiguration<LicenseLevel>
        {

            public LicenseLevelEntityTypeConfiguration()
            {
                this.ToTable("LicenseLevel", "FleetManagement");
                this.HasKey<int>(t => t.LicenseLevelId);

            }
        }
        public class OwningandOperatingCostConstantEntityTypeConfiguration : EntityTypeConfiguration<OwningandOperatingCostConstant>
        {

            public OwningandOperatingCostConstantEntityTypeConfiguration()
            {
                this.ToTable("OwningandOperatingCostConstant", "FleetManagement");
                this.HasKey<int>(t => t.OwningandOperatingCostConstantId);

            }
        }
        public class OperatorLicenseEntityTypeConfiguration : EntityTypeConfiguration<OperatorLicense>
        {

            public OperatorLicenseEntityTypeConfiguration()
            {
                this.ToTable("OperatorLicense", "FleetManagement");
                this.HasKey<int>(t => t.OperatorLicenseId);

            }
        }
        public class FleetConditionEntityTypeConfiguration : EntityTypeConfiguration<FleetCondition>
        {

            public FleetConditionEntityTypeConfiguration()
            {
                this.ToTable("FleetCondition", "FleetManagement");
                this.HasKey<int>(t => t.FleetConditionId);

            }
        }
        public class FleetPettyCashTypeEntityTypeConfiguration : EntityTypeConfiguration<FleetPettyCashType>
        {

            public FleetPettyCashTypeEntityTypeConfiguration()
            {
                this.ToTable("FleetPettyCashType", "FleetManagement");
                this.HasKey<int>(t => t.FleetPettyCashTypeId);

            }
        }

        public class OwningandOperatingCostEntityTypeConfiguration : EntityTypeConfiguration<OwningandOperatingCost>
        {

            public OwningandOperatingCostEntityTypeConfiguration()
            {
                this.ToTable("OwningandOperatingCost", "FleetManagement");
                this.HasKey<int>(t => t.OwningandOperatingCostId);

            }
        }
        public class SiteVechicleTransferTyreDetailEntityTypeConfiguration : EntityTypeConfiguration<SiteVechicleTransferTyreDetail>
        {

            public SiteVechicleTransferTyreDetailEntityTypeConfiguration()
            {
                this.ToTable("SiteVechicleTransferTyreDetail", "FleetManagement");
                this.HasKey<int>(t => t.SiteVechicleTransferTyreDetailId);

            }
        }
        public class ExternalRentedEquipmentEntityTypeConfiguration : EntityTypeConfiguration<ExternalRentedEquipment>
        {

            public ExternalRentedEquipmentEntityTypeConfiguration()
            {
                this.ToTable("ExternalRentedEquipment", "FleetManagement");
                this.HasKey<Guid>(t => t.ExternalRentedEquipmentId);

            }
        }
        public class FleetIFuelConsumptionEntityTypeConfiguration : EntityTypeConfiguration<FleetIFuelConsumption>
        {

            public FleetIFuelConsumptionEntityTypeConfiguration()
            {
                this.ToTable("FleetIFuelConsumption", "FleetManagement");
                this.HasKey<int>(t => t.FleetIFuelConsumptionId);

            }
        }
        //public class PMScheduleEntityTypeConfiguration : EntityTypeConfiguration<PMSchedule>
        //{

        //    public PMScheduleEntityTypeConfiguration()
        //    {
        //        this.ToTable("PMSchedule", "FleetManagement");
        //        this.HasKey<int>(t => t.PMScheduleId);

        //    }
        //}
        // public class DailyCareDetailItemEntityTypeConfiguration : EntityTypeConfiguration<DailyCareDetailItem>
        //{

        //    public DailyCareDetailItemEntityTypeConfiguration()
        //    {
        //        this.ToTable("DailyCareDetailItem", "FleetManagement");
        //        this.HasKey<Guid>(t => t.DailyCareDetailItemId);

        //    }
        //}
        

        public class FleetInsuranceTypeDetailEntityTypeConfiguration : EntityTypeConfiguration<FleetInsuranceTypeDetail>
        {

            public FleetInsuranceTypeDetailEntityTypeConfiguration()
            {
                this.ToTable("FleetInsuranceTypeDetail", "FleetManagement");
                this.HasKey<int>(t => t.FleetInsuranceTypeDetailId);

            }
        }
        //public class PMScheduleDetailEntityTypeConfiguration : EntityTypeConfiguration<PMScheduleDetail>
        //{

        //    public PMScheduleDetailEntityTypeConfiguration()
        //    {
        //        this.ToTable("PMScheduleDetail", "FleetManagement");
        //        this.HasKey<int>(t => t.PMScheduleDetailId);

        //    }
        //}
        //public class LogisticPlanEntityTypeConfiguration : EntityTypeConfiguration<LogisticPlan>
        //{

        //    public LogisticPlanEntityTypeConfiguration()
        //    {
        //        this.ToTable("LogisticPlan", "FleetManagement");
        //        this.HasKey<int>(t => t.LogisticPlanId);

        //    }
        //}
        //public class LogisticPlanDetailEntityTypeConfiguration : EntityTypeConfiguration<LogisticPlanDetail>
        //{

        //    public LogisticPlanDetailEntityTypeConfiguration()
        //    {
        //        this.ToTable("LogisticPlanDetail", "FleetManagement");
        //        this.HasKey<int>(t => t.LogisticPlanDetailId);

        //    }
        //}
        //public class ServiceCrewWorkLogEntityTypeConfiguration : EntityTypeConfiguration<ServiceCrewWorkLog>
        //{

        //    public ServiceCrewWorkLogEntityTypeConfiguration()
        //    {
        //        this.ToTable("ServiceCrewWorkLog", "FleetManagement");
        //        this.HasKey<Guid>(t => t.ServiceCrewWorkLogId);

        //    }
        //}
        //public class FailureReasonEntityTypeConfiguration : EntityTypeConfiguration<FailureReason>
        //{

        //    public FailureReasonEntityTypeConfiguration()
        //    {
        //        this.ToTable("FailureReason", "FleetManagement");
        //        this.HasKey<Guid>(t => t.FailureReasonId);

        //    }
        //}
        //public class DailyCareLogEntityTypeConfiguration : EntityTypeConfiguration<DailyCareLog>
        //{

        //    public DailyCareLogEntityTypeConfiguration()
        //    {
        //        this.ToTable("DailyCareLog", "FleetManagement");
        //        this.HasKey<Guid>(t => t.DailyCareLogId);

        //    }
        //}


        //public class DailyCareDetailLogEntityTypeConfiguration : EntityTypeConfiguration<DailyCareDetailLog>
        //{

        //    public DailyCareDetailLogEntityTypeConfiguration()
        //    {
        //        this.ToTable("DailyCareDetailLog", "FleetManagement");
        //        this.HasKey<Guid>(t => t.DailyCareDetailLogId);

        //    }
        //}
        //public class DailyCareCategoryEntityTypeConfiguration : EntityTypeConfiguration<DailyCareCategory>
        //{

        //    public DailyCareCategoryEntityTypeConfiguration()
        //    {
        //        this.ToTable("DailyCareCategory", "FleetManagement");
        //        this.HasKey<Guid>(t => t.DailyCareCategoryId);

        //    }
        //}
        //public class DailyCareEntityTypeConfiguration : EntityTypeConfiguration<DailyCare>
        //{

        //    public DailyCareEntityTypeConfiguration()
        //    {
        //        this.ToTable("DailyCare", "FleetManagement");
        //        this.HasKey<Guid>(t => t.DailyCareId);

        //    }
        //}
        //public class AdvanceMeasureEntityTypeConfiguration : EntityTypeConfiguration<AdvanceMeasure>
        //{

        //    public AdvanceMeasureEntityTypeConfiguration()
        //    {
        //        this.ToTable("AdvanceMeasure", "FleetManagement");
        //        this.HasKey<Guid>(t => t.AdvanceMeasureId);

        //    }
        //}


        public class FleetPettyCashEntityTypeConfiguration : EntityTypeConfiguration<FleetPettyCash>
        {

            public FleetPettyCashEntityTypeConfiguration()
            {
                this.ToTable("FleetPettyCash", "FleetManagement");
                this.HasKey<Guid>(t => t.FleetPettyCashId);

            }
        }
        public class CannibalizationDetailEntityTypeConfiguration : EntityTypeConfiguration<CannibalizationDetail>
        {

            public CannibalizationDetailEntityTypeConfiguration()
            {
                this.ToTable("CannibalizationDetail", "FleetManagement");
                this.HasKey<Guid>(t => t.CannibalizationDetailId);

            }
        }
        public class SiteVechicleTransferDetailEntityTypeConfiguration : EntityTypeConfiguration<SiteVechicleTransferDetail>
        {

            public SiteVechicleTransferDetailEntityTypeConfiguration()
            {
                this.ToTable("SiteVechicleTransferDetail", "FleetManagement");
                this.HasKey<int>(t => t.SiteVechicleTransferDetailId);

            }
        }
        public class BatteryIssueEntityTypeConfiguration : EntityTypeConfiguration<BatteryIssue>
        {

            public BatteryIssueEntityTypeConfiguration()
            {
                this.ToTable("BatteryIssue", "FleetManagement");
                this.HasKey<Guid>(t => t.BatteryIssueId);

            }
        }
        public class HRFleetMappingEntityTypeConfiguration : EntityTypeConfiguration<HRFleetMapping>
        {

            public HRFleetMappingEntityTypeConfiguration()
            {
                this.ToTable("HRFleetMapping", "FleetManagement");
                this.HasKey<Guid>(t => t.HRFleetMappingId);

            }
        }
        public class BatteryReturnEntityTypeConfiguration : EntityTypeConfiguration<BatteryReturn>
        {

            public BatteryReturnEntityTypeConfiguration()
            {
                this.ToTable("BatteryReturn", "FleetManagement");
                this.HasKey<Guid>(t => t.BatteryReturnId);

            }
        }
        public class BatteryApprovalEntityTypeConfiguration : EntityTypeConfiguration<BatteryApproval>
        {

            public BatteryApprovalEntityTypeConfiguration()
            {
                this.ToTable("BatteryApproval", "FleetManagement");
                this.HasKey<Guid>(t => t.BatteryApprovalId);

            }
        }
        //public class InventorySparePartMappingEntityTypeConfiguration : EntityTypeConfiguration<InventorySparePartMapping>
        //{

        //    public InventorySparePartMappingEntityTypeConfiguration()
        //    {
        //        this.ToTable("InventorySparePartMapping", "FleetManagement");
        //        this.HasKey<Guid>(t => t.InventorySparePartMappingId);

        //    }
        //}
        public class EquipmentArrivalApprovalEntityTypeConfiguration : EntityTypeConfiguration<EquipmentArrivalApproval>
        {

            public EquipmentArrivalApprovalEntityTypeConfiguration()
            {
                this.ToTable("EquipmentArrivalApproval", "FleetManagement");
                this.HasKey<Guid>(t => t.EquipmentArrivalApprovalId);

            }
        }
        //public class MaintenaceInspectionLogEntityTypeConfiguration : EntityTypeConfiguration<MaintenaceInspectionLog>
        //{

        //    public MaintenaceInspectionLogEntityTypeConfiguration()
        //    {
        //        this.ToTable("MaintenaceInspectionLog", "FleetManagement");
        //        this.HasKey<Guid>(t => t.MaintenaceInspectionLogId);

        //    }
        //}

        //public class ExternalComponentEntityTypeConfiguration : EntityTypeConfiguration<ExternalComponent>
        //{

        //    public ExternalComponentEntityTypeConfiguration()
        //    {
        //        this.ToTable("ExternalComponent", "FleetManagement");
        //        this.HasKey<Guid>(t => t.ExternalComponentId);

        //    }
        //}
        //public class ExternalComponentDetailEntityTypeConfiguration : EntityTypeConfiguration<ExternalComponentDetail>
        //{

        //    public ExternalComponentDetailEntityTypeConfiguration()
        //    {
        //        this.ToTable("ExternalComponentDetail", "FleetManagement");
        //        this.HasKey<Guid>(t => t.ExternalComponentDetailId);

        //    }
        //}
        public class EquipmentArrivalEntityTypeConfiguration : EntityTypeConfiguration<EquipmentArrival>
        {

            public EquipmentArrivalEntityTypeConfiguration()
            {
                this.ToTable("EquipmentArrival", "FleetManagement");
                this.HasKey<Guid>(t => t.EquipmentArrivalId);

            }
        }

        //public class FollowUpEntityTypeConfiguration : EntityTypeConfiguration<FollowUp>
        //{

        //    public FollowUpEntityTypeConfiguration()
        //    {
        //        this.ToTable("FollowUp", "FleetManagement");
        //        this.HasKey<Guid>(t => t.FollowUpId);

        //    }
        //}
        public class EquipmentTyreLogEntityTypeConfiguration : EntityTypeConfiguration<EquipmentTyreLog>
        {

            public EquipmentTyreLogEntityTypeConfiguration()
            {
                this.ToTable("EquipmentTyreLog", "FleetManagement");
                this.HasKey<Guid>(t => t.EquipmentTyreLogId);

            }
        }
        public class EquipmentBatteryEntityTypeConfiguration : EntityTypeConfiguration<EquipmentBattery>
        {

            public EquipmentBatteryEntityTypeConfiguration()
            {
                this.ToTable("EquipmentBattery", "FleetManagement");
                this.HasKey<Guid>(t => t.EquipmentBatteryId);

            }
        }
        public class EquipmentBatteryLogEntityTypeConfiguration : EntityTypeConfiguration<EquipmentBatteryLog>
        {

            public EquipmentBatteryLogEntityTypeConfiguration()
            {
                this.ToTable("EquipmentBatteryLog", "FleetManagement");
                this.HasKey<Guid>(t => t.EquipmentBatteryLogId);

            }
        }
        //public class MechanicOnHandEntityTypeConfiguration : EntityTypeConfiguration<MechanicOnHand>
        //{

        //    public MechanicOnHandEntityTypeConfiguration()
        //    {
        //        this.ToTable("MechanicOnHand", "FleetManagement");
        //        this.HasKey<Guid>(t => t.MechanicOnHandId);

        //    }
        //}

        //public class MaintenanceTypeEntityTypeConfiguration : EntityTypeConfiguration<MaintenanceType>
        //{

        //    public MaintenanceTypeEntityTypeConfiguration()
        //    {
        //        this.ToTable("MaintenanceType", "FleetManagement");
        //        this.HasKey<Guid>(t => t.MaintenanceTypeId);

        //    }
        //}
        //public class JobCausesEntityTypeConfiguration : EntityTypeConfiguration<JobCauses>
        //{

        //    public JobCausesEntityTypeConfiguration()
        //    {
        //        this.ToTable("JobCauses", "FleetManagement");
        //        this.HasKey<Guid>(t => t.JobCausesId);

        //    }
        //}

        //public class JobRequestEntityTypeConfiguration : EntityTypeConfiguration<JobRequest>
        //{

        //    public JobRequestEntityTypeConfiguration()
        //    {
        //        this.ToTable("JobRequest", "FleetManagement");
        //        this.HasKey<Guid>(t => t.JobRequestId);

        //    }
        //}
        //public class WorkUnitSectionEntityTypeConfiguration : EntityTypeConfiguration<WorkUnitSection>
        //{

        //    public WorkUnitSectionEntityTypeConfiguration()
        //    {
        //        this.ToTable("WorkUnitSection", "FleetManagement");
        //        this.HasKey<Guid>(t => t.WorkUnitSectionId);

        //    }
        //}
        public class FleetsEntityTypeConfiguration : EntityTypeConfiguration<Fleets>
        {

            public FleetsEntityTypeConfiguration()
            {
                this.ToTable("Fleets", "FleetManagement");
                this.HasKey<Guid>(t => t.FleetsID);

            }
        }

        //public class JobStoreRequisitionEntityTypeConfiguration : EntityTypeConfiguration<JobStoreRequisition>
        //{

        //    public JobStoreRequisitionEntityTypeConfiguration()
        //    {
        //        this.ToTable("JobStoreRequisition", "FleetManagement");
        //        this.HasKey<Guid>(t => t.JobStoreRequisitionId);

        //    }
        //}

        public class FuelConsumptionAlertLevelEntityTypeConfiguration : EntityTypeConfiguration<FuelConsumptionAlertLevel>
        {

            public FuelConsumptionAlertLevelEntityTypeConfiguration()
            {
                this.ToTable("FuelConsumptionAlertLevel", "FleetManagement");
                this.HasKey<Guid>(t => t.FuelConsumptionAlertLevelId);

            }
        }
        public class EquipmentTyreEntityTypeConfiguration : EntityTypeConfiguration<EquipmentTyre>
        {

            public EquipmentTyreEntityTypeConfiguration()
            {
                this.ToTable("EquipmentTyre", "FleetManagement");
                this.HasKey<Guid>(t => t.EquipmentTyreId);

            }
        }
        public class TyreTransferEntityTypeConfiguration : EntityTypeConfiguration<TyreTransfer>
        {

            public TyreTransferEntityTypeConfiguration()
            {
                this.ToTable("TyreTransfer", "FleetManagement");
                this.HasKey<Guid>(t => t.TyreTransferId);

            }
        }
        public class TyreTransferApprovalEntityTypeConfiguration : EntityTypeConfiguration<TyreTransferApproval>
        {

            public TyreTransferApprovalEntityTypeConfiguration()
            {
                this.ToTable("TyreTransferApproval", "FleetManagement");
                this.HasKey<int>(t => t.TyreTransferApprovalId);

            }
        }

        public class DriverAssignmentEntityTypeConfiguration : EntityTypeConfiguration<DriverAssignment>
        {

            public DriverAssignmentEntityTypeConfiguration()
            {
                this.ToTable("DriverAssignment", "FleetManagement");
                this.HasKey<Guid>(t => t.DriverAssignmentId);

            }
        }

        public class TimeSheetApprovalEntityTypeConfiguration : EntityTypeConfiguration<TimeSheetApproval>
        {

            public TimeSheetApprovalEntityTypeConfiguration()
            {
                this.ToTable("TimeSheetApproval", "FleetManagement");
                this.HasKey<int>(t => t.TimeSheetApprovalId);

            }

        }
        public class FuelTypeEntityTypeConfiguration : EntityTypeConfiguration<FuelType>
        {

            public FuelTypeEntityTypeConfiguration()
            {
                this.ToTable("FuelType", "FleetManagement");
                this.HasKey<Guid>(t => t.FuelTypeId);

            }
        }

        public class FuelInventoryEntityTypeConfiguration : EntityTypeConfiguration<FuelInventory>
        {

            public FuelInventoryEntityTypeConfiguration()
            {
                this.ToTable("FuelInventory", "FleetManagement");
                this.HasKey<Guid>(t => t.FuelInventoryId);

            }
        }


        public class LocationEntityTypeConfiguration : EntityTypeConfiguration<EquipmentLocation>
        {

            public LocationEntityTypeConfiguration()
            {
                this.ToTable("EquipmentLocation", "FleetManagement");
                this.HasKey<Guid>(t => t.EquipmentLocationId);

            }
        }
        public class EquipmentPlanEvaluationApprovalApprovalEntityTypeConfiguration : EntityTypeConfiguration<EquipmentPlanEvaluationApproval>
        {

            public EquipmentPlanEvaluationApprovalApprovalEntityTypeConfiguration()
            {
                this.ToTable("EquipmentPlanEvaluationApproval", "FleetManagement");
                this.HasKey<int>(t => t.EquipmentPlanEvaluationApprovalId);
              
            }
        }


        public class RequestedEquipmentEntityTypeConfiguration : EntityTypeConfiguration<RequestedEquipment>
        {

            public RequestedEquipmentEntityTypeConfiguration()
            {
                this.ToTable("RequestedEquipment", "FleetManagement");
                this.HasKey<Guid>(t => t.RequestedEquipmentId);

            }
        }
        public class AccessoriesEntityTypeConfiguration : EntityTypeConfiguration<Accessories>
        {

            public AccessoriesEntityTypeConfiguration()
            {
                this.ToTable("Accessories", "FleetManagement");
                this.HasKey<Guid>(t => t.AccessoriesID);

            }
        }

        public class CheckListEntityTypeConfiguration : EntityTypeConfiguration<CheckList>
        {

            public CheckListEntityTypeConfiguration()
            {
                this.ToTable("CheckList", "FleetManagement");
                this.HasKey<int>(t => t.CheckListId);

            }
        }
        public class FleetCheckListEntityTypeConfiguration : EntityTypeConfiguration<FleetCheckList>
        {

            public FleetCheckListEntityTypeConfiguration()
            {
                this.ToTable("FleetCheckList", "FleetManagement");
                this.HasKey<int>(t => t.FleetCheckListId);

            }
        }
        public class FleetLineCheckListListEntityTypeConfiguration : EntityTypeConfiguration<FleetLineCheckList>
        {

            public FleetLineCheckListListEntityTypeConfiguration()
            {
                this.ToTable("FleetLineCheckList", "FleetManagement");
                this.HasKey<int>(t => t.FleetLineCheckListId);

            }
        }
        public class CheckListLogEntityTypeConfiguration : EntityTypeConfiguration<CheckListLog>
        {

            public CheckListLogEntityTypeConfiguration()
            {
                this.ToTable("CheckListLog", "FleetManagement");
                this.HasKey<int>(t => t.CheckListLogId);

            }
        }
        /*   public class DisposalAndReplacementApprovalEntityTypeConfiguration : EntityTypeConfiguration<DisposalAndReplacementApproval>
           {

               public DisposalAndReplacementApprovalEntityTypeConfiguration()
               {
                   this.ToTable("DisposalAndReplacementApproval", "FleetManagement");
                   this.HasKey<int>(t => t.);

               }
           }*/
        public class DisposalandReplacementRecommendationEntityTypeConfiguration : EntityTypeConfiguration<DisposalandReplacementRecommendation>
        {

            public DisposalandReplacementRecommendationEntityTypeConfiguration()
            {
                this.ToTable("DisposalandReplacementRecommendation", "FleetManagement");
                this.HasKey<Guid>(t => t.DisposalandReplacementRecommendationID);

            }
        }
        public class InspectionApprovalEntityTypeConfiguration : EntityTypeConfiguration<InspectionApproval>
        {

            public InspectionApprovalEntityTypeConfiguration()
            {
                this.ToTable("InspectionApproval", "FleetManagement");
                this.HasKey<int>(t => t.InspectionApprovalId);

            }
        }
        public class InspectionLogEntityTypeConfiguration : EntityTypeConfiguration<InspectionLog>
        {

            public InspectionLogEntityTypeConfiguration()
            {
                this.ToTable("InspectionLog", "FleetManagement");
                this.HasKey<int>(t => t.InspectionLogId);

            }
        }

        public class LineCheckListEntityTypeConfiguration : EntityTypeConfiguration<LineCheckList>
        {

            public LineCheckListEntityTypeConfiguration()
            {
                this.ToTable("LineCheckList", "FleetManagement");
                this.HasKey<int>(t => t.LineCheckListId);

            }
        }


        public class EquipmentPlanandEvaluationsEntityTypeConfiguration : EntityTypeConfiguration<EquipmentPlanandEvaluations>
        {

            public EquipmentPlanandEvaluationsEntityTypeConfiguration()
            {
                this.ToTable("EquipmentPlanandEvaluations", "FleetManagement");
                this.HasKey<Guid>(t => t.EquipmentPlanandEvaluationsID);
            
            }
        }

        public class PlanAndEvaluationApprovalEntityTypeConfiguration : EntityTypeConfiguration<PlanAndEvaluationApproval>
        {

            public PlanAndEvaluationApprovalEntityTypeConfiguration()
            {
                this.ToTable("PlanAndEvaluationApproval", "FleetManagement");
                this.HasKey<int>(t => t.PlanAndEvaluationApprovalID);

            }
        }



        public class RentForInternalApprovalEntityTypeConfiguration : EntityTypeConfiguration<RentForInternalApproval>
        {

            public RentForInternalApprovalEntityTypeConfiguration()
            {
                this.ToTable("RentForInternalApproval", "FleetManagement");
                this.HasKey<int>(t => t.RentForInternalApprovalID);

            }
        }


        public class RentFromExternalEntityTypeConfiguration : EntityTypeConfiguration<RentFromExternal>
        {

            public RentFromExternalEntityTypeConfiguration()
            {
                this.ToTable("RentFromExternal", "FleetManagement");
                this.HasKey<Guid>(t => t.RentFromExternalID);

            }
        }


        public class RentFromExternalApprovalEntityTypeConfiguration : EntityTypeConfiguration<RentFromExternalApproval>
        {

            public RentFromExternalApprovalEntityTypeConfiguration()
            {
                this.ToTable("RentFromExternalApproval", "FleetManagement");
                this.HasKey<int>(t => t.RentFromExternalApprovalID);

            }
        }

        public class RentInternalProjectEntityTypeConfiguration : EntityTypeConfiguration<RentInternalProject>
        {

            public RentInternalProjectEntityTypeConfiguration()
            {
                this.ToTable("RentInternalProject", "FleetManagement");
                this.HasKey<Guid>(t => t.RentInternalProjectID);

            }
        }


        public class RentOtherSuppliersEntityTypeConfiguration : EntityTypeConfiguration<RentToExternal>
        {

            public RentOtherSuppliersEntityTypeConfiguration()
            {
                this.ToTable("RentToExternals", "FleetManagement");
                this.HasKey<Guid>(t => t.RentToExternalID);

            }
        }
        public class RentOtherSupplierApprovalEntityTypeConfiguration : EntityTypeConfiguration<RentToExternalApproval>
        {

            public RentOtherSupplierApprovalEntityTypeConfiguration()
            {
                this.ToTable("RentToExternalApproval", "FleetManagement");
                this.HasKey<int>(t => t.RentToExternalApprovalID);

            }
        }

        public class RentRequestEntityTypeConfiguration : EntityTypeConfiguration<RentRequest>
        {

            public RentRequestEntityTypeConfiguration()
            {
                this.ToTable("RentRequest", "FleetManagement");
                this.HasKey<Guid>(t => t.RentRequestID);

            }
        }
        public class RentRequestApprovalEntityTypeConfiguration : EntityTypeConfiguration<RentRequestApproval>
        {

            public RentRequestApprovalEntityTypeConfiguration()
            {
                this.ToTable("RentRequestApproval", "FleetManagement");
                this.HasKey<int>(t => t.RentRequestApprovalID);

            }
        }

        //public class AdministrativeJobApprovalEntityTypeConfiguration : EntityTypeConfiguration<AdministrativeJobApproval>
        //{

        //    public AdministrativeJobApprovalEntityTypeConfiguration()
        //    {
        //        this.ToTable("AdministrativeJobApproval", "FleetManagement");
        //        this.HasKey<int>(t => t.AdministrativeJobApprovalID);

        //    }
        //}

        //public class ExternalCrewEntityTypeConfiguration : EntityTypeConfiguration<ExternalCrew>
        //{

        //    public ExternalCrewEntityTypeConfiguration()
        //    {
        //        this.ToTable("ExternalCrew", "FleetManagement");
        //        this.HasKey<Guid>(t => t.ExternalCrewID);

        //    }
        //}

        //public class ExternalMaterialEntityTypeConfiguration : EntityTypeConfiguration</*ExternalMaterial*/>
        //{

        //    public ExternalMaterialEntityTypeConfiguration()
        //    {
        //        this.ToTable("ExternalMaterial", "FleetManagement");
        //        this.HasKey<Guid>(t => t.ExternalMaterialID);

        //    }
        //}
        //public class JobOrderEntityTypeConfiguration : EntityTypeConfiguration<JobOrder>
        //{

        //    public JobOrderEntityTypeConfiguration()
        //    {
        //        this.ToTable("JobOrder", "FleetManagement");
        //        this.HasKey<Guid>(t => t.JobOrderID);

        //    }
        //}
        //public class JobOrderDetailEntityTypeConfiguration : EntityTypeConfiguration<JobOrderDetail>
        //{

        //    public JobOrderDetailEntityTypeConfiguration()
        //    {
        //        this.ToTable("JobOrderDetail", "FleetManagement");
        //        this.HasKey<Guid>(t => t.JobOrderDetailID);

        //    }
        //}


        //public class MaintenanceCenterEntityTypeConfiguration : EntityTypeConfiguration<MaintenanceCenter>
        //{

        //    public MaintenanceCenterEntityTypeConfiguration()
        //    {
        //        this.ToTable("MaintenanceCenter", "FleetManagement");
        //        this.HasKey<Guid>(t => t.MaintenanceCenterID);

        //    }
        //}
        //public class MaintenanceToolEntityTypeConfiguration : EntityTypeConfiguration<MaintenanceTool>
        //{

        //    public MaintenanceToolEntityTypeConfiguration()
        //    {
        //        this.ToTable("MaintenanceTool", "FleetManagement");
        //        this.HasKey<Guid>(t => t.MaintenanceToolID);

        //    }
        //}

        //public class MechanicCrewEntityTypeConfiguration : EntityTypeConfiguration<MechanicCrew>
        //{

        //    public MechanicCrewEntityTypeConfiguration()
        //    {
        //        this.ToTable("MechanicCrew", "FleetManagement");
        //        this.HasKey<Guid>(t => t.MechanicCrewID);

        //    }
        //}

        //public class OtherCompanyMaintenaceApprovalEntityTypeConfiguration : EntityTypeConfiguration<OtherCompanyMaintenaceApproval>
        //{

        //    public OtherCompanyMaintenaceApprovalEntityTypeConfiguration()
        //    {
        //        this.ToTable("OtherCompanyMaintenaceApproval", "FleetManagement");
        //        this.HasKey<int>(t => t.OtherCompanyMaintenaceApprovalID);

        //    }
        //}



        //public class OtherCompanyMaintenanceEntityTypeConfiguration : EntityTypeConfiguration<OtherCompanyMaintenance>
        //{

        //    public OtherCompanyMaintenanceEntityTypeConfiguration()
        //    {
        //        this.ToTable("OtherCompanyMaintenance", "FleetManagement");
        //        this.HasKey<Guid>(t => t.OtherCompanyMaintenanceID);

        //    }
        //}


        //public class OutsourceMaintenanceEntityTypeConfiguration : EntityTypeConfiguration<OutsourceMaintenance>
        //{

        //    public OutsourceMaintenanceEntityTypeConfiguration()
        //    {
        //        this.ToTable("OutsourceMaintenance", "FleetManagement");
        //        this.HasKey<Guid>(t => t.OutsourceMaintenanceID);

        //    }
        //}


        //public class OutsourceMaintenanceApprovalEntityTypeConfiguration : EntityTypeConfiguration<OutsourceMaintenanceApproval>
        //{

        //    public OutsourceMaintenanceApprovalEntityTypeConfiguration()
        //    {
        //        this.ToTable("OutsourceMaintenanceApproval", "FleetManagement");
        //        this.HasKey<int>(t => t.OutsourceMaintenanceApprovalId);

        //    }
        //}
        public class AnnualEquipmentInventoryInspectionEntityTypeConfiguration : EntityTypeConfiguration<AnnualEquipmentInventoryInspection>
        {

            public AnnualEquipmentInventoryInspectionEntityTypeConfiguration()
            {
                this.ToTable("AnnualEquipmentInventoryInspection", "FleetManagement");
                this.HasKey<Guid>(t => t.AnnualEquipmentInventoryInspectionID);

            }
        }
        public class AnnualInventoryInspectionApprovalEntityTypeConfiguration : EntityTypeConfiguration<AnnualInventoryInspectionApproval>
        {

            public AnnualInventoryInspectionApprovalEntityTypeConfiguration()
            {
                this.ToTable("AnnualInventoryInspectionApproval", "FleetManagement");
                this.HasKey<int>(t => t.AnnualInventoryInspectionApprovalID);

            }
        }

        //public class PlannerEntityTypeConfiguration : EntityTypeConfiguration<Planner>
        //{

        //    public PlannerEntityTypeConfiguration()
        //    {
        //        this.ToTable("Planner", "FleetManagement");
        //        this.HasKey<Guid>(t => t.PlannerID);

        //    }
        //}

        //public class RepairTypeEntityTypeConfiguration : EntityTypeConfiguration<RepairType>
        //{

        //    public RepairTypeEntityTypeConfiguration()
        //    {
        //        this.ToTable("RepairType", "FleetManagement");
        //        this.HasKey<Guid>(t => t.RepairTypeID);

        //    }
        //}
        //public class ServiceEntityTypeConfiguration : EntityTypeConfiguration<Service>
        //{

        //    public ServiceEntityTypeConfiguration()
        //    {
        //        this.ToTable("Service", "FleetManagement");
        //        this.HasKey<Guid>(t => t.ServiceID);

        //    }
        //}
        //public class ServiceCrewEntityTypeConfiguration : EntityTypeConfiguration<ServiceCrew>
        //{

        //    public ServiceCrewEntityTypeConfiguration()
        //    {
        //        this.ToTable("ServiceCrew", "FleetManagement");
        //        this.HasKey<Guid>(t => t.ServiceCrewID);

        //    }
        //}
        //public class ServiceSpareEntityTypeConfiguration : EntityTypeConfiguration<ServiceSpare>
        //{

        //    public ServiceSpareEntityTypeConfiguration()
        //    {
        //        this.ToTable("ServiceSpare", "FleetManagement");
        //        this.HasKey<Guid>(t => t.ServiceSpareID);

        //    }
        //}
        //public class ServiceToolEntityTypeConfiguration : EntityTypeConfiguration<ServiceTool>
        //{

        //    public ServiceToolEntityTypeConfiguration()
        //    {
        //        this.ToTable("ServiceTool", "FleetManagement");
        //        this.HasKey<Guid>(t => t.ServiceToolID);

        //    }
        //}
        //public class SparePartEntityTypeConfiguration : EntityTypeConfiguration<SparePart>
        //{

        //    public SparePartEntityTypeConfiguration()
        //    {
        //        this.ToTable("SparePart", "FleetManagement");
        //        this.HasKey<Guid>(t => t.SparePartID);

        //    }
        //}

        //public class TechnicalJobApprovalEntityTypeConfiguration : EntityTypeConfiguration<TechnicalJobApproval>
        //{

        //    public TechnicalJobApprovalEntityTypeConfiguration()
        //    {
        //        this.ToTable("TechnicalJobApproval", "FleetManagement");
        //        this.HasKey<int>(t => t.TechnicalJobApprovalID);

        //    }
        //}

        public class AccidentEntityTypeConfiguration : EntityTypeConfiguration<Accident>
        {

            public AccidentEntityTypeConfiguration()
            {
                this.ToTable("Accident", "FleetManagement");
                this.HasKey<Guid>(t => t.AccidentID);

            }
        }


        public class EducationallevelEntityTypeConfiguration : EntityTypeConfiguration<Educationallevel>
        {

            public EducationallevelEntityTypeConfiguration()
            {
                this.ToTable("Educationallevel", "FleetManagement");
                this.HasKey<Guid>(t => t.EducationallevelID);

            }
        }
        public class EquipmentApprovalEntityTypeConfiguration : EntityTypeConfiguration<EquipmentApproval>
        {

            public EquipmentApprovalEntityTypeConfiguration()
            {
                this.ToTable("EquipmentApproval", "FleetManagement");
                this.HasKey<int>(t => t.EquipmentApprovalId);

            }
        }
        public class EquipmentAttachmentEntityTypeConfiguration : EntityTypeConfiguration<EquipmentAttachment>
        {

            public EquipmentAttachmentEntityTypeConfiguration()
            {
                this.ToTable("EquipmentAttachment", "FleetManagement");
                this.HasKey<int>(t => t.EquipmentAttachmentID);

            }
        }

        public class EquipmentIdentityEntityTypeConfiguration : EntityTypeConfiguration<EquipmentIdentity>
        {

            public EquipmentIdentityEntityTypeConfiguration()
            {
                this.ToTable("EquipmentIdentity", "FleetManagement");
                this.HasKey<Guid>(t => t.EquipmentIdentityID);

            }
        }

        public class EquipmentNameEntityTypeConfiguration : EntityTypeConfiguration<EquipmentName>
        {

            public EquipmentNameEntityTypeConfiguration()
            {
                this.ToTable("EquipmentName", "FleetManagement");
                this.HasKey<Guid>(t => t.EquipmentNameID);

            }
        }


        public class EquipmentTypeEntityTypeConfiguration : EntityTypeConfiguration<EquipmentType>
        {

            public EquipmentTypeEntityTypeConfiguration()
            {
                this.ToTable("EquipmentType", "FleetManagement");
                this.HasKey<Guid>(t => t.EquipmentTypeID);
                this.HasRequired<EquipmentIdentity>(x => x.EquipmentIdentity).WithMany().WillCascadeOnDelete(false);

            }
        }
        public class FleetHistoryEntityTypeConfiguration : EntityTypeConfiguration<FleetHistory>
        {

            public FleetHistoryEntityTypeConfiguration()
            {
                this.ToTable("FleetHistory", "FleetManagement");
                this.HasKey<Guid>(t => t.FleetHistoryID);

            }
        }



        public class FleetInsuranceEntityTypeConfiguration : EntityTypeConfiguration<FleetInsurance>
        {

            public FleetInsuranceEntityTypeConfiguration()
            {
                this.ToTable("FleetInsurances", "FleetManagement");
                this.HasKey<int>(t => t.FleetInsuranceId);

            }
        }
        public class InsuranceCompanyEntityTypeConfiguration : EntityTypeConfiguration<InsuranceCompany>
        {

            public InsuranceCompanyEntityTypeConfiguration()
            {
                this.ToTable("InsuranceCompany", "FleetManagement");
                this.HasKey<int>(t => t.InsuranceCompanyId);

            }
        }

        public class ManufacturerEntityTypeConfiguration : EntityTypeConfiguration<Manufacturer>
        {

            public ManufacturerEntityTypeConfiguration()
            {
                this.ToTable("Manufacturer", "FleetManagement");
                this.HasKey<Guid>(t => t.ManufacturerID);

            }
        }
        public class OperatorEntityTypeConfiguration : EntityTypeConfiguration<Operator>
        {

            public OperatorEntityTypeConfiguration()
            {
                this.ToTable("Operator", "FleetManagement");
                this.HasKey<Guid>(t => t.OperatorID);

            }
        }
        public class OperatorPositionEntityTypeConfiguration : EntityTypeConfiguration<OperatorPosition>
        {

            public OperatorPositionEntityTypeConfiguration()
            {
                this.ToTable("OperatorPosition", "FleetManagement");
                this.HasKey<Guid>(t => t.OperatorPositionID);

            }
        }
        public class TimeSheetEntityTypeConfiguration : EntityTypeConfiguration<TimeSheet>
        {

            public TimeSheetEntityTypeConfiguration()
            {
                this.ToTable("TimeSheet", "FleetManagement");
                this.HasKey<Guid>(t => t.TimeSheetID);

            }
        }

        public class AssignedEquipmentEntityTypeConfiguration : EntityTypeConfiguration<AssignedEquipment>
        {

            public AssignedEquipmentEntityTypeConfiguration()
            {
                this.ToTable("AssignedEquipment", "FleetManagement");
                this.HasKey<Guid>(t => t.AssignedEquipmentID);

            }
        }



        public class AssignEquipmentApprovalEntityTypeConfiguration : EntityTypeConfiguration<AssignEquipmentApproval>
        {

            public AssignEquipmentApprovalEntityTypeConfiguration()
            {
                this.ToTable("AssignEquipmentApproval", "FleetManagement");
                this.HasKey<int>(t => t.AssignEquipmentApprovalID);

            }
        }


        public class DriverTransferApprovalEntityTypeConfiguration : EntityTypeConfiguration<DriverTransferApproval>
        {

            public DriverTransferApprovalEntityTypeConfiguration()
            {
                this.ToTable("DriverTransferApproval", "FleetManagement");
                this.HasKey<int>(t => t.DriverTransferApprovalID);

            }
        }

        public class DriverVehicleTransferEntityTypeConfiguration : EntityTypeConfiguration<DriverVehicleTransfer>
        {

            public DriverVehicleTransferEntityTypeConfiguration()
            {
                this.ToTable("DriverVehicleTransfer", "FleetManagement");
                this.HasKey<Guid>(t => t.DriverVehicleTransferID);

            }
        }

        public class SitesVehicleTransferEntityTypeConfiguration : EntityTypeConfiguration<SitesVehicleTransfer>
        {

            public SitesVehicleTransferEntityTypeConfiguration()
            {
                this.ToTable("SitesVehicleTransfer", "FleetManagement");
                this.HasKey<Guid>(t => t.SitesVehicleTransferID);

            }
        }


        public class SiteVehicleApprovalEntityTypeConfiguration : EntityTypeConfiguration<SiteVehicleApproval>
        {

            public SiteVehicleApprovalEntityTypeConfiguration()
            {
                this.ToTable("SiteVehicleApproval", "FleetManagement");
                this.HasKey<int>(t => t.SiteVehicleApprovalID);

            }
        }

        public class VehicleDemandEntityTypeConfiguration : EntityTypeConfiguration<VehicleDemand>
        {

            public VehicleDemandEntityTypeConfiguration()
            {
                this.ToTable("VehicleDemand", "FleetManagement");
                this.HasKey<Guid>(t => t.VehicleDemandId);

            }
        }



        public class VehicleDemandApprovalEntityTypeConfiguration : EntityTypeConfiguration<VehicleDemandApproval>
        {

            public VehicleDemandApprovalEntityTypeConfiguration()
            {
                this.ToTable("VehicleDemandApproval", "FleetManagement");
                this.HasKey<int>(t => t.VehicleDemandApprovalID);

            }
        }


        public class VehicleDistributionEntityTypeConfiguration : EntityTypeConfiguration<VehicleDistribution>
        {

            public VehicleDistributionEntityTypeConfiguration()
            {
                this.ToTable("VehicleDistribution", "FleetManagement");
                this.HasKey<Guid>(t => t.VehicleDistributionID);

            }
        }


        public class VehicleReturnEntityTypeConfiguration : EntityTypeConfiguration<VehicleReturn>
        {

            public VehicleReturnEntityTypeConfiguration()
            {
                this.ToTable("VehicleReturn", "FleetManagement");
                this.HasKey<Guid>(t => t.VehicleReturnID);

            }
        }
        public class VehicleReturnApprovalEntityTypeConfiguration : EntityTypeConfiguration<VehicleReturnApproval>
        {

            public VehicleReturnApprovalEntityTypeConfiguration()
            {
                this.ToTable("VehicleReturnApproval", "FleetManagement");
                this.HasKey<int>(t => t.VehicleReturnApprovalID);

            }
        }

        public class VehicleDistributionApprovalEntityTypeConfiguration : EntityTypeConfiguration<VehicleDistributionApproval>
        {

            public VehicleDistributionApprovalEntityTypeConfiguration()
            {
                this.ToTable("VehicleDistributionApproval", "FleetManagement");
                this.HasKey<int>(t => t.VehicleDistributionApprovalID);

            }
        }
        public class CannibalizationEntityTypeConfiguration : EntityTypeConfiguration<Cannibalization>
        {

            public CannibalizationEntityTypeConfiguration()
            {
                this.ToTable("Cannibalization", "FleetManagement");
                this.HasKey<Guid>(t => t.CannibalizationID);

            }
        }


        public class CannibalizationApprovalEntityTypeConfiguration : EntityTypeConfiguration<CannibalizationApproval>
        {

            public CannibalizationApprovalEntityTypeConfiguration()
            {
                this.ToTable("CannibalizationApproval", "FleetManagement");
                this.HasKey<int>(t => t.CannibalizationApprovalID);

            }
        }
        public class GoodRecivingNoteApprovalEntityTypeConfiguration : EntityTypeConfiguration<GoodRecivingNoteApproval>
        {

            public GoodRecivingNoteApprovalEntityTypeConfiguration()
            {
                this.ToTable("GoodRecivingNoteApproval", "FleetManagement");
                this.HasKey<int>(t => t.GoodRecivingNoteApprovalID);

            }
        }

        public class SalvageStoreEntityTypeConfiguration : EntityTypeConfiguration<SalvageStore>
        {

            public SalvageStoreEntityTypeConfiguration()
            {
                this.ToTable("SalvageStore", "FleetManagement");
                this.HasKey<Guid>(t => t.SalvageStoreID);

            }
        }

        public class SalvageStoreApprovalEntityTypeConfiguration : EntityTypeConfiguration<SalvageStoreApproval>
        {

            public SalvageStoreApprovalEntityTypeConfiguration()
            {
                this.ToTable("SalvageStoreApproval", "FleetManagement");
                this.HasKey<int>(t => t.SalvageStoreApprovalID);

            }
        }
        public class SalvageStoreGoodReceivingEntityTypeConfiguration : EntityTypeConfiguration<SalvageStoreGoodReceiving>
        {

            public SalvageStoreGoodReceivingEntityTypeConfiguration()
            {
                this.ToTable("SalvageStoreGoodReceiving", "FleetManagement");
                this.HasKey<Guid>(t => t.SalvageStoreGoodReceivingID);

            }
        }

        public class TireIssueEntityTypeConfiguration : EntityTypeConfiguration<TireIssue>
        {

            public TireIssueEntityTypeConfiguration()
            {
                this.ToTable("TireIssue", "FleetManagement");
                this.HasKey<Guid>(t => t.TireIssueID);

            }
        }

        public class TireReturnEntityTypeConfiguration : EntityTypeConfiguration<TireReturn>
        {

            public TireReturnEntityTypeConfiguration()
            {
                this.ToTable("TireReturn", "FleetManagement");
                this.HasKey<Guid?>(t => t.TireReturnID);

            }
        }

        public class TireReturnApprovalEntityTypeConfiguration : EntityTypeConfiguration<TireReturnApproval>
        {

            public TireReturnApprovalEntityTypeConfiguration()
            {
                this.ToTable("TireReturnApproval", "FleetManagement");
                this.HasKey<int>(t => t.TireReturnApprovalID);

            }
        }


        public class FuelEntityTypeConfiguration : EntityTypeConfiguration<Fuel>
        {

            public FuelEntityTypeConfiguration()
            {
                this.ToTable("Fuel", "FleetManagement");
                this.HasKey<Guid>(t => t.FuelID);

            }
        }

        public class FuelIssueApprovalEntityTypeConfiguration : EntityTypeConfiguration<FuelIssueApproval>
        {

            public FuelIssueApprovalEntityTypeConfiguration()
            {
                this.ToTable("FuelIssueApproval", "FleetManagement");
                this.HasKey<int>(t => t.FuelIssueApprovalID);

            }
        }



        public class TireIssueApprovalEntityTypeConfiguration : EntityTypeConfiguration<TireIssueApproval>
        {

            public TireIssueApprovalEntityTypeConfiguration()
            {
                this.ToTable("TireIssueApproval", "FleetManagement");
                this.HasKey<int>(t => t.TireIssueApprovalID);

            }
        }
        public class BoloRegistrationDocumentEntityTypeConfiguration : EntityTypeConfiguration<BoloRegistrationDocument>
        {

            public BoloRegistrationDocumentEntityTypeConfiguration()
            {
                this.ToTable("BoloRegistrationDocuments", "FleetManagement");
                this.HasKey<int>(t => t.BoloRegistrationDocumentId);

            }
        }
        public class FleetBoloRegistrationHistoryEntityTypeConfiguration : EntityTypeConfiguration<FleetBoloRegistrationHistory>
        {
            
            public FleetBoloRegistrationHistoryEntityTypeConfiguration()
            {
                this.ToTable("FleetBoloRegistrationsHistorys", "FleetManagement");
                this.HasKey<int>(t => t.FleetBoloRegistrationHistoryId);

            }
        }     public class FleetInsuranceHistoryEntityTypeConfiguration : EntityTypeConfiguration<FleetInsuranceHistory>
        {
            
            public FleetInsuranceHistoryEntityTypeConfiguration()
            {
                this.ToTable("FleetInsuranceHistorys", "FleetManagement");
                this.HasKey<int>(t => t.FleetInsuranceHistoryId);

            }
        }
    }
}



