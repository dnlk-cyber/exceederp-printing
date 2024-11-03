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

    [AuthorizeRoles(HrmConstants.HRAttendanceUser,
          HrmConstants.HRDepartmentHead,
          HrmConstants.HREmployeeSelfService)]
    public class LateCommingController : BaseController
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

            var list = inputs.GetEmployees(User.Identity.Name).ToList();
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            ViewData["PeriodList"] = db.GLPeriods .Where(x => x.Status ==Core.Domain.Finance.GL.FiscalYearStatus.Open );
        }
        public void SetDate(string name, string value)
        {
            if (name == "DateLateAmharic")
            {
                DateAmharic = value;
                var splitDate = DateAmharic.Split('/');
                Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

        }     
        public ActionResult LateCommings_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<LateComming> latecommings = db.LateCommings.Where(x => employees.Contains(x.EmployeeId.ToString()));
            DataSourceResult result = latecommings.ToDataSourceResult(request, lateComming => new {
                LateCommingId = lateComming.LateCommingId,
                EmployeeId = lateComming.EmployeeId,
                Date = lateComming.Date,

                DateLateAmharic = lateComming.DateLateAmharic,
                Time = lateComming.Time,
                Remark = lateComming.Remark,
                EmployeeCode = lateComming.EmployeeCode
            });

            return Json(result);
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LateCommings_Create([DataSourceRequest]DataSourceRequest request, LateComming lateComming)
        {ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (DateAmharic == null)
                {
                    ModelState.AddModelError("", "Date to value is mandatory!!");
                }
                else if (Date.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Start Date should be less or equal to the current date!!");
                }
                if (lateComming.Time > 8)
                {
                    ModelState.AddModelError("", "Time Field  not proceed 8 hours");
                }

                else
                {
                    var entity = new LateComming
                    {
                        EmployeeId = lateComming.EmployeeId,
                        Date = Date,
                        DateLateAmharic = DateAmharic,
                        Time = lateComming.Time,
                        Remark = lateComming.Remark,
                        EmployeeCode =
                            inputs.GetEmployees(User.Identity.Name)
                                .Where(x => x.PersonId == lateComming.EmployeeId)
                                .FirstOrDefault()
                                .EmployeeCode
                    };

                  

                    db.LateCommings.Add(entity);
                    db.SaveChanges();
                    lateComming.LateCommingId = entity.LateCommingId;

                    #region operation log

                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                   
                        UserHelper.OperationLog(Request.UserHostAddress, "Late Comming", "Success", User.Identity.Name, User.Identity.Name + " add late comming for " + User.Identity.Name + " at date " + DateTime.Today,Modules.HRM);

                    #endregion

                }
            }

            return Json(new[] { lateComming }.ToDataSourceResult(request, ModelState));
        }
    
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LateCommings_Update([DataSourceRequest]DataSourceRequest request, LateComming lateComming)
        {ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (DateAmharic == null)
                {
                    DateAmharic = lateComming.DateLateAmharic;
                    var splitDate = DateAmharic.Split('/');
                    Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
               
                if (Date.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Start Date should be less or equal to the current date!!");
                }
                if (lateComming.Time > 8)
                {
                    ModelState.AddModelError("", "Time Field  not proceed 8 hours");
                }
                else
                {
                    var entity = new LateComming
                    {
                        LateCommingId = lateComming.LateCommingId,
                        EmployeeId = lateComming.EmployeeId,
                        Date = Date,
                        DateLateAmharic = DateAmharic,
                        Time = lateComming.Time,
                        Remark = lateComming.Remark,
                        EmployeeCode =
                            inputs.GetEmployees(User.Identity.Name)
                                .Where(x => x.PersonId == lateComming.EmployeeId)
                                .FirstOrDefault()
                                .EmployeeCode
                    };

                    db.LateCommings.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    #region operation log

                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                   
                        UserHelper.OperationLog(Request.UserHostAddress, "Late Comming", "Success", User.Identity.Name, User.Identity.Name + " updated late comming info of " + User.Identity.Name + " at date " + DateTime.Today,Modules.HRM);

                    #endregion

                }
            }

            return Json(new[] { lateComming }.ToDataSourceResult(request, ModelState));
        }
  
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LateCommings_Destroy([DataSourceRequest]DataSourceRequest request, LateComming lateComming)
        {
            if (ModelState.IsValid)
            {
                var entity = new LateComming
                {
                    LateCommingId = lateComming.LateCommingId,
                    EmployeeId = lateComming.EmployeeId,
                    Date = lateComming.Date,
                    DateLateAmharic = lateComming.DateLateAmharic,
                    Time = lateComming.Time,
                    Remark = lateComming.Remark,
                    EmployeeCode = lateComming.EmployeeCode
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee late comming information of employee" + lateComming.EmployeeCode);
                #endregion
                db.LateCommings.Attach(entity);
                db.LateCommings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { lateComming }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
