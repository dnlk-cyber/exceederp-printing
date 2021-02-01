using ExceedERP.Core.Domain.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.APTS
{
   public class DrugUnit
    {
        public int DrugUnitId { get; set; }
        public string UnitName { get; set; }
        public string UoMName { get; set; }
        public UoM UoM { get; set; }
        public string Remark { get; set; }
    }
}
