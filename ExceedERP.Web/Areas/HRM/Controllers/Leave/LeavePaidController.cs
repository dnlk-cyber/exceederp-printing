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
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{

    [AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRAdmin, HrmConstants.HRLeaveUser)]
    public class LeavePaidController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
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
        private JsonResult GetPeriod()
        {
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(periodList.Select(p => new { GlFiscalYearId = p.GlFiscalYearId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
      
        [AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRAdmin, HrmConstants.HRLeaveUser)]
        public ActionResult LeavePaid_Read([DataSourceRequest]DataSourceRequest request)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<LeavePaid> leavepaid = db.LeavePaid.Where(x => employees.Contains(x.EmployeeId.ToString()));
            DataSourceResult result = leavepaid.ToDataSourceResult(request, leavePaid => new
            {
                LeavePaidId = leavePaid.LeavePaidId,
                EmployeeId = leavePaid.EmployeeId,
                GlFiscalYearId = leavePaid.GlFiscalYearId,
                Amount = leavePaid.Amount,
                Remark = leavePaid.Remark,
                DateCreated = leavePaid.DateCreated,
                LastModified = leavePaid.LastModified,
                CreatedBy = leavePaid.CreatedBy,
                ModifiedBy = leavePaid.ModifiedBy,
                EmployeeCode = leavePaid.EmployeeCode,
                OrgBranchName = leavePaid.OrgBranchName
            });

            return Json(result);
        }
      
        [AuthorizeRoles( HrmConstants.HRLeaveUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePaid_Create([DataSourceRequest]DataSourceRequest request, LeavePaid leavePaid)
        {
            ModelState.Clear();
            var leaveEntitleBalance = db.LeavesEntitle.Where(x => !x.IsExpired && x.EmployeeId == leavePaid.EmployeeId && x.GlFiscalYearId == leavePaid.GlFiscalYearId).ToList();
            if (leaveEntitleBalance.Count() > 0)
            {
                if ((decimal)leavePaid.Amount > leaveEntitleBalance.Sum(x => x.Balance))
                {
                    ModelState.AddModelError("exist", "the amount you entered is more than your balance which is =" + leaveEntitleBalance.Sum(x => x.Balance));
                }
            }
            else
            {
                decimal maximumValue = 0;
                var maximumLeave = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_LEAVE_ENTITLEMENT.ToString());
                if (maximumLeave != null)
                {
                    maximumValue = Convert.ToInt32(maximumLeave.CurrentValue);
                }
                if ((decimal)leavePaid.Amount > maximumValue)
                {
                    ModelState.AddModelError("exist", "the amount you entered is more than maximum  Leave Value is =" + maximumValue);
                }
            }

            if (ModelState.IsValid)
            {
                var entity = new LeavePaid
                {
                    EmployeeId = leavePaid.EmployeeId,
                    GlFiscalYearId = leavePaid.GlFiscalYearId,
                    Amount = leavePaid.Amount,
                    Remark = leavePaid.Remark,
                    DateCreated = DateTime.Now,
                    LastModified = leavePaid.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = leavePaid.ModifiedBy,
                    EmployeeCode = inputs.GetEmployees().Where(x => x.PersonId == leavePaid.EmployeeId).FirstOrDefault().EmployeeCode,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
                };

                db.LeavePaid.Add(entity);
                db.SaveChanges();
                leavePaid.LeavePaidId = entity.LeavePaidId;

                decimal dateAsked = Convert.ToDecimal(leavePaid.Amount);
                decimal dateLeft = 0;
                do
                {

                    List<LeaveModel> leaveList = new List<LeaveModel>();
                    var yearLists = db.LeavesEntitle.Where(x => !x.IsExpired && x.EmployeeId == leavePaid.EmployeeId).ToList();



                    foreach (var year in yearLists)
                    {
                        var leavel = new LeaveModel();
                        var gl = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == year.GlFiscalYearId);
                        if (gl != null)
                        {
                            leavel.Year = gl.DateTo;
                        }

                        leaveList.Add(leavel);
                    }
                    var smallestDate = leaveList.Min(x => x.Year);
                    if (smallestDate != null)
                    {
                        var id = db.GLFiscalYears.FirstOrDefault(x => x.DateTo.Year == smallestDate.Year);
                        if (id != null)
                        {
                            var entiteledEmployee = db.LeavesEntitle.FirstOrDefault
                                (x => x.EmployeeId == leavePaid.EmployeeId && x.GlFiscalYearId == id.GlFiscalYearId && !x.IsExpired);
                            if (entiteledEmployee != null)
                            {
                                if (entiteledEmployee.Balance < dateAsked)
                                {
                                    dateLeft = dateAsked - entiteledEmployee.Balance;
                                    if (dateLeft >= 0)
                                    {
                                        entiteledEmployee.Balance = 0;
                                        entiteledEmployee.IsExpired = true;

                                        db.Entry(entiteledEmployee).State = EntityState.Modified;
                                        db.SaveChanges();

                                        dateAsked = dateLeft;
                                    }
                                }

                                else
                                {
                                    dateLeft = entiteledEmployee.Balance - dateAsked;
                                    if (dateLeft >= 0)
                                    {
                                        entiteledEmployee.Balance = dateLeft;

                                        if (dateLeft == 0)
                                        {
                                            entiteledEmployee.IsExpired = true;
                                        }

                                        db.Entry(entiteledEmployee).State = EntityState.Modified;
                                        db.SaveChanges();

                                        dateLeft = 0;
                                    }
                                }
                            }
                        }
                    }


                } while (dateLeft > 0);

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                   
                        UserHelper.OperationLog(Request.UserHostAddress, "Leave Paid", "Success", User.Identity.Name, "Leave Paid of " + person.EmployeeCode + " is Created By  " + User.Identity.Name,Modules.HRM);
                    
                }
                #endregion
            }

            return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
        }
      
        [AuthorizeRoles(HrmConstants.HRLeaveUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePaid_Update([DataSourceRequest]DataSourceRequest request, LeavePaid leavePaid)
        {
            ModelState.Clear();
            var leaveEntitleBalance = db.LeavesEntitle.Where(x => !x.IsExpired && x.EmployeeId == leavePaid.EmployeeId && x.GlFiscalYearId == leavePaid.GlFiscalYearId).ToList();
            if (leaveEntitleBalance.Count() > 0)
            {
                if ((decimal)leavePaid.Amount > leaveEntitleBalance.Sum(x=>x.Balance))
                {
                    ModelState.AddModelError("exist", "the amount you entered is more than your balance which is =" + leaveEntitleBalance.Sum(x => x.Balance));
                }
            }
            else
            {
                decimal maximumValue = 0;
                var maximumLeave = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_LEAVE_ENTITLEMENT.ToString());
                if (maximumLeave != null)
                {
                    maximumValue = Convert.ToInt32(maximumLeave.CurrentValue);
                }
                if ((decimal)leavePaid.Amount > maximumValue)
                {
                    ModelState.AddModelError("exist", "the amount you entered is more than maximum  Leave Value is =" + maximumValue);
                }
            }
            
            if (ModelState.IsValid)
            {
                var entity = new LeavePaid
                {
                    LeavePaidId = leavePaid.LeavePaidId,
                    EmployeeId = leavePaid.EmployeeId,
                    GlFiscalYearId = leavePaid.GlFiscalYearId,
                    Amount = leavePaid.Amount,
                    Remark = leavePaid.Remark,
                    DateCreated = leavePaid.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = leavePaid.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    EmployeeCode = inputs.GetEmployees().Where(x => x.PersonId == leavePaid.EmployeeId).FirstOrDefault().EmployeeCode,
                    OrgBranchName = leavePaid.OrgBranchName
                };

                db.LeavePaid.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                   
                        UserHelper.OperationLog(Request.UserHostAddress, "Leave Paid", "Success", User.Identity.Name, "Leave Paid of " + person.EmployeeCode + " is Updated By  " + User.Identity.Name,Modules.HRM);

                }
                #endregion
            }

            return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRAdmin)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePaid_Destroy([DataSourceRequest]DataSourceRequest request, LeavePaid leavePaid)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new LeavePaid
                {
                    LeavePaidId = leavePaid.LeavePaidId,
                    EmployeeId = leavePaid.EmployeeId,
                    GlFiscalYearId = leavePaid.GlFiscalYearId,
                    Amount = leavePaid.Amount,
                    Remark = leavePaid.Remark,
                    DateCreated = leavePaid.DateCreated,
                    LastModified = leavePaid.LastModified,
                    CreatedBy = leavePaid.CreatedBy,
                    ModifiedBy = leavePaid.ModifiedBy,
                    EmployeeCode = leavePaid.EmployeeCode,
                    OrgBranchName = leavePaid.OrgBranchName
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee leave paid in cash  of employee" + leavePaid.EmployeeCode + " and fiscal year " + leavePaid.GlFiscalYear.Name);
                #endregion
                db.LeavePaid.Attach(entity);
                db.LeavePaid.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { leavePaid }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported employee leave paid in cash");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported employee leave paid in cash");
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
