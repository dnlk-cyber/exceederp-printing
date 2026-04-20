using ExceedERP.Core.Domain.Manufacturing.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Furniture.ViewModel 
{
    public class FurnitureEstimationViewModel : FurnitureCommonViewModel
    {
        public int EstimationNo { get; set; }
        public string CustomerName { get; set; }
        public string JobType { get; set; }
        public decimal Quantity { get; set; }
        public DateTime? Date { get; set; }
    }
    public class MatrialCostViewModel
    {
        public string ItemName { get; set; }
        public string UOM { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }

    }
    public class LaborCostViewModel
    {
        public WorkShop TaskType { get; set; }
        public decimal TotalWorkingTime { get; set; }
        public decimal CostPerMinute { get; set; }
        public decimal TotalCost { get; set; }
    }

    public class OverAllCostViewModel
    {
        public decimal MaterialCost { get; set; }
        public decimal LaborCost { get; set; }
        public decimal OverHeadCost { get; set; }
        public decimal ManufacturedCost { get; set; }
        public decimal AdminstCost { get; set; }
        public decimal TotalCost { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal SellingPrice { get; set; }

    }

    public class FurnitureBoqViewModel : FurnitureCommonViewModel
    {
        public string JobCode { get; set; }
        public string JobType { get; set; }
        public string CustomerName { get; set; }
        public decimal Quantity { get; set; }

       
    }
    public class MaterialBoqViewModel
    {
        public WorkShop WorkShop { get; set; }
        public string MaterialCategory { get; set; }
        public string MaterialItem { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
    }
    public class LaborBoqViewModel
    {
        public WorkShop WorkShop { get; set; }
        public int NumberOfEmployees { get; set; }
        public decimal EstimatedMinute { get; set; }
    }
}
