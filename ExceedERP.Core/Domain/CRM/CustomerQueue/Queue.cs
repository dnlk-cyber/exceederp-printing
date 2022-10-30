using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.CRM
{
    public class Queue : Operations
    {
        [Key]
        public int QueueId { get; set; }

        [Display(Name = "Queue #")]
        public int QueueNumber { get; set; }

        [Display(Name = "Customer")]
        public int QueueCustomerId { get; set; }
        public virtual QueueCustomer QueueCustomer { get; set; }

        [Display(Name = "Queue Type")]
        public int QueueTypeId { get; set; }
        public virtual QueueType QueueType { get; set; }

        //
        // Customer Information
        [NotMapped]
        [Display(Name ="Customer Name")]
        public string CustomerName { get; set; }

        [NotMapped]
        [Display(Name = "TIN #")]
        public string TinNumber { get; set; }

        [NotMapped]
        [Display(Name = "Phone #")]
        public string Phone { get; set; }

        [NotMapped]
        [Display(Name = "Investment Type")]
        public string InvestmentType { get; set; }

        [NotMapped]
        [Display(Name = "Investment/Project amount")]
        public decimal InvestmentAmount { get; set; }

        [NotMapped]
        [Display(Name = "Grade")]
        public string Grade { get; set; }
        //
        [Required]
        [ExclusiveRange(0, ErrorMessage ="Total Request Quantity Must be greater than 0")]
        [Display(Name = "Total Request")]
        public decimal TotalRequestedAmount { get; set; }

        [Display(Name = "Weekely Request")]
        public decimal WeeklyRequest { get; set; }
        [Display(Name = "Approved Quantity")]
        public decimal ApprovedQuantity { get; set; }
        [Display(Name ="Branch")]
        public int BranchId { get; set; }
        [Display(Name ="Location")]
        public int LocationMapId { get; set; }
        public string Store { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Queue Date")]
        public DateTime QueueDate { get; set; }

        [Display(Name ="Queue Status")]
        public CustomerQueueStatus QueueStatus { get; set; }

        [Display(Name = "Advance Payment")]
        public decimal AdvancePayment { get; set; }

        public string Remark { get; set; }

    }

    public class QueueValidation : VoucherValidation
    {
        [Key]
        public int QueueValidationId { get; set; }
        public int QueueId { get; set; }

        public virtual Queue Queue { get; set; }
        //public decimal RequestedQuantity { get; set; }

        public decimal ApprovedQuantity { get; set; }
    }
}
