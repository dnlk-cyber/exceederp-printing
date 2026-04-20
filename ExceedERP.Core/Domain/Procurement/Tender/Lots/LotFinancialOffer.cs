using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Tender.Lots
{
    public class LotFinancialOffer : TrackUserOperation
    {
        public int LotFinancialOfferID { get; set; }
        public int LotID { get; set; }
        public virtual Lot Lot { get; set; }
        [Display(Name = "Supplier ID")]//supplier database
        public string SupplierId { get; set; }
        [Display(Name = "New Supplier")]//manual supplier
        public bool IsNewSupplier { get; set; }
        [Display(Name = "Supplier Name")]//manual supplier
        public string SupplierName { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }
}
