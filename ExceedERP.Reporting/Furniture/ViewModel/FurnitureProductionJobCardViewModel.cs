using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Furniture.ViewModel
{
    public class FurnitureProductionJobCardViewModel : FurnitureCommonViewModel
    {
        public string JobNo { get; set; }
        public string CustomerName { get; set; }
        public string JobType { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public DateTime DeliveryDate { get; set; }

    }
    public class FurnitureBillofQuantityMaterialViewModel
    {
        public string WorkShop { get; set; }
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        
    }
    public class FurnitureBillofQuantityLaborViewModel
    {
        public string WorkShop { get; set; }
        public string TaskCategory { get; set; }
        public int NoOfEmployee { get; set; }
        public decimal EstimatedMinute { get; set; }

    }
    public class FurnitureJobAssignmentViewModel
    {
        public string EmployeeName { get; set; }
        public string Type { get; set; }
        public string TaskCategory { get; set; }
        public DateTime TaskAssignedDate { get; set; }
        public DateTime TaskEndDate { get; set; }

    }
}
