using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.CRM
{
    public class ServiceUsage : Operations
    {
        [Key]
        public int ServiceUsageId { get; set; }
        [Required]
        public string Customer { get; set; }
        public string PaymentMethod { get; set; }
                [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> Date { get; set; }
                [Required]
        public string ReceiveMethod { get; set; }

        [NotMapped]
        public decimal GrandTotal { get; set; }

      
        public virtual OrganizationCustomer OrganizationCustomer { get; set; }
    }
}
