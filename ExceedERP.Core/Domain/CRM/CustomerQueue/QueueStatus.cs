using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.CRM
{
    public class QueueStatus : Operations
    {
        [Key]
        public int QueueStatusId { get; set; }
        [Display(Name = "Queue #")]
        public int QueueId { get; set; }
        public virtual Queue Queue { get; set; }

        [NotMapped]
        [Display(Name = "Queue #")]
        public int QueueNumber { get; set; }


        //
        // Customer Information
        [NotMapped]
        public string CustomerName { get; set; }

        [NotMapped]
        [Display(Name = "TIN #")]
        public string TinNumber { get; set; }

        [NotMapped]
        [Display(Name = "Phone #")]
        public string Phone { get; set; }

        [NotMapped]
        [Display(Name = "Investment/Project Type")]
        public string InvestmentType { get; set; }

        [NotMapped]
        [Display(Name = "Total Request")]
        public decimal TotalRequestedAmount { get; set; }

        [NotMapped]
        [Display(Name = "Weekly Request")]
        public decimal WeeklyRequest { get; set; }

        [NotMapped]
        [Display(Name = "Approved Quantity")]
        public decimal ApprovedQuantity { get; set; }

        [ExclusiveRange(0, ErrorMessage = "Shipped Quantity Must be greater than 0")]
        [Display(Name = "Shipped Quantity")]
        public decimal ShippedQuantity { get; set; }
        [Display(Name = "Remaining Quantity")]
        public decimal RemainingQuantity { get; set; }
        [Display(Name = "Queue Status")]
        public CustomerQueueStatus QueueStatusStatus { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Shipped Date")]
        public DateTime ShippedDate { get; set; }

        [NotMapped]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }

        [NotMapped]
        [Display(Name = "Location")]
        public int LocationMapId { get; set; }

        [NotMapped]
        public string Store { get; set; }

        public string Remark { get; set; }
    }

    public class QueueStatusValidation : VoucherValidation
    {
        [Key]
        public int QueueStatusValidationId { get; set; }
        public int QueueStatusId { get; set; }

        public virtual QueueStatus QueueStatus { get; set; }
    }
}
