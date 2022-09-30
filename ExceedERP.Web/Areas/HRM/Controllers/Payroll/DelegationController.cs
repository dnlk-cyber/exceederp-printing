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
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Web.Models;
using Microsoft.AspNet.Identity.Owin;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    [Authorize(Roles = HR_Profile_Admin + "," + HR_Profile_User + "," + HR_Admin)]
    public class DelegationController : BaseController
    {
        private ApplicationRoleManager RoleManager;

        public DelegationController()
        {
            RoleManager = ApplicationRoleManager.RoleManager;
        }

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
                                                           // var listt = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = db.Person_Employee.ToList();
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
            // List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);

            var placements = db.Placements.Where(x => x.isCurrent).ToList();
            int branchId = UserInformationHelper.GetUserBranchByUserName(User.Identity.Name);

            var employees = placements.Where(x => x.BranchId == branchId).Select(x => x.EmployeeId);

            IQueryable<Delegation> delegations = db.Delegations;//.Where(x => employees.Contains(x.EmployeeId));

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
                GLPeriodId = delegation.GLPeriodId,
                IsOpen = delegation.IsOpen,
                HideButtons = CheckDate(delegation.DelegationId)
            });

            return Json(result);
        }
        public ActionResult Profile_Delegations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {

            IQueryable<Delegation> delegations = db.Delegations.Where(x => x.EmployeeId == id || x.FromEmployeeId == id);

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
                GLPeriodId = delegation.GLPeriodId,
                IsOpen = delegation.IsOpen,
                HideButtons = CheckDate(delegation.DelegationId)
            });

            return Json(result);
        }
        public bool CheckDate(int id)
        {
            var delegation = db.Delegations.FirstOrDefault(x => x.DelegationId == id);
            if (delegation != null)
            {
                if (delegation.ToDateTime < DateTime.Now)
                {
                    return true;
                }
            }
            return false;
        }
        public ActionResult DelegationsCC_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<Delegation> delegations = new List<Delegation>();
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var place = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);
            if (place != null)
            {
                var delegationCC = db.DelegationCCs.Where(x => x.CC == place.OrgStructureId).Select(x => x.DelegationId);

                delegations = db.Delegations.Where(x => delegationCC.Contains(x.DelegationId)).ToList();
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
            if (delegation.FromDateTimeAm != null)
            {
                var splitDate = delegation.FromDateTimeAm.Split('/');
                delegation.FromDateTime = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            if (delegation.ToDateTimeAm != null)
            {
                var splitDate = delegation.ToDateTimeAm.Split('/');
                delegation.FromDateTime = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            var checkexistancep = db.PayrollProcessViewModel.FirstOrDefault(x => x.GLPeriodId == delegation.GLPeriodId);
            if (checkexistancep != null)
            {
                ModelState.AddModelError("", "You cannot add this delegation. This period is already processed!!");
            }
            if (delegation.FromEmployeeId == delegation.EmployeeId)
            {
                ModelState.AddModelError("", "The assigned employee should be diffretnt from Delegator!!");
            }
            if (ModelState.IsValid)
            {

                var entity = new Delegation
                {
                    FromDateTime = delegation.FromDateTime,
                    ToDateTime = delegation.ToDateTime,
                    FromDateTimeAm = delegation.FromDateTimeAm,
                    ToDateTimeAm = delegation.ToDateTimeAm,
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
            return Json(new[] { delegation }.ToDataSourceResult(request, ModelState));
        }
        [Authorize(Roles = HR_Profile_User)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delegations_Update([DataSourceRequest]DataSourceRequest request, Delegation delegation)
        {
            ModelState.Clear();

            if (delegation.FromDateTimeAm != null)
            {
                var splitDate = delegation.FromDateTimeAm.Split('/');
                delegation.FromDateTime = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            if (delegation.ToDateTimeAm != null)
            {
                var splitDate = delegation.ToDateTimeAm.Split('/');
                delegation.FromDateTime = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            var checkexistancep = db.PayrollProcessViewModel.FirstOrDefault(x => x.GLPeriodId == delegation.GLPeriodId);
            if (checkexistancep != null)
            {
                ModelState.AddModelError("", "You cannot add this delegation. This period is already processed!!");
            }
            if (delegation.FromEmployeeId == delegation.EmployeeId)
            {
                ModelState.AddModelError("", "The assigned employee should be diffretnt from Delegator!!");
            }
            var checkExit = db.PayrollProcessViewModel.FirstOrDefault(x => x.GLPeriodId == delegation.GLPeriodId);
            if (checkExit != null)
            {
                ModelState.AddModelError("", "You cannot add this delegation. This period is already processed!!");
            }
            if (delegation.FromDateTime > delegation.ToDateTime)
            {
                ModelState.AddModelError("", "Please check the date range!!");
            }
            if (ModelState.IsValid)
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
        private ApplicationGroupManager _groupManager;
        public ApplicationGroupManager GroupManager
        {
            get
            {
                return _groupManager ?? new ApplicationGroupManager();
            }
            private set
            {
                _groupManager = value;
            }
        }

        public ActionResult OpenDelegation(int id)
        {
            ApplicationDbContext adb = new ApplicationDbContext();

            var delegation = db.Delegations.FirstOrDefault(x => x.DelegationId == id);

            if (delegation != null)
            {
                delegation.IsOpen = true;
                delegation.FromDateTime = DateTime.Now;
                delegation.FromDateTimeAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);

                db.Entry(delegation).State = EntityState.Modified;
                db.SaveChanges();

                var placementFrom = db.Placements.FirstOrDefault(x => x.EmployeeId == delegation.FromEmployeeId && x.isCurrent);
                if (placementFrom != null && placementFrom.IsHead)
                {
                    placementFrom.IsHead = false;

                    db.Entry(placementFrom).State = EntityState.Modified;
                    db.SaveChanges();

                    var placementTo = db.Placements.FirstOrDefault(x => x.EmployeeId == delegation.EmployeeId && x.isCurrent);
                    if (placementTo != null)
                    {
                        placementTo.IsHead = true;

                        db.Entry(placementTo).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                var user = adb.Users.FirstOrDefault(x => x.EmployeeId == delegation.FromEmployeeId);
                if (user != null)
                {
                    List<string> roleNames = new List<string>();
                    var toUser = adb.Users.FirstOrDefault(x => x.EmployeeId == delegation.EmployeeId);
                    if (toUser != null)
                    {
                        foreach (var rm in user.Roles)
                        {
                            var chek = toUser.Roles.FirstOrDefault(x => x.RoleId == rm.RoleId);
                            if (chek == null)
                            {
                                var identityRole = adb.Roles.Find(rm.RoleId);

                                if (identityRole != null)
                                {
                                    roleNames.Add(identityRole.Name);
                                }
                            }
                        }


                        //  var _userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

                        ApplicationUserManager _userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();


                        _userManager.AddToRolesAsync(toUser.Id, roleNames.ToArray());

                        //  _userManager.AddToRolesAsync(toUser.Id, rolesArray);

                        foreach (var tou in roleNames)
                        {
                            _userManager.AddToRoleAsync(toUser.Id, tou);

                            var tempo = new DelegationTempoRole();

                            tempo.FromUserId = user.Id;
                            tempo.ToUserId = toUser.Id;
                            tempo.RoleName = tou;

                            db.DelegationTempoRoles.Add(tempo);
                            db.SaveChanges();
                        }
                    }
                }

                var formattedData = new
                {
                    Message = "You successfully open this delegation .",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            var formattedData1 = new
            {
                Message = "the employee is not open.",
                Status = "FAIL"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);

        }
        public ActionResult CloseDelegation(int id)
        {
            ApplicationDbContext adb = new ApplicationDbContext();
            var delegation = db.Delegations.FirstOrDefault(x => x.DelegationId == id);

            if (delegation != null)
            {
                delegation.IsOpen = false;
                delegation.ToDateTime = DateTime.Now;
                delegation.ToDateTimeAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);

                db.Entry(delegation).State = EntityState.Modified;
                db.SaveChanges();

                var placementTo = db.Placements.FirstOrDefault(x => x.EmployeeId == delegation.EmployeeId && x.isCurrent);
                if (placementTo != null && placementTo.IsHead)
                {
                    placementTo.IsHead = false;

                    db.Entry(placementTo).State = EntityState.Modified;
                    db.SaveChanges();

                    var placementFrom = db.Placements.FirstOrDefault(x => x.EmployeeId == delegation.FromEmployeeId && x.isCurrent);
                    if (placementFrom != null)
                    {
                        placementFrom.IsHead = true;

                        db.Entry(placementFrom).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }


                var user = adb.Users.FirstOrDefault(x => x.EmployeeId == delegation.FromEmployeeId);
                if (user != null)
                {
                    //List<string> roleNames = new List<string>();

                    //foreach (var rm in user.Roles)
                    //{
                    //    var identityRole = adb.Roles.Find(rm.RoleId);
                    //    if (identityRole != null)
                    //    {
                    //        roleNames.Add(identityRole.Name);
                    //    }
                    //}

                    //var rolesArray = roleNames.ToArray();

                    var toUser = adb.Users.FirstOrDefault(x => x.EmployeeId == delegation.EmployeeId);
                    if (toUser != null)
                    {
                        var tempoRoles = db.DelegationTempoRoles.Where(x => x.FromUserId == user.Id && x.ToUserId == toUser.Id).ToList();

                        List<string> roleNames = new List<string>();

                        foreach (var temp in tempoRoles)
                        {
                            roleNames.Add(temp.RoleName);
                        }

                        var rolesArray = roleNames.ToArray();

                        var _userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

                        _userManager.RemoveFromRolesAsync(toUser.Id, rolesArray);

                        foreach (var tm in tempoRoles)
                        {
                            db.Entry(tm).State = EntityState.Deleted;
                            db.SaveChanges();
                        }

                        //db.Entry(tempoRoles).State = EntityState.Deleted;
                        //db.SaveChanges();
                    }
                }

                var formattedData = new
                {
                    Message = "You successfully close this delegation .",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            var formattedData1 = new
            {
                Message = "the employee is not closed.",
                Status = "FAIL"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }

    public class ToEmpRoleVM
    {
        public string RoleId { get; set; }
        public string UserId { get; set; }
    }
}
