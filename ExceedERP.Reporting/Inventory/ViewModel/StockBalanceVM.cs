using System;
using System.Collections.Generic;
using ExceedERP.Core.Domain.HRM.Payroll;

namespace ExceedERP.Reporting.Inventory.ViewModel
{
    public class StockBalanceMultipleVm
    {
        public List<StockBalanceBinCard> StockBalanceBinCards { get; set; }
        public string AccountCode { get; set; }
        public string ItemName { get; set; }
        public string PartNo { get; set; }
        public string Unit { get; set; }
        public decimal GrandTotal { get; set; }

    }
    public class StockBalanceBinCard
    {
        public DateTime? Date { get; set; }
        public string StoreCode { get; set; }
        public string ReferenceDoc { get; set; }
        public decimal ReceivedQTY { get; set; }
        public decimal ReceivedUPrice { get; set; }
        public decimal ReceivedTPrice { get; set; }

        public decimal IssuedQTY { get; set; }
        public decimal IssuedUPrice { get; set; }
        public decimal IssuedTPrice { get; set; }
        public decimal IssuedOrReceivedQty { get; set; }
        public decimal IssuedOrReceivedUPrice { get; set; }

        public decimal BalanceQTY { get; set; }
        public decimal BalanceUPrice { get; set; }
        public decimal BalanceTPrice { get; set; }

        public decimal AveragePrice { get; set; }
        public int Id { get; set; }
        public string OriginalRef { get; set; }
        public string SupplierName { get; set; }
        public string IssuedTo { get; set; }
    }

   //public  class StockBalanceVm
   // {
       
        
   //     public DateTime? Date { get; set; }
   //     public string ReferenceDoc { get; set; }
   //     public decimal ReceivedQTY { get; set; }
   //     public decimal ReceivedUPrice { get; set; }
   //     public decimal ReceivedTPrice { get; set; }

   //     public decimal IssuedQTY { get; set; }
   //     public decimal IssuedUPrice { get; set; }
   //     public decimal IssuedTPrice { get; set; }

   //     public decimal BalanceQTY { get; set; }
   //     public decimal BalanceUPrice { get; set; }
   //     public decimal BalanceTPrice { get; set; }

   //     public decimal AveragePrice { get; set; }

   //    //new
   //     public string AccountCode { get; set; }
   //     public string ItemName { get; set; }
   //     public string PartNo { get; set; }
   //     public string Unit { get; set; }
        
       
   // }
}
