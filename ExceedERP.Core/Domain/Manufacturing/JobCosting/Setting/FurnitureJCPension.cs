using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.JobCosting.Setting
{
    public class FurnitureJcPension
    {
        public int FurnitureJcPensionId { get; set; }
        public decimal Rate { get; set; }
        public string Description { get; set; }
    }
}
