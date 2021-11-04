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
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    [Authorize(Roles = Payroll_Input+","+Payroll_Admin)]
    public class OverTimeController : BaseController
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
        
           var list = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            return View();
        }
    
        public ActionResult OverTimes_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<OverTime> overtimes = db.OverTimes.Where(x => employees.Contains(x.EmployeeId.ToString()));
            DataSourceResult result = overtimes.ToDataSourceResult(request, overTime => new {
                OverTimeId = overTime.OverTimeId,
                EmployeeId = overTime.EmployeeId,
                GLPeriodId = overTime.GLPeriodId,
                NormalHours = overTime.NormalHours,
                NightHours = overTime.NightHours,
                HolidayHours = overTime.HolidayHours,
                RestdayHours = overTime.RestdayHours,
                Remark = overTime.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverTimes_Create([DataSourceRequest]DataSourceRequest request, OverTime overTime)
        {
            var otexist = db.OverTimes.Where(x => x.GLPeriodId == overTime.GLPeriodId).FirstOrDefault(x => x.EmployeeId == overTime.EmployeeId);
            if (otexist != null)
                ModelState.AddModelError("", "You cannot add multiple overtime for one employee!!");


            if (ModelState.IsValid)
            {
                var employeeBranchId = EmployeeHelper.GetEmployeePlacementBranch(overTime.EmployeeId);
                var checkexistance = db.PayrollProcessViewModel
                     .Where(x=>x.BranchId==employeeBranchId)
                    .FirstOrDefault(x => x.GLPeriodId == overTime.GLPeriodId);
                if (checkexistance != null)
                {
                    ModelState.AddModelError("", "You cannot add this overtime. This period is already processed!!");
                }
                else
                {
                    var entity = new OverTime
                    {
                        EmployeeId = overTime.EmployeeId,
                        GLPeriodId = overTime.GLPeriodId,
                        NormalHours = overTime.NormalHours,
                        NightHours = overTime.NightHours,
                        HolidayHours = overTime.HolidayHours,
                        RestdayHours = overTime.RestdayHours,
                        Remark = overTime.Remark
                    };

                    db.OverTimes.Add(entity);
                    db.SaveChanges();
                    overTime.OverTimeId = entity.OverTimeId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "payroll overtime", "Success", User.Identity.Name, $"added overtime for employee  {entity.EmployeeId} period {entity.GLPeriodId} normal {entity.NormalHours} night {entity.NightHours} holiday {entity.HolidayHours} rest {entity.RestdayHours} ", Modules.Payroll);
                    #endregion
                }
            }

            return Json(new[] { overTime }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverTimes_Update([DataSourceRequest]DataSourceRequest request, OverTime overTime)
        {
            var otexist = db.OverTimes.Where(x => x.OverTimeId != overTime.OverTimeId).Where(x => x.GLPeriodId == overTime.GLPeriodId).FirstOrDefault(x => x.EmployeeId == overTime.EmployeeId);
            if (otexist != null)
                ModelState.AddModelError("", "You cannot add multiple overtime for one employee!!");

            if (ModelState.IsValid)
            {
                var employeeBranchId = EmployeeHelper.GetEmployeePlacementBranch(overTime.EmployeeId);
                var checkexistance = db.PayrollProcessViewModel
                    .Where(x => x.BranchId == employeeBranchId)
                    .FirstOrDefault(x => x.GLPeriodId == overTime.GLPeriodId && x.EmployeeId == overTime .EmployeeId);
                if (checkexistance != null)
                {

                    ModelState.AddModelError("", "You cannot update this overtime. This period is already processed!!");
                }
                else
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
                    UserHelper.OperationLog(Request.UserHostAddress, "payroll overtime", "Success", User.Identity.Name, $"updated overtime for employee  {entity.EmployeeId} period {entity.GLPeriodId} normal {entity.NormalHours} night {entity.NightHours} holiday {entity.HolidayHours} rest {entity.RestdayHours} ", Modules.Payroll);
                    #endregion
                }
            }

            return Json(new[] { overTime }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverTimes_Destroy([DataSourceRequest]DataSourceRequest request, OverTime overTime)
        {
            if (ModelState.IsValid)
            {
                var checkexistance = db.PayrollProcessViewModel.Where(x => x.EmployeeId == overTime.EmployeeId && x.GLPeriodId == overTime.GLPeriodId).FirstOrDefault();
                if (checkexistance != null)
                {
                    
                        ModelState.AddModelError("", "You cannot delete this data. It already processed!!");
                    }
                    else
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
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee over time information of employee " + overTime.EmployeeId + " and period " + overTime.GLPeriodId,Modules.Payroll);
                #endregion
                db.OverTimes.Attach(entity);
                db.OverTimes.Remove(entity);
                db.SaveChanges();
            }
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
