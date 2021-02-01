using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AR
{
    public class ARSetupReceiptBalanceAccount:TrackUserSettingOperation
    {
        public int ARSetupReceiptBalanceAccountID { get; set; }
        //[Required]
        [Display(Name = "Unidentified")]
        public string UnidentifiedAccountCode { get; set; }
        //[Required]
        [Display(Name = "On Account")]
        public string OnAccountAccountCode { get; set; }
        //[Required]
        [Display(Name = "Unapplied")]
        public string UnappliedAccountCode { get; set; }
        [Required]
        [Display(Name = "Prepayment")]
        public string PrepaymentAccountCode { get; set; }
        //[Required]
        [Display(Name = "Branch")]
        public string Branch { get; set; }
    }
}
