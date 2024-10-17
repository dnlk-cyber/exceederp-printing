using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Depreciation;
using ExceedERP.Core.Domain.FixedAsset.Setting;

namespace ExceedERP.Core.Domain.FixedAsset.Registrations
{
    public enum RegistrationStatus
    {
        Active,
        Salvage,
        Revalued,
        Disposed,
        Sold,
        NotFound
    }
    public class AssetRegistration : TrackUserOperation
    {
        public int AssetRegistrationId { get; set; }

        [DisplayName("Branch")]
        public int BranchId { get; set; }
        [DisplayName("Business Unit")]
        public int BusinessUnitId { get; set; }

        [DisplayName("Work Unit")]
        public string OrgStructureId { get; set; }

        [DisplayName("Employee")]
        public string EmployeeId { get; set; }

        [DisplayName("Store")]
        public string StoreCode { get; set; }

        [DisplayName("Category"), Required]
        public int FixedAssetCategoryID { get; set; }

        public virtual FixedAssetCategory FixedAssetCategory { get; set; }

       
        [DisplayName("Sub Category"), Required]
        public int FixedAssetSubCategoryID { get; set; }

        public virtual FixedAssetSubCategory FixedAssetSubCategory { get; set; }

        public int  FixedAssetNameID { get; set; }

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
        [Display(Name = "SIV Date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true), DataType(DataType.Date)]
        public DateTime? SIVDate { get; set; }


        [Display(Name = "Purchase Date"), DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? PurchaseDate { get; set; }

        [Display(Name = "Purchase Price")]
        public decimal PurchasePrice { get; set; }

        [Display(Name = "GRN")]
        public string GRN { get; set; }

        
        [Display(Name = "GRN Date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true), DataType(DataType.Date)]
        public DateTime? GRNDate { get; set; }

        [Display(Name = "Original Value")]
        public decimal OriginalValue { get; set; }
        [Required]

        [Display(Name = "Condition")]
        public int FixedAssetConditionId { get; set; }

        public virtual FixedAssetCondition Condition { get; set; }

        [Display(Name = "Book Value")]
        public decimal BookValue { get; set; }

        public RegistrationStatus Status { get; set; }


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
        [Display(Name = "Color")]
        public string Color { get; set; }


        //only for building
        [Display(Name = "Building (Tick if yes)")]
        public bool IsBuilding { get; set; }

        [Display(Name = "Location of Building")]
        public string Location { get; set; }

        [Display(Name = "Contractor")]
        public string Contractor { get; set; }

       
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true), DataType(DataType.Date), Display(Name = "Construction Start Date")]
        public DateTime? ConstructionStartDate { get; set; }

        
        [Display(Name = "Construction End Date "), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true), DataType(DataType.Date)]
        public DateTime? ConstructionEndDate { get; set; }

        [DisplayName("Is Group Item ?")]
        public bool IsGroup { get; set; }

        [Display(Name = "Group Code")]
        public string GroupCode { get; set; }

        public int Quanity { get; set; }
        //[Required]
        public string Unit { get; set; }
        public string Remark { get; set; }
        public bool Disable { get; set; }

        public virtual ICollection<Location> Locations { get; set; }
        public virtual ICollection<Appreciation> Appreciations { get; set; }
        public virtual ICollection<AnnualDepreciation> Depreciations { get; set; }
        public virtual ICollection<FixedAssetRevaluation> FixedAssetRevaluations { get; set; }
        public virtual ICollection<AssetDisposal> AssetDisposals { get; set; }
        public virtual ICollection<Sales> Sales { get; set; }
        public virtual ICollection<LocationTracking> LocationTrackings { get; set; }
    }
}
