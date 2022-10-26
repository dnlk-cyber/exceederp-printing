using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.CRM
{
    public class SalesShipment : Operations
    {
        [Key]
        public int SalesShipmentId { get; set; }

        [Display(Name = "Invoice No.")]
        public int? CashSalesInvoiceID { get; set; }
        [Display(Name ="Fs Invoice No.")]
        public string FsInvoiceNumber { get; set; }
        [Display(Name = "Sales Order No.")]
        public int? SalesOrderVoucherID { get; set; }


        //public virtual CashSalesInvoice CashSalesInvoice { get; set; }

        [Display(Name = "Shipment Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ShipmentDate { get; set; }

        public ShipmentSource Source { get; set; }

        [Display(Name ="Customer")]
        public int OrganizationCustomerId { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        [Display(Name="Store")]
        public int StoreID { get; set; }
        [Display(Name = "Period")]
        public int CRMPeriodId { get; set; }
        [Display(Name ="Method of Shipment")]
        public string MethodOfShipment { get; set; }
        [Display(Name ="Plate No.")]
        public string PlateNo { get; set; }

        [Display(Name = "Trailer Plate No.")]
        public string TrailerPlateNo { get; set; }

        [NotMapped]
        [Display(Name ="Customer Name")]
        public string NewCustomer { get; set; }

        /// <summary>
        /// maintenance job order number
        /// </summary>
        public string JobOrderNumber { get; set; }
        /// <summary>
        /// maintenance store requisition number
        /// </summary>
        public string StoreRequisitionNumber { get; set; }

        [NotMapped]
        public bool IsJobOrderClosed { get; set; }
    }

    public class SalesShipmentLineItem : TrackUserOperation
    {
        [Key]
        public int SalesShipmentLineItemId { get; set; }

        // Reference to the invoice (cash or credit sale) line item
        public int LineItemId { get; set; }

        public int SalesShipmentId { get; set; }
        public virtual SalesShipment SalesShipment { get; set; }

        [Display(Name = "Store")]
        public int SalesStoreId { get; set; }
        [Display(Name ="Product")]
        public int ProductId { get; set; }
        public string Variety { get; set; }
        public string Class { get; set; }
        [Display(Name ="Sales Price")]
        public decimal Price { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalQuantity { get; set; }
        // total shiped before current shipment
        [Display(Name ="Previous Shipment Qty")]
        public decimal ShipedQuantityTilPreShipment { get; set; }
        // at current shipment
        [Display(Name ="Shipment Qty")]
        public decimal ShipedQuantity { get; set; }
        public decimal RemainingQuantity { get; set; }
        public string  Remark { get; set; }
    }
    public class SalesShipmentValidation : VoucherValidation
    {
        [Key]
        public int SalesShipmentValidationId { get; set; }
        public int SalesShipmentId { get; set; }
        public virtual SalesShipment SalesShipment { get; set; }
    }
    public class SalesShipmentDistribution : VoucherDistribution
    {
        [Key]
        public int SalesShipmentDistributionId { get; set; }
        public int SalesShipmentId { get; set; }
        public virtual SalesShipment SalesShipment { get; set; }
        public SalesShipmentDistributionType DistributionType { get; set; }
    }
    public class CostDistributionViewModel  : TrackUserOperation
    {
        [Key]
        public int CostDistributionId { get; set; }
        public int SalesShipmentId { get; set; }
        public virtual SalesShipment SalesShipment { get; set; }
        public SalesShipmentDistributionType DistributionType { get; set; }

        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Account Description")]
        public string AccountDesc { get; set; }

        [Display(Name = "Additional Description")]
        public string AdditionalDescription { get; set; }
        [Display(Name ="Debit")]
        public decimal Debit { get; set; }
        [Display(Name ="Credit")]
        public decimal Credit { get; set; }
    }
    public class RevenueDistributionViewModel : VoucherDistribution
    {
        [Key]
        public int RevenueDistributionId { get; set; }
        public int SalesShipmentId { get; set; }
        public virtual SalesShipment SalesShipment { get; set; }
        public SalesShipmentDistributionType DistributionType { get; set; }
    }
    public enum SalesShipmentDistributionType
    {
        CostDistribution, 
        RevenuDistribution
    }

   
}
