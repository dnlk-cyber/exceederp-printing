using ExceedERP.Core.Domain.Inventory.BaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Inventory.ArchiveModel
{
    public class BinCardLineArchive: BinCardLineBase
    {
        public int BinCardLineArchiveId { get; set; }
        public int BinCardLineID { get; set; }
        public int StoreItemAssignmentID { get; set; }
    }
}
