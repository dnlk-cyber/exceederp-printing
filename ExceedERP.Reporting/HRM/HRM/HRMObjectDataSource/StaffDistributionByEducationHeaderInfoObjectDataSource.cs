using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class StaffDistributionByEducationHeaderInfoObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IEnumerable<StaffDistributionByEducationHeaderInfoVM> GetHeaderInfo(string glFiscalYearId, object[] glPeriodId, DateTime? fromDate, DateTime? toDate)
        {
            int.TryParse(glFiscalYearId, out int fiscalYearId);

            HashSet<int> periodIds = new HashSet<int>();
            if (glPeriodId != null && glPeriodId.Length > 0)
            {
                foreach (var p in glPeriodId)
                {
                    if (p != null && int.TryParse(p.ToString(), out int pId))
                    {
                        periodIds.Add(pId);
                    }
                }
            }

            var periodsQuery = db.GLPeriods.Where(x => x.GlFiscalYearId == fiscalYearId);
            if (periodIds.Count > 0)
            {
                periodsQuery = periodsQuery.Where(x => periodIds.Contains(x.GLPeriodId));
            }
            var periods = periodsQuery.OrderBy(x => x.Index).ToList();

            var fiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == fiscalYearId);

            string periodLabel;
            int monthsSpanned;
            if (fromDate.HasValue || toDate.HasValue)
            {
                // An explicit date range narrows/overrides the selected period(s)
                DateTime rangeFrom = fromDate ?? (periods.Any() ? periods.Min(x => x.DateFrom) : DateTime.Today);
                DateTime rangeTo = toDate ?? (periods.Any() ? periods.Max(x => x.DateTo) : DateTime.Today);
                periodLabel = $"{rangeFrom:dd/MM/yyyy} - {rangeTo:dd/MM/yyyy}";
                monthsSpanned = MonthsBetween(rangeFrom, rangeTo);
            }
            else if (periods.Any())
            {
                periodLabel = string.Join("+", periods.Select(x => x.Name));
                monthsSpanned = periods.Count;
            }
            else
            {
                periodLabel = DateTime.Today.ToString("dd/MM/yyyy");
                monthsSpanned = 1;
            }

            return new List<StaffDistributionByEducationHeaderInfoVM>
            {
                new StaffDistributionByEducationHeaderInfoVM
                {
                    FiscalYearName = fiscalYear?.Name,
                    PeriodLabel = periodLabel,
                    ReportTitle = ClassifyReportTitle(monthsSpanned)
                }
            };
        }

        private static int MonthsBetween(DateTime from, DateTime to)
        {
            return ((to.Year - from.Year) * 12) + to.Month - from.Month + 1;
        }

        // Names the report based on how many months it covers, matching the Monthly/Quarterly/Semi-Annual/Annual templates
        private static string ClassifyReportTitle(int monthsSpanned)
        {
            if (monthsSpanned <= 1)
            {
                return "ወርሃዊ ሪፖርት";
            }
            if (monthsSpanned <= 3)
            {
                return "የሩብ ዓመት ሪፖርት";
            }
            if (monthsSpanned <= 6)
            {
                return "የስድስት ወር ሪፖርት";
            }
            if (monthsSpanned <= 12)
            {
                return "ዓመታዊ ሪፖርት";
            }
            return "ወቅታዊ ሪፖርት";
        }
    }
}
