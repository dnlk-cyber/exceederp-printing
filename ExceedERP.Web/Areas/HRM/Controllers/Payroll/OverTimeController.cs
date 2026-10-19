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
using System;
using System.Configuration;

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
            ViewData["PeriodList"] = db.GLPeriods
                .Select(x =>new {
                    x.Name,
                    x.GLPeriodId
            }).ToList(); ;
        
           var list = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            return View();
        }
        public JsonResult GetPeriod()
        {
            var now = DateTime.Now;

            var fiscalYearId = db.GLFiscalYears
                .Where(x => x.DateFrom <= now && x.DateTo >= now)
                .Select(x => x.GlFiscalYearId)
                .FirstOrDefault();

            var periods = db.GLPeriods
                .Where(x => x.GlFiscalYearId == fiscalYearId)
                .Select(x => new
                {
                    x.GLPeriodId,
                    x.Name
                })
                .ToList();

            return Json(periods, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OverTimes_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<int> employees = inputs.GetUserBranchAndBusinessUnitEmployees(User.Identity.Name); //inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<OverTime> overtimes = db.OverTimes.Where(x => employees.Contains(x.EmployeeId));
            DataSourceResult result = overtimes.ToDataSourceResult(request, overTime => new {
                OverTimeId = overTime.OverTimeId,
                EmployeeId = overTime.EmployeeId,
                GLPeriodId = overTime.GLPeriodId,
                NormalHours = overTime.NormalHours,
                NightHours = overTime.NightHours,
                HolidayHours = overTime.HolidayHours,
                RestdayHours = overTime.RestdayHours,
                Remark = overTime.Remark,
                IsFromOverTimeRequest = overTime.IsFromOverTimeRequest,
                OverTimeRequestId = overTime.OverTimeRequestId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverTimes_Create([DataSourceRequest]DataSourceRequest request, OverTime overTime)
        {
            var otexist = db.OverTimes.Where(x => x.GLPeriodId == overTime.GLPeriodId).FirstOrDefault(x => x.EmployeeId == overTime.EmployeeId);
            if (otexist != null)
            {
                ModelState.AddModelError("", "You cannot add multiple overtime for one employee!!");
            }
            var employeeBranchId = EmployeeHelper.GetEmployeePlacementBranch(overTime.EmployeeId);
            var checkexistance = db.PayrollProcessViewModel
                 .Where(x => x.BranchId == employeeBranchId)
                .FirstOrDefault(x => x.GLPeriodId == overTime.GLPeriodId);
            if (checkexistance != null)
            {
                ModelState.AddModelError("", "You cannot add this overtime. This period is already processed!!");
                return Json(new[] { overTime }.ToDataSourceResult(request, ModelState));

            }
            var placement = db.Placements.FirstOrDefault(p => p.EmployeeId == overTime.EmployeeId && p.isCurrent);
            if (placement == null)
            {
                ModelState.AddModelError("", "You cannot add this overtime. Employee doesn't have current placement!!");
                return Json(new[] { overTime }.ToDataSourceResult(request, ModelState));
            }

            int? branchId = placement.BranchId;

            // Check branch-specific setting first
            //var branchSetting = db.OverTimeSettings.FirstOrDefault(s => s.BranchId == branchId);
            //if (branchSetting != null && placement.IsHead && branchSetting.NotAllowedForHead)
            //{
            //    ModelState.AddModelError("", "Overtime is not allowed for head employees in this Branch!!");
            //    return Json(new[] { overTime }.ToDataSourceResult(request, ModelState));
            //}

            //// If no branch-specific setting, check global setting (BranchId is null)
            //var globalSetting = db.OverTimeSettings.FirstOrDefault(s => s.BranchId == null);
            //if (globalSetting != null && placement.IsHead && globalSetting.NotAllowedForHead)
            //{
            //    ModelState.AddModelError("", "Overtime is not allowed for head employees !!");
            //    return Json(new[] { overTime }.ToDataSourceResult(request, ModelState));
            //}

            if (ModelState.IsValid)
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

            return Json(new[] { overTime }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverTimes_Update([DataSourceRequest]DataSourceRequest request, OverTime overTime)
        {
            var existing = db.OverTimes.AsNoTracking().FirstOrDefault(x => x.OverTimeId == overTime.OverTimeId);
            if (existing != null && existing.IsFromOverTimeRequest)
            {
                ModelState.AddModelError("", "This overtime was generated from an approved Overtime Request and cannot be edited here!!");
                return Json(new[] { overTime }.ToDataSourceResult(request, ModelState));
            }

            var otexist = db.OverTimes.Where(x => x.OverTimeId != overTime.OverTimeId).Where(x => x.GLPeriodId == overTime.GLPeriodId).FirstOrDefault(x => x.EmployeeId == overTime.EmployeeId);
            if (otexist != null)
            {
                ModelState.AddModelError("", "You cannot add multiple overtime for one employee!!");
                return Json(new[] { overTime }.ToDataSourceResult(request, ModelState));

            }
            var employeeBranchId = EmployeeHelper.GetEmployeePlacementBranch(overTime.EmployeeId);
            var checkexistance = db.PayrollProcessViewModel
                .Where(x => x.BranchId == employeeBranchId)
                .FirstOrDefault(x => x.GLPeriodId == overTime.GLPeriodId && x.EmployeeId == overTime.EmployeeId);
            if (checkexistance != null)
            {

                ModelState.AddModelError("", "You cannot update this overtime. This period is already processed!!");
                return Json(new[] { overTime }.ToDataSourceResult(request, ModelState));
            }
            var placement = db.Placements.FirstOrDefault(p => p.EmployeeId == overTime.EmployeeId && p.isCurrent);
            if (placement == null)
            {
                ModelState.AddModelError("", "You cannot add this overtime. Employee doesn't have current placement!!");
                return Json(new[] { overTime }.ToDataSourceResult(request, ModelState));
            }

            int? branchId = placement.BranchId;

            // Check branch-specific setting first
            //var branchSetting = db.OverTimeSettings.FirstOrDefault(s => s.BranchId == branchId);
            //if (branchSetting != null && placement.IsHead && branchSetting.NotAllowedForHead)
            //{
            //    ModelState.AddModelError("", "Overtime is not allowed for head employees in this Branch!!");
            //    return Json(new[] { overTime }.ToDataSourceResult(request, ModelState));
            //}

            //// If no branch-specific setting, check global setting (BranchId is null)
            //var globalSetting = db.OverTimeSettings.FirstOrDefault(s => s.BranchId == null);
            //if (globalSetting != null && placement.IsHead && globalSetting.NotAllowedForHead)
            //{
            //    ModelState.AddModelError("", "Overtime is not allowed for head employees !!");
            //    return Json(new[] { overTime }.ToDataSourceResult(request, ModelState));
            //}
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
                UserHelper.OperationLog(Request.UserHostAddress, "payroll overtime", "Success", User.Identity.Name, $"updated overtime for employee  {entity.EmployeeId} period {entity.GLPeriodId} normal {entity.NormalHours} night {entity.NightHours} holiday {entity.HolidayHours} rest {entity.RestdayHours} ", Modules.Payroll);
                #endregion
                
            }

            return Json(new[] { overTime }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverTimes_Destroy([DataSourceRequest]DataSourceRequest request, OverTime overTime)
        {
            var existingForDestroy = db.OverTimes.AsNoTracking().FirstOrDefault(x => x.OverTimeId == overTime.OverTimeId);
            if (existingForDestroy != null && existingForDestroy.IsFromOverTimeRequest)
            {
                ModelState.AddModelError("", "This overtime was generated from an approved Overtime Request and cannot be deleted here!!");
                return Json(new[] { overTime }.ToDataSourceResult(request, ModelState));
            }

            var checkexistance = db.PayrollProcessViewModel.Where(x => x.EmployeeId == overTime.EmployeeId && x.GLPeriodId == overTime.GLPeriodId).FirstOrDefault();
            if (checkexistance != null)
            {

                ModelState.AddModelError("", "You cannot delete this data. It already processed!!");
                return Json(new[] { overTime }.ToDataSourceResult(request, ModelState));

            }
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
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee over time information of employee " + overTime.EmployeeId + " and period " + overTime.GLPeriodId,Modules.Payroll);
                #endregion
                db.OverTimes.Attach(entity);
                db.OverTimes.Remove(entity);
                db.SaveChanges();


            }

            return Json(new[] { overTime }.ToDataSourceResult(request, ModelState));
        }
        //private OTSetting GetOTSetting()
        //{
        //    OTSetting oTSetting = new OTSetting();
        //    var otrates = db.Configurations.Where(x => x.Reference == "OverTime").ToList();
        //    if (otrates.Any())
        //    {
        //        Configuration config = new Configuration();
        //        config = otrates.Where(o => o.Attribute == OverTimeAttribute.RESTDAY_RATE.ToString())
        //            .FirstOrDefault();
        //        if (config != null)
        //        {
        //            oTSetting.RestDayRate =
        //               Convert.ToDecimal(config.CurrentValue);
        //        }
        //        config = otrates.Where(o => o.Attribute == OverTimeAttribute.NORMAL_RATE.ToString())
        //            .FirstOrDefault();
        //        if (config != null)
        //        {
        //            oTSetting.NormalRate =
        //               Convert.ToDecimal(config.CurrentValue);
        //        }
        //        config = otrates.Where(o => o.Attribute == OverTimeAttribute.NIGHT_RATE.ToString())
        //            .FirstOrDefault();
        //        if (config != null)
        //        {
        //            oTSetting.NightRate =
        //               Convert.ToDecimal(config.CurrentValue);
        //        }
        //        config = otrates.Where(o => o.Attribute == OverTimeAttribute.HOLIDAY_RATE.ToString())
        //            .FirstOrDefault();
        //        if (config != null)
        //        {
        //            oTSetting.HolidayRate =
        //           Convert.ToDecimal(config.CurrentValue);
        //        }
        //        config = otrates.Where(o => o.Attribute == OverTimeAttribute.COEFFICIENT.ToString())
        //            .FirstOrDefault();
        //        if (config != null)
        //        {
        //            oTSetting.OTCoefficient =
        //               Convert.ToDecimal(config.CurrentValue);
        //        }
        //    }
        //    return oTSetting;
        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }

    
}
