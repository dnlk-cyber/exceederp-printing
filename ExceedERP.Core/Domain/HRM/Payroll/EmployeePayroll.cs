////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Domain\HRM\Payroll\OverTime.cs
//
// summary:	Implements the over time class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.Payroll.Setting;

namespace ExceedERP.Core.Domain.HRM.Payroll
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   An employee payroll. </summary>
    ///
    /// <remarks>   Mekanehiwot, 10/31/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class EmployeePayroll
    {
    }
#region OverTime
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   An over time. </summary>
    ///
    /// <remarks>   Mekanehiwot, 10/31/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class OverTime:TrackUserOperation 
    {
        [Key ]
        public int OverTimeId { get; set; }
        [Required]
        [Display(Name = "HR_Payroll_EmployeePayroll_OverTime_EmployeeId", ResourceType = typeof(Localization.Resources))]
         public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
         [Required]
         [Display(Name = "HR_Payroll_EmployeePayroll_OverTime_PeriodId", ResourceType = typeof(Localization.Resources))]
        public int GLPeriodId { get; set; }
         //public virtual PayrollPeriod GLPeriod { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_OverTime_NormalHours", ResourceType = typeof(Localization.Resources))]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public decimal NormalHours { get; set; }
         [Display(Name = "HR_Payroll_EmployeePayroll_OverTime_NightHours", ResourceType = typeof(Localization.Resources))]
         [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public decimal NightHours { get; set; }
         [Display(Name = "HR_Payroll_EmployeePayroll_OverTime_HolidayHours", ResourceType = typeof(Localization.Resources))]
         [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public decimal HolidayHours { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_OverTime_RestdayHours", ResourceType = typeof(Localization.Resources))]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public decimal RestdayHours { get; set; }
         [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
 
    }
#endregion 

    #region OverTime
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   An over time. </summary>
    ///
    /// <remarks>   Mekanehiwot, 10/31/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class OverTimePerDay
    {
     
     [Key]
     [Display(Name = "HR_Payroll_EmployeePayroll_OverTimePerDay_TicketNumber", ResourceType = typeof(Localization.Resources))]
        public string TicketNumber { get; set; }
        [Required]
        [Display(Name = "HR_Payroll_EmployeePayroll_OverTimePerDay_EmployeeId", ResourceType = typeof(Localization.Resources))]
         public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required]
        [Display(Name = "HR_Payroll_EmployeePayroll_OverTimePerDay_PeriodId", ResourceType = typeof(Localization.Resources))]
        public int GLPeriodId { get; set; }
        //public virtual PayrollPeriod GLPeriod { get; set; }
          [Display(Name = "HR_Payroll_EmployeePayroll_OverTime_NormalHours", ResourceType = typeof(Localization.Resources))]
        public bool NormalHours { get; set; }
         [Display(Name = "HR_Payroll_EmployeePayroll_OverTime_NightHours", ResourceType = typeof(Localization.Resources))]
        public bool NightHours { get; set; }
         [Display(Name = "HR_Payroll_EmployeePayroll_OverTime_HolidayHours", ResourceType = typeof(Localization.Resources))]
        public bool HolidayHours { get; set; }
         [Display(Name = "HR_Payroll_EmployeePayroll_OverTime_RestdayHours", ResourceType = typeof(Localization.Resources))]
        public bool RestdayHours { get; set; }
        [Required ]
        [DataType(DataType.Date)]
         [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
         [Display(Name = "HR_Payroll_EmployeePayroll_OverTimePerDay_Date", ResourceType = typeof(Localization.Resources))]
        public DateTime Date { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss}")]
        [Required]
        [Display(Name = "HR_Payroll_EmployeePayroll_OverTimePerDay_From", ResourceType = typeof(Localization.Resources))]
        public DateTime From { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss}")]
        [Required]
        [Display(Name = "HR_Payroll_EmployeePayroll_OverTimePerDay_To", ResourceType = typeof(Localization.Resources))]
            public DateTime To { get; set; }
          [Display(Name = "HR_Payroll_EmployeePayroll_OverTimePerDay_TotalHr", ResourceType = typeof(Localization.Resources))]
        public decimal TotalHr { get; set; }
         [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }

    }
    public class OverTimeTransfer:TrackUserOperation 
    {

        [Key]
        public int OverTimeTransferId { get; set; }
        [Required]
        [Display(Name = "HR_Payroll_EmployeePayroll_OverTimeTransfer_EmployeeId", ResourceType = typeof(Localization.Resources))]
         public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
         [Display(Name = "HR_Payroll_EmployeePayroll_OverTimeTransfer_PeriodId", ResourceType = typeof(Localization.Resources))]
        public int GLPeriodId { get; set; }
         //public virtual PayrollPeriod GLPeriod { get; set; }
        [Required]
        [DataType(DataType.Date)]
         [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
         [Display(Name = "HR_Payroll_EmployeePayroll_OverTimeTransfer_Date", ResourceType = typeof(Localization.Resources))]
        public DateTime Date { get; set; }
         [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
         [Display(Name = "HR_Payroll_EmployeePayroll_OverTime_NormalHours", ResourceType = typeof(Localization.Resources))]
        public decimal NormalHours { get; set; }
         [Display(Name = "HR_Payroll_EmployeePayroll_OverTime_NightHours", ResourceType = typeof(Localization.Resources))]
         public decimal NightHours { get; set; }
         [Display(Name = "HR_Payroll_EmployeePayroll_OverTime_HolidayHours", ResourceType = typeof(Localization.Resources))]
         public decimal HolidayHours { get; set; }
         [Display(Name = "HR_Payroll_EmployeePayroll_OverTime_RestdayHours", ResourceType = typeof(Localization.Resources))]
         public decimal RestdayHours { get; set; }
         public bool IsSendForApproval { get; set; }
         public bool Check { get; set; }
        
    }

    public class OverTimeTrasferDetail
    {
        [Key]
        public int OverTimeTrasferDetailId { get; set; }
        public int OverTimeTransferId { get; set; }
        public virtual OverTimeTransfer OverTimeTransfer { get; set; }
        public bool NormalHours { get; set; }
        public bool NightHours { get; set; }
        public bool HolidayHours { get; set; }
        public bool RestdayHours { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss}")]
        [Required]
        public DateTime From { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss}")]
        [Required]
        public DateTime To { get; set; }
        public string Remark { get; set; }
    }
    #endregion 
#region Loan

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A loan. </summary>
    ///
    /// <remarks>   Mekanehiwot, 10/31/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class Loan:TrackUserOperation 
    {
        public int LoanId { get; set; }
        [Required ]
        [Display(Name = "HR_Payroll_EmployeePayroll_Loan_EmployeeId", ResourceType = typeof(Localization.Resources))]
         public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
               
        [Required]
        [Display(Name = "HR_Payroll_EmployeePayroll_Loan_PeriodId", ResourceType = typeof(Localization.Resources))]
        public int GLPeriodId { get; set; }
        public virtual GLPeriod GLPeriod { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_Loan_LoanAmount", ResourceType = typeof(Localization.Resources))]
        public decimal LoanAmount { get; set; }
        public int Months { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_Loan_MonthlyAmount", ResourceType = typeof(Localization.Resources))]
        public decimal MonthlyAmount { get; set; }
         [Display(Name = "HR_Payroll_EmployeePayroll_Loan_Balance", ResourceType = typeof(Localization.Resources))]
        public decimal Balance { get; set; }
          [Display(Name = "HR_Payroll_EmployeePayroll_Loan_JumpHoliday", ResourceType = typeof(Localization.Resources))]
        public bool JumpHoliday { get; set; }
       [Display(Name = "HR_Payroll_EmployeePayroll_Loan_Description", ResourceType = typeof(Localization.Resources))]
        [Required]
        public string Description { get; set;}
          [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
       public string Remark { get; set; }
         [Display(Name = "HR_Payroll_EmployeePayroll_RegularContribution_IsActive", ResourceType = typeof(Localization.Resources))]
          public bool IsActive { get; set; }

    }

    #endregion 
#region Regular Contribution 

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A regular contribution. </summary>
    ///
    /// <remarks>   Mekanehiwot, 10/31/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class RegularContribution:TrackUserOperation 
    {
        [Key]
        public int RegContributionId { get; set; }
        [Required]
        [Display(Name = "HR_Payroll_EmployeePayroll_RegularContribution_EmployeeId", ResourceType = typeof(Localization.Resources))]
         public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required]
        [Display(Name = "HR_Payroll_EmployeePayroll_RegularContribution_PeriodId", ResourceType = typeof(Localization.Resources))]
        public int GLPeriodId { get; set; }
     //   public virtual PayrollPeriod GLPeriod { get; set; }
        [Required ]
        [Display(Name = "HR_Payroll_EmployeePayroll_RegularContribution_LookupId", ResourceType = typeof(Localization.Resources))]

        public int ContributionTypeId { get; set; }
        public virtual ContributionType ContributionType { get; set; }
         [Display(Name = "HR_Payroll_EmployeePayroll_RegularContribution_Amount", ResourceType = typeof(Localization.Resources))]
         [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public decimal Amount { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_RegularContribution_AdditionalAmount", ResourceType = typeof(Localization.Resources))]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public decimal AdditionalAmount { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_FixedContribution_NetGross", ResourceType = typeof(Localization.Resources))]
        public FixedContAmtType NetGross { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_RegularContribution_IsActive", ResourceType = typeof(Localization.Resources))]
        public bool IsActive { get; set; }
              [Display(Name = "HR_Payroll_EmployeePayroll_FixedContribution_ApplyToAll", ResourceType = typeof(Localization.Resources))]
              public bool ApplyToAll { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }

    }

    #endregion 
#region Fixed Contribution 

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A fixed contribution. </summary>
    ///
    /// <remarks>   Mekanehiwot, 10/31/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class FixedContribution:TrackUserOperation 
    {
        [Key]
        public int FixedContributionId { get; set; }
        [Required]
        [Display(Name = "HR_Payroll_EmployeePayroll_FixedContribution_EmployeeId", ResourceType = typeof(Localization.Resources))]
         public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required]
        [Display(Name = "HR_Payroll_EmployeePayroll_FixedContribution_PeriodId", ResourceType = typeof(Localization.Resources))]
        public int GLPeriodId { get; set; }
      //  public virtual PayrollPeriod GLPeriod { get; set; }
        [Required]
        [Display(Name = "HR_Payroll_EmployeePayroll_FixedContribution_LookupId", ResourceType = typeof(Localization.Resources))]
        public int ContributionTypeId { get; set; }
        public virtual ContributionType ContributionType { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_FixedContribution_Amount", ResourceType = typeof(Localization.Resources))]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public decimal Amount { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_FixedContribution_AmountPercent", ResourceType = typeof(Localization.Resources))]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public decimal AmountPercent { get; set; }
         [Display(Name = "HR_Payroll_EmployeePayroll_FixedContribution_Months", ResourceType = typeof(Localization.Resources))]
        public int Months { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_FixedContribution_MonthlyAmount", ResourceType = typeof(Localization.Resources))]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public decimal MonthlyAmount { get; set; }
       [Display(Name = "HR_Payroll_EmployeePayroll_FixedContribution_Balance", ResourceType = typeof(Localization.Resources))]
       [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public decimal Balance { get; set; }
       [Display(Name = "HR_Payroll_EmployeePayroll_FixedContribution_NetGross", ResourceType = typeof(Localization.Resources))]
       public FixedContAmtType  NetGross { get; set; }

       [Display(Name = "HR_Payroll_EmployeePayroll_FixedContribution_AmtPercnet", ResourceType = typeof(Localization.Resources))]
       public FixedContPecntType AmtPercnet { get; set; }
       [Display(Name = "HR_Payroll_EmployeePayroll_FixedContribution_ApplyToAll", ResourceType = typeof(Localization.Resources))]
       public bool ApplyToAll { get; set; }
         [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
         [Display(Name = "HR_Payroll_EmployeePayroll_RegularContribution_IsActive", ResourceType = typeof(Localization.Resources))]
         public bool IsActive { get; set; }

    }
#endregion 
    #region Association

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   An association. </summary>
    ///
    /// <remarks>   Mekanehiwot, 10/31/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class Association:TrackUserOperation 
    {
        [Key]
        public int AssociationId { get; set; }
        [Required]
        [Display(Name = "HR_Payroll_EmployeePayroll_Association_EmployeeId", ResourceType = typeof(Localization.Resources))]
         public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required]
        [Display(Name = "HR_Payroll_EmployeePayroll_Association_PeriodId", ResourceType = typeof(Localization.Resources))]
        public int GLPeriodId { get; set; }
    //    public virtual PayrollPeriod GLPeriod { get; set; }
         [Display(Name = "HR_Payroll_EmployeePayroll_Association_Type", ResourceType = typeof(Localization.Resources))]
        public AssocationType Type { get; set; }
         [Required]
         [Display(Name = "HR_Payroll_EmployeePayroll_Association_MonthlyAmount", ResourceType = typeof(Localization.Resources))]
         [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public decimal MonthlyAmount { get; set;}
         [Display(Name = "HR_Payroll_EmployeePayroll_FixedContribution_Amount", ResourceType = typeof(Localization.Resources))]
         [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
         public decimal Amount { get; set; }
         [Required]
         [Display(Name = "HR_Payroll_EmployeePayroll_Association_AdditionalAmount", ResourceType = typeof(Localization.Resources))]
         [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
         public decimal AdditionalAmount { get; set; }
         public FixedContAmtType NetGross { get; set; }

         [Display(Name = "HR_Payroll_EmployeePayroll_FixedContribution_AmtPercnet", ResourceType = typeof(Localization.Resources))]
         public FixedContPecntType AmtPercnet { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_Association_IsMemeber", ResourceType = typeof(Localization.Resources))]
         public bool IsMemeber { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_FixedContribution_ApplyToAll", ResourceType = typeof(Localization.Resources))]
        public bool ApplyToAll { get; set; }
          [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
         public string Remark { get; set; }

    }

    public class ProvidentFund : TrackUserOperation
    {
        [Key]
        public int ProvidentFundId { get; set; }
        [Required]
        [Display(Name = "HR_Payroll_EmployeePayroll_Association_EmployeeId", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }

        [Display(Name = "HR_Payroll_EmployeePayroll_Association_IsMemeber", ResourceType = typeof(Localization.Resources))]
        public bool IsMemeber { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }

    }

    #endregion 
#region Advance

    public class Advance : TrackUserOperation
    {
        [Key ]
        public int AdvanceId { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_Advance_EmployeeId", ResourceType = typeof(Localization.Resources))]
         public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required]
        [Display(Name = "HR_Payroll_EmployeePayroll_Advance_PeriodId", ResourceType = typeof(Localization.Resources))]
        public int GLPeriodId { get; set; }
        public virtual GLPeriod Period { get; set; }
         [Display(Name = "HR_Payroll_EmployeePayroll_Advance_AdvanceAmount", ResourceType = typeof(Localization.Resources))]
         [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public decimal AdvanceAmount { get; set; }
         [Display(Name = "HR_Payroll_EmployeePayroll_Advance_Months", ResourceType = typeof(Localization.Resources))]
        public int Months { get; set; }
           [Display(Name = "HR_Payroll_EmployeePayroll_Advance_MonthlyAmount", ResourceType = typeof(Localization.Resources))]
           [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public decimal MonthlyAmount { get; set; }
         [Display(Name = "HR_Payroll_EmployeePayroll_Advance_Balance", ResourceType = typeof(Localization.Resources))]
         [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public decimal Balance { get; set; }

        [Display(Name = "HR_Payroll_EmployeePayroll_Advance_JumpHoliday", ResourceType = typeof(Localization.Resources))]
        public bool JumpHoliday { get; set; }
        [Required ]
        [Display(Name = "HR_Payroll_EmployeePayroll_Advance_Description", ResourceType = typeof(Localization.Resources))]
        public string Description { get; set; }

      [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
      [Display(Name = "HR_Payroll_EmployeePayroll_RegularContribution_IsActive", ResourceType = typeof(Localization.Resources))]
      public bool IsActive { get; set; }
    }
   
    //not used
    public class EmpPayrollLongTermDeduction
    {
        public int EmpPayrollLongTermDeductionId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        [Required]
        [Display(Name = "Employee ID")]
         public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required]
        [Display(Name = "Full Name")]
        public string EmpFullName { get; set; }
        [Display(Name = "Deduction Amount")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public decimal DeductionAmount { get; set; }
        public int Months { get; set; }
        [Display(Name = "Monthly Amount")]
        public decimal MonthlyAmount { get; set; }
        public decimal Balance { get; set; }
        [Required]
        [Display(Name = "Deduction Type")]
        public string DeductionType { get; set; }

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

    public class EmpPayrollServiceChargeViewModel
    {
        public int EmpPayrollServiceChargeViewModelId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        [Display(Name = "Employee ID")]
         public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Display(Name = "Full Name")]
        public string EmpFullName { get; set; }
        [Display(Name = "Payroll Period")]
        public string PeriodName { get; set; }
        public decimal Amount { get; set; }

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
    #endregion 


    public class EmpPayrollAddition
    {
        public int EmpPayrollAdditionId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        [Required]
        [Display(Name = "Employee ID")]
         public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required]
        [Display(Name = "Full Name")]
        public string EmpFullName { get; set; }
        [Required]
        [Display(Name = "Payroll Period")]
        public string PeriodName { get; set; }
        public decimal Amount { get; set; }
        [Required]
        [Display(Name = "Addition Type")]
        public string AdditionType { get; set; }

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
    public class EmpPayrollDeduction
    {
        public int EmpPayrollDeductionId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        [Required]
        [Display(Name = "Employee ID")]
         public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required]
        [Display(Name = "Full Name")]
        public string EmpFullName { get; set; }
        [Required]
        [Display(Name = "Payroll Period")]
        public string PeriodName { get; set; }
        public decimal Amount { get; set; }
        [Required]
        [Display(Name = "Deduction Type")]
        public string DeductionType { get; set; }

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

    public class Bonus:TrackUserOperation
    {
        [Key]
        public int BonusId { get; set; }
          [Display(Name = "HR_Payroll_EmployeePayroll_Bonus_GlFiscalYearId", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
         [Display(Name = "HR_Payroll_EmployeePayroll_Bonus_EmployeeId", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee PersonEmployee { get; set; }
           [Display(Name = "HR_Payroll_EmployeePayroll_Bonus_BounsFactor", ResourceType = typeof(Localization.Resources))]
        public decimal BounsFactor { get; set; }
          [Display(Name = "HR_Payroll_EmployeePayroll_Bonus_PerformanceRate", ResourceType = typeof(Localization.Resources))]
        public string PerformanceRate { get; set; }
          [Display(Name = "HR_Payroll_EmployeePayroll_Bonus_BonusType", ResourceType = typeof(Localization.Resources))]
        public BonusType BonusType { get; set; }

          [Display(Name = "HR_Payroll_EmployeePayroll_Bonus_TotalBonus", ResourceType = typeof(Localization.Resources))]
          public double TotalBonus { get; set; }
          [Display(Name = "HR_Payroll_EmployeePayroll_Bonus_TotalIncomeTax", ResourceType = typeof(Localization.Resources))]
          public double TotalIncomeTax { get; set; }
          [Display(Name = "HR_Payroll_EmployeePayroll_Bonus_isClosed", ResourceType = typeof(Localization.Resources))]
          public bool IsClosed { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
         

    }

    public class BackPayment:TrackUserOperation
    {
        [Key]
        public int BackPaymentId { get; set; }
           [Display(Name = "HR_Payroll_EmployeePayroll_BackPayment_GLPeriodId", ResourceType = typeof(Localization.Resources))]
        public int GLPeriodId { get; set; }
           public virtual GLPeriod GLPeriod { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_Bonus_EmployeeId", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee PersonEmployee { get; set; }
         [Display(Name = "HR_Payroll_EmployeePayroll_BackPayment_BasicSalary", ResourceType = typeof(Localization.Resources))]
        public decimal BasicSalary { get; set; }

         [Display(Name = "HR_Payroll_EmployeePayroll_BackPayment_Allowances", ResourceType = typeof(Localization.Resources))]
         public decimal Allowances { get; set; }
         [Display(Name = "HR_Payroll_EmployeePayroll_BackPayment_IncomeTax", ResourceType = typeof(Localization.Resources))]
         public decimal IncomeTax { get; set; }
         [Display(Name = "HR_Payroll_EmployeePayroll_BackPayment_Pension", ResourceType = typeof(Localization.Resources))]
         public decimal Pension { get; set; }
         [Display(Name = "HR_Payroll_EmployeePayroll_BackPayment_Net", ResourceType = typeof(Localization.Resources))]
         public decimal Net { get; set; }
         [Display(Name = "HR_Payroll_EmployeePayroll_Bonus_isClosed", ResourceType = typeof(Localization.Resources))]
         public bool IsClosed { get; set; }
         [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }

    public class EmployeeBankAccountView
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
         [Display(Name = "Common_PersonEmployee_FirstNameEnglish", ResourceType = typeof(Localization.Resources))]
        public string FirstNameEnglish { get; set; }
         [Display(Name = "Common_PersonEmployee_MiddleNameEnglish", ResourceType = typeof(Localization.Resources))]
        public string MiddleNameEnglish { get; set; }
         [Display(Name = "Common_PersonEmployee_LastNameEnglish", ResourceType = typeof(Localization.Resources))]
        public string LastNameEnglish { get; set; }
        [Display(Name = "Common_PersonEmployee_MothersFullName", ResourceType = typeof(Localization.Resources))]
        public string MothersFullName { get; set; }
         [Display(Name = "Common_PersonEmployee_Gender", ResourceType = typeof(Localization.Resources))]
        public Gender Gender { get; set; }
         [Display(Name = "Common_PersonEmployee_DateOfBirth", ResourceType = typeof(Localization.Resources))]
        public string DateOfBirthAm { get; set; }
           [Display(Name = "Common_PersonEmployee_BirthPlace", ResourceType = typeof(Localization.Resources))]
        public string BirthPlace { get; set; }
          [Display(Name = "Common_PersonEmployee_HiredDate", ResourceType = typeof(Localization.Resources))]
        public string HiredDateAmharic { get; set; }
        public string AccountNumber { get; set; }
         [Display(Name = "HR_EmployeeDetail_Placement_Branch", ResourceType = typeof(Localization.Resources))]
        public string Branch { get; set; }
        public int BranchId { get; set; }
        public int BankReferenceId { get; set; }
       //  [Display(Name = "HR_EmployeeDetail_Placement_Assignment", ResourceType = typeof(Localization.Resources))]
        public string Assignment { get; set; }
    }


}
