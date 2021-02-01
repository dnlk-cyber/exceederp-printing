using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AR.SalesOrder
{
    public enum OrderType
    {
        Cash,
        Credit
    }
    public enum OrderStatus
    {
        WaitingService,
        DeliveredService,
        DelayedService
    }
   public  class ArSalesOrder: TrackTransaction
    {
        public int ArSalesOrderId { get; set; }
        [Display(Name = "Order Type")]
        public OrderType OrderType { get; set; }
        [Display(Name = "Customer")]
        public string CustomerId { get; set; }
        public bool IsNewCustomer { get; set; }
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }//for cash sales 
        [Display(Name = "Order Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime OrderDate { get; set; }
        [Display(Name = "Station Name")]
        public string Station { get; set; }
        
        [Display(Name = "Sales Person")]
        public int SalesPersonId { get; set; }
        [Display(Name = "Partner")]
        public int PartnerId { get; set; }
        [Display(Name = "Payment Terms")]
        public int PaymentTerm { get; set; }
        [Display(Name = "Order Reference")]
        public string OrderReference { get; set; }
        [Display(Name = "Order Status")]
        public OrderStatus OrderStatus { get; set; }
        [Display(Name = "Invoice Status")]
        public InvoiceStatus InvoiceStatus { get; set; }
        [Display(Name = "Amount Due")]
        public decimal AmountDue { get; set; }
        [DisplayName("Branch or Project")]
        public int BranchId { get; set; }
        [DisplayName("Product")]
        public string Product { get; set; }
        [DisplayName("Length Second/Minute")]
        public string Length { get; set; }
        [DisplayName("Repeatition")]
        public string Repeatition { get; set; }
        public bool IsOnlineTransferred { get; set; }
        public string OnlineTransferredBy { get; set; }
        public DateTime? OnlineTransferredTime { get; set; }
        public bool IsPostedToSecondLedger { get; set; }
        public bool IsReceiptPrinted { get; set; }
        public virtual ICollection<ArSalesOrderLine> ArSalesOrderLines { get; set; }
        public virtual ICollection<ArSalesOrderPartialPrint> ArSalesOrderPartialPrints { get; set; }
        public virtual ICollection<ArSalesOrderAdvanceLine> ArSalesOrderAdvanceLines { get; set; }
        public virtual ICollection<ArSalesOrderValidation> ArSalesOrderValidations { get; set; }
        public virtual ICollection<ArSalesOrderDistribution> ArSalesOrderDistribution { get; set; }
        public virtual ICollection<ArSalesOrderStatus> ArSalesOrderStatus { get; set; }
        public virtual ICollection<ArSalesOrderBroadcastTime> ArSalesOrderBroadcastTime { get; set; }
        public virtual ICollection<ArSalesOrderPaymentSchedule> ArSalesOrderPaymentSchedules { get; set; }
        public virtual ICollection<ArSalesOrderContract> ArSalesOrderContracts { get; set; }
    }

    public class ArSalesOrderPartialPrint : Operations
    {
        public int ArSalesOrderPartialPrintId { get; set; }
        public int ArSalesOrderId { get; set; }
        public virtual ArSalesOrder ArSalesOrder { get; set; }
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public bool IsReceiptPrinted { get; set; }
        public virtual ICollection<ArSalesOrderPartial> ArSalesOrderPartials { get; set; }
        public virtual ICollection<ArSalesOrderPartialPrintDistribution> ArSalesOrderPartialPrintDistributions { get; set; }
        public virtual ICollection<ArSalesOrderPartialPrintValidation> ArSalesOrderPartialPrintValidations { get; set; }
    }
    public class ArSalesOrderPaymentSchedule : TrackUserOperation
    {
        public int ArSalesOrderPaymentScheduleId { get; set; }
        public int ArSalesOrderId { get; set; }
        public virtual ArSalesOrder ArSalesOrder { get; set; }
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public string Amount { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
    }
    public class ArSalesOrderContract : TrackUserOperation
    {
        public int ArSalesOrderContractId { get; set; }
        public int ArSalesOrderId { get; set; }
        public virtual ArSalesOrder ArSalesOrder { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
