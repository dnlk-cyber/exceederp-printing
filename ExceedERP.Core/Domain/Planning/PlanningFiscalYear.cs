using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Planning
{
    public class PlanningFiscalYear
    {
        public int PlanningFiscalYearId { get; set; }
        //public GlFiscalYear GlFiscalYear { get; set; }
        [Display(Name = "Fiscal Year")]
        public int GlFiscalYearId { get; set; }
        public string Description { get; set; }
        public bool IsPlan { get; set; }
        public bool IsActual { get; set; }

    }
}
