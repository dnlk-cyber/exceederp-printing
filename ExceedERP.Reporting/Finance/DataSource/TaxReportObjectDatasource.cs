using System;
using System.Collections.Generic;
using System.ComponentModel;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.ViewModel;

namespace ExceedERP.Reporting.Finance.DataSource
{
    [DataObject]
    public class TaxReportObjectDatasource : List<TaxReportVm>, IDisposable
    {
        private readonly ExceedDbContext _db;
        public TaxReportObjectDatasource()
        {
            _db = new ExceedDbContext();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TaxReportVm> GetTaxReport(string taxId)
        {
            List<TaxReportVm> taxReportVms = new List<TaxReportVm> { };
            TaxReportVm taxReportVm = new TaxReportVm();


            /*..Do somthing.*/

            taxReportVms.Add(taxReportVm);
            return taxReportVms;
        }

        public void Dispose()
        {
            _db?.Dispose();
        }
    }
   

}
