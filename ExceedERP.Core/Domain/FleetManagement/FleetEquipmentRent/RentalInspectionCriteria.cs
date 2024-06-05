using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent
{
    public class RentalInspectionCriteria: TrackUserSettingOperation
    {
        public int RentalInspectionCriteriaId { get; set; }
        public decimal Index { get; set; }
        public string CriteriaCategory { get; set; }
        
    }
    public class RentalInspectionCriteriaDetail : TrackUserSettingOperation
    {
        public int RentalInspectionCriteriaDetailId { get; set; }
        public decimal Index { get; set; }
        public string CriteriaName { get; set; }
        [NotMapped]
        [Display(Name="Index")]
        public string SerialNumber { get; set; }
        public int RentalInspectionCriteriaId { get; set; }
        public RentalInspectionCriteria InspectionCriteria { get; set; }



    }
}
