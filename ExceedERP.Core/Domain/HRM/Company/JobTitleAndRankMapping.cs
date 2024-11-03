using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.SalaryStructure;

namespace ExceedERP.Core.Domain.HRM.Company
{
    public class JobTitleAndRankMapping : TrackUserOperation 
    {
        [Key]
        public int JobTitleAndRankMappingId { get; set; }
        [Display(Name = "HR_Company_JobTitleMapping_JobTitle", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }
        [Display(Name = "HR_SalaryStructure_Salary_RankId", ResourceType = typeof(Localization.Resources))]
        public int RankId { get; set; }
        public virtual Rank Rank { get; set;}
        
    }
}