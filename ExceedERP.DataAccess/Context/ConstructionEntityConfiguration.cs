using System.Data.Entity.ModelConfiguration;
using ExceedERP.Core.Domain.Construction;

namespace ExceedERP.DataAccess
{
    //public class ConstructionEntityConfiguration
    //{

    //}
    public class ProjectDefinationEntityTypeConfiguration : EntityTypeConfiguration<ProjectInformation>
    {

        public ProjectDefinationEntityTypeConfiguration()
        {
            this.ToTable("ProjectInformation", "Construction");
            this.HasKey<int>(o => o.ProjectInformationId);
        }
    }
    public class ConstructionProjectUserAssignmentEntityTypeConfiguration : EntityTypeConfiguration<ConstructionProjectUserAssignment>
    {

        public ConstructionProjectUserAssignmentEntityTypeConfiguration()
        {
            this.ToTable("ConstructionProjectUserAssignment", "Construction");
            this.HasKey<int>(o => o.ConstructionProjectUserAssignmentId);
        }
    }
    public class LineDateEntityTypeConfiguration : EntityTypeConfiguration<LineDate>
    {

        public LineDateEntityTypeConfiguration()
        {
            this.ToTable("LineDate", "Construction");
            this.HasKey<int>(o => o.LineDateId);
        }
    }

    public class ProjectCalanderEntityTypeConfiguration : EntityTypeConfiguration<ProjectCalendar>
    {

        public ProjectCalanderEntityTypeConfiguration()
        {
            this.ToTable("ProjectCalander", "Construction");
            this.HasKey<int>(o => o.ProjectCalanderId);
        }
    }

    public class ConstructionHolidayEntityTypeConfiguration : EntityTypeConfiguration<ConstructionHoliday>
    {

        public ConstructionHolidayEntityTypeConfiguration()
        {
            this.ToTable("ConstructionHoliday", "Construction");
            this.HasKey<int>(o => o.ConstructionHolidayId);
        }
    }
    public class SettingCalendarEntityTypeConfiguration : EntityTypeConfiguration<SettingCalendar>
    {

        public SettingCalendarEntityTypeConfiguration()
        {
            this.ToTable("Calander", "Construction");
            this.HasKey<int>(o => o.SettingCalendarId);
        }
    }
    public class ProjectConstraintEntityTypeConfiguration : EntityTypeConfiguration<ProjectConstraintsAndRisk>
    {

        public ProjectConstraintEntityTypeConfiguration()
        {
            this.ToTable("ProjectConstraintsAndRisk", "Construction");
            this.HasKey<int>(o => o.ProjectConstraintsAndRiskId);
        }
    }
    public class ProjectSpecificSettingAttributeConfiguration : EntityTypeConfiguration<ProjectSpecificSettingAttribute>
    {

        public ProjectSpecificSettingAttributeConfiguration()
        {
            this.ToTable("ProjectSpecificSettingAttribute", "Construction");
            this.HasKey<int>(o => o.ProjectSpecificSettingAttributeId);
        }
    }

    public class ProjectWorkBreakDownEntityTypeConfiguration : EntityTypeConfiguration<ProjectWorkBreakDown>
    {
        public ProjectWorkBreakDownEntityTypeConfiguration()
        {
            this.ToTable("ProjectWorkBreakDown", "Construction");
            this.HasKey<int>(o => o.ProjectWorkBreakDownId);
        }
    }

    #region Contract Management
    public class MainContractEntityTypeConfiguration : EntityTypeConfiguration<ProjectContract>
    {
        public MainContractEntityTypeConfiguration()
        {
            this.ToTable("ProjectContract", "Construction");
            this.HasKey<int>(o => o.ProjectContractId);

        }
    }
    public class SubContratEntityTypeConfiguration : EntityTypeConfiguration<ProjectSubContract>
    {
        public SubContratEntityTypeConfiguration()
        {
            this.ToTable("ProjectSubContract", "Construction");
            this.HasKey<int>(o => o.ProjectContractId);
        }
    }
    public class ContractVariationOrderEntityTypeConfiguration : EntityTypeConfiguration<ContractVariationOrder>
    {
        public ContractVariationOrderEntityTypeConfiguration()
        {
            this.ToTable("ContractVariationOrder", "Construction");
            this.HasKey<int>(o => o.ContractVariationOrderId);
        }
    }
    public class ContractSuspensionOrderEntityTypeConfiguration : EntityTypeConfiguration<ContractSuspensionOrder>
    {
        public ContractSuspensionOrderEntityTypeConfiguration()
        {
            this.ToTable("ContractSuspensionOrder", "Construction");
            this.HasKey<int>(o => o.ContractSuspensionOrderId);
        }
    }
    public class SupplementalAgreementOrderEntityTypeConfiguration : EntityTypeConfiguration<SupplementalAgreement>
    {
        public SupplementalAgreementOrderEntityTypeConfiguration()
        {
            this.ToTable("SupplementalAgreement", "Construction");
            this.HasKey<int>(o => o.SupplementalAgreementId);
        }
    }
    public class ContractClaimEntityTypeConfiguration : EntityTypeConfiguration<ContractClaim>
    {
        public ContractClaimEntityTypeConfiguration()
        {
            this.ToTable("ContractClaim", "Construction");
            this.HasKey<int>(o => o.ContractClaimId);
        }
    }
    public class ContractClaimValidationEntityTypeConfiguration : EntityTypeConfiguration<ContractClaimValidation>
    {
        public ContractClaimValidationEntityTypeConfiguration()
        {
            this.ToTable("ContractClaimValidation", "Construction");
            this.HasKey<int>(o => o.ContractClaimValidationId);
        }
    }
    public class ContractClaimTypeEntityTypeConfiguration : EntityTypeConfiguration<ContractClaimType>
    {
        public ContractClaimTypeEntityTypeConfiguration()
        {
            this.ToTable("ContractClaimType", "Construction");
            this.HasKey<int>(o => o.ContractClaimTypeId);
        }
    }
    public class ExtensionOfTimeEntityTypeConfiguration : EntityTypeConfiguration<ExtensionOfTime>
    {
        public ExtensionOfTimeEntityTypeConfiguration()
        {
            this.ToTable("ExtensionOfTime", "Construction");
            this.HasKey<int>(o => o.ExtensionOfTimeId);
        }
    }
    public class ExecutedWorkEntityTypeConfiguration : EntityTypeConfiguration<ExecutedWork>
    {
        public ExecutedWorkEntityTypeConfiguration()
        {
            this.ToTable("ExecutedWork", "Construction");
            this.HasKey<int>(o => o.ExecutedWorkId);
        }
    }


    public class CertifiedPaymentEntityTypeConfiguration : EntityTypeConfiguration<CertifiedPayment>
    {
        public CertifiedPaymentEntityTypeConfiguration()
        {
            this.ToTable("CertifiedPayment", "Construction");
            this.HasKey<int>(o => o.CertifiedPaymentId);
        }
    }
    public class VariationOrderTypeEntityTypeConfiguration : EntityTypeConfiguration<VariationOrderType>
    {
        public VariationOrderTypeEntityTypeConfiguration()
        {
            this.ToTable("VariationOrderType", "Construction");
            this.HasKey<int>(o => o.VariationOrderTypeId);
        }
    }
    public class ContractRevisionEntityTypeConfiguration : EntityTypeConfiguration<ContractRevision>
    {
        public ContractRevisionEntityTypeConfiguration()
        {
            this.ToTable("ContractRevision", "Construction");
            this.HasKey<int>(o => o.ContractRevisionId);
        }
    }
    public class ProjectPaymentTermEntityTypeConfiguration : EntityTypeConfiguration<ProjectPaymentTerm>
    {
        public ProjectPaymentTermEntityTypeConfiguration()
        {
            this.ToTable("ProjectPaymentTerm", "Construction");
            this.HasKey<int>(o => o.ProjectPaymentTermId);
        }
    }
    #endregion
    public class ProjectTakeOffEntityTypeConfiguration : EntityTypeConfiguration<ProjectTakeOff>
    {
        public ProjectTakeOffEntityTypeConfiguration()
        {
            this.ToTable("ProjectTakeOff", "Construction");
            this.HasKey<int>(o => o.ProjectTakeOffId);
        }
    }
    public class ProjectTakeOffLineEntityTypeConfiguration : EntityTypeConfiguration<ProjectTakeOffLine>
    {
        public ProjectTakeOffLineEntityTypeConfiguration()
        {
            this.ToTable("ProjectTakeOffLine", "Construction");
            this.HasKey<int>(o => o.ProjectTakeOffLineId);
            this.Property(o => o.Width).HasPrecision(18, 4);
            this.Property(o => o.Depth).HasPrecision(18, 4);
            this.Property(o => o.Length).HasPrecision(18, 4);
        }
    }
    public class PaymentCertificateEntityTypeConfiguration : EntityTypeConfiguration<ProjectPaymentCertificate>
    {
        public PaymentCertificateEntityTypeConfiguration()
        {
            this.ToTable("PaymentCertificate", "Construction");
            this.HasKey<int>(o => o.ProjectPaymentCertificateId);
        }
    }
    public class ProjectPaymentCertificateValidationEntityTypeConfiguration : EntityTypeConfiguration<ProjectPaymentCertificateValidation>
    {
        public ProjectPaymentCertificateValidationEntityTypeConfiguration()
        {
            this.ToTable("ProjectPaymentCertificateValidation", "Construction");
            this.HasKey<int>(o => o.ProjectPaymentCertificateValidationId);
        }
    }
    public class BoQOfExecutedWorkEntityTypeConfiguration : EntityTypeConfiguration<BoQOfExecutedWork>
    {
        public BoQOfExecutedWorkEntityTypeConfiguration()
        {
            this.ToTable("BoQOfExecutedWork", "Construction");
            this.HasKey<int>(o => o.BoQOfExecutedWorkId);
        }
    }
    public class BoQOfExecutedWorkContentEntityTypeConfiguration : EntityTypeConfiguration<BoQOfExecutedWorkContent>
    {
        public BoQOfExecutedWorkContentEntityTypeConfiguration()
        {
            this.ToTable("BoQOfExecutedWorkContent", "Construction");
            this.HasKey<int>(o => o.BoQOfExecutedWorkContentId);
        }
    }
    public class BoQOfExecutedWorkContentDetailEntityTypeConfiguration : EntityTypeConfiguration<BoQOfExecutedWorkContentDetail>
    {
        public BoQOfExecutedWorkContentDetailEntityTypeConfiguration()
        {
            this.ToTable("BoQOfExecutedWorkContentDetail", "Construction");
            this.HasKey<int>(o => o.BoQOfExecutedWorkContentDetailId);
        }
    }
    public class LineAmountEntityTypeConfiguration : EntityTypeConfiguration<ProjectPaymenCertificatetLine>
    {
        public LineAmountEntityTypeConfiguration()
        {
            this.ToTable("PaymenCertificatetLine", "Construction");
            this.HasKey<int>(o => o.ProjectPaymenCertificatetLineId);
        }
    }
    //public class AdvancePaymentEntityTypeConfiguration : EntityTypeConfiguration<ProjectPaymenAdvance>
    //{
    //    public AdvancePaymentEntityTypeConfiguration()
    //    {
    //        this.ToTable("ProjectPaymenAdvance", "Construction");
    //        this.HasKey<int>(o => o.ProjectPaymenAdvanceId);
    //    }
    //}
    public class RetentionEntityTypeConfiguration : EntityTypeConfiguration<ProjectPaymenRetention>
    {
        public RetentionEntityTypeConfiguration()
        {
            this.ToTable("Retention", "Construction");
            this.HasKey<int>(o => o.ProjectPaymenRetentionId);
        }
    }
    public class ProjectDefineBudgetEntityTypeConfiguration : EntityTypeConfiguration<ProjectDefineBudget>
    {
        public ProjectDefineBudgetEntityTypeConfiguration()
        {
            this.ToTable("ProjectDefineBudget", "Construction");
            this.HasKey<int>(o => o.ProjectDefineBudgetId);
        }
    }

