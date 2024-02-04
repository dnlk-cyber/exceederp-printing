using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.printing.JobCosting
{
   public class JobOrderTimeRegistration
    {
        public int JobOrderTimeRegistrationId { get; set; }
        public int JobId { get; set; }
        public string Remark { get; set; }
    }
}
