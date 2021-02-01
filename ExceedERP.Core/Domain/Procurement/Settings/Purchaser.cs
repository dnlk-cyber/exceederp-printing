using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Settings
{
    public class Purchaser : TrackUserOperation
    {
        public int PurchaserId { get; set; }
        [Required]
        [Display(Name = "Purchaser")]
        public int EmployeeId { get; set; }
        [Required]
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        public bool Enable { get; set; }
    }
}