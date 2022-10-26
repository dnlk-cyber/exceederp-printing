using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CRM
{
    public class EquipmentInformation : TrackUserOperation
    {
        [Key]
        [ScaffoldColumn(false)]
        public Guid EquipmentInformationId { get; set; }

        [Display(Name = "Vehicle Type")]
        public string GroupName { get; set; }
        [Display(Name = "Plate Number")]
        public string LicensePlate { get; set; }
        [Display(Name = "Vehicle Model")]
        public string DeviceNumber { get; set; }
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        //[Display(Name ="Mileage(km)")]
        //public decimal Mileage { get; set; }
        public decimal InitialOdometer { get; set; }
        public decimal OdometerOnLastMaintenance { get; set; }
        public decimal CurrentOdometer { get; set; }

        [Display(Name ="Regular Maintenance Interval (km)")]
        public decimal RegularMaintenanceInterval { get; set; }
        [Display(Name = "Notify UpComing Maintenance Before (km)")]
        public decimal NotifyUpComingMaintenanceBefore { get; set; }

        public decimal OdometerOnNextRegularMaintenance { get; set; }
        public decimal RemainingKmForUpComingMaintenance { get; set; }

        // for notifying a maintenance in advance.
        public FutureMaintenanceAlert FutureMaintenanceAlert { get; set; }
        public string Status { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime? LastMaintained { get; set; }
    }
    public enum FutureMaintenanceAlert
    {
        Off = 0,
        On = 1
    }
    public class EquipmentMaintenanceAlertViewModel
    {
        public string Attribute { get; set; }
        public string Value { get; set; }
    }
    public class EquipmentMileage : TrackUserOperation
    {
        [Key]
        [ScaffoldColumn(false)]
        public Guid EquipmentMileageId { get; set; }

        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Group Name")]
        public string GroupName { get; set; }
        [Display(Name = "License Plate")]
        public string LicensePlate { get; set; }
        [Display(Name = "Device Number")]
        public string DeviceNumber { get; set; }
        [Display(Name = "Mileage(km)")]
        public decimal Mileage { get; set; }
    }
}
