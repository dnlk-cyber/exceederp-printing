using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Helpers;
using System;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Attendance
{
    [AuthorizeRoles(HrmConstants.HRAttendanceUser)]
    public class EmployeeAttendanceController : BaseController
    {
        // GET: HRM/EmployeeExperience
          private ExceedDbContext db = new ExceedDbContext();
          HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {

            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
        }
        public ActionResult Attendances_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<Core.Domain.HRM.Attendance.Attendance> attendances = db.Attendances.Where(x => employees.Contains(x.EmployeeId.ToString()));
            DataSourceResult result = attendances.ToDataSourceResult(request, attendance => new
            {
                AttendanceId = attendance.AttendanceId,
                EmployeeId = attendance.EmployeeId,
                FromDateTime = attendance.FromDateTime,
                ToDateTime = attendance.ToDateTime,
                DelayTime = attendance.DelayTime,
                NoOfAbcent = attendance.NoOfAbcent,
                MeasureTaken = attendance.MeasureTaken,
                Remark = attendance.Remark,
                EmployeeCode = attendance.EmployeeCode
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Attendances_Create([DataSourceRequest]DataSourceRequest request, Core.Domain.HRM.Attendance.Attendance attendance)
        {
            if (ModelState.IsValid)
            {
                var entity = new Core.Domain.HRM.Attendance.Attendance
                {
                    FromDateTime = attendance.FromDateTime,
                    EmployeeId = attendance.EmployeeId,
                    ToDateTime = attendance.ToDateTime,
                    DelayTime = attendance.DelayTime,
                    NoOfAbcent = attendance.NoOfAbcent,
                    MeasureTaken = attendance.MeasureTaken,
                    Remark = attendance.Remark,
                    EmployeeCode = inputs. GetEmployees( ).Where(x => x.PersonId == attendance.EmployeeId).FirstOrDefault().EmployeeCode
                };

                db.Attendances.Add(entity);
                db.SaveChanges();
                attendance.AttendanceId = entity.AttendanceId;

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
               
                    UserHelper.OperationLog(Request.UserHostAddress, "Employee Attendance", "Success", User.Identity.Name, User.Identity.Name + " add employee attendace for " + User.Identity.Name + " at date " + DateTime.Today,Modules.HRM);
                
                #endregion

            }

            return Json(new[] { attendance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Attendances_Update([DataSourceRequest]DataSourceRequest request, Core.Domain.HRM.Attendance.Attendance attendance)
        {
            if (ModelState.IsValid)
            {
                var entity = new Core.Domain.HRM.Attendance.Attendance
                {
                    AttendanceId = attendance.AttendanceId,
                    EmployeeId = attendance.EmployeeId,
                    FromDateTime = attendance.FromDateTime,
                    ToDateTime = attendance.ToDateTime,
                    DelayTime = attendance.DelayTime,
                    NoOfAbcent = attendance.NoOfAbcent,
                    MeasureTaken = attendance.MeasureTaken,
                    Remark = attendance.Remark,
                    EmployeeCode = inputs. GetEmployees( ).Where(x => x.PersonId == attendance.EmployeeId).FirstOrDefault().EmployeeCode
                };

                db.Attendances.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
               
                    UserHelper.OperationLog(Request.UserHostAddress, "Employee Attendance", "Success", User.Identity.Name, User.Identity.Name + " updated employee attendace of " + User.Identity.Name + " at date " + DateTime.Today,Modules.HRM);
                
                #endregion

            }

            return Json(new[] { attendance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Attendances_Destroy([DataSourceRequest]DataSourceRequest request, Core.Domain.HRM.Attendance.Attendance attendance)
        {
            if (ModelState.IsValid)
            {
                var entity = new Core.Domain.HRM.Attendance.Attendance
                {
                    AttendanceId = attendance.AttendanceId,
                    EmployeeId = attendance.EmployeeId,
                    FromDateTime = attendance.FromDateTime,
                    ToDateTime = attendance.ToDateTime,
                    DelayTime = attendance.DelayTime,
                    NoOfAbcent = attendance.NoOfAbcent,
                    MeasureTaken = attendance.MeasureTaken,
                    Remark = attendance.Remark,
                    EmployeeCode = attendance.EmployeeCode
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee attendance information of employee" + attendance.EmployeeCode);
                #endregion
                db.Attendances.Attach(entity);
                db.Attendances.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { attendance }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}