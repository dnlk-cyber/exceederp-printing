
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public partial class TimeSheet : Operations
    {

        [ScaffoldColumn(false)]
        public Guid TimeSheetID { get; set; }
        [Required]
        [DisplayName("Equipment")]
        public Guid FleetsID { get; set; }

        public Guid FleetTypeID { get; set; }

        public Guid FleetCategoryID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> Date { get; set; }
        [DisplayName(" Rental Rate ")]
        public decimal RentalRate { get; set; }
        [DisplayName("Ideal Rental Rate ")]
        public decimal IdealRentalRate { get; set; }

        [DisplayName(" Type of Work ")]
        [Required]
        public string TypeofWork { get; set; }
        [DisplayName("Plate/Serial No")]
        public string PlateSerialNo { get; set; }
        public string Type { get; set; }
        [DisplayName(" Equipment Name")]
        public string EquipmentName { get; set; }
        [DisplayName(" Downs Reason ")]
        public string ReasonType { get; set; }
        [DisplayName("Intial KMHr Reading ")]
        [Required]
        public decimal KMHrReading { get; set; }
        [DisplayName("Final KMHr Reading ")]
        [Required]
        public decimal FinalKMHrReading { get; set; }
        [DisplayName("  Morning From")]
        [Required]
        [DataType(DataType.Time)]
        public Nullable<DateTime> WorkingHoursMorningFrom { get; set; }
        [DisplayName("  Morning To")]
        [Required]
        [DataType(DataType.Time)]
        public Nullable<DateTime> WorkingHoursMorningto { get; set; }
        [DisplayName("  Afternoon From ")]
        [Required]
        [DataType(DataType.Time)]
        public Nullable<DateTime> WorkingHoursAfternoonFrom { get; set; }
        [DisplayName("  Afternoon To ")]
        [Required]
        [DataType(DataType.Time)]
        public Nullable<DateTime> WorkingHoursAfternoonTo { get; set; }
        [DisplayName(" Idle Time ")]

        public int IdleTime { get; set; }
        [DisplayName(" Down Time ")]

        public int DownTime { get; set; }
        [DisplayName(" Break Down Time ")]

        public int BreakDownTime { get; set; }
        [DisplayName(" Location ")]
        [Required]
        public string CustomerName { get; set; }
        [DisplayName("Rent Type")]

        public RentType RentType { get; set; }

        [DisplayName("  Administrator Name")]

        public string EquipmentAdministratorName { get; set; }

        public bool IsExternal { get; set; }


    }
}