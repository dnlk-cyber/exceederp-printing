using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP.Travel
{

    public class ApTravelExpenseClaim : NewOperations
    {
        public int ApTravelExpenseClaimId { get; set; }
        [Display(Name = "Travel Order No.")]
        public int? ApTravelOrderId { get; set; }
        [Required]
        [Display(Name = "Name")]
        public int EmployeeId { get; set; }
        [Required]
        [DisplayName("Branch")]
        public int BranchId { get; set; }
        [Required]
        [DisplayName("Work Unit")]
        public int OrgStructureId { get; set; }
        [Display(Name = "Purpose of Travel")]
        public String PurposeOfTravel { get; set; }
        [Display(Name = "Claim Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ClaimDate { get; set; }
       // public string PaymentMethod { get; set; }
        [Display(Name = "Reference")]
        public string Reference { get; set; }
        public bool IsForOther { get; set; }
        public bool Paid { get; set; }
        public virtual ICollection<ApTravelExpenseExpense> Expenses { get; set; }
        public virtual ICollection<ApTravelExpensePayment> Payments { get; set; }
        public virtual ICollection<ApTravelExpenseRefund> Refunds { get; set; }
        public virtual ICollection<ApTravelExpenseValidation> Validations { get; set; }
        public virtual ICollection<ApTravelExpenseDistribution> Distributions { get; set; }
    }
}
