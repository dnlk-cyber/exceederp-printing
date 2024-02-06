using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp
{
    public class FurnitureGraphic
    {
        public int FurnitureGraphicID { get; set; }
        public int FurnitureJobID { get; set; }

        [StringLength(50)]
        [Display(Name = ("FurnitureUserID"))]
        public string FurnitureUserID { get; set; }

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
