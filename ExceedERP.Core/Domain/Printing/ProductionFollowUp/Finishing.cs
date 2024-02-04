using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing.ProductionFollowUp
{
    public class Finishing
    {
        public int FinishingId { get; set; }

        public int JobId { get; set; }
        //public virtual Job Job { get; set; }

        [StringLength(50)]
        [Display(Name = "Login CheckOut")]
        public string LoginCheckOut { get; set; }

        [Display(Name = "Check Out")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CheckOut { get; set; }

        [StringLength(50)]
        [Display(Name = "Login CheckIn")]
        public string LoginCheckIn { get; set; }

        [Display(Name = "Check In")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CheckIn { get; set; }
    }
    public class FinishingSubWorkFlow
    {
        public int FinishingSubWorkFlowId { get; set; }

        public int JobId { get; set; }
        //public virtual Job Job { get; set; }

        [StringLength(50)]
        [Display(Name = "SubWork flow")]
        public string SubWorkflow { get; set; }

        [Display(Name = "Quantity")]
        public int Qty { get; set; }
    }
    public class FinishedTransfer
    {
        public int FinishedTransferId { get; set; }

        public int JobID { get; set; }
        public virtual Job Job { get; set; }

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

        public int Quantity { get; set; }

        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }

        [StringLength(50)]
        public string Invoice { get; set; }
    }
}
