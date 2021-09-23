using ExceedERP.Core.Domain.Inventory;

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
