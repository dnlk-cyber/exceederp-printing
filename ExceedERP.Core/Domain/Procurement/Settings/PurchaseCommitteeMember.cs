using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Settings
{
    public enum PurchaseCommitteeType
    {
        Proforma = 1,
        Tender = 2
    }

    public class PurchaseCommitteeMember : TrackUserOperation
    {
        public int PurchaseCommitteeMemberId { get; set; }
        public PurchaseCommitteeType CommitteeType { get; set; }
        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        [Display(Name = "Title")]
        public string Title { get; set; }
    }
}
