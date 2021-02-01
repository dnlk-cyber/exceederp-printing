using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ExceedERP.Core.Domain.Construction
{
    public class CashFlowYear : TrackUserOperation
    {
        [Key]
        public int CashFlowYearId { get; set; }
        public int ProjectInformationId { get; set; }
        public virtual ProjectInformation ProjectInformation { get; set; }

        [Display(Name = "Budget Year")]
        public int GlFiscalYearId { get; set; }
        public string Remark { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
    }
    public class CashFlowContractDataBase : TrackUserOperation
    {
        public decimal ContractorAmountWithVat { get; set; }
        public decimal ProvisionalSum { get; set; }
        public decimal SubContractWorks { get; set; }
        public decimal DayWorks { get; set; }

        //
        [Display(Name = "Total Original Contract (with VAT)")]
        public decimal TotalOriginalContractAmount { get; set; }
        public decimal SupplementaryAgreement { get; set; }
        public decimal VariationWorks { get; set; }
        [Display(Name = "Total Contract Amount (with VAT)")]
        public decimal TotalContractAmount { get; set; }
    }
    public class CashFlowContractData : CashFlowContractDataBase
    {
        [Key]
        public int CashFlowContractDataId { get; set; }
        public int CashFlowYearId { get; set; }
        public virtual CashFlowYear CashFlowYear { get; set; }

        //public decimal ContractorAmountWithVat { get; set; }
        //public decimal ProvisionalSum { get; set; }
        //public decimal SubContractWorks { get; set; }
        //public decimal DayWorks { get; set; }
        public decimal TCPPercentage { get; set; }

        //
        //[Display(Name ="Total Original Contract (with VAT)")]
        //public decimal TotalOriginalContractAmount { get; set; }
        //public decimal SupplementaryAgreement { get; set; }
        //public decimal VariationWorks { get; set; }
        //[Display(Name ="Total Contract Amount (with VAT)")]
        //public decimal TotalContractAmount { get; set; }

    }
    public class CashFlowContractDataDetail : CashFlowContractDataBase
    {
        [Key]
        public int CashFlowContractDataDetailId { get; set; }
        public int CashFlowContractDataId { get; set; }
        [Display(Name = "Period")]
        public int ConstructionPeriodId { get; set; }
        public decimal MonthlyProgress { get; set; }
        public decimal CummulativeProgress { get; set; }
        public virtual CashFlowContractData CashFlowContractData { get; set; }
        public virtual ConstructionPeriod ConstructionPeriod { get; set; }
    }
    public class CashFlowFinanceSourceBase : TrackUserOperation
    {
        public decimal MainContractAdvancePayment { get; set; }
        public decimal Revenue { get; set; }
        [Display(Name = "Supplier's Credit")]
        public decimal SuppliersCredit { get; set; }
        public decimal SubContractorFinancing { get; set; }
        public decimal SubContractorRetentionWithholding { get; set; }
        public decimal SubContractorAdvanceRepayment { get; set; }
        public decimal HeadOfficeFinancing { get; set; }
        public decimal MainContractRetentionReleased { get; set; }
        public decimal MaterialOnSite { get; set; }
        public decimal Loan { get; set; }
        public decimal TotalFinanceSource { get; set; }
    }
    public class CashFlowFinanceSource : CashFlowFinanceSourceBase
    {
        [Key]
        public int CashFlowFinanceSourceId { get; set; }
        public int CashFlowYearId { get; set; }
        public virtual CashFlowYear CashFlowYear { get; set; }

    }
    public class CashFlowFinanceSourceDetail : CashFlowFinanceSourceBase
    {
        public int CashFlowFinanceSourceDetailId { get; set; }
        public int CashFlowFinanceSourceId { get; set; }
        [Display(Name = "Period")]
        public int ConstructionPeriodId { get; set; }
        public virtual ConstructionPeriod ConstructionPeriod { get; set; }
        public virtual CashFlowFinanceSource CashFlowFinaceSource { get; set; }
    }
    public class CashFlowDeductionBase : TrackUserOperation
    {
        public decimal MainContractAdvanceRepayment { get; set; }
        public decimal IncomeTaxes { get; set; }
        public decimal MainContractRetentionWithheld { get; set; }
        public decimal Rebate { get; set; }
        public decimal OmmissionWorks { get; set; }
        public decimal Others { get; set; }
        public decimal TotalDeduction { get; set; }
    }
    public class CashFlowDeduction : CashFlowDeductionBase
    {
        [Key]
        public int CashFlowDeductionId { get; set; }
        public int CashFlowYearId { get; set; }
        public virtual CashFlowYear CashFlowYear { get; set; }


    }
    public class CashFlowDeductionDetail : CashFlowDeductionBase
    {
        [Key]
        public int CashFlowDeductionDetailId { get; set; }
        public int CashFlowDeductionId { get; set; }

        [Display(Name = "Period")]
        public int ConstructionPeriodId { get; set; }
        public virtual ConstructionPeriod ConstructionPeriod { get; set; }
        public virtual CashFlowDeduction CashFlowDeduction { get; set; }
    }
    public class NetOperationalCahFlowViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Cum. Net Operational Cash Flow")]
        public decimal CummulativeNetOperationalCashFlow { get; set; }
    }
    public class NetOperationalCahFlowDetailViewModel
    {
        public string Month { get; set; }
        public decimal CummulativeNetOperationalCashFlow { get; set; }
    }
    // this models are Costs / expenses, not net operation
    public class NetOperationCashFlowBase : TrackUserOperation
    {
        public decimal CummulativeNetOperationalCashFlow { get; set; }
        public decimal DirectMaterialExpense { get; set; }
        public decimal DirectAndIndirectlaborExpense { get; set; }
        public decimal DirectEquipmentCost { get; set; }
        public decimal ProjectOverheadExpense { get; set; }
        public decimal HeadOfficeOverheadExpense { get; set; }
        public decimal SubContractWorksAdvance { get; set; }
        public decimal SubContractPiecerateWorksSettlement { get; set; }
        public decimal HeadOfficeFinancingSettlement { get; set; }
        public decimal SuppliersCreditSettlement { get; set; }
        public decimal DayWorksSettlement { get; set; }
        public decimal TaxSettlement { get; set; }
        public decimal LoanSettlement { get; set; }
        public decimal TotalExpense { get; set; }
    }
    public class NetOperationCashFlow : NetOperationCashFlowBase
    {
        public int NetOperationCashFlowId { get; set; }
        public int CashFlowYearId { get; set; }
        public virtual CashFlowYear CashFlowYear { get; set; }

    }
    public class NetOperationCashFlowDetail : NetOperationCashFlowBase
    {
        [Key]
        public int NetOperationCashFlowDetailId { get; set; }
        public int NetOperationCashFlowId { get; set; }

        [Display(Name = "Period")]
        public int ConstructionPeriodId { get; set; }
        public virtual ConstructionPeriod ConstructionPeriod { get; set; }
        public virtual NetOperationCashFlow NetOperationCashFlow { get; set; }
    }

    public class CashFlowBalanceVieModel {
        public int Id { get; set; }
        [Display(Name = "Cash Flow Balance")]
        public decimal NetBalance { get; set; }
    }
    public class CashFlowBalanceDetailVieModel
    {
        public string Month { get; set; }
        [Display(Name ="Cash Flow Balance")]
        public decimal NetBalance { get; set; }
    }
}
