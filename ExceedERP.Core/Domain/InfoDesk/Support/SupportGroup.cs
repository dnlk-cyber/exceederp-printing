using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.InfoDesk.Support
{
    public class SupportGroup_HelpDesk : TrackUserSettingOperation
    {
        public int SupportGroup_HelpDeskId { get; set; }
        [Required]
        public string Name { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Branch", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        //public virtual Branch Branch { get; set; }
        [Display(Name = "Placement_BusinessUnit", ResourceType = typeof(Localization.Resources))]
        public int? BusinessUnitId { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
    }
    public class BranchVM
    {
        public int BranchId { get; set; }
        public string Name { get; set; }
    }
    public class BussinesUnitVM
    {
        public int BusinessUnitId { get; set; }
        public string Name { get; set; }
    }
    public class OrgStructureVM
    {
        public int OrgStructureId { get; set; }
        public string Name { get; set; }
    }
}
