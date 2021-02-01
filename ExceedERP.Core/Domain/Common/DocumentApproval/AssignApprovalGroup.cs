using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common.DocumentApproval
{
    public class AssignApprovalGroup:TrackUserSettingOperation
    {
        public int AssignApprovalGroupId { get; set; }
        [Display(Name = "Opertating Unit")]
        public string OpertatingUnit { get; set; }
        [Display(Name = "Organization")]
        public string Organization { get; set; }
        [Display(Name = "Position")]
        public string Position { get; set; }
        [Display(Name = "Job")]
        public string Job { get; set; }
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }//specific approver

        public ICollection<AssignApprovalGroupAssignment> AssignApprovalGroupAssignments { get; set; }
    }
}