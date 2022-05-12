using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.ProductionFollowUp.ViewModel
{
    public class JobSupplyandSalesVM : Organization
    {
        public string From { get; set; }
        public string To { get; set; }
        public decimal PreviousWeeklySupply { get; set; }
        public decimal WeeklySupplyAndSalesPlan { get; set; }
        public decimal WeeklyReceivedActual { get; set; }
        public decimal WeeklySupplyPerformance { get; set; }
        public decimal WeeklySalesActual { get; set; }
        public decimal WeeklySalesPerformance { get; set; }
        public string PreparedBy { get; set; }
        public string CheckedBy { get; set; }
        public string ApprovedBy { get; set; }
        public List<JobSupplyandSalesItemVM> JobSupplyandSalesItemVMs { get; set; }


    }
    public class JobSupplyandSalesItemVM
    {
        public string Code { get; set; }       
        public decimal DailySupply { get; set; }
        public decimal PreviousWeeklySupply { get; set; }
        public decimal WeeklySupplyAndSalesPlan { get; set; }
        public decimal WeeklyReceivedActual { get; set; }
        public decimal WeeklySupplyPerformance { get; set; }
        public decimal WeeklySalesActual { get; set; }
        public decimal WeeklySalesPerformance { get; set; }
    }
}
