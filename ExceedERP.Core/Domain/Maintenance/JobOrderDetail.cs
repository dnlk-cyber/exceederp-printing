
using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Maintenance
{
    public  class JobOrderDetail : OperationWithSecondValidation
    {
        [ScaffoldColumn(false)]
        public Guid JobOrderDetailID { get; set; }
        public Guid JobOrderID { get; set; }
        [DisplayName("Vehicle Category")]
        public string SubCategoryCode { get; set; }
        [DisplayName("Service Group")]
        public ServiceGroup ServiceGroup { get; set; }
        public int ServiceGroupSettingId { get; set; }
        public Guid ServiceID { get; set; }
        public string ServiceName { get; set; }
        public bool IsFromWorkShop { get; set; }
        public string DescriptionOfJobs { get; set; }
        public bool IsFromWorkShopRejected { get; set; }
        public bool IsFromWorkShopApproved { get; set; }
        [DisplayName("Cause")]
        public Guid JobCausesId { get; set; }
        [DisplayName("Service Name")]
        public Guid RepairTypeID { get; set; }
        public RepairType RepairType { get; set; }
        public ApprovalStatuses ApprovalStatus { get; set; }
        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> StartingDate { get; set; }
        [DisplayName("End Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public Nullable<DateTime> ToDate { get; set; }


        public bool IsApproved { get; set; }
        [NotMapped]
        public string Section { get; set; }
        [NotMapped]
        public string Cause { get; set; }
        [NotMapped]
        public string MaintenanceCenter { get; set; }
        [NotMapped]
        public decimal Efficency { get; set; }
        public decimal Odometer { get; set; }
        //public virtual Service Service { get; set; }
        public virtual JobCauses JobCauses { get; set; }

        //nav to ServiceCenter
        [Display(Name ="Service Center")]
        public int? ServiceCenterId { get; set; } // new ft added

    }
}
