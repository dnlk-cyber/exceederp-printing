using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CRM
{
    public class CRMDashboardViewModel 
    {
        public CRMDashboardTotalTodateSalesViewModel TodateTotalSales { get; set; } = new CRMDashboardTotalTodateSalesViewModel();
        public CRMDashboardTotalTodateSalesViewModel TodateCashSales { get; set; } = new CRMDashboardTotalTodateSalesViewModel();
        public CRMDashboardTotalTodateSalesViewModel TodateCreditSales { get; set; } = new CRMDashboardTotalTodateSalesViewModel();

        public CRMDashboardTodaySalesViewModel TotalSalesToday { get; set; } = new CRMDashboardTodaySalesViewModel();
        public CRMDashboardTodaySalesViewModel CashSalesToday { get; set; } = new CRMDashboardTodaySalesViewModel();
        public CRMDashboardTodaySalesViewModel CreditSalesToday { get; set; } = new CRMDashboardTodaySalesViewModel();

    }

    public class CRMDashboardTotalTodateSalesViewModel
    {
        public CRMDashboardSalesType SalesType { get; set; }
        public decimal Amount { get; set; }
        public string   AmountStr { get; set; }

    }
    public class CRMDashboardTodaySalesViewModel
    {
        public CRMDashboardSalesType SalesType { get; set; }
        public decimal Amount { get; set; }
        public string AmountStr { get; set; }

    }
    public class CRMDashboardSalesBranchDistributionViewModel
    {
        public string Branch { get; set; }
        public decimal Amount { get; set; }
    }
    public enum CRMDashboardSalesType
    {
        All,
        Cash,
        Credit
    }
}
