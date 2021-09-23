using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.ProductionFollowUp
{
    public class Graphic
    {
        public int GraphicID { get; set; }
        public int JobID { get; set; }

        [StringLength(50)]
        [Display(Name = ("UserID"))]
        public string UserID { get; set; }

        [Display(Name = ("Time"))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Time { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = ("Date"))]
        public DateTime Date { get; set; }
    }
}
