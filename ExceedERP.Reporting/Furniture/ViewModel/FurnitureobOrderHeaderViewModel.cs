using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Furniture.ViewModel
{
    public class FurnitureobOrderHeaderViewModel : FurnitureCommonViewModel
    {
        public string CustomerName { get; set; }
        public DateTime? JobStartDate { get; set; }
        public DateTime? JobEndDate { get; set; }
        public string CRVNo { get; set; }
        public string Remark { get; set; }

    }
    public class FurnitureJobOrderDetailViewModel
    {
        public string JobNO { get; set; }
        public string JobType { get; set; }
        public string UoM { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPriceBeforeVAT { get; set; }
        public decimal UnitPriceWithVAT { get; set; }
        public string Remark { get; set; } 
    }
}
