using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
   public class RecruitmetnInterviewerPerseon
    {
        public int RecruitmetnInterviewerPerseonId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_PersonEmployeeID", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public int VacancyId { get; set; }
        public string Remark { get; set; }
    }
}
