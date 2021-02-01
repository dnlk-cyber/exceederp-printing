
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
   
    [DataObject]
    public class PeriodDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public IEnumerable<GlPeriodVm> GetPeriods()
        {
           return db.GLPeriods.Select(x=> new GlPeriodVm
               {
                    GLPeriodId= x.GLPeriodId,
                    GlFiscalYearId = x.GlFiscalYearId,
                    Name =x.Name,DateFrom=x.DateFrom,
                    DateTo=x.DateTo
               })
                .ToList();
           
        }

    }
    public class GlPeriodVm 
    {
        public int GLPeriodId { get; set; }
        public int GlFiscalYearId { get; set; }
        public string Name { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }

}
