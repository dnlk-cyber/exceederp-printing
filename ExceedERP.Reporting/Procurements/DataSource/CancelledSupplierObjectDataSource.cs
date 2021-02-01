using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Procurements.ViewModel;

namespace ExceedERP.Reporting.Procurements.DataSource
{

    [DataObject]
    public class CancelledSupplierObjectDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SupplierListViewModel> GetParticipants(string id)
        {
            List<SupplierListViewModel> supplierList = new List<SupplierListViewModel>();
            SupplierListViewModel supplier = new SupplierListViewModel();
         
            if (id != null)
            {
                var lots = db.Lots.FirstOrDefault(x=>x.TenderID.ToString() == id);
                if(lots!=null)
                {
                    var participatingSuppliers = db.BidPParticipatingSupplierses.Where(x=>x.LotID==lots.LotID).ToList();
                    foreach(var participatingSupplier in participatingSuppliers)
                    {
                        supplier = new SupplierListViewModel();

                        supplier.ID = participatingSupplier.SupplierId;
                        supplier.SupplierName = participatingSupplier.SupplierName;

                        supplierList.Add(supplier);
                    }
                }
            }          


            return supplierList;
        }
    }
}
