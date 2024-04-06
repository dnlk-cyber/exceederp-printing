using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Localization;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    /// <summary>   A recruitment. </summary>
    ///
    /// <remarks>   Mekanehiwot, 10/31/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class Recruitment

    {
        [Key]
        public int RecruitmentId { get; set; }
      
        public bool IsForContrat { get; set; }
        [Required]
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.JobCode))]
        public int LookupId { get; set; }
        public Lookup Lookup { get; set; }
        [Required ]
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.NoOfVacantPosition))]
        public int NoOfVaccantPostion { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.NoOfCurrentStaff))]
        public int NoOfCurrentStaff { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.NoOfShouldBeStaff))]
        public int NoOfShouldBeStaff { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Remark))]
        public string Remark;
    }
}