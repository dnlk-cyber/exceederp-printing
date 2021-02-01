using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP
{
    public enum AdvanceType
    {
        Employee,
        Supplier,
        Purchaser
    }
    public class ApSetupReceiptBalanceAccount : TrackUserSettingOperation
    {
        public int ApSetupReceiptBalanceAccountId { get; set; }
        [Required]
        [Display(Name = "Prepayment")]
        public string PrepaymentAccountCode { get; set; }
        [Required]
        [Display(Name = "Prepayment Type")]
        public AdvanceType Type { get; set; }
    }
}
