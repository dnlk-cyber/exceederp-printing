using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Guarantee
{

    public enum GuaranteeType
    {
        PerformanceBond,
        BidBond,
        AdvancePayment,
        CashReceiept
    }

    public enum ValidityStatus
    {
        Released,
        Forfeited,
        Expired
    }

    public class BankGuarantee : TrackUserOperation
    {
        public int BankGuaranteeID { get; set; }

        [Display(Name = "Guarantee Type")]
        public GuaranteeType GuaranteeType { get; set; }
        [Display(Name = "Guarantee Ref No.")]
        public string GuaranteeRefNo { get; set; }
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }
        [Display(Name = "Issuing Currency")]
        public string Currency { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }
        [Display(Name = "Issuing Bank Branch")]
        public string BankBranch { get; set; }
        [Display(Name = "Bank Address")]
        public string BankAddress { get; set; }
        [Display(Name = "Validity period (Days)")]
        public int ValidityPeriod { get; set; }
        [Display(Name = "Validity Status")]
        public ValidityStatus ValidityStatus { get; set; }
        [Display(Name = "Guarantee Presenter Name")]
        public string GuaranteePresenterName { get; set; }
        [Display(Name = "Guarantee Presenter Address")]
        public string GuaranteePresenterAddress { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }
}
