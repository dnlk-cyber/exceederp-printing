using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
   public class InternalApplicantExperience
    {
        [Key]
        public int InternalApplicantExperienceId { get; set; }
        public int ApplicationFormId { get; set; }
        public int InternalApplicantsId { get; set; }
        public OrganizationType OrganizationType { get; set; }
        public int JobTitleId { get; set; }
        public int? Jobtitle { get; set; }
        public int? Year { get; set; }
        public string OrganizationName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? From { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? To { get; set; }
        public string Remark { get; set; }
    }
}
