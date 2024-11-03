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
using ExceedERP.Web.Models;
using Microsoft.AspNet.Identity.Owin;
using ExceedERP.Core.Domain;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
    public class DelegationSelfcaresController : BaseController
    {
        private ApplicationRoleManager RoleManager;

        public DelegationSelfcaresController()
        {
            RoleManager = ApplicationRoleManager.RoleManager;
        }

        private ExceedDbContext db = new ExceedDbContext();
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
            ViewData["PeriodList"] = db.GLPeriods.ToList();
                                                           
            ViewData["EmployeeList"] = db.Person_Employee.ToList();
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
        public ActionResult Delegations_Read([DataSourceRequest]DataSourceRequest request)
        {
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            //var subordinates = EmployeeHelper.GetSubordinates_FilteredByBranchAlso(id);
            var subordinates = EmployeeHelper.GetSubordinates_FilteredByHead(id);

            IQueryable<Delegation> delegations = db.Delegations.Where(x => subordinates.Contains(x.EmployeeId));

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
               // GLPeriodId = delegation.GLPeriodId,
                IsOpen = delegation.IsOpen,
                HideButtons = CheckDate(delegation.DelegationId)
            });

            return Json(result);
        }
        //public ActionResult Delegations_Hr_Read([DataSourceRequest]DataSourceRequest request)
        //{
        //    IQueryable<Delegation> delegations = db.Delegations;//.Where(x => employees.Contains(x.EmployeeId));

        //    DataSourceResult result = delegations.ToDataSourceResult(request, delegation => new
        //    {
        //        DelegationId = delegation.DelegationId,
        //        FromDateTime = delegation.FromDateTime,
        //        ToDateTime = delegation.ToDateTime,
        //        FromDateTimeAm = delegation.FromDateTimeAm,
        //        ToDateTimeAm = delegation.ToDateTimeAm,
        //        EmployeeId = delegation.EmployeeId,
        //        FromEmployeeId = delegation.FromEmployeeId,
        //        Remark = delegation.Remark,
        //        OrgBranchName = delegation.OrgBranchName,
        //        DateCreated = delegation.DateCreated,
        //        LastModified = delegation.LastModified,
        //        CreatedBy = delegation.CreatedBy,
        //        ModifiedBy = delegation.ModifiedBy,
        //        //Reason = delegation.Reason,
        //        GLPeriodId = delegation.GLPeriodId,
        //        IsOpen = delegation.IsOpen,
        //        HideButtons = CheckDate(delegation.DelegationId)
        //    });

        //    return Json(result);
        //}
        //public ActionResult Profile_Delegations_Read([DataSourceRequest]DataSourceRequest request, int id)
        //{

        //    IQueryable<Delegation> delegations = db.Delegations.Where(x => x.EmployeeId == id || x.FromEmployeeId == id);

        //    DataSourceResult result = delegations.ToDataSourceResult(request, delegation => new
        //    {
        //        DelegationId = delegation.DelegationId,
        //        FromDateTime = delegation.FromDateTime,
        //        ToDateTime = delegation.ToDateTime,
        //        FromDateTimeAm = delegation.FromDateTimeAm,
        //        ToDateTimeAm = delegation.ToDateTimeAm,
        //        EmployeeId = delegation.EmployeeId,
        //        FromEmployeeId = delegation.FromEmployeeId,
        //        Remark = delegation.Remark,
        //        OrgBranchName = delegation.OrgBranchName,
        //        DateCreated = delegation.DateCreated,
        //        LastModified = delegation.LastModified,
        //        CreatedBy = delegation.CreatedBy,
        //        ModifiedBy = delegation.ModifiedBy,
        //        //Reason = delegation.Reason,
        //        GLPeriodId = delegation.GLPeriodId,
        //        IsOpen = delegation.IsOpen,
        //        HideButtons = CheckDate(delegation.DelegationId)
        //    });

        //    return Json(result);
        //}
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
        ////public ActionResult DelegationsCC_Read([DataSourceRequest]DataSourceRequest request)
        ////{
        ////    List<Delegation> delegations = new List<Delegation>();
        ////    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
        ////    var place = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);
        ////    if (place != null)
        ////    {
        ////        var delegationCC = db.DelegationCCs.Where(x => x.CC == place.OrgStructureId).Select(x => x.DelegationId);

        ////        delegations = db.Delegations.Where(x => delegationCC.Contains(x.DelegationId)).ToList();
        ////    }
        ////    DataSourceResult result = delegations.ToDataSourceResult(request, delegation => new
        ////    {
        ////        DelegationId = delegation.DelegationId,
        ////        FromDateTime = delegation.FromDateTime,
        ////        ToDateTime = delegation.ToDateTime,
        ////        FromDateTimeAm = delegation.FromDateTimeAm,
        ////        ToDateTimeAm = delegation.ToDateTimeAm,
        ////        EmployeeId = delegation.EmployeeId,
        ////        FromEmployeeId = delegation.FromEmployeeId,
        ////        Remark = delegation.Remark,
        ////        OrgBranchName = delegation.OrgBranchName,
        ////        DateCreated = delegation.DateCreated,
        ////        LastModified = delegation.LastModified,
        ////        CreatedBy = delegation.CreatedBy,
        ////        ModifiedBy = delegation.ModifiedBy,
        ////        //Reason = delegation.Reason,
        ////        GLPeriodId = delegation.GLPeriodId
        ////    });

        ////    return Json(result);
        ////}
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delegations_Create([DataSourceRequest]DataSourceRequest request, Delegation delegation)
        {
            ModelState.Clear();
            if (string.IsNullOrEmpty(delegation.FromDateTimeAm) || string.IsNullOrEmpty(delegation.ToDateTimeAm))
            {
                ModelState.AddModelError("", "please enter both From and To dates are Mandatory!!");
                return Json(new[] { delegation }.ToDataSourceResult(request, ModelState));
            }

            if (!string.IsNullOrEmpty(delegation.FromDateTimeAm))
            {
                var splitDate = delegation.FromDateTimeAm.Split('/');
                delegation.FromDateTime = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

            if (!string.IsNullOrEmpty(delegation.ToDateTimeAm))
            {
                var splitDate = delegation.ToDateTimeAm.Split('/');
                delegation.ToDateTime = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            if (delegation.FromDateTime > delegation.ToDateTime)
            {
                ModelState.AddModelError("", "Please check the date range!!");
                return Json(new[] { delegation }.ToDataSourceResult(request, ModelState));

            }
            if (delegation.FromEmployeeId == delegation.EmployeeId)
            {
                ModelState.AddModelError("", "The assigned employee should be diffretnt from Delegator!!");
                return Json(new[] { delegation }.ToDataSourceResult(request, ModelState));

            }

            if (db.Delegations.Any(x => x.FromEmployeeId == delegation.FromEmployeeId
                                        && x.FromDateTime >= delegation.FromDateTime
                                        && x.ToDateTime <= delegation.ToDateTime))
            {
                ModelState.AddModelError("", "You  Delegated for another delegate too ! Remove your delegator!!");
                return Json(new[] { delegation }.ToDataSourceResult(request, ModelState));


            }

            if (db.Delegations.Any(x => x.EmployeeId == delegation.EmployeeId
                                        && x.FromDateTime >= delegation.FromDateTime
                                        && x.ToDateTime <= delegation.ToDateTime))
            {
                ModelState.AddModelError("", "He/She is  Delegated for another employee!!");
                return Json(new[] { delegation }.ToDataSourceResult(request, ModelState));
            }

            if (ModelState.IsValid)
            {

                delegation.DateCreated = DateTime.Now;
                delegation.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(delegation);
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
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delegations_Update([DataSourceRequest]DataSourceRequest request, Delegation delegation)
        {
            ModelState.Clear();
            
            if (string.IsNullOrEmpty(delegation.FromDateTimeAm) || string.IsNullOrEmpty(delegation.ToDateTimeAm))
            {
                ModelState.AddModelError("", "please enter both From and To dates are Mandatory!!");
                return Json(new[] { delegation }.ToDataSourceResult(request, ModelState));
            }

            if (!string.IsNullOrEmpty(delegation.FromDateTimeAm))
            {
                var splitDate = delegation.FromDateTimeAm.Split('/');
                delegation.FromDateTime = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

            if (!string.IsNullOrEmpty(delegation.ToDateTimeAm))
            {
                var splitDate = delegation.ToDateTimeAm.Split('/');
                delegation.ToDateTime = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            if (delegation.FromDateTime > delegation.ToDateTime)
            {
                ModelState.AddModelError("", "Please check the date range!!");
                return Json(new[] { delegation }.ToDataSourceResult(request, ModelState));

            }
            if (delegation.FromEmployeeId == delegation.EmployeeId)
            {
                ModelState.AddModelError("", "The assigned employee should be diffretnt from Delegator!!");
                return Json(new[] { delegation }.ToDataSourceResult(request, ModelState));

            }

            if (db.Delegations.Any(x => x.FromEmployeeId == delegation.FromEmployeeId 
                                        && x.FromDateTime >= delegation.FromDateTime 
                                        && x.ToDateTime <= delegation.ToDateTime
                                        &&x.DelegationId !=delegation.DelegationId))
            {
                ModelState.AddModelError("", "You  Delegated for another delegate too ! Remove your delegator!!");
                return Json(new[] { delegation }.ToDataSourceResult(request, ModelState));


            }

            if (db.Delegations.Any(x => x.EmployeeId == delegation.EmployeeId
                                        && x.DelegationId != delegation.DelegationId
                                        && x.FromDateTime >= delegation.FromDateTime 
                                        && x.ToDateTime <= delegation.ToDateTime))
            {
                ModelState.AddModelError("", "He/She is  Delegated for another employee!!");
                return Json(new[] { delegation }.ToDataSourceResult(request, ModelState));
            }
            if (ModelState.IsValid)
            {

                delegation.LastModified = DateTime.Now;
                delegation.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(delegation);
                db.Delegations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "delegation", "Success", User.Identity.Name, $"updated delegation from  {entity.FromDateTime} to {entity.ToDateTime} ", Modules.Payroll);
                #endregion

            }
            return Json(new[] { delegation }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delegations_Destroy([DataSourceRequest]DataSourceRequest request, Delegation delegation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                
                var entity = GetMappedObject(delegation);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted delegation information of employee " + delegation.FromEmployeeId );
                #endregion
                db.Delegations.Attach(entity);
                db.Delegations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { delegation }.ToDataSourceResult(request, ModelState));
        }
        private Delegation GetMappedObject(Delegation delegation)
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
                IsOpen = delegation.IsOpen,
                IsWithBenefit = delegation.IsWithBenefit,
                //Reason = delegation.Reason,
                //GLPeriodId = delegation.GLPeriodId
            };
            return entity;
        }

        // ================================== Delegation CC ==============================================

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

    
}
