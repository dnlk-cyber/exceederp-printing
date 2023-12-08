using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CRM
{
    public class SalesReturnLine : TrackUserSettingOperation
    {
        [Key]
        public int SalesReturnLineId { get; set; }

        public virtual SalesReturn SalesReturn { get; set; }
        public int SalesReturnId { get; set; }

        [Display(Name ="Payment Type")]
        public SalesReturnPaymentType PaymentType { get; set; }

        [Display(Name ="Sales Reference")]
        public string SalesReference { get; set; }

        [Display(Name ="Shipment Reference")]
        public string ShipmentReference { get; set; }

        [Display(Name ="Product")]
        public int? ProductId { get; set; }

        [Display(Name ="Unit Price")]
        public decimal UnitPrice { get; set; }

        public decimal Quantity { get; set; }

        [Display(Name ="Sub Total")]
        public decimal SubTotal { get; set; }

        [Display(Name ="Tax Type")]
        public int? TaxTypeId { get; set; }

        [Display(Name ="Tax Amount")]
        public decimal TaxAmount { get; set; }

        [Display(Name ="Total Amount")]
        public decimal TotalAmount { get; set; }

        public string Remark { get; set; }

    }
}
