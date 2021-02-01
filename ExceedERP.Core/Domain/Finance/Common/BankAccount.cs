using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.Common
{
    public class BankAccount : TrackUserOperation
    {
        public int BankAccountId { get; set; }
        [Required]
        [Display(Name = "Account Name")]
        public string AccountName { get; set; }
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }
        [Display(Name = "Bank Name")]
        public string BankName { get; set; }
        [Display(Name = "Bank Branch")]
        public string BankBranch { get; set; }
        [Required]
        [Display(Name = "Bank Adress")]
        public string BankAdress { get; set; }
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Description")]
        public string AccountDesc { get; set; }
        [Display(Name = "Account Control")]
        public string AccountControl { get; set; }
        [Display(Name = "Account Use")]
        public string AccountUse { get; set; }
        [Display(Name = "Active?")]
        public bool Status { get; set; }
    }
}
