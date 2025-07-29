using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Maintenance
{
    public class ExternalServiceCenterDetail
    {
        public int ExternalServiceCenterDetailId { get; set; }
        public string ServiceCenterJobId { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public string Remark { get; set; }
    }
}
