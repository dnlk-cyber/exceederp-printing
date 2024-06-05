using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Maintenance
{
    public class SparePart : TrackUserOperation
    {

        [ScaffoldColumn(false)]
        public Guid SparePartID { get; set; }
        [Display(Name ="Category")]
        public string SpareMaterialCategoryId { get; set; }
        [NotMapped]
        public string Category { get; set; }

        [DisplayName("Name")]
        public string SparePartName { get; set; }

        [DisplayName("Part Number")]
        //[Required]
        public string PartNo { get; set; }
        public Nullable<int> Quantity { get; set; }
        [DisplayName("Unit Price")]
       
        public decimal UnitPrice { get; set; }
        [DisplayName("Receipt Number ")]
        //[Required]
        public Nullable<int> ReceiptNo { get; set; }

        public virtual SpareMaterialCategory SpareMaterialCategory { get; set; }


    }
    public class SpareMaterialCategory : TrackUserOperation
    {
        public SpareMaterialCategory()
        {
            this.SpareMaterialCategoryId = Guid.NewGuid();
        }
        [Key]
        public Guid SpareMaterialCategoryId { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
    }
}