    public class ProjectBudgetRequestEntityTypeConfiguration : EntityTypeConfiguration<ProjectBudgetRequest>
    {
        public ProjectBudgetRequestEntityTypeConfiguration()
        {
            this.ToTable("ProjectBudgetRequest", "Construction");
            this.HasKey<int>(o => o.ProjectBudgetRequestId);
        }
    }
    public class ProjectBudgetTransferEntityTypeConfiguration : EntityTypeConfiguration<ProjectBudgetTransfer>
    {
        public ProjectBudgetTransferEntityTypeConfiguration()
        {
            this.ToTable("ProjectBudgetTransfer", "Construction");
            this.HasKey<int>(o => o.ProjectBudgetTransferId);
        }

    }
    public class ProjectBudgetUtilizationEntityTypeConfiguration : EntityTypeConfiguration<ProjectBudgetUtilization>
    {
        public ProjectBudgetUtilizationEntityTypeConfiguration()
        {
            this.ToTable("ProjectBudgetUtilization", "Construction");
            this.HasKey<int>(o => o.ProjectBudgetUtilizationId);
        }

    }

    public class ProjectMaterialRequestEntityTypeConfiguration : EntityTypeConfiguration<ProjectMaterialRequest>
    {
        public ProjectMaterialRequestEntityTypeConfiguration()
        {
            this.ToTable("ProjectMaterialRequest", "Construction");
            this.HasKey<int>(o => o.ProjectMaterialRequestId);
        }
    }
    public class ProjectMaterialIssuedEntityTypeConfiguration : EntityTypeConfiguration<ProjectMaterialIssued>
    {
        public ProjectMaterialIssuedEntityTypeConfiguration()
        {
            this.ToTable("ProjectMaterialIssued", "Construction");
            this.HasKey<int>(o => o.ProjectMaterialIssuedId);
        }
    }
    public class ProjectMaterialDeliveryEntityTypeConfiguration : EntityTypeConfiguration<ProjectMaterialDelivery>
    {
        public ProjectMaterialDeliveryEntityTypeConfiguration()
        {
            this.ToTable("ProjectMaterialDelivery", "Construction");
            this.HasKey<int>(o => o.ProjectMaterialDeliveryId);
        }
    }
    public class ProjectEquipmentUtilizationEntityTypeConfiguration : EntityTypeConfiguration<ProjectEquipmentUtilization>
    {
        public ProjectEquipmentUtilizationEntityTypeConfiguration()
        {
            this.ToTable("ProjectEquipmentUtilization", "Construction");
            this.HasKey<int>(o => o.ProjectEquipmentUtilizationId);
        }
    }
    public class ProjectEquipmentHourEntityTypeConfiguration : EntityTypeConfiguration<ProjectEquipmentHour>
    {
        public ProjectEquipmentHourEntityTypeConfiguration()
        {
            this.ToTable("ProjectEquipmentHour", "Construction");
            this.HasKey<int>(o => o.ProjectEquipmentHourId);
        }
    }
    public class ProjectEquipmentPerformanceEntityTypeConfiguration : EntityTypeConfiguration<ProjectEquipmentPerformance>
    {
        public ProjectEquipmentPerformanceEntityTypeConfiguration()
        {
            this.ToTable("ProjectEquipmentPerformance", "Construction");
            this.HasKey<int>(o => o.ProjectEquipmentPerformanceId);
        }
    }
    public class ProjectEquipmentFuelconsumptionEntityTypeConfiguration : EntityTypeConfiguration<ProjectEquipmentFuelconsumption>
    {
        public ProjectEquipmentFuelconsumptionEntityTypeConfiguration()
        {
            this.ToTable("EquipmentFuelconsumption", "Construction");
            this.HasKey<int>(o => o.ProjectEquipmentFuelconsumptionId);
        }
    }
    public class ProjectEquipmentMobilizationEntityTypeConfiguration : EntityTypeConfiguration<ProjectEquipmentMobilization>
    {
        public ProjectEquipmentMobilizationEntityTypeConfiguration()
        {
            this.ToTable("ProjectEquipmentMobilization", "Construction");
            this.HasKey<int>(o => o.ProjectEquipmentMobilizationId);
        }
    }
    public class ProjectHumanResourceUtilizationEntityTypeConfiguration : EntityTypeConfiguration<ProjectHumanResourceUtilization>
    {
        public ProjectHumanResourceUtilizationEntityTypeConfiguration()
        {
            this.ToTable("ProjectHumanResourceUtilization", "Construction");
            this.HasKey<int>(o => o.ProjectHumanResourceUtilizationId);
        }
    }
    public class EquipmentMobilizationEntityTypeConfiguration : EntityTypeConfiguration<EquipmentMobilization>
    {
        public EquipmentMobilizationEntityTypeConfiguration()
        {
            this.ToTable("EquipmentMobilization", "Construction");
            this.HasKey<int>(o => o.EquipmentMobilizationId);
        }
    }
    public class ProjectEstimationNameEntityTypeConfiguration : EntityTypeConfiguration<ProjectEstimationName>
    {
        public ProjectEstimationNameEntityTypeConfiguration()
        {
            this.ToTable("ProjectEstimationName", "Construction");
            this.HasKey<int>(o => o.ProjectEstimationNameId);
        }
    }
    public class ProjectEstimationBoqEntityTypeConfiguration : EntityTypeConfiguration<ProjectEstimationBoq>
    {
        public ProjectEstimationBoqEntityTypeConfiguration()
        {
            this.ToTable("ProjectEstimationBoq", "Construction");
            this.HasKey<int>(o => o.ProjectEstimationBoqid);
            this.Property(o => o.TotalCost).HasPrecision(18, 4);

        }
    }
    public class ProjectEstimationBoqBreakdownEntityTypeConfiguration : EntityTypeConfiguration<ProjectEstimationBoqBreakdown>
    {
        public ProjectEstimationBoqBreakdownEntityTypeConfiguration()
        {
            this.ToTable("ProjectEstimnBoqBreakdown", "Construction");
            this.HasKey<int>(o => o.ProjectEstimationBoqBreakdownId);
            this.Property(o => o.TotalCost).HasPrecision(18, 4);
            this.Property(o => o.Quantity).HasPrecision(18, 4);
            this.Property(o => o.UnitRate).HasPrecision(18, 4);
        }
    }
    public class ProjectEstimationWorkCategoryEntityTypeConfiguration : EntityTypeConfiguration<ProjectEstimationWorkCategory>
    {
        public ProjectEstimationWorkCategoryEntityTypeConfiguration()
        {
            this.ToTable("ProjectEstmnWorkCategory", "Construction");
            this.HasKey<int>(o => o.ProjectEstimationWorkCategoryId);
        }
    }
    public class ProjectEstimationWorkItemEntityTypeConfiguration : EntityTypeConfiguration<ProjectEstimationWorkItem>
    {
        public ProjectEstimationWorkItemEntityTypeConfiguration()
        {
            this.ToTable("ProjectEstimnWorkItem", "Construction");
            this.HasKey<int>(o => o.ProjectEstimationWorkItemId);
            this.Property(d => d.TotalUnitCost).HasPrecision(18, 4);
            this.Property(d => d.MaterialCostPerUnit).HasPrecision(18, 4);
            this.Property(d => d.LaborCostPerUnit).HasPrecision(18, 4);
            this.Property(d => d.EquipmentCostPerUnit).HasPrecision(18, 4);
            this.Property(d => d.DirectCost).HasPrecision(18, 4);
            this.Property(d => d.MaterialCost).HasPrecision(18, 4);
            this.Property(d => d.LabourOutPut).HasPrecision(18, 4);
            this.Property(d => d.EquipmentOutput).HasPrecision(18, 4);
            this.Property(d => d.OverHeadCost).HasPrecision(18, 4);
            this.Property(d => d.ProfitCost).HasPrecision(18, 4);
            this.Property(d => d.TotalQuantityOfWork).HasPrecision(18, 4);
            this.Property(d => d.LabourHourlyOutPut).HasPrecision(18, 4);

        }
    }
    public class WorkCategoryEntityTypeConfiguration : EntityTypeConfiguration<WorkCategory>
    {
        public WorkCategoryEntityTypeConfiguration()
        {
            this.ToTable("WorkCategory", "Construction");
            this.HasKey<int>(o => o.WorkCategoryId);
        }
    }
    public class ProjectEstimationMaterialEntityTypeConfiguration : EntityTypeConfiguration<ProjectEstimationMaterial>
    {
        public ProjectEstimationMaterialEntityTypeConfiguration()
        {
            this.ToTable("ProjectEstmnMaterial", "Construction");
            this.HasKey<int>(o => o.ProjectEstimationMaterialId);
            this.Property(o => o.Quantity).HasPrecision(18, 4);
            this.Property(o => o.TotalCost).HasPrecision(18, 4);
            this.Property(o => o.UnitPrice).HasPrecision(18, 4);

        }
    }
    public class ProjectEstimationDirectLabourEntityTypeConfiguration : EntityTypeConfiguration<ProjectEstimationDirectLabour>
    {
        public ProjectEstimationDirectLabourEntityTypeConfiguration()
        {
            this.ToTable("ProjectEstmnDirectLabour", "Construction");
            this.HasKey<int>(o => o.ProjectEstimationDirectLabourId);
            this.Property(o => o.Quantity).HasPrecision(18, 4);
            this.Property(o => o.TotalCost).HasPrecision(18, 4);
            this.Property(o => o.UnitFactory).HasPrecision(18, 4);
        }
    }
    public class ProjectEstimationIndirectLabourEntityTypeConfiguration : EntityTypeConfiguration<ProjectEstimationIndirectLabour>
    {
        public ProjectEstimationIndirectLabourEntityTypeConfiguration()
        {
            this.ToTable("ProjectEstmnIndirectLabour", "Construction");
            this.HasKey<int>(o => o.ProjectEstimationIndirectLabourId);
            this.Property(o => o.Quantity).HasPrecision(18, 4);
            this.Property(o => o.TotalCost).HasPrecision(18, 4);
            this.Property(o => o.DailyPay).HasPrecision(18, 4);
            this.Property(o => o.UnitFactory).HasPrecision(18, 4);
        }
    }
    public class ProjectEstimationEquipmentEntityTypeConfiguration : EntityTypeConfiguration<ProjectEstimationEquipment>
    {
        public ProjectEstimationEquipmentEntityTypeConfiguration()
        {
            this.ToTable("ProjectEstmnEquipment", "Construction");
            this.HasKey<int>(o => o.ProjectEstimationEquipmentId);
            this.Property(o => o.Quantity).HasPrecision(18, 4);
            this.Property(d => d.UnitFactory).HasPrecision(18, 4);
            this.Property(o => o.TotalCost).HasPrecision(18, 4);
            this.Property(o => o.RentalCost).HasPrecision(18, 4);
        }
    }
    public class UnitRateEntityTypeConfiguration : EntityTypeConfiguration<UnitRate>
    {
        public UnitRateEntityTypeConfiguration()
        {
            this.ToTable("UnitRate", "Construction");
            this.HasKey<int>(o => o.UnitRateId);
        }
    }
    public class ProjectEstimationUnitFactoryEntityTypeConfiguration : EntityTypeConfiguration<ProjectEstimationUnitFactory>
    {
        public ProjectEstimationUnitFactoryEntityTypeConfiguration()
        {
            this.ToTable("ProjectEstmnUnitFactory", "Construction");
            this.HasKey<int>(o => o.ProjectEstimationUnitFactoryId);
        }
    }
    public class ProjectEstimationMobilizationEntityTypeConfiguration : EntityTypeConfiguration<ProjectEstimationMobilization>
    {
        public ProjectEstimationMobilizationEntityTypeConfiguration()
        {
            this.ToTable("ProjectEstmnMobilization", "Construction");
            this.HasKey<int>(o => o.ProjectEstimationMobilizationId);
        }
    }
    public class ProjectEstimationCoefficientsEntityTypeConfiguration : EntityTypeConfiguration<ProjectEstimationCoefficients>
    {
        public ProjectEstimationCoefficientsEntityTypeConfiguration()
        {
            this.ToTable("ProjectEstmnCoefficients", "Construction");
            this.HasKey<int>(o => o.ProjectEstimationCoefficientsId);
        }
    }
    public class ProjectEstimationRawDataEntityTypeConfiguration : EntityTypeConfiguration<ProjectEstimationRawData>
    {
        public ProjectEstimationRawDataEntityTypeConfiguration()
        {
            this.ToTable("ProjectEstmnRawData", "Construction");
            this.HasKey<int>(o => o.ProjectEstimationRawDataId);
        }
    }
    public class ProjectEstimationIndexLabourEntityTypeConfiguration : EntityTypeConfiguration<ProjectEstimationIndexLabour>
    {
        public ProjectEstimationIndexLabourEntityTypeConfiguration()
        {
            this.ToTable("ProjectEstmnIndexLabour", "Construction");
            this.HasKey<int>(o => o.ProjectEstimationIndexLabourId);
        }
    }

