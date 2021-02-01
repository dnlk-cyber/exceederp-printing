using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
   
    [DataObject]
    public class FiscalYearDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GlFiscalYearVm> GetFiscalYearss()
        {
            List<GlFiscalYearVm> fiscalYears = db.GLFiscalYears
                .OrderByDescending(x=>x.DateFrom)
                .Select(x => new GlFiscalYearVm
                {
                   
                    GlFiscalYearId = x.GlFiscalYearId,
                    Name = x.Name,
                    DateFrom = x.DateFrom,
                    DateTo = x.DateTo
                })
                .ToList();
            return fiscalYears;
        }

    }

    public class GlFiscalYearVm
    {
        public int GlFiscalYearId { get; set; }
        public string Name { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
