using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.FixedAsset.ViewModel
{

    public class FixedAssetPurchaseItemVm
    {
        public string Id { get; set; }
        public string Reference { get; set; }
        public DateTime ReceiveDate { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Name { get; set; }
        public string TagNo { get; set; }
        public string GRNNo { get; set; }
        public string SerialNo { get; set; }
        public string ModelNo { get; set; }
        public string ChasisNo { get; set; }
        public string EngineNo { get; set; }
        public string PlateNo { get; set; }
        public string UoM { get; set; }
        public decimal Quantity { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Sub Total")]
        public decimal SubTotal { get; set; }
        [Display(Name = "Tax Type")]
        public int GLTaxRateID { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        [Display(Name = "Store")]
        public string StoreName { get; set; }
        public string Supplier { get; set; }
    }
}
