using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Construction
{
    //public class ContractManagement
    //{
    //}
    //public class ProjectContractBase : TrackUserOperation
    //{

    //}
    public class ProjectContract : TrackUserOperation
    {
        [Key]
        public int ProjectContractId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Contract Signing Date")]
        public DateTime ContractSigningDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Commencement Date")]
        public DateTime CommencementDate { get; set; }

        [Display(Name = "Contract Amount")]
        public decimal ContractAmount { get; set; }

        [Display(Name = "Revised Contract Amnt")]
        public decimal? RevisedContractAmount { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Original Completion Date")]
        public DateTime ContractTime { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Site Handover")]
        public DateTime SiteHandoverTime { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Rvised Completion Date")]
        public DateTime? RevisedCompletionDate { get; set; }

        [DisplayName("Provisional Sum")]
        public decimal ProvisionalSum { get; set; }

        [Display(Name = "Contract Period")]
        public decimal? ContractPeriod { get; set; }

        [Display(Name = "Approved Time Ext.")]
        public decimal? ApprovedTimeExtension { get; set; }

        [Display(Name = "Total Proj. Period")]
        public decimal? TotalProjectPeriod { get; set; }
        [Display(Name = "Time Elapsed to Date")]
        public decimal? TimeElapsedToDate { get; set; }
        public decimal? Rebate { get; set; }
        public decimal? Variation { get; set; }
        public decimal? Supplemetary { get; set; }
        [Display(Name = "Total Project Amount")]
        public decimal? TotalProjectAmount { get; set; }
        [Display(Name = "Net Grand")]
        public decimal? NetGrand { get; set; }
        [Display(Name = "Executed to date")]
        public decimal? ExecutedToDate { get; set; }

        public int ProjectInformationId { get; set; }
        public virtual ProjectInformation Project { get; set; }

        public virtual ICollection<ContractVariationOrder> VariationOrders { get; set; }
        public virtual ICollection<ContractSuspensionOrder> SuspensionOrders { get; set; }
        public virtual ICollection<SupplementalAgreement> SupplementalAgreements { get; set; }
        public virtual ICollection<ContractClaim> Claims { get; set; }
        public virtual ICollection<ExtensionOfTime> EoTClaims { get; set; }
        public virtual ICollection<ExecutedWork> ExecutedWorks { get; set; }
        public virtual ICollection<ContractRevision> Revisions { get; set; }
        public virtual ICollection<CertifiedPayment> CertifiedPayments { get; set; }
    }
    public class ProjectSubContract : ProjectContract
    {
        //[Key]
        //public int ProjectSubContractId { get; set; }

        [DisplayName("Sub Contract Work Type")]
        [StringLength(100)]
        public string WorkType { get; set; }

        [DisplayName("Sub Contractor")]
        [StringLength(200)]
        public string SubContractor { get; set; }

        //[DisplayName("Contract Signing Date")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString =  "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[Required]
        //public DateTime ContractSigningDate { get; set; }

        //[DisplayName("Commencement Date")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString =  "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[Required]
        //public DateTime CommencementDate { get; set; }

        //[DisplayName("Contract Time")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString =  "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[Required]
        //public DateTime ContractTime { get; set; }

        //[DisplayName("Contract Amount")]
        //public decimal ContractAmount { get; set; }
        ////[DisplayName("Supplementary Agreement")]
        ////public string SupplimentaryAggrement { get; set; }
        //[DisplayName("Provisional Sum")]
        //public decimal ProvisionalSum { get; set; }


        //[DisplayName("Suspension Order")]
        //[StringLength(200)]
        //public string SuspensionOrder { get; set; }
        //[DisplayName("Claim Request")]
        //public string ClaimRequest { get; set; }
        //[DisplayName("Revised Contract Time")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        //[Required]
        //public DateTime RevisedContractTime { get; set; }
        //[DisplayName("Revised Contract Amount")]
        //public decimal RevisedContractAmount { get; set; }
        //[DisplayName("Certified Payment")]
        //public decimal CertifiedPayment { get; set; }
        //public int ProjectInformationId { get; set; }
        //public virtual ProjectInformation Project { get; set; }
    }
    public class ProjectPaymentTerm : TrackUserOperation
    {
        [Key]
        public int ProjectPaymentTermId { get; set; }
        public int ProjectContractId { get; set; }
        [Display(Name = "Attribute")]
        public ProjectPaymentAttributeName AttributeName { get; set; }
        public decimal Value { get; set; }

        public virtual ProjectContract Contract { get; set; }
    }
    public class ContractVariationOrder : TrackUserOperation
    {
        [Key]
        public int ContractVariationOrderId { get; set; }

        public int VariationOrderTypeId { get; set; }
        public int ProjectContractId { get; set; }

        [Display(Name = "Variation Order No.")]
        public string VariationOrderNumber { get; set; }
        //public string OrderStatus { get; set; }
        //public string Priority { get; set; }


        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString =  "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //public DateTime DateIssued { get; set; }

        [Display(Name = "Days (added)")]
        public int DaysAdded { get; set; }
        //public string IssuedBy { get; set; }
        [Display(Name = "Variation amnt. ")]
        public decimal ChangeCost { get; set; }
        public virtual ProjectContract Contract { get; set; }
        public virtual VariationOrderType Type { get; set; }


    }
    public class ContractSuspensionOrder
    {
        [Key]
        public int ContractSuspensionOrderId { get; set; }

        public int ProjectContractId { get; set; }
        public string SuspensionOrderNumber { get; set; }
        public string ReasonOfSuspension { get; set; }
        public string AffectedWork { get; set; }
        public int SuspensionAmount { get; set; }
        public virtual ProjectContract Contract { get; set; }
    }
    public class SupplementalAgreement
    {
        [Key]
        public int SupplementalAgreementId { get; set; }

        public int ProjectContractId { get; set; }
        public string SupplementAgreementNumber { get; set; }

        public virtual ProjectContract Contract { get; set; }

    }
    public class ContractClaim : Operations
    {
        [Key]
        public int ContractClaimId { get; set; }

        public int ProjectContractId { get; set; }
        //public int ContractClaimTypeId { get; set; }
        [Display(Name = "Description of Details")]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RequestedDate { get; set; }
        [Display(Name = "Ref. No.")]
        public string RefNumber { get; set; }
        [Display(Name = "Ref. Letter")]
        public string ReferanceLetter { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? RespondedDate { get; set; }
        [Display(Name = "Ref. No.")]
        public string ResponseRefNumber { get; set; }
        [Display(Name = "Ref. Letter")]
        public string ResponseReferanceLatter { get; set; }
        [Display(Name = "Time Consumed(day)")]
        public decimal TimeConsumed { get; set; }
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString =  "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //public DateTime IssuedDate { get; set; }

        //public string IssuedBy { get; set; }
        ////public string ClaimType { get; set; }

        //[Display(Name = "Status")]
        //public string ClaimStatus { get; set; }

        public string Remark { get; set; }

        public virtual ProjectContract Contract { get; set; }
        //public virtual ContractClaimType Type { get; set; }
    }
    public class ContractClaimValidation : VoucherValidation
    {
        public int ContractClaimValidationId { get; set; }
        public int ContractClaimId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? RespondedDate { get; set; }
        [Display(Name = "Ref. No.")]
        public string ResponseRefNumber { get; set; }
        [Display(Name = "Ref. Letter")]
        public string ResponseReferanceLatter { get; set; }
        public virtual ContractClaim ContractClaim { get; set; }

    }
    public class WorkItemContractCostNegotiationViewModel
    {
        public int Id { get; set; }
        public string SubProjectTypeName { get; set; }
        public string StructureyType { get; set; }
        public string WorkCategory { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public decimal TotalQuantityOfWork { get; set; }

        public decimal TotalUnitCost { get; set; }
        public string Remark { get; set; }
        public decimal LabourHourlyOutPut { get; set; }
        public string UnitOfMeasurement { get; set; }
        public bool IsSubCategory { get; set; }
        public decimal Amount { get; set; }
    }
    public class ExtensionOfTime : TrackUserOperation
    {
        [Key]
        public int ExtensionOfTimeId { get; set; }
        public int ProjectContractId { get; set; }

        [Display(Name = "Reason for EoT Request")]
        public string Reason { get; set; }
        public string Justification { get; set; }

        [Display(Name = "Requested Days")]
        public int RequestedDays { get; set; }

        [Display(Name = "Approved Days")]
        public int ApprovedDays { get; set; }
        public string Comment { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "EoT Request Date")]
        public DateTime EoTRequesteDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "EoT Approve Date")]
        public DateTime EoTApproveDate { get; set; }

        public virtual ProjectContract Contract { get; set; }
    }
    public class ExecutedWork
    {
        [Key]
        public int ExecutedWorkId { get; set; }

        public int ProjectContractId { get; set; }
        public string Title { get; set; }
        public DateTime PlanComplitionDate { get; set; }
        public DateTime ForecastCompletionDateLastMonth { get; set; }
        public DateTime CurrentForecastCompletionDate { get; set; }
        public DateTime ActualCompletionDate { get; set; }

        public virtual ProjectContract Contract { get; set; }
    }
    public class CertifiedPayment
    {
        [Key]
        public int CertifiedPaymentId { get; set; }
        public int ProjectContractId { get; set; }
        public virtual ProjectContract Contract { get; set; }

    }
    public class VariationOrderType : TrackUserOperation
    {
        [Key]
        public int VariationOrderTypeId { get; set; }
        public string Name { get; set; }

        [Display(Name = "Type/Category")]
        public VariationOrderTypeCategory Type { get; set; }

        public string Description { get; set; }

        //public virtual ICollection<ContractVariationOrder> VariationOrders { get; set; }
    }
    public class ContractRevision
    {
        [Key]
        public int ContractRevisionId { get; set; }

        public int ProjectContractId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RevisionDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RevisedContractTime { get; set; }

        public decimal RevisedContractAmount { get; set; }
        public string Remark { get; set; }

        public virtual ProjectContract Contract { get; set; }
    }

    public class ContractClaimType
    {

        [Key]
        public int ContractClaimTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    #region Price Adjustment related classes
    public class ProjectPriceAdjustmentFormulaType : TrackUserOperation
    {
        [Key]
        public int ProjectPriceAdjustmentFormulaTypeId { get; set; }

        [Display(Name ="Project")]
        public int ProjectInformationId { get; set; }
        public PriceAdjustmentFormulaType FormulaType { get; set; }
        public string Remark { get; set; }

        public virtual ProjectInformation ProjectInformation { get; set; }
    }
    public class ProjectRepresentativeElement : TrackUserOperation
    {
        [Key]
        public int ProjectRepresentativeElementId { get; set; }
        public int ProjectInformationId { get; set; }
        // for materials
        public string ItemCode { get; set; }
        // for equipment
        [Display(Name = "Equipment Type")]
        public Nullable<Guid> FleetsID { get; set; }
        // for labor
        [Display(Name = "Title")]
        public Nullable<int> ProjectEstimationIndexLabourId { get; set; }
        public string Unit { get; set; }
        [Display(Name = "Basic Price")]
        public decimal BasicPrice { get; set; }
        public string RepresentedItems { get; set; }
        public RepresentativeElementIndexType IndexType { get; set; }
        public string Source { get; set; }

        [Display(Name = "Index Code")]
        public string IndexCode { get; set; }
        public string Description { get; set; }

    }
    public class PriceAdjustmentWeight : TrackUserOperation
    {
        [Key]
        public int PriceAdjustmentWeightId { get; set; }
        public int ProjectInformationId { get; set; }
        public int WorkCategoryId { get; set; }
        public virtual WorkCategory WorkCategory { get; set; }
        public decimal ContractAmount { get; set; }
        [Display(Name = "CA WITHOUT PP,PS,LS & CC")]
        public decimal ActualAmount { get; set; }

        public ICollection<PriceAdjustmentWeightItem> WeightItems { get; set; }

    }
    public class RepresentativeElementWeightViewModel
    {
        public int Id { get; set; }
        public string Item { get; set; }
        public decimal Weight { get; set; }
    }
    public class PriceAdjustmentWeightItem : TrackUserOperation
    {
        [Key]
        public int PriceAdjustmentWeightItemId { get; set; }
        public int ProjectRepresentativeElementId { get; set; }
        public int PriceAdjustmentWeightId { get; set; }
        public string ItemName { get; set; }

        // for materials
        public string ItemCode { get; set; }
        // for equipment
        public Nullable<Guid> FleetsID { get; set; }
        // for labor
        public Nullable<int> ProjectEstimationIndexLabourId { get; set; }
        public decimal Amount { get; set; }
        [Display(Name = "Amnt. (revised & approved)")]
        public decimal ApprovedAmount { get; set; }
        public virtual PriceAdjustmentWeight PriceAdjustmentWeight { get; set; }

        public virtual ProjectRepresentativeElement RepresentativeElement { get; set; }
    }
    // weight ranges for road projects
    public class RepresentativeElementWeightRange : TrackUserOperation
    {
        public int RepresentativeElementWeightRangeId { get; set; }
        public int ProjectRepresentativeElementId { get; set; }
        [Display(Name = "Min")]
        public decimal PercentageMin { get; set; }
        [Display(Name = "Max")]
        public decimal PercentageMax { get; set; }
        [Display(Name = "Work Category(Series)")]
        public int ConfigurationId { get; set; }
        public virtual Configuration Configuration { get; set; }
        public virtual ProjectRepresentativeElement ProjectRepresentativeElement { get; set; }
    } 
    #endregion

}
