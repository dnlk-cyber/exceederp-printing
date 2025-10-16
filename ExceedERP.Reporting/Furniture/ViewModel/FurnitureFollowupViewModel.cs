using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Furniture.ViewModel
{
    public class FurnitureFollowupViewModel
    {
        public string JobNo { get; set; }
        public string CustomerName { get; set; }
        public string JobType { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public DateTime DeliveryDate { get; set; }
    }

    public class DailyFollowupViewModel
    {
        public string WorkShop { get; set; }
        public string EmployeeName { get; set; }
        public string TaskType { get; set; }
        public decimal DailyProdPlan { get; set; }
        public decimal ActualProduction { get; set; }
        public DateTime? DateExcuted { get; set; } 
    }
    
    public class UnfinishedMaterialTransferViewModel
    {
        public string JobType { get; set; }
        public DateTime TransferDate { get; set; }
        public string Transferrer { get; set; }
        public string Reciever { get; set; }
        public string Quality { get; set; }
        public decimal Quantity { get; set; }
    }
    
}
