using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Construction
{
    public class ProjectWorkBreakDown : TrackUserOperation
    {
        [Key]
        public int ProjectWorkBreakDownId { get; set; }

        [DisplayName("WB Code")]
        [StringLength(100)]
        public string WBSCode { get; set; }

        [DisplayName("Description")]
        [StringLength(200)]
        public string Description { get; set; }

        public int ProjectInformationId { get; set; }

        public int OriginalDuration { get; set; }
        public int RemainingDuration { get; set; }

        public int ConfigurationId { get; set; }

        public virtual ProjectInformation Project { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }

    }
    public class ProjectTaskSchedule : TrackUserOperation
    {
        [Key]
        public int ProjectTaskScheduleId { get; set; }

        [DisplayName("Task Name")]
        [StringLength(100)]
        public string TaskName { get; set; }

        [DisplayName("Duration")]
        [StringLength(50)]
        public string Duration { get; set; }

        [DisplayName("Start Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DisplayName("End Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [DisplayName("Complete %")]
        [StringLength(50)]
        public string CompletePercentage { get; set; }

        public int ProjectInformationId { get; set; }
        public virtual ProjectInformation Project { get; set; }
        public int RecurrenceId { get; set; }
        public string StartTimezone { get; set; }
        public string EndTimezone { get; set; }
        public string Description { get; set; }
        public bool IsAllDay { get; set; }
        public string RecurrenceException { get; set; }
        public string RecurrenceRule { get; set; }

    }

    public class TaskViewModel : IGanttTask
    {
        public int TaskID { get; set; }
        public int? ParentID { get; set; }
        public int Version { get; set; }
        public int? PreviousVersionTaskId { get; set; }
        //public int? WorkItemDurationId { get; set; }
        public Nullable<int> WorkItemDurationId { get; set; }

        public string Title { get; set; }

        [Display(Name = "Id")]
        public int ProjectSpecificId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        private DateTime start;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Start
        {
            get
            {
                return start;
            }
            set
            {
                start = value.ToUniversalTime();
            }
        }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        private DateTime end;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime End
        {
            get
            {
                return end;
            }
            set
            {
                end = value.ToUniversalTime();
            }
        }

        public decimal BudgetCost { get; set; }
        public decimal TotalQuantityOfWork { get; set; }
        public decimal DirectCost { get; set; }
        public decimal MaterialCost { get; set; }
        public decimal LabourCost { get; set; }
        public decimal EquipmentCost { get; set; }

        public bool Summary { get; set; }
        public bool Expanded { get; set; }
        public decimal PercentComplete { get; set; }
        public int OrderId { get; set; }
        //public int ProjectWorkBreakDownId { get; set; }

        //public string ProjectWorkBreakDownCode { get; set; }
        public int ProjectInformationId { get; set; }

        [Display(Name = "Original Duration")]
        public decimal ActualDuration { get; set; }

        [Display(Name = "Remaining Duration")]
        public int RemainingDuration { get; set; }

        public decimal EstimatedDuration { get; set; }

        public int WorkCategoryId { get; set; }
        public int ConfigurationId { get; set; }
        public int BlockId { get; set; }
        public int SubProjectTypeId { get; set; }

        public string SubProjectTypeName { get; set; }
        public string BlockName { get; set; }
        public string StructureyType { get; set; }
        public string WorkCategory { get; set; }
        //public virtual ProjectWorkBreakDown ProjectWorkBreakDown { get; set; }
        //public virtual WorkItemDuration WorkItemDuration { get; set; }
        public virtual string Predecessors { get; set; }

        [Display(Name = "Qty Prev Ver.")]
        public decimal ExecutedQuantityTillPrevVersion { get; set; }
        [Display(Name = "Qty This Ver.")]
        public decimal ExecutedQuantityThisVersion { get; set; }
        [Display(Name = "Qty Todate ")]
        public decimal ExecutedQuantityToDate { get; set; }

        [Display(Name = "Amnt. Prev Ver.")]
        public decimal ExecutedAmountTillPrevVersion { get; set; }
        [Display(Name = "Amnt. This Ver.")]
        public decimal ExecutedAmountThisVersion { get; set; }
        [Display(Name = "Amnt. Todate")]
        public decimal ExecutedAmountToDate { get; set; }

        public bool IsSubActivityCreatedInSchedule { get; set; }


        public Task ToEntity()
        {
            return new Task
            {
                ID = TaskID,
                ParentID = ParentID,
                Version = Version,
                PreviousVersionTaskId = PreviousVersionTaskId,
                //WorkItemDurationId = WorkItemDurationId,
                //WorkItemDuration = WorkItemDuration,
                WorkItemDurationId = WorkItemDurationId,
                WorkCategoryId = WorkCategoryId,
                ConfigurationId = ConfigurationId,
                BlockId = BlockId,
                SubProjectTypeId = SubProjectTypeId,
                WorkCategory = WorkCategory,
                StructureyType = StructureyType,
                BlockName = BlockName,
                SubProjectTypeName = SubProjectTypeName,

                ExecutedQuantityTillPrevVersion = ExecutedQuantityTillPrevVersion,
                ExecutedQuantityThisVersion = ExecutedQuantityThisVersion,
                ExecutedQuantityToDate = ExecutedQuantityToDate,

                ExecutedAmountThisVersion = ExecutedAmountThisVersion,
                ExecutedAmountTillPrevVersion = ExecutedAmountTillPrevVersion,
                ExecutedAmountToDate = ExecutedAmountToDate,

                Title = Title,
                Start = Start,
                End = End,
                BudgetCost = BudgetCost,
                TotalQuantityOfWork = TotalQuantityOfWork,
                DirectCost = DirectCost,
                MaterialCost = MaterialCost,
                LabourCost = LabourCost,
                EquipmentCost = EquipmentCost,
                EstimatedDuration = EstimatedDuration,
                ActualDuration = ActualDuration,
                ProjectSpecificId = ProjectSpecificId,
                Summary = Summary,
                Expanded = Expanded,
                PercentComplete = PercentComplete,
                OrderID = OrderId,
                IsSubActivityCreatedInSchedule = IsSubActivityCreatedInSchedule,
                
            };
        }
    }

    public partial class Task
    {
        public Task()
        {
            this.Tasks1 = new HashSet<Task>();
        }

        public int ID { get; set; }
        //public Nullable<int> WorkItemDurationId { get; set; }
        public Nullable<int> WorkItemDurationId { get; set; }

        public Nullable<int> ParentID { get; set; }
        public int OrderID { get; set; }
        //public int ProjectWorkBreakDownId { get; set; }
        public string Title { get; set; }

        [Display(Name ="Id")]
        public int ProjectSpecificId { get; set; }
        public int Version { get; set; }
        public Nullable<int> PreviousVersionTaskId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime Start { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime End { get; set; }

        public decimal BudgetCost { get; set; }

        public decimal TotalQuantityOfWork { get; set; }
        public decimal DirectCost { get; set; }
        public decimal MaterialCost { get; set; }
        public decimal LabourCost { get; set; }
        public decimal EquipmentCost { get; set; }
        public decimal EstimatedDuration { get; set; }
        public decimal ActualDuration { get; set; }
        public decimal PercentComplete { get; set; }
        public bool Expanded { get; set; }
        public bool Summary { get; set; }

        public int WorkCategoryId { get; set; }
        public int ConfigurationId { get; set; }
        public int BlockId { get; set; }
        public int SubProjectTypeId { get; set; }

        public string SubProjectTypeName { get; set; }
        public string BlockName { get; set; }
        public string StructureyType { get; set; }
        public string WorkCategory { get; set; }

        // used for back log quantity schedling
        public decimal ExecutedQuantityTillPrevVersion { get; set; }
        public decimal ExecutedQuantityThisVersion { get; set; }
        public decimal ExecutedQuantityToDate { get; set; }

        public decimal ExecutedAmountTillPrevVersion { get; set; }
        public decimal ExecutedAmountThisVersion { get; set; }
        public decimal ExecutedAmountToDate { get; set; }
        //
        // used when further spliting exists in schedule
        public bool IsSubActivityCreatedInSchedule { get; set; }

        public virtual ICollection<Task> Tasks1 { get; set; }
        public virtual Task Task1 { get; set; }
        public virtual WorkItemDuration WorkItemDuration { get; set; }
    }

    public class DependencyViewModel : IGanttDependency
    {
        public int DependencyID { get; set; }
        [Display(Name ="Predecessor")]
        public int PredecessorID { get; set; }
        [Display(Name ="Succeessor")]
        public int? SuccessorID { get; set; }
        public DependencyType Type { get; set; }

        [Display(Name = "Title")]
        public string predecessorTaskname { get; set; }

        [Display(Name = "Title")]
        public string SuccessorTaskname { get; set; }

        [Display(Name = "Lag")]
        public int LagTime { get; set; }


        public Dependency ToEntity()
        {
            return new Dependency
            {
                ID = DependencyID,
                PredecessorID = PredecessorID,
                SuccessorID = (int)SuccessorID,
                Type = (int)Type,
                LagTime = LagTime
            };
        }
    }
    public class ResourceAssignmentViewModel
    {
        public int ID { get; set; }
        public int TaskID { get; set; }
        public int ResourceID { get; set; }
        public decimal Units { get; set; }

        public ResourceAssignment ToEntity()
        {
            return new ResourceAssignment()
            {
                ID = ID,
                TaskID = TaskID,
                ResourceID = ResourceID,
                Units = Units
            };
        }
    }
    public class ResourceAssignment
    {
        public int ID { get; set; }
        public int TaskID { get; set; }
        public int ResourceID { get; set; }
        public decimal Units { get; set; }
    }
    public partial class Dependency
    {
        public Dependency() { }
        public Dependency(int predecessor, int succssor, int type)
        {
            this.PredecessorID = predecessor;
            this.SuccessorID = succssor;
            this.Type = type;
            this.LagTime = LagTime;
        }

        public int ID { get; set; }
        public int PredecessorID { get; set; }
        public int SuccessorID { get; set; }
        public int Type { get; set; }
        public int LagTime { get; set; }

    }

    public class PredecssorViewModel
    {
        // the key
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public DependencyType Type { get; set; }
        // lag time in days
        [Display(Name = "Lag")]
        public int LagTime { get; set; }

        // project specific counter
        [Display(Name = "Id")]
        public int ProjectSpecificId { get; set; }
    }
    public class ResourceViewModel
    {
        [Display(Name = "Resource Id")]
        public int ID { get; set; }
        public string Name { get; set; }

        [Display(Name = "Display Color")]
        public string Color { get; set; }

        public int ResourceTypeId { get; set; }


        public virtual ResourceType Type { get; set; }

        [Display(Name = "Resource Type")]
        public string ResourceType { get; set; }

        [Display(Name = "Unit of Measure")]
        public string UnitofMeasure { get; set; }

        [Display(Name = "Primary Role")]
        public string PrimaryRole { get; set; }

        public Resource ToEntity()
        {
            return new Resource
            {
                ID = ID,
                Name = Name,
                Color = Color,

                ResourceTypeId = ResourceTypeId,
                Type = Type,
                UnitofMeasure = UnitofMeasure,
                PrimaryRole = PrimaryRole
            };
        }
    }
    public partial class Resource
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }

        public int ResourceTypeId { get; set; }
        public virtual ResourceType Type { get; set; }
        public string UnitofMeasure { get; set; }
        public string PrimaryRole { get; set; }
    }

    public partial class ResourceType
    {
        public int ResourceTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Resource> Resources { get; set; }
    }

   

    public class DiagramViewModel
    {
        public int TaskID { get; set; }
        public int ProjectSpecificId { get; set; }
        public int? ParentID { get; set; }

        public string Title { get; set; }

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString =  "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //private DateTime start;

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString =  "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //public DateTime Start
        //{
        //    get
        //    {
        //        return start;
        //    }
        //    set
        //    {
        //        start = value.ToUniversalTime();
        //    }
        //}

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString =  "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //private DateTime end;

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString =  "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //public DateTime End
        //{
        //    get
        //    {
        //        return end;
        //    }
        //    set
        //    {
        //        end = value.ToUniversalTime();
        //    }
        //}
        public string End;
        public string Start;
        public int Duration { get; set; }
        public int ProjectInformationId { get; set; }
        public int ProjectWorkBreakDownId { get; set; }
        public int PathWeight { get; set; }
        public string Resources { get; set; }

        //[Display(Name = "Original Duration")]
        //public int OriginalDuration { get; set; }

        //[Display(Name = "Remaining Duration")]
        //public int RemainingDuration { get; set; }

        public virtual List<DiagramViewModel> Successors { get; set; }
    }

    public class TaskFromEstimation
    {
        [Key]
        public int TaskFromEstimationId { get; set; }

        [Display(Name = "Category")]
        [Required]
        public int WorkCategoryId { get; set; }

        //public virtual WorkCategory Category { get; set; }
        public string Category { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Start { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime End { get; set; }

        [Display(Name = "Estimated Cost")]
        public decimal EstimatedTotalCost { get; set; }

        public int ProjectTypeId { get; set; }
        //public virtual ProjectType ProjectType { get; set; }

        [Display(Name = "Parent")]
        public int ParentId { get; set; }//work item if it has parent

        public int? BlockId { get; set; }
    }

    public class TaskDetail : TrackUserOperation
    {
        [Key]
        public int TaskDetailId { get; set; }
        [Display(Name = "Work Item")]
        public int ID { get; set; }

        [Display(Name = "Period")]
        public int ConstructionPeriodId { get; set; }

        [Display(Name = "Qty of Work")]
        public decimal PlannedQuantityOfWork { get; set; }

        [Display(Name = "Material Cost")]
        public decimal PlannedMaterialCost { get; set; }

        [Display(Name = "Equipment Cost")]
        public decimal PlannedEquipmentCost { get; set; }

        [Display(Name = "Labor Cost")]
        public decimal PlannedLaborCost { get; set; }

        [Display(Name = "Direct Cost")]
        public decimal PlannedDirectCost { get; set; }

        [Display(Name = "Profit Cost")]
        public decimal PlannedProfitCost { get; set; }

        [Display(Name = "Overhead Cost")]
        public decimal PlannedOverHeadCost { get; set; }

        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }

        public virtual Task Task { get; set; }

    }
    public class TaskDetailBreakDown : TrackUserOperation
    {
        [Key]
        public int TaskDetailBreakDownId { get; set; }
        public int TaskDetailId { get; set; }
        [Display(Name = "Time Range")]
        public int ConstructionSubPeriodId { get; set; }


        [Display(Name = "Qty of Work")]
        public decimal PlannedQuantityOfWork { get; set; }

        [Display(Name = "Material Cost")]
        public decimal PlannedMaterialCost { get; set; }

        [Display(Name = "Equipment Cost")]
        public decimal PlannedEquipmentCost { get; set; }

        [Display(Name = "Labor Cost")]
        public decimal PlannedLaborCost { get; set; }

        [Display(Name = "Direct Cost")]
        public decimal PlannedDirectCost { get; set; }

        [Display(Name = "Profit Cost")]
        public decimal PlannedProfitCost { get; set; }

        [Display(Name = "Overhead Cost")]
        public decimal PlannedOverHeadCost { get; set; }

        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }

        public virtual TaskDetail TaskDetail { get; set; }
    }
    // 
    // Models for schedule revision
    public class ScheduleRevision : Operations
    {
        [Key]
        public int ScheduleRevisionId { get; set; }
        public int ProjectInformationId { get; set; }
        public int Version { get; set; }
        public ProjectScheduleStatus ScheduleStatus { get; set; }
        public Nullable<DateTime> DateOfApproval { get; set; }
        public string ChangeDescription { get; set; }

        public ProjectInformation Project { get; set; }
    }
    //
    public class ScheduleRevisionValidation : VoucherValidation
    {
        [Key]
        public int ScheduleRevisionValidationId { get; set; }
        public int ScheduleRevisionId { get; set; }
        public virtual ScheduleRevision ScheduleRevision { get; set; }
    }
    public class ScheduleRevisionProjectViewModel
    {
        public int ProjectInformationId { get; set; }
        public string Name { get; set; }

        public string Code { get; set; }

        public int BranchId { get; set; }
        //public virtual Branch Branch { get; set; }
        public decimal ContractAmount { get; set; }

        public string ProjectScope { get; set; }

        public int? EmployeeId { get; set; }
        public string Client { get; set; }
        public string Consultant { get; set; }
        public AvailableProjectTypes ProjectType { get; set; }
        //dates
        public ProjectStatus ProjectStatus { get; set; }
        public bool PreviousVersionNotApproved { get; set; }
    }
}
