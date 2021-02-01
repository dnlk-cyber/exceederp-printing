using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.APTS
{
    public class LaboratoryRequestItem
    {
        public int LaboratoryRequestItemId { get; set; }
        public int LaboratoryRequestId { get; set; }
        public string Description { get; set; }
        public decimal QtyRequested { get; set; }
        public decimal QtyApproved { get; set; }
        public decimal QtyAllowed { get; set; }
    }
}
