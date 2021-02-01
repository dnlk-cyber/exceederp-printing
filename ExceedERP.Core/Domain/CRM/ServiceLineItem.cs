using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 using ExceedERP.Core.Domain.Common;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CRM
{
    public class ServiceLineItem : Operations
    {
        public int ServiceLineItemId { get; set; }

        public int ServiceUsageId { get; set; }
        [Required]
        public int CustomerServiceId { get; set; }
           [Required]
        public decimal UnitPrice { get; set; }
           [Required]
        public string Tax { get; set; }
           [Required]
        public decimal TotalAmount { get; set; }

        public virtual CustomerService CustomerService { get; set; }
    }
}
