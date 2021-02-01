using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.FleetResource
{
    public class DriverAssignment : Operations
    {
        public Guid DriverAssignmentId { get; set; }
        [Required]
        [DisplayName("Equipment ")]
        public Guid FleetId { get; set; }
        [Required]
        [DisplayName("Equipment Name")]
        public string EquipmentName { get; set; }
        [DisplayName(" From Operator")]
        public string FromOperatorName { get; set; }
        [Required]
        [DisplayName("To Operator")]
        public string ToOperatorName { get; set; }
        [DisplayName("Starting Date")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> StartingDate { get; set; }
        [DisplayName("To Date")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> ToDate { get; set; }

        public decimal Odometer { get; set; }
    }
}
