using ExceedERP.Core.Domain.Printing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Printing.ViewModel
{
    public class PrintSummaryHeadVM : PrintingCommonViewModel
    {
        public string CustomerName { get; set; }
        
    }

    public class PrintSummaryDetailVM
    {
        public PrintJobType PrintJobType { get; set; } 
        public string JobDesc { get; set; }
        public decimal Quantity { get; set; }
        public decimal MaterialCost { get; set; }
        public decimal LaborCost { get; set; }
        public decimal OverHeadCost { get; set; }
        public decimal ManufCost { get; set; }
        public decimal AdminCost { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
