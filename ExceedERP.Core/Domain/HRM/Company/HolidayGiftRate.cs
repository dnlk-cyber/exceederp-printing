using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.Company
{
    public class HolidayGiftRateSetting: TrackUserOperation
    {
        [Key]
        public int HolidayGiftRateSettingId { get; set; }

        /// <summary>
        /// Minimum gross salary (inclusive)
        /// </summary>
        [Display(Name = "Min Gross Salary")]
        public decimal MinGrossSalary { get; set; }

        /// <summary>
        /// Maximum gross salary (inclusive).
        /// Use null for "and above"
        /// </summary>
        [Display(Name = "Max Gross Salary")]
        public decimal? MaxGrossSalary { get; set; }

        /// <summary>
        /// Holiday gift amount
        /// </summary>
        [Display(Name = "Gift Amount")]
        public decimal GiftAmount { get; set; }

        public bool IsActive { get; set; }
    }

}
