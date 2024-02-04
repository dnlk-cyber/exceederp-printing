using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing.ProductionFollowUp
{
    public class Delivery
    {
        public int DeliveryID { get; set; }
        public int JobID { get; set; }
        //public virtual Job Job { get; set; }

        [StringLength(50)]
        [Required]
        [Display(Name = "Login CheckOut")]
        public string LoginCheckOut { get; set; }

        [Required]
        [Display(Name = "CheckOut")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CheckOut { get; set; }

        [StringLength(50)]
        [Required]
        [Display(Name = "Login CheckIn")]
        public string LoginCheckIn { get; set; }

        [Required]
        [Display(Name="CheckIn")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CheckIn { get; set; }
    }
    public class DeliveryTransfer
    {
        public int DeliveryTransferID { get; set; }

        public int JobID { get; set; }
        public virtual Job Job { get; set; }
        
        [StringLength(50)]
        
        public string Userid { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Time { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Display(Name="Total Price")]
        public decimal TotalPrice { get; set; }

        [StringLength(50)]
        [Required]
        [Display(Name = "Sales Voucher No")]
        public string SalesVoucherNo { get; set; }

        [Required]
        public decimal Cash { get; set; }

        [Required]
        public decimal Credit { get; set; }

        [StringLength(50)]
        [Required]
        [Display(Name = "Cash Credit Invoice")]
        public string CashCreditInvoice { get; set; }
    }
}
