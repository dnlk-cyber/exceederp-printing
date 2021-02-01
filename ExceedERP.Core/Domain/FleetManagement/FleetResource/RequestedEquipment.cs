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
    public class RequestedEquipment : Operations
    {
        public Guid RequestedEquipmentId { get; set; }
        public Guid RequestNumber { get; set; }
        public Guid EquipmentID { get; set; }
        [Required]
        public int Quantity { get; set; }
        public string ManufacturedYear { get; set; }
        [Required]
        public string EquipmentName { get; set; }
            [DisplayName(" Equipment Name")]
        [Required]
        public string SubEquipmentName { get; set; }
        [Required]
        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? AssignedDate { get; set; }
        public DateTime? RequestedDate { get; set; }
        [Required]
        public string Type { get; set; }

        public string CategoryRef { get; set; }
        [DisplayName("Equipment Status")]
        public string EquipmentStatus { get; set; }
        public string RequestDesc { get; set; }

        public enum EquipmentStatusenum { New, Good, NeverMind }
        public bool IsAssigned { get; set; }

    }
}
