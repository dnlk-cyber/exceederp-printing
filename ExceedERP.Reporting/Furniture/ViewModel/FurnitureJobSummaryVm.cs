using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Furniture.ViewModel
{
    public class FurnitureJobSummaryVm
    {
        public string JobNo { get; set; }
        public string CustomerName { get; set; }
        public string JobType { get; set; } 
        public decimal Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Transport { get; set; }
        public TypeOfPayment PaymentType { get; set; }
    }

    
}
