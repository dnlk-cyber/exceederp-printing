using System.Collections.Generic;
using System.ComponentModel;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.HRM.Payroll.DataSource
{
    [DataObject]
    public class SourceTypeObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetSources()
        {
            List<string> sourceList = new List<string>
            { 
                //"Deduction", 
                //"Long Term Deduction", 
                "Fixed Contribution",
                "Regular Contribution"
            };

            return sourceList;
        }
    }
}