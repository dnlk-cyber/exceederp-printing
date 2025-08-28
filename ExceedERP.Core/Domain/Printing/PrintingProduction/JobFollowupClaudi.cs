//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;

//namespace ExceedERP.Core.Domain.Printing.PrintingProduction
//{
//    // Separate workflow state management
//    public class WorkflowState
//    {
//        public bool IsChecked { get; set; }
//        public bool IsApproved { get; set; }
//        public string CheckedBy { get; set; }
//        public string ApprovedBy { get; set; }
//        public DateTime? CheckedTime { get; set; }
//        public DateTime? ApprovedTime { get; set; }
//    }

//    public enum JobStatus
//    {
//        Created = 0,
//        PrePressInProgress = 1,
//        PrePressCompleted = 2,
//        PressInProgress = 3,
//        PressCompleted = 4,
//        PostPressInProgress = 5,
//        PostPressCompleted = 6,
//        Finished = 7,
//        Shipped = 8
//    }

//    public enum WorkshopType
//    {
//        PrePress = 1,
//        Press = 2,
//        PostPress = 3
//    }

//    // Main job entity - simplified
//    public class PrintingJob : BaseEntity
//    {
//        [Key]
//        public int PrintingJobId { get; set; }

//        [Display(Name = "Job No.")]
//        public string JobNumber { get; set; }

//        [Display(Name = "Customer")]
//        public int CustomerId { get; set; }

//        public PrintJobType PrintJobType { get; set; }
//        public BindingStyle BindingStyle { get; set; }
//        public string JobDescription { get; set; }
//        public decimal Quantity { get; set; }
//        public int Pages { get; set; }

//        // Current job status
//        public JobStatus CurrentStatus { get; set; } = JobStatus.Created;

//        // Workshop states
//        public WorkflowState PrePressState { get; set; } = new WorkflowState();
//        public WorkflowState PressState { get; set; } = new WorkflowState();
//        public WorkflowState PostPressState { get; set; } = new WorkflowState();

//        // Navigation properties
//        public virtual List<JobWorkshopTask> Tasks { get; set; } = new List<JobWorkshopTask>();
//        public virtual List<JobMaterial> Materials { get; set; } = new List<JobMaterial>();
//    }

//    // Workshop tasks - simplified structure
//    public class JobWorkshopTask : BaseEntity
//    {
//        [Key]
//        public int TaskId { get; set; }

//        public int PrintingJobId { get; set; }
//        public WorkshopType Workshop { get; set; }
//        public string ActivityName { get; set; }
//        public int? MachineTypeId { get; set; }
//        public int AssignedOperatorId { get; set; }

//        // Planning
//        public string UnitOfMeasure { get; set; }
//        public decimal PlannedQuantity { get; set; }
//        public decimal EstimatedHours { get; set; }

//        // Execution tracking
//        public TaskStatus Status { get; set; } = TaskStatus.Pending;
//        public decimal ActualQuantity { get; set; }
//        public decimal ActualHours { get; set; }

//        // Workflow state
//        public bool IsChecked { get; set; }
//        public bool IsApproved { get; set; }
//        public string CheckedBy { get; set; }
//        public string ApprovedBy { get; set; }
//        public DateTime? CheckedTime { get; set; }
//        public DateTime? ApprovedTime { get; set; }

//        // Navigation
//        public virtual PrintingJob Job { get; set; }
//        public virtual List<TaskExecution> Executions { get; set; } = new List<TaskExecution>();
//        public virtual List<TaskIssue> Issues { get; set; } = new List<TaskIssue>();
//    }

//    public enum TaskStatus
//    {
//        Pending = 0,
//        InProgress = 1,
//        Completed = 2,
//        OnHold = 3
//    }

//    // Daily execution records
//    public class TaskExecution : BaseEntity
//    {
//        [Key]
//        public int ExecutionId { get; set; }

//        public int TaskId { get; set; }
//        public int OperatorId { get; set; }

//        [DataType(DataType.DateTime)]
//        public DateTime StartTime { get; set; }

//        [DataType(DataType.DateTime)]
//        public DateTime EndTime { get; set; }

//        public decimal QuantityProduced { get; set; }
//        public decimal WastageAmount { get; set; }
//        public string Notes { get; set; }

//        // Calculated properties
//        public double DurationHours => (EndTime - StartTime).TotalHours;

//        public virtual JobWorkshopTask Task { get; set; }
//        public virtual List<ExecutionDelay> Delays { get; set; } = new List<ExecutionDelay>();
//        public virtual List<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();
//    }

//    // Issues/delays tracking
//    public class TaskIssue : BaseEntity
//    {
//        [Key]
//        public int IssueId { get; set; }

//        public int TaskId { get; set; }
//        public IssueType Type { get; set; }
//        public string Description { get; set; }
//        public DateTime ReportedTime { get; set; }
//        public DateTime? ResolvedTime { get; set; }
//        public string Resolution { get; set; }
//        public int ReportedBy { get; set; }

//        public virtual JobWorkshopTask Task { get; set; }
//    }

//    public enum IssueType
//    {
//        QualityIssue = 1,
//        MachineBreakdown = 2,
//        MaterialShortage = 3,
//        OperatorAbsence = 4,
//        Other = 5
//    }

//    public class ExecutionDelay : BaseEntity
//    {
//        [Key]
//        public int DelayId { get; set; }

//        public int ExecutionId { get; set; }
//        public string Reason { get; set; }
//        public string Description { get; set; }
//        public int DurationMinutes { get; set; }

//        public virtual TaskExecution Execution { get; set; }
//    }

//    public class MaintenanceRecord : BaseEntity
//    {
//        [Key]
//        public int MaintenanceId { get; set; }

//        public int ExecutionId { get; set; }
//        public string Reason { get; set; }
//        public DateTime IssueReportedTime { get; set; }
//        public DateTime? MaintenanceStartTime { get; set; }
//        public DateTime? MaintenanceEndTime { get; set; }
//        public int TechnicianId { get; set; }
//        public string Resolution { get; set; }

//        public virtual TaskExecution Execution { get; set; }
//    }

//    // Materials allocation
//    public class JobMaterial : BaseEntity
//    {
//        [Key]
//        public int JobMaterialId { get; set; }

//        public int PrintingJobId { get; set; }
//        public EstimationMaterialType MaterialType { get; set; }
//        public int MaterialItemId { get; set; }
//        public string UnitOfMeasure { get; set; }
//        public decimal AllocatedQuantity { get; set; }
//        public decimal UsedQuantity { get; set; }

//        public virtual PrintingJob Job { get; set; }
//    }

//    // Base classes
//    public abstract class BaseEntity
//    {
//        public DateTime DateCreated { get; set; } = DateTime.Now;
//        public DateTime? DateModified { get; set; }
//        public string CreatedBy { get; set; }
//        public string ModifiedBy { get; set; }
//        public bool IsDeleted { get; set; } = false;
//    }
//}