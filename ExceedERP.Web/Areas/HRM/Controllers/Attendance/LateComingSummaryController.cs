using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.Attendance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Attendance
{
    [AuthorizeRoles(HrmConstants.HRAttendanceUser, HrmConstants.HRAdmin)]
    public class LateComingSummaryController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        HRMInputs inputs = new HRMInputs();
        public static DateTime Date { get; set; }
        public static string DateAmharic { get; set; }
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {

            var list = from e in inputs.GetEmployees(User.Identity.Name).ToList()
                       join l in db.LateCommings
               on e.PersonId equals l.EmployeeId
                       //where e.IsActive
                       select (new { e });

            ViewData["Period"] = db.GLPeriods.ToList();
            ViewData["EmployeeList"] = db.Person_Employee.ToList();//list.Select(x => x.e).ToList();
            //GetPeriod();
        }
        //public JsonResult GetPeriod()
        //{
        //    var periodList = db.PayrollPeriods.Where(x => x.Status == FiscalYearStatus.Open);
        //    var listt = from p in periodList
        //                join g in db.GLPeriods
        //                    on p.Name equals g.Name
        //                select (new
        //                {
        //                    g
        //                });
        //    ViewData["PeriodList"] = listt.Select(x => x.g).ToList();

        //    return Json(listt.Select(p => new { GLPeriodId = p.g.GLPeriodId, Name = p.g.Name }), JsonRequestBehavior.AllowGet);
        //}
        public void SetDate(string name, string value)
        {
            if (name == "DateLateAmharic")
            {
                DateAmharic = value;
                var splitDate = DateAmharic.Split('/');
                Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

        }

        public ActionResult LateComingSummaries_Read([DataSourceRequest]DataSourceRequest request)
        {
            //var periodList = from p in db.PayrollPeriods
            //                 join g in db.GLPeriods
            //                 on p.Name equals g.Name
            //                 where p.Status == FiscalYearStatus.Open
            //                 select (new { g.GLPeriodId });

           // List<int> selectedPeriod = periodList.Select(x => x.GLPeriodId).ToList();
            List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<LateComingSummary> LateComingSummaries = db.LateComingSummaries;//.Where(x => employees.Contains(x.EmployeeId.ToString()) /*&& selectedPeriod.Contains(x.GLPeriodId)*/);
            DataSourceResult result = LateComingSummaries.ToDataSourceResult(request, LateComingSummary => new
            {
                LateComingSummaryId = LateComingSummary.LateComingSummaryId,
                EmployeeId = LateComingSummary.EmployeeId,
                TotalDays = LateComingSummary.TotalDays,
                GLPeriodId = LateComingSummary.GLPeriodId,
                TotalHours = LateComingSummary.TotalHours,
                Remark = LateComingSummary.Remark,
                Check = LateComingSummary.Check,
                Processed = CheckPayrollProcess(LateComingSummary.EmployeeId, LateComingSummary.GLPeriodId),
            });

            return Json(result);
        }
        private bool CheckPayrollProcess(int employeeId, int periodId)
        {

            //var checkexistance = db.PayrollProcessViewModel.Where(x => x.EmployeeId == employeeId && x.GLPeriodId == periodId).FirstOrDefault();
            //if (checkexistance != null)
            //{
            //    return true;
            //}

            return false;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LateComingSummaries_Create([DataSourceRequest]DataSourceRequest request, LateComingSummary LateComingSummary)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                {
                    var entity = new LateComingSummary
                    {
                        EmployeeId = LateComingSummary.EmployeeId,
                        TotalDays = LateComingSummary.TotalDays,
                        GLPeriodId = LateComingSummary.GLPeriodId,
                        TotalHours = LateComingSummary.TotalHours,
                        Remark = LateComingSummary.Remark,

                    };


                    db.LateComingSummaries.Add(entity);
                    db.SaveChanges();
                    LateComingSummary.LateComingSummaryId = entity.LateComingSummaryId;
                }
            }

            return Json(new[] { LateComingSummary }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LateComingSummaries_Update([DataSourceRequest]DataSourceRequest request, LateComingSummary LateComingSummary)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                {
                    var entity = new LateComingSummary
                    {
                        LateComingSummaryId = LateComingSummary.LateComingSummaryId,
                        EmployeeId = LateComingSummary.EmployeeId,
                        TotalDays = LateComingSummary.TotalDays,
                        GLPeriodId = LateComingSummary.GLPeriodId,
                        TotalHours = LateComingSummary.TotalHours,
                        Remark = LateComingSummary.Remark,

                    };

                    db.LateComingSummaries.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { LateComingSummary }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LateComingSummaries_Destroy([DataSourceRequest]DataSourceRequest request, LateComingSummary LateComingSummary)
        {
            if (ModelState.IsValid)
            {
                var entity = new LateComingSummary
                {
                    LateComingSummaryId = LateComingSummary.LateComingSummaryId,
                    EmployeeId = LateComingSummary.EmployeeId,
                    TotalDays = LateComingSummary.TotalDays,
                    GLPeriodId = LateComingSummary.GLPeriodId,
                    TotalHours = LateComingSummary.TotalHours,
                    Remark = LateComingSummary.Remark,

                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee late comming information of employee" + LateComingSummary.EmployeeId);
                #endregion
                db.LateComingSummaries.Attach(entity);
                db.LateComingSummaries.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { LateComingSummary }.ToDataSourceResult(request, ModelState));
        }
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult Editing_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<LateComingSummary> summaries)
        //{
        //    var results = new List<LateComingSummary>();
        //    LateComingSummary LateComingSummary = new LateComingSummary();
        //    ExceedDbContext dbs = new ExceedDbContext();
        //    if (summaries != null && ModelState.IsValid)
        //    {
        //        foreach (var summary in summaries)
        //        {
        //            db = new ExceedDbContext();
        //            bool isProcessed = CheckPayrollProcess(summary.EmployeeId, summary.GLPeriodId);
        //            if (!isProcessed)
        //            {
        //                var oneData = db.LateComingSummaries.Where(x => x.LateComingSummaryId == summary.LateComingSummaryId).FirstOrDefault();
        //                LateComingSummary = new LateComingSummary();
        //                LateComingSummary = oneData;
        //                if (oneData != null)
        //                {
        //                    db.LateComingSummaries.Remove(oneData);
        //                    db.SaveChanges();
        //                }
        //                db = new ExceedDbContext();
        //                LateComingSummary.TotalDays = summary.TotalDays;
        //                LateComingSummary.TotalHours = summary.TotalHours;
        //                LateComingSummary.Check = summary.Check;
        //                LateComingSummary.GLPeriodId = summary.GLPeriodId;
        //                LateComingSummary.EmployeeId = summary.EmployeeId;
        //                LateComingSummary.Processed = summary.Processed;
        //                LateComingSummary.TotalDays = LateComingSummary.TotalHours / 8;
        //                db.LateComingSummaries.Add(LateComingSummary);
        //                db.SaveChanges();
        //                oneData.LateComingSummaryId = LateComingSummary.LateComingSummaryId;
        //                if (summary.Check)
        //                {
        //                    SaveAttendance(summary.GLPeriodId, summary.EmployeeId, LateComingSummary.TotalDays);
        //                }
        //                else
        //                {
        //                    RemoveAttendance(summary.GLPeriodId, summary.EmployeeId);
        //                }
        //            }
        //        }
        //    }


        //    return Json(results.ToDataSourceResult(request, ModelState));
        //}
        //private void RemoveAttendance(int period, int empId)
        //{
        //    var checkExistance = db.EmpPayrollNoDaysWorkeds.FirstOrDefault(x => x.GLPeriodId == period && x.EmployeeId == empId && x.Remark == "Late Coming");
        //    if (checkExistance != null)
        //    {
        //        db.EmpPayrollNoDaysWorkeds.Attach(checkExistance);
        //        db.EmpPayrollNoDaysWorkeds.Remove(checkExistance);
        //        db.SaveChanges();
        //    }
        //}
        //private void SaveAttendance(int period, int empId, decimal days)

        //{
        //    var checkExistance = db.EmpPayrollNoDaysWorkeds.FirstOrDefault(x => x.GLPeriodId == period && x.EmployeeId == empId && x.Remark == "Late Coming");
        //    if (checkExistance != null)
        //    {
        //        db.EmpPayrollNoDaysWorkeds.Attach(checkExistance);
        //        db.EmpPayrollNoDaysWorkeds.Remove(checkExistance);
        //        db.SaveChanges();
        //    }

        //    var entity = new EmpPayrollNoDaysWorked
        //    {
        //        EmployeeId = empId,
        //        GLPeriodId = period,
        //        Days = days,
        //        Remark = "Late Coming",
        //        DateCreated = DateTime.Now,
        //        LastModified = null,
        //        CreatedBy = User.Identity.Name,
        //        ModifiedBy = null,
        //        Reason = "Late Coming",
        //        MeasureTaken = null
        //    };

        //    db.EmpPayrollNoDaysWorkeds.Add(entity);
        //    db.SaveChanges();
        //}
        public void Summary(int periodId)
        {
            var period = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == periodId);
            if (period != null)
            {
                var lateComings = db.LateCommings.Where(x => x.Date >= period.DateFrom && x.Date <= period.DateTo).ToList();

                lateComings = lateComings.DistinctByField(x => x.EmployeeId).ToList();

                foreach (var late in lateComings)
                {
                    var singleEmpLates = db.LateCommings.Where(x => x.EmployeeId == late.EmployeeId && x.Date >= period.DateFrom && x.Date <= period.DateTo).ToList();

                    var checkExistance = db.LateComingSummaries.FirstOrDefault(x => x.EmployeeId == late.EmployeeId && x.GLPeriodId == period.GLPeriodId);
                    if (checkExistance != null)
                    {

                        if (singleEmpLates.Any())
                        {
                            checkExistance.TotalHours = singleEmpLates.Sum(x => x.Time);
                            checkExistance.TotalDays = (singleEmpLates.Sum(x => x.Time)) / 8;

                            db.Entry(checkExistance).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        var entity = new LateComingSummary
                        {
                            EmployeeId = late.EmployeeId,
                            TotalDays = 0,
                            GLPeriodId = periodId,
                            TotalHours = singleEmpLates.Sum(x => x.Time),
                            Remark = null,

                        };
                        entity.TotalDays = entity.TotalHours / 8;

                        db.LateComingSummaries.Add(entity);
                        db.SaveChanges();
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
