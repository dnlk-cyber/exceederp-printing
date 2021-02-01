using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Procurements.ViewModel
{
    public class MonthlyPurchaseByCategoryAndAmountReportViewModel
    {
        public int No { get; set; }
        public string PurchaseType { get; set; }
        public decimal Price { get; set; }
        public string Remark { get; set; }
    }
}
