using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExceedERP.Web.Areas.Printing.Controllers.ProductionFollowUp.ViewModel
{
    public class ChangeOfOrderVM
    {
        public int JobId { get; set; }
        public DateTime Date { get; set; }
        public string TypeAndQuantityOfMaterialNeeded { get; set; }
        public int ChangedQuantity { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string Reason { get; set; }
    }
}