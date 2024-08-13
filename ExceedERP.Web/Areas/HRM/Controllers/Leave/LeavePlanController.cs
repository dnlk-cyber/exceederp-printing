using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.HRMPlan;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{
    [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
    public class LeavePlanController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime To { get; set; }
        public static string ToAmharic { get; set; }
        public static DateTime From { get; set; }
        public static string FromAmharic { get; set; }
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {

            GetPeriod();
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
            if (name == "FromAmharic")
            {
                FromAmharic = value;
                var splitDate = FromAmharic.Split('/');
                From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "ToAmharic")
            {
                ToAmharic = value;
                var splitDate = ToAmharic.Split('/');
                To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }


        }
        public JsonResult GetPeriod()
        {
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(periodList.Select(p => new { GlFiscalYearId = p.GlFiscalYearId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
    
        public ActionResult LeavePlans_Read([DataSourceRequest]DataSourceRequest request)
        {
            var employeeid = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            List<int> employeeids = EmployeeHelper.GetSubordinates_FilteredByBranchAlso(employeeid);
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
           // List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<LeavePlan> leaveplans = db.LeavePlans.Where(x => employeeids.Contains(x.EmployeeId));
            DataSourceResult result = leaveplans.ToDataSourceResult(request, leavePlan => new
            {
                LeavePlanId = leavePlan.LeavePlanId,
                EmployeeId = leavePlan.EmployeeId,
                GlFiscalYearId = leavePlan.GlFiscalYearId,
                From = leavePlan.From,
                To = leavePlan.To,
                LeaveAmount = leavePlan.LeaveAmount,
                Remark = leavePlan.Remark,
                FromAmharic = leavePlan.FromAmharic,
                ToAmharic = leavePlan.ToAmharic,
                EmployeeCode = leavePlan.EmployeeCode,
                DateCreated = leavePlan.DateCreated,
                LastModified = leavePlan.LastModified,
                CreatedBy = leavePlan.CreatedBy,
                ModifiedBy = leavePlan.ModifiedBy,
                OrgBranchName = leavePlan.OrgBranchName,
            });

            return Json(result);
        }

      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePlans_Create([DataSourceRequest]DataSourceRequest request, LeavePlan leavePlan)
        {
            ModelState.Clear();

            var year = db.GLFiscalYears.Find(leavePlan.GlFiscalYearId);
            int selectedToYear = year.DateTo.Year;
            int selectedFromYear = year.DateFrom.Year;
            int currentYear = DateTime.Now.Year;
            var splitDateFrom = leavePlan.FromAmharic.Split('/');
            From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateFrom[0]), Convert.ToInt32(splitDateFrom[1]), Convert.ToInt32(splitDateFrom[2]));
            var splitDateTo = leavePlan.ToAmharic.Split('/');
            To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateTo[0]), Convert.ToInt32(splitDateTo[1]), Convert.ToInt32(splitDateTo[2]));

            var date = db.LeavePlans.Where(x => x.EmployeeId == leavePlan.EmployeeId && x.From >= From && x.From <= To);
            var date2 = db.LeavePlans.Where(x => x.EmployeeId == leavePlan.EmployeeId && x.To >= From && x.To <= To);

            if (From.Date > To.Date)
            {
                ModelState.AddModelError("", "Please Check the date range!!");
            }
            if (FromAmharic == null || ToAmharic == null)
            {
                ModelState.AddModelError("", "Date form and to values are mandatory!!");
            }
            if ((selectedFromYear < currentYear && selectedToYear < currentYear) || (selectedFromYear > currentYear && selectedToYear > currentYear))
            {
                ModelState.AddModelError("", "You can not Plan for other years");
            }
            if (date.Any() || date2.Any())
            {
                ModelState.AddModelError("", "This range of Date is arranged for other plan");
            }


            if (ModelState.IsValid)
            {



                int resultDate = To.Subtract(From).Days;
                //var years = db.GLFiscalYears.Find(leavePlan.GlFiscalYearId);
                //int year = Convert.ToInt32(years.Name);

                //var entitlew = db.LeavesEntitle.Where(x => x.GlFiscalYearId == leavePlan.GlFiscalYearId && x.EmployeeId == leavePlan.EmployeeId).ToList().Sum(x => x.Amount);
                //var taken = db.Leaves.Where(x => x.GlFiscalYearId == leavePlan.GlFiscalYearId && x.EmployeeId == leavePlan.EmployeeId).ToList().Sum(x => x.Amount);
                //var tranferedList = db.LeaveTransfer.Where(x => x.To == leavePlan.GlFiscalYearId && x.EmployeeId == leavePlan.EmployeeId).ToList().Sum(x => x.Amount);
                //var total = entitlew + tranferedList - taken;

                var totalSum = db.LeavesEntitle.Where(x=>!x.IsExpired && x.EmployeeId == leavePlan.EmployeeId).ToList();
                var totalBalance = totalSum.Sum(x=>x.Balance);
                if (leavePlan.LeaveAmount > totalBalance)
                {
                    //string messages = "You can not plan such amount of leave!!";
                    //return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                    ModelState.AddModelError("", "You can not plan such amount of leave!!");
                }
                else if (leavePlan.LeaveAmount > resultDate)
                {
                    ModelState.AddModelError("", "Leave Date cannot be out of range");
                }
                else
                {
                    var entity = new LeavePlan
                    {

                        EmployeeId = leavePlan.EmployeeId,
                        GlFiscalYearId = leavePlan.GlFiscalYearId,
                        LeaveAmount = leavePlan.LeaveAmount,
                        From = From,
                        To = To,
                        Remark = leavePlan.Remark,
                        FromAmharic = FromAmharic,
                        ToAmharic = ToAmharic,
                        EmployeeCode = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == leavePlan.EmployeeId)?.EmployeeCode,
                        DateCreated = leavePlan.DateCreated,
                        LastModified = leavePlan.LastModified,
                        CreatedBy = leavePlan.CreatedBy,
                        ModifiedBy = leavePlan.ModifiedBy,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    };

                    db.LeavePlans.Add(entity);
                    db.SaveChanges();
                    leavePlan.LeavePlanId = entity.LeavePlanId;

                    #region operation log
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                    if (person != null)
                    {
                        var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                        
                            UserHelper.OperationLog(Request.UserHostAddress, "Leave Plan", "Success", User.Identity.Name, "Leave Plan for " + person.EmployeeCode + " is Created By  " + User.Identity.Name,Modules.HRM);
                        
                    }
                    #endregion
                }

            }

            return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
        }

      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePlans_Update([DataSourceRequest]DataSourceRequest request, LeavePlan leavePlan)
        {
            ModelState.Clear();
            //var years = db.GLFiscalYears.Find(leavePlan.GlFiscalYearId);
            //int year = Convert.ToInt32(years.Name);
            int resultDate = To.Subtract(From).Days;
            //var entitlew = db.LeavesEntitle.Where(x => x.GlFiscalYearId == leavePlan.GlFiscalYearId && x.EmployeeId == leavePlan.EmployeeId).ToList().Sum(x => x.Amount);
            //var taken = db.Leaves.Where(x => x.GlFiscalYearId == leavePlan.GlFiscalYearId && x.EmployeeId == leavePlan.EmployeeId).ToList().Sum(x => x.Amount);
            //var tranferedList = db.LeaveTransfer.Where(x => x.To == leavePlan.GlFiscalYearId && x.EmployeeId == leavePlan.EmployeeId).ToList().Sum(x => x.Amount);
            //var total = entitlew + tranferedList - taken;

            var totalSum = db.LeavesEntitle.Where(x => !x.IsExpired && x.EmployeeId == leavePlan.EmployeeId).ToList();
            var totalBalance = totalSum.Sum(x => x.Balance);
            if (leavePlan.LeaveAmount > totalBalance)
            {
                //string messages = "You can not plan such amount of leave!!";
                //return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                ModelState.AddModelError("", "You can not plan such amount of leave!!");
            }
            else if (leavePlan.LeaveAmount > resultDate)
            {
                ModelState.AddModelError("", "Leave Date cannot be out of range");
            }
            if (ModelState.IsValid)
            {
                if (FromAmharic == null)
                {
                    FromAmharic = leavePlan.FromAmharic;
                    var splitDate = FromAmharic.Split('/');
                    From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (ToAmharic == null)
                {
                    ToAmharic = leavePlan.ToAmharic;
                    var splitDate = FromAmharic.Split('/');
                    To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (From.Date > To.Date)
                {
                    ModelState.AddModelError("", "Please Check the date range!!");
                }
                else
                {
                    var entity = new LeavePlan
                    {
                        LeavePlanId = leavePlan.LeavePlanId,
                        EmployeeId = leavePlan.EmployeeId,
                        GlFiscalYearId = leavePlan.GlFiscalYearId,
                        From = From,
                        To = To,
                        LeaveAmount = leavePlan.LeaveAmount,
                        Remark = leavePlan.Remark,
                        FromAmharic = FromAmharic,
                        ToAmharic = ToAmharic,
                        EmployeeCode = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == leavePlan.EmployeeId)?.EmployeeCode,
                        DateCreated = leavePlan.DateCreated,
                        LastModified = leavePlan.LastModified,
                        CreatedBy = leavePlan.CreatedBy,
                        ModifiedBy = leavePlan.ModifiedBy,
                        OrgBranchName = leavePlan.OrgBranchName,
                    };

                    db.LeavePlans.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    #region operation log
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                    if (person != null)
                    {
                        var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                       
                            UserHelper.OperationLog(Request.UserHostAddress, "Leave Plan", "Success", User.Identity.Name, "Leave Plan of "+person.EmployeeCode +" is Updated By  " + User.Identity.Name,Modules.HRM);
                        
                    }
                    #endregion
                }
            }

            return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
        }
  
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePlans_Destroy([DataSourceRequest]DataSourceRequest request, LeavePlan leavePlan)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new LeavePlan
                {
                    LeavePlanId = leavePlan.LeavePlanId,
                    EmployeeId = leavePlan.EmployeeId,
                    GlFiscalYearId = leavePlan.GlFiscalYearId,
                    From = leavePlan.From,
                    To = leavePlan.To,
                    LeaveAmount = leavePlan.LeaveAmount,
                    Remark = leavePlan.Remark,
                    FromAmharic = leavePlan.FromAmharic,
                    ToAmharic = leavePlan.ToAmharic,
                    EmployeeCode = leavePlan.EmployeeCode,
                    DateCreated = leavePlan.DateCreated,
                    LastModified = leavePlan.LastModified,
                    CreatedBy = leavePlan.CreatedBy,
                    ModifiedBy = leavePlan.ModifiedBy,
                    OrgBranchName = leavePlan.OrgBranchName,
                };
                #region delete log
                var leaves = db.GLFiscalYears.Find(leavePlan.GlFiscalYearId);
                var delete = leaves.Name;
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee leave Plan of employee" + leavePlan.EmployeeCode + " and fiscal year " + delete);
                #endregion
                db.LeavePlans.Attach(entity);
                db.LeavePlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported employee leave plan");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported employee leave plan");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