    public class ProjectMachineryRentHeaderEntityTypeConfiguration : EntityTypeConfiguration<ProjectMachineryRentHeader>
    {
        public ProjectMachineryRentHeaderEntityTypeConfiguration()
        {
            this.ToTable("ProjectMachineryRentHeader", "Construction");
            this.HasKey<int>(o => o.ProjectEstimationMachineryRentId);
        }
    }
    public class ProjectEstimationTransportCostEntityTypeConfiguration : EntityTypeConfiguration<ProjectEstimationTransportCost>
    {
        public ProjectEstimationTransportCostEntityTypeConfiguration()
        {
            this.ToTable("ProjectEstmnTransportCost", "Construction");
            this.HasKey<int>(o => o.ProjectEstimationTransportCostId);
        }
    }
    public class MapWorkItemCategoryEntityTypeConfiguration : EntityTypeConfiguration<MapWorkItemCategory>
    {
        public MapWorkItemCategoryEntityTypeConfiguration()
        {
            this.ToTable("MapWorkItemCategory", "Construction");
            this.HasKey<int>(o => o.MapWorkCategoryId);
        }
    }
    public class MaterialUtilizationEntityTypeConfiguration : EntityTypeConfiguration<MaterialUtilization>
    {
        public MaterialUtilizationEntityTypeConfiguration()
        {
            this.ToTable("MaterialUtilization", "Construction");
            this.HasKey<int>(o => o.MaterialUtilizationId);
        }
    }
    #region Scheduling
    public class ScheduleEntityTypeConfiguration : EntityTypeConfiguration<Core.Domain.Construction.Task>
    {
        public ScheduleEntityTypeConfiguration()
        {
            this.ToTable("Task", "Construction");
            this.HasKey<int>(o => o.ID);
        }
    }
    public class GanttResourceAssignmentEntityTypeConfiguration : EntityTypeConfiguration<ResourceAssignment>
    {
        public GanttResourceAssignmentEntityTypeConfiguration()
        {
            this.ToTable("ResourceAssignment", "Construction");
            this.HasKey<int>(o => o.ID);
        }
    }
    public class GanttDependencyEntityTypeConfiguration : EntityTypeConfiguration<Dependency>
    {
        public GanttDependencyEntityTypeConfiguration()
        {
            this.ToTable("Dependency", "Construction");
            this.HasKey<int>(o => o.ID);
        }
    }
    public class GanttResourceEntityTypeConfiguration : EntityTypeConfiguration<Resource>
    {
        public GanttResourceEntityTypeConfiguration()
        {
            this.ToTable("Resource", "Construction");
            this.HasKey<int>(o => o.ID);
        }
    }

    public class ResourceTypeEntityTypeConfiguration : EntityTypeConfiguration<ResourceType>
    {
        public ResourceTypeEntityTypeConfiguration()
        {
            this.ToTable("ResourceType", "Construction");
            this.HasKey<int>(rt => rt.ResourceTypeId);
        }
    }

    public class ScheduleRevisionEntityTypeConfiguration : EntityTypeConfiguration<ScheduleRevision>
    {
        public ScheduleRevisionEntityTypeConfiguration()
        {
            this.ToTable("ScheduleRevision", "Construction");
            this.HasKey<int>(rt => rt.ScheduleRevisionId);
        }
    }
    public class ScheduleRevisionValidationEntityTypeConfiguration : EntityTypeConfiguration<ScheduleRevisionValidation>
    {
        public ScheduleRevisionValidationEntityTypeConfiguration()
        {
            this.ToTable("ScheduleRevisionValidation", "Construction");
            this.HasKey<int>(rt => rt.ScheduleRevisionValidationId);
        }
    }
    public class TaskDetailEntityTypeConfiguration : EntityTypeConfiguration<TaskDetail>
    {
        public TaskDetailEntityTypeConfiguration()
        {
            this.ToTable("TaskDetail", "Construction");
            this.HasKey<int>(rt => rt.TaskDetailId);
        }
    }
    public class TaskDetailBreakDownEntityTypeConfiguration : EntityTypeConfiguration<TaskDetailBreakDown>
    {
        public TaskDetailBreakDownEntityTypeConfiguration()
        {
            this.ToTable("TaskDetailBreakDown", "Construction");
            this.HasKey<int>(rt => rt.TaskDetailBreakDownId);
        }
    }
    #endregion
    public class ProjectTakeOffLineForKgEntityTypeConfiguration : EntityTypeConfiguration<ProjectTakeOffLineForKg>
    {
        public ProjectTakeOffLineForKgEntityTypeConfiguration()
        {
            this.ToTable("ProjectTakeOffLineForKg", "Construction");
            this.HasKey<int>(o => o.ProjectTakeOffLineForKgId);
        }
    }
    public class MachinaryRentEntityTypeConfiguration : EntityTypeConfiguration<MachinaryRent>
    {
        public MachinaryRentEntityTypeConfiguration()
        {
            this.ToTable("MachinaryRent", "Construction");
            this.HasKey<int>(o => o.MachinaryRentId);
            this.Property(d => d.ApprovedCost).HasPrecision(18, 4);
            this.Property(d => d.Rental).HasPrecision(18, 4);
            this.Property(d => d.RentalWithFuel).HasPrecision(18, 4);


        }
    }
    public class InternalMachinaryRentEntityTypeConfiguration : EntityTypeConfiguration<InternalMachinaryRent>
    {
        public InternalMachinaryRentEntityTypeConfiguration()
        {
            this.ToTable("InternalMachinaryRent", "Construction");
            this.HasKey<int>(o => o.InternalMachinaryRentId);
        }
    }
    public class ProjectEstmnBoqSummaryEntityTypeConfiguration : EntityTypeConfiguration<ProjectEstmnBoqSummary>
    {
        public ProjectEstmnBoqSummaryEntityTypeConfiguration()
        {
            this.ToTable("ProjectEstmnBoqSummary", "Construction");
            this.HasKey<int>(o => o.ProjectEstmnBoqSummaryId);
        }
    }
    public class ProjectEstimationBoqDetailEntityTypeConfiguration : EntityTypeConfiguration<ProjectEstimationBoqDetail>
    {
        public ProjectEstimationBoqDetailEntityTypeConfiguration()
        {
            this.ToTable("ProjectEstimationBoqDetail", "Construction");
            this.HasKey<int>(o => o.ProjectEstmnBoqDetailId);
        }
    }

