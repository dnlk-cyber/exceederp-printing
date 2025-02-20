using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.InfoDesk.Support
{
   public class MaterialsUsed
    {
        public int MaterialsUsedId { get; set; }
        public int AssignedTicketId { get; set; }
        public int CustomerTicketId { get; set; }
        public string Remark { get; set; }
        public double Quantity { get; set; }
        public int MaterialItemsId { get; set; }
        public virtual MaterialItems MaterialItem { get; set; }

    }
}
