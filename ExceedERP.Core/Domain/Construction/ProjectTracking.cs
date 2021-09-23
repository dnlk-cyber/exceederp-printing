using ExceedERP.Core.Domain.Common;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Construction
{
    class ProjectTracking
    {
    }
    public class EarnedValueManagementTerm
    {
        [Key]
        public int EarnedValueManagementTermId { get; set; }
        public EarnedValueManagementTermCategory Category { get; set; }
        public string Term { get; set; }
        public string Abbreviation { get; set; }
        public string Definition { get; set; }
        public string Formula { get; set; }
    }

    public class EarnedValuePerProject : TrackUserOperation
    {
        public int EarnedValuePerProjectId { get; set; }

        public int ProjectInformationId { get; set; }
        //public string Year { get; set; }
        //public Months Month { get; set; }
        [Display(Name = "Period")]
        public int ConstructionPeriodId { get; set; }
        [Display(Name = "Year")]
        public string GlFiscalYear { get; set; }

        [Display(Name = " BAC ")]
        public decimal BAC { get; set; }

        public decimal BCWS { get; set; }
        [Display(Name = "BCWS")]
        public decimal BCWSRaw { get; set; }

        public decimal BCWP { get; set; }
        [Display(Name = "BCWP")]
        public decimal BCWPRaw { get; set; }

        public decimal ACWP { get; set; }
        [Display(Name = "ACWP")]
        public decimal ACWPRaw { get; set; }

        public decimal PCP { get; set; }
        public decimal PCA { get; set; }
        public decimal CV { get; set; }
        public decimal CVPercentage { get; set; }
        public decimal CPI { get; set; }
        public decimal SV { get; set; }
        public decimal SVPercentage { get; set; }
        public decimal SPI { get; set; }
        public decimal CR { get; set; }
        public decimal TCPI { get; set; }
        public decimal TSPI { get; set; }
        public decimal EAC { get; set; }
        public decimal VAC { get; set; }
        public decimal ETC { get; set; }
        public decimal ISAC { get; set; }

        //public virtual ProjectInformation Project { get; set; }

    }
    public class EVMProjectGeneralInfomationViewModel
    {
        public decimal ExpectedProfitRate { get; set; }
        public decimal ExpectedProfit { get; set; }
        public decimal BAC { get; set; }
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
        [DisplayName("Type")]
        public AvailableProjectTypes ProjectType { get; set; }
    }
    //public class EVMPerItemPeriodMapping
    //{
    //    [Key]
    //    public int EVMPerItemPeriodMappingId { get; set; }
    //    public int ConstructionPeriodId { get; set; }
    //    public int ProjectInformationId { get; set; }
    //    public string Remark { get; set; }

    //    public virtual ProjectInformation ProjectInformation { get; set; }
    //    public virtual ConstructionPeriod ConstructionPeriod { get; set; }
    //}
    public class EarnedValuePerItemPeriodViewModel
    {
        public EarnedValuePerItemPeriodViewModel()
        {
            ItemList = new List<EarnedValuePerItem>();
        }
        public string Name { get; set; }

        public List<EarnedValuePerItem> ItemList { get; set; }
    }
    public class EarnedValuePerItem : TrackUserOperation
    {
        [Key]
        public int EarnedValuePerItemId { get; set; }
        [Display(Name = "Work Description")]
        public int WorkItemPerProjectId { get; set; }
        public string Unit { get; set; }
        [Display(Name = "Budget")]
        public decimal BudgetedQuantity { get; set; }
        [Display(Name = "Planned")]
        public decimal PlannedQuanity { get; set; }
        [Display(Name = "Executed")]
        public decimal ExecutedQuantity { get; set; }
        [Display(Name = "Estimated")]
        public decimal EstimatedUnitPrice { get; set; }
        [Display(Name = "Actual")]
        public decimal ActualUnitPrice { get; set; }

        //public string Year { get; set; }
        //public Months Month { get; set; }
        public int EarnedValueManagementPeriodId { get; set; }
        public virtual EarnedValueManagementPeriod Period { get; set; }
        public string GlFiscalYear { get; set; }

        [Display(Name = " BAC ")]
        public decimal BAC { get; set; }
        public decimal BCWS { get; set; }
        public decimal BCWP { get; set; }
        public decimal ACWP { get; set; }
        public decimal PCP { get; set; }
        public decimal PCA { get; set; }
        public decimal CV { get; set; }
        public decimal CVPercentage { get; set; }
        public decimal CPI { get; set; }
        public decimal SV { get; set; }
        public decimal SVPercentage { get; set; }
        public decimal SPI { get; set; }
        public decimal CR { get; set; }
        public decimal TCPI { get; set; }
        public decimal TSPI { get; set; }
        public decimal EAC { get; set; }
        public decimal VAC { get; set; }
        public decimal ETC { get; set; }
        public decimal ISAC { get; set; }
        //public virtual WorkItemPerProject WorkItem { get; set; }

    }
    public class EarnedValueManagementPeriod : TrackUserOperation
    {
        [Key]
        public int EarnedValueManagementPeriodId { get; set; }
        public int ConstructionPeriodId { get; set; }
        public int ProjectInformationId { get; set; }
        public string Remark { get; set; }

        public virtual ProjectInformation ProjectInformation { get; set; }
        public virtual ConstructionPeriod ConstructionPeriod { get; set; }
    }
    public class EarnedValueManagementTermDetail
    {
        [Key]
        public int EarnedValueManagementTermDetailId { get; set; }
        public int EarnedValueManagementTermId { get; set; }

        public string OperandOne { get; set; }
        public string OperandTwo { get; set; }
        public EVMOperation Operator { get; set; }
        public string HiddenFormulaValue { get; set; }
        public string ConstantValue { get; set; }
        public string Formula { get; set; }
        public string OperandOneHiddenValue { get; set; }
        public string OperandTwoHiddenValue { get; set; }

        public virtual EarnedValueManagementTerm EarnedValueManagementTerm { get; set; }
    }
    public class EarnedValueManagementTermFormulaConstant : TrackUserOperation
    {
        [Key]
        public int EarnedValueManagementTermFormulaConstantId { get; set; }
        public int EarnedValueManagementTermDetailId { get; set; }
        [Display(Name ="Operation")]
        public EVMOperation Operator { get; set; }
        public decimal ConstantValue { get; set; }
        public string FormulaValue { get; set; }
        public string HiddenFormulaValue { get; set; }

        public virtual EarnedValueManagementTermDetail EarnedValueManagementTermDetail { get; set; }
    }
    public class EVMTermOperandComponent
    {
        [Key]
        public int EVMTermOperandComponentId { get; set; }
        public int EarnedValueManagementTermDetailId { get; set; }
        public string OperandOne { get; set; }
        public string OperandTwo { get; set; }
        public EVMOperation Operator { get; set; }
        public string HiddenFormulaValue { get; set; }
        public string ConstantValue { get; set; }

        public EVMParentOperand ParentOperand { get; set; }
        public string Formula { get; set; }

        public virtual EarnedValueManagementTermDetail EarnedValueManagementTermDetail { get; set; }
    }

    public class DelayType
    {
        public int DelayTypeId { get; set; }
        public WorkDelayTypes Type { get; set; }
        public string Description { get; set; }
        public string Abbriviation { get; set; }
    }
    public class DelayAnalysisTask
    {
        public DelayAnalysisTask()
        {
            this.ChildrenTasks = new HashSet<DelayAnalysisTask>();
        }
        [Key]
        public int DelayAnalysisTaskId { get; set; }
        public int ID { get; set; }
        public Nullable<int> ParentID { get; set; }
        public int OrderID { get; set; }
        public string Title { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString =  "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime Start { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString =  "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime End { get; set; }

        public decimal PercentComplete { get; set; }
        public bool Expanded { get; set; }
        public bool Summary { get; set; }
        public bool HasDelay { get; set; }
        public string DelayDuration { get; set; }

        public virtual ICollection<DelayAnalysisTask> ChildrenTasks { get; set; }
        public virtual DelayAnalysisTask ParentTask { get; set; }
        public virtual Task RootTask { get; set; }
    }

    public class DelayAnalysisTaskViewModel : IGanttTask
    {
        public int TaskID { get; set; }
        public int? ParentID { get; set; }
        public int RootTaskId { get; set; }
        public string Title { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString =  "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        private DateTime start;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString =  "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
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
        [DisplayFormat(DataFormatString =  "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        private DateTime end;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString =  "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
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

        public bool Summary { get; set; }
        public bool Expanded { get; set; }
        public decimal PercentComplete { get; set; }
        public int OrderId { get; set; }
        public bool HasDelay { get; set; }

        public int ProjectInformationId { get; set; }

        [Display(Name = "Planned Duration")]
        public int PlannedDuration { get; set; }

        [Display(Name = "Delay Duration")]
        public string DelayDuration { get; set; }

        // added after adding schedule versions
        public  int TaskVersion { get; set; }

        public DelayAnalysisTask ToEntity()
        {
            return new DelayAnalysisTask
            {
                DelayAnalysisTaskId = TaskID,
                ID = RootTaskId,
                ParentID = ParentID,
                Title = Title,
                Start = Start,
                End = End,
                Summary = Summary,
                Expanded = Expanded,
                PercentComplete = PercentComplete,
                OrderID = OrderId,
            };
        }


    }
    public partial class DelayTaskDependency
    {
        [Key]
        public int DelayTaskDependencyId { get; set; }
        public int ID { get; set; }
        public int PredecessorID { get; set; }
        public int SuccessorID { get; set; }
        public int Type { get; set; }
        public virtual Dependency RootDependency { get; set; }
    }
    public class DelayTaskDependencyViewModel : IGanttDependency
    {
        public int DependencyID { get; set; }
        public int RootDependencyId { get; set; }
        public int PredecessorID { get; set; }
        public int? SuccessorID { get; set; }
        public DependencyType Type { get; set; }

        [Display(Name = "Predecessor Task")]
        public string predecessorTaskname { get; set; }

        public DelayTaskDependency ToEntity()
        {
            return new DelayTaskDependency
            {
                DelayTaskDependencyId = DependencyID,
                ID = RootDependencyId,
                PredecessorID = PredecessorID,
                SuccessorID = (int)SuccessorID,
                Type = (int)Type
            };
        }
    }
    public class WorkDelay : TrackUserOperation
    {
        [Key]
        public int WorkDelayId { get; set; }

        [Display(Name = "Work Item/Task")]
        public int ID { get; set; }

        //public int DelayAnalysisTaskId { get; set; }

        [Display(Name = "As Planned Duration")]
        public int PlannedDuration { get; set; }

        [Display(Name = "Chronology of Delay")]
        public int Order { get; set; }
        public string Description { get; set; }
        [Display(Name = "Delay Type")]
        public int DelayTypeId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString =  "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Start { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString =  "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime End { get; set; }

        [Display(Name = "Delay Duration")]
        public int Duration { get; set; }
        //public virtual DelayAnalysisTask Task { get; set; }
        public virtual Task Task { get; set; }
        public virtual DelayType DelayType { get; set; }

    }
}
