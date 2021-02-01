using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.HRMPlan;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class ApplicantRegistrationViewModel
    {
        public int ApplicantRegistrationViewModelId { get; set; }
        public int LookUpId { get; set; }
        public int VacancyId { get; set; }
        public string Description { get; set; }

        
    }
}
