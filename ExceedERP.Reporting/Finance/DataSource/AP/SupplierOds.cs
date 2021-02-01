using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
//using ExceedERP.Core.Domain.Finance.Report;
using SupplierListViewModel = ExceedERP.Reporting.Finance.ViewModel.SupplierListViewModel;

namespace ExceedERP.Reporting.Finance.DataSource.AP
{
    [DataObject]
    public class SupplierOds : List<SupplierListViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SupplierListViewModel> GetSupplierList()
        {
            List<SupplierListViewModel> list = new List<SupplierListViewModel> { };

           var supplierlist = _db.ProcurementSuppliers
               .Select(x => new { x.Name, x.SupplierId, x.AccountCode, x.TradeLicense, x.Address, x.IsActive, x.IsCredit })
                .ToList();
           
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
