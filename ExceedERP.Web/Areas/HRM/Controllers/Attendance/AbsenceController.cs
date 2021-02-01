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
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Attendance
{
     
    [AuthorizeRoles(HrmConstants.HRAttendanceUser)]
    public class AbsenceController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        HRMInputs inputs = new HRMInputs();
        public static DateTime From { get; set; }
        public static DateTime To { get; set; }
        public static string FromAmharic { get; set; }
        public static string ToAmharic { get; set; }
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
        public void SetDate(string name, string value)
        {
            if (name == "FromAbscAmharic")
            {
                FromAmharic = value;
                var splitDate = FromAmharic.Split('/');
                From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "ToAbscAmharic")
            {
                ToAmharic = value;
                var splitDate = ToAmharic.Split('/');
                To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
        }
        public ActionResult Absence_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<Absence> absences = db.Absence.Where(x => employees.Contains(x.EmployeeId.ToString()));
            DataSourceResult result = absences.ToDataSourceResult(request, absence => new
            {
                AbsenceId = absence.AbsenceId,
                EmployeeId = absence.EmployeeId,
                From = absence.From,
                To = absence.To,
                FromAbscAmharic = absence.FromAbscAmharic ,
                ToAbscAmharic = absence.ToAbscAmharic,
                NoOfWorkingDays = absence.NoOfWorkingDays,
                Reason = absence.Reason,
                MeasureTaken = absence.MeasureTaken,
                Remark = absence.Remark,
                EmployeeCode = absence.EmployeeCode
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Absence_Create([DataSourceRequest]DataSourceRequest request, Absence absence)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (ToAmharic == null)
                {
                    ModelState.AddModelError("", "Date to value is mandatory!!");
                }
                else if (From.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Start Date should be less or equal to the current date!!");
                }
                else if (From.Date > To.Date)
                {

                    ModelState.AddModelError("", "Start date should be less or equal to end date!!");
                }

                else
                {
                    var entity = new Absence
                    {
                        EmployeeId = absence.EmployeeId,
                        From = From,
                        To = To,
                        FromAbscAmharic = FromAmharic,
                        ToAbscAmharic = ToAmharic,
                        NoOfWorkingDays = (absence.From.Date - absence.To.Date).Days,
                        Reason = absence.Reason,
                        MeasureTaken = absence.MeasureTaken,
                        Remark = absence.Remark,
                        EmployeeCode =
                            inputs.GetEmployees()
                                .Where(x => x.PersonId == absence.EmployeeId)
                                .FirstOrDefault()
                                .EmployeeCode
                    };

                    db.Absence.Add(entity);
                    db.SaveChanges();
                    absence.AbsenceId = entity.AbsenceId;

                    #region operation log

                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                    
                        UserHelper.OperationLog(Request.UserHostAddress, "Absence", "Success", User.Identity.Name, User.Identity.Name + " add new absence for " + User.Identity.Name + " at date " + DateTime.Today,Modules.HRM);
                    
                    #endregion
                }
            }

            return Json(new[] { absence }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Absence_Update([DataSourceRequest]DataSourceRequest request, Absence absence)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                 if (FromAmharic == null)
                {
                    FromAmharic = absence.FromAbscAmharic;
                    var splitDate = FromAmharic.Split('/');
                    From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                else if (ToAmharic == null)
                {
                    ToAmharic = absence.ToAbscAmharic;
                    var splitDate = ToAmharic.Split('/');
                    To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (From.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Start Date should be less or equal to the current date!!");
                }
                else if (From.Date > To.Date)
                {

                    ModelState.AddModelError("", "From should be less or equal to To date!!");
                }
                else
                {
                    var entity = new Absence
                    {
                        AbsenceId = absence.AbsenceId,
                        EmployeeId = absence.EmployeeId,
                        From = From,
                        To = To,
                        FromAbscAmharic = FromAmharic,
                        ToAbscAmharic = ToAmharic,
                        NoOfWorkingDays = (absence.From.Date - absence.To.Date).Days,
                        Reason = absence.Reason,
                        MeasureTaken = absence.MeasureTaken,
                        Remark = absence.Remark,
                        EmployeeCode =
                            inputs.GetEmployees()
                                .Where(x => x.PersonId == absence.EmployeeId)
                                .FirstOrDefault()
                                .EmployeeCode
                    };

                    db.Absence.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    #region operation log

                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                    
                        UserHelper.OperationLog(Request.UserHostAddress, "Absence", "Success", User.Identity.Name, User.Identity.Name + " updated absence info of " + User.Identity.Name + " at date " + DateTime.Today,Modules.HRM);
                    
                    #endregion
                }
            }

            return Json(new[] { absence }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Absence_Destroy([DataSourceRequest]DataSourceRequest request, Absence absence)
        {
            if (ModelState.IsValid)
            {
                var entity = new Absence
                {
                    AbsenceId = absence.AbsenceId,
                    EmployeeId = absence.EmployeeId,
                    From = absence.From,
                    To = absence.To,
                    FromAbscAmharic = absence.FromAbscAmharic,
                    ToAbscAmharic = absence.ToAbscAmharic,
                    NoOfWorkingDays = absence.NoOfWorkingDays,
                    Reason = absence.Reason,
                    MeasureTaken = absence.MeasureTaken,
                    Remark = absence.Remark,
                    EmployeeCode = absence.EmployeeCode
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee absence information of employee" + absence.EmployeeCode );
                #endregion
                db.Absence.Attach(entity);
                db.Absence.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { absence }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
