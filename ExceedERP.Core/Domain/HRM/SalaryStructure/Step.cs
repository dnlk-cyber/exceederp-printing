using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.SalaryStructure
{
    /// <summary>    A step. </summary>
    ///
    /// <remarks>    Mekanehiwot, 10/31/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class Step:TrackUserOperation 
    {
        [Key]
        public int StepId { get; set; }
        [Required]
        [Display(Name = "HR_SalaryStructure_Step_StepName", ResourceType = typeof(Localization.Resources))]
        public string StepName { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "HR_SalaryStructure_Step_StepSequence", ResourceType = typeof(Localization.Resources))]
        public int StepSequence { get; set; }

    }
}