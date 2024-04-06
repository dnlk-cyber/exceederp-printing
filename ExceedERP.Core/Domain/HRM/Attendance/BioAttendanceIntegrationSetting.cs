using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.Attendance
{
    public class BioAttendanceIntegrationSetting
    {
        public int BioAttendanceIntegrationSettingId { get; set; }
        [Display(Name = "Rpt_Type", ResourceType = typeof(Localization.Resources))]
        public ApiIntegrationSettingAttribute Type { get; set; }
        [Display(Name = "HR_RecruitmentManagment_ApplicantScreeningResult_SettingValue", ResourceType = typeof(Localization.Resources))]
        public string Value { get; set; }
        [Display(Name = "HR_SelfCare_TrainingApplicant_Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}
