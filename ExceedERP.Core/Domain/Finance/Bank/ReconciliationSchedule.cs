using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.Finance.Bank
{
    public enum RecordType
    {
        Standard,
        Beginning,
        Exception

    }
    public class ReconciliationSchedule : Operations
    {
        public int ReconciliationScheduleId { get; set; }
        [Required]
        [Display(Name = "Bank Account")]
        public string BankAccount { get; set; }
        [Display(Name = "Statement Number")]
        public string StatementNum { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Display(Name = "Bank Ending Balance")]
        public decimal BankEndingBalance { get; set; }
        public virtual ICollection<ReconciliationBankServiceCharge> ServiceCharges { get; set; }
        public virtual ICollection<ReconciliationBankServiceChargeDistribution> ReconciliationBankServiceChargeDistributions { get; set; }
        public virtual ICollection<ReconciliationBankServiceChargeValidation> ReconciliationBankServiceChargeValidations { get; set; }
    }

    public class ReconciliationBankServiceCharge : TrackUserOperation
    {
        public int ReconciliationBankServiceChargeId { get; set; }
        public int ReconciliationScheduleId { get; set; }
        public virtual ReconciliationSchedule Schedule { get; set; }
        public decimal Amount { get; set; }
        public decimal ExRate { get; set; }
        public decimal AmountNonFun { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public string Remark { get; set; }
       
    }

    public class ReconciliationBankServiceChargeValidation : TrackUserOperation
    {
        public int ReconciliationBankServiceChargeValidationId { get; set; }
        public int ReconciliationScheduleId { get; set; }
        public virtual ReconciliationSchedule Schedule { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }

    public class ReconciliationBankServiceChargeDistribution : VoucherDistribution
    {
        public int ReconciliationBankServiceChargeDistributionId { get; set; }
        public int ReconciliationScheduleId { get; set; }
        public virtual ReconciliationSchedule Schedule { get; set; }
    }
}
