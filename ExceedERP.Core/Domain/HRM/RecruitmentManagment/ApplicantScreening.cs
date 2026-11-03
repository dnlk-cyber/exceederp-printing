using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
   public class ApplicantScreening
    {
        public int ApplicantScreeningId { get; set; }
        public int ApplicantDetailId { get; set; }
        public int ApplicantScreenResultId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Gender))]
        public Gender Sex { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Age))]
        public int Age { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.ApplicantName))]
        public string ApplicantName { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Address))]
        public string Address { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Rpt_TotalResult))]
        public decimal TotalResult { get; set; }
        public decimal Experience { get; set; }
        [NotMapped]
        public string PassOrFail { get; set; } 
        public string Remark { get; set; } 
        public string Status { get; set; } 
     
        public bool IsProfileGenerated { get; set; }
        public bool IsPassed { get; set; }

    }
}
