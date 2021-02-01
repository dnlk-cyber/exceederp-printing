using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP.Travel
{
    public class NewOperations : Operations
    {
        public string ForwardedFrom { get; set; }
        public string ForwardedTo { get; set; }
        public bool IsFinalApproval { get; set; }
    }
    public enum TravelPaymentType
    {
        PerDiem,
        Fuel,
        Transport,
        Other
    }
    public class ApTravelOrder : NewOperations
    {
        public int ApTravelOrderId { get; set; }
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
        [Display(Name = "Request Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RequestDate { get; set; }
        [Display(Name = "Advance Amount")]//from line ,read
        public decimal AdvanceAmount { get; set; }
       
        [Display(Name = "Reference")]
        public string Reference { get; set; }
        public bool IsForOther { get; set; }
        public bool Paid { get; set; }
        public virtual ICollection<ApTravelOrderAdvancePayment> ApTravelOrderAdvancePayments { get; set; }
        public virtual ICollection<ApTravelOrderDeparture> ApTravelOrderDepartures { get; set; }
        public virtual ICollection<ApTravelOrderValidation> ApTravelOrderValidations { get; set; }
        public virtual ICollection<ApTravelOrderDistribution> ApTravelOrderDistributions { get; set; }
    }
}