    public class DeformedBarEntityTypeConfiguration : EntityTypeConfiguration<DeformedBar>
    {
        public DeformedBarEntityTypeConfiguration()
        {
            this.ToTable("DeformedBar", "Construction");
            this.HasKey<int>(df => df.DeformedBarId);
            this.Property(b => b.Diameter).HasPrecision(18, 4);
            this.Property(b => b.UnitWeight).HasPrecision(18, 4);
        }
    }
    #region Project Crew 
    public class ProjectCrewEntityTypeConfiguration : EntityTypeConfiguration<ProjectCrew>
    {
        public ProjectCrewEntityTypeConfiguration()
        {
            this.ToTable("ProjectCrew", "Construction");
            this.HasKey<int>(df => df.ProjectCrewId);
        }
    }
    public class CrewHumanResourceEntityTypeConfiguration : EntityTypeConfiguration<CrewHumanResource>
    {
        public CrewHumanResourceEntityTypeConfiguration()
        {
            this.ToTable("CrewHumanResource", "Construction");
            this.HasKey<int>(df => df.CrewHumanResourceId);
        }
    }
    public class CrewEquipmentEntityTypeConfiguration : EntityTypeConfiguration<CrewEquipment>
    {
        public CrewEquipmentEntityTypeConfiguration()
        {
            this.ToTable("CrewEquipment", "Construction");
            this.HasKey<int>(df => df.CrewEquipmentId);
        }
    }
    public class CrewMaterialEntityTypeConfiguration : EntityTypeConfiguration<CrewMaterial>
    {
        public CrewMaterialEntityTypeConfiguration()
        {
            this.ToTable("CrewMaterial", "Construction");
            this.HasKey<int>(df => df.CrewMaterialId);
        }
    }
    #endregion
    public class BarDiameterEntityTypeConfiguration : EntityTypeConfiguration<BarDiameter>
    {
        public BarDiameterEntityTypeConfiguration()
        {
            this.ToTable("BarDiameter", "Construction");
            this.HasKey<int>(bd => bd.BarDiameterId);
        }
    }
    public class WorkItemDurationEntityTypeConfiguration : EntityTypeConfiguration<WorkItemDuration>
    {
        public WorkItemDurationEntityTypeConfiguration()
        {
            this.ToTable("WorkItemDuration", "Construction");
            this.HasKey(o => o.WorkItemDurationId);
            this.Property(o => o.Quantity).HasPrecision(18, 4);
            this.Property(o => o.ProductivityOfCrewPerDay).HasPrecision(18, 4);
            this.Property(o => o.DurationPerCrew).HasPrecision(18, 4);
            this.Property(o => o.EstimatedDuration).HasPrecision(18, 4);
        }
    }
    public class ResourceEstimationLabourEntityTypeConfiguration : EntityTypeConfiguration<ResourceEstimationLabour>
    {
        public ResourceEstimationLabourEntityTypeConfiguration()
        {
            this.ToTable("ResourceEstimationLabour", "Construction");
            this.HasKey(o => o.ResourceEstimationLabourId);
            this.Property(o => o.UnitFactor).HasPrecision(18, 4);
            this.Property(o => o.NumberPerDay).HasPrecision(18, 4);
            this.Property(o => o.UnitFactorAndNumberPerCrew).HasPrecision(18, 4);
            this.Property(o => o.IndexedHourlySalary).HasPrecision(18, 4);
            this.Property(o => o.TotalCostPerDay).HasPrecision(18, 4);
        }
    }
    public class ResourceEstimationEquipmentEntityTypeConfiguration : EntityTypeConfiguration<ResourceEstimationEquipment>
    {
        public ResourceEstimationEquipmentEntityTypeConfiguration()
        {
            this.ToTable("ResourceEstimationEquipment", "Construction");
            this.HasKey(o => o.ResourceEstimationEquipmentId);
            this.Property(o => o.UnitFactor).HasPrecision(18, 4);
            this.Property(o => o.NumberPerDay).HasPrecision(18, 4);
            this.Property(o => o.UnitFactorAndNumberPerCrew).HasPrecision(18, 4);
            this.Property(o => o.HourlyCost).HasPrecision(18, 4);
            this.Property(o => o.TotalCostPerDay).HasPrecision(18, 4);
        }
    }
    public class ResourceEstimationMaterialEntityTypeConfiguration : EntityTypeConfiguration<ResourceEstimationMaterial>
    {
        public ResourceEstimationMaterialEntityTypeConfiguration()
        {
            this.ToTable("ResourceEstimationMaterial", "Construction");
            this.HasKey(o => o.ResourceEstimationMaterialId);
            this.Property(o => o.QuantityPerUnit).HasPrecision(18, 4);
            this.Property(o => o.TotalQuantity).HasPrecision(18, 4);
            this.Property(o => o.UnitPrice).HasPrecision(18, 4);
            this.Property(o => o.TotalCost).HasPrecision(18, 4);
        }
    }
    public class MonthlyFuelCostEntityTypeConfiguration : EntityTypeConfiguration<MonthlyFuelCost>
    {
        public MonthlyFuelCostEntityTypeConfiguration()
        {
            this.ToTable("MonthlyFuelCost", "Construction");
            this.HasKey<int>(o => o.MonthlyFuelCostId);
            this.Property(o => o.Price).HasPrecision(18, 4);
        }
    }
    public class LaborRateIndexFactorAttributeNameEntityTypeConfiguration : EntityTypeConfiguration<LaborRateIndexFactorAttributeName>
    {
        public LaborRateIndexFactorAttributeNameEntityTypeConfiguration()
        {
            this.ToTable("LaborRateIndexFactorAttributeName", "Construction");
            this.HasKey<int>(o => o.LaborRateIndexFactorAttributeNameId);
        }
    }
    public class LaborRateIndexFactorAttributeEntityTypeConfiguration : EntityTypeConfiguration<LaborRateIndexFactorAttribute>
    {
        public LaborRateIndexFactorAttributeEntityTypeConfiguration()
        {
            this.ToTable("LaborRateIndexFactorAttribute", "Construction");
            this.HasKey(o => o.LaborRateIndexFactorAttributeId);
            this.Property(p => p.Index).HasPrecision(18, 4);
            this.Property(p => p.ApprovedIndex).HasPrecision(18, 4);
        }
    }
    #region Resource Utilization
    public class MethodOfWorkEntityTypeConfiguration : EntityTypeConfiguration<MethodOfWork>
    {
        public MethodOfWorkEntityTypeConfiguration()
        {
            this.ToTable("MethodOfWork", "Construction");
            this.HasKey<int>(m => m.MethodOfWorkId);
        }
    }
    public class ResourceUtilizationPeriodEntityTypeConfiguration : EntityTypeConfiguration<ResourceUtilizationPeriod>
    {
        public ResourceUtilizationPeriodEntityTypeConfiguration()
        {
            this.ToTable("ResourceUtilizationPeriod", "Construction");
            this.HasKey<int>(m => m.ResourceUtilizationPeriodId);
        }
    }
    public class ResourceUtilizationDayEntityTypeConfiguration : EntityTypeConfiguration<ResourceUtilizationDay>
    {
        public ResourceUtilizationDayEntityTypeConfiguration()
        {
            this.ToTable("ResourceUtilizationDays", "Construction");
            this.HasKey<int>(m => m.ResourceUtilizationDayId);
        }
    }
    public class ResourceUtilizationWorkItemkEntityTypeConfiguration : EntityTypeConfiguration<ResourceUtilizationWorkItem>
    {
        public ResourceUtilizationWorkItemkEntityTypeConfiguration()
        {
            this.ToTable("ResourceUtilizationWorkItem", "Construction");
            this.HasKey<int>(m => m.ResourceUtilizationWorkItemId);
            this.Property(d => d.Income).HasPrecision(18, 4);
            this.Property(d => d.DirectContractUnitPrice).HasPrecision(18, 4);
            this.Property(d => d.TotalDirectCost).HasPrecision(18, 4);
            this.Property(d => d.MaterialCost).HasPrecision(18, 4);
            this.Property(d => d.LaborCost).HasPrecision(18, 4);
            this.Property(d => d.EquipmentCost).HasPrecision(18, 4);
            this.Property(d => d.TotalDirectCost).HasPrecision(18, 4);
        }
    }
    public class DetailedWorkProgressEntityTypeConfiguration : EntityTypeConfiguration<DetailedWorkProgress>
    {
        public DetailedWorkProgressEntityTypeConfiguration()
        {
            this.ToTable("DetailedWorkProgress", "Construction");
            this.HasKey<int>(m => m.DetailedWorkProgressId);
            this.Property(d => d.PlannedQuantity).HasPrecision(18, 4);
            this.Property(d => d.PlannedAmount).HasPrecision(18, 4);
            this.Property(d => d.ActualQuantity).HasPrecision(18, 4);
            this.Property(d => d.ActualAmount).HasPrecision(18, 4);

            this.Property(d => d.TodateActualQuantity).HasPrecision(18, 4);
            this.Property(d => d.TodateActualAmount).HasPrecision(18, 4);
            this.Property(d => d.TodatePlannedQuantity).HasPrecision(18, 4);
            this.Property(d => d.TodatePlannedAmount).HasPrecision(18, 4);

            this.Property(d => d.PreviousActualQuantity).HasPrecision(18, 4);
            this.Property(d => d.PreviousActualAmount).HasPrecision(18, 4);
            this.Property(d => d.PreviousPlannedQuantity).HasPrecision(18, 4);
            this.Property(d => d.PreviousPlannedAmount).HasPrecision(18, 4);
        }
    }
    public class ResourceUtilizationMaterialEntityTypeConfiguration : EntityTypeConfiguration<ResourceUtilizationMaterial>
    {
        public ResourceUtilizationMaterialEntityTypeConfiguration()
        {
            this.ToTable("ResourceUtilizationMaterial", "Construction");
            this.HasKey<int>(m => m.ResourceUtilizationMaterialId);
            this.Property(d => d.UnitPrice).HasPrecision(18, 4);
            this.Property(d => d.TotalCost).HasPrecision(18, 4);
        }
    }
    public class ResourceUtilizationDirectLabourEntityTypeConfiguration : EntityTypeConfiguration<ResourceUtilizationDirectLabour>
    {
        public ResourceUtilizationDirectLabourEntityTypeConfiguration()
        {
            this.ToTable("ResourceUtilizationDirectLabour", "Construction");
            this.HasKey<int>(m => m.ResourceUtilizationDirectLabourId);
            this.Property(d => d.UnitFactory).HasPrecision(18, 4);
            this.Property(d => d.IndexedHourlySalary).HasPrecision(18, 4);
            this.Property(d => d.TotalCost).HasPrecision(18, 4);
        }
    }
    public class ResourceUtilizationEquipmentEntityTypeConfiguration : EntityTypeConfiguration<ResourceUtilizationEquipment>
    {
        public ResourceUtilizationEquipmentEntityTypeConfiguration()
        {
            this.ToTable("ResourceUtilizationEquipment", "Construction");
            this.HasKey<int>(m => m.ResourceUtilizationEquipmentId);
            this.Property(d => d.UnitFactory).HasPrecision(18, 4);
            this.Property(d => d.RentalCost).HasPrecision(18, 4);
            this.Property(d => d.TotalCost).HasPrecision(18, 4);
        }
    }
    #region Permissions
    public class PurchasePriceComparisonEntityTypeConfiguration : EntityTypeConfiguration<PurchasePriceComparison>
    {
        public PurchasePriceComparisonEntityTypeConfiguration()
        {
            this.ToTable("PurchasePriceComparison", "Construction");
            this.HasKey<int>(m => m.PurchasePriceComparisonId);
        }
    }
    public class PurchaseSupplierEntityTypeConfiguration : EntityTypeConfiguration<PurchaseSupplier>
    {
        public PurchaseSupplierEntityTypeConfiguration()
        {
            this.ToTable("PurchaseSupplier", "Construction");
            this.HasKey<int>(m => m.PurchaseSupplierId);
        }
    }
    #endregion
    #endregion
    #region Tracking | Earned value management
    public class EarnedValueManagementTermsEntityTypeConfiguration : EntityTypeConfiguration<EarnedValueManagementTerm>
    {
        public EarnedValueManagementTermsEntityTypeConfiguration()
        {
            this.ToTable("EarnedValueManagementTerms", "Construction");
            this.HasKey<int>(m => m.EarnedValueManagementTermId);
        }
    }
    public class EarnedValuePerProjectEntityTypeConfiguration : EntityTypeConfiguration<EarnedValuePerProject>
    {
        public EarnedValuePerProjectEntityTypeConfiguration()
        {
            this.ToTable("EarnedValuePerProject", "Construction");
            this.HasKey<int>(e => e.EarnedValuePerProjectId);

            this.Property(d => d.BAC).HasPrecision(18, 4);
            this.Property(d => d.BCWSRaw).HasPrecision(18, 4);
            this.Property(d => d.BCWS).HasPrecision(18, 4);
            this.Property(d => d.BCWPRaw).HasPrecision(18, 4);
            this.Property(d => d.BCWP).HasPrecision(18, 4);
            this.Property(d => d.ACWPRaw).HasPrecision(18, 4);
            this.Property(d => d.ACWP).HasPrecision(18, 4);
            this.Property(d => d.PCP).HasPrecision(18, 4);
            this.Property(d => d.PCA).HasPrecision(18, 4);
            this.Property(d => d.CV).HasPrecision(18, 4);
            this.Property(d => d.CVPercentage).HasPrecision(18, 4);
            this.Property(d => d.SV).HasPrecision(18, 4);
            this.Property(d => d.SVPercentage).HasPrecision(18, 4);
            this.Property(d => d.CPI).HasPrecision(18, 4);
            this.Property(d => d.SPI).HasPrecision(18, 4);
            this.Property(d => d.CR).HasPrecision(18, 4);
            this.Property(d => d.TCPI).HasPrecision(18, 4);
            this.Property(d => d.TSPI).HasPrecision(18, 4);
            this.Property(d => d.EAC).HasPrecision(18, 4);
            this.Property(d => d.VAC).HasPrecision(18, 4);
            this.Property(d => d.ETC).HasPrecision(18, 4);
            this.Property(d => d.ISAC).HasPrecision(18, 4);
        }
    }
    public class EarnedValuePerItemEntityTypeConfiguration : EntityTypeConfiguration<EarnedValuePerItem>
    {
        public EarnedValuePerItemEntityTypeConfiguration()
        {
            this.ToTable("EarnedValuePerItem", "Construction");
            this.HasKey<int>(e => e.EarnedValuePerItemId);
            this.Property(d => d.BAC).HasPrecision(18, 4);
            this.Property(d => d.BCWS).HasPrecision(18, 4);
            this.Property(d => d.BCWP).HasPrecision(18, 4);
            this.Property(d => d.ACWP).HasPrecision(18, 4);
            this.Property(d => d.PCP).HasPrecision(18, 4);
            this.Property(d => d.PCA).HasPrecision(18, 4);
            this.Property(d => d.CV).HasPrecision(18, 4);
            this.Property(d => d.CVPercentage).HasPrecision(18, 4);
            this.Property(d => d.SV).HasPrecision(18, 4);
            this.Property(d => d.SVPercentage).HasPrecision(18, 4);
            this.Property(d => d.CPI).HasPrecision(18, 4);
            this.Property(d => d.SPI).HasPrecision(18, 4);
            this.Property(d => d.CR).HasPrecision(18, 4);
            this.Property(d => d.TCPI).HasPrecision(18, 4);
            this.Property(d => d.TSPI).HasPrecision(18, 4);
            this.Property(d => d.EAC).HasPrecision(18, 4);
            this.Property(d => d.VAC).HasPrecision(18, 4);
            this.Property(d => d.ETC).HasPrecision(18, 4);
            this.Property(d => d.ISAC).HasPrecision(18, 4);
        }
    }
    public class EarnedValueManagementTermDetailEntityTypeConfiguration : EntityTypeConfiguration<EarnedValueManagementTermDetail>
    {
        public EarnedValueManagementTermDetailEntityTypeConfiguration()
        {
            this.ToTable("EarnedValueManagementTermDetail", "Construction");
            this.HasKey<int>(e => e.EarnedValueManagementTermDetailId);
        }
    }
    public class EVMTermOperandComponentEntityTypeConfiguration : EntityTypeConfiguration<EVMTermOperandComponent>
    {
        public EVMTermOperandComponentEntityTypeConfiguration()
        {
            this.ToTable("EVMTermOperandComponent", "Construction");
            this.HasKey<int>(e => e.EVMTermOperandComponentId);
        }
    }
    public class EarnedValueManagementTermFormulaConstantEntityTypeConfiguration : EntityTypeConfiguration<EarnedValueManagementTermFormulaConstant>
    {
        public EarnedValueManagementTermFormulaConstantEntityTypeConfiguration()
        {
            this.ToTable("EarnedValueManagementTermFormulaConstants", "Construction");
            this.HasKey<int>(e => e.EarnedValueManagementTermFormulaConstantId);
        }
    }
    public class EarnedValueManagementPeriodEntityTypeConfiguration : EntityTypeConfiguration<EarnedValueManagementPeriod>
    {
        public EarnedValueManagementPeriodEntityTypeConfiguration()
        {
            this.ToTable("EarnedValueManagementPeriods", "Construction");
            this.HasKey<int>(e => e.EarnedValueManagementPeriodId);
        }
    }
    #endregion
    public class ConstructionPeriodEntityTypeConfiguration : EntityTypeConfiguration<ConstructionPeriod>
    {
        public ConstructionPeriodEntityTypeConfiguration()
        {
            this.ToTable("ConstructionPeriod", "Construction");
            this.HasKey<int>(cp => cp.ConstructionPeriodId);
        }
    }
    public class ConstructionSubPeriodEntityTypeConfiguration : EntityTypeConfiguration<ConstructionSubPeriod>
    {
        public ConstructionSubPeriodEntityTypeConfiguration()
        {
            this.ToTable("ConstructionSubPeriod", "Construction");
            this.HasKey<int>(cp => cp.ConstructionSubPeriodId);
        }
    }
    #region Delay Analysis
    public class DelayTypeEntityTypeConfiguration : EntityTypeConfiguration<DelayType>
    {
        public DelayTypeEntityTypeConfiguration()
        {
            this.ToTable("DelayTypes", "Construction");
            this.HasKey<int>(dt => dt.DelayTypeId);
        }
    }
    public class WorkDelayEntityTypeConfiguration : EntityTypeConfiguration<WorkDelay>
    {
        public WorkDelayEntityTypeConfiguration()
        {
            this.ToTable("WorkDelay", "Construction");
            this.HasKey<int>(dt => dt.WorkDelayId);
        }
    }
    public class DelayAnalysisTaskEntityTypeConfiguration : EntityTypeConfiguration<DelayAnalysisTask>
    {
        public DelayAnalysisTaskEntityTypeConfiguration()
        {
            this.ToTable("DelayAnalysisTask", "Construction");
            this.HasKey<int>(dt => dt.DelayAnalysisTaskId);
        }
    }
    public class DelayTaskDependencyEntityTypeConfiguration : EntityTypeConfiguration<DelayTaskDependency>
    {
        public DelayTaskDependencyEntityTypeConfiguration()
        {
            this.ToTable("DelayTaskDependency", "Construction");
            this.HasKey<int>(dt => dt.DelayTaskDependencyId);
        }
    }
    #endregion
    #region Project Performance Evaluation
    public class PEVCostSettingEntityTypeConfiguration : EntityTypeConfiguration<PEVCostSetting>
    {
        public PEVCostSettingEntityTypeConfiguration()
        {
            this.ToTable("PEVCostSetting", "Construction");
            this.HasKey<int>(mp => mp.PEVCostSettingId);
            this.Property(d => d.Value).HasPrecision(18, 7);
        }
    }
    public class PerformanceEvaluationComponentWeightEntityTypeConfiguration : EntityTypeConfiguration<PerformanceEvaluationComponentWeight>
    {
        public PerformanceEvaluationComponentWeightEntityTypeConfiguration()
        {
            this.ToTable("PerformanceEvaluationComponentWeight", "Construction");
            this.HasKey<int>(mp => mp.PerformanceEvaluationComponentWeightId);
        }
    }
    public class MonthlyProjectProgressEntityTypeConfiguration : EntityTypeConfiguration<MonthlyProjectProgress>
    {
        public MonthlyProjectProgressEntityTypeConfiguration()
        {
            this.ToTable("MonthlyProjectProgress", "Construction");
            this.HasKey<int>(mp => mp.MonthlyProjectProgressId);
        }
    }
    public class MonthlyProjectQualityEntityTypeConfiguration : EntityTypeConfiguration<MonthlyProjectQuality>
    {
        public MonthlyProjectQualityEntityTypeConfiguration()
        {
            this.ToTable("MonthlyProjectQuality", "Construction");
            this.HasKey<int>(mp => mp.MonthlyProjectQualityId);
        }
    }
    public class QualityPlanningEntityTypeConfiguration : EntityTypeConfiguration<QualityPlanning>
    {
        public QualityPlanningEntityTypeConfiguration()
        {
            this.ToTable("QualityPlanning", "Construction");
            this.HasKey<int>(mp => mp.QualityPlanningId);
        }
    }
    public class QualityAssuranceEntityTypeConfiguration : EntityTypeConfiguration<QualityAssurance>
    {
        public QualityAssuranceEntityTypeConfiguration()
        {
            this.ToTable("QualityAssurance", "Construction");
            this.HasKey<int>(mp => mp.QualityAssuranceId);
        }
    }
    public class QualityControlEntityTypeConfiguration : EntityTypeConfiguration<QualityControl>
    {
        public QualityControlEntityTypeConfiguration()
        {
            this.ToTable("QualityControl", "Construction");
            this.HasKey<int>(mp => mp.QualityControlId);
        }
    }
    public class MonthlyProjectQualityComplaintEntityTypeConfiguration : EntityTypeConfiguration<MonthlyProjectQualityComplaint>
    {
        public MonthlyProjectQualityComplaintEntityTypeConfiguration()
        {
            this.ToTable("MonthlyProjectQualityComplaints", "Construction");
            this.HasKey<int>(mp => mp.MonthlyProjectQualityComplaintId);
        }
    }
    public class PeriodMappingEntityTypeConfiguration : EntityTypeConfiguration<PeriodMapping>
    {
        public PeriodMappingEntityTypeConfiguration()
        {
            this.ToTable("PeriodMapping", "Construction");
            this.HasKey<int>(mp => mp.PeriodMappingId);
        }
    }
    public class MonthlyProjectCostEntityTypeConfiguration : EntityTypeConfiguration<MonthlyProjectCost>
    {
        public MonthlyProjectCostEntityTypeConfiguration()
        {
            this.ToTable("MonthlyProjectCost", "Construction");
            this.HasKey<int>(mp => mp.MonthlyProjectCostId);
        }
    }
    public class MonthlyProjectPaymentEntityTypeConfiguration : EntityTypeConfiguration<MonthlyProjectPayment>
    {
        public MonthlyProjectPaymentEntityTypeConfiguration()
        {
            this.ToTable("MonthlyProjectPayment", "Construction");
            this.HasKey<int>(mp => mp.MonthlyProjectPaymentId);
        }
    }
    public class MonthlyProjectPlanningEntityTypeConfiguration : EntityTypeConfiguration<MonthlyProjectPlanning>
    {
        public MonthlyProjectPlanningEntityTypeConfiguration()
        {
            this.ToTable("MonthlyProjectPlanning", "Construction");
            this.HasKey<int>(mp => mp.MonthlyProjectPlanningId);
        }
    }
    public class RScheduleSubmissionAcToOperationManualEntityTypeConfiguration : EntityTypeConfiguration<RScheduleSubmissionAcToOperationManual>
    {
        public RScheduleSubmissionAcToOperationManualEntityTypeConfiguration()
        {
            this.ToTable("RScheduleSubmissionAcToOperationManual", "Construction");
            this.HasKey<int>(mp => mp.RScheduleSubmissionAcToOperationManualId);
        }
    }
    public class RScheduleSubmissionAcToContractEntityTypeConfiguration : EntityTypeConfiguration<RScheduleSubmissionAcToContract>
    {
        public RScheduleSubmissionAcToContractEntityTypeConfiguration()
        {
            this.ToTable("RScheduleSubmissionAcToContract", "Construction");
            this.HasKey<int>(mp => mp.RScheduleSubmissionAcToContractId);
        }
    }
    public class ProjectPnanningCompletenessEntityTypeConfiguration : EntityTypeConfiguration<ProjectPnanningCompleteness>
    {
        public ProjectPnanningCompletenessEntityTypeConfiguration()
        {
            this.ToTable("ProjectPnanningCompleteness", "Construction");
            this.HasKey<int>(mp => mp.ProjectPnanningCompletenessId);
        }
    }
    public class ProjectPlanningQualityAndAcceptanceEntityTypeConfiguration : EntityTypeConfiguration<ProjectPlanningQualityAndAcceptance>
    {
        public ProjectPlanningQualityAndAcceptanceEntityTypeConfiguration()
        {
            this.ToTable("ProjectPlanningQualityAndAcceptance", "Construction");
            this.HasKey<int>(mp => mp.ProjectPlanningQualityAndAcceptanceId);
        }
    }
    public class MonthlyProjectClaimEntityTypeConfiguration : EntityTypeConfiguration<MonthlyProjectClaim>
    {
        public MonthlyProjectClaimEntityTypeConfiguration()
        {
            this.ToTable("MonthlyProjectClaim", "Construction");
            this.HasKey<int>(mp => mp.MonthlyProjectClaimId);
        }
    }
    public class ContempraryRecordEntityTypeConfiguration : EntityTypeConfiguration<ContempraryRecord>
    {
        public ContempraryRecordEntityTypeConfiguration()
        {
            this.ToTable("ContempraryRecord", "Construction");
            this.HasKey<int>(mp => mp.ContempraryRecordId);
        }
    }
    public class RequirementForMonthlyClaimsEntityTypeConfiguration : EntityTypeConfiguration<RequirementForMonthlyClaims>
    {
        public RequirementForMonthlyClaimsEntityTypeConfiguration()
        {
            this.ToTable("RequirementForMonthlyClaims", "Construction");
            this.HasKey<int>(mp => mp.RequirementForMonthlyClaimsId);
        }
    }
    public class AnalysisTechniqueApplicationForMonthlyClaimsEntityTypeConfiguration : EntityTypeConfiguration<AnalysisTechniqueApplicationForMonthlyClaims>
    {
        public AnalysisTechniqueApplicationForMonthlyClaimsEntityTypeConfiguration()
        {
            this.ToTable("AnalysisTechniqueApplicationForMonthlyClaims", "Construction");
            this.HasKey<int>(mp => mp.AnalysisTechniqueApplicationForMonthlyClaimsId);
        }
    }
    public class ApprovalStatusForClaimsRespondedEntityTypeConfiguration : EntityTypeConfiguration<ApprovalStatusForClaimsResponded>
    {
        public ApprovalStatusForClaimsRespondedEntityTypeConfiguration()
        {
            this.ToTable("ApprovalStatusForClaimsResponded", "Construction");
            this.HasKey<int>(mp => mp.ApprovalStatusForClaimsRespondedId);
        }
    }
    public class MonthlyProjectReportEntityTypeConfiguration : EntityTypeConfiguration<MonthlyProjectReport>
    {
        public MonthlyProjectReportEntityTypeConfiguration()
        {
            this.ToTable("MonthlyProjectReport", "Construction");
            this.HasKey<int>(mp => mp.MonthlyProjectReportId);
        }
    }
    public class MonthlyProjectReportCompletnessComponentEntityTypeConfiguration : EntityTypeConfiguration<MonthlyProjectReportCompletnessComponent>
    {
        public MonthlyProjectReportCompletnessComponentEntityTypeConfiguration()
        {
            this.ToTable("MonthlyProjectReportCompletnessComponent", "Construction");
            this.HasKey<int>(mp => mp.MonthlyProjectReportCompletnessComponentId);
        }
    }
    public class MonthlyProjectReportNarrativeEntityTypeConfiguration : EntityTypeConfiguration<MonthlyProjectReportNarrative>
    {
        public MonthlyProjectReportNarrativeEntityTypeConfiguration()
        {
            this.ToTable("MonthlyProjectReportNarrative", "Construction");
            this.HasKey<int>(mp => mp.MonthlyProjectReportNarrativeId);
        }
    }
    public class MonthlyProjectCommunicationEntityTypeConfiguration : EntityTypeConfiguration<MonthlyProjectCommunication>
    {
        public MonthlyProjectCommunicationEntityTypeConfiguration()
        {
            this.ToTable("MonthlyProjectCommunication", "Construction");
            this.HasKey<int>(mp => mp.MonthlyProjectCommunicationId);
        }
    }
    public class MonthlyProjectCommunicationWithinProjectEntityTypeConfiguration : EntityTypeConfiguration<MonthlyProjectCommunicationWithinProject>
    {
        public MonthlyProjectCommunicationWithinProjectEntityTypeConfiguration()
        {
            this.ToTable("MonthlyProjectCommunicationWithinProject", "Construction");
            this.HasKey<int>(mp => mp.MonthlyProjectCommunicationWithinProjectId);
        }
    }
    public class MonthlyProjectCommunicationWithHeadOfficeEntityTypeConfiguration : EntityTypeConfiguration<MonthlyProjectCommunicationWithHeadOffice>
    {
        public MonthlyProjectCommunicationWithHeadOfficeEntityTypeConfiguration()
        {
            this.ToTable("MonthlyProjectCommunicationWithHeadOffice", "Construction");
            this.HasKey<int>(mp => mp.MonthlyProjectCommunicationWithHeadOfficeId);
        }
    }
    public class MonthlyProjectCommunicationWithConsultantEntityTypeConfiguration : EntityTypeConfiguration<MonthlyProjectCommunicationWithConsultant>
    {
        public MonthlyProjectCommunicationWithConsultantEntityTypeConfiguration()
        {
            this.ToTable("MonthlyProjectCommunicationWithConsultant", "Construction");
            this.HasKey<int>(mp => mp.MonthlyProjectCommunicationWithConsultantId);
        }
    }
    public class MonthlyProjectPerformanceModifierEntityTypeConfiguration : EntityTypeConfiguration<MonthlyProjectPerformanceModifier>
    {
        public MonthlyProjectPerformanceModifierEntityTypeConfiguration()
        {
            this.ToTable("MonthlyProjectPerformanceModifier", "Construction");
            this.HasKey<int>(mp => mp.MonthlyProjectPerformanceModifierId);
        }
    }
    public class PerformanceModifierHealthAndSafetyEntityTypeConfiguration : EntityTypeConfiguration<PerformanceModifierHealthAndSafety>
    {
        public PerformanceModifierHealthAndSafetyEntityTypeConfiguration()
        {
            this.ToTable("PerformanceModifierHealthAndSafety", "Construction");
            this.HasKey<int>(mp => mp.PerformanceModifierHealthAndSafetyId);
        }
    }
    public class PerformanceModifierBudgetRequestEntityTypeConfiguration : EntityTypeConfiguration<PerformanceModifierBudgetRequest>
    {
        public PerformanceModifierBudgetRequestEntityTypeConfiguration()
        {
            this.ToTable("PerformanceModifierBudgetRequest", "Construction");
            this.HasKey<int>(mp => mp.PerformanceModifierBudgetRequestId);
        }
    }
    public class PerformanceModifierBudgetUtilizationEntityTypeConfiguration : EntityTypeConfiguration<PerformanceModifierBudgetUtilization>
    {
        public PerformanceModifierBudgetUtilizationEntityTypeConfiguration()
        {
            this.ToTable("PerformanceModifierBudgetUtilization", "Construction");
            this.HasKey<int>(mp => mp.PerformanceModifierBudgetUtilizationId);
        }
    }
    public class PerformanceModifierMaterialUtilizationEntityTypeConfiguration : EntityTypeConfiguration<PerformanceModifierMaterialUtilization>
    {
        public PerformanceModifierMaterialUtilizationEntityTypeConfiguration()
        {
            this.ToTable("PerformanceModifierMaterialUtilization", "Construction");
            this.HasKey<int>(mp => mp.PerformanceModifierMaterialUtilizationId);
        }
    }
    public class PerformanceModifierSubContractAdministrationEntityTypeConfiguration : EntityTypeConfiguration<PerformanceModifierSubContractAdministration>
    {
        public PerformanceModifierSubContractAdministrationEntityTypeConfiguration()
        {
            this.ToTable("PerformanceModifierSubContractAdministration", "Construction");
            this.HasKey<int>(mp => mp.PerformanceModifierSubContractAdministrationId);
        }
    }
    #endregion
    #region Project Payments
    public class ProjectAdvancePaymentEntityTypeConfiguration : EntityTypeConfiguration<ProjectAdvancePayment>
    {
        public ProjectAdvancePaymentEntityTypeConfiguration()
        {
            this.ToTable("ProjectAdvancePayments", "Construction");
            this.HasKey<int>(cp => cp.ProjectAdvancePaymentId);
        }
    }
    public class ProjectPaymentAttributeEntityTypeConfiguration : EntityTypeConfiguration<ProjectPaymentAttribute>
    {
        public ProjectPaymentAttributeEntityTypeConfiguration()
        {
            this.ToTable("ProjectPaymentAttributes", "Construction");
            this.HasKey<int>(cp => cp.ProjectPaymentAttributeId);
        }
    }
    public class WorkInProgressEntityTypeConfiguration : EntityTypeConfiguration<WorkInProgress>
    {
        public WorkInProgressEntityTypeConfiguration()
        {
            this.ToTable("WorkInProgress", "Construction");
            this.HasKey<int>(cp => cp.WorkInProgressId);
        }
    }
    public class ConsPaymentAgingPeriodEntityTypeConfiguration : EntityTypeConfiguration<ConsPaymentAgingPeriod>
    {
        public ConsPaymentAgingPeriodEntityTypeConfiguration()
        {
            this.ToTable("ConsPaymentAgingPeriod", "Construction");
            this.HasKey<int>(cp => cp.ConsPaymentAgingPeriodId);
        }
    }

