using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Returns
{




    public class FixedAssetReturn : Operations
    {

        public int FixedAssetReturnId { get; set; }
        [Required]
        [DisplayName("Branch or Project")]
        public int BranchId { get; set; }
        [Required]
        [DisplayName("Work Unit")]
        public int OrgStructureId { get; set; }
        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        [Required]
        [Display(Name = "Store")]
        public string StoreCode { get; set; }
        [Display(Name = " Reference")]
        public string Reference { get; set; }
        [Display(Name = "Return Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReturnDate { get; set; }
        public virtual ICollection<FixedAssetReturnItem> FixedAssetReturnItems { get; set; }
        public virtual ICollection<FixedAssetReturnValidation> FixedAssetReturnValidation { get; set; }
        //public virtual ICollection<FixedAssetReturnDistribution> FixedAssetReturnDistribution { get; set; }
    }


    //public class FixedAssetReturnDistribution:VoucherDistribution
    //{
    //    public int FixedAssetReturnDistributionId { get; set; }
    //    public int FixedAssetReturnId { get; set; }
    //    public virtual FixedAssetReturn FixedAssetReturn { get; set; }
    //}
}