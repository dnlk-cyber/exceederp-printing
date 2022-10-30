using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System.Collections.Generic;

namespace ExceedERP.Reporting.CRM.ViewModel
{
    //class ShipmentReportViewModels
    //{
    //}

    public class ShipmentReportHeaderViewModel : Organization
    {

    }
    public class ShipmentReportViewModel 
    {
        public int Id { get; set; }

        public string Customer { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerType { get; set; }
        public string MethodOfShipment { get; set; }
        public string TruckPlateNumber { get; set; }
        public string Shop { get; set; }
        public string Store { get; set; }

        public string IssueNumber { get; set; }
        public string IssueDate { get; set; }
        public string SalesNumber { get; set; }
        public string IssueStatus { get; set; }
        public string PrintStatus { get; set; }
        public string StoreKeeperName { get; set; }
        public string Branch { get; set; }

        public string RequestedBy { get; set; }
        public string DateOfRequest { get; set; }
        public string ApprovedBy { get; set; }
        public string DateOfApproval { get; set; }
        public string ReceivedBy { get; set; }
        public string DateOfReceive { get; set; }

        public List<ShipmentLineItemReportViewModel> LineItem { get; set; } = new List<ShipmentLineItemReportViewModel>();
        public string ShipmentDate { get; internal set; }
        public int ShipmentNumber { get; internal set; }
        public string Source { get; internal set; }
        public int? SalesOrderOrInvoiceNumber { get; internal set; }
        public string ShipmentStatus { get; internal set; }
        public string TrailerPlateNumber { get; set; }
        public string IssuedBy { get; internal set; }
    }

    public class ShipmentLineItemReportViewModel
    {
        public string    Category { get; set; }
        public string ProductName { get; set; }
        public string UoM { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal   InvoiceQuantity { get; set; }
        public decimal IssuedQuantity { get; set; }
        public decimal RemainingQuantity { get; set; }
        public string ItemCode { get; set; }
        public string Variety { get; set; }
        public string Class { get; set; }

        public decimal TotalInvoiceAmount { get; set; }
        public decimal TotalIssuedAmount { get; set; }
        public decimal TotalRemainingAmount { get; set; }

        public string Remark { get; set; }



       
    }
}
