using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Web.Areas.FleetManagement.Models
{

    public class AssetRegistrationViewModel 
    {
        public int AssetRegistrationId { get; set; }
        [DisplayName("Store")]
        public string StoreCode { get; set; }
        //[Required]
        [DisplayName("Category")]
        public int FixedAssetCategoryID { get; set; }
         //[Required]
        [DisplayName("Sub Category")]
        public int FixedAssetSubCategoryID { get; set; }
         [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Size")]
        public string Size { get; set; }
        [Display(Name = "Tag/PIN")]
        public string Tag { get; set; }
        [Display(Name = "Bar Code No.")]
        public string BarCodeNo { get; set; }
        [Display(Name = "SIV")]
        public string SIV { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Purchase Date")]
        public DateTime? PurchaseDate { get; set; }
        [Display(Name = "Total Price")]
        public decimal PurchasePrice { get; set; }
        [Display(Name = "GRN")]
        public string GRN { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "GRN Date")]
        public DateTime? GRNDate { get; set; }
        [Display(Name = "Original Value")]
        public decimal OriginalValue { get; set; }
       [Display(Name = "Condition")]
       public string Condition { get; set; }
        [Display(Name = "Book Value")]
        public decimal BookValue { get; set; }
     //   public RegistrationStatus Status { get; set; }
        //only for machinery
        [Display(Name = "Machinery (Tick if yes)")]
        public bool IsMachinery { get; set; }
        public string Supplier { get; set; }
        public string Manufacturer { get; set; }
        [Display(Name = "Serial No.")]
        public string SerialNo { get; set; }
        [Display(Name = "Plate No.")]
        public string PlateNo { get; set; }
        [Display(Name = "Vehicle Type")]
        public string PlateType { get; set; }
        [Display(Name = "Chassis No.")]
        public string ChassisNo { get; set; }
        [Display(Name = "Engine No.")]
        public string EngineNo { get; set; }
        [Display(Name = "Motor No.")]
        public string MotorNo { get; set; }
        [Display(Name = "Model No.")]
        public string ModelNo { get; set; }
        [Display(Name = "Load Capacity")]
        public string LoadCapacity { get; set; }
        [Display(Name = "Fuel Type")]
        public string FuelType { get; set; }
        [Display(Name = "Manufacture Year")]
        public int ManufacturDate { get; set; }
        [Display(Name = "Net Weight")]
        public string NetWeight { get; set; }
        [Display(Name = "Total Weight")]
        public string TotalWeight { get; set; }
        [Display(Name = "Battery")]
        public string Battery { get; set; }
        [Display(Name = "Tyre")]
        public string Tyre { get; set; }
        //only for building
        [Display(Name = "Building (Tick if yes)")]
        public bool IsBuilding { get; set; }
        [Display(Name = "Location of Building")]
        public string Location { get; set; }
        [Display(Name = "Contractor")]
        public string Contractor { get; set; }
         public bool  IsRegistered { get; set; }
        
       
    }
}

