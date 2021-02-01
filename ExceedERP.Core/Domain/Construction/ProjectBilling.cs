using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Inventory;

namespace ExceedERP.Core.Domain.Construction
{
    public class ProjectBilling { }
    /// <summary>
    /// This take off is used for registering actual executed quantity of work
    /// </summary>
    public class ProjectTakeOff : TrackUserOperation
    {
        [Key]
        public int ProjectTakeOffId { get; set; }

        [DisplayName("Take of Number")]
        //[StringLength(100)]
        public string TakeOffNo { get; set; }
        [Display(Name = "Is Reinforcement bar take off ?")]
        public bool IsReinforcementbarTakeOff { get; set; }
        public int WorkItemPerProjectId { get; set; }

        [Display(Name = "Project")]
        public int ProjectInformationId { get; set; }
        public virtual ICollection<ProjectTakeOffLine> ProjectTakeOffLine { get; set; }
        public string Remark { get; set; }
    }
    public class ProjectTakeOffLine : TrackUserOperation
    {
        [Key]
        public int ProjectTakeOffLineId { get; set; }

        [DisplayName("No.")]
        public decimal No { get; set; }

        [Display(Name = "Unit")]
        public int UnitOfMeasurementId { get; set; }

        [Display(Name = "D")]
        public decimal Depth { get; set; }
        [Display(Name ="L")]
        public decimal Length { get; set; }
        [Display(Name ="W")]
        public decimal Width { get; set; }

        [DisplayName("Quantity")]
        public decimal Quantity { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }
        public int ProjectTakeOffId { get; set; }
        public virtual ProjectTakeOff ProjectTakeOff { get; set; }
       
        public bool IsDeductive { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfInformation { get; set; }

    }
    public class ProjectTakeOffLineForKg : TrackUserOperation
    {
        [Key]
        public int ProjectTakeOffLineForKgId { get; set; }
        public string Location { get; set; }
        public string Shape { get; set; }
        [Display(Name = "Diameter")]
        public int DeformedBarId { get; set; }
        public virtual DeformedBar DeformedBar { get; set; }

        //public int UnitRateId { get; set; }
        //public virtual UnitRate UnitRate { get; set; }
        public decimal Length { get; set; }
        public decimal Diameter { get; set; }
        public decimal TotalWeight { get; set; }

        [Display(Name = "No.of Bars")]
        public decimal NoOfBars { get; set; }
        [Display(Name = "No.of elem.")]
        public decimal NoOfElement { get; set; }
        [Display(Name = "Total")]
        public decimal TotalBar { get; set; }
        [Display(Name = "Total Length(m)")]
        public decimal TotalLength { get; set; }
        //[Display(Name ="Total Weight")]
        public decimal GrandTotal { get; set; }

        [DisplayName("Description")]
        //[StringLength(200)]
        public string Description { get; set; }
        public string Type { get; set; }

        public int ProjectTakeOffId { get; set; }
        public virtual ProjectTakeOff ProjectTakeOff { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfInformation { get; set; }
    }
    public class ProjectPaymentCertificate : Operations
    {
        [Key]
        public int ProjectPaymentCertificateId { get; set; }
        public int ProjectInformationId { get; set; }

        //[Range(1, int.MaxValue)]
        [Display(Name = "Payment No.")]
        public int PaymentNumber { get; set; }

        [Required]
        [Display(Name ="BoQ Ref.")]
        public string BillReference { get; set; }
        public decimal ValueOfWorkExecutedToDate { get; set; }
        [Display(Name = "Tax type")]
        public int GLTaxRateID { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }

        [Display(Name = "Total including tax")]
        public decimal TotalWithVat { get; set; }

        // Deduction (ተቀናሽ)
        [Display(Name = "Previous Payments")]
        public decimal PreviousPayment { get; set; }
        [Display(Name = "Retention")]
        public decimal Retention { get; set; }
        [Display(Name = "Penality")]
        public decimal Penality { get; set; }
        [Display(Name ="Correction")]
        public decimal Correction { get; set; }
        [Display(Name ="Adv. Repayment")]
        public decimal AdvanceRepayment { get; set; }
        [Display(Name = "Others")]
        public decimal OtherDeductions { get; set; }
        [Display(Name ="Material on Site")]
        public decimal MaterialOnSite { get; set; }

