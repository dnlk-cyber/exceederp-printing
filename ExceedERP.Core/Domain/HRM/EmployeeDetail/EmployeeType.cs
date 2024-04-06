using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class EmployeeType : TrackUserOperation
    {
        [Key]
        public int EmployeeTypeId { get; set; }
        [Display(Name = "Name", ResourceType = typeof(Localization.Resources))]
        public string Name { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewOutgoingLetter_Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}
