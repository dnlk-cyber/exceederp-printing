using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.LeaveManagment
{
   public class LeaveEntitleStartingDate
    {
        public int LeaveEntitleStartingDateId { get; set; }
        [Display(Name = "Leave_Entitle_Starting", ResourceType = typeof(Localization.Resources))]
        public LeaveEntitleStarting Start { get; set; }
        [Display(Name = "Common_Identification_Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
    
}
