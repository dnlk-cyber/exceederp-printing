using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement
{
    public class InternalRentalRate:TrackUserSettingOperation
    {
        public int InternalRentalRateId { get; set; }
        [Required]
        public string CategoryId { get; set; }
        [Required]
        public string EquipmentTypeId { get; set; }
        [Required]
        public decimal Rate { get; set; }
        [Required]
        public decimal MarketPrice { get; set; }
        public RentType RentType { get; set; }
        public string Description { get; set; }
    }
}
