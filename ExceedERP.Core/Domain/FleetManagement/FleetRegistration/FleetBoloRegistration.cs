using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public class FleetBoloRegistration : TrackUserSettingOperation
    {
        [Key]
        public int FleetBoloRegistrationId { get; set; }
        [Display(Name ="Plate Number")]
        public Guid FleetId { get; set; }
        [DataType(DataType.Date), Display(Name ="Bolo Registration Date")]
        public DateTime BoloRegistrationDate { get; set; }
        [DataType(DataType.Date), Display(Name = "Bolo Expiration Date")]
        public DateTime BoloExpireDate { get; set; }
        public string Remark { get; set; }


    }
}