        //
        [Display(Name = "Total Payable Amount (including tax)")]
        public decimal NetPayableAmount { get; set; }

        //[StringLength(100)]
        //public string TakeOffNo { get; set; }
        public virtual ProjectInformation Project { get; set; }
        //public virtual ICollection<ProjectPaymenCertificatetLine> ProjectPaymenCertificatetLine { get; set; }
        //public virtual ICollection<ProjectPaymenAdvance> ProjectPaymenAdvance { get; set; }
        //public virtual ICollection<ProjectPaymenRetention> ProjectPaymenRetention { get; set; }
    }
    public class PaymentCertificateViewModel
    {
        public int ProjectPaymentCertificateId { get; set; }
        public string PaymentNumber { get; set; }
    }
    public class ProjectPaymentCertificateValidation : VoucherValidation
    {
        public int ProjectPaymentCertificateValidationId { get; set; }
        public int ProjectPaymentCertificateId { get; set; }

        public virtual ProjectPaymentCertificate PaymentCertificate { get; set; }
    }
    public class ProjectPaymenCertificatetLine : TrackUserOperation
    {
        [Key]
        public int ProjectPaymenCertificatetLineId { get; set; }
        [DisplayName("Contract Amount")]
        public decimal ContractAmount { get; set; }
        [DisplayName("Supplementary Contract")]
        [StringLength(100)]
        public string SupplimentaryContract { get; set; }
        [DisplayName("Variation Order")]
        [StringLength(200)]
        public string VariationOrder { get; set; }
        [DisplayName("VAT (15%)")]
        public decimal VAT { get; set; }
        [DisplayName("Total Amount")]
        public decimal TotalAmount { get; set; }
        public int ProjectInformationId { get; set; }
        public virtual ProjectInformation ProjectInformation { get; set; }
    }
    public class ProjectAdvancePayment : TrackUserOperation
    {
        [Key]
        public int ProjectAdvancePaymentId { get; set; }
        public int ProjectInformationId { get; set; }
        public decimal TotalProjectAmount { get; set; }
        public decimal AdvancePaymentRate { get; set; }
        public decimal ApprovedPaymentRate { get; set; }
        public decimal Amount { get; set; }
        public decimal ApprovedAmount { get; set; }
        public ProjectInformation Project { get; set; }
    }
    public class ProjectPaymenAdvance : TrackUserOperation
    {
        [Key]
        public int ProjectPaymenAdvanceId { get; set; }
        [DisplayName("Taken Before VAT")]
        public decimal AdvanceTakenBeforeVat { get; set; }
        [DisplayName("Taken After VAT")]
        public decimal AdvanceTakenAfterVat { get; set; }
        [DisplayName("Total Before VAT")]
        public decimal TotalAdvanceTakenBeforeVat { get; set; }
        [DisplayName("Total After VAT")]
        public decimal TotalAdvanceTakenAfterVat { get; set; }
        [DisplayName("Outstanding Before VAT")]
        public decimal OutstandingBeforeVat { get; set; }
        [DisplayName("Outstanding After VAT")]
        public decimal OutstandingAfterVat { get; set; }
        public int ProjectInformationId { get; set; }
        public virtual ProjectInformation ProjectInformation { get; set; }
    }
    public class ProjectPaymenRetention : TrackUserOperation
    {
        [Key]
        public int ProjectPaymenRetentionId { get; set; }
        [DisplayName("Retention Before VAT")]
        public decimal RetentionBeforeVat { get; set; }
        [DisplayName("Retention After VAT")]
        public decimal RetentionAfterVat { get; set; }
        [DisplayName("Total Retention Before VAT")]
        public decimal TotalRetentionBeforeVat { get; set; }
        [DisplayName("Total Retention After VAT")]
        public decimal TotalRetentionAfterVat { get; set; }
        public int ProjectInformationId { get; set; }
        public virtual ProjectInformation ProjectInformation { get; set; }
    }

