using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public class EquipmentArrival : Operations
    {

        public Guid EquipmentArrivalId { get; set; }
        public Guid AssignmentId { get; set; }
        [Display(Name ="Equipment Category")]
        public Guid EquipmentIdentityId { get; set; }
        [Display(Name ="Equipment Type")]
        public Guid EquipmentTypeId { get; set; }
        [Display(Name ="Equipment Name")]
        public Guid EquipmentNameId { get; set; }

        [DisplayName("Plate No")]
        public Guid FleetsId { get; set; }
        [DisplayName("Equipment Type")]
        public string EquipmentType { get; set; }
        [DisplayName("Arrival Place")]
        public string ArrivalPlace { get; set; }
        public string Model { get; set; }
        [DisplayName("Asset No")]
        public string DCENo { get; set; }
        public string OwnedBy { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Arrival Date")]
        public DateTime Date { get; set; }
        [DisplayName("KMHR Reading")]
        public decimal KMHRReading { get; set; }
        [DisplayName("Means Of Travelling")]
        [Required]
        public MeansOfTravelling MeansOfTravelling { get; set; }
        [DisplayName("Operator")]
        public string OperatorName { get; set; }

    }

    public enum MeansOfTravelling
    {
        [Display(Name = "By ItSelf"), Description("By ItSelf")]
        ByItself,
        [Display(Name = "Other Equipment"), Description("Other Equipment")]
        OtherEquipment
    }
}
