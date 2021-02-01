using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EvaluationPeriodsObjectDataSource : List<GLPeriod>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Quarter> GetEvaluationPeriods(string year)
        {
            List<Quarter> list = new List<Quarter> { };

            //var periods = from p in db.QuarterLengths
            //              join f in db.GLFiscalYears
            //                  on p.GlFiscalYearId equals f.GlFiscalYearId
            //              join q in db.Quarters
            //              on p.QuarterLengthId equals q.QuarterLengthId
            //              where f.Status == FiscalYearStatus.Open
            //              select (new { q });

            //if (!periods.Any())
            //{
            //    return list;
            //}

            var quarterLength = db.QuarterLengths.FirstOrDefault(x=>x.GlFiscalYearId.ToString() == year);

            if(quarterLength != null)
            {
                var quarters = db.Quarters.Where(x=>x.QuarterLengthId == quarterLength.QuarterLengthId).ToList();
                foreach(var q in quarters)
                {
                    Quarter quarter = new Quarter();
                    quarter.Name = q.Name;
                    quarter.QuarterId = q.QuarterId;
                    quarter.AmharicName = q.AmharicName;

                    list.Add(quarter);
                }
            }

            return list;// periods.Select(x=> x.q).ToList ();
        }
    }
}