    public class ConstructionCustomerAccountEntityTypeConfiguration : EntityTypeConfiguration<ConstructionCustomerAccount>
    {
        public ConstructionCustomerAccountEntityTypeConfiguration()
        {
            this.ToTable("ConstructionCustomerAccount", "Construction");
            this.HasKey<int>(cp => cp.ConstructionCustomerAccountId);
        }
    }

    public class WorkInProgressValidationEntityTypeConfiguration : EntityTypeConfiguration<WorkInProgressValidation>
    {
        public WorkInProgressValidationEntityTypeConfiguration()
        {
            this.ToTable("WorkInProgressValidation", "Construction");
            this.HasKey<int>(cp => cp.WorkInProgressValidationId);
        }
    }
    public class WorkInProgressDetailEntityTypeConfiguration : EntityTypeConfiguration<WorkInProgressDetail>
    {
        public WorkInProgressDetailEntityTypeConfiguration()
        {
            this.ToTable("WorkInProgressDetails", "Construction");
            this.HasKey<int>(cp => cp.WorkInProgressDetailId);
            this.Property(d => d.UnitPrice).HasPrecision(18, 4);
            this.Property(d => d.Quantity).HasPrecision(18, 4);
            this.Property(d => d.SubTotal).HasPrecision(18, 4);
            this.Property(d => d.TaxAmount).HasPrecision(18, 4);
            this.Property(d => d.TotalAmount).HasPrecision(18, 4);

        }
    }
    public class WorkInProgressDistributionEntityTypeConfiguration : EntityTypeConfiguration<WorkInProgressDistribution>
    {
        public WorkInProgressDistributionEntityTypeConfiguration()
        {
            this.ToTable("WorkInProgressDistribution", "Construction");
            this.HasKey<int>(cp => cp.WorkInProgressDistributionId);
            this.Property(d => d.Credit).HasPrecision(18, 4);
            this.Property(d => d.Debit).HasPrecision(18, 4);
        }
    }
    public class ReceivableInvoiceEntityTypeConfiguration : EntityTypeConfiguration<ConsReceivableInvoice>
    {
        public ReceivableInvoiceEntityTypeConfiguration()
        {
            this.ToTable("ConsReceivableInvoice", "Construction");
            this.HasKey<int>(cp => cp.ConsReceivableInvoiceId);
        }
    }
    public class ReceivableInvoiceValidationEntityTypeConfiguration : EntityTypeConfiguration<ConsReceivableInvoiceValidation>
    {
        public ReceivableInvoiceValidationEntityTypeConfiguration()
        {
            this.ToTable("ConsReceivableInvoiceValidation", "Construction");
            this.HasKey<int>(cp => cp.ConsReceivableInvoiceValidationId);
        }
    }
    public class ReceivableInvoiceDetailEntityTypeConfiguration : EntityTypeConfiguration<ConsReceivableInvoiceDetail>
    {
        public ReceivableInvoiceDetailEntityTypeConfiguration()
        {
            this.ToTable("ConsReceivableInvoiceDetail", "Construction");
            this.HasKey<int>(cp => cp.ConsReceivableInvoiceDetailId);
            this.Property(p => p.Quantity).HasPrecision(18, 4);
            this.Property(p => p.UnitPrice).HasPrecision(18, 4);
            this.Property(p => p.SubTotal).HasPrecision(18, 4);
            this.Property(p => p.TaxAmount).HasPrecision(18, 4);
            this.Property(p => p.TotalAmount).HasPrecision(18, 4);
            this.Property(p => p.Retention).HasPrecision(18, 4);
            this.Property(p => p.AdvanceRepayment).HasPrecision(18, 4);
            this.Property(p => p.Penality).HasPrecision(18, 4);
            this.Property(p => p.Correction).HasPrecision(18, 4);
            this.Property(p => p.Others).HasPrecision(18, 4);
            this.Property(p => p.NetAmount).HasPrecision(18, 4);
        }
    }
    public class ConsReceivableInvoiceDistributionEntityTypeConfiguration : EntityTypeConfiguration<ConsReceivableInvoiceDistribution>
    {
        public ConsReceivableInvoiceDistributionEntityTypeConfiguration()
        {
            this.ToTable("ConsReceivableInvoiceDistribution", "Construction");
            this.HasKey<int>(cp => cp.ConsReceivableInvoiceDistributionId);
            this.Property(p => p.Debit).HasPrecision(18, 4);
            this.Property(p => p.Credit).HasPrecision(18, 4);

        }
    }
    public class ARStandardCollectionEntityTypeConfiguration : EntityTypeConfiguration<ConsARStandardCollection>
    {
        public ARStandardCollectionEntityTypeConfiguration()
        {
            this.ToTable("ConsARStandardCollection", "Construction");
            this.HasKey<int>(cp => cp.ConsARStandardCollectionId);
        }
    }
    public class ARStandardCollectionDetailEntityTypeConfiguration : EntityTypeConfiguration<ConsARStandardCollectionDetail>
    {
        public ARStandardCollectionDetailEntityTypeConfiguration()
        {
            this.ToTable("ConsARStandardCollectionDetail", "Construction");
            this.HasKey<int>(cp => cp.ConsARStandardCollectionDetailId);
            this.Property(p => p.ApplyAmount).HasPrecision(18, 4);
            this.Property(p => p.BalanceDue).HasPrecision(18, 4);
            this.Property(p => p.TaxAmount).HasPrecision(18, 4);
            this.Property(p => p.WithholdingTaxAmount).HasPrecision(18, 4);
            this.Property(p => p.AdvanceAmount).HasPrecision(18, 4);
            this.Property(p => p.Retention).HasPrecision(18, 4);
            

        }
    }
    public class ARStandardCollectionValidationEntityTypeConfiguration : EntityTypeConfiguration<ConsARStandardCollectionValidation>
    {
        public ARStandardCollectionValidationEntityTypeConfiguration()
        {
            this.ToTable("ConsARStandardCollectionValidation", "Construction");
            this.HasKey<int>(cp => cp.ConsARStandardCollectionValidationId);
        }
    }
    public class ConsStandardCollectionDistributionEntityTypeConfiguration : EntityTypeConfiguration<ConsStandardCollectionDistribution>
    {
        public ConsStandardCollectionDistributionEntityTypeConfiguration()
        {
            this.ToTable("ConsStandardCollectionDistribution", "Construction");
            this.HasKey<int>(cp => cp.ConsStandardCollectionDistributionId);
            this.Property(p => p.Credit).HasPrecision(18, 4);
            this.Property(p => p.Debit).HasPrecision(18, 4);
        }
    }
    #endregion
    #region project expense
    public class ProjectMaterialExpenseEntityTypeConfiguration : EntityTypeConfiguration<ProjectMaterialExpense>
    {
        public ProjectMaterialExpenseEntityTypeConfiguration()
        {
            this.ToTable("ProjectMaterialExpense", "Construction");
            this.HasKey<int>(cp => cp.ProjectMaterialExpenseId);
        }
    }

