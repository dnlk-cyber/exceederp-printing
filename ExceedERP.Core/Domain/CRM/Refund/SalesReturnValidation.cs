using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CRM
{
    public class SalesReturnValidation : VoucherValidation
    {
        [Key]
        public int SalesReturnValidationId { get; set; }

        public virtual SalesReturn SalesReturn { get; set; }
        public int SalesReturnId { get; set; }

        
    }
}
