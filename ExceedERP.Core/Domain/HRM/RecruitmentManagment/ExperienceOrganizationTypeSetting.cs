using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
   public class ExperienceOrganizationTypeSetting
    {
        public int ExperienceOrganizationTypeSettingId { get; set; }
        public OrganizationType OrganizationType { get; set; }
        public string Remark { get; set; }
    }
}
