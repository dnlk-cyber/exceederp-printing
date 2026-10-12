using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.HRM.LeaveManagment
{
    public class LeavePaid:TrackUserOperation 

    {
        [Key ]
        public int LeavePaidId { get; set; }
        [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
        [Display(Name = "HR_LeaveManagment_LeavePaid_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        [Required ]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required ]
        [Display(Name = "HR_LeaveManagment_LeavePaid_LeavePeriod", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }

        // <summary>
        /// Unexpired Accumulated Balance fetched from Leave Entitlement History (e.g., 399 days)
        /// </summary>
        /// 
        [Display(Name = "Accumulated Leave Days")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public decimal? AccumulatedLeaveDays { get; set; }

        /// <summary>
        /// Total Days Paid = Current Leave Days + Accumulated Leave Days
        /// </summary>
        [Display(Name = "Total Paid Days")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public decimal TotalPayableDays { get; set; }
        /// <summary>
        /// Total days allowed for payout (e.g., 40 days)
        /// </summary>
        [Required]
        [Display(Name = "HR_LeaveManagment_LeavePaid_Amount", ResourceType = typeof(Localization.Resources))]
        public double Amount { get; set; }

        // --- SALARY & RATE BREAKDOWN ---

        [Display(Name = "Basic Salary")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public decimal BasicSalary { get; set; }

        [Display(Name = "Daily Rate")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public decimal DailyRate { get; set; }

        // --- GROSS, TAX, AND NET AMOUNTS ---

        /// <summary>
        /// Total Gross Payout before tax deduction (e.g., 90,908.80)
        /// </summary>
        [Display(Name = "Gross Amount (ETB)")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public decimal GrossAmount { get; set; }

        /// <summary>
        /// Legacy/General Paid Amount field (Maps to Gross Amount in Birr)
        /// </summary>
        [Display(Name = "Paid Amount (ETB)")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public decimal AmountInBirr { get; set; }

        /// <summary>
        /// Total Calculated Tax (e.g., 31,818.00)
        /// </summary>
        [Display(Name = "Tax Payable (ETB)")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public decimal TaxPayable { get; set; }

        /// <summary>
        /// Take-home amount after tax deduction (e.g., 59,090.80)
        /// </summary>
        [Display(Name = "Net Pay (ETB)")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public decimal NetPay { get; set; }

        // --- STATUS & WORKFLOW FLAGS ---

        // ---------------------------------------------

        [Display(Name = "Is Calculated")]
        public bool IsCalculated { get; set; }
        [Display(Name = "Sent to Finance")]
        public bool IsSentToFinance { get; set; }

        [Display(Name = "Date Sent")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateSent { get; set; }

        [Display(Name = "Sent By")]
        public string SentBy { get; set; }

        public int? PaymentRequestId { get; set; }

        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }


    }


    public class LeavePaidDeductionDetail : TrackUserOperation
    {
        [Key]
        public int LeavePaidDeductionDetailId { get; set; }

        [Required]
        [ForeignKey("LeavePaid")]
        public int LeavePaidId { get; set; }
        public virtual LeavePaid LeavePaid { get; set; }

        /// <summary>
        /// Populated when deducted from regular active LeavesEntitle (Nullable)
        /// </summary>
        public int? LeaveEntitleId { get; set; }

        /// <summary>
        /// Populated when deducted from historical LeaveEntitlementHistories (Nullable)
        /// </summary>
        public int? LeaveEntitlementHistoryId { get; set; }

        /// <summary>
        /// Identifies whether deduction source is "Active" or "History"
        /// </summary>
        public string SourceType { get; set; }

        /// <summary>
        /// Exact number of days deducted from this specific record
        /// </summary>
        [Required]
        public decimal DeductedDays { get; set; }
    }
}