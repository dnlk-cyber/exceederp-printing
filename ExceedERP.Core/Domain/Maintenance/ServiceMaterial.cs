using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Maintenance
{
    class ServiceMaterial
    {

        public Guid ServiceSpareID { get; set; }
        [DisplayName("Service")]
        [Required]
        public Guid ServiceID { get; set; }
        public Guid SparePartID { get; set; }
        public string ServiceName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> Date { get; set; }
        [DisplayName("Unit Amount ")]
        public decimal UnitAmount { get; set; }
        public string Type { get; set; }

    }
}
