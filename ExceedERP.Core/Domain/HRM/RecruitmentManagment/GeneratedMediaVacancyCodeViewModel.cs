using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class GeneratedMediaVacancyCodeViewModel
    {
        public int GeneratedMediaVacancyCodeViewModelId { get; set; }
        public int WorkUnitId { get; set; }
        public string VacancyCode { get; set; }
        public string MediaVacancyCode { get; set; }
        public int LookupId { get; set; }
        public string JobtitleName { get; set; }

        public int NoOfVacantPost { get; set; }

        public DateTime From { get; set; }

        public string FromAmharic { get; set; }

        public DateTime To { get; set; }

        public string ToAmharic { get; set; }

        public VacancyType RecruitmentType { get; set; }
        public string Remark { get; set; }
        public int Status { get; set; }
    }
}
