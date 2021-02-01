using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Construction
{
    public class ProjectCosting { }
    //material utilization
    public class ProjectDefineBudget : TrackUserOperation
    {
        [Key]
        public int ProjectDefineBudgetId { get; set; }
        public int ProjectInformationId { get; set; }
        // public virtual ProjectInformation Project { get; set; }

        [Required]
        [Display(Name = "Budget Code")]
        [StringLength(100)]
        public string BudgetCode { get; set; }
        [Required]
        [Display(Name = "Total project budget")]
        public decimal TotalBudget { get; set; }
        [Display(Name = "Estimated amount")]
        public decimal EstimatedAmount { get; set; }
        [Display(Name = "Description")]
        [StringLength(200)]
        public string Description { get; set; }
        public virtual ICollection<ProjectBudgetTransfer> ProjectBudgetTransfer { get; set; }
        public virtual ICollection<ProjectBudgetRequest> ProjectBudgetRequest { get; set; }
        public virtual ICollection<ProjectBudgetUtilization> ProjectBudgetUtilization { get; set; }

    }
    public class ProjectMaterialRequest
    {
        [Key]
        public int ProjectMaterialRequestId { get; set; }
        [Required]
        [Display(Name = "Request Date")]

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime RequestDate { get; set; }
        [Required]
        [Display(Name = "Material")]
        [StringLength(100)]
        public string ItemCode { get; set; }
        [Required]
        [Display(Name = "Requested QTY")]
        public decimal RequestedQty { get; set; }
        [Display(Name = "Description")]
        [StringLength(200)]
        public string Decription { get; set; }
        public virtual ProjectInformation Project { get; set; }

    }
    public class ProjectMaterialIssued
    {
        [Key]
        public int ProjectMaterialIssuedId { get; set; }
        [Required]
        [Display(Name = "Issue Date")]

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime IssueDate { get; set; }
        [Required]
        [Display(Name = "Material")]
        [StringLength(100)]
        public string ItemCode { get; set; }
        [Required]
        [Display(Name = "Issued QTY")]
        public decimal IssuedQty { get; set; }
        [Display(Name = "Description")]
        [StringLength(200)]
        public string Decription { get; set; }
        public virtual ProjectInformation Project { get; set; }

    }
    public class ProjectMaterialDelivery
    {
        [Key]
        public int ProjectMaterialDeliveryId { get; set; }
        [Required]
        [Display(Name = "Delivery Date")]

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DeliveryDate { get; set; }
        [Required]
        [Display(Name = "Material")]
        [StringLength(100)]
        public string ItemCode { get; set; }
        [Required]
        [Display(Name = "Delivered QTY")]
        public decimal DeliveredQty { get; set; }
        [Display(Name = "Description")]
        [StringLength(200)]
        public string Decription { get; set; }
        public virtual ProjectInformation Project { get; set; }

    }
    //equipment utilization
    public class ProjectEquipmentUtilization : TrackUserOperation
    {
        [Key]
        public int ProjectEquipmentUtilizationId { get; set; }
        [Display(Name = "Equipment Type")]
        public int ProjectEquipmentId { get; set; }
        // public virtual ProjectEquipment ProjectEquipment { get; set; }
        public virtual ICollection<ProjectEquipmentHour> ProjectEquipmentHour { get; set; }
        public virtual ICollection<ProjectEquipmentPerformance> ProjectEquipmentPerformance { get; set; }
        public virtual ICollection<ProjectEquipmentFuelconsumption> ProjectEquipmentFuelconsumption { get; set; }
        public virtual ICollection<ProjectEquipmentMobilization> ProjectEquipmentMobilization { get; set; }
        public int ProjectInformationId { get; set; }
        // public virtual ProjectInformation Project { get; set; }
        public string Remark { get; set; }

    }
    public class ProjectEquipmentHour : TrackUserOperation
    {
        [Key]
        public int ProjectEquipmentHourId { get; set; }
        public int ProjectEquipmentUtilizationId { get; set; }
        public virtual ProjectEquipmentUtilization ProjectEquipmentUtilization { get; set; }
        [Required]
        [Display(Name = "Planned Hours")]
        public decimal PlannedHours { get; set; }
        [Required]
        [Display(Name = "Available Hours")]
        public decimal AvailableHours { get; set; }
        [Required]
        [Display(Name = "Utilized Hours")]
        public decimal UtilizedHours { get; set; }
        [Display(Name = "Available Efficiency")]
        public decimal AvailableEfficiency { get; set; }
        [Display(Name = "Utilized Efficiency")]
        public decimal UtilizedEfficiency { get; set; }
        [Display(Name = "Overall Efficiency")]
        public decimal OverallEfficiency { get; set; }
        [Display(Name = "Description")]
        [StringLength(200)]
        public string Remark { get; set; }

    }
    public class ProjectEquipmentPerformance : TrackUserOperation
    {
        [Key]
        public int ProjectEquipmentPerformanceId { get; set; }
        public int ProjectEquipmentUtilizationId { get; set; }
        public virtual ProjectEquipmentUtilization ProjectEquipmentUtilization { get; set; }
        [Required]
        [Display(Name = "Date")]

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }
        //[Required]
        //[Display(Name = "Equipment Type")]
        //public string EquipmentType { get; set; }
        [Required]
        [Display(Name = "Previous Performance")]
        public decimal PreviousPerformance { get; set; }
        [Display(Name = "Current Period Performance")]
        public decimal CurrentPeriodPerformance { get; set; }
        [Display(Name = "To Date Performance")]
        public decimal ToDatePerformance { get; set; }
        public string Remark { get; set; }

    }
    public class ProjectEquipmentFuelconsumption : TrackUserOperation
    {
        [Key]
        public int ProjectEquipmentFuelconsumptionId { get; set; }
        public int ProjectEquipmentUtilizationId { get; set; }
        public virtual ProjectEquipmentUtilization ProjectEquipmentUtilization { get; set; }
        [Required]
        [Display(Name = "Previous Delivered QTY")]
        public decimal PreviousDelivered { get; set; }
        [Required]
        [Display(Name = "Current Period Delivered QTY")]
        public decimal ToDateDelivered { get; set; }
        [Required]
        [Display(Name = "To Date Delivered QTY")]
        public decimal CurrentPeriodDelivered { get; set; }
        [Display(Name = "Total Previous Consumed")]
        public decimal TotalPreviousConsumed { get; set; }
        [Display(Name = "Total Current Consumed")]
        public decimal TotalCurrentConsumed { get; set; }
        [Display(Name = "Total To Date Consumed")]
        public decimal TotalToDateConsumed { get; set; }
        [Display(Name = "To Date Balance")]
        public decimal ToDateBalance { get; set; }
        public string Remark { get; set; }

    }
    public class ProjectEquipmentMobilization : TrackUserOperation
    {
        [Key]
        public int ProjectEquipmentMobilizationId { get; set; }
        public int ProjectEquipmentUtilizationId { get; set; }
        public virtual ProjectEquipmentUtilization ProjectEquipmentUtilization { get; set; }
        //[Required]
        //[Display(Name = "Equipment Type")]

        //public string  EquipmentType { get; set; }
        [Required]
        [Display(Name = "From")]
        public string From { get; set; }
        [Required]
        [Display(Name = "To")]
        public string To { get; set; }
        [Required]
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }
        [Required]
        [Display(Name = "Time")]
        public string Time { get; set; }
        public string Remark { get; set; }
    }
    //human resource
    public class ProjectHumanResourceUtilization : TrackUserOperation
    {
        [Key]
        public int ProjectHumanResourceUtilizationId { get; set; }
        [Required]
        [Display(Name = "Crew Code")]
        [StringLength(100)]
        public string CrewCode { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name = "Crew Name")]
        public string CrewName { get; set; }
        [Required]
        [Display(Name = "Crew Leader")]
        public int EmployeeId { get; set; }
        public virtual Person_Employee PersonEmployee { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name = "Combination of Crew")]
        public string CombinationOfCrew { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name = "Type of Work")]
        public string TypeOfWork { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name = "Crew Output")]
        public string CrewOutput { get; set; }
        [Required]

        [Display(Name = "Crew Direct Cost")]
        public decimal CrewDirectCost { get; set; }
        [Required]

        [Display(Name = "Crew Overhead Cost")]
        public decimal CrewIndirectCost { get; set; }
        public int ProjectInformationId { get; set; }
        public virtual ProjectInformation Project { get; set; }
    }
    public class ProjectBudgetRequest : TrackUserOperation
    {
        [Key]
        public int ProjectBudgetRequestId { get; set; }
        public int ProjectDefineBudgetId { get; set; }
        public virtual ProjectDefineBudget ProjectDefineBudget { get; set; }
        [Required]
        [Display(Name = "Budget Requested For")]
        [StringLength(200)]
        public string RequestedFor { get; set; }
        [Required]
        [Display(Name = "Budget Requested Amount")]
        public decimal Requested { get; set; }
        [Required]
        [Display(Name = "Budget Approved Amount")]
        public decimal Approved { get; set; }
        public decimal Difference { get; set; }

    }
    public class ProjectBudgetTransfer : TrackUserOperation
    {
        [Key]
        public int ProjectBudgetTransferId { get; set; }
        public int ProjectDefineBudgetId { get; set; }
        public virtual ProjectDefineBudget ProjectDefineBudget { get; set; }
        [Required]
        [Display(Name = "Previous Transferred Amount")]
        public decimal PreviousTransferred { get; set; }
        [Required]
        [Display(Name = "Current Transferred Amount")]
        public decimal CurrentPeriodTransferred { get; set; }

    }
    public class ProjectBudgetUtilization : TrackUserOperation
    {
        [Key]
        public int ProjectBudgetUtilizationId { get; set; }
        public int ProjectDefineBudgetId { get; set; }
        public virtual ProjectDefineBudget ProjectDefineBudget { get; set; }
        [Required]
        [Display(Name = "Budget Requested For")]
        [StringLength(200)]
        public string RequestedFor { get; set; }
        [Required]

        [Display(Name = "Transferred Amount")]
        public decimal Transferred { get; set; }
        [Required]
        [Display(Name = "Utilized Amount")]
        public decimal Utilized { get; set; }
        public decimal Difference { get; set; }

    }
    public class MaterialUtilization : TrackUserOperation
    {
        [Key]
        public int MaterialUtilizationId { get; set; }
        [Display(Name = "Material Type")]
        public int ProjectMaterialId { get; set; }
        // public virtual  ProjectMaterial ProjectMaterial { get; set; }
        [Display(Name = "Requested Qty")]
        public decimal RequestedQuantity { get; set; }
        [Display(Name = "Delivered Qty")]
        public decimal DeliveredQuantity { get; set; }
        [Display(Name = "Remaining Qty")]
        public decimal RemainingQuantity { get; set; }

        [Display(Name = "Issued To Site")]
        public decimal MaterialIssuedToSite { get; set; }
        public string Remark { get; set; }
        public int ProjectInformationId { get; set; }
        public virtual ProjectInformation Project { get; set; }

    }
    public class MaterialUtilizationDetail : TrackUserOperation
    {
        [Key]
        public int MaterialUtilzationDetlId { get; set; }
        public int MaterialUtilizationId { get; set; }
        public virtual MaterialUtilization MaterialUtilization { get; set; }
        [Display(Name = "Utilized Qty")]
        public decimal UtilizedQuantity { get; set; }
        [Display(Name = "Utilization Of Item")]
        public decimal UtilizationOfItem { get; set; }//[ utilized quantity divided by material issued]
        [Display(Name = "Material Wastage")]
        public decimal MaterialWastage { get; set; }
        public string Remark { get; set; }
    }
    public class MaterialDemand : TrackUserOperation
    {
        [Key]
        public int MaterialDemandId { get; set; }
        public int MaterialUtilizationId { get; set; }
        public virtual MaterialUtilization MaterialUtilization { get; set; }
        [Display(Name = "Additional Demand")]
        public decimal AdditionalDemand { get; set; }
        public string Remark { get; set; }
    }
    public class MethodOfWork
    {
        [Key]
        public int MethodOfWorkId { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
    }
    /// <summary>
    /// These classes, the classes that are related to ResourceUtilization are going to be used for standardazation purpose
    /// only. for actual daily or monthly resource utilization, WorkProgress related models are added.
    /// </summary>
    public class ResourceUtilizationPeriod : TrackUserOperation
    {
        [Key]
        public int ResourceUtilizationPeriodId { get; set; }
        [Display(Name = "Period")]
        public int ConstructionPeriodId { get; set; }
        [Display(Name = "Project")]
        public int ProjectInformationId { get; set; }
        public string Description { get; set; }

        public virtual ConstructionPeriod Period { get; set; }
        public virtual ProjectInformation Project { get; set; }
    }
    public class WorkProgressPeriod : TrackUserOperation
    {
        [Key]
        public int WorkProgressPeriodId { get; set; }

        [Display(Name = "Period")]
        public int ConstructionPeriodId { get; set; }
        [Display(Name = "Project")]
        public int ProjectInformationId { get; set; }
        public string Description { get; set; }

        public virtual ConstructionPeriod Period { get; set; }
        public virtual ProjectInformation Project { get; set; }
    }
    public class MonthlyWorkProgress : TrackUserOperation
    {
        [Key]
        public int MonthlyWorkProgressId { get; set; }
        public int WorkItemPerProjectId { get; set; }
        //public virtual WorkItemPerProject WorkItemPerProject { get; set; }
        public int WorkProgressPeriodId { get; set; }
        public virtual WorkProgressPeriod WorkProgressPeriod { get; set; }

    }
    public class DetailedWorkProgress : TrackUserOperation
    {
        [Key]
        public int DetailedWorkProgressId { get; set; }
        public int ResourceUtilizationDayId { get; set; }

        public int WorkItemPerProjectId { get; set; }
        public virtual WorkItemPerProject WorkItemPerProject { get; set; }

        // 
        // Current date, or time range
        public decimal PlannedQuantity { get; set; }
        public decimal PlannedAmount { get; set; }

        public decimal ActualQuantity { get; set; }
        public decimal ActualAmount { get; set; }
        //

        //
        // previous day, or time range
        public decimal PreviousPlannedQuantity { get; set; }
        public decimal PreviousPlannedAmount { get; set; }

        public decimal PreviousActualQuantity { get; set; }
        public decimal PreviousActualAmount { get; set; }
        //

        //
        //  todate
        public decimal TodatePlannedQuantity { get; set; }
        public decimal TodatePlannedAmount { get; set; }

        public decimal TodateActualQuantity { get; set; }
        public decimal TodateActualAmount { get; set; }
        //
    }

    public class DetailedWorkProgressViewModel
    {
        public int Id { get; set; }

        public int WorkItemPerProjectId { get; set; }
        [Display(Name ="Description")]
        public string ItemName { get; set; }
        [Display(Name = "No.")]
        public string ItemCode { get; set; }
        public bool IsSubCategory { get; set; }
        public int ParentId { get; set; }
        public string SubProjectTypeName { get; set; }
        public string BlockName { get; set; }
        public string StructureType { get; set; }
        [Display(Name = "Category")]
        public string WorkCategory { get; set; }
        [Display(Name = "Unit")]
        public string UoM { get; set; }
        [Display(Name = "Contract Qty.")]
        public decimal ContractQuantity { get; set; }
        [Display(Name = "Take off Qty")]
        public decimal TakeOffQuantity { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        // 
        // Current date, or time range
        [Display(Name = "Plan Qty this day")]
        public decimal PlannedQuantity { get; set; }
        [Display(Name = "Plan Amount this day")]
        public decimal PlannedAmount { get; set; }

        [Display(Name = "Actual Qty this day")]
        public decimal ActualQuantity { get; set; }
        [Display(Name = "Actual Amount this day")]
        public decimal ActualAmount { get; set; }
        //

        //
        // previous day, or time range
        [Display(Name = "Previous Plan Qty")]
        public decimal PreviousPlannedQuantity { get; set; }
        [Display(Name = "Previous Plan Amount")]
        public decimal PreviousPlannedAmount { get; set; }

        [Display(Name = "Previous Actual Qty")]
        public decimal PreviousActualQuantity { get; set; }
        [Display(Name = "Previous Actual Amount")]
        public decimal PreviousActualAmount { get; set; }
        //

        //
        //  todate
        [Display(Name = "Todate Plan Qty")]
        public decimal TodatePlannedQuantity { get; set; }
        [Display(Name = "Todate Plan Amount")]
        public decimal TodatePlannedAmount { get; set; }

        [Display(Name = "Todate Actual Qty")]
        public decimal TodateActualQuantity { get; set; }
        [Display(Name = "Todate Actual Amount")]
        public decimal TodateActualAmount { get; set; }
        //
    }

    public class ResourceUtilizationWorkItem : TrackUserOperation
    {
        [Key]
        public int ResourceUtilizationWorkItemId { get; set; }

        [Display(Name = "Method of Work")]
        public int MethodOfWorkId { get; set; }

        [Display(Name = "Work Item")]
        public int WorkItemPerProjectId { get; set; }
        [Display(Name = "Date")]
        public int ResourceUtilizationDayId { get; set; }

        //public int ResourceUtilizationPeriodId { get; set; }



        [Display(Name = "Item Name")]
        public string ItemName { get; set; }

        [Display(Name = "Unit")]
        public string UnitOfMeasurnment { get; set; }

        [Display(Name = "Executed Work")]
        public decimal ExecutedWork { get; set; }
        [Display(Name = "Work Duration  per day (in hrs)")]
        public decimal WorkDurationInHoursPerDay { get; set; }
        [Display(Name = "Output per day")]
        public decimal OutPutPerDay { get; set; }
        [Display(Name = "Direct Contract Unit Price")]
        public decimal DirectContractUnitPrice { get; set; }
        [Display(Name = "Income")]
        public decimal Income { get; set; }

        public decimal MaterialCost { get; set; }
        public decimal EquipmentCost { get; set; }
        public decimal LaborCost { get; set; }
        public decimal TotalDirectCost { get; set; }
        public decimal OverheadCost { get; set; }
        public decimal ProfitCost { get; set; }
        public decimal TotalCost { get; set; }

        public bool IsSubCategory { get; set; }
        public int ParentId { get; set; }
        public string SubProjectTypeName { get; set; }
        public string BlockName { get; set; }
        public string StructureyType { get; set; }
        [Display(Name = "Category")]
        public string WorkCategory { get; set; }

        public virtual MethodOfWork MethodOfWork { get; set; }
        //public virtual ResourceUtilizationDay Day { get; set; }
        public virtual WorkItemPerProject WorkItemPerProject { get; set; }

        public virtual ICollection<ResourceUtilizationMaterial> MaterialResources { get; set; }
        public virtual ICollection<ResourceUtilizationDirectLabour> LaborResources { get; set; }
        public virtual ICollection<ResourceUtilizationEquipment> EquipmentResources { get; set; }

    }
    public class ResourceUtilizationDay : TrackUserOperation
    {
        [Key]
        public int ResourceUtilizationDayId { get; set; }
        public int ResourceUtilizationPeriodId { get; set; }
        public string Name { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public virtual ResourceUtilizationPeriod ResourceUtilizationPeriod { get; set; }
    }
    public class ResourceUtilizationMaterial : TrackUserOperation
    {
        //Define direct cost [Material Cost] 
        [Key]
        public int ResourceUtilizationMaterialId { get; set; }

        [Display(Name = "Work Item")]
        public int ResourceUtilizationWorkItemId { get; set; }
        public virtual ResourceUtilizationWorkItem ResourceUtilizationWorkItem { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Material Type")]
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
    public class ResourceUtilizationDirectLabour : TrackUserOperation
    {
        //Define direct cost [Direct Labour Cost] 
        [Key]
        public int ResourceUtilizationDirectLabourId { get; set; }
        public int ResourceUtilizationWorkItemId { get; set; }
        public virtual ResourceUtilizationWorkItem ResourceUtilizationWorkItem { get; set; }

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
        [Display(Name = "Unit Factory")]
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
    public class ResourceUtilizationEquipment : TrackUserOperation
    {
        //Define direct cost [Equipment Cost] 
        [Key]
        public int ResourceUtilizationEquipmentId { get; set; }
        public int ResourceUtilizationWorkItemId { get; set; }
        public virtual ResourceUtilizationWorkItem ResourceUtilizationWorkItem { get; set; }

        [Required]
        [Display(Name = "Equipment Name")]
        public Guid FleetsID { get; set; }
        //[Display(Name = "Equipment Type")]
        //public Guid EquipmentTypeID { get; set; }
        //[Display(Name = "Equipment Category")]
        //public Guid EquipmentIdentityID { get; set; }
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
    public class MaterialExpenseProjectViewModel
    {
        public int BranchId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

    }
    public class ProjectMaterialExpense : TrackUserOperation
    {
        [Key]
        public int ProjectMaterialExpenseId { get; set; }
        //public int ProjectInformationId { get; set; }
        public int BranchId { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        //public virtual GLPeriod GLPeriod { get; set; }
        [Required]
        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        [Required]
        [DisplayName("Item Type")]
        public string ItemCode { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        // this month
        [Display(Name = "Delivered to proj.")]
        public decimal DeliveredQuantity { get; set; }
        [Display(Name = "Consume")]
        public decimal ConsumedQuantity { get; set; }
        [Display(Name = "Tran. in")]
        public decimal TranIn { get; set; }
        [Display(Name = "Tran. out")]
        public decimal TranOut { get; set; }
        // previous month
        [Display(Name = "Delivered to proj.")]
        public decimal DeliveredQuantityPrevMonth { get; set; }
        [Display(Name = "consume")]
        public decimal ConsumedQuantityPrevMonth { get; set; }
        [Display(Name = "Material on site")]
        public decimal MaterilOnSitePrevMonth { get; set; }
        // to date
        [Display(Name = "Delivered to proj.")]
        public decimal DeliveredQuantityToDate { get; set; }
        [Display(Name = "Consume")]
        public decimal ConsumedQuantityToDate { get; set; }
        [Display(Name = "ship")]
        public decimal ShipedQuantityToDate { get; set; }
        [Display(Name = "Material on site")]
        public decimal MaterialOnSiteToDate { get; set; }
        [Display(Name = "Stock Balance")]
        public decimal StockBalance { get; set; }
        // cost
        [Display(Name = "this Month")]
        public decimal CostOfConsumedMaterialThisMonth { get; set; }
        [Display(Name = "Previos")]
        public decimal CostOfConsumedMaterialPrevMonth { get; set; }
        [Display(Name = "To Date")]
        public decimal CostOfConsumedMaterialToDate { get; set; }
        [Display(Name = "Stock Amount")]
        public decimal StockAmount { get; set; }
        [Display(Name = "material on site Amount")]
        public decimal MaterialOnSiteAmount { get; set; }
        //public virtual ProjectInformation Project { get; set; }
    }

    public class ProjectDirectMaterialExpense : TrackUserOperation
    {
        [Key]
        public int ProjectDirectMaterialExpenseId { get; set; }
        //public int ProjectInformationId { get; set; }
        public int BranchId { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        //public virtual GLPeriod GLPeriod { get; set; }
        [Required]
        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        [Required]
        [DisplayName("Item Type")]
        public string ItemCode { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        // this month
        [Display(Name = "Delivered to proj.")]
        public decimal DeliveredQuantity { get; set; }
        [Display(Name = "Consume")]
        public decimal ConsumedQuantity { get; set; }
        [Display(Name = "Tran. in")]
        public decimal TranIn { get; set; }
        [Display(Name = "Tran. out")]
        public decimal TranOut { get; set; }
        // previous month
        [Display(Name = "Delivered to proj.")]
        public decimal DeliveredQuantityPrevMonth { get; set; }
        [Display(Name = "consume")]
        public decimal ConsumedQuantityPrevMonth { get; set; }
        [Display(Name = "Material on site")]
        public decimal MaterilOnSitePrevMonth { get; set; }
        // to date
        [Display(Name = "Delivered to proj.")]
        public decimal DeliveredQuantityToDate { get; set; }
        [Display(Name = "Consume")]
        public decimal ConsumedQuantityToDate { get; set; }
        [Display(Name = "ship")]
        public decimal ShipedQuantityToDate { get; set; }
        [Display(Name = "Material on site")]
        public decimal MaterialOnSiteToDate { get; set; }
        [Display(Name = "Stock Balance")]
        public decimal StockBalance { get; set; }
        // cost
        [Display(Name = "this Month")]
        public decimal CostOfConsumedMaterialThisMonth { get; set; }
        [Display(Name = "Previos")]
        public decimal CostOfConsumedMaterialPrevMonth { get; set; }
        [Display(Name = "To Date")]
        public decimal CostOfConsumedMaterialToDate { get; set; }
        [Display(Name = "Stock Amount")]
        public decimal StockAmount { get; set; }
        [Display(Name = "material on site Amount")]
        public decimal MaterialOnSiteAmount { get; set; }
        //public virtual ProjectInformation Project { get; set; }
    }

    public class ProjectIndirectMaterialExpense : TrackUserOperation
    {
        [Key]
        public int ProjectIndirectMaterialExpenseId { get; set; }
        //public int ProjectInformationId { get; set; }
        public int BranchId { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        //public virtual GLPeriod GLPeriod { get; set; }
        [Required]
        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        [Required]
        [DisplayName("Item Type")]
        public string ItemCode { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        // this month
        [Display(Name = "Delivered to proj.")]
        public decimal DeliveredQuantity { get; set; }
        [Display(Name = "Consume")]
        public decimal ConsumedQuantity { get; set; }
        [Display(Name = "Tran. in")]
        public decimal TranIn { get; set; }
        [Display(Name = "Tran. out")]
        public decimal TranOut { get; set; }
        // previous month
        [Display(Name = "Delivered to proj.")]
        public decimal DeliveredQuantityPrevMonth { get; set; }
        [Display(Name = "consume")]
        public decimal ConsumedQuantityPrevMonth { get; set; }
        [Display(Name = "Material on site")]
        public decimal MaterilOnSitePrevMonth { get; set; }
        // to date
        [Display(Name = "Delivered to proj.")]
        public decimal DeliveredQuantityToDate { get; set; }
        [Display(Name = "Consume")]
        public decimal ConsumedQuantityToDate { get; set; }
        [Display(Name = "ship")]
        public decimal ShipedQuantityToDate { get; set; }
        [Display(Name = "Material on site")]
        public decimal MaterialOnSiteToDate { get; set; }
        [Display(Name = "Stock Balance")]
        public decimal StockBalance { get; set; }
        // cost
        [Display(Name = "this Month")]
        public decimal CostOfConsumedMaterialThisMonth { get; set; }
        [Display(Name = "Previos")]
        public decimal CostOfConsumedMaterialPrevMonth { get; set; }
        [Display(Name = "To Date")]
        public decimal CostOfConsumedMaterialToDate { get; set; }
        [Display(Name = "Stock Amount")]
        public decimal StockAmount { get; set; }
        [Display(Name = "material on site Amount")]
        public decimal MaterialOnSiteAmount { get; set; }
        //public virtual ProjectInformation Project { get; set; }
    }

    public class ProjectLabourExpense { }
    public class ProjectEquipmentExpense { }

}
