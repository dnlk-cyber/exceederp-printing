////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	domain\hrm\allowance.cs
//
// summary:	Implements the allowance class by Kibrom Abera Dec 1,2025
////////////////////////////////////////////////////////////////////////////////////////////////////

using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.Allowance
{
    public class AllowanceGenerationSetting : TrackUserOperation
    {
        [Key]
        public int AllowanceGenerationSettingId { get; set; }
        [Display(Name = "Generate for New Employee")]
        public bool AllowGenerationForNewEmployee { get; set; }
        [Display(Name = "Regenerate for Updated Employee")]
        public bool AllowRegenerationForUpdatedEmployee { get; set; }
        public string Remark { get; set; }

    }         


}
