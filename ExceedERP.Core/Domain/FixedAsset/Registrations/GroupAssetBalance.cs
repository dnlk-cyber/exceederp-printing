using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Setting;

namespace ExceedERP.Core.Domain.FixedAsset.Registrations
{
   
    public class GroupAssetBalance : TrackUserOperation
    {
        public int GroupAssetBalanceId { get; set; }
        [DisplayName("Category")]
        public int FixedAssetCategoryID { get; set; }
        public virtual FixedAssetCategory FixedAssetCategory { get; set; }
        [DisplayName("Sub Category")]
        public int FixedAssetSubCategoryID { get; set; }
        public virtual FixedAssetSubCategory FixedAssetSubCategory { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Size")]
        public string Size { get; set; }
        [Display(Name = "Group Code")]
        public string GroupCode { get; set; }
        [Display(Name = "Type")]
        public LocationType LocationType { get; set; }

        [DisplayName("Employee")]
        public string EmployeeId { get; set; }
        [DisplayName("Store")]
        public string StoreCode { get; set; }        
       
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]      
        public DateTime Date { get; set; }
        public string Description { get; set; }
        [Display(Name = "Branch Or Project")]
        public int BranchId { get; set; }
        [DisplayName("Work Unit")]
        public string OrgStructureId { get; set; }
        [DisplayName("Transaction Type")]
        public string TransactionType { get; set; }
        [DisplayName("Transaction No")]
        public string TransactionNo { get; set; }
        public int Quanity { get; set; }
        public int UnitPrice { get; set; }
        public int TotalPrice { get; set; }

    }
}
