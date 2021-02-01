using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Budget.GAAP
{
    public class InvestmentPlan: TrackUserOperation
    {
        public int InvestmentPlanId { get; set; }
             [Required]
        public string Name { get; set; }
             [DisplayName("1st Quarter")]
             public decimal First { get; set; }
             [DisplayName("2nd Quarter")]
             public decimal Second { get; set; }
             [DisplayName("3rd Quarter")]
             public decimal Third { get; set; }
             [DisplayName("4th Quarter")]
             public decimal Fourth { get; set; }
        [NotMapped]
        public decimal Total { get; set; }

        [DisplayName("From Account")]
        public string FromAccCode { get; set; }
        [DisplayName("To Account")]
        public string ToAccCode { get; set; }
          [DisplayName("Year")]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
    }
}
