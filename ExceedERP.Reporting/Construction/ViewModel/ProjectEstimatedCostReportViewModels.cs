using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.ViewModel
{
    //class ProjectEstimatedCostReportViewModels
    //{
    //}
    public class ProjectEstimatedCostProjectVM
    {
        public ProjectEstimatedCostProjectVM()
        {
            MaterialList = new List<ProjectEstimatedCostMaterialVM>();
            LaborList = new List<ProjectEstimatedCostLaborVM>();
            EquipmentList = new List<ProjectEstimatedCostEquipmentVM>();
        }
        public int Id { get; set; }
        public string Name { get; set; }

        public decimal MaterialCost { get; set; }
        public decimal LaborCost { get; set; }
        public decimal EquipmentCost { get; set; }

        public List<ProjectEstimatedCostMaterialVM> MaterialList { get; set; }
        public List<ProjectEstimatedCostLaborVM> LaborList { get; set; }
        public List<ProjectEstimatedCostEquipmentVM> EquipmentList { get; set; }

    }
    public class ProjectEstimatedCostMaterialVM
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalCost { get; set; }
    }
    public class ProjectEstimatedCostEquipmentVM
    {
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalCost { get; set; }
    }
    public class ProjectEstimatedCostLaborVM
    {
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal IndexedHourlySalary { get; set; }
        public decimal TotalCost { get; set; }
    }
}
