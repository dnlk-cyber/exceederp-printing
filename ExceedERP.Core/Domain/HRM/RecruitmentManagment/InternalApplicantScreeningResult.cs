using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class InternalApplicantScreeningResult
    {
        public int InternalApplicantScreeningResultId { get; set; }
        public int ApplicationFormId { get; set; }
        public int VacancyId { get; set; }
        public string Remark { get; set; }
    }
}
