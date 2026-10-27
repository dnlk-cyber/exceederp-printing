﻿using System;
using System.Collections.Generic;
﻿using System.Data.Entity;
﻿using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.Payroll;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
     [Authorize(Roles = Payroll_Input + "," + Payroll_Admin)]
    public class OverTimePerDayController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string Payroll_Input = "Payroll_Input";
        private const string Payroll_Admin = "Payroll_Admin";
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            var periodList = db.PayrollPeriods.Where(x => x.Status == FiscalYearStatus.Open || x.Status == FiscalYearStatus.Close);
            var lists = from p in periodList
                        join g in db.GLPeriods
                            on p.Name equals g.Name
                        select (new
                        {
                            g
                        });
            ViewData["PeriodList"] = lists.Select(x => x.g).ToList(); ;
            var tickets = db.OverTimePerDays.AsQueryable();
            ViewData["TicketList"] = tickets;
          
         
           var list = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            return View();
        }
     
        public ActionResult OverTimePerDays_Read([DataSourceRequest]DataSourceRequest request, string ticketNo)
        {
            List<int> employees = inputs.GetUserBranchEmployees(User.Identity.Name);

            IQueryable<OverTimePerDay> overtimeperdays = db.OverTimePerDays.Where(x => employees.Contains(x.EmployeeId));
            if (ticketNo != "")
            {
                overtimeperdays = overtimeperdays.Where(x => x.TicketNumber == ticketNo);
            }
            DataSourceResult result = overtimeperdays.ToDataSourceResult(request, overTimePerDay => new {
                TicketNumber = overTimePerDay.TicketNumber,
                EmployeeId = overTimePerDay.EmployeeId,
                GLPeriodId = overTimePerDay.GLPeriodId,
                NormalHours = overTimePerDay.NormalHours,
                NightHours = overTimePerDay.NightHours,
                Date = overTimePerDay.Date,
                HolidayHours = overTimePerDay.HolidayHours,
                RestdayHours = overTimePerDay.RestdayHours,
                From = overTimePerDay.From,
                To = overTimePerDay.To,
                TotalHr = overTimePerDay.TotalHr,
                Remark = overTimePerDay.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverTimePerDays_Create([DataSourceRequest]DataSourceRequest request, OverTimePerDay overTimePerDay)
        {
            if (ModelState.IsValid)
            {
                var entity = new OverTimePerDay
                {
                    TicketNumber = overTimePerDay.TicketNumber,
                    EmployeeId = overTimePerDay.EmployeeId,
                    GLPeriodId = overTimePerDay.GLPeriodId,
                    NormalHours = overTimePerDay.NormalHours,
                    NightHours = overTimePerDay.NightHours,
                    HolidayHours = overTimePerDay.HolidayHours,
                    RestdayHours = overTimePerDay.RestdayHours,
                    From = overTimePerDay.From,
                    Date = overTimePerDay.Date,
                    To = overTimePerDay.To,
                    TotalHr = overTimePerDay.TotalHr,
                    Remark = overTimePerDay.Remark
                };

                db.OverTimePerDays.Add(entity);
                db.SaveChanges();
                overTimePerDay.TicketNumber = entity.TicketNumber;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll overtime per day", "Success", User.Identity.Name, $"added overtime per day for employee  {entity.EmployeeId} period {entity.GLPeriodId} ticket number {entity.TicketNumber} normal {entity.NormalHours} night {entity.NightHours} holiday {entity.HolidayHours} rest {entity.RestdayHours} ", Modules.Payroll);
                #endregion
            }

            return Json(new[] { overTimePerDay }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverTimePerDays_Update([DataSourceRequest]DataSourceRequest request, OverTimePerDay overTimePerDay)
        {
            if (ModelState.IsValid)
            {
                var entity = new OverTimePerDay
                {
                    TicketNumber = overTimePerDay.TicketNumber,
                    EmployeeId = overTimePerDay.EmployeeId,
                    GLPeriodId = overTimePerDay.GLPeriodId,
                    NormalHours = overTimePerDay.NormalHours,
                    NightHours = overTimePerDay.NightHours,
                    HolidayHours = overTimePerDay.HolidayHours,
                    RestdayHours = overTimePerDay.RestdayHours,
                    From = overTimePerDay.From,
                    Date = overTimePerDay.Date,
                    To = overTimePerDay.To,
                    TotalHr = overTimePerDay.TotalHr,
                    Remark = overTimePerDay.Remark
                };

                db.OverTimePerDays.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll overtime per day", "Success", User.Identity.Name, $"updated overtime per day for employee  {entity.EmployeeId} period {entity.GLPeriodId} ticket number {entity.TicketNumber} normal {entity.NormalHours} night {entity.NightHours} holiday {entity.HolidayHours} rest {entity.RestdayHours} ", Modules.Payroll);
                #endregion
            }

            return Json(new[] { overTimePerDay }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverTimePerDays_Destroy([DataSourceRequest]DataSourceRequest request, OverTimePerDay overTimePerDay)
        {
            if (ModelState.IsValid)
            {
                var entity = new OverTimePerDay
                {
                    TicketNumber = overTimePerDay.TicketNumber,
                    EmployeeId = overTimePerDay.EmployeeId,
                    GLPeriodId = overTimePerDay.GLPeriodId,
                    NormalHours = overTimePerDay.NormalHours,
                    NightHours = overTimePerDay.NightHours,
                    HolidayHours = overTimePerDay.HolidayHours,
                    RestdayHours = overTimePerDay.RestdayHours,
                    From = overTimePerDay.From,
                    Date = overTimePerDay.Date,
                    To = overTimePerDay.To,
                    TotalHr = overTimePerDay.TotalHr,
                    Remark = overTimePerDay.Remark
                };
                #region delete log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee over time per day information of employee " + overTimePerDay.EmployeeId+ " and period " + overTimePerDay.GLPeriodId,Modules.Payroll);
                #endregion
                db.OverTimePerDays.Attach(entity);
                db.OverTimePerDays.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { overTimePerDay }.ToDataSourceResult(request, ModelState));
        }

        public decimal GetTotalOt(DateTime from, DateTime to)
        {
            var timespan = from.TimeOfDay  - to.TimeOfDay;
           return Convert.ToDecimal(  timespan);
          //  return 0;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
