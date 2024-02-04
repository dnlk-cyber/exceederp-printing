
using ExceedERP.Core.Domain.printing.JobCosting;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ExceedERP.Web.Areas.Printing.Models
{
    public class UpdateDLR
    {

        internal  void UpdateDLRRow(DLRSalary dLRSalary, DLRHour dLRHour, ExceedDbContext db, bool check=true)
        {
            var id = 0;
            var dlrId = 0;
            if (dLRSalary != null)
            {
                 dlrId = dLRSalary.DailyLaborRateId;
            }
            if (dLRHour != null)
            {
                dlrId = dLRHour.DailyLaborRateId;
            }
            decimal? TotalSalary = db.DLRSalaries
                                   .Where(q => q.DailyLaborRateId == dlrId).ToList()
                                   .Sum(s => s.TotalSalary);
            decimal? TotalHour = db.DLRHours
                                  .Where(q => q.DailyLaborRateId == dlrId).ToList()
                                  .Sum(s => s.ExpectedTime);
            
            if (dLRHour != null)
            {
                id = dLRHour.DailyLaborRateId;
                var hour = db.DLRHours.Find(dLRHour.DLRHourId)?.ExpectedTime;
                if(hour==null)
                {
                    hour = 0;
                }
                if (TotalHour == null)
                {
                    TotalHour = 0;

                }
                TotalHour = TotalHour + dLRHour.ExpectedTime - hour;

                if (check==false)
                {
                    TotalHour = TotalHour - dLRHour.ExpectedTime;

                }

            }
            if (dLRSalary != null)
            {
                id = dLRSalary.DailyLaborRateId;
                var salary = db.DLRSalaries.Find(dLRSalary.DLRSalaryId)?.TotalSalary;
                if (salary == null)
                {
                    salary = 0;
                }
                if (TotalSalary == null)
                {
                    TotalSalary = 0;

                }
                TotalSalary = TotalSalary + dLRSalary.TotalSalary - salary;

                if (check == false)
                {
                    TotalSalary = TotalSalary - dLRSalary.TotalSalary ;

                }
            }
            DailyLaborRate dailyLaborRate = db.DailyLaborRates
                                             .Find(id);

            decimal? dlr = 0;
            if (TotalHour > 0)
            {
                dlr = TotalSalary / TotalHour;
            }
            
            dailyLaborRate.TotalExpectedHour = TotalHour;
            dailyLaborRate.TotalSalary = TotalSalary;
            dailyLaborRate.DLR = dlr;
            db.DailyLaborRates.Attach(dailyLaborRate);
            db.Entry(dailyLaborRate).State = EntityState.Modified;
        }

        internal void UpdateDLRRow(DLRSalary entity, DLRHour dLRHour, ExceedDbContext db)
        {
            throw new NotImplementedException();
        }
    }
}