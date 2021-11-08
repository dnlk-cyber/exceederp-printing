using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
   public class RecruitmentApplicantPassSetting
    {
        public int RecruitmentApplicantPassSettingId { get; set; }
        public double TotalValue { get; set; }
        public double PassScore { get; set; }
        public int VacancyId { get; set; }
        public string Remark { get; set; }
    }
}
