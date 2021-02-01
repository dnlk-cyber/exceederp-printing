using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.Procurements.DataSource
{
    [DataObject]
    public class LotOds
    {
       ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Lot> GetLots()
        {
            List<Lot> getLots = db.Lots.ToList();
            return getLots; 
        }
        
    }
}
