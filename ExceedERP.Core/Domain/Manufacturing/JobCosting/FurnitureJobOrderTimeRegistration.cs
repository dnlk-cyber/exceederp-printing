using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.JobCosting
{
   public class FurnitureJobOrderTimeRegistration
    {
        public int FurnitureJobOrderTimeRegistrationId { get; set; }
        public int FurnitureJobId { get; set; }
        public string Remark { get; set; }
    }
}
