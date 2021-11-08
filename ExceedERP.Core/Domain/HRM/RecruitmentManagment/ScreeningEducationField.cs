using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
   public class ScreeningEducationField
    {
        public int ScreeningEducationFieldId { get; set; }
        public int ScreeingSettingId { get; set; }
        [Display(Name ="Education Field")]
        public int EducationFieldId { get; set; }
        public string Remark { get; set; }
    }
}
