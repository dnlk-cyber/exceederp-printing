using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.JobCosting
{
    public class Unit
    {
        public int UnitID { get; set; }
        public string RecordedBy { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RecordTime { get; set; }
        [Required]
        [Display(Name = "Unit of Measurement")]
        public string UnitName { get; set; }
    }
}