    public class ProjectDirectMaterialExpenseEntityTypeConfiguration : EntityTypeConfiguration<ProjectDirectMaterialExpense>
    {
        public ProjectDirectMaterialExpenseEntityTypeConfiguration()
        {
            this.ToTable("ProjectDirectMaterialExpense", "Construction");
            this.HasKey<int>(cp => cp.ProjectDirectMaterialExpenseId);
        }
    }
    public class ProjectIndirectMaterialExpenseEntityTypeConfiguration : EntityTypeConfiguration<ProjectIndirectMaterialExpense>
    {
        public ProjectIndirectMaterialExpenseEntityTypeConfiguration()
        {
            this.ToTable("ProjectIndirectMaterialExpense", "Construction");
            this.HasKey<int>(cp => cp.ProjectIndirectMaterialExpenseId);
        }
    }
    #endregion
    #region Price Adjustment
    public class ProjectRepresentativeElementEntityTypeConfiguration : EntityTypeConfiguration<ProjectRepresentativeElement>
    {
        public ProjectRepresentativeElementEntityTypeConfiguration()
        {
            this.ToTable("ProjectRepresentativeElement", "Construction");
            this.HasKey<int>(cp => cp.ProjectRepresentativeElementId);
        }
    }
    public class PriceAdjustmentWeightEntityTypeConfiguration : EntityTypeConfiguration<PriceAdjustmentWeight>
    {
        public PriceAdjustmentWeightEntityTypeConfiguration()
        {
            this.ToTable("PriceAdjustmentWeight", "Construction");
            this.HasKey<int>(cp => cp.PriceAdjustmentWeightId);
        }
    }
    public class ProjectPriceAdjustmentFormulaTypeEntityTypeConfiguration : EntityTypeConfiguration<ProjectPriceAdjustmentFormulaType>
    {
        public ProjectPriceAdjustmentFormulaTypeEntityTypeConfiguration()
        {
            this.ToTable("ProjectPriceAdjustmentFormulaType", "Construction");
            this.HasKey<int>(cp => cp.ProjectPriceAdjustmentFormulaTypeId);
        }
    }
    public class PriceAdjustmentWeightItemEntityTypeConfiguration : EntityTypeConfiguration<PriceAdjustmentWeightItem>
    {
        public PriceAdjustmentWeightItemEntityTypeConfiguration()
        {
            this.ToTable("PriceAdjustmentWeightItem", "Construction");
            this.HasKey<int>(cp => cp.PriceAdjustmentWeightItemId);
        }
    }
    public class RepresentativeElementWeightRangeEntityTypeConfiguration : EntityTypeConfiguration<RepresentativeElementWeightRange>
    {
        public RepresentativeElementWeightRangeEntityTypeConfiguration()
        {
            this.ToTable("RepresentativeElementWeightRange", "Construction");
            this.HasKey<int>(cp => cp.RepresentativeElementWeightRangeId);
        }
    }
    public class RepresentativeElementCostInBillEntityTypeConfiguration : EntityTypeConfiguration<RepresentativeElementCostInBill>
    {
        public RepresentativeElementCostInBillEntityTypeConfiguration()
        {
            this.ToTable("RepresentativeElementCostInBill", "Construction");
            this.HasKey<int>(cp => cp.RepresentativeElementCostInBillId);
        }
    }

