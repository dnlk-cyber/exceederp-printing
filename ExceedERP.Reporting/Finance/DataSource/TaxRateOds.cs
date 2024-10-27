using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
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
    public class TaxRateOds : List<TaxRateVm>, IDisposable
    {
        private readonly ExceedDbContext _db;
        public TaxRateOds()
        {
            _db = new ExceedDbContext();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TaxRateVm> GetTaxRate()
        {
            return _db.GLTaxRates
                                 .Select(x => new TaxRateVm
                                 {
                                     GLTaxRateID = x.GLTaxRateID,
                                     TaxName = x.TaxName
                                 })
                                 .AsNoTracking() 
                                 .ToList(); 
        }

        public void Dispose()
        {
            _db?.Dispose();
        }

    }
}
