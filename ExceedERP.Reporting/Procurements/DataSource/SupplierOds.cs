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
    public class SupplierOds : List<SupplierListViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SupplierListViewModel> GetSupplierList()
        {
            List<SupplierListViewModel> list = new List<SupplierListViewModel> { };

           var supplierlist = _db.ProcurementSuppliers.ToList();
           
            foreach (var item in supplierlist)
            {
                var record = new SupplierListViewModel
                {
                    SupplierName = item.Name,
                    ID = item.SupplierId,
                    Account = item.AccountCode,
                    TradeLicense = item.TradeLicense,
                    Address = item.Address,
                    IsCredit = item.IsCredit ? "Yes" : "No",
                    IsActive = item.IsActive ? "Yes" : "No"
                };
                list.Add(record);
                  
            }
            return list;
        }

       
    }
}
