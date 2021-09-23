using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Construction
{
    //class ConstructionPermissions
    //{
    //}
    public class PurchasePriceComparison : TrackUserOperation
    {
        [Key]
        public int PurchasePriceComparisonId { get; set; }
        [Display(Name ="Project")]
        public int ProjectInformationId { get; set; }

        public string ItemName { get; set; }
        [Display(Name ="Unit")]
        public int UnitOfMeasurementId { get; set; }
        [Display(Name ="Qty")]
        public decimal Quantity { get; set; }

        [Display(Name ="Selected Price")]
        public decimal SelectedPrice { get; set; }
        [Display(Name ="Selected Supplier")]
        public string SelectedSupplier { get; set; }
        public decimal SubTotal { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public virtual ProjectInformation Project { get; set; }
    }
    public class PurchaseSupplier : TrackUserOperation
    {
        [Key]
        public int PurchaseSupplierId { get; set; }

        public int PurchasePriceComparisonId { get; set; }

        [Display(Name ="Name")]
        public string SupplierName { get; set; }
        public decimal  UnitPrice { get; set; }
        public bool IsSelectedSupplier { get; set; }

        public virtual PurchasePriceComparison PurchasePriceComparison { get; set; }
    }
}
