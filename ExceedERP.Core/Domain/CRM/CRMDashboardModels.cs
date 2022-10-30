using ExceedERP.Core.Domain.Common.HRM;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CRM
{
    public class CRMDashboardTodateSalesData
    {
        [Key]
        public int CRMDashboardTodateSalesDataId { get; set; }
        public CRMDashboardSalesDataType SalesType { get; set; }
        public decimal Amount { get; set; }
    }
    public class CRMDashboardTodateSalesBranchData
    {
        [Key]
        public int CRMDashboardTodateSalesBranchDataId { get; set; }
        public int BranchId { get; set; }
        public CRMDashboardSalesDataType SalesType { get; set; }
        public decimal Amount { get; set; }
        public virtual Branch Branch { get; set; }
    }
    public enum CRMDashboardSalesDataType
    {
        TotalSales,
        CashSales,
        CreditSales
    }
}
