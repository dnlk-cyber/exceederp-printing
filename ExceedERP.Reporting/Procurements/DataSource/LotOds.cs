using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.Procurements.DataSource
{
    [DataObject]
    public class TenderOds
    {
       ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Tender> GetTenders()
        {
            List<Tender> tenders = db.Tenders.ToList();
            return tenders; 
        }
        
    }
}
