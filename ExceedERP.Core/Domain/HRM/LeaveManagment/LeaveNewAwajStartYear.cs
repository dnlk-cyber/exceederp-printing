using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.LeaveManagment
{
   public class LeaveNewAwajStartYear
    {
        public int LeaveNewAwajStartYearId { get; set; }
        [Display(Name = "Fiscal_Year", ResourceType = typeof(Localization.Resources))]
        public int GlFisicalYearId { get; set; }
       
    }
}
