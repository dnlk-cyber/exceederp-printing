using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Finance.GL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Budget
{
    public class PlanCategory : DepartmentNewOperations
    {
     
        public int PlanCategoryID { get; set; }

        public int BudgetCategoryID { get; set; }

        public string Name { get; set; } //remove this, replaced by the above

        [Display(Name = "Budget Year")]
        [Required]
        public int GlFiscalYearId { get; set; }

        
        [Display(Name = "Initial Total")]
        public decimal GrandTotal { get; set; }
        
        [Display(Name = "Current Total")]
        public decimal CurrentTotal { get; set; }
     
        [Display(Name = "Progress % ")]
        public decimal Progress { get; set; }
        
        [Display(Name = "Actual GL  ")]
        public decimal ActualConsumption { get; set; }
 
        [Display(Name = "Budget Status")]
        public string BudgetStatus { get; set; }
 
        [Display(Name = "Budget Type")]
        public string BudgetType { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        [Display(Name = "Branch/Project")]
        public int BranchId { get; set; }
        [Display(Name = "Business Unit")]
        public int BusinessUnitId { get; set; }
        [Display(Name = "Department")]
        public int OrgStuructureId { get; set; }
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        //for revision
        public bool IsRevisionSendForApproval { get; set; }
        public string RevisionSendForApprovalBy { get; set; }
        public DateTime? RevisionSendForApprovalTime { get; set; }
        public bool IsOpenforrevision { get; set; }
        public bool IsRevisionOnlineApproved { get; set; }
        public string RevisionOnlineApprovedBy { get; set; }
        public DateTime? RevisionOnlineApprovedTime { get; set; }
        public virtual ICollection<BudgetPlanAttachment> BudgetPlanAttachments { get; set; }
    }

    public class BudgetPlanAttachment
    {
        public int BudgetPlanAttachmentId { get; set; }
        public int PlanCategoryID { get; set; }
        public virtual PlanCategory PlanCategory { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
