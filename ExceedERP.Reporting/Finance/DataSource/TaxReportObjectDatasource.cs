using System.Collections.Generic;
using System.ComponentModel;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.ViewModel;

namespace ExceedERP.Reporting.Finance.DataSource
{
    [DataObject]
    public class TaxReportObjectDatasource : List<TaxReportVm>
    {
        
            private ExceedDbContext _db = new ExceedDbContext();

            [DataObjectMethod(DataObjectMethodType.Select)]
            public List<TaxReportVm> GetTaxReport(string taxId)
            {
                List<TaxReportVm> taxReportVms = new List<TaxReportVm> { };
                TaxReportVm taxReportVm = new TaxReportVm();

               
                /*..Do somthing.*/

                taxReportVms.Add(taxReportVm);
                return taxReportVms;
            }
        }
    
}
