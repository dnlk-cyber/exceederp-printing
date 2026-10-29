using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.SalaryStructure
{
   public class SalaryStructureVersion: TrackUserOperation
    {
        [Key]
        public int SalaryStructureVersionId { get; set; }
        [Required]
        [Display(Name = "HR_SalaryStructure_SalaryStructureVersion_StructureVersionName", ResourceType = typeof(Localization.Resources))]
        public string StructureVersionName { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "Common_PersonEmployee_IsActive", ResourceType = typeof(Localization.Resources))]
        public bool IsActive { get; set; }
    }
}
