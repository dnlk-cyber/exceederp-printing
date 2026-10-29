using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.Payroll.Setting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.Discipline
{
    public class HRMContributionSetting: TrackUserOperation
    {
        [Key]
        public int HRMContributionSettingId { get; set; }
        [Display(Name = "Contribution Type")]
        public int ContributionTypeId { get; set; }
        public ContributionType Contribution { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
    }

}
