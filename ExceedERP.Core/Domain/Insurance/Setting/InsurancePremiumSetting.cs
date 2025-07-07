using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Insurance.Setting
{

    ///// <summary>
    /// Represents the configuration settings for an employee's insurance premium.
    /// Created by Kibrom Abera, June 11, 2025.
    /// </summary>
    public class InsurancePremiumSetting : TrackUserSettingOperation
    {
        /// <summary>
        /// Gets or sets the unique identifier for the insurance premium setting.
        /// </summary>
        public int InsurancePremiumSettingId { get; set; }

        /// <summary>
        /// Gets or sets the premium percentage.
        /// </summary>
        [Display(Name = "Premium (%)")]
        public decimal Premium { get; set; }

        /// <summary>
        /// Gets or sets the total number of months applicable for the premium calculation.
        /// </summary>
        [Display(Name = "Total Months")]
        public decimal TotalMonths { get; set; }

        /// <summary>
        /// Gets or sets additional remarks or comments related to the insurance premium setting.
        /// </summary>
        public string Remark { get; set; }
    }


}
