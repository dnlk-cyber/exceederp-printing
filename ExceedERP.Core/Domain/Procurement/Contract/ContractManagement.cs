using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Procurement.Settings;

namespace ExceedERP.Core.Domain.Procurement.Contract
{
    public enum ContractStatus
    {
        New,
        Terminated,
        Active,
        Closed
    }
    public enum DeliveryType
    {
        FullDelivery,
        PartialDelivery
    }
   
    public class ContractManagement : NewOperations
    {
        public int ContractManagementID { get; set; }
        [Display(Name = "Branch/Project")]
        public int BranchId { get; set; }
        [Required]
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }
        [Display(Name = "New Supplier")]//manual supplier
        public bool IsNewSupplier { get; set; }
        [Display(Name = "Supplier ID")]
        public string SupplierId { get; set; }
        [Display(Name = "Contract Ref No.")]
        public string ContractRefNo { get; set; }
        [Display(Name = "Tender Ref No.")]
        public string TenderRefNo { get; set; }
        [Display(Name = "Procurement Ref No.")]
        public int ProcurementRefNo { get; set; }
        [Display(Name = "Sign Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime SigningDate { get; set; }
        [Display(Name = "Contract Type")]
        public ProcurementCategory ContractType { get; set; }
        [Display(Name = "Purchase Type")]
        public PurchaseCategory PurchaseCategory { get; set; }
        [Display(Name = "Delivery Place")]
        public string DeliveryPlace { get; set; }
        [Display(Name = "Delivery Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DeliveryDate { get; set; }
        [Display(Name = "Delivery Type")]
        public DeliveryType DeliveryType { get; set; }
        [Display(Name = "Contract Amount")]
        public decimal ContractAmount { get; set; }
        [Display(Name = "Retention %")]
        public decimal RetentionPercentage { get; set; }
        [Display(Name = "Advance Taken")]
        public decimal Advance { get; set; }
        public int ProcurementOfficer { get; set; }
        [DisplayName("Transferred To Payment?")]
        public bool IsTransferredToPayment { get; set; }
        [DisplayName("Transferred To Followup?")]
        public bool IsTransferredToFollowUp { get; set; }
        public bool IsFullfilled { get; set; }
        [DisplayName("Is Paid")]
        public bool IsPaid { get; set; }
        [DisplayName("Contract Status")]
        public ContractStatus ContractStatus { get; set; }
        
        public virtual ICollection<ContractManagementItem> ContractManagementItem { get; set; }
        public virtual ICollection<ContractPaymentSchedule> ContractPaymentSchedules { get; set; }
        public virtual ICollection<ContractRenewal> Renewals { get; set; }
        public virtual ICollection<ContractAttachment> Attachements { get; set; }
        public virtual ICollection<ContractPayment> Payments { get; set; }
        public virtual ICollection<ContractManagementValidation> ContaContractManagementValidations { get; set; }
        //termination,penality,renewal
    }
}
