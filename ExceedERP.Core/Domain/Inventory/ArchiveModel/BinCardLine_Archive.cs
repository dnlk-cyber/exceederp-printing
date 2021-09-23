using ExceedERP.Core.Domain.Inventory.BaseModel;

namespace ExceedERP.Core.Domain.Inventory.ArchiveModel
{
    public class BinCardLineArchive: BinCardLineBase
    {
        public int BinCardLineArchiveId { get; set; }
        public int BinCardLineID { get; set; }
        public int StoreItemAssignmentID { get; set; }
    }
}
