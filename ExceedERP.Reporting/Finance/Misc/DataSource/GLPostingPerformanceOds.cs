using ExceedERP.DataAccess.Context;

using ExceedERP.Reporting.Finance.Misc.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.Finance.Misc.DataSource
{
    [DataObject]
    public class GlPostingPerformanceOds
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public IOrderedEnumerable<GlPostingPerformanceViewModel> GetGlPostingPerformance( string periodFrom, string periodTo)
        {
            List<GlPostingPerformanceViewModel> list = new List<GlPostingPerformanceViewModel>();

            if (!string.IsNullOrWhiteSpace(periodFrom)&& !string.IsNullOrWhiteSpace(periodTo))
            {
                
                int from = int.Parse(periodFrom);
                int to = int.Parse(periodTo);

                var fromObj = db.GLPeriods.Find(from);
                var toObj = db.GLPeriods.Find(to);

                var periods = db.GLPeriods.Where(x => x.DateFrom >= fromObj.DateFrom)
                    .Where(x => x.DateTo <= toObj.DateTo).Select(x => x.Name).ToList();

                var glrecordJournal = db.GLRecordJournals.Where(x=>x.IsOnlineApproved&&x.isPosted&&!x.isVoid).Where(x => periods.Contains(x.PeriodName))
                    .Select(x=>new {x.GlRecordJournalId,x.PostedBy}).ToList();
                var distinctUsers = glrecordJournal.Select(x => x.PostedBy).Distinct();

                foreach (var user in distinctUsers)
                {
                    var performance = new GlPostingPerformanceViewModel
                    {
                        User = user,
                        FullName = "",
                        Count=glrecordJournal.Count(x => x.PostedBy==user),
                        Remark = ""
                    };

                    list.Add(performance);

                }

            }

            return list.OrderBy(x=>x.Count);
        }
    }
}
