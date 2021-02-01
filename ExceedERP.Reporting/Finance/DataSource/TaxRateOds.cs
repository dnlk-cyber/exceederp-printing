using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Finance.GL;
//using ExceedERP.Core.Domain.Finance.Report;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.ViewModel;

namespace ExceedERP.Reporting.Finance.DataSource
{
    [DataObject]
    public class TaxRateOds : List<TaxRateVm>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TaxRateVm> GetTaxRate()
        {
            List<TaxRateVm> list = new List<TaxRateVm> { };
            var taxRateVmlist = _db.GLTaxRates.Select(x=>new{x.GLTaxRateID,x.TaxName}).ToList();
            foreach (var item in taxRateVmlist)
            {
                var record = new TaxRateVm
                {

                    GLTaxRateID = item.GLTaxRateID,
                    TaxName = item.TaxName,
                    
                };
                list.Add(record);
            }
            return list;
        }
        
    }
}
