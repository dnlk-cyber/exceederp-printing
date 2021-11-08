using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
   public class ScreeningSetting
    {
        public int ScreeningSettingId { get; set; }
        [Display(Name ="Vacancy")]
        public int VacancyId { get; set; }       
        public Gender Gender { get; set; }
        [Display(Name = "Organization Type")]
        public OrganizationType OrganizationType { get; set; }
        [Display(Name = "Experience Year")]
        public int ExperienceYear { get; set; }
        public Languages Language { get; set; }
    }
}
