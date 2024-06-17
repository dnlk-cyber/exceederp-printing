using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Planning.Project
{
    public class ProjectExcutiveSetting
    {
        public int ProjectExcutiveSettingId { get; set; }
        [Display(Name = "Indicator")]
        [Required]
        public string Category { get; set; }
        public string Description { get; set; }

    }
    public class ProjectExcutiveSummarySetting
    {
        [Key]
        public int ProjectExcutiveSummarySettingId { get; set; }
        [Display(Name = "KPI Name")]
        [Required]
        public int ProjectKPIId { get; set; }
        [Display(Name = "Sub-KPI Name")]
        [Required]
        public int ProjectSubKPIId { get; set; }
        public int ProjectExcutiveSettingId { get; set; }
        public virtual ProjectExcutiveSetting ProjectExcutive { get; set; }



    }
}
