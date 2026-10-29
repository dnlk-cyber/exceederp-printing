using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Construction
{
    public class ProjectInformation : UndoableUserOperationBase
    {
        [Key]
        public int ProjectInformationId { get; set; }
        [DisplayName("Project Name")]
        [StringLength(200)]
        public string Name { get; set; }
        [DisplayName("Project Code")]
        [StringLength(50)]
        public string Code { get; set; }
        [DisplayName("Project Category")]
        public ProjectCategory Category { get; set; }

        [DisplayName("Project Location")]
        public int BranchId { get; set; }
        //public virtual Branch Branch { get; set; }
        [DisplayName("Contract Amount")]
        public decimal ContractAmount { get; set; }
        [DisplayName("Project Scope")]

        public string ProjectScope { get; set; }
        [DisplayName("Project Manager")]

        public int? EmployeeId { get; set; }
        //public virtual Person_Employee PersonEmployee { get; set; }
        [DisplayName("Client")]
        [StringLength(200)]
        public string Client { get; set; }
        [DisplayName("Consultant")]
        [StringLength(200)]
        public string Consultant { get; set; }
        [Display(Name = "Project Type")]
        public int ProjectTypeId { get; set; }
        [DisplayName("Type")]
        public AvailableProjectTypes ProjectType { get; set; }
        //public virtual ProjectType ProjectType { get; set; }
        //dates
        public ProjectStatus ProjectStatus { get; set; }
        public virtual ICollection<ProjectConstraintsAndRisk> ProjectConstraintsAndRisk { get; set; }
        public virtual ICollection<ProjectCalendar> ProjectCalander { get; set; }

        public virtual ICollection<ProjectWorkBreakDown> ProjectWorkBreakDown { get; set; }
        public virtual ICollection<ProjectDefineBudget> ProjectDefineBudget { get; set; }
        public virtual ICollection<ProjectMaterialRequest> ProjectMaterialRequest { get; set; }
        public virtual ICollection<ProjectMaterialIssued> ProjectMaterialIssued { get; set; }
        public virtual ICollection<ProjectMaterialDelivery> ProjectMaterialDelivery { get; set; }
        public virtual ICollection<ProjectEquipmentUtilization> ProjectEquipmentUtilization { get; set; }
        public virtual ICollection<ProjectHumanResourceUtilization> ProjectHumanResourceUtilization { get; set; }
        public virtual ICollection<ProjectEstimationBoqBreakdown> ProjectEstimationBoqBreakdown { get; set; }
        public virtual ICollection<WorkItemPerProject> WorkItemPerProject { get; set; }


    }
    public class ProjectStatusEditViewModel
    {
        public int ProjectInformationId { get; set; }

        public string ProjectName { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public decimal ContractAmount { get; set; }
        [Display(Name = "Status")]
        public ProjectStatus ProjectStatus { get; set; }
    }
    public class ProjectGeneralInformationViewModel
    {
        public int ProjectInformationId { get; set; }
        [DisplayName("Project Name")]
        public string Name { get; set; }
        [DisplayName("Project Code")]
        [StringLength(50)]
        public string Code { get; set; }

        [DisplayName("Project Location")]
        public int BranchId { get; set; }
        //public virtual Branch Branch { get; set; }
        [DisplayName("Contract Amount")]
        public decimal ContractAmount { get; set; }

        [DisplayName("Project Scope")]
        public string ProjectScope { get; set; }

        [DisplayName("Client")]
        public string Client { get; set; }

        [DisplayName("Consultant")]
        public string Consultant { get; set; }
        [DisplayName("Type")]
        public AvailableProjectTypes ProjectType { get; set; }
        [Display(Name = "Status")]
        public string ProjectStatus { get; set; }
        public string DateCreated { get; set; }
        public string LastModified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
    public class ProjectSpecificSettingAttribute : TrackUserOperation
    {
        public int ProjectSpecificSettingAttributeId { get; set; }
        public int ProjectInformationId { get; set; }
        public ProjectSettingAttributes Attribute { get; set; }
        public decimal Value { get; set; }
        public virtual ProjectInformation Project { get; set; }
    }
    public class ProjectConstraintsAndRisk : TrackUserOperation
    {
        [Key]
        public int ProjectConstraintsAndRiskId { get; set; }

        public string Assumptions { get; set; }
        public string Constraints { get; set; }
        public string RiskDescription { get; set; }
        public RiskStatus RiskStatus { get; set; }
        public RiskImpact RiskImpact { get; set; }
        public RiskProbablity RiskProbablity { get; set; }
        public int ProjectInformationId { get; set; }
        public virtual ProjectInformation Project { get; set; }
    }
    public class ProjectCalendar : TrackUserOperation
    {
        [Key]
        public int ProjectCalanderId { get; set; }

        [Required]
        public Days Day { get; set; }

        [Display(Name = "Type")]
        public WorkingHourType WorkingHourType { get; set; }

        [Display(Name = "From")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime FromHour { get; set; }

        [Display(Name = "To")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime ToHour { get; set; }
        public int ProjectInformationId { get; set; }
        public virtual ProjectInformation Project { get; set; }
        public bool Selected { get; set; }

        // reference to the General Calendar setting
        public int SettingCalendarId { get; set; }

    }
    public class ConstructionHoliday : TrackUserOperation
    {
        public int ConstructionHolidayId { get; set; }
        public int ProjectInformationId { get; set; }
        // ref to the general Holiday setting in HR
        public int HolidayId { get; set; }

        [Display(Name = "Year")]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Display(Name = "Date")]
        public string DateHoAmhaic { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Remark")]
        public string Remark { get; set; }

        public virtual ProjectInformation ProjectInformation { get; set; }
    }

    public class ProjectCalendarViewModel : TrackUserOperation
    {
        public Days Day { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime FromHour { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime ToHour { get; set; }
        public int ProjectInformationId { get; set; }
        public int ProjectCalanderId { get; set; }
        public int SettingCalendarId { get; set; }
        public bool Selected { get; set; }

    }
    public class SettingCalendar : TrackUserOperation
    {
        [Key]
        public int SettingCalendarId { get; set; }

        [Required]
        public Days Day { get; set; }

        [Display(Name = "Type")]
        public WorkingHourType WorkingHourType { get; set; }
        [Display(Name = "From")]
        //[DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime FromHour { get; set; }

        [Display(Name = "To")]
        //[DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime ToHour { get; set; }

    }
    public class LineDate : TrackUserOperation

    {
        [Key]
        public int LineDateId { get; set; }
        public int ProjectInformationId { get; set; }
        public virtual ProjectInformation Project { get; set; }
        [DisplayName("Contract Award Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime AwardDate { get; set; }
        [DisplayName("Contract Period")]
        public string ContractPeriod { get; set; }
        [DisplayName("Contract Signing Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime ContractSigningDate { get; set; }
        [DisplayName("Site Hand Over Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime SiteHandOverDate { get; set; }
        [DisplayName("Estimated Start Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime EstimatedStartDate { get; set; }
        [DisplayName("Estimated End Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime EstimatedEndDate { get; set; }
        [DisplayName("Completion Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime CompletionDate { get; set; }
        [DisplayName("Liability Period")]
        public int PeriodId { get; set; }
        public virtual Period Period { get; set; }
    }
    public class AreaWorkItem : TrackUserOperation
    {
        public int AreaWorkItemId { get; set; }
        [DisplayName("Category")]
        public int WorkCategoryId { get; set; }
        public virtual WorkCategory WorkCategory { get; set; }
        public string WorkItemName { get; set; }
        [DisplayName("W")]
        public decimal Width { get; set; }
        [DisplayName("H")]
        public decimal Height { get; set; }
        public decimal Area { get; set; }
        [DisplayName("Price/m2")]
        public decimal Price { get; set; }
        public decimal MaterialCost { get; set; }
        public decimal TransportCost { get; set; }
        public decimal CylinderLock { get; set; }
        public decimal DirectCost { get; set; }
        [DisplayName("Over Head + Profit")]
        public decimal OverHeadProfitCost { get; set; }
        public int ProjectInformationId { get; set; }
        public virtual ProjectInformation ProjectInformation { get; set; }
        public int BlockId { get; set; }
        public string ItemCode { get; set; }
        public int GenericAreaWorkItemId { get; set; }

    }
    public class GenericAreaWorkItem : TrackUserOperation
    {
        [Key]
        public int GenericAreaWorkItemId { get; set; }
        [DisplayName("Category")]
        public int WorkCategoryId { get; set; }
        public virtual WorkCategory WorkCategory { get; set; }
        public string WorkItemName { get; set; }
        [DisplayName("W")]
        public decimal Width { get; set; }
        [DisplayName("H")]
        public decimal Height { get; set; }
        public decimal Area { get; set; }
        [DisplayName("Price/m2")]
        public decimal Price { get; set; }
        public decimal MaterialCost { get; set; }
        public decimal TransportCost { get; set; }
        public decimal CylinderLock { get; set; }
        public decimal DirectCost { get; set; }
        [DisplayName("Over Head + Profit")]
        public decimal OverHeadProfitCost { get; set; }
        public int ProjectInformationId { get; set; }
        public virtual ProjectInformation ProjectInformation { get; set; }
        public int SubProjectTypeId { get; set; }
        public string ItemCode { get; set; }
    }
    public class SubContractWorkItem : TrackUserOperation
    {
        public int SubContractWorkItemId { get; set; }
        [DisplayName("Category")]
        public int WorkCategoryId { get; set; }
        public virtual WorkCategory WorkCategory { get; set; }
        public string WorkItemName { get; set; }
        [DisplayName("UOM")]
        public int UnitOfMeasurementId { get; set; }
        public UnitOfMeasurement UnitOfMeasurement { get; set; }
        [DisplayName("QTY")]
        public decimal Quntity { get; set; }
        [DisplayName("Sub contract cost  1")]
        public decimal SubContractCostOne { get; set; }
        [DisplayName("Sub contract cost  2")]
        public decimal SubContractCostTwo { get; set; }
        [DisplayName("Sub contract cost  3")]
        public decimal SubContractCostThree { get; set; }
        [DisplayName("Selected cost")]
        public decimal SelectedCost { get; set; }
        [DisplayName("Over heaad & profit")]
        public decimal OverHeadProfit { get; set; }
        [DisplayName("Total cost(including VAT")]
        public decimal TotalCost { get; set; }
        public int ProjectInformationId { get; set; }
        public virtual ProjectInformation ProjectInformation { get; set; }
        public int BlockId { get; set; }
        public string ItemCode { get; set; }
        public int GenericSubContractWorkItemId { get; set; }


    }
    public class GenericSubContractWorkItem : TrackUserOperation
    {
        [Key]
        public int GenericSubContractWorkItemId { get; set; }

        [DisplayName("Category")]
        public int WorkCategoryId { get; set; }
        public virtual WorkCategory WorkCategory { get; set; }
        public string WorkItemName { get; set; }
        [DisplayName("UOM")]
        public int UnitOfMeasurementId { get; set; }
        public UnitOfMeasurement UnitOfMeasurement { get; set; }
        [DisplayName("QTY")]
        public decimal Quntity { get; set; }
        [DisplayName("Sub contract cost  1")]
        public decimal SubContractCostOne { get; set; }
        [DisplayName("Sub contract cost  2")]
        public decimal SubContractCostTwo { get; set; }
        [DisplayName("Sub contract cost  3")]
        public decimal SubContractCostThree { get; set; }
        [DisplayName("Selected cost")]
        public decimal SelectedCost { get; set; }
        [DisplayName("Over heaad & profit")]
        public decimal OverHeadProfit { get; set; }
        [DisplayName("Total cost(including VAT")]
        public decimal TotalCost { get; set; }
        public int ProjectInformationId { get; set; }
        public virtual ProjectInformation ProjectInformation { get; set; }
        public int SubProjectTypeId { get; set; }
        public string ItemCode { get; set; }
    }
    public class WorkItemPerProject : TrackUserOperation
    {
        public WorkItemPerProject()
        {
            ThicknessFactor = 1;
        }
        //Define direct cost [Work Item] 
        [Key]
        public int WorkItemPerProjectId { get; set; }

        public int ProjectInformationId { get; set; }
        public virtual ProjectInformation ProjectInformation { get; set; }

        [Display(Name = "Category")]
        [Required]
        public int WorkCategoryId { get; set; }
        public virtual WorkCategory WorkCategory { get; set; }

        [Display(Name = "Item Name")]
        public int ProjectEstimationWorkItemId { get; set; }

        //[StringLength(200)]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }
        //public string Name { get; set; }

        [Display(Name = "Total Quantity Of Work")]
        [Required]
        public decimal TotalQuantityOfWork { get; set; }

        // total costs
        [Display(Name = "Total Material Cost")]
        public decimal MaterialCost { get; set; }

        [Display(Name = "Total Labour Cost")]
        public decimal LabourOutPut { get; set; }

        [Display(Name = "Total Equipment Cost")]
        public decimal EquipmentOutput { get; set; }
        //

        //
        //  costs per unit of work per hour 
        [Display(Name = "Material unit Cost")]
        public decimal MaterialCostPerUnit { get; set; }

        [Display(Name = "Equipment unit Cost")]
        public decimal EquipmentCostPerUnit { get; set; }

        [Display(Name = "Labor unit Cost")]
        public decimal LaborCostPerUnit { get; set; }

        [Display(Name = "Direct Cost")]
        public decimal DirectCost { get; set; }

        public decimal OverHeadCost { get; set; }
        public decimal ProfitCost { get; set; }
        [Display(Name = "Total Unit Cost")]
        public decimal TotalUnitCost { get; set; }
        //

        [Display(Name = "Project Type")]
        public AvailableProjectTypes ProjectType { get; set; }
        // public virtual Lookup Lookup { get; set; }
        public string Remark { get; set; }
        [Display(Name = "Hrly Output")]
        public decimal LabourHourlyOutPut { get; set; }
        [Display(Name = "UOM")]
        public int? UnitOfMeasurementId { get; set; }
        //public UnitOfMeasurement UnitOfMeasurement { get; set; }
        [Display(Name = "Overhead Rate")]
        public decimal OverheadRate { get; set; }
        [Display(Name = "Profit Rate")]
        public decimal ProfitMargin { get; set; }
        [Display(Name = "Parent")]
        public int ParentId { get; set; }//work item if it has parent

        public Nullable<int> BlockId { get; set; }
        public string ItemCode { get; set; }
        public bool IsSubCategory { get; set; }
        public int? CopiedFromId { get; set; }
        public int GenericWorkItemPerProjectId { get; set; }

        // used when actual executed work is filled in the take off sheet
        [Display(Name = "Actual Qty of work")]
        public decimal ActualQuantityOfWork { get; set; }

        //
        // for variation and supplementary purpose
        public bool IsVariationWorkItem { get; set; }
        public bool IsSupplementaryWorkItem { get; set; }
        //public decimal VariationQuantityOfWork { get; set; }
        //public decimal SupplemetaryQuantityOfWork { get; set; }
        public decimal AddedQuantityOfWork { get; set; }
        public decimal OmittedQuantityOfWork { get; set; }
        //

        // for work items that contain other work items as input.
        public bool ContainsWorkItemAsInput { get; set; }

        public bool HasAdditionalTransportationCost { get; set; }
        public decimal TransportCostRate { get; set; }
        public decimal CalculatedTransportCost { get; set; }
        public decimal TotalUnitCostIncludingTransport { get; set; }

        [Display(Name = "Thickness")]
        public decimal ThicknessFactor { get; set; }

        public virtual ICollection<ProjectMaterial> ProjectEstimationMaterial { get; set; }
        public virtual ICollection<ProjectDirectLabour> ProjectEstimationDirectLabour { get; set; }
        public virtual ICollection<ProjectEquipment> ProjectEstimationEquipment { get; set; }


    }
    public class GenericWorkItemPerProject : ItemDurationBase
    {
        public GenericWorkItemPerProject()
        {
            ThicknessFactor = 1;
        }
        [Key]
        public int GenericWorkItemPerProjectId { get; set; }

        public int ProjectInformationId { get; set; }
        public virtual ProjectInformation ProjectInformation { get; set; }

        [Display(Name = "Category")]
        [Required]
        public int WorkCategoryId { get; set; }
        public virtual WorkCategory WorkCategory { get; set; }

        [Display(Name = "Item Name")]
        public int ProjectEstimationWorkItemId { get; set; }

        [Display(Name = "Item Name")]
        public string ItemName { get; set; }

        [Display(Name = "Total Qty Of Work")]
        [Required]
        public decimal TotalQuantityOfWork { get; set; }

        // total costs
        [Display(Name = "Total Material Cost")]
        public decimal MaterialCost { get; set; }

        [Display(Name = "Total Labour Cost")]
        public decimal LabourOutPut { get; set; }

        [Display(Name = "Total Equipment Cost")]
        public decimal EquipmentOutput { get; set; }
        //

        //
        //  costs per unit of work per hour 
        [Display(Name = "Material unit Cost")]
        public decimal MaterialCostPerUnit { get; set; }

        [Display(Name = "Equipment unit Cost")]
        public decimal EquipmentCostPerUnit { get; set; }

        [Display(Name = "Labor unit Cost")]
        public decimal LaborCostPerUnit { get; set; }

        [Display(Name = "Direct Cost")]
        public decimal DirectCost { get; set; }

        public decimal OverHeadCost { get; set; }
        public decimal ProfitCost { get; set; }
        [Display(Name = "Total Unit Cost")]
        public decimal TotalUnitCost { get; set; }
        //

        [Display(Name = "Project Type")]
        public AvailableProjectTypes ProjectType { get; set; }
        public string Remark { get; set; }
        [Display(Name = "Hrly Output")]
        public decimal LabourHourlyOutPut { get; set; }
        [Display(Name = "UOM")]
        public int? UnitOfMeasurementId { get; set; }
        [Display(Name = "Overhead Rate")]
        public decimal OverheadRate { get; set; }
        [Display(Name = "Profit Rate")]
        public decimal ProfitMargin { get; set; }
        [Display(Name = "Parent")]
        public int ParentId { get; set; }//work item if it has parent

        public Nullable<int> SubProjectTypeId { get; set; }
        public string ItemCode { get; set; }
        public bool IsSubCategory { get; set; }
        public int? CopiedFromId { get; set; }

        //
        // for variation and supplementary purpose
        [Display(Name = "Is Variation Work")]
        public bool IsVariationWorkItem { get; set; }
        [Display(Name = "Is Supplementary Work")]
        public bool IsSupplementaryWorkItem { get; set; }
        //public decimal VariationQuantityOfWork { get; set; }
        //public decimal SupplemetaryQuantityOfWork { get; set; }
        [Display(Name = "Additional Qty of Work")]
        public decimal AddedQuantityOfWork { get; set; }
        [Display(Name = "Omitted Qty of Work")]
        public decimal OmittedQuantityOfWork { get; set; }
        //

        // for work items that contain other work items as input.
        public bool ContainsWorkItemAsInput { get; set; }

        // transportation cost information
        public bool HasAdditionalTransportationCost { get; set; }
        public decimal TransportCostRate { get; set; }
        public decimal CalculatedTransportCost { get; set; }
        public decimal TotalUnitCostIncludingTransport { get; set; }

        // thickness factor
        [Display(Name ="Thickness")]
        public decimal ThicknessFactor { get; set; }

    }
    public class GenericWorkItemThicknessFactorEditViewModel
    {
        public int ItemId { get; set; }
        [Display(Name ="Thickness")]
        public decimal ThicknessFactor { get; set; }
    }
    public class GenericWorkItemTransportCostViewwModel
    {
        public int ItemId { get; set; }
        [Display(Name ="Total Material Cost")]
        public decimal TotalMaterailCost { get; set; }
        [Display(Name ="Transport Cost Rate")]
        public decimal TransportCostRate { get; set; }
        [Display(Name ="Transport Cost")]
        public decimal CalculatedTransportCost { get; set; }
        //public decimal TotalUnitCostIncludingTransport { get; set; }
    }
    public class WorkItemPerProjectViewModel
    {
        public int Id { get; set; }
        public string SubProjectTypeName { get; set; }
        public string BlockName { get; set; }
        public string StructureyType { get; set; }
        public string WorkCategory { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public decimal TotalQuantityOfWork { get; set; }
        [Display(Name = "Qty of Work Excuted todate")]
        public decimal ActualQuantityOfWork { get; set; }
        public decimal DirectCost { get; set; }
        public decimal MaterialCost { get; set; }
        public decimal LabourOutPut { get; set; }
        public decimal EquipmentOutput { get; set; }
        public decimal MaterialCostPerUnit { get; set; }
        public decimal LaborCostPerUnit { get; set; }
        public decimal EquipmentCostPerUnit { get; set; }
        public decimal TotalUnitCost { get; set; }
        public decimal OverHeadCost { get; set; }
        public decimal ProfitCost { get; set; }
        public string Remark { get; set; }
        public decimal LabourHourlyOutPut { get; set; }
        public string UnitOfMeasurement { get; set; }
        public decimal OverheadRate { get; set; }
        public decimal ProfitMargin { get; set; }
        public bool IsSubCategory { get; set; }
        public bool ContainsWorkItemAsInput { get; set; }
        public AvailableProjectTypes ProjectType { get; set; }
    }
    public class CopyEstimationDataToDurationViewModel
    {
        public int Id { get; set; }
        public bool IsCopied { get; set; }
        public string ItemName { get; set; }


        public string SubProjectTypeName { get; set; }
        public string BlockName { get; set; }
        public string StructureType { get; set; }
        public string WorkCategory { get; set; }
        public decimal TotalQuantityOfWork { get; set; }
        public decimal TotalUnitRate { get; set; }
        public string UnitOfMeasurement { get; set; }
        public bool HasSubItems { get; set; }
        //public int WorkCategoryId { get; set; }
    }
    public class GenericWorkItemPerProjectViewModel
    {
        public int Id { get; set; }
        public string SubProjectTypeName { get; set; }
        public int? SubProjectTypeCode { get; set; }
        public string StructureyType { get; set; }
        public string WorkCategory { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        [Display(Name = "Total Qty of Work")]
        public decimal TotalQuantityOfWork { get; set; }
        public decimal ActualQuantityOfWork { get; set; }
        public decimal DirectCost { get; set; }
        public decimal MaterialCost { get; set; }
        public decimal LabourOutPut { get; set; }
        public decimal EquipmentOutput { get; set; }
        public decimal MaterialCostPerUnit { get; set; }
        public decimal LaborCostPerUnit { get; set; }
        public decimal EquipmentCostPerUnit { get; set; }
        public decimal TotalUnitCost { get; set; }
        public decimal OverHeadCost { get; set; }
        public decimal ProfitCost { get; set; }
        public string Remark { get; set; }
        [Display(Name = "Hrly Output")]
        public decimal LabourHourlyOutPut { get; set; }
        [Display(Name = "UoM")]
        public int UnitOfMeasurementId { get; set; }
        public string UnitOfMeasurement { get; set; }
        public decimal OverheadRate { get; set; }
        public decimal ProfitMargin { get; set; }
        public bool IsSubCategory { get; set; }
        // rate * quantity
        public decimal Amount { get; set; }

        public bool ContainsWorkItemAsInput { get; set; }
        public int WorkCategoryId { get; set; }

        //
        // duration estimation 
        [Display(Name = "Productivity /Crew /Day")]
        public decimal? ProductivityOfCrewPerDay { get; set; }
        [Display(Name = "Duration /Crew")]
        public decimal? DurationPerCrew { get; set; }
        [Display(Name = "No. of Crews")]
        public int? NumberOfCrew { get; set; }
        public decimal? EstimatedDuration { get; set; }
        public int ProjectInformationId { get; set; }
        //
    }
    public class GenericWorkItemPerProjectDetailVM
    {
        public int Id { get; set; }
        public string Attribute { get; set; }
        public string Value { get; set; }
    }
    public class GenericWorkItemQuantityImportViewModel
    {
        public int Id { get; set; }
        public string SubProjectTypeName { get; set; }
        public string StructureyType { get; set; }
        public string WorkCategory { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        [Display(Name = "Total Qty of Work")]
        public decimal TotalQuantityOfWork { get; set; }
        public decimal ActualQuantityOfWork { get; set; }
        public decimal DirectCost { get; set; }
        public decimal MaterialCost { get; set; }
        public decimal LabourOutPut { get; set; }
        public decimal EquipmentOutput { get; set; }
        public decimal MaterialCostPerUnit { get; set; }
        public decimal LaborCostPerUnit { get; set; }
        public decimal EquipmentCostPerUnit { get; set; }
        public decimal TotalUnitCost { get; set; }
        public decimal OverHeadCost { get; set; }
        public decimal ProfitCost { get; set; }
        public string Remark { get; set; }
        [Display(Name = "Hrly Output")]
        public decimal LabourHourlyOutPut { get; set; }
        public string UnitOfMeasurement { get; set; }
        public decimal OverheadRate { get; set; }
        public decimal ProfitMargin { get; set; }
        public bool IsSubCategory { get; set; }
        public bool HasParent { get; set; }
        // rate * quantity
        public decimal Amount { get; set; }

        public bool ContainsWorkItemAsInput { get; set; }

    }
    public class ImportWorkItemToScheduleViewModel
    {
        // work item id
        public int Id { get; set; }
        public string BlockName { get; set; }
        public string StructureyType { get; set; }
        public string WorkCategory { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public decimal TotalQuantityOfWork { get; set; }

        public decimal DirectCost { get; set; }
        public decimal MaterialCost { get; set; }

        public decimal LabourOutPut { get; set; }
        public decimal EquipmentOutput { get; set; }
        public decimal MaterialCostPerUnit { get; set; }
        public decimal LaborCostPerUnit { get; set; }
        public decimal EquipmentCostPerUnit { get; set; }
        public decimal TotalUnitCost { get; set; }
        public decimal OverHeadCost { get; set; }
        public decimal ProfitCost { get; set; }
        public string Remark { get; set; }
        public decimal LabourHourlyOutPut { get; set; }
        public string UnitOfMeasurement { get; set; }
        public decimal OverheadRate { get; set; }
        public decimal ProfitMargin { get; set; }
        public bool IsSubCategory { get; set; }
        public bool IsImported { get; set; }
    }
    // view model for copying work items from project structure to other structure within that project or other one
    public class CopyGenericTemplateWorkItemViewModel
    {
        public int Id { get; set; }
        public bool IsCopied { get; set; }
        public string ItemName { get; set; }
        public string Category { get; set; }
        public decimal TotalQuantityOfWork { get; set; }
        public decimal TotalUnitRate { get; set; }
        public string UnitOfMeasurement { get; set; }
        public bool IsSubCategory { get; set; }
        public int WorkCategoryId { get; set; }
    }
    public class PieceRateViewModel
    {
        public string BlockName { get; set; }
        public string StructureType { get; set; }
        [DisplayName("Category")]
        public string CategoryName { get; set; }
        public int Id { get; set; }
        public string WorkItemName { get; set; }
        [DisplayName("W")]
        public decimal Width { get; set; }
        [DisplayName("H")]
        public decimal Height { get; set; }
        public decimal Area { get; set; }
        [DisplayName("Price/m2")]
        public decimal Price { get; set; }
        public decimal MaterialCost { get; set; }
        public decimal TransportCost { get; set; }
        public decimal CylinderLock { get; set; }
        public decimal DirectCost { get; set; }
        [DisplayName("Over Head + Profit")]
        public decimal OverHeadProfitCost { get; set; }
        public int ProjectInformationId { get; set; }
        public string ItemCode { get; set; }
    }
    public class GenericPieceRateViewModel
    {
        public string SubProjectTypeName { get; set; }
        public string StructureyType { get; set; }
        public string WorkCategory { get; set; }
        public int Id { get; set; }
        public string WorkItemName { get; set; }
        [DisplayName("W")]
        public decimal Width { get; set; }
        [DisplayName("H")]
        public decimal Height { get; set; }
        public decimal Area { get; set; }
        [DisplayName("Price/m2")]
        public decimal Price { get; set; }
        public decimal MaterialCost { get; set; }
        public decimal TransportCost { get; set; }
        public decimal CylinderLock { get; set; }
        public decimal DirectCost { get; set; }
        [DisplayName("Over Head + Profit")]
        public decimal OverHeadProfitCost { get; set; }
        public int ProjectInformationId { get; set; }
        public string ItemCode { get; set; }
    }
    public class GenericPieceRateCopyViewModel
    {
        public string BlockName { get; set; }
        public string StructureType { get; set; }
        [DisplayName("Category")]
        public string CategoryName { get; set; }
        public int Id { get; set; }
        public string WorkItemName { get; set; }
        public bool IsCopied { get; set; }

        public decimal Area { get; set; }
        [DisplayName("Price/m2")]
        public decimal Price { get; set; }
        public decimal MaterialCost { get; set; }
        public decimal TransportCost { get; set; }
        public decimal DirectCost { get; set; }
        [DisplayName("Over Head + Profit")]
        public decimal OverHeadProfitCost { get; set; }
        public string ItemCode { get; set; }
    }
    public class SubContractWorkItemViewModel
    {
        public string BlockName { get; set; }
        [Display(Name = "Str.")]
        public string StructureType { get; set; }
        public string Category { get; set; }
        public int Id { get; set; }
        public virtual WorkCategory WorkCategory { get; set; }
        public string WorkItemName { get; set; }
        [DisplayName("UOM")]
        public string UnitOfMeasurement { get; set; }
        [DisplayName("QTY")]
        public decimal Quntity { get; set; }
        [DisplayName("Sub contract cost  1")]
        public decimal SubContractCostOne { get; set; }
        [DisplayName("Sub contract cost  2")]
        public decimal SubContractCostTwo { get; set; }
        [DisplayName("Sub contract cost  3")]
        public decimal SubContractCostThree { get; set; }
        [DisplayName("Selected cost")]
        public decimal SelectedCost { get; set; }
        [DisplayName("Over heaad & profit")]
        public decimal OverHeadProfit { get; set; }
        [DisplayName("Total cost(including VAT")]
        public decimal TotalCost { get; set; }
        public string ItemCode { get; set; }
    }
    public class GenericSubContractWorkItemViewModel
    {
        public string SubProjectTypeName { get; set; }
        public string StructureyType { get; set; }
        public string WorkCategory { get; set; }
        public int Id { get; set; }
        public string WorkItemName { get; set; }
        [DisplayName("UOM")]
        public string UnitOfMeasurement { get; set; }
        [DisplayName("QTY")]
        public decimal Quntity { get; set; }
        [DisplayName("Sub contract cost  1")]
        public decimal SubContractCostOne { get; set; }
        [DisplayName("Sub contract cost  2")]
        public decimal SubContractCostTwo { get; set; }
        [DisplayName("Sub contract cost  3")]
        public decimal SubContractCostThree { get; set; }
        [DisplayName("Selected cost")]
        public decimal SelectedCost { get; set; }
        [DisplayName("Over heaad & profit")]
        public decimal OverHeadProfit { get; set; }
        [DisplayName("Total cost(including VAT")]
        public decimal TotalCost { get; set; }
        public string ItemCode { get; set; }
    }
    public class GenericSubContractWorkItemCopyViewModel
    {
        public string WorkCategory { get; set; }
        public int Id { get; set; }
        public string WorkItemName { get; set; }
        public bool IsCopied { get; set; }
        [DisplayName("UOM")]
        public string UnitOfMeasurement { get; set; }
        public decimal SelectedCost { get; set; }
        [DisplayName("Over heaad & profit")]
        public decimal OverHeadProfit { get; set; }
        [DisplayName("Total cost(including VAT")]
        public decimal TotalCost { get; set; }
        public string ItemCode { get; set; }
    }
    public class ProjectCustomWorkItemViewModel
    {
        public string BlockName { get; set; }
        public string StructureType { get; set; }
        public string Category { get; set; }
        public int Id { get; set; }

        [Display(Name = "Item Name")]
        public string WorkItemName { get; set; }

        [Display(Name = "Unit Rate")]
        public decimal TotalUnitRate { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalCost { get; set; }
        public string ItemCode { get; set; }
    }
    public class ProjectCustomWorkItem : TrackUserOperation
    {
        [Key]
        public int ProjectCustomWorkItemId { get; set; }

        [Display(Name = "Category")]
        public int WorkCategoryId { get; set; }

        [Display(Name = "Item Name")]
        public string WorkItemName { get; set; }
        public int ProjectInformationId { get; set; }

        [Display(Name = "Unit Rate")]
        public decimal TotalUnitRate { get; set; }
        [Display(Name = "Project Type")]
        public AvailableProjectTypes ProjectType { get; set; }
        public int? BlockId { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalCost { get; set; }
        public string Remark { get; set; }

        public string ItemCode { get; set; }
        public int GenericProjectCustomWorkItemId { get; set; }

        [Display(Name = "Total Qty Of Work")]
        [Required]
        public decimal TotalQuantityOfWork { get; set; }

        [Display(Name = "Hrly Output")]
        public decimal HourlyOutPut { get; set; }
        [Display(Name = "UOM")]
        public int? UnitOfMeasurementId { get; set; }
        [Display(Name = "Overhead Rate")]
        public decimal OverheadRate { get; set; }
        [Display(Name = "Profit Rate")]
        public decimal ProfitRate { get; set; }

        //
        //  costs per unit of work per hour 
        [Display(Name = "Material unit Cost")]
        public decimal MaterialCostPerUnit { get; set; }

        [Display(Name = "Equipment unit Cost")]
        public decimal EquipmentCostPerUnit { get; set; }

        [Display(Name = "Labor unit Cost")]
        public decimal LaborCostPerUnit { get; set; }

        [Display(Name = "Direct Cost")]
        public decimal DirectCost { get; set; }

        [Display(Name = "Overhead Cost")]
        public decimal OverHeadCost { get; set; }
        [Display(Name = "Profit Cost")]
        public decimal ProfitCost { get; set; }



    }
    public class GenericProjectCustomWorkItemViewModel
    {
        public string SubProjectTypeName { get; set; }
        public string StructureyType { get; set; }
        public string WorkCategory { get; set; }
        public int Id { get; set; }

        [Display(Name = "Item Name")]
        public string WorkItemName { get; set; }

        [Display(Name = "Unit Rate")]
        public decimal TotalUnitRate { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalCost { get; set; }
        public string ItemCode { get; set; }
        public decimal TotalQuantityOfWork { get; set; }

        [Display(Name = "Hrly Output")]
        public decimal HourlyOutPut { get; set; }
        [Display(Name = "UOM")]
        public string UnitOfMeasurement { get; set; }
        [Display(Name = "Overhead Rate")]
        public decimal OverheadRate { get; set; }
        [Display(Name = "Profit Rate")]
        public decimal ProfitRate { get; set; }

        //
        //  costs per unit of work per hour 
        [Display(Name = "Material unit Cost")]
        public decimal MaterialCostPerUnit { get; set; }

        [Display(Name = "Equipment unit Cost")]
        public decimal EquipmentCostPerUnit { get; set; }

        [Display(Name = "Labor unit Cost")]
        public decimal LaborCostPerUnit { get; set; }

        [Display(Name = "Direct Cost")]
        public decimal DirectCost { get; set; }

        [Display(Name = "Overhead Cost")]
        public decimal OverHeadCost { get; set; }
        [Display(Name = "Profit Cost")]
        public decimal ProfitCost { get; set; }
    }
    public class GenericProjectCustomWorkItem : TrackUserOperation
    {
        [Key]
        public int GenericProjectCustomWorkItemId { get; set; }
        [Display(Name = "Category")]
        public int WorkCategoryId { get; set; }

        [Display(Name = "Item Name")]
        public string WorkItemName { get; set; }
        public int ProjectInformationId { get; set; }

        [Display(Name = "Unit Rate")]
        public decimal TotalUnitRate { get; set; }
        [Display(Name = "Project Type")]
        public AvailableProjectTypes ProjectType { get; set; }
        public int? SubProjectTypeId { get; set; }

        public decimal TotalQuantityOfWork { get; set; }
        public decimal TotalCost { get; set; }
        public string Remark { get; set; }

        public string ItemCode { get; set; }


        //[Display(Name = "Total Qty Of Work")]
        //public decimal TotalQuantityOfWork { get; set; }

        [Display(Name = "Hrly Output")]
        public decimal HourlyOutPut { get; set; }
        [Display(Name = "UOM")]
        public int? UnitOfMeasurementId { get; set; }
        [Display(Name = "Overhead Rate")]
        public decimal OverheadRate { get; set; }
        [Display(Name = "Profit Rate")]
        public decimal ProfitRate { get; set; }

        //
        //  costs per unit of work per hour 
        [Display(Name = "Material unit Cost")]
        public decimal MaterialCostPerUnit { get; set; }

        [Display(Name = "Equipment unit Cost")]
        public decimal EquipmentCostPerUnit { get; set; }

        [Display(Name = "Labor unit Cost")]
        public decimal LaborCostPerUnit { get; set; }

        [Display(Name = "Direct Cost")]
        public decimal DirectCost { get; set; }

        [Display(Name = "Overhead Cost")]
        public decimal OverHeadCost { get; set; }
        [Display(Name = "Profit Cost")]
        public decimal ProfitCost { get; set; }

    }
    public class GenericProjectCustomWorkItemDetail : TrackUserOperation
    {
        [Key]
        public int GenericProjectCustomWorkItemDetailId { get; set; }

        [Display(Name = "Work Item")]
        public int ProjectEstimationWorkItemId { get; set; }
        // its used to show under which work item this content/detail is used
        public int GenericWorkItemPerProjectId { get; set; }

        [Display(Name = "Qty/unit")]
        public decimal QuantityPerUnit { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitCost { get; set; }
        [Display(Name = "Totl Cost")]
        public decimal TotalCost { get; set; }

        public virtual ProjectEstimationWorkItem RootWorkItem { get; set; }
        //public virtual GenericWorkItemPerProject GenericWorkItemPerProject { get; set; }
    }
    public class ProjectCustomWorkItemDetail : TrackUserOperation
    {
        public int ProjectCustomWorkItemDetailId { get; set; }

        [Display(Name = "Work Item")]
        public int ProjectEstimationWorkItemId { get; set; }
        // its used to show under which work item this content/detail is used
        public int WorkItemPerProjectId { get; set; }

        [Display(Name = "Qty/unit")]
        public decimal QuantityPerUnit { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitCost { get; set; }
        [Display(Name = "Totl Cost")]
        public decimal TotalCost { get; set; }

        public virtual ProjectEstimationWorkItem RootWorkItem { get; set; }
    }
    public class ProjectMaterial : TrackUserOperation
    {
        //Define direct cost [Material Cost] 
        [Key]
        public int ProjectMaterialId { get; set; }
        public int WorkItemPerProjectId { get; set; }
        public virtual WorkItemPerProject WorkItemPerProject { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Type Of Material")]
        public string ItemCode { get; set; }
        public virtual ExceedERP.Core.Domain.Inventory.InventoryItem Item { get; set; }
        [Display(Name = "Category")]
        public string ItemCategoryCode { get; set; }
        public string Unit { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        [Required]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Required]
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }
        public decimal MaterialUnitCost { get; set; }
    }
    //public class GenericWorkItemTransportationCost
    //{
    //    public int GenericWorkItemTransportationCostId { get; set; }

    //    public int GenericWorkItemPerProjectId { get; set; }
    //    public virtual GenericWorkItemPerProject WorkItem { get; set; }

    //    public decimal TotalMaterialCost { get; set; }
    //    public decimal TransportCostPercentage { get; set; }
    //    public decimal MyProperty { get; set; }
    //}
    public class GenericProjectMaterial : TrackUserOperation
    {
        [Key]
        public int GenericProjectMaterialId { get; set; }

        public int GenericWorkItemPerProjectId { get; set; }
        public virtual GenericWorkItemPerProject WorkItem { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Type Of Material")]
        public string ItemCode { get; set; }
        public virtual ExceedERP.Core.Domain.Inventory.InventoryItem Item { get; set; }
        [Display(Name = "Item Category")]
        public string ItemCategoryCode { get; set; }
        public string Unit { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        [Required]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Required]
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }
        public decimal MaterialUnitCost { get; set; }
    }
    public class ProjectDirectLabour : TrackUserOperation
    {
        //Define direct cost [Direct Labour Cost] 
        [Key]
        public int ProjectDirectLabourId { get; set; }
        public int WorkItemPerProjectId { get; set; }
        public virtual WorkItemPerProject WorkItemPerProject { get; set; }

        //[Required]
        //[StringLength(100)]
        //[Display(Name = "Item No.")]
        //public string ItemNo { get; set; }
        [Required]
        [Display(Name = "Labour Name")]
        // public int LabourName { get; set; }
        public int ProjectEstimationIndexLabourId { get; set; }
        public virtual ProjectEstimationIndexLabour ProjectEstimationIndexLabour { get; set; }
        public string Unit { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        [Required]
        [Display(Name = "U.F.")]
        public decimal UnitFactory { get; set; }

        [Display(Name = "Indexed hrly sal.")]
        public decimal IndexedHourlySalary { get; set; }
        [Required]
        [Display(Name = "Daily Pay")]
        public decimal DailyPay { get; set; }
        [Required]
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }
        public decimal ManpowerUnitCost { get; set; }
    }
    public class GenericProjectDirectLabour : TrackUserOperation
    {
        [Key]
        public int GenericProjectDirectLabourId { get; set; }
        public int GenericWorkItemPerProjectId { get; set; }
        public virtual GenericWorkItemPerProject WorkItem { get; set; }
        [Display(Name = "Labour Name")]
        // public int LabourName { get; set; }
        public int ProjectEstimationIndexLabourId { get; set; }
        public virtual ProjectEstimationIndexLabour ProjectEstimationIndexLabour { get; set; }
        public string Unit { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        [Required]
        [Display(Name = "U.F.")]
        public decimal UnitFactory { get; set; }

        [Display(Name = "Indexed hrly sal.")]
        public decimal IndexedHourlySalary { get; set; }
        [Required]
        [Display(Name = "Daily Pay")]
        public decimal DailyPay { get; set; }
        [Required]
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }
        public decimal ManpowerUnitCost { get; set; }
    }
    public class ProjectEquipment : TrackUserOperation
    {
        //Define direct cost [Equipment Cost] 
        [Key]
        public int ProjectEquipmentId { get; set; }
        public int WorkItemPerProjectId { get; set; }
        public virtual WorkItemPerProject WorkItemPerProject { get; set; }
        [Required]

        [Display(Name = "Equipment Name")]
        public Guid FleetsID { get; set; }
        //[Display(Name = "Equipment Type")]
        //public Guid EquipmentTypeID { get; set; }
        //[Display(Name = "Equipment Category")]
        //public Guid EquipmentIdentityID { get; set; }
        public virtual Fleets Fleet { get; set; }
        public string Unit { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        [Required]
        [Display(Name = "Unit Factory")]
        public decimal UnitFactory { get; set; }
        [Required]
        [Display(Name = "Rental Cost")]
        public decimal RentalCost { get; set; }
        [Required]
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }
        public decimal EquipmentUnitCost { get; set; }
        public string EquipmentType { get; set; }

    }
    public class GenericProjectEquipment : TrackUserOperation
    {
        [Key]
        public int GenericProjectEquipmentId { get; set; }
        public int GenericWorkItemPerProjectId { get; set; }
        public virtual GenericWorkItemPerProject WorkItem { get; set; }

        [Required]
        [Display(Name = "Equipment Name")]
        public Guid FleetsID { get; set; }

        public virtual Fleets Fleet { get; set; }
        public string Unit { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        [Required]
        [Display(Name = "Unit Factory")]
        public decimal UnitFactory { get; set; }
        [Required]
        [Display(Name = "Rental Cost")]
        public decimal RentalCost { get; set; }
        [Required]
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }
        public decimal EquipmentUnitCost { get; set; }
        public string EquipmentType { get; set; }
    }
    public class TakeOffPerProject : TrackUserOperation
    {
        [Key]
        public int TakeOffPerProjectId { get; set; }
        [DisplayName("Take of Number")]
        [StringLength(200)]
        public string TakeOffNo { get; set; }
        [Display(Name = "Work Item")]
        public int WorkItemPerProjectId { get; set; }
        //  public virtual WorkItemPerProject WorkItemPerProject { get; set; }

        [Display(Name = "Is Reinforcement bar take off ?")]
        public bool IsReinforcementbarTakeOff { get; set; }
        [Display(Name = "Project")]
        public int ProjectInformationId { get; set; }
        //public virtual ProjectEstimationWorkItem ProjectEstimationWorkItem { get; set; }
        //public virtual ICollection<ProjectTakeOffLine> ProjectTakeOffLine { get; set; }
        public string Remark { get; set; }
    }
    public class TakeOffLinePerProject : TrackUserOperation
    {
        [Key]
        public int TakeOffLinePerProjectId { get; set; }

        [DisplayName("No.")]
        public decimal No { get; set; }
        [DisplayName("D")]

        public decimal D { get; set; }

        [DisplayName("Quantity")]
        public decimal Quantity { get; set; }
        [DisplayName("Description")]
        //[StringLength(400)]
        public string Description { get; set; }
        public int TakeOffPerProjectId { get; set; }
        public virtual TakeOffPerProject TakeOffPerProject { get; set; }
        [Required]
        [Display(Name = "Unit")]
        public int UnitOfMeasurementId { get; set; }
        //  public virtual Lookup Lookup { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Depth { get; set; }
        public bool IsDeductive { get; set; }
    }
    public class TakeOffLineForKgPerProject : TrackUserOperation
    {
        [Key]
        public int TakeOffLineForKgPerProjectId { get; set; }

        public string Location { get; set; }
        public string Shape { get; set; }

        [Display(Name = "Diameter")]
        public int DeformedBarId { get; set; }
        public virtual DeformedBar DeformedBar { get; set; }

        //public int UnitRateId { get; set; }
        //public virtual UnitRate UnitRate { get; set; }
        public decimal Length { get; set; }
        public decimal Diameter { get; set; }
        public decimal TotalWeight { get; set; }

        [Display(Name = "No.of Bars")]
        public decimal NoOfBars { get; set; }

        [Display(Name = "No.of elem.")]
        public decimal NoOfElement { get; set; }
        [Display(Name = "Total")]
        public decimal TotalBar { get; set; }
        [Display(Name = "Total Length(m)")]
        public decimal TotalLength { get; set; }
        [Display(Name ="Total Weight")]
        public decimal GrandTotal { get; set; }

        [DisplayName("Description")]
        //[StringLength(200)]
        public string Description { get; set; }
        public string Type { get; set; }
        public int TakeOffPerProjectId { get; set; }
        public virtual TakeOffPerProject TakeOffPerProject { get; set; }
    }
}
