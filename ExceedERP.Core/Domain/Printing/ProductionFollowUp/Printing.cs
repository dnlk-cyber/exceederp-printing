using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing.ProductionFollowUp
{
    public class Printing
    {
        public int PrintingID { get; set; }
        public int JobID { get; set; }
        //public virtual Job Job { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name="Login Check Out")]
        public string LoginCheckOut { get; set; }

        [Required]
        [Display(Name = "CheckOut")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CheckOut { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Login CheckIn")]
        public string LoginCheckIn { get; set; }

        [Required]
        [Display(Name = "CheckIn")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CheckIn { get; set; }
    }
    public class PrintedTransfer
    {
        public int PrintedTransferID { get; set; }

        public int JobID { get; set; }
        public virtual Job Job { get; set; }

        [StringLength(50)]
        [Display(Name=("UserID"))]
        public string UserID { get; set; }

        [Display(Name = ("Time"))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Time { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = ("Date"))]
        public DateTime Date { get; set; }
        public int Quantity { get; set; }

        [Display(Name = ("Total Price"))]
        public decimal TotalPrice { get; set; }
    }
}
