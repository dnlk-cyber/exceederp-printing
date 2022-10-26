using System;

namespace ExceedERP.Reporting.CRM.ViewModel
{
    public class ShipmentDetailViewModel
    {
        public int ShipmentNumber { get; set; }
        public string Customer { get; set; }
        //public string ShipmentType { get; set; }
        public string DocumentNumber { get; set; }
        public string FsInvoiceNumber { get; set; }
        public string ShipmentReference { get; set; }
        public DateTime ShipmentDate { get; set; }
        public DateTime? ShipmentDueDate { get; set; }
        public string ShipmentStatus { get; set; }

        //public List<ShipmentDetailLineItemViewModel> LineItems { get; set; } = new List<ShipmentDetailLineItemViewModel>();

        public string Store { get; set; }
        public string ItemCategory { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string Variety { get; set; }
        public string Class { get; set; }
        public string UoM { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal ShipmentQuantity { get; set; }
        public decimal ShipedQuantity { get; set; }
        public decimal RemainingQuantity { get; set; }
        public string Remarks { get; set; }
        public decimal TotalQuantity { get; internal set; }
        public string DocumentType { get; internal set; }
    }
    //public class ShipmentDetailLineItemViewModel
    //{
    //    public string Store { get; set; }
    //    public string ItemCategory { get; set; }
    //    public string ItemName { get; set; }
    //    public string ItemCode { get; set; }
    //    public string Variety { get; set; }
    //    public string Class { get; set; }
    //    public string UoM { get; set; }
    //    public decimal UnitPrice { get; set; }

    //    public decimal ShipmentQuantity { get; set; }
    //    public decimal ShipedQuantity { get; set; }
    //    public decimal RemainingQuantity { get; set; }
    //    public string Remarks { get; set; }
    //    public decimal TotalQuantity { get; internal set; }
    //}
}
