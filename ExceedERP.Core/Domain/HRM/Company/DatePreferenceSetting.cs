using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.Company
{
   public class DatePreferenceSetting: TrackUserOperation
    {
        public int DatePreferenceSettingId { get; set; }
        [Required]
        [Display(Name = "Use Ethiopian Date")]
        public bool UseEthiopianDate { get; set; } = true; // Default: Ethiopian
                                                          
        // Auto-generated descriptive remark
        public string Remark
        {
            get => UseEthiopianDate
                ? "System uses Ethiopian dates  by default"
                : "User switched to Gregorian dates";

            private set { } // EF needs a setter, but we control logic via getter
        }
    }
}
