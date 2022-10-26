using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.CRM.ViewModel
{
   public  class SalesPlanAndActualReportViewModel
    {
        public string ItemType { get; set; }
        public string UoM { get; set; }
        // current month
        public decimal CurrentMonthPlanQuantity { get; set; }
        public decimal CurrentMonthActualQuantity { get; set; }
        public decimal PercentageOfCurrentMonthActualQuantity { get; set; }
        public decimal CurrentMonthPlanValue { get; set; }
        public decimal CurrentMonthActualValue { get; set; }
        public decimal PercentageOfCurrentMonthActualValue { get; set; }

        // year todate
        public decimal YearTodatePlanQuantity { get; set; }
        public decimal YearTodateActualQuantity { get; set; }
        public decimal PercentageOfYearTodateActualQuantity { get; set; }
        public decimal YearTodatePlanValue { get; set; }
        public decimal YearTodateActualValue { get; set; }
        public decimal PercentageOfYearTodateActualValue { get; set; }

    }
}
