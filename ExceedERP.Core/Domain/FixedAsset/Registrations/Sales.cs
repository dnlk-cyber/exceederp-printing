using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Registrations
{
public enum FixedAssetCategoryList
    {
    Capitalaized ,CIP ,Expenssed
    }
    public class  Sales : TrackUserOperation
    {
        public int  SalesID { get; set; }
        [Required]
        public int AssetRegistrationId { get; set; }
        [Display(Name = "Fixed Asset")]
        public virtual  AssetRegistration  AssetRegistration { get; set; }
        [Required]
        [Display(Name = "Sales Amount")]
        public decimal Amount { get; set; }
         [Required]
        [DisplayName("Tax Type")]
        public int GLTaxRateID { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        [Required]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }
        public string Description { get; set; }
    }
}
