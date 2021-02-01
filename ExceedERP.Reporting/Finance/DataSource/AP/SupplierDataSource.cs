using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.ViewModel;

namespace ExceedERP.Reporting.Finance.DataSource.AP
{
    [DataObject]
    public class SupplierDataSource : List<SupplierListViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SupplierListViewModel> GetSuplierLists(bool isCredit, bool isActive)
        {
            List<SupplierListViewModel> list = new List<SupplierListViewModel> { };

            var supplierlist = _db.ProcurementSuppliers
                .Where(x=>x.IsActive==isActive)
                .Where(x => x.IsCredit == isCredit)
                .Select(x=>new{x.Name,x.SupplierId,x.AccountCode,x.TradeLicense,x.Address,x.IsActive,x.IsCredit})
                .ToList();

            foreach (var supplier in supplierlist)
            {
                var record = new SupplierListViewModel
                {
                    SupplierName = supplier.Name,
                    ID = supplier.SupplierId,
                    Account = supplier.AccountCode,
                    TradeLicense = supplier.TradeLicense,
                    Address = supplier.Address,
                    IsCredit = supplier.IsCredit ? "Yes" : "No",
                    IsActive = supplier.IsActive ? "Yes" : "No"
                };
                list.Add(record);
            }
            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetSupplierTypes()
        {
            List<string> list = new List<string> { };
            var types = _db.APSupplierTypes.Select(s => new
            {
                supplierSelector = s.Supplier
            }).OrderBy(x => x.supplierSelector);


            if (!types.Any())
            {
                return list;
            }

            return types.Select(x => x.supplierSelector).ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetBusinessTypes()
        {
            List<string> list = new List<string> { };
            var types = _db.APBusinessTypes.Select(s => new
            {
                businessTypeSelector = s.TypeOfBusiness
            }).OrderBy(x => x.businessTypeSelector);


            if (!types.Any())
            {
                return list;
            }

            return types.Select(x => x.businessTypeSelector).ToList();
        }
    }
}
