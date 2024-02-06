using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp
{
   public class FurnitureChangeOrder : TrackUserSettingOperation
    {
        public int FurnitureChangeOrderId { get; set; }
        public int FurnitureJobId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public string Reason { get; set; }
        public int Quantity { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string InvoiceNo { get; set; }
        public string OrderNo { get; set; }
        public string PreparedBy { get; set; }
        public string TypeAndQuantityOfMaterialNeeded { get; set; }
        public string Checked { get; set; }
    }
}
