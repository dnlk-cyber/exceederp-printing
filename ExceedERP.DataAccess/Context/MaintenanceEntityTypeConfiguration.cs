using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Core.Domain.Maintenance.Settings;
using ExceedERP.Core.Domain.Maintenance.SpareStoreRequisition;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.DataAccess.Context
{
    public class MaintenanceCompanyDocumentTypeEntityTypeConfiguration : EntityTypeConfiguration<MaintenanceCompanyDocumentType>
    {
        public MaintenanceCompanyDocumentTypeEntityTypeConfiguration()
        {
            this.ToTable("MaintenanceCompanyDocumentType", "Maintenance");
            this.HasKey<int>(p => p.MaintenanceCompanyDocumentTypeId);
        }
    }
    public class SatisfactionNoteEntityTypeConfiguration : EntityTypeConfiguration<SatisfactionNote>
    {
        public SatisfactionNoteEntityTypeConfiguration()
        {
            this.ToTable("SatisfactionNote", "Maintenance");
            this.HasKey<int>(p => p.SatisfactionNoteId);
        }
    }
    public class ExternalServiceCenterDetailEntityTypeConfiguration : EntityTypeConfiguration<ExternalServiceCenterDetail>
    {
        public ExternalServiceCenterDetailEntityTypeConfiguration()
        {
            this.ToTable("ExternalServiceCenterDetail", "Maintenance");
            this.HasKey<int>(p => p.ExternalServiceCenterDetailId);
        }
    }
    public class ExternalMaintenancePaymentRequestValidationEntityTypeConfiguration : EntityTypeConfiguration<ExternalMaintenancePaymentRequestValidation>
    {
        public ExternalMaintenancePaymentRequestValidationEntityTypeConfiguration()
        {
            this.ToTable("ExternalMaintenancePaymentRequestValidations", "Maintenance");
            this.HasKey<int>(p => p.ExternalMaintenancePaymentRequestValidationId);
        }
    }
    public class AccesseryInfoEntityTypeConfiguration : EntityTypeConfiguration<AccesseryInfo>
    {
        public AccesseryInfoEntityTypeConfiguration()
        {
            this.ToTable("AccesseryInfo", "Maintenance");
            this.HasKey<int>(p => p.AccesseryInfoId);
        }
    }
    public class TyreSerialInfoEntityTypeConfiguration : EntityTypeConfiguration<TyreSerialInfo>
    {
        public TyreSerialInfoEntityTypeConfiguration()
        {
            this.ToTable("TyreSerialInfo", "Maintenance");
            this.HasKey<int>(p => p.TyreSerialInfoId);
        }
    }
    public class MaintenanceStageEntityTypeConfiguration : EntityTypeConfiguration<MaintenanceStage>
    {
        public MaintenanceStageEntityTypeConfiguration()
        {
            this.ToTable("MaintenanceStage", "Maintenance");
            this.HasKey<int>(p => p.MaintenanceStageId);
        }
    }
    public class DescriptionOfAchievementEntityTypeConfiguration : EntityTypeConfiguration<DescriptionOfAchievement>
    {
        public DescriptionOfAchievementEntityTypeConfiguration()
        {
            this.ToTable("DescriptionOfAchievement", "Maintenance");
            this.HasKey<int>(p => p.DescrtiptionOfAchievementId);
        }
    }
    public class ExternalMaintenancePaymentRequestDepValidationEntityTypeConfiguration : EntityTypeConfiguration<ExternalMaintenancePaymentRequestDepValidation>
    {
        public ExternalMaintenancePaymentRequestDepValidationEntityTypeConfiguration()
        {
            this.ToTable("ExternalMaintenancePaymentRequestDepValidations", "Maintenance");
            this.HasKey<int>(p => p.ExternalMaintenancePaymentRequestDepValidationId);
        }
    }   
    public class ExternalPaymentRequestJobEntityTypeConfiguration : EntityTypeConfiguration<ExternalPaymentRequestJob>
    {
        public ExternalPaymentRequestJobEntityTypeConfiguration()
        {
            this.ToTable("ExternalMaintenancePaymentRequestHeaders", "Maintenance");
            this.HasKey<int>(p => p.ExternalPaymentRequestJobId);
        }
    }
    public class InternalJobToExternalEntityTypeConfiguration : EntityTypeConfiguration<InternalJobToExternal>
    {
        public InternalJobToExternalEntityTypeConfiguration()
        {
            this.ToTable("InternalJobToExternal", "Maintenance");
            this.HasKey<int>(p => p.InternalJobToExternalId);
        }
    }
    public class InternalJobToExternalValidationEntityTypeConfiguration : EntityTypeConfiguration<InternalJobToExternalValidation>
    {
        public InternalJobToExternalValidationEntityTypeConfiguration()
        {
            this.ToTable("InternalJobToExternalValidation", "Maintenance");
            this.HasKey<int>(p => p.InternalJobToExternalValidationId);
        }
    }
    public class VehicleReceptionApprovalEntityTypeConfiguration : EntityTypeConfiguration<VehicleReceptionApproval>
    {
        public VehicleReceptionApprovalEntityTypeConfiguration()
        {
            this.ToTable("VehicleReceptionApprovals", "Maintenance");
            this.HasKey<int>(p => p.VehicleReceptionApprovalId);
        }
    }
   
    public class CanibalizationDepartmentApprovalEntityTypeConfiguration : EntityTypeConfiguration<CanibalizationDepartmentApproval>
    {
        public CanibalizationDepartmentApprovalEntityTypeConfiguration()
        {
            this.ToTable("CanibalizationDepartmentApproval", "Maintenance");
            this.HasKey<int>(p => p.CanibalizationDepartmentApprovalId);
        }
    }
    public class PmSchedulerEntityTypeConfiguration : EntityTypeConfiguration<PmScheduler>
    {
        public PmSchedulerEntityTypeConfiguration()
        {
            this.ToTable("PmSchedulers", "Maintenance");
            this.HasKey<int>(p => p.PmSchedulerId);
        }
    }
    public class ServiceGroupSettingEntityTypeConfiguration : EntityTypeConfiguration<ServiceGroupSetting>
    {
        public ServiceGroupSettingEntityTypeConfiguration()
        {
            this.ToTable("ServiceGroupSettings", "Maintenance");
            this.HasKey<int>(p => p.ServiceGroupSettingId);
        }
    }
    public class AdministrativeJobDepartmentApprovalEntityTypeConfiguration : EntityTypeConfiguration<AdministrativeJobDepartmentApproval>
    {
        public AdministrativeJobDepartmentApprovalEntityTypeConfiguration()
        {
            this.ToTable("AdministrativeJobDepartmentApproval", "Maintenance");
            this.HasKey<int>(p => p.AdministrativeJobDepartmentApprovalID);
        }
    }
    public class SOPDocumentEntityTypeConfiguration : EntityTypeConfiguration<SOPDocument>
    {
        public SOPDocumentEntityTypeConfiguration()
        {
            this.ToTable("SOPDocuments", "Maintenance");
            this.HasKey<int>(p => p.SOPDocumentId);
        }
    }
    public class PmSettingEntityTypeConfiguration : EntityTypeConfiguration<PmSetting>
    {
        public PmSettingEntityTypeConfiguration()
        {
            this.ToTable("PmSetting", "Maintenance");
            this.HasKey<int>(p => p.PmSettingId);
        }
    }
    public class CloseWorkShopWithRemarkEntityTypeConfiguration : EntityTypeConfiguration<CloseWorkShopWithRemark>
    {
        public CloseWorkShopWithRemarkEntityTypeConfiguration()
        {
            this.ToTable("CloseWorkShopWithRemarks", "Maintenance");
            this.HasKey<int>(p => p.CloseWorkShopWithRemarkId);
        }
    }
    public class CanibalizationEntityTypeConfiguration : EntityTypeConfiguration<Canibalization>
    {
        public CanibalizationEntityTypeConfiguration()
        {
            this.ToTable("Canibalizations", "Maintenance");
            this.HasKey<int>(p => p.CanibalizationId);
        }
    }
    public class CanibalizedPartsEntityTypeConfiguration : EntityTypeConfiguration<CanibalizedParts>
    {
        public CanibalizedPartsEntityTypeConfiguration()
        {
            this.ToTable("CanibalizedParts", "Maintenance");
            this.HasKey<int>(p => p.CanibalizedPartsId);
        }
    }
    public class CanibalizationApprovalEntityTypeConfiguration : EntityTypeConfiguration<CanibalizationApproval>
    {
        public CanibalizationApprovalEntityTypeConfiguration()
        {
            this.ToTable("CanibalizationApprovals", "Maintenance");
            this.HasKey<int>(p => p.CanibalizationApprovalId);
        }
    }
    public class MaintenanceCompanyDocumentEntityTypeConfiguration : EntityTypeConfiguration<MaintenanceCompanyDocument>
    {
        public MaintenanceCompanyDocumentEntityTypeConfiguration() 
        {
            this.ToTable("MaintenanceCompanyDocument", "Maintenance");
            this.HasKey<int>(p => p.MaintenanceCompanyDocumentId);
        }
    }  
    
    public class ExternalMaintenancePaymentRequestEntityTypeConfiguration : EntityTypeConfiguration<ExternalMaintenancePaymentRequest>
    {
        public ExternalMaintenancePaymentRequestEntityTypeConfiguration()
        {
            this.ToTable("ExternalMaintenancePaymentRequest", "Maintenance");
            this.HasKey<int>(p => p.ExternalMaintenancePaymenntrequestId);
        }
    }
    public class ExternalMaintenancePaymentRequestDetailEntityTypeConfiguration : EntityTypeConfiguration<ExternalMaintenancePaymentRequestDetail>
    {
        public ExternalMaintenancePaymentRequestDetailEntityTypeConfiguration()
        {
            this.ToTable("ExternalMaintenancePaymentRequestDetail", "Maintenance");
            this.HasKey<int>(p => p.ExternalMaintenancePaymentRequestDetailId);
        }
    }

    public class MaintenanceDailyPlanEntityTypeConfiguration : EntityTypeConfiguration<MaintenanceDailyPlan>
    {
        public MaintenanceDailyPlanEntityTypeConfiguration()
        {
            this.ToTable("MaintenanceDailyPlan", "Maintenance");
            this.HasKey<int>(p => p.MaintenanceDailyPlanId);
        }
    }
    public class MaintenancePlanEntityTypeConfiguration : EntityTypeConfiguration<MaintenancePlan>
    {
        public MaintenancePlanEntityTypeConfiguration()
        {
            this.ToTable("MaintenancePlan", "Maintenance");
            this.HasKey<int>(p => p.MaintenancePlanId);
        }
    }
    public class MaintenancePlanDetailEntityTypeConfiguration : EntityTypeConfiguration<MaintenancePlanDetail>
    {
        public MaintenancePlanDetailEntityTypeConfiguration()
        {
            this.ToTable("MaintenancePlanDetail", "Maintenance");
            this.HasKey<int>(p => p.MaintenancePlanDetailId);
        }
    }
    public class AdditionalWorkShopDetailEntityTypeConfiguration : EntityTypeConfiguration<AdditionalWorkShopDetail> 
    {
        public AdditionalWorkShopDetailEntityTypeConfiguration()
        {
            this.ToTable("AdditionalWorkShopDetail", "Maintenance");
            this.HasKey<int>(p => p.AdditionalWorkShopDetailId);
        }
    }
    public class ServiceCenterJobEntityTypeConfiguration : EntityTypeConfiguration<ServiceCenterJob>
    {

        public ServiceCenterJobEntityTypeConfiguration()
        {
            this.ToTable("ServiceCenterJob", "Maintenance");
            this.HasKey<Guid>(t => t.ServiceCenterJobId);

        }
    }
    public class WorkshopFieldWorkToolEntityTypeConfiguration : EntityTypeConfiguration<WorkshopFieldWorkTool>
    {

        public WorkshopFieldWorkToolEntityTypeConfiguration()
        {
            this.ToTable("WorkshopFieldWorkTool", "Maintenance");
            this.HasKey<int>(t => t.WorkshopFieldWorkToolId);

        }
    }
    public class ExternalServiceAttachementEntityTypeConfiguration : EntityTypeConfiguration<ExternalServiceAttachement>
    {

        public ExternalServiceAttachementEntityTypeConfiguration()
        {
            this.ToTable("ExternalServiceAttachement", "Maintenance");
            this.HasKey<int>(t => t.ExternalServiceAttachementId);

        }
    }
    public class ExternalServiceCenterPaymentEntityTypeConfiguration : EntityTypeConfiguration<ExternalServiceCenterPayment>
    {
        public ExternalServiceCenterPaymentEntityTypeConfiguration()
        {
            this.ToTable("ExternalServiceCenterPayment", "Maintenance");
            this.HasKey<int>(t => t.ExternalServiceCenterPaymentId);
        }
    }
    public class JobOrderServiceCenterDetailEntityTypeConfiguration : EntityTypeConfiguration<JobOrderServiceCenterDetail>
    {
        public JobOrderServiceCenterDetailEntityTypeConfiguration()
        {
            this.ToTable("JobOrderServiceCenterDetail", "Maintenance");
            this.HasKey<Guid>(t => t.JobOrderServiceCenterDetailId);
        }
    }
    public class VehicleReceptionFormEntityTypeConfiguration : EntityTypeConfiguration<VehicleReceptionForm>
    {

        public VehicleReceptionFormEntityTypeConfiguration()
        {
            this.ToTable("VehicleReceptionForm", "Maintenance");
            this.HasKey<int>(t => t.VehicleReceptionFormId);

        }
    }
    public class VehicleReceptionDocumentEntityTypeConfiguration : EntityTypeConfiguration<VehicleReceptionDocument>
    {

        public VehicleReceptionDocumentEntityTypeConfiguration()
        {
            this.ToTable("VehicleReceptionDoucment", "Maintenance");
            this.HasKey<int>(t => t.VehicleReceptionDocumentId);

        }
    }
    public class ServiceCenterEntityTypeConfiguration : EntityTypeConfiguration<ServiceCenter> //
    {

        public ServiceCenterEntityTypeConfiguration()
        {
            this.ToTable("ServiceCenter", "Maintenance");
            this.HasKey<int>(t => t.ServiceCenterId);

        }
    }
    public class VehiclePartManagementEntityTypeConfiguration : EntityTypeConfiguration<VehiclePartManagement>
    {

        public VehiclePartManagementEntityTypeConfiguration()
        {
            this.ToTable("VehiclePartManagement", "Maintenance");
            this.HasKey<int>(t => t.VehiclePartManagementId);

        }
    }
    public class VehiclePartDamageTypeEntityTypeConfiguration : EntityTypeConfiguration<VehiclePartDamageType>
    {

        public VehiclePartDamageTypeEntityTypeConfiguration()
        {
            this.ToTable("VehiclePartDamageType", "Maintenance");
            this.HasKey<int>(t => t.VehiclePartDamageTypeId);

        }
    }
    public class VehiclePartEntityTypeConfiguration : EntityTypeConfiguration<VehiclePart>
    {

        public VehiclePartEntityTypeConfiguration()
        {
            this.ToTable("VehiclePart", "Maintenance");
            this.HasKey<int>(t => t.VehiclePartId);

        }
    }
    public class PMScheduleEntityTypeConfiguration : EntityTypeConfiguration<PMSchedule>
    {

        public PMScheduleEntityTypeConfiguration()
        {
            this.ToTable("PMSchedule", "Maintenance");
            this.HasKey<int>(t => t.PMScheduleId);

        }
    }
    public class DailyCareDetailItemEntityTypeConfiguration : EntityTypeConfiguration<DailyCareDetailItem>
    {

        public DailyCareDetailItemEntityTypeConfiguration()
        {
            this.ToTable("DailyCareDetailItem", "Maintenance");
            this.HasKey<Guid>(t => t.DailyCareDetailItemId);
        }
    }
    public class PMScheduleDetailEntityTypeConfiguration : EntityTypeConfiguration<PMScheduleDetail>
    {

        public PMScheduleDetailEntityTypeConfiguration()
        {
            this.ToTable("PMScheduleDetail", "Maintenance");
            this.HasKey<int>(t => t.PMScheduleDetailId);

        }
    }
    public class LogisticPlanEntityTypeConfiguration : EntityTypeConfiguration<LogisticPlan>
    {

        public LogisticPlanEntityTypeConfiguration()
        {
            this.ToTable("LogisticPlan", "Maintenance");
            this.HasKey<int>(t => t.LogisticPlanId);

        }
    }
    public class LogisticPlanDetailEntityTypeConfiguration : EntityTypeConfiguration<LogisticPlanDetail>
    {

        public LogisticPlanDetailEntityTypeConfiguration()
        {
            this.ToTable("LogisticPlanDetail", "Maintenance");
            this.HasKey<int>(t => t.LogisticPlanDetailId);

        }
    }
    public class ServiceCrewWorkLogEntityTypeConfiguration : EntityTypeConfiguration<ServiceCrewWorkLog>
    {

        public ServiceCrewWorkLogEntityTypeConfiguration()
        {
            this.ToTable("ServiceCrewWorkLog", "Maintenance");
            this.HasKey<Guid>(t => t.ServiceCrewWorkLogId);

        }
    }
    public class FailureReasonEntityTypeConfiguration : EntityTypeConfiguration<FailureReason>
    {

        public FailureReasonEntityTypeConfiguration()
        {
            this.ToTable("FailureReason", "Maintenance");
            this.HasKey<Guid>(t => t.FailureReasonId);

        }
    }
    public class DailyCareLogEntityTypeConfiguration : EntityTypeConfiguration<DailyCareLog>
    {

        public DailyCareLogEntityTypeConfiguration()
        {
            this.ToTable("DailyCareLog", "Maintenance");
            this.HasKey<Guid>(t => t.DailyCareLogId);

        }
    }
    public class DailyCareDetailLogEntityTypeConfiguration : EntityTypeConfiguration<DailyCareDetailLog>
    {

        public DailyCareDetailLogEntityTypeConfiguration()
        {
            this.ToTable("DailyCareDetailLog", "Maintenance");
            this.HasKey<Guid>(t => t.DailyCareDetailLogId);

        }
    }
    public class DailyCareCategoryEntityTypeConfiguration : EntityTypeConfiguration<DailyCareCategory>
    {

        public DailyCareCategoryEntityTypeConfiguration()
        {
            this.ToTable("DailyCareCategory", "Maintenance");
            this.HasKey<Guid>(t => t.DailyCareCategoryId);

        }
    }
    public class DailyCareEntityTypeConfiguration : EntityTypeConfiguration<DailyCare>
    {

        public DailyCareEntityTypeConfiguration()
        {
            this.ToTable("DailyCare", "Maintenance");
            this.HasKey<Guid>(t => t.DailyCareId);

        }
    }
    public class AdvanceMeasureEntityTypeConfiguration : EntityTypeConfiguration<AdvanceMeasure>
    {

        public AdvanceMeasureEntityTypeConfiguration()
        {
            this.ToTable("AdvanceMeasure", "Maintenance");
            this.HasKey<Guid>(t => t.AdvanceMeasureId);

        }
    }
    public class InventorySparePartMappingEntityTypeConfiguration : EntityTypeConfiguration<InventorySparePartMapping>
    {

        public InventorySparePartMappingEntityTypeConfiguration()
        {
            this.ToTable("InventorySparePartMapping", "Maintenance");
            this.HasKey<Guid>(t => t.InventorySparePartMappingId);

        }
    }
    public class ExternalComponentEntityTypeConfiguration : EntityTypeConfiguration<ExternalComponent>
    {

        public ExternalComponentEntityTypeConfiguration()
        {
            this.ToTable("ExternalComponent", "Maintenance");
            this.HasKey<Guid>(t => t.ExternalComponentId);

        }
    }
    public class ExternalComponentDetailEntityTypeConfiguration : EntityTypeConfiguration<ExternalComponentDetail>
    {

        public ExternalComponentDetailEntityTypeConfiguration()
        {
            this.ToTable("ExternalComponentDetail", "Maintenance");
            this.HasKey<Guid>(t => t.ExternalComponentDetailId);

        }
    }
    public class MaintenaceInspectionLogEntityTypeConfiguration : EntityTypeConfiguration<MaintenaceInspectionLog>
    {

        public MaintenaceInspectionLogEntityTypeConfiguration()
        {
            this.ToTable("MaintenaceInspectionLog", "Maintenance");
            this.HasKey<Guid>(t => t.MaintenaceInspectionLogId);

        }
    }
    public class FollowUpEntityTypeConfiguration : EntityTypeConfiguration<FollowUp>
    {

        public FollowUpEntityTypeConfiguration()
        {
            this.ToTable("FollowUp", "Maintenance");
            this.HasKey<Guid>(t => t.FollowUpId);

        }
    }
    public class MechanicOnHandEntityTypeConfiguration : EntityTypeConfiguration<MechanicOnHand>
    {

        public MechanicOnHandEntityTypeConfiguration()
        {
            this.ToTable("MechanicOnHand", "Maintenance");
            this.HasKey<Guid>(t => t.MechanicOnHandId);

        }
    }

    public class MaintenanceTypeEntityTypeConfiguration : EntityTypeConfiguration<MaintenanceType>
    {

        public MaintenanceTypeEntityTypeConfiguration()
        {
            this.ToTable("MaintenanceType", "Maintenance");
            this.HasKey<Guid>(t => t.MaintenanceTypeId);

        }
    }
    public class JobCausesEntityTypeConfiguration : EntityTypeConfiguration<JobCauses>
    {

        public JobCausesEntityTypeConfiguration()
        {
            this.ToTable("JobCauses", "Maintenance");
            this.HasKey<Guid>(t => t.JobCausesId);

        }
    }

    public class JobRequestEntityTypeConfiguration : EntityTypeConfiguration<JobRequest>
    {

        public JobRequestEntityTypeConfiguration()
        {
            this.ToTable("JobRequests", "Maintenance");
            this.HasKey<Guid>(t => t.JobRequestId);

        }
    }
    public class WorkUnitSectionEntityTypeConfiguration : EntityTypeConfiguration<WorkUnitSection>
    {

        public WorkUnitSectionEntityTypeConfiguration()
        {
            this.ToTable("WorkUnitSection", "Maintenance");
            this.HasKey<Guid>(t => t.WorkUnitSectionId);

        }
    }
    public class JobStoreRequisitionEntityTypeConfiguration : EntityTypeConfiguration<JobStoreRequisition>
    {

        public JobStoreRequisitionEntityTypeConfiguration()
        {
            this.ToTable("JobStoreRequisition", "Maintenance");
            this.HasKey<Guid>(t => t.JobStoreRequisitionId);

        }
    }
    //public class StoreRequisitionSecondValidationEntityTypeConfiguration : EntityTypeConfiguration<StoreRequisitionSecondValidation>
    //{

    //    public StoreRequisitionSecondValidationEntityTypeConfiguration()
    //    {
    //        this.ToTable("StoreRequisitionSecondValidation", "Maintenance");
    //        this.HasKey<Guid>(t => t.StoreRequisitionSecondValidationId);

    //    }
    //}
    public class AdministrativeJobApprovalEntityTypeConfiguration : EntityTypeConfiguration<AdministrativeJobApproval>
    {

        public AdministrativeJobApprovalEntityTypeConfiguration()
        {
            this.ToTable("AdministrativeJobApproval", "Maintenance");
            this.HasKey<int>(t => t.AdministrativeJobApprovalID);

        }
    }

    public class ExternalCrewEntityTypeConfiguration : EntityTypeConfiguration<ExternalCrew>
    {

        public ExternalCrewEntityTypeConfiguration()
        {
            this.ToTable("ExternalCrew", "Maintenance");
            this.HasKey<Guid>(t => t.ExternalCrewID);

        }
    }

    public class ExternalMaterialEntityTypeConfiguration : EntityTypeConfiguration<ExternalMaterial>
    {

        public ExternalMaterialEntityTypeConfiguration()
        {
            this.ToTable("ExternalMaterial", "Maintenance");
            this.HasKey<Guid>(t => t.ExternalMaterialID);

        }
    }
    public class JobOrderEntityTypeConfiguration : EntityTypeConfiguration<JobOrder>
    {

        public JobOrderEntityTypeConfiguration()
        {
            this.ToTable("JobOrder", "Maintenance");
            this.HasKey<Guid>(t => t.JobOrderID);

        }
    }
    
    public class JobOrderDetailEntityTypeConfiguration : EntityTypeConfiguration<JobOrderDetail>
    {

        public JobOrderDetailEntityTypeConfiguration()
        {
            this.ToTable("JobOrderDetail", "Maintenance");
            this.HasKey<Guid>(t => t.JobOrderDetailID);

        }
    }

    public class MaintenanceCenterEntityTypeConfiguration : EntityTypeConfiguration<MaintenanceCenter>
    {

        public MaintenanceCenterEntityTypeConfiguration()
        {
            this.ToTable("MaintenanceCenter", "Maintenance");
            this.HasKey<Guid>(t => t.MaintenanceCenterID);

        }
    }
    public class MaintenanceToolEntityTypeConfiguration : EntityTypeConfiguration<MaintenanceTool>
    {

        public MaintenanceToolEntityTypeConfiguration()
        {
            this.ToTable("MaintenanceTool", "Maintenance");
            this.HasKey<Guid>(t => t.MaintenanceToolID);

        }
    }

    public class MechanicCrewEntityTypeConfiguration : EntityTypeConfiguration<MechanicCrew>
    {

        public MechanicCrewEntityTypeConfiguration()
        {
            this.ToTable("MechanicCrew", "Maintenance");
            this.HasKey<Guid>(t => t.MechanicCrewID);

        }
    }

    public class OtherCompanyMaintenaceApprovalEntityTypeConfiguration : EntityTypeConfiguration<OtherCompanyMaintenaceApproval>
    {

        public OtherCompanyMaintenaceApprovalEntityTypeConfiguration()
        {
            this.ToTable("OtherCompanyMaintenaceApproval", "Maintenance");
            this.HasKey<int>(t => t.OtherCompanyMaintenaceApprovalID);

        }
    }
    public class OtherCompanyMaintenanceEntityTypeConfiguration : EntityTypeConfiguration<OtherCompanyMaintenance>
    {

        public OtherCompanyMaintenanceEntityTypeConfiguration()
        {
            this.ToTable("OtherCompanyMaintenance", "Maintenance");
            this.HasKey<Guid>(t => t.OtherCompanyMaintenanceID);

        }
    }


    public class OutsourceMaintenanceEntityTypeConfiguration : EntityTypeConfiguration<OutsourceMaintenance>
    {

        public OutsourceMaintenanceEntityTypeConfiguration()
        {
            this.ToTable("OutsourceMaintenance", "Maintenance");
            this.HasKey<Guid>(t => t.OutsourceMaintenanceID);

        }
    }


    public class OutsourceMaintenanceApprovalEntityTypeConfiguration : EntityTypeConfiguration<OutsourceMaintenanceApproval>
    {

        public OutsourceMaintenanceApprovalEntityTypeConfiguration()
        {
            this.ToTable("OutsourceMaintenanceApproval", "Maintenance");
            this.HasKey<int>(t => t.OutsourceMaintenanceApprovalId);

        }
    }
    public class PlannerEntityTypeConfiguration : EntityTypeConfiguration<Planner>
    {

        public PlannerEntityTypeConfiguration()
        {
            this.ToTable("Planner", "Maintenance");
            this.HasKey<Guid>(t => t.PlannerID);

        }
    }
    public class RepairTypeEntityTypeConfiguration : EntityTypeConfiguration<RepairType>
    {

        public RepairTypeEntityTypeConfiguration()
        {
            this.ToTable("RepairType", "Maintenance");
            this.HasKey<Guid>(t => t.RepairTypeID);

        }
    }
    public class ServiceEntityTypeConfiguration : EntityTypeConfiguration<Service>
    {

        public ServiceEntityTypeConfiguration()
        {
            this.ToTable("Service", "Maintenance");
            this.HasKey<Guid>(t => t.ServiceID);

        }
    }
    public class ServiceCrewEntityTypeConfiguration : EntityTypeConfiguration<ServiceCrew>
    {

        public ServiceCrewEntityTypeConfiguration()
        {
            this.ToTable("ServiceCrew", "Maintenance");
            this.HasKey<Guid>(t => t.ServiceCrewID);

        }
    }
    public class ServiceSpareEntityTypeConfiguration : EntityTypeConfiguration<ServiceSpare>
    {

        public ServiceSpareEntityTypeConfiguration()
        {
            this.ToTable("ServiceSpare", "Maintenance");
            this.HasKey<Guid>(t => t.ServiceSpareID);

        }
    }
    public class ServiceToolEntityTypeConfiguration : EntityTypeConfiguration<ServiceTool>
    {

        public ServiceToolEntityTypeConfiguration()
        {
            this.ToTable("ServiceTool", "Maintenance");
            this.HasKey<Guid>(t => t.ServiceToolID);

        }
    }
    public class SparePartEntityTypeConfiguration : EntityTypeConfiguration<SparePart>
    {

        public SparePartEntityTypeConfiguration()
        {
            this.ToTable("SparePart", "Maintenance");
            this.HasKey<Guid>(t => t.SparePartID);

        }
    }
    public class TechnicalJobApprovalEntityTypeConfiguration : EntityTypeConfiguration<TechnicalJobApproval>
    {

        public TechnicalJobApprovalEntityTypeConfiguration()
        {
            this.ToTable("TechnicalJobApproval", "Maintenance");
            this.HasKey<int>(t => t.TechnicalJobApprovalID);

        }
    }
    public class MaintenanceToolIssueAndReturnEntityTypeConfiguration : EntityTypeConfiguration<MaintenanceToolIssueAndReturn>
    {

        public MaintenanceToolIssueAndReturnEntityTypeConfiguration()
        {
            this.ToTable("MaintenanceToolIssueAndReturn", "Maintenance");
            this.HasKey<Guid>(t => t.MaintenanceToolIssueAndReturnId);

        }
    }
    public class SpareMaterialCategoryEntityTypeConfiguration : EntityTypeConfiguration<SpareMaterialCategory>
    {
        public SpareMaterialCategoryEntityTypeConfiguration()
        {
            this.ToTable("SpareMaterialCategory", "Maintenance");
            this.HasKey<Guid>(t => t.SpareMaterialCategoryId);

        }
    }
    public class EquipmentInformationEntityTypeConfiguration : EntityTypeConfiguration<EquipmentInformation>
    {
        public EquipmentInformationEntityTypeConfiguration()
        {
            this.ToTable("EquipmentInformation", "Maintenance");
            this.HasKey<Guid>(t => t.EquipmentInformationId);

        }
    }
    public class MaintenanceIntervalEntityTypeConfiguration : EntityTypeConfiguration<MaintenanceInterval>
    {
        public MaintenanceIntervalEntityTypeConfiguration()
        {
            this.ToTable("MaintenanceIntervals", "Maintenance");
            this.HasKey<Guid>(t => t.MaintenanceIntervalId);

        }
    }
}
