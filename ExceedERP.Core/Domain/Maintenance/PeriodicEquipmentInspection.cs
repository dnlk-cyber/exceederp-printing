using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Maintenance
{
    public  class PeriodicEquipmentInspection : TrackUserOperation
    {
        public Guid PeriodicEquipmentInspectionId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public string Owner { get; set; }
        public string PlateNumber { get; set; }
        public string VehicleType { get; set; }
        [Display(Name="Capacity Kg")]
        public decimal Capacity { get; set; }
        public decimal AmountPaid { get; set; }
        public string Remark { get; set; }
    }
}
