using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    [Authorize(Roles = Payroll_Input + "," + Payroll_Admin)]
    public class OverTimeTrasferDetailController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string Payroll_Input = "Payroll_Input";
        private const string Payroll_Admin = "Payroll_Admin";
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
            HRMInputs inputs = new HRMInputs();
            var list = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            return View();
        }

        public ActionResult OverTimes_Read([DataSourceRequest]DataSourceRequest request, int id, int period, int tId)
        {
            IQueryable<OverTime> overtimes = db.OverTimes.Where( x=> x.Remark == tId .ToString( ));
            DataSourceResult result = overtimes.ToDataSourceResult(request, overTime => new
            {
                OverTimeId = overTime.OverTimeId,
                EmployeeId = id,
                GLPeriodId = period ,
                NormalHours = overTime.NormalHours,
                NightHours = overTime.NightHours,
                HolidayHours = overTime.HolidayHours,
                RestdayHours = overTime.RestdayHours,
                Remark = tId .ToString( )
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverTimes_Create([DataSourceRequest]DataSourceRequest request, OverTime overTime, int idTwo, int periodTwo, int tIdTwo)
        {
            if (ModelState.IsValid)
            {
                var entity = new OverTime
                {
                    EmployeeId = idTwo ,
                    GLPeriodId = periodTwo  ,
                    NormalHours = overTime.NormalHours,
                    NightHours = overTime.NightHours,
                    HolidayHours = overTime.HolidayHours,
                    RestdayHours = overTime.RestdayHours,
                    Remark = tIdTwo  .ToString( )
                };

                db.OverTimes.Add(entity);
                db.SaveChanges();
                overTime.OverTimeId = entity.OverTimeId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll overtime transfer detail", "Success", User.Identity.Name, $"added overtime transfer for employee  {entity.Employee.EmployeeCode} period {entity.GLPeriodId} normal {entity.NormalHours} night {entity.NightHours} holiday {entity.HolidayHours} rest {entity.RestdayHours} ", Modules.Payroll);
                #endregion
            }

            return Json(new[] { overTime }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverTimes_Update([DataSourceRequest]DataSourceRequest request, OverTime overTime)
        {
            if (ModelState.IsValid)
            {
                var entity = new OverTime
                {
                    OverTimeId = overTime.OverTimeId,
                    EmployeeId = overTime.EmployeeId,
                    GLPeriodId = overTime.GLPeriodId,
                    NormalHours = overTime.NormalHours,
                    NightHours = overTime.NightHours,
                    HolidayHours = overTime.HolidayHours,
                    RestdayHours = overTime.RestdayHours,
                    Remark = overTime.Remark
                };

                db.OverTimes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll overtime transfer detail", "Success", User.Identity.Name, $"updated overtime transfer for employee  {entity.Employee.EmployeeCode} period {entity.GLPeriodId} normal {entity.NormalHours} night {entity.NightHours} holiday {entity.HolidayHours} rest {entity.RestdayHours} ", Modules.Payroll);
                #endregion
            }

            return Json(new[] { overTime }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverTimes_Destroy([DataSourceRequest]DataSourceRequest request, OverTime overTime)
        {
            if (ModelState.IsValid)
            {
                var entity = new OverTime
                {
                    OverTimeId = overTime.OverTimeId,
                    EmployeeId = overTime.EmployeeId,
                    GLPeriodId = overTime.GLPeriodId,
                    NormalHours = overTime.NormalHours,
                    NightHours = overTime.NightHours,
                    HolidayHours = overTime.HolidayHours,
                    RestdayHours = overTime.RestdayHours,
                    Remark = overTime.Remark
                };
                #region delete log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee over time transfer information of employee " + overTime.EmployeeId + " and period " + overTime.GLPeriodId,Modules.Payroll);
                #endregion
                db.OverTimes.Attach(entity);
                db.OverTimes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { overTime }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
