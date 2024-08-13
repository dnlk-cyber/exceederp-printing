using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.SalaryStructure;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Insurance.Setting
{

    /// <summary>
    /// Represents the settings for insurance payment based on employee status and responsibility level.
    /// By Kibrom Abera,  Augest 9, 2024
    /// </summary>
    public class InsurancePaymentSetting: TrackUserSettingOperation
    {
        /// <summary>
        /// Gets or sets the unique identifier for the insurance payment setting.
        /// </summary>
        public int InsurancePaymentSettingId { get; set; }

        /// <summary>
        /// Gets or sets the responsibility level of the employee.
        /// The <see cref="ResponsibilityLevel"/> enum defines different levels, such as Top Management, Medium Management, etc.
        /// </summary>
        [Display(Name = "Responsibility Level")]
        public ResponsibilityLevel Level { get; set; }

        /// <summary>
        /// Gets or sets the employment status of the employee.
        /// The <see cref="EmployeeStatus"/> enum includes types like Permanent, Contract, Freelancer, etc.
        /// </summary>
        [Display(Name = "Employee Type")]
        public EmployeeStatus EmployeeType { get; set; }
        [Display(Name = "Benefit Type")]
        public InsuranceRefuendType BenefitType { get; set; }
        /// <summary>
        /// Gets or sets the type of payment amount.
        /// The <see cref="PaymentAmount"/> enum categorizes the payment as NA (Not Applicable), Limited, or Unlimited.
        /// </summary>
        public PaymentAmount PaymentAmount { get; set; }
        /// <summary>
        /// Gets or sets the current monetary value associated with the payment setting.
        /// This value is nullable, and may be null if <see cref="PaymentAmount"/> is set to Unlimited.
        /// </summary>
        public decimal? CurrentValue { get; set; }
        public string  Remark { get; set; }


    }

    public enum PaymentAmount
    {
        NA,        // Not Applicable
        Limited,   // A specific limited amount
        Unlimited  // Represents an "infinite" or unlimited amount
    }
    public enum InsuranceRefuendType
    {
        NA,
        Accident,
        Death,
        Medical,
        Family

    }
}
