using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
  public  class PlanEvaluationViewModel : Organization
    {


        public Guid FleetsID { get; set; }
        public string Equipment { get; set; }
        public string Year  { get; set; }
        public string Month { get; set; }
        public decimal PlanWorkingtime { get; set; }
        public decimal PlanDowntime { get; set; }
        public decimal PlanIdletime { get; set; }
        public decimal PlanFuelConsumption { get; set; }

        public decimal ActualFuelConsumption { get; set; }
        public decimal ActualPlanWorkingtime { get; set; }
        public decimal ActualPlanDowntime { get; set; }
        public decimal ActualPlanIdletime { get; set; }
        public decimal ActualPlanFuelConsumption { get; set; }

        public decimal DiffPlanFuelConsumption { get; set; }
        public decimal DiffPlanWorkingtime { get; set; }
        public decimal DiffActualPlanDowntime { get; set; }
        public decimal DiffActualPlanIdletime { get; set; }
        public decimal DiffActualPlanFuelConsumption { get; set; }


         public decimal RentalRate { get; set; }
        public string UnitOfMeasurement { get; set; }
        public decimal PlanIncome { get; set; }
        public decimal PlanTyreUtilization { get; set; }
        public decimal PlanEquipmentsPittyCashExpenditure { get; set; }
        public string Description { get; set; }
      
    }
}
