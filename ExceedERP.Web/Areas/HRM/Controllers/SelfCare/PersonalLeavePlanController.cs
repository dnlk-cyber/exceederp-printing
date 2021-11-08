using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.HRM.HRMPlan;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HRLeaveUser,HrmConstants.HRLeaveAdmin,  HrmConstants.HRDepartmentHead)]
    public class PersonalLeavePlanController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime To { get; set; }
        public static string ToAmharic { get; set; }
        public static DateTime From { get; set; }
        static int EmployeeId { get; set; }
        public static string FromAmharic { get; set; }
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {

            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList.Select(p => new { GlFiscalYearId = p.GlFiscalYearId, Name = p.Name });
            EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            return View();
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
        
        [AuthorizeRoles(HrmConstants.HRLeaveUser, HrmConstants.HRLeaveAdmin, HrmConstants.HRAdmin, HrmConstants.HRDepartmentHead)]
        public ActionResult LeavePlans_Read([DataSourceRequest]DataSourceRequest request)
        {

            IQueryable<LeavePlan> leaveplans = db.LeavePlans.Where(x => x.EmployeeId == EmployeeId);
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
        
        [AuthorizeRoles(HrmConstants.HRLeaveUser, HrmConstants.HRDepartmentHead)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePlans_Create([DataSourceRequest]DataSourceRequest request, LeavePlan leavePlan)
        {
            ModelState.Clear();
            if (From.Date > To.Date)
            {
                ModelState.AddModelError("", "Please Check the date range!!");
            }
            else if (FromAmharic == null || ToAmharic == null)
            {
                ModelState.AddModelError("", "Date form and to values are mandatory!!");
            }

            else
            {
                var entitlew =
                    db.LeavesEntitle.Where(
                        x => x.GlFiscalYearId == leavePlan.GlFiscalYearId && x.EmployeeId == EmployeeId)
                        .ToList()
                        .Sum(x => x.Amount);
                var taken =
                    db.Leaves.Where(
                        x => x.GlFiscalYearId == leavePlan.GlFiscalYearId && x.EmployeeId == EmployeeId)
                        .ToList()
                        .Sum(x => x.Amount);
                //var tranferedList =
                //    db.LeaveTransfer.Where(x => x.To == leavePlan.GlFiscalYearId && x.EmployeeId == EmployeeId)
                //        .ToList()
                //        .Sum(x => x.Amount);
                var total = entitlew/* + tranferedList */- taken;
                if (leavePlan.LeaveAmount > total)
                {
                    ModelState.AddModelError("", "You can not plan such amount of leave!!");

                }
            }
            if (ModelState.IsValid)
            {




                var entity = new LeavePlan
                {

                    EmployeeId = EmployeeId,
                    GlFiscalYearId = leavePlan.GlFiscalYearId,
                    LeaveAmount = leavePlan.LeaveAmount,
                    From = From,
                    To = To,
                    Remark = leavePlan.Remark,
                    FromAmharic = FromAmharic,
                    ToAmharic = ToAmharic,
                    EmployeeCode = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == leavePlan.EmployeeId).EmployeeCode,
                    DateCreated = leavePlan.DateCreated,
                    LastModified = leavePlan.LastModified,
                    CreatedBy = leavePlan.CreatedBy,
                    ModifiedBy = leavePlan.ModifiedBy,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                };

                db.LeavePlans.Add(entity);
                db.SaveChanges();
                leavePlan.LeavePlanId = entity.LeavePlanId;

            }

            return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
        }
        
        [AuthorizeRoles(HrmConstants.HRLeaveUser,  HrmConstants.HRDepartmentHead)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePlans_Update([DataSourceRequest]DataSourceRequest request, LeavePlan leavePlan)
        {
            ModelState.Clear();
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
                        EmployeeCode = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == leavePlan.EmployeeId).EmployeeCode,
                        DateCreated = leavePlan.DateCreated,
                        LastModified = leavePlan.LastModified,
                        CreatedBy = leavePlan.CreatedBy,
                        ModifiedBy = leavePlan.ModifiedBy,
                        OrgBranchName = leavePlan.OrgBranchName,
                    };

                    db.LeavePlans.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
        }
      
        [AuthorizeRoles( HrmConstants.HRLeaveAdmin, HrmConstants.HRAdmin)]
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
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee leave Plan of employee" + leavePlan.EmployeeCode + " and fiscal year " + leavePlan.GlFiscalYear.Name);
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
