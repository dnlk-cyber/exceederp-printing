using ExceedERP.Core.Domain.Construction;
using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Production
{
    public static class ProductionReportsHelper
    {
        public static List<SeedProductionOrderItem> GetSortedValue(List<SeedProductionOrderItem> unsortedList)
        {
            var db = new ExceedDbContext();

            List<SeedProductionOrderItem> list = new List<SeedProductionOrderItem>();
            //var ids = new HashSet<int>(unsortedList.Select(l => l.ProductionP));
            //var periods = db.ConstructionPeriod.Where(p => (ids.Contains(p.ConstructionPeriodId))).OrderBy(p => p.DateTo).ToList();

            //foreach (ConstructionPeriod period in periods)
            //{
            //    var recordOfThisPeriod = unsortedList.Where(rec => rec.ConstructionPeriodId == period.ConstructionPeriodId).ToList();
            //    if (recordOfThisPeriod.Any())
            //    {
            //        list.AddRange(recordOfThisPeriod);
            //    }
            //}
            return list;
        }
    }
}
