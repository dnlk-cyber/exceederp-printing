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
    public class BidPurchasingSuppliersOds
    {
       ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BidPurchasingSuppliers> GetTenders()
        {
            List<BidPurchasingSuppliers> purchasingSupplierses = db.BidPurchasingSupplierses.ToList();
            return purchasingSupplierses; 
        }
        
    }
}
