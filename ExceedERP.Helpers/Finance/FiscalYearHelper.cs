using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Helpers.Finance
{
    public  class FiscalYearHelper
    {


        public void UpdatePeriodIndex(int id)
        {
            ExceedDbContext Db = new ExceedDbContext();
            var periods = Db.GLPeriods.Where(x => x.GlFiscalYearId == id).OrderBy(x => x.DateFrom).ToArray();
            for (int i = 0; i < periods.Length; i++)
            {
                periods[i].Index = i + 1;
                Db.Entry(periods[i]).State = EntityState.Modified;
            }
            Db.SaveChanges();

        }

        public  string GetCurrentFYearName()
        { 
           ExceedDbContext Db = new ExceedDbContext();

        string currentYear = "";
            DateTime today = DateTime.Today;
            var fyear = Db.GLFiscalYears.Where(x => x.DateFrom <= today)
                                        .Where(x => x.DateTo >= today).ToList();
            if (fyear.Any())
            {
                var myYear = fyear.FirstOrDefault();
                if (myYear != null) currentYear = myYear.Name;
            }

            return currentYear;
        }
        public int GetCurrentFYearId()
        {
            ExceedDbContext Db = new ExceedDbContext();
            int currentYear = 0;
            DateTime today = DateTime.Today;
            var fyear = Db.GLFiscalYears.Where(x => x.DateFrom <= today)
                .Where(x => x.DateTo >= today).ToList();
            if (fyear.Any())
            {
                var myYear = fyear.FirstOrDefault();
                if (myYear != null) currentYear = myYear.GlFiscalYearId;
            }

            return currentYear;
        }

        public  List<GLPeriod> GetRecentNPeriodsInYear(int n)
        {
            ExceedDbContext Db = new ExceedDbContext();
            List<GLPeriod> listOfNPeriod = new List<GLPeriod>();
            DateTime today = DateTime.Today;
            var glPeriods = Db.GLPeriods.Where(x => x.DateFrom <= today).OrderByDescending(x=>x.DateTo).Take(n).ToList();

            if (glPeriods.Any())
            {
                listOfNPeriod.AddRange(glPeriods);
            }

            return listOfNPeriod;
        }

        public  async Task<List<GLPeriod>> GetRecentNPeriodsInYearAsync1(int n)
        {

            ExceedDbContext db = new ExceedDbContext();

            List<GLPeriod> listOfNPeriod = new List<GLPeriod>();
            DateTime today = DateTime.Today;
            var glPeriods = await db.GLPeriods.Where(x => x.DateFrom <= today).OrderBy(x => x.DateFrom).Take(n).ToListAsync();

            if (glPeriods.Any())
            {
                listOfNPeriod.AddRange(glPeriods);
            }
            return listOfNPeriod;
        }

        public async Task<List<GLPeriod>> GetRecentNPeriodsInYearAsync(int n)
        {
            ExceedDbContext db = new ExceedDbContext();
            int currentYearId = GetCurrentFYearId();

            List<GLPeriod> listOfNPeriod = new List<GLPeriod>();
            var glPeriods = await db.GLPeriods.Where(x => x.GlFiscalYearId ==currentYearId).OrderBy(x => x.DateFrom).ToListAsync();

            if (glPeriods.Any())
            {
                listOfNPeriod.AddRange(glPeriods);
            }
            return listOfNPeriod;
        }

        public  string GetParentFiscalYear(string period)
        {
            ExceedDbContext Db = new ExceedDbContext();
            string fyear = string.Empty;
            var periods = Db.GLPeriods.FirstOrDefault(x => x.Name == period);
            if (periods != null)
            {
                fyear = periods.GlFiscalYear.Name.ToString();
            }

            return fyear;
        }

        public DateTime GetQuarterStart(int yearid, int quarter)
        {
            ExceedDbContext Db = new ExceedDbContext();
            DateTime date = new DateTime();
            var periods = Db.GLPeriods.Where(x => x.GlFiscalYearId == yearid).ToList();
            switch (quarter)
            {
                case 1:
                    var p = periods.FirstOrDefault(x => x.Index == 1);
                    if (p != null)
                        date = p.DateFrom;
                    break;
                case 2:
                    var p2 = periods.FirstOrDefault(x => x.Index == 4);
                    if (p2 != null)
                        date = p2.DateFrom;
                    break;
                case 3:
                    var p3 = periods.FirstOrDefault(x => x.Index == 7);
                    if (p3 != null)
                        date = p3.DateFrom;
                    break;
                case 4:
                    var p4 = periods.FirstOrDefault(x => x.Index == 10);
                    if (p4 != null)
                        date = p4.DateFrom;
                    break;
            }

            return date;
        }
        public DateTime GetQuarterEnd(int yearid,int quarter)
        {
            ExceedDbContext Db = new ExceedDbContext();
            DateTime date=new DateTime();
            var periods = Db.GLPeriods.Where(x => x.GlFiscalYearId == yearid).ToList();
            switch (quarter)
            {
                case 1:
                    var p = periods.FirstOrDefault(x => x.Index == 3);
                    if (p!=null)
                        date = p.DateTo;
                    break;
                case 2:
                    var p2 = periods.FirstOrDefault(x => x.Index == 6);
                    if (p2 != null)
                        date = p2.DateTo;
                    break;
                case 3:
                    var p3 = periods.FirstOrDefault(x => x.Index == 9);
                    if (p3 != null)
                        date = p3.DateTo;
                    break;
                case 4:
                    var p4 = periods.FirstOrDefault(x => x.Index == 12);
                    if (p4 != null)
                        date = p4.DateTo;
                    break;
            }

            return date;
        }

        public  DateTime GetStartOfMonth(DateTime date)
        {
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);

            return firstDayOfMonth;
        }

        public  DateTime GetEndOfMonth(DateTime date)
        {
            var lastDayOfMonth = GetStartOfMonth(date).AddMonths(1).AddDays(-1);

            return lastDayOfMonth;
        }

        public DateTime GetStartOfPeriod(DateTime date)
        {
            var period = GetPeriodObjOfDate(date);
            if (period != null)
                return period.DateFrom;

            return new DateTime(2000,1,1);
        }

        public DateTime GetEndOfPeriod(DateTime date)
        {
            var period = GetPeriodObjOfDate(date);
            if (period != null)
                return period.DateTo;

            return new DateTime(2000, 1, 1);
        }
        public  int GetPreviousFYearId(int currentYear)
        {
            ExceedDbContext Db = new ExceedDbContext();
            int previousYear = 0;
            var currYearObj = Db.GLFiscalYears.Find(currentYear);
            if (currYearObj != null)
            {
                var yearsObj = Db.GLFiscalYears.Where(x => x.DateTo < currYearObj.DateFrom).ToList();
                {
                    if (yearsObj.Any())
                    {
                        DateTime maxdate = yearsObj.Max(x => x.DateTo);
                        previousYear = yearsObj.FirstOrDefault(x => x.DateTo == maxdate).GlFiscalYearId;
                    }
                }
            }

            return previousYear;
        }

        public  int GetNextFYearName(string currentYear)
        {
            ExceedDbContext Db = new ExceedDbContext();
            int nextYear = 0;
            var currYearObj = Db.GLFiscalYears.Where(x => x.Name == currentYear).FirstOrDefault();
            if (currYearObj != null)
            {
                var yearsObj = Db.GLFiscalYears.Where(x => x.DateFrom > currYearObj.DateTo).ToList();
                if (yearsObj.Any())
                {
                    DateTime mindate = yearsObj.Min(x => x.DateFrom);
                    nextYear = yearsObj.FirstOrDefault(x => x.DateFrom == mindate).GlFiscalYearId;
                }
            }

            return nextYear;
        }

        public int GetNextFYearId(int currentYear)
        {
            ExceedDbContext Db = new ExceedDbContext();
            int nextYear = 0;
            var currYearObj = Db.GLFiscalYears.Find(currentYear);
            if (currYearObj != null)
            {
                var yearsObj = Db.GLFiscalYears.Where(x => x.DateFrom > currYearObj.DateTo).ToList();
                if (yearsObj.Any())
                {
                    DateTime mindate = yearsObj.Min(x => x.DateFrom);
                    nextYear = yearsObj.FirstOrDefault(x => x.DateFrom == mindate).GlFiscalYearId;
                }
            }

            return nextYear;
        }

        public  string GetFirstPeriodName(int currentYear)
        {
            string periodName = string.Empty;
            //var periods = db.GLPeriods.Where(x => x.GLFiscalYear.Name == currentYear).ToList();
            //if (periods != null)
            //{
            //    DateTime mindate = periods.Min(x => x.DateFrom);
            //    periodName = periods.Where(x => x.DateFrom == mindate).Select(x => x.Name).FirstOrDefault();
            //}

            return periodName;
        }

        public  DateTime GetFiscalYearBeginningDate(DateTime date)
        {
            ExceedDbContext Db = new ExceedDbContext();
            DateTime beginningDate = DateTime.Today;
            var fyear = Db.GLFiscalYears.Where(x => x.DateFrom <= date)
                                        .Where(x => x.DateTo >= date).ToList();
            if (fyear != null)
            {

                if (fyear.Count() == 1)
                {
                    var myYear = fyear.FirstOrDefault();
                    beginningDate = myYear.DateFrom;
                }
                else if (fyear.Count() >= 1)
                {
                    var myYear = fyear.FirstOrDefault();
                    beginningDate = myYear.DateFrom;
                }

            }

            return beginningDate;
        }

        public  DateTime GetFiscalYearEndDate(string year)
        {
            ExceedDbContext Db = new ExceedDbContext();
            DateTime endDate = DateTime.Today;
            var fyear = Db.GLFiscalYears.Where(x => x.Name == year).ToList();
            if (fyear != null)
            {

                endDate = fyear.FirstOrDefault().DateTo;

            }

            return endDate;
        }

        public  int GetFiscalYearOfDate(DateTime date)
        {
            ExceedDbContext Db = new ExceedDbContext();
            int year = 0;
            var fyear = Db.GLFiscalYears.Where(x => x.DateFrom <= date).FirstOrDefault(x => x.DateTo >= date);
            if (fyear != null)
            {

                year = fyear.GlFiscalYearId;

            }

            return year;
        }

        public  int GetPeriodOfDate(DateTime date)
        {
            ExceedDbContext Db = new ExceedDbContext();
            int glPeriodId = 0;
            var glPeriod = Db.GLPeriods.Where(x => x.DateFrom <= date.Date && x.DateTo >= date.Date).FirstOrDefault();
            if (glPeriod != null)
                glPeriodId = glPeriod.GLPeriodId;

            return glPeriodId;
        }

        public GLPeriod GetPeriodObjOfDate(DateTime date)
        {
            ExceedDbContext Db = new ExceedDbContext();
            
            var glPeriod = Db.GLPeriods.Where(x => x.DateFrom <= date.Date).FirstOrDefault(x => x.DateTo >= date.Date);
            if (glPeriod != null)
                return glPeriod;

            return null;
        }

        public  string GetPeriodName(int id)
        {
            ExceedDbContext Db = new ExceedDbContext();
            string periodName = "";
            var glPeriod = Db.GLPeriods.Find(id);
            if (glPeriod != null)
                periodName = glPeriod.Name;
            return periodName;
        }
        public  int GetParallelPeriodInYear(int glperiod,int year)
        {
            ExceedDbContext Db = new ExceedDbContext();
            int periodId = 0;
          
            var periodobj = Db.GLPeriods.Find(glperiod);
            var glPeriod = Db.GLPeriods.Where(x => x.GlFiscalYearId == year).FirstOrDefault(x => x.Index == periodobj.Index);

            if (glPeriod != null) periodId = glPeriod.GLPeriodId;

            return periodId;
        }

        public  void UpdatePeriodIndex()
        {
            ExceedDbContext Db = new ExceedDbContext();
            var years = Db.GLFiscalYears.ToList();
            foreach (var year in years)
            {
                int index = 0;
                var periods = Db.GLPeriods.Where(x => x.GlFiscalYearId == year.GlFiscalYearId).OrderBy(x=>x.DateFrom).ToList();
                foreach (var period in periods)
                {
                    index = index + 1;
                    period.Index = index;
                    Db.Entry(period).State = EntityState.Modified;
                    Db.SaveChanges();
                }
            }
           
        }
        public int GetFiscalYearLastPeriod(int fiscalYear)
        {
            ExceedDbContext Db = new ExceedDbContext();
            int period = 0;
            var months = Db.GLPeriods.Where(x => x.GlFiscalYearId == fiscalYear).ToList();

            if (months.Any())
            {
                var lastMonthIndex = months.Max(x => x.Index);
                var lastMonthObj = months.FirstOrDefault(x => x.Index == lastMonthIndex);
                if (lastMonthObj != null)
                {
                    period = lastMonthObj.GLPeriodId;
                }
            }

            return period;

        }

        public int GetFiscalYearFirstPeriod(int fiscalYear)
        {
            ExceedDbContext Db = new ExceedDbContext();
            int period = 0;
            var months = Db.GLPeriods.Where(x => x.GlFiscalYearId == fiscalYear).ToList();

            if (months.Any())
            {
                var firstMonthIndex = months.Min(x => x.Index);
                var firstMonthObj = months.FirstOrDefault(x => x.Index == firstMonthIndex);
                if (firstMonthObj != null)
                {
                    period = firstMonthObj.GLPeriodId;
                }
            }

            return period;

        }
    }
}
