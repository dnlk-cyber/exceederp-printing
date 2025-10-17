using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Printing.ViewModel
{
    public class JobCardViewModel : PrintingCommonViewModel
    {
        public string JobOrderNo { get; set; }
        public string CustomerName { get; set; }
        public string JobType { get; set; }
        public string JobDescription { get; set; }
        public string PaperForText { get; set; }
        public string PaperForCover { get; set; }
        public int TextNoColor { get; set; }
        public int CoverNoColor { get; set; }
        public int Page { get; set; }
        public decimal Quantity { get; set; }
        public DateTime OrderedDate { get; set; }
        public DateTime DeliveryDate { get; set; }

    }
    public class BOQMaterialViewModel
    {
        public string MaterialType { get; set; }
        public string ItemName { get; set; }
        public string UOM { get; set; }
        public decimal Quantity { get; set; }

    }
}
