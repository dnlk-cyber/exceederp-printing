
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetResource
{
    public partial class VehicleReturn : Operations
    {
        public Guid VehicleReturnID { get; set; }
        public Guid FleetsID { get; set; }
        [Required]
        public Nullable<DateTime> ReturnDate { get; set; }
        [Required]
        public string TotalTimeElapsed { get; set; }
        [Required]
        public string ReturnedBy { get; set; }
        public string ReturnStatus { get; set; }
        public enum ReasonforReturnenum { WorkisExecuted, InconvenienceforWork, EquipmentUnderMaintenance }
        [Required]
        public string ReasonforReturn { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }


        public virtual Fleets Fleet { get; set; }
    }
}