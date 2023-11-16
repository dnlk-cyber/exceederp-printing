using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP.Travel
{
   
    public enum TravelPaymentType
    {
        PerDiem,
        Fuel,
        Transport,
        Other
    }
    public class ApTravelOrder : DepartmentNewOperations
    {
        public int ApTravelOrderId { get; set; }
        [Required]
        [Display(Name = "Name")]
        public int EmployeeId { get; set; }
       
        [Required]
        [DisplayName("Work Unit")]
        public int OrgStructureId { get; set; }
        [Required]
        [Display(Name = "Purpose of Travel")]
        public String PurposeOfTravel { get; set; }
        [Display(Name = "Request Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RequestDate { get; set; }
        [Display(Name = "Advance Amount")]//from line ,read
        public decimal AdvanceAmount { get; set; }
        [Display(Name = "Total Duration")]//from line departure and return ,read
        public decimal TotalDuration { get; set; }

        [Display(Name = "Reference")]
        public string Reference { get; set; }

        [Display(Name = "Transport Request")]
        public string TransportReference { get; set; }
        public bool IsForOther { get; set; }
        public bool Paid { get; set; }
        //budget controll
        public bool IsBudgetApproved { get; set; }
        public string BudgetApprovedBy { get; set; }
        public DateTime? BudgetApprovedTime { get; set; }       
        public int BranchId { get; set; }
        public int BusinessUnitId { get; set; }

        [Display(Name = "From Branch/Project")]
        public int FromBranchId { get; set; }
        [Display(Name = "From Business Unit")]
        public int FromBusinessUnitId { get; set; }
        //for access or view
        public int CreatedAtBranchId { get; set; }
        public int CreatedAtBusinessUnitId { get; set; }
        public int CreatedAtOrgStructureId { get; set; }
        public virtual ICollection<ApTravelOrderAdvancePayment> ApTravelOrderAdvancePayments { get; set; }
        public virtual ICollection<ApTravelOrderDeparture> ApTravelOrderDepartures { get; set; }
        public virtual ICollection<ApTravelOrderValidation> ApTravelOrderValidations { get; set; }
        public virtual ICollection<ApTravelOrderDepartmentValidation> ApTravelOrderDepartmentValidations { get; set; }
        public virtual ICollection<ApTravelOrderBudgetValidation> ApTravelOrderBudgetValidations { get; set; }
        public virtual ICollection<ApTravelOrdertAttachment> Attachments { get; set; }
        //public virtual ICollection<ApTravelOrderDistribution> ApTravelOrderDistributions { get; set; }
        public Guid ? VehicleDistributionID { get; set; }
    }

    public class ApTravelOrdertAttachment
    {
        public int ApTravelOrdertAttachmentId { get; set; }
        public int ApTravelOrderId { get; set; }
        public virtual ApTravelOrder TravelOrder { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