    #endregion
    #region Project
    public class WorkItemPerProjectEntityTypeConfiguration : EntityTypeConfiguration<WorkItemPerProject>
    {
        public WorkItemPerProjectEntityTypeConfiguration()
        {
            this.ToTable("WorkItemPerProject", "Construction");
            this.HasKey<int>(o => o.WorkItemPerProjectId);
            this.Property(d => d.TotalUnitCost).HasPrecision(18, 4);
            this.Property(d => d.TotalQuantityOfWork).HasPrecision(18, 4);
            this.Property(d => d.DirectCost).HasPrecision(18, 4);
            this.Property(d => d.MaterialCostPerUnit).HasPrecision(18, 4);
            this.Property(d => d.LaborCostPerUnit).HasPrecision(18, 4);
            this.Property(d => d.EquipmentCostPerUnit).HasPrecision(18, 4);
            this.Property(d => d.MaterialCost).HasPrecision(18, 4);
            this.Property(d => d.LabourOutPut).HasPrecision(18, 4);
            this.Property(d => d.EquipmentOutput).HasPrecision(18, 4);
            this.Property(d => d.LabourHourlyOutPut).HasPrecision(18, 4);
            this.Property(d => d.ProfitCost).HasPrecision(18, 4);
            this.Property(d => d.OverHeadCost).HasPrecision(18, 4);

            this.Property(d => d.TotalUnitCostIncludingTransport).HasPrecision(18, 4);
            this.Property(d => d.TransportCostRate).HasPrecision(18, 4);
            this.Property(d => d.CalculatedTransportCost).HasPrecision(18, 4);
            this.Property(d => d.ThicknessFactor).HasPrecision(18, 4);
        }
    }
    public class GenericWorkItemPerProjectEntityTypeConfiguration : EntityTypeConfiguration<GenericWorkItemPerProject>
    {
        public GenericWorkItemPerProjectEntityTypeConfiguration()
        {
            this.ToTable("GenericWorkItemPerProject", "Construction");
            this.HasKey<int>(o => o.GenericWorkItemPerProjectId);
            this.Property(d => d.TotalUnitCost).HasPrecision(18, 4);
            this.Property(d => d.TotalQuantityOfWork).HasPrecision(18, 4);
            this.Property(d => d.DirectCost).HasPrecision(18, 4);
            this.Property(d => d.MaterialCostPerUnit).HasPrecision(18, 4);
            this.Property(d => d.LaborCostPerUnit).HasPrecision(18, 4);
            this.Property(d => d.EquipmentCostPerUnit).HasPrecision(18, 4);
            this.Property(d => d.MaterialCost).HasPrecision(18, 4);
            this.Property(d => d.LabourOutPut).HasPrecision(18, 4);
            this.Property(d => d.EquipmentOutput).HasPrecision(18, 4);
            this.Property(d => d.LabourHourlyOutPut).HasPrecision(18, 4);
            this.Property(d => d.ProfitCost).HasPrecision(18, 4);
            this.Property(d => d.OverHeadCost).HasPrecision(18, 4);

            this.Property(d => d.TotalUnitCostIncludingTransport).HasPrecision(18, 4);
            this.Property(d => d.TransportCostRate).HasPrecision(18, 4);
            this.Property(d => d.CalculatedTransportCost).HasPrecision(18, 4);
            this.Property(d => d.ThicknessFactor).HasPrecision(18, 4);
        }
    }
    public class ProjectMaterialEntityTypeConfiguration : EntityTypeConfiguration<ProjectMaterial>
    {
        public ProjectMaterialEntityTypeConfiguration()
        {
            this.ToTable("ProjectMaterial", "Construction");
            this.HasKey<int>(o => o.ProjectMaterialId);
            this.Property(d => d.UnitPrice).HasPrecision(18, 4);
            this.Property(d => d.Quantity).HasPrecision(18, 4);
            this.Property(d => d.TotalCost).HasPrecision(18, 4);
        }
    }
    public class GenericProjectMaterialEntityTypeConfiguration : EntityTypeConfiguration<GenericProjectMaterial>
    {
        public GenericProjectMaterialEntityTypeConfiguration()
        {
            this.ToTable("GenericProjectMaterial", "Construction");
            this.HasKey<int>(o => o.GenericProjectMaterialId);
            this.Property(d => d.UnitPrice).HasPrecision(18, 4);
            this.Property(d => d.Quantity).HasPrecision(18, 4);
            this.Property(d => d.TotalCost).HasPrecision(18, 4);
        }
    }
    public class ProjectDirectLabourEntityTypeConfiguration : EntityTypeConfiguration<ProjectDirectLabour>
    {
        public ProjectDirectLabourEntityTypeConfiguration()
        {
            this.ToTable("ProjectDirectLabour", "Construction");
            this.HasKey<int>(o => o.ProjectDirectLabourId);
            this.Property(o => o.Quantity).HasPrecision(18, 4);
            this.Property(o => o.TotalCost).HasPrecision(18, 4);
            this.Property(o => o.UnitFactory).HasPrecision(18, 4);
            this.Property(o => o.IndexedHourlySalary).HasPrecision(18, 4);
            this.Property(o => o.DailyPay).HasPrecision(18, 4);
            this.Property(o => o.ManpowerUnitCost).HasPrecision(18, 4);
        }
    }
    public class GenericProjectDirectLabourEntityTypeConfiguration : EntityTypeConfiguration<GenericProjectDirectLabour>
    {
        public GenericProjectDirectLabourEntityTypeConfiguration()
        {
            this.ToTable("GenericProjectDirectLabour", "Construction");
            this.HasKey<int>(o => o.GenericProjectDirectLabourId);
            this.Property(o => o.Quantity).HasPrecision(18, 4);
            this.Property(o => o.TotalCost).HasPrecision(18, 4);
            this.Property(o => o.UnitFactory).HasPrecision(18, 4);
            this.Property(o => o.IndexedHourlySalary).HasPrecision(18, 4);
            this.Property(o => o.DailyPay).HasPrecision(18, 4);
            this.Property(o => o.ManpowerUnitCost).HasPrecision(18, 4);
        }
    }
    public class ProjectEquipmentEntityTypeConfiguration : EntityTypeConfiguration<ProjectEquipment>
    {
        public ProjectEquipmentEntityTypeConfiguration()
        {
            this.ToTable("ProjectEquipment", "Construction");
            this.HasKey<int>(o => o.ProjectEquipmentId);
            this.Property(o => o.Quantity).HasPrecision(18, 4);
            this.Property(o => o.TotalCost).HasPrecision(18, 4);
            this.Property(o => o.UnitFactory).HasPrecision(18, 4);
            this.Property(o => o.RentalCost).HasPrecision(18, 4);
        }
    }
    public class GenericProjectEquipmentEntityTypeConfiguration : EntityTypeConfiguration<GenericProjectEquipment>
    {
        public GenericProjectEquipmentEntityTypeConfiguration()
        {
            this.ToTable("GenericProjectEquipment", "Construction");
            this.HasKey<int>(o => o.GenericProjectEquipmentId);
            this.Property(o => o.Quantity).HasPrecision(18, 4);
            this.Property(o => o.TotalCost).HasPrecision(18, 4);
            this.Property(o => o.UnitFactory).HasPrecision(18, 4);
            this.Property(o => o.RentalCost).HasPrecision(18, 4);
        }
    }
    public class TakeOffPerProjectEntityTypeConfiguration : EntityTypeConfiguration<TakeOffPerProject>
    {
        public TakeOffPerProjectEntityTypeConfiguration()
        {
            this.ToTable("TakeOffPerProject", "Construction");
            this.HasKey<int>(o => o.TakeOffPerProjectId);

        }
    }
    public class TakeOffLinePerProjectEntityTypeConfiguration : EntityTypeConfiguration<TakeOffLinePerProject>
    {
        public TakeOffLinePerProjectEntityTypeConfiguration()
        {
            this.ToTable("TakeOffLinePerProject", "Construction");
            this.HasKey<int>(o => o.TakeOffLinePerProjectId);
            this.Property(o => o.Depth).HasPrecision(18, 4);
            this.Property(o => o.Width).HasPrecision(18, 4);
            this.Property(o => o.D).HasPrecision(18, 4);
            this.Property(o => o.Quantity).HasPrecision(18, 4);
            this.Property(o => o.Length).HasPrecision(18, 4);
            this.Property(o => o.No).HasPrecision(18, 4);
        }
    }

    public class TakeOffLineForKgPerProjectEntityTypeConfiguration : EntityTypeConfiguration<TakeOffLineForKgPerProject>
    {
        public TakeOffLineForKgPerProjectEntityTypeConfiguration()
        {
            this.ToTable("TakeOffLineForKgPerProject", "Construction");
            this.HasKey<int>(o => o.TakeOffLineForKgPerProjectId);
            this.Property(o => o.GrandTotal).HasPrecision(18, 4);
            this.Property(o => o.Length).HasPrecision(18, 4);
            this.Property(o => o.TotalLength).HasPrecision(18, 4);
            this.Property(o => o.TotalBar).HasPrecision(18, 4);
            this.Property(o => o.TotalWeight).HasPrecision(18, 4);
        }

    }
    public class ProjectJobTitleEntityTypeConfiguration : EntityTypeConfiguration<ProjectJobTitle>
    {
        public ProjectJobTitleEntityTypeConfiguration()
        {
            this.ToTable("ProjectJobTitle", "Construction");
            this.HasKey<int>(o => o.ProjectJobTitleId);
        }
    }
    public class UnitOfMeasurementEntityTypeConfiguration : EntityTypeConfiguration<UnitOfMeasurement>
    {
        public UnitOfMeasurementEntityTypeConfiguration()
        {
            this.ToTable("UnitOfMeasurement", "Construction");
            this.HasKey<int>(o => o.UnitOfMeasurementId);
        }
    }
    //public class ProjectTypeEntityTypeConfiguration : EntityTypeConfiguration<ProjectType>
    //{
    //    public ProjectTypeEntityTypeConfiguration()
    //    {
    //        this.ToTable("ProjectType", "Construction");
    //        this.HasKey<int>(o => o.ProjectTypeId);
    //    }
    //}
    public class ProjectTypeMappingEntityTypeConfiguration : EntityTypeConfiguration<ProjectTypeMapping>
    {
        public ProjectTypeMappingEntityTypeConfiguration()
        {
            this.ToTable("ProjectTypeMapping", "Construction");
            this.HasKey<int>(o => o.ProjectTypeMappingId);
        }
    }
    public class MaterialPriceIndexEntityTypeConfiguration : EntityTypeConfiguration<MaterialPriceIndex>
    {
        public MaterialPriceIndexEntityTypeConfiguration()
        {
            this.ToTable("MaterialPriceIndex", "Construction");
            this.HasKey<int>(o => o.MaterialPriceIndexId);
            this.Property(d => d.ApprovedCost).HasPrecision(18, 4);
            this.Property(d => d.TotalCost).HasPrecision(18, 4);

        }
    }
    public class MaterialTransportationCostEntityTypeConfiguration : EntityTypeConfiguration<MaterialTransportationCost>
    {
        public MaterialTransportationCostEntityTypeConfiguration()
        {
            this.ToTable("MaterialTransportationCost", "Construction");
            this.HasKey<int>(o => o.MaterialTransportationCostId);

            this.Property(d => d.TrackCapacityPerTrip).HasPrecision(18, 4);
            this.Property(d => d.AverageSpeedOfTrack).HasPrecision(18, 4);
            this.Property(d => d.RentalCostWithFuel).HasPrecision(18, 4);
            this.Property(d => d.DriverPerdiem).HasPrecision(18, 4);
            this.Property(d => d.AssistantDriverPerdiem).HasPrecision(18, 4);
            this.Property(d => d.RequiredTime).HasPrecision(18, 4);
            this.Property(d => d.AverageDistanceFromSourceToSite).HasPrecision(18, 4);
            this.Property(d => d.TotalCost).HasPrecision(18, 4);
            this.Property(d => d.CostPerUnit).HasPrecision(18, 4);
            this.Property(d => d.LoadingUnloadingCost).HasPrecision(18, 4);
            this.Property(d => d.TotalCostPerUnit).HasPrecision(18, 4);


        }
    }
    public class AreaWorkItemEntityTypeConfiguration : EntityTypeConfiguration<AreaWorkItem>
    {
        public AreaWorkItemEntityTypeConfiguration()
        {
            this.ToTable("AreaWorkItem", "Construction");
            this.HasKey<int>(o => o.AreaWorkItemId);
        }
    }
    public class GenericAreaWorkItemEntityTypeConfiguration : EntityTypeConfiguration<GenericAreaWorkItem>
    {
        public GenericAreaWorkItemEntityTypeConfiguration()
        {
            this.ToTable("GenericAreaWorkItem", "Construction");
            this.HasKey<int>(o => o.GenericAreaWorkItemId);
        }
    }
    public class ProjectEstimationCustomWorkItemEntityTypeConfiguration : EntityTypeConfiguration<ProjectEstimationCustomWorkItem>
    {
        public ProjectEstimationCustomWorkItemEntityTypeConfiguration()
        {
            this.ToTable("ProjectEstimationCustomWorkItem", "Construction");
            this.HasKey<int>(o => o.ProjectEstimationCustomWorkItemId);
        }
    }

