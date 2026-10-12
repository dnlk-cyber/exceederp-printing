using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Localization;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.Payroll.Setting
{
    /// <summary>   A DelegationBenefitSetting. </summary>
    ///
    /// <remarks>   Kibrom Abera, 10/31/2024. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

  
    public class DelegationBenefitSetting : TrackUserOperation
    {
        [Key]
        public int DelegationBenefitSettingId { get; set; }
        [Display(Name = "From Days")]
        public int FromDays { get; set; }
        [Display(Name = "To Days")]
        public int ToDays { get; set; }
        [Display(Name = "Payable Salary in %")]
        [Range(0, 100, ErrorMessage = "Payable Salary in % must be between 0 and 100.")]
        public decimal PayableSalaryInPercent { get; set; }
    }
    
    public class DelegationStepIncrementSetting : TrackUserOperation
    {
        [Key]
        public int DelegationStepIncrementSettingId { get; set; }

        /// <summary>
        /// Enables or disables automatic step increment for delegated employees.
        /// </summary>
        [Display(Name = "Enable Auto Step Increment")]
        public bool EnableAutoStepIncrement { get; set; } = true;

        /// <summary>
        /// Number of months after which the first step increment occurs.
        /// </summary>
        [Display(Name = "Initial Increment Period (Months)")]
        public int InitialIncrementPeriodMonths { get; set; } = 3;

        /// <summary>
        /// Number of months after which the next step increment occurs.
        /// </summary>
        [Display(Name = "Subsequent Increment Period (Months)")]
        public int SubsequentIncrementPeriodMonths { get; set; } = 3;

        /// <summary>
        /// Maximum number of automatic step increments allowed.
        /// Example: 2 means Step1→Step2, then Step2→Step3.
        /// </summary>
        [Display(Name = "Maximum Step Increments")]
        public int MaxStepIncrements { get; set; } = 2;

        /// <summary>
        /// If true, applies only when delegated employee’s salary 
        /// is greater than or equal to the delegator’s.
        /// </summary>
        [Display(Name = "Apply Only If Salary ≥ Delegator")]
        public bool ApplyOnlyIfSalaryGreaterOrEqual { get; set; } = true;

        /// <summary>
        /// The date when this configuration becomes active.
        /// </summary>
        [Display(Name = "Effective From Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime EffectiveFromDate { get; set; } = DateTime.Now;
        [Display(Name = "Effective From Date")]
        public string EffectiveFromDateAm { get; set; } 



    }
   


}
