using System;
using System.Drawing;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Procurements.Print.VM
{
    

    public class PoHeader : Organization
    {
        public string PoNo { get; set; }
        public string PRNo { get; set; }
        public DateTime PoDate { get; set; }
        public string Supplier { get; set; }
        public string DeliverTo { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierFax { get; set; }
        public string SupplierEmail { get; set; }
        public string SupplierTel { get; set; }
        public DateTime DeliveryPeriod { get; set; }
        public string Proforma { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public Image CreatedBySignature { get; set; }
        public string Checker { get; set; }
        public DateTime? CheckedDate { get; set; }
        public Image CheckerSignature { get; set; }
        public string Approver { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public Image ApproverSignature { get; set; }
        public decimal Total { get; set; }
    }

    public class POItemVM
    {       
        public string CatCode { get; set; }
        public string CategoryName { get; set; }
        public string ItmCode { get; set; }
        public string ItemName { get; set; }
        public string PartNo { get; set; }
        public decimal Quantity { get; set; }
        public string ItemSpecification { get; set; }
        public string Name { get; set; }
        public string ModelNo { get; set; }
        public string UoM { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public decimal Discount { get; set; }
        public string Remark { get; set; }
    }
}