    public class EstimationCustomWorkItemMaterialEntityTypeConfiguration : EntityTypeConfiguration<EstimationCustomWorkItemMaterial>
    {
        public EstimationCustomWorkItemMaterialEntityTypeConfiguration()
        {
            this.ToTable("EstimationCustomWorkItemMaterial", "Construction");
            this.HasKey<int>(o => o.EstimationCustomWorkItemMaterialId);
        }
    }
    public class EstimationCustomWorkItemEquipmentEntityTypeConfiguration : EntityTypeConfiguration<EstimationCustomWorkItemEquipment>
    {
        public EstimationCustomWorkItemEquipmentEntityTypeConfiguration()
        {
            this.ToTable("EstimationCustomWorkItemEquipment", "Construction");
            this.HasKey<int>(o => o.EstimationCustomWorkItemEquipmentId);
        }
    }
    public class EstimationCustomWorkItemDirectLaborEntityTypeConfiguration : EntityTypeConfiguration<EstimationCustomWorkItemDirectLabor>
    {
        public EstimationCustomWorkItemDirectLaborEntityTypeConfiguration()
        {
            this.ToTable("EstimationCustomWorkItemDirectLabor", "Construction");
            this.HasKey<int>(o => o.EstimationCustomWorkItemDirectLaborId);
        }
    }

    public class EstimationCustomWorkItemContentEntityTypeConfiguration : EntityTypeConfiguration<EstimationCustomWorkItemContent>
    {
        public EstimationCustomWorkItemContentEntityTypeConfiguration()
        {
            this.ToTable("EstimationCustomWorkItemContent", "Construction");
            this.HasKey<int>(o => o.EstimationCustomWorkItemContentId);
            this.Property(d => d.QuantityPerUnit).HasPrecision(18, 4);
            this.Property(d => d.UnitCost).HasPrecision(18, 4);
            this.Property(d => d.UnitCost).HasPrecision(18, 4);
        }
    }
    public class EstimationTemplateCustomWorkItemDetailEntityTypeConfiguration : EntityTypeConfiguration<EstimationTemplateCustomWorkItemDetail>
    {
        public EstimationTemplateCustomWorkItemDetailEntityTypeConfiguration()
        {
            this.ToTable("EstimationTemplateCustomWorkItemDetail", "Construction");
            this.HasKey<int>(o => o.EstimationTemplateCustomWorkItemDetailId);
            this.Property(d => d.QuantityPerUnit).HasPrecision(18, 4);
            this.Property(d => d.UnitCost).HasPrecision(18, 4);
            this.Property(d => d.TotalCost).HasPrecision(18, 4);
        }
    }
    public class ProjectEstimationAreaWorkItemEntityTypeConfiguration : EntityTypeConfiguration<ProjectEstimationAreaWorkItem>
    {
        public ProjectEstimationAreaWorkItemEntityTypeConfiguration()
        {
            this.ToTable("EstimationAreaWorkItem", "Construction");
            this.HasKey<int>(o => o.ProjectEstimationAreaWorkItemId);
        }
    }
    public class EstimationSubContractWorkItemEntityTypeConfiguration : EntityTypeConfiguration<EstimationSubContractWorkItem>
    {
        public EstimationSubContractWorkItemEntityTypeConfiguration()
        {
            this.ToTable("SubContractWorkItemEstmn", "Construction");
            this.HasKey<int>(o => o.EstimationSubContractWorkItemId);
        }
    }
    public class SubContractWorkItemEntityTypeConfiguration : EntityTypeConfiguration<SubContractWorkItem>
    {
        public SubContractWorkItemEntityTypeConfiguration()
        {
            this.ToTable("SubContractWorkItem", "Construction");
            this.HasKey<int>(o => o.SubContractWorkItemId);
        }
    }
    public class GenericSubContractWorkItemEntityTypeConfiguration : EntityTypeConfiguration<GenericSubContractWorkItem>
    {
        public GenericSubContractWorkItemEntityTypeConfiguration()
        {
            this.ToTable("GenericSubContractWorkItem", "Construction");
            this.HasKey<int>(o => o.GenericSubContractWorkItemId);
        }
    }
    public class BlockEntityTypeConfiguration : EntityTypeConfiguration<Block>
    {
        public BlockEntityTypeConfiguration()
        {
            this.ToTable("Block", "Construction");
            this.HasKey<int>(o => o.BlockId);
        }
    }
    public class BlockMappingEntityTypeConfiguration : EntityTypeConfiguration<BlockMapping>
    {
        public BlockMappingEntityTypeConfiguration()
        {
            this.ToTable("BlockMapping", "Construction");
            this.HasKey<int>(o => o.BlockMappingId);
        }
    }

    public class SubProjectTypeEntityTypeConfiguration : EntityTypeConfiguration<SubProjectType>
    {
        public SubProjectTypeEntityTypeConfiguration()
        {
            this.ToTable("SubProjectType", "Construction");
            this.HasKey<int>(o => o.SubProjectTypeId);
        }
    }
    public class SubProjectStructureTypeMappingEntityTypeConfiguration : EntityTypeConfiguration<SubProjectStructureTypeMapping>
    {
        public SubProjectStructureTypeMappingEntityTypeConfiguration()
        {
            this.ToTable("SubProjectStructureTypeMapping", "Construction");
            this.HasKey<int>(o => o.SubProjectStructureTypeMappingId);
        }
    }

    public class ConstructionItemCategoryEntityTypeConfiguration : EntityTypeConfiguration<ConstructionItemCategory>
    {
        public ConstructionItemCategoryEntityTypeConfiguration()
        {
            this.ToTable("ConstructionItemCategory", "Construction");
            this.HasKey<int>(o => o.ConstructionItemCategoryId);
        }
    }

    public class RoadEntityTypeConfiguration : EntityTypeConfiguration<Road>
    {
        public RoadEntityTypeConfiguration()
        {
            this.ToTable("Road", "Construction");
            this.HasKey<int>(o => o.RoadId);
        }
    }
    public class RoadMappingEntityTypeConfiguration : EntityTypeConfiguration<RoadMapping>
    {
        public RoadMappingEntityTypeConfiguration()
        {
            this.ToTable("RoadMapping", "Construction");
            this.HasKey<int>(o => o.RoadMappingId);
        }
    }


    public class ProjectCustomWorkItemEntityTypeConfiguration : EntityTypeConfiguration<ProjectCustomWorkItem>
    {
        public ProjectCustomWorkItemEntityTypeConfiguration()
        {
            this.ToTable("ProjectCustomWorkItem", "Construction");
            this.HasKey<int>(o => o.ProjectCustomWorkItemId);
        }
    }
    public class GenericProjectCustomWorkItemEntityTypeConfiguration : EntityTypeConfiguration<GenericProjectCustomWorkItem>
    {
        public GenericProjectCustomWorkItemEntityTypeConfiguration()
        {
            this.ToTable("GenericProjectCustomWorkItem", "Construction");
            this.HasKey<int>(o => o.GenericProjectCustomWorkItemId);
        }
    }
    public class ProjectCustomWorkItemDetailEntityTypeConfiguration : EntityTypeConfiguration<ProjectCustomWorkItemDetail>
    {
        public ProjectCustomWorkItemDetailEntityTypeConfiguration()
        {
            this.ToTable("ProjectCustomWorkItemDetail", "Construction");
            this.HasKey<int>(o => o.ProjectCustomWorkItemDetailId);
        }
    }
    public class GenericProjectCustomWorkItemDetailEntityTypeConfiguration : EntityTypeConfiguration<GenericProjectCustomWorkItemDetail>
    {
        public GenericProjectCustomWorkItemDetailEntityTypeConfiguration()
        {
            this.ToTable("GenericProjectCustomWorkItemDetail", "Construction");
            this.HasKey<int>(o => o.GenericProjectCustomWorkItemDetailId);
        }
    }
    #endregion

    #region cash flow
    public class CashFlowYearEntityTypeConfiguration : EntityTypeConfiguration<CashFlowYear>
    {
        public CashFlowYearEntityTypeConfiguration()
        {
            this.ToTable("CashFlowYear", "Construction");
            this.HasKey<int>(o => o.CashFlowYearId);
        }
    }
    public class CashFlowContractDataEntityTypeConfiguration : EntityTypeConfiguration<CashFlowContractData>
    {
        public CashFlowContractDataEntityTypeConfiguration()
        {
            this.ToTable("CashFlowContractData", "Construction");
            this.HasKey<int>(o => o.CashFlowContractDataId);
        }
    }
    public class CashFlowContractDataDetailEntityTypeConfiguration : EntityTypeConfiguration<CashFlowContractDataDetail>
    {
        public CashFlowContractDataDetailEntityTypeConfiguration()
        {
            this.ToTable("CashFlowContractDataDetail", "Construction");
            this.HasKey<int>(o => o.CashFlowContractDataDetailId);
        }
    }
    public class CashFlowFinanceSourceEntityTypeConfiguration : EntityTypeConfiguration<CashFlowFinanceSource>
    {
        public CashFlowFinanceSourceEntityTypeConfiguration()
        {
            this.ToTable("CashFlowFinanceSource", "Construction");
            this.HasKey<int>(o => o.CashFlowFinanceSourceId);
        }
    }
    public class CashFlowFinanceSourceDetailEntityTypeConfiguration : EntityTypeConfiguration<CashFlowFinanceSourceDetail>
    {
        public CashFlowFinanceSourceDetailEntityTypeConfiguration()
        {
            this.ToTable("CashFlowFinanceSourceDetail", "Construction");
            this.HasKey<int>(o => o.CashFlowFinanceSourceDetailId);
        }
    }
    public class CashFlowDeductionEntityTypeConfiguration : EntityTypeConfiguration<CashFlowDeduction>
    {
        public CashFlowDeductionEntityTypeConfiguration()
        {
            this.ToTable("CashFlowDeduction", "Construction");
            this.HasKey<int>(o => o.CashFlowDeductionId);
        }
    }
    public class CashFlowDeductionDetailEntityTypeConfiguration : EntityTypeConfiguration<CashFlowDeductionDetail>
    {
        public CashFlowDeductionDetailEntityTypeConfiguration()
        {
            this.ToTable("CashFlowDeductionDetail", "Construction");
            this.HasKey<int>(o => o.CashFlowDeductionDetailId);
        }
    }
    public class NetOperationCashFlowEntityTypeConfiguration : EntityTypeConfiguration<NetOperationCashFlow>
    {
        public NetOperationCashFlowEntityTypeConfiguration()
        {
            this.ToTable("NetOperationCashFlow", "Construction");
            this.HasKey<int>(o => o.NetOperationCashFlowId);
        }
    }
    public class NetOperationCashFlowDetailEntityTypeConfiguration : EntityTypeConfiguration<NetOperationCashFlowDetail>
    {
        public NetOperationCashFlowDetailEntityTypeConfiguration()
        {
            this.ToTable("NetOperationCashFlowDetail", "Construction");
            this.HasKey<int>(o => o.NetOperationCashFlowDetailId);
        }
    }
    #endregion

}
