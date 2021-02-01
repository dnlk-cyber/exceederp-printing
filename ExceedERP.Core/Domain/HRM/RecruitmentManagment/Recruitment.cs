using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common.HRM;

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
        [Display(Name = "Job Code")]
        public int LookupId { get; set; }
        public Lookup Lookup { get; set; }
        [Required ]
        [Display(Name = "No Of Vacant Position")]
        public int NoOfVaccantPostion { get; set; }
        [Display(Name = "No Of Current Staff")]
        public int NoOfCurrentStaff { get; set; }
        [Display(Name = "No Of Should be Staff")]
        public int NoOfShouldBeStaff { get; set; }

        public string Remark;
    }
}