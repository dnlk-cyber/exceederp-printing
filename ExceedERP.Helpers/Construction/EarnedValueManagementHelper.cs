using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Construction
{
   public static class EarnedValueManagementHelper
    {
        public static List<EarnedValuePerProject> GetSortedValue(List<EarnedValuePerProject> unsortedList) {
            var db = new ExceedDbContext();

            List<EarnedValuePerProject> list = new List<EarnedValuePerProject>();
            var ids = new HashSet<int>(unsortedList.Select(l => l.ConstructionPeriodId));
            var periods = db.ConstructionPeriod.Where(p => (ids.Contains(p.ConstructionPeriodId))).OrderBy(p => p.DateTo).ToList();

            foreach(ConstructionPeriod period in periods)
            {
                var recordOfThisPeriod = unsortedList.Where(rec => rec.ConstructionPeriodId == period.ConstructionPeriodId).ToList();
                if (recordOfThisPeriod.Any())
                {
                    list.AddRange(recordOfThisPeriod);
                }
            }
            return list;
        }
    }
}
