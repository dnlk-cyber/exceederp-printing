
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetInsurances;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{


    public partial class FleetInsurance : Operations
    {
        [Key]
        public int FleetInsuranceId { get; set; }
        [Display(Name = "Equipment Category")]
        public Guid EquipmentCategoryId { get; set; }
        [Display(Name = "Equipment Type")]
        public Guid EquipmentTypeId { get; set; }
        [Display(Name = "Equipment Name")]
        public Guid EquipmentNameId { get; set; }
        [NotMapped]
        public bool AddToHistory { get; set; }
        [NotMapped]
        public string PlateNumber { get; set; }
        public decimal Payment { get; set; }
        public Guid FleetsID { get; set; }
        [Display(Name = "Cover Type")]
        public int? InsuranceCoverTypeId { get; set; }
        public enum InsuranceTypeenum { TypeOne, TypeTwo, TypeThree, Comprehensive }
        [Required]
        [DisplayName("Company ")]
        public int InsuranceCompanyId { get; set; }

        [DisplayName("Premium Cost")]
        [Required]
        public decimal PremiumCost { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName(" Due Date ")]
        public DateTime? DueDate { get; set; }
        [DisplayName("Effective Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ExpireDate { get; set; }
        [NotMapped]
        public bool HasExpired { get; set; }
        public string Attachment { get; set; }
        public string Description { get; set; }
        [Display(Name = "Policy No")]
        public string PolicyNo { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? HazardOfExpiryDate { get; set; }

        public string ThirdParty { get; set; }
        public FleetInsuranceTypeEnum InsuranceType { get; set; }
        [Display(Name = "Third Party")]
        public decimal ThirdPartyInsurance { get; set; }
        [Display(Name = "Comprehensive")]
        public decimal ComprehensiveInsurance { get; set; }
        public bool IsPaymentRequestGenerated {get;set;}
        [NotMapped]
        public StatusEnum StatusEnum { get; set; }
    }
    public enum StatusEnum {
        Expired,
        Active
    }
    public partial class FleetInsuranceHistory : Operations
    {
        [Key]
        public int FleetInsuranceHistoryId { get; set; }
        public int FleetInsuranceId { get; set; }
        [Display(Name = "Equipment Category")]
        public Guid EquipmentCategoryId { get; set; }
        [Display(Name = "Equipment Type")]
        public Guid EquipmentTypeId { get; set; }
        [Display(Name = "Equipment Name")]
        public Guid EquipmentNameId { get; set; }
        public decimal Payment { get; set; }
        public Guid FleetsID { get; set; }
        public enum InsuranceTypeenum { TypeOne, TypeTwo, TypeThree, Comprehensive }
        [Required]
        [DisplayName("Company ")]
        public int InsuranceCompanyId { get; set; }
        [DisplayName("Premium Cost")]
        [Required]
        public decimal PremiumCost { get; set; }
        public int? InsuranceCoverTypeId { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName(" Due Date ")]
        public DateTime? DueDate { get; set; }
        [DisplayName("Effective Date")]   
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ExpireDate { get; set; } 
        [NotMapped]
        public bool HasExpired { get; set; }
        public bool IsPaymentRequestGenerated { get; set; }
        public string Attachment { get; set; }
        public string Description { get; set; }
       
        [Display(Name = "Policy No")]
        public string PolicyNo { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? HazardOfExpiryDate { get; set; }
       
        public string ThirdParty { get; set; }
        public FleetInsuranceTypeEnum InsuranceType { get; set; }
        [Display(Name ="Third Party")]
        public decimal ThirdPartyInsurance { get; set; }    
        [Display(Name ="Comprehensive")]
        public decimal ComprehensiveInsurance { get; set; }

    }
    public class FleetInsuranceValidation:TrackUserSettingOperation
    {
        public int FleetInsuranceValidationId { get; set; }
        public int FleetInsuranceId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
    public class FleetInsurancePaymentRequestValidation : TrackUserSettingOperation
    {
        public int FleetInsurancePaymentRequestValidationId { get; set; }
        public int FleetInsurancePaymentRequestId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
    public class FleetInsurancePaymentRequest : Operations
    {
        public int FleetInsurancePaymentRequestId { get; set; }

        [DisplayName("Employee Name")]
        public int EmployeeId { get; set; }       
        [DisplayName("Duration From")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DurationFrom { get; set; }
        [DisplayName(" Duration To")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DurationTo { get; set; }
        
        [DisplayName("Reason of Payment")]
        public string ReasonofPayment { get; set; }
        [DisplayName("Request Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RequestDate { get; set; }
        public int FleetInsuranceId { get; set; }
        [Display(Name = "Equipment Category")]
        public Guid EquipmentCategoryId { get; set; }
        [Display(Name = "Equipment Type")]
        public Guid EquipmentTypeId { get; set; }
        [Display(Name = "Equipment Name")]
        public Guid EquipmentNameId { get; set; }
        
        public decimal Payment { get; set; }
        public Guid FleetsID { get; set; }
        [Display(Name = "Cover Type")]
        public int? InsuranceCoverTypeId { get; set; }
        public enum InsuranceTypeenum { TypeOne, TypeTwo, TypeThree, Comprehensive }
        public FleetInsuranceTypeEnum InsuranceType { get; set; }
        [Required]
        [DisplayName("Company ")]
        public int InsuranceCompanyId { get; set; }
        public bool IsBudgetApproved { get; set; }
        public string BudgetApprovedBy { get; set; }
        public DateTime? BudgetApprovedTime { get; set; }
        [DisplayName("Refference")]
        public string Refference { get; set; }
        public bool Paid { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountDue { get; set; }
        // Finace 
        public bool IsFinanceSendForApproval { get; set; }
        public string FinanceSendForApprovalBy { get; set; }
        public DateTime? FinanceSendForApprovalTime { get; set; }
        public bool IsFinanceApproved { get; set; }
        public string FinanceApprovedBy { get; set; }
        public DateTime? FinanceApprovedTime { get; set; }
        public bool IsAuthorized { get; set; }
        public string AuthorizedBy { get; set; }
        public DateTime? AuthorizedTime { get; set; }

    }
    public class FleetInsuranceDocument
    {
        public int FleetInsuranceDocumentId { get; set; }
        public int FleetInsuranceId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }

    }
    public enum FleetInsuranceTypeEnum
    {
        [Display(Name ="Third party and comprehensive")]
        Third_Party_and_Comprehensive,
        [Display(Name ="Political Violence")]
        Political_Violence,
        [Display(Name ="Motor Own Damage")]
        Motor_Own_Damage
    }
}
 