    public class ProjectPaymentAttribute : TrackUserOperation
    {
        [Key]
        public int ProjectPaymentAttributeId { get; set; }
        public ProjectPaymentAttributeName AttributeName { get; set; }
        public decimal Value { get; set; }
    }


    public class BoQOfExecutedWork : TrackUserOperation
    {
        [Key]
        public int BoQOfExecutedWorkId { get; set; }
        public int ProjectInformationId { get; set; }
        [Display(Name ="Reference No.")]
        public string BillReference { get; set; }
        [Display(Name ="Bill No.")]
        public int BillNumber { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public string Remark { get; set; }

        public virtual ProjectInformation Project { get; set; }

    }
    public class BoQOfExecutedWorkContent
    {
        [Key]
        public int BoQOfExecutedWorkContentId { get; set; }
        public int BoQOfExecutedWorkId { get; set; }
        //
        // for ordering the items
        [Display(Name = "Item No.")]
        public string ItemCode { get; set; }
        public int Index { get; set; }
        //
        public int WorkItemPerProjectId { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
      
        [Display(Name = "Unit")]
        public int UnitOfMeasurementId { get; set; }

        [Display(Name = "Quantity")]
        public decimal ContractQuantity { get; set; }
        public decimal QuantityTillPrevPayment { get; set; }
        public decimal QuantityThisPayment { get; set; }
        public decimal QuantityToDate { get; set; }

        [Display(Name = "Unit Rate")]
        public decimal UnitRate { get; set; }

        public decimal AmountTillPrevPayment { get; set; }
        public decimal AmountThisPayment { get; set; }
        public decimal AmountToDatePayment { get; set; }

        public int WorkCategoryId { get; set; }
        public int ConfigurationId { get; set; }

        public int ParentBoQContentId { get; set; }

        public string SubProjectTypeName { get; set; }
        public string BlockName { get; set; }
        public string StructureyType { get; set; }
        public string WorkCategory { get; set; }


        public virtual BoQOfExecutedWork BoQOfExecutedWork { get; set; }
    }
    public class BoQOfExecutedWorkContentDetail
    {
        [Key]
        public int BoQOfExecutedWorkContentDetailId { get; set; }
        public int BoQOfExecutedWorkContentId { get; set; }

        [Display(Name = "Item No.")]
        public string Itemcode { get; set; }
        public int Index { get; set; }
        //
        public int WorkItemPerProjectId { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Unit")]
        public int UnitOfMeasurementId { get; set; }

        [Display(Name = "Quantity")]
        public decimal ContractQuantity { get; set; }
        public decimal QuantityTillPrevPayment { get; set; }
        public decimal QuantityThisPayment { get; set; }
        public decimal QuantityToDate { get; set; }

        [Display(Name = "Unit Rate")]
        public decimal UnitRate { get; set; }

        public decimal AmountTillPrevPayment { get; set; }
        public decimal AmountThisPayment { get; set; }
        public decimal AmountToDatePayment { get; set; }

    }
    public class RepresentativeElementCostInBill : TrackUserOperation
    {
        [Key]
        public int RepresentativeElementCostInBillId { get; set; }
        public int ProjectRepresentativeElementId { get; set; }
        public int BoQOfExecutedWorkId { get; set; }
        // for materials
        public string ItemCode { get; set; }
        // for equipment        
        public Nullable<Guid> FleetsID { get; set; }
        // for labor
        public Nullable<int> ProjectEstimationIndexLabourId { get; set; }
        [Display(Name ="Item")]
        public string ItemName { get; set; }
        public string Unit { get; set; }
        [Display(Name = "Basic Price")]
        public decimal BasicPrice { get; set; }
        [Display(Name = "Current Price")]
        public decimal CurrentPrice { get; set; }
        // for road projects
        [Display(Name ="Bidder's Proposed Weighting")]
        public decimal BidderProposedWeight { get; set; }
        public virtual BoQOfExecutedWork BoQOfExecutedWork { get; set; }
        public virtual ProjectRepresentativeElement RepresentativeElement { get; set; }
    }
    public class PriceAdjustmentFormulaViewModel
    {
        public string Type { get; set; }
        public string Formula { get; set; }
    }
    public class WorkInProgress : Operations
    {
        [Key]
        public int WorkInProgressId { get; set; }
        [Display(Name = "WIP No.")]
        public string WorkInProgressNumber { get; set; }
        public int ProjectInformationId { get; set; }
        [Display(Name = "Client")]
        public string ClientName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString =  "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Display(Name = "WIP Status")]
        public WorkInProgressStatus WorkInProgressStatus { get; set; }
        public virtual ProjectInformation ProjectInformation { get; set; }
        public virtual ICollection<WorkInProgressDetail> WorkInProgressItems { get; set; } = new List<WorkInProgressDetail>();
    }
    public class WorkInProgressValidation : VoucherValidation
    {
        [Key]
        public int WorkInProgressValidationId { get; set; }
        public int WorkInProgressId { get; set; }
        public virtual WorkInProgress WorkInProgress { get; set; }
    }
    public class ConstructionCustomerAccount : TrackUserOperation
    {
        [Key]
        public int ConstructionCustomerAccountId { get; set; }
        public int ProjectInformationId { get; set; }

