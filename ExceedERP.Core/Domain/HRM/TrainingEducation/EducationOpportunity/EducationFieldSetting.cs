using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation.EducationOpportunity
{
    public class EducationFieldSetting
    {
        public int EducationFieldSettingId { get; set; }
        public string EducationField { get; set; }
        public int EducationLevelId { get; set; }
        public int RelevantFieldPoint { get; set; }
        public int RelatedFieldPoint { get; set; }
    }
}
