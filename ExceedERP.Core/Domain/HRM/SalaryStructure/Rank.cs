using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.SalaryStructure
{
    /// <summary>    A rank. </summary>
    ///
    /// <remarks>    Mekanehiwot, 10/31/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class Rank:TrackUserOperation 
    {
        [Key]
        public int RankId { get; set; }
        [Required]
        [Display(Name = "HR_SalaryStructure_Rank_RankName", ResourceType = typeof(Localization.Resources))]
        public string RankName { get; set; }
        [Required]
        [Display(Name = "HR_SalaryStructure_Rank_StartingSalary", ResourceType = typeof(Localization.Resources))]
        public decimal StartingSalary { get; set; }
        
        [Required]
        [Display(Name = "HR_SalaryStructure_Rank_MaximumSalary", ResourceType = typeof(Localization.Resources))]
        public decimal MaximumSalary { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        //public virtual Placement Placement { get; set; }
    }
}