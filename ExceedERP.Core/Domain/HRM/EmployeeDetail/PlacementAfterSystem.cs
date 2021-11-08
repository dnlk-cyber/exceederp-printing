using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
   public class PlacementAfterSystem
    {
        public int PlacementAfterSystemId { get; set; }
        public int PlacementId { get; set; }
        public int EmployeeId { get; set; }
        [Display(Name ="Department")]
        public int OrgStructureId { get; set; }
        [Display(Name = "Job Title")]
        public int LookupId { get; set; }
        [Display(Name = "Rank")]
        public int RankId { get; set; }
        [Display(Name = "Step")]
        public int StepId { get; set; }
        public decimal Salary { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime From { get; set; }
        [Display(Name = "From")]
        public string FromAm { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime To { get; set; }
        [Display(Name = "To")]
        public string ToAm { get; set; }
        [Display(Name = "Assignment Type")]
        public AssignmentType AssignmentType { get; set; }
        [Display(Name = "Resp Level")]
        public ResponsibilityLevel ResponsibilityLevel { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        [Display(Name = "Is Head")]
        public bool IsHead { get; set; }
        [Display(Name = "Is Current")]
        public bool IsCurrent { get; set; }
        [Display(Name = "Emp Type")]
        public EmployeeStatus EmployementType { get; set; }

    }
}
