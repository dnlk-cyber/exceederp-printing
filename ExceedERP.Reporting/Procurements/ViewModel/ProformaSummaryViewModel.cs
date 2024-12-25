using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Procurements.ViewModel
{
   public class ProformaAndLotSummaryViewModel
    {
        public string Description { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public string SupplierName { get; set; }
        public string UPrice { get; set; }
        public string TPrice { get; set; }
    }

    public class ProformaTeamVm
    {
        public string FullName { get; set; }
        public string Title { get; set; }
    }
}
