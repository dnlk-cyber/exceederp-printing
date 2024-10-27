using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Inventory
{
    public class BankUserAssignment : TrackUserSettingOperation
    {
        public int BankUserAssignmentId { get; set; }
        [Required]
        [Display(Name = "Bank Account")]
        public string BankAccount { get; set; }
        [Required]
        [Display(Name = "User")]
        public string User { get; set; }
       
    }
  


}