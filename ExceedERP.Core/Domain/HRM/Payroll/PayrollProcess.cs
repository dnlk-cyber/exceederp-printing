using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.Payroll
{
    public class PayrollProcess
    {
    }


    public class PayrollProcessViewModel
    {
        [Key]
        public int PayrollProcessViewModelId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        public string FullName { get; set; }
        public string LocationCode { get; set; }
        public string PensionNumber { get; set; }
        public string ProvidentNumber { get; set; }
        public int GLPeriodId { get; set; }
        public virtual GLPeriod GLPeriod { get; set; }
        public string PeriodName { get; set; }
        public decimal DaysAbsent { get; set; }
        public decimal AbsentDeduction { get; set; }
        public decimal UnpaidLeaveDays { get; set; }
        public decimal UnpaidLeaveDeduction { get; set; }
        public decimal BasicSalary { get; set; }
        //added
        public decimal TaxableDesertAllowance { get; set; }
        public decimal NonTaxableDesertAllowance { get; set; }
        public decimal TotalDesertAllowance { get; set; }
        //awwce added
        public decimal TaxablePositionAllowance { get; set; }
        public decimal NonTaxablePositionAllowance { get; set; }
        public decimal TotalPositionAllowance { get; set; }

        public decimal TaxableTopUpAllowance { get; set; }
        public decimal NonTaxableTopUpAllowance { get; set; }
        public decimal TotalTopUpAllowance { get; set; }

        public decimal TaxableProfessionalAllowance { get; set; }
        public decimal NonTaxableProfessionalAllowance { get; set; }
        public decimal TotalProfessionalAllowance { get; set; }

        public decimal TaxableTransportAllowance { get; set; }
        public decimal NonTaxableTransportAllowance { get; set; }
        public decimal TotalTransportAllowance { get; set; }

        public decimal TaxableHouseAllowance { get; set; }
        public decimal NonTaxableHouseAllowance { get; set; }
        public decimal TotalHouseAllowance { get; set; }
        public decimal TaxableTelephoneAllowance { get; set; }
        public decimal NonTaxableTelephoneAllowance { get; set; }
        public decimal TotalTelephoneAllowance { get; set; }
        public decimal NonTaxableResponsiblityAllowance { get; set; }
        public decimal TaxableResponsiblityAllowance { get; set; }
        public decimal TotalResponsiblityAllowance { get; set; }
        public decimal NonTaxableProjectAllowance { get; set; }
        public decimal TaxableProjectAllowance { get; set; }
        public decimal TotalProjectAllowance { get; set; }
        //
        public decimal TaxableAllowance { get; set; }
        public decimal NonTaxableAllowance { get; set; }
        public decimal TotalAllowance { get; set; }
        public decimal TotalOvertime { get; set; }
        public decimal TotalAdditions { get; set; }
        public decimal TotalTaxAdditions { get; set; }
        public decimal TotalNonTaxAdditions { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalLongTermDeductions { get; set; }
        public decimal TotalLoan { get; set; }
        public decimal TotalPenality { get; set; }
        //added
        public decimal TotalAdvance { get; set; }
        [NotMapped]
        public decimal RedCross { get; set; }
        public decimal TotalRegularContribution { get; set; }
        public decimal TotalFixedContribution { get; set; }
        public decimal LabourUnionContribution { get; set; }
        public decimal AssocationContribution { get; set; }
        public decimal SportFee { get; set; }
        public decimal GrossEarning { get; set; }
        public decimal GrossTaxableEarning { get; set; }
        public decimal GrossNonTaxableEarning { get; set; }
        public decimal GrossDeduction { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal NetEarning { get; set; }
        public decimal IncomeTax { get; set; }

        public decimal PensionFromCompany { get; set; }
        public decimal PensionFromEmployee { get; set; }
        public decimal TotalPension { get; set; }

        //Provident

        public decimal ProvidentFromCompany { get; set; }
        public decimal ProvidentFromEmployee { get; set; }
        public decimal TotalProvident { get; set; }
        //contract,permanet
        public EmployeeStatus Type { get; set; }
        public string CostCenterAccountCode { get; set; }
        public string CostCenterAccountDesc { get; set; }
        public string OrgBranchAccountCode { get; set; }
        public string OrgBranchName { get; set; }
        public string Status { get; set; }
        public bool isSendForApproval { get; set; }
        public bool isOnlineApproved { get; set; }
        public string OnlineApprovedBy { get; set; }
        public DateTime? OnlineApprovedTime { get; set; }
        public bool isVoid { get; set; }
        public string VoidBy { get; set; }
        public DateTime? VoidTime { get; set; }
        public string BankAccount { get; set; }
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
    }

    public class PayrollDistribution : TrackUserOperation
    {
        [Key]
        public int PayrollDistributionId { get; set; }
        public int GLPeriodId { get; set; }
        public virtual GLPeriod GLPeriod { get; set; }
        public string AccountCode { get; set; }
        [Display(Name = "Account Description")]
        public string AccountDesc { get; set; }
        [Display(Name = "Description")]
        [NotMapped]
        public string Description { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        [Display(Name = "Branch Code")]
        public string OrgBranchName { get; set; }
        public string Status { get; set; }
        [Display(Name = "Cost Center Code")]
        public string CostCenterCode { get; set; }
        public string LocationCode { get; set; }
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        //public bool isSendForApproval { get; set; }
        //public bool isOnlineApproved { get; set; }
        //public string OnlineApprovedBy { get; set; }
        //public DateTime? OnlineApprovedTime { get; set; }
        public bool isMannual { get; set; }
        //public string VoidBy { get; set; }
        //public DateTime? VoidTime { get; set; }
    }
    public class PayrollProcessAllowances
    {
        [Key]
        public int PayrollProcessAllowancesId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public int PayrollProcessViewModelId { get; set; }
        public PayrollProcessViewModel PayrollProcessViewModel { get; set; }

        public string Name { get; set; }
        public AllowanceType AllowanceType { get; set; }
        public decimal Taxable { get; set; }
        public decimal NonTaxable { get; set; }
        public decimal Total { get; set; }
        public string CostCenterAccountCode { get; set; }
        public string CostCenterAccountDesc { get; set; }
        [Display(Name = "Branch Name")]
        public string OrgBranchName { get; set; }
        public string Status { get; set; }
        public bool isSendForApproval { get; set; }
        public bool isOnlineApproved { get; set; }
        public string OnlineApprovedBy { get; set; }
        public DateTime? OnlineApprovedTime { get; set; }
        public bool isVoid { get; set; }
        public string VoidBy { get; set; }
        public DateTime? VoidTime { get; set; }
    }
    public class PayrollProcessAdditions
    {
        [Key]
        public int PayrollProcessAdditionsId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public int PayrollProcessViewModelId { get; set; }
        public PayrollProcessViewModel PayrollProcessViewModel { get; set; }
        public int AdditionId { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string CostCenterAccountCode { get; set; }
        public string CostCenterAccountDesc { get; set; }
        [Display(Name = "Branch Name")]
        public string OrgBranchName { get; set; }
        public string Status { get; set; }
        public bool isSendForApproval { get; set; }
        public bool isOnlineApproved { get; set; }
        public string OnlineApprovedBy { get; set; }
        public DateTime? OnlineApprovedTime { get; set; }
        public bool isVoid { get; set; }
        public string VoidBy { get; set; }
        public DateTime? VoidTime { get; set; }
    }
    public class PayrollProcessDeductions
    {
        [Key]
        public int PayrollProcessDeductionsId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public int PayrollProcessViewModelId { get; set; }
        public PayrollProcessViewModel PayrollProcessViewModel { get; set; }
        public string Name { get; set; }
        public decimal OutstandingAmount { get; set; }
        public decimal Amount { get; set; }
        public string CostCenterAccountCode { get; set; }
        public string CostCenterAccountDesc { get; set; }
        [Display(Name = "Branch Name")]
        public string OrgBranchName { get; set; }
        public string Status { get; set; }
        public bool isSendForApproval { get; set; }
        public bool isOnlineApproved { get; set; }
        public string OnlineApprovedBy { get; set; }
        public DateTime? OnlineApprovedTime { get; set; }
        public bool isVoid { get; set; }
        public string VoidBy { get; set; }
        public DateTime? VoidTime { get; set; }
        public int Index { get; set; }
        public string Remark { get; set; }
        public decimal Months { get; set; }
    }
    public class PayrollProcessLongTermDeductions
    {
        [Key]
        public int PayrollProcessLongTermDeductionsId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public int EmpPayrollLongTermDeductionId { get; set; }
        public int PayrollProcessViewModelId { get; set; }
        public PayrollProcessViewModel PayrollProcessViewModel { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public decimal Outstanding { get; set; }
        public string CostCenterAccountCode { get; set; }
        public string CostCenterAccountDesc { get; set; }
        [Display(Name = "Branch Name")]
        public string OrgBranchName { get; set; }
        public string Status { get; set; }
        public bool isSendForApproval { get; set; }
        public bool isOnlineApproved { get; set; }
        public string OnlineApprovedBy { get; set; }
        public DateTime? OnlineApprovedTime { get; set; }
        public bool isVoid { get; set; }
        public string VoidBy { get; set; }
        public DateTime? VoidTime { get; set; }
    }
    public class PayrollProcessLoans
    {
        [Key]
        public int PayrollProcessLoansId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public int LoanId { get; set; }
        //   public virtual Loan Loan { get; set; }
        public int PayrollProcessViewModelId { get; set; }
        public PayrollProcessViewModel PayrollProcessViewModel { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public decimal Outstanding { get; set; }
        public string CostCenterAccountCode { get; set; }
        public string CostCenterAccountDesc { get; set; }
        [Display(Name = "Branch Name")]
        public string OrgBranchName { get; set; }
        public string Status { get; set; }
        public bool isSendForApproval { get; set; }
        public bool isOnlineApproved { get; set; }
        public string OnlineApprovedBy { get; set; }
        public DateTime? OnlineApprovedTime { get; set; }
        public bool isVoid { get; set; }
        public string VoidBy { get; set; }
        public DateTime? VoidTime { get; set; }
    }
    public class PayrollProcessAdvances
    {
        [Key]
        public int PayrollProcessAdvancesId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public int AdvanceId { get; set; }
        //     public virtual Advance Advance { get; set; }
        public int PayrollProcessViewModelId { get; set; }
        public PayrollProcessViewModel PayrollProcessViewModel { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public decimal Outstanding { get; set; }
        public string CostCenterAccountCode { get; set; }
        public string CostCenterAccountDesc { get; set; }
        [Display(Name = "Branch Name")]
        public string OrgBranchName { get; set; }
        public string Status { get; set; }
        public bool isSendForApproval { get; set; }
        public bool isOnlineApproved { get; set; }
        public string OnlineApprovedBy { get; set; }
        public DateTime? OnlineApprovedTime { get; set; }
        public bool isVoid { get; set; }
        public string VoidBy { get; set; }
        public DateTime? VoidTime { get; set; }
    }
    public class PayrollProcessFixedContribution
    {
        [Key]
        public int PayrollProcessFixedContributionID { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public int FixedContributionId { get; set; }
        // public virtual FixedContribution FixedContribution { get; set; }
        public int PayrollProcessViewModelId { get; set; }
        public PayrollProcessViewModel PayrollProcessViewModel { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public decimal Outstanding { get; set; }
        public string CostCenterAccountCode { get; set; }
        public string CostCenterAccountDesc { get; set; }
        [Display(Name = "Branch Name")]
        public string OrgBranchName { get; set; }
        public string Status { get; set; }
        public bool isSendForApproval { get; set; }
        public bool isOnlineApproved { get; set; }
        public string OnlineApprovedBy { get; set; }
        public DateTime? OnlineApprovedTime { get; set; }
        public bool isVoid { get; set; }
        public string VoidBy { get; set; }
        public DateTime? VoidTime { get; set; }
    }
    public class PayrollProcessRegularContribution
    {
        [Key]
        public int PayrollProcessRegularContributionID { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public int RegContributionId { get; set; }
        //public virtual RegularContribution RegularContribution { get; set; }
        public int PayrollProcessViewModelId { get; set; }
        public PayrollProcessViewModel PayrollProcessViewModel { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string CostCenterAccountCode { get; set; }
        public string CostCenterAccountDesc { get; set; }
        [Display(Name = "Branch Name")]
        public string OrgBranchName { get; set; }
        public string Status { get; set; }
        public bool isSendForApproval { get; set; }
        public bool isOnlineApproved { get; set; }
        public string OnlineApprovedBy { get; set; }
        public DateTime? OnlineApprovedTime { get; set; }
        public bool isVoid { get; set; }
        public string VoidBy { get; set; }
        public DateTime? VoidTime { get; set; }
    }

    public class PeriodStatusViewModel
    {
        public int PeriodStatusViewModelId { get; set; }
        public int GLPeriodId { get; set; }
        public virtual GLPeriod GLPeriod { get; set; }
        [Display(Name = "Common_Period_PeriodName", ResourceType = typeof(Localization.Resources))]
        public string PeriodName { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Common_Period_StartDate", ResourceType = typeof(Localization.Resources))]
        public DateTime StartDate { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Common_Period_EndDate", ResourceType = typeof(Localization.Resources))]
        public DateTime EndDate { get; set; }
        [Display(Name = "Common_Period_Status", ResourceType = typeof(Localization.Resources))]
        public PeriodStatusList Status { get; set; }
        [Display(Name = "Common_Period_BranchId", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }



    }
}
