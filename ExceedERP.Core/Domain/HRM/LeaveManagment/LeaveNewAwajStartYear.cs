using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.LeaveManagment
{
    public class LeaveNewAwajStartYear
    {
        public int LeaveNewAwajStartYearId { get; set; }
        [Display(Name = "Fiscal_Year", ResourceType = typeof(Localization.Resources))]
        public int GlFisicalYearId { get; set; }
       
    }
}
