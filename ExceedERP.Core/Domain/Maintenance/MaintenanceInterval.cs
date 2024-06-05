using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Maintenance
{
    public class MaintenanceInterval : TrackUserOperation
    {
        public MaintenanceInterval()
        {
            MaintenanceIntervalId = Guid.NewGuid();
        }
        [Key]
        public Guid MaintenanceIntervalId { get; set; }
        public MaintenanceIntervalType Type { get; set; }
        [Display(Name ="Value (km)")]
        public decimal Value { get; set; }
    }

    public enum MaintenanceIntervalType
    {
        RegularMaintenanceInterval = 0,
        NotifyUpComingMaintenanceBefore = 1
    }
}
