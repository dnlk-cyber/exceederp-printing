using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.ProductionFollowUp
{
    public class ProductionPlan
    {
        public int ProductionPlanId { get; set; }
        [Display(Name="Fiscal Year")]
        public int GlFiscalYearId   { get; set; }
        [Display(Name = "Total Production Plan")]
        public decimal? TotalProduct { get; set; }
        [Display(Name = "Total Sales Plan")]
        public decimal TotalPrice { get; set; }
    }
}
