using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
   public class QuarterLengthODS:IDisposable
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<QuarterLengthVM> GetQuarterLengths()
        {
            List<QuarterLengthVM> quarLengthList = new List<QuarterLengthVM>();

            var quarterLengths = db.QuarterLengths.OrderByDescending(x=>x.GlFiscalYearId).ToList();

            foreach(var quart in quarterLengths)
            {
                var len = new QuarterLengthVM();

                len.QuarterLengthId = quart.QuarterLengthId;
                len.GLFiscalYearId = quart.GlFiscalYearId;

                var fiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == quart.GlFiscalYearId);
                if(fiscalYear != null)
                {
                    len.Name = fiscalYear.Name;
                }

                quarLengthList.Add(len);
            }

            return quarLengthList.OrderByDescending(x=>x.GLFiscalYearId).ToList();
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }    

}
