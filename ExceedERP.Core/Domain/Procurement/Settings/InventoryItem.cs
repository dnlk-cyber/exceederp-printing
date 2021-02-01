using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;

namespace ExceedERP.Core.Domain.Procurement.Settings
{


    public class ServiceCategory : TrackUserSettingOperation
    {

        [Required]
        [Key]
        [Display(Name = "Code")]
        public string ServiceCategoryCode { get; set; }
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
        [Required]
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Required]
        [DisplayName("Issue Account")]
        public string IssueAccountCode { get; set; }
        public bool HaveParent { get; set; }
        public string ParentName { get; set; }
        public string Abbreviation { get; set; }
        public virtual ICollection<ServiceItem> ServiceItems { get; set; }

    }


    public class ServiceItem : TrackUserSettingOperation
    {
        [Required]
        [Key]
        [DisplayName("Service Code")]
        public string ServiceItemCode { get; set; }
        public string ServiceCategoryCode { get; set; }
        public virtual ServiceCategory ServiceCategory { get; set; }
        [Required]
        [Display(Name = "Service Name")]
        public string Name { get; set; }
        public ItemTypes Type { get; set; }
        [DisplayName("Unit")]
        public string UoMName { get; set; }
        public virtual UoM UoM { get; set; }
        public decimal Balance { get; set; }
        public decimal AvgPrice { get; set; }
        public bool HasTransaction { get; set; }
        public bool Enable { get; set; }
    }





}
