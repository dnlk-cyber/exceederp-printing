using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Planning.DataSource
{
    [DataObject]
    public class FiscalYearODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GlFiscalYear> GetFiscalYear()
        {

            return db.GLFiscalYears.ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FiscalYearValue> GetFiscalYearValues()
        {
            return db.GLFiscalYears
                .OrderByDescending(x => x.DateFrom)
                .Select(x => new FiscalYearValue
                {
                    FiscalYear = x.DateFrom.Year,
                    Name = x.Name
                })
                .ToList();
        }
    }

    public class FiscalYearValue
    {
        public int FiscalYear { get; set; }
        public string Name { get; set; }
    }
}
