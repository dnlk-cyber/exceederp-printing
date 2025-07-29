using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Maintenance
{
    public class ServiceCrewWorkLog : Operations
    {
        [Key]
        public Guid ServiceCrewWorkLogId { get; set; }
        public Guid JobId { get; set; }
        [DisplayName("Vehicle Category")]
        public string SubCategoryCode { get; set; }
        [DisplayName("Service Group")]
        public ServiceGroup ServiceGroup { get; set; }
        [DisplayName("Service")]
        public Guid JobOrderDetailId { get; set; }
        [DisplayName("Workshop Type")]
        public WorkShopType WorkShopType { get; set; }

        [DisplayName("Mecanic")]
        public Guid MecanicId { get; set; }
        public int Date { get; set; }
        [DataType(DataType.Date)]
        public DateTime? RecordedDate { get; set; }
        public string RepiarTypeId { get; set; }
        [DisplayName("  Morning From")]
        //[Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh-mm}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> WorkingHoursMorningFrom { get; set; }
        [DisplayName("  Morning To")]
        //[Required]
        [DisplayFormat(DataFormatString = "{0:hh-mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Time)]
        public Nullable<DateTime> WorkingHoursMorningto { get; set; }
        [DisplayName("  Afternoon From ")]
        //[Required]
        [DisplayFormat(DataFormatString = "{0:hh-mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Time)]
        public Nullable<DateTime> WorkingHoursAfternoonFrom { get; set; }
        [DisplayName("  Afternoon To ")]
        //[Required]
        [DisplayFormat(DataFormatString = "{0:hh-mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Time)]
        public Nullable<DateTime> WorkingHoursAfternoonTo { get; set; }
        
        [DisplayName("Hourly Wage")]
        public decimal HourlyWage { get; set; }

        public decimal TotalLabourHours { get; set; }
        [DisplayName("Standared Hour")]
        public decimal StandaredHours { get; set; }
        //[NotMapped]
        public string AdditionalWork { get; set; }
        [DisplayName("TotalLabour Cost")]
        public decimal TotalLabourCost { get; set; }

        [ForeignKey("MecanicId")]
        public virtual MechanicCrew MechanicCrew { get; set; }
    }
    public class CloseWorkShopWithRemark
    {
        public int CloseWorkShopWithRemarkId { get; set; }
       public string JobId { get; set; }
       public string JobOrderId { get; set; }
        public string ServiceName { get; set; }
        public string Remark { get; set; }
    }
    public enum WorkShopType
    {
        Internal_WorkShop,
        External_WorkShop
    }
}
