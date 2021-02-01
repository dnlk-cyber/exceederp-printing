using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using ExceedERP.Core.Domain.Global;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    [Authorize(Roles = HR_Profile_Admin + "," + HR_Profile_User + "," + HR_Admin)]
    public class DelegationController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Profile_Admin = "HR_Profile_Admin";
        private const string HR_Profile_User = "HR_Profile_User";
        private const string HR_Admin = "HR_Admin";
        public static DateTime FromDate { get; set; }
        public static DateTime? ToDate { get; set; }
        public static string FromDateAm { get; set; }
        public static string ToDateAm { get; set; }
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            ViewData["CC"] = db.MainCompanyStructures.ToList();

            var periodList = db.PayrollPeriods.Where(x => x.Status == FiscalYearStatus.Open);
            var list = from p in periodList
                       join g in db.GLPeriods
                           on p.Name equals g.Name
                       select (new
                       {
                           g
                       });
            ViewData["PeriodList"] = db.GLPeriods.ToList();// list.Select(x => x.g).ToList(); ;
            var listt = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = listt.Where(x => x.IsActive);
            return View();
        }
        public ActionResult IndexForCC()
        {
            ViewData["CC"] = db.MainCompanyStructures.ToList();

            var periodList = db.PayrollPeriods.Where(x => x.Status == FiscalYearStatus.Open);
            var list = from p in periodList
                       join g in db.GLPeriods
                           on p.Name equals g.Name
                       select (new
                       {
                           g
                       });
            ViewData["PeriodList"] = db.GLPeriods.ToList();// list.Select(x => x.g).ToList(); ;
            var listt = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = listt.Where(x => x.IsActive);
            return View();
        }
        public void SetDate(string name, string value)
        {
            if (name == "FromDateTimeAm")
            {
                FromDateAm = value;
                var splitDate = FromDateAm.Split('/');
                FromDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "ToDateTimeAm")
            {
                ToDateAm = value;
                var splitDate = ToDateAm.Split('/');
                ToDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
        }
        [Authorize(Roles = HR_Profile_Admin + "," + HR_Profile_User + "," + HR_Admin)]
        public ActionResult Delegations_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<Delegation> delegations = db.Delegations.Where(x => employees.Contains(x.EmployeeId.ToString()));
            DataSourceResult result = delegations.ToDataSourceResult(request, delegation => new
            {
                DelegationId = delegation.DelegationId,
                FromDateTime = delegation.FromDateTime,
                ToDateTime = delegation.ToDateTime,
                FromDateTimeAm = delegation.FromDateTimeAm,
                ToDateTimeAm = delegation.ToDateTimeAm,
                EmployeeId = delegation.EmployeeId,
                FromEmployeeId = delegation.FromEmployeeId,
                Remark = delegation.Remark,
                OrgBranchName = delegation.OrgBranchName,
                DateCreated = delegation.DateCreated,
                LastModified = delegation.LastModified,
                CreatedBy = delegation.CreatedBy,
                ModifiedBy = delegation.ModifiedBy,
                //Reason = delegation.Reason,
                GLPeriodId = delegation.GLPeriodId
            });

            return Json(result);
        }
        public ActionResult DelegationsCC_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<Delegation> delegations = new List<Delegation>();
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var place = db.Placements.FirstOrDefault(x=>x.EmployeeId == empId && x.isCurrent);
            if(place != null)
            {               
                var delegationCC = db.DelegationCCs.Where(x => x.CC == place.OrgStructureId).Select(x=>x.DelegationId);

                 delegations = db.Delegations.Where(x=> delegationCC.Contains(x.DelegationId)).ToList();
            }
            DataSourceResult result = delegations.ToDataSourceResult(request, delegation => new
            {
                DelegationId = delegation.DelegationId,
                FromDateTime = delegation.FromDateTime,
                ToDateTime = delegation.ToDateTime,
                FromDateTimeAm = delegation.FromDateTimeAm,
                ToDateTimeAm = delegation.ToDateTimeAm,
                EmployeeId = delegation.EmployeeId,
                FromEmployeeId = delegation.FromEmployeeId,
                Remark = delegation.Remark,
                OrgBranchName = delegation.OrgBranchName,
                DateCreated = delegation.DateCreated,
                LastModified = delegation.LastModified,
                CreatedBy = delegation.CreatedBy,
                ModifiedBy = delegation.ModifiedBy,
                //Reason = delegation.Reason,
                GLPeriodId = delegation.GLPeriodId
            });

            return Json(result);
        }
        [Authorize(Roles = HR_Profile_User)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delegations_Create([DataSourceRequest]DataSourceRequest request, Delegation delegation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var checkexistancep = db.PayrollProcessViewModel.Where(x => x.GLPeriodId == delegation.GLPeriodId).FirstOrDefault();
                if (checkexistancep != null)
                {


                    ModelState.AddModelError("", "You cannot add this delegation. This period is already processed!!");
                }
                else
                {
                    if (delegation.FromEmployeeId == delegation.EmployeeId)
                    {
                        ModelState.AddModelError("", "The assigned employee should be diffretnt from Delegator!!");
                    }
                    //else if (FromDate.Date > ToDate.Date)
                    //{
                    //    ModelState.AddModelError("", "Please check the date range !!");
                    //}
                    else
                    {

                        var entity = new Delegation
                        {
                            FromDateTime = FromDate,
                            ToDateTime = ToDate,
                            FromDateTimeAm = FromDateAm,
                            ToDateTimeAm = ToDateAm,
                            EmployeeId = delegation.EmployeeId,
                            Remark = delegation.Remark,
                            FromEmployeeId = delegation.FromEmployeeId,
                            OrgBranchName = delegation.OrgBranchName,
                            DateCreated = DateTime.Now,
                            LastModified = delegation.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = delegation.ModifiedBy,
                           // Reason = delegation.Reason,
                            GLPeriodId = delegation.GLPeriodId
                        };

                        db.Delegations.Add(entity);
                        db.SaveChanges();
                        delegation.DelegationId = entity.DelegationId;

                        #region operation log
                        
                        UserHelper.OperationLog(Request.UserHostAddress, "Delegation", "Success", User.Identity.Name, User.Identity.Name + " add new Delegation at date" + DateTime.Today, Modules.HRM);

                        #endregion
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "delegation", "Success", User.Identity.Name, $"added delegation from  {entity.FromDateTime} to {entity.ToDateTime} ", Modules.Payroll);
                        #endregion

                    }
                }
            }
            return Json(new[] { delegation }.ToDataSourceResult(request, ModelState));
        }
        [Authorize(Roles = HR_Profile_User)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delegations_Update([DataSourceRequest]DataSourceRequest request, Delegation delegation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (FromDateAm == null)
                {
                    FromDateAm = delegation.FromDateTimeAm;
                    var splitDate = FromDateAm.Split('/');
                    FromDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (ToDateAm == null)
                {
                    ToDateAm = delegation.ToDateTimeAm;
                    var splitDate = ToDateAm.Split('/');
                    ToDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                var checkexistancep = db.PayrollProcessViewModel.Where(x => x.GLPeriodId == delegation.GLPeriodId).FirstOrDefault();
                if (checkexistancep != null)
                {


                    ModelState.AddModelError("", "You cannot add this delegation. This period is already processed!!");
                }
                else
                {
                    if (FromDate > ToDate)
                    {
                        ModelState.AddModelError("", "Please check the date range!!");
                    }
                    else
                    {
                        var entity = new Delegation
                        {
                            DelegationId = delegation.DelegationId,
                            FromDateTime = FromDate,
                            ToDateTime = ToDate,
                            FromDateTimeAm = FromDateAm,
                            ToDateTimeAm = ToDateAm,
                            EmployeeId = delegation.EmployeeId,
                            FromEmployeeId = delegation.FromEmployeeId,
                            Remark = delegation.Remark,
                            OrgBranchName = delegation.OrgBranchName,
                            DateCreated = delegation.DateCreated,
                            LastModified = DateTime.Now,
                            CreatedBy = delegation.CreatedBy,
                            ModifiedBy = User.Identity.Name,
                            //Reason = delegation.Reason,
                            GLPeriodId = delegation.GLPeriodId
                        };

                        db.Delegations.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                        db.SaveChanges();
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "delegation", "Success", User.Identity.Name, $"updated delegation from  {entity.FromDateTime} to {entity.ToDateTime} ", Modules.Payroll);
                        #endregion
                    }
                }
            }
            return Json(new[] { delegation }.ToDataSourceResult(request, ModelState));
        }
        [Authorize(Roles = HR_Profile_Admin + "," + HR_Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delegations_Destroy([DataSourceRequest]DataSourceRequest request, Delegation delegation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Delegation
                {
                    DelegationId = delegation.DelegationId,
                    FromDateTime = delegation.FromDateTime,
                    ToDateTime = delegation.ToDateTime,
                    FromDateTimeAm = delegation.FromDateTimeAm,
                    ToDateTimeAm = delegation.ToDateTimeAm,
                    EmployeeId = delegation.EmployeeId,
                    FromEmployeeId = delegation.FromEmployeeId,
                    Remark = delegation.Remark,
                    OrgBranchName = delegation.OrgBranchName,
                    DateCreated = delegation.DateCreated,
                    LastModified = delegation.LastModified,
                    CreatedBy = delegation.CreatedBy,
                    ModifiedBy = delegation.ModifiedBy,
                    //Reason = delegation.Reason,
                    GLPeriodId = delegation.GLPeriodId
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted delegation information of employee " + delegation.FromEmployeeId + " period" + delegation.GLPeriodId);
                #endregion
                db.Delegations.Attach(entity);
                db.Delegations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { delegation }.ToDataSourceResult(request, ModelState));
        }

        // ================================== Delegation CC ==============================================

        [Authorize(Roles = HR_Profile_Admin + "," + HR_Profile_User + "," + HR_Admin)]
        public ActionResult DelegationCC_Read([DataSourceRequest]DataSourceRequest request, int id)
        {

            IQueryable<DeligationCC> delegations = db.DelegationCCs.Where(x => x.DelegationId == id);
            DataSourceResult result = delegations.ToDataSourceResult(request, delegation => new
            {
                DelegationId = delegation.DelegationId,
                DeligationCCId = delegation.DeligationCCId,
                CC = delegation.CC,
            });

            return Json(result);
        }
        [Authorize(Roles = HR_Profile_User)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DelegationCC_Create([DataSourceRequest]DataSourceRequest request, DeligationCC delegation, int id)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new DeligationCC
                {
                    DelegationId = id,
                    CC = delegation.CC,
                };

                db.DelegationCCs.Add(entity);
                db.SaveChanges();
                delegation.DelegationId = entity.DelegationId;


            }
            return Json(new[] { delegation }.ToDataSourceResult(request, ModelState));
        }
        [Authorize(Roles = HR_Profile_User)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DelegationCC_Update([DataSourceRequest]DataSourceRequest request, DeligationCC delegation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new DeligationCC
                {
                    DelegationId = delegation.DelegationId,
                    DeligationCCId = delegation.DeligationCCId,
                    CC = delegation.CC,
                };
                db.DelegationCCs.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }


            return Json(new[] { delegation }.ToDataSourceResult(request, ModelState));
        }
        [Authorize(Roles = HR_Profile_Admin + "," + HR_Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DelegationCC_Destroy([DataSourceRequest]DataSourceRequest request, DeligationCC delegation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new DeligationCC
                {
                    DelegationId = delegation.DelegationId,
                    DeligationCCId = delegation.DeligationCCId,
                    CC = delegation.CC,
                };
                //#region delete log
                //UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted delegation information of employee " + delegation.FromEmployeeId + " period" + delegation.GLPeriodId);
                //#endregion
                db.DelegationCCs.Attach(entity);
                db.DelegationCCs.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { delegation }.ToDataSourceResult(request, ModelState));
        }
        // ================================== Delegation Reason ==============================================

        [Authorize(Roles = HR_Profile_Admin + "," + HR_Profile_User + "," + HR_Admin)]
        public ActionResult DelegationReason_Read([DataSourceRequest]DataSourceRequest request, int id)
        {

            IQueryable<DelegationReason> delegations = db.DelegationReasons.Where(x => x.DelegationId == id);
            DataSourceResult result = delegations.ToDataSourceResult(request, delegation => new
            {
                DelegationId = delegation.DelegationId,
                DelegationReasonId = delegation.DelegationReasonId,
                Reason = delegation.Reason,
            });

            return Json(result);
        }
        [Authorize(Roles = HR_Profile_User)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DelegationReason_Create([DataSourceRequest]DataSourceRequest request, DelegationReason delegation, int id)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new DelegationReason
                {
                    DelegationId = id,
                    Reason = delegation.Reason,
                };

                db.DelegationReasons.Add(entity);
                db.SaveChanges();
                delegation.DelegationId = entity.DelegationId;


            }
            return Json(new[] { delegation }.ToDataSourceResult(request, ModelState));
        }
        [Authorize(Roles = HR_Profile_User)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DelegationReason_Update([DataSourceRequest]DataSourceRequest request, DelegationReason delegation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new DelegationReason
                {
                    DelegationId = delegation.DelegationId,
                    DelegationReasonId = delegation.DelegationReasonId,
                    Reason = delegation.Reason,
                };

                db.DelegationReasons.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }


            return Json(new[] { delegation }.ToDataSourceResult(request, ModelState));
        }
        [Authorize(Roles = HR_Profile_Admin + "," + HR_Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DelegationReason_Destroy([DataSourceRequest]DataSourceRequest request, DelegationReason delegation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new DelegationReason
                {
                    DelegationId = delegation.DelegationId,
                    DelegationReasonId = delegation.DelegationReasonId,
                    Reason = delegation.Reason,
                };
                //#region delete log
                //UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted delegation information of employee " + delegation.FromEmployeeId + " period" + delegation.GLPeriodId);
                //#endregion
                db.DelegationReasons.Attach(entity);
                db.DelegationReasons.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { delegation }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
