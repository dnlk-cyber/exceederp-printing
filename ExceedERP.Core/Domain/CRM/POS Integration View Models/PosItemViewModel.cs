using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CRM
{
    public class PosItemViewModel
    {
        public string CategoryIdentifierId { get; set; }
        public string CategoryName { get; set; }
        public string ItemIdentifierId { get; set; }
        public string ItemDescription { get; set; }
        public string ItemCode { get; set; }
        public string UomIdentifierId { get; set; }
        public string UomName { get; set; }
        public decimal SalesUnitPrice { get; set; }
        public int TaxType { get; set; }
    }
}