        public string AccountCode { get; set; }
        public string Description { get; set; }
        public ConstructionCustomerAccountTypes AccountType { get; set; }
        public virtual ProjectInformation ProjectInformation { get; set; }

    }
    public class WorkInProgressDetail : TrackUserOperation
    {
        [Key]
        public int WorkInProgressDetailId { get; set; }
        public int WorkInProgressId { get; set; }
        public string Service { get; set; }
        public string Description { get; set; }
        public string AccountCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        [Display(Name = "Tax type")]
        public int GLTaxRateID { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public int WorkItemPerProjectId { get; set; }
        public virtual WorkInProgress WorkInProgress { get; set; }
        public virtual GLTaxRate GLTaxRate { get; set; }
    }
    public class WorkInProgressDistribution : VoucherDistribution
    {
        [Key]
        public int WorkInProgressDistributionId { get; set; }

        public int WorkInProgressId { get; set; }
        public  int? WorkInProgressWorkItemId { get; set; }
        
        public virtual WorkInProgress WorkInProgress { get; set; }
    }
    public class ConsReceivableInvoice : Operations
    {
        public ConsReceivableInvoice()
        {
            InvoiceItems = new List<ConsReceivableInvoiceDetail>();
        }
        public int ConsReceivableInvoiceId { get; set; }
        public int ProjectInformationId { get; set; }
        public ConstructionInvoiceSource Source { get; set; }
        public string InvoiceNumber { get; set; }
        public string Client { get; set; }
        [Display(Name = "Transaction Class")]
        public ConstructionTransactionClass ConstructionTransactionClass { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString =  "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public int PaymentTerm { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString =  "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DueDate { get; set; }
        public string Reference { get; set; }
        [Display(Name = "Invoice Status")]
        public ConstructionInvoiceStatus ConstructionInvoiceStatus { get; set; }
        public OperationStatuses OperationStatus { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal CollectedAmount { get; set; }

        // used when source of Invoice is WIP
        public int WorkInProgressId { get; set; }
        public bool IsInvoiceFromWorkInProgress { get; set; }

        public virtual ProjectInformation ProjectInformation { get; set; }

        public virtual ICollection<ConsReceivableInvoiceDetail> InvoiceItems { get; set; }
    }
    public class ConsReceivableInvoiceValidation : VoucherValidation
    {
        [Key]
        public int ConsReceivableInvoiceValidationId { get; set; }
        public int ConsReceivableInvoiceId { get; set; }
        public virtual ConsReceivableInvoice ConsReceivableInvoice { get; set; }


    }
    public class ConsReceivableInvoiceDetail : TrackUserOperation
    {
        public int ConsReceivableInvoiceDetailId { get; set; }
        public int ConsReceivableInvoiceId { get; set; }
        [Display(Name = "Tax type")]
        public int GLTaxRateID { get; set; }
        public string Service { get; set; }
        public string Description { get; set; }
        public string AccountCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public int WorkItemPerProjectId { get; set; }
        [Display(Name ="Payment Certificate")]
        public int ProjectPaymentCertificateId { get; set; }

        public decimal Penality { get; set; }
        public decimal Retention { get; set; }
        public decimal AdvanceRepayment { get; set; }
        public decimal Correction { get; set; }
        public decimal Others { get; set; }
        public decimal PreviousPayment { get; set; }
        public decimal NetAmount { get; set; }
        public decimal MaterialOnSite { get; set; }
        public virtual ConsReceivableInvoice ConsReceivableInvoice { get; set; }

    }
    public class ConsReceivableInvoiceDistribution : VoucherDistribution
    {
        [Key]
        public int ConsReceivableInvoiceDistributionId { get; set; }
        public int ConsReceivableInvoiceId { get; set; }
        
        public virtual ConsReceivableInvoice ConsReceivableInvoice { get; set; }

    }

    // AR Standard Colection
    public class ConsARStandardCollection : Operations
    {
        [Key]
        public int ConsARStandardCollectionId { get; set; }
        public int ReceiptID { get; set; }
        public int ProjectInformationId { get; set; }
        public StandardCollectionReceiptType ReceiptType { get; set; }
        public string ClientName { get; set; }
        public string ReceiptNumber { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString =  "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReceiptDate { get; set; }
        public int PaymentMethodID { get; set; }
        public string Reference { get; set; }
        public decimal PrepaymentBalance { get; set; }
        public decimal OutstandingBalance { get; set; }
        public virtual ProjectInformation ProjectInformation { get; set; }
        public virtual ICollection<ConsARStandardCollectionDetail> StandardCollectionItems { get; set; } = new List<ConsARStandardCollectionDetail>();

    }
    public class ConsARStandardCollectionValidation : VoucherValidation
    {
        [Key]
        public int ConsARStandardCollectionValidationId { get; set; }
        public int ConsARStandardCollectionId { get; set; }

        public virtual ConsARStandardCollection ConsArStandardCollection { get; set; }
    }
    public class ConsARStandardCollectionDetail : TrackUserOperation
    {
        [Key]
        public int ConsARStandardCollectionDetailId { get; set; }
        public int ConsARStandardCollectionId { get; set; }

        public string InvoiceNumber { get; set; }
        public decimal ApplyAmount { get; set; }
        public decimal BalanceDue { get; set; }
        [Display(Name = "Advance Repayment amnt.")]
        public decimal AdvanceAmount { get; set; }
        public decimal Retention { get; set; }
        // for vat
        [Display(Name = "VAT")]
        public int GLTaxRateID { get; set; }
        public decimal TaxAmount { get; set; }
        [Display(Name = "VAT Withholding")]
        public bool VatWithholding { get; set; }
        // withholding tax rate
        [Display(Name = "Withholding Type")]
        public int WithholdingTaxRateId { get; set; }
        public decimal WithholdingTaxAmount { get; set; }
        public string Remark { get; set; }

        public virtual ConsARStandardCollection StandardCollection { get; set; }
    }

    public class ConsStandardCollectionDistribution : VoucherDistribution
    {
        [Key]
        public int ConsStandardCollectionDistributionId { get; set; }
        public int ConsARStandardCollectionId { get; set; }

        public virtual ConsARStandardCollection ConsArStandardCollection { get; set; }

    }

    public class ConsPaymentAgingPeriod : TrackUserOperation
    {
        [Key]
        public int ConsPaymentAgingPeriodId { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public AgingPeriodTimeUnit TimeUnit { get; set; }
        public string ConcatnatedRateDescription { get; set; }
        [Display(Name ="Provision Rate (%)")]
        public decimal ProvisionRate { get; set; }
    }
}
