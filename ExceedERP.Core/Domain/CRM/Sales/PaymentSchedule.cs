using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CRM
{
    public class PaymentSchedule : TrackUserOperation
    {
        [Key]
        public int PaymentScheduleId { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public string Description { get; set; }
        public decimal Amount { get; set; }

        public string Remark { get; set; }

        [Display(Name ="Voucher")]
        public int VoucherId { get; set; }
        public VoucherType VoucherType { get; set; }
    }
}
