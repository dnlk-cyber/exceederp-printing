﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
 using ExceedERP.Core.Domain;
 using ExceedERP.Core.Domain.Common;
﻿using ExceedERP.Core.Domain.Common.HRM;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService,HrmConstants.HRDepartmentHead)] 
    public class ClearanceRequestDepartmentsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            PopulateViewData();
            GetDepartments();
            return View();
        }
        public void PopulateViewData()
        {
            ViewData["Department"] = db.MainCompanyStructures.ToList();
            ViewData["clearanceType"] = db.Lookups.Where(x=>x.LookupType ==LookUpTypes.CLEARANCE_TYPE).ToList();
            var employess = db.Person_Employee.Select(x => new { x.EmployeeId, HeadId = x.EmployeeId, FullNameaEnglish = x.EmployeeCode + "-" + x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish }).ToList();
            ViewData["EmployeeList"] = employess;

        }
        public JsonResult GetDepartments()
        {
            List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();

            var allDepo = db.MainCompanyStructures.ToList();
            allDepartment.AddRange(allDepo);
            ViewData["Departments"] = allDepartment;
            return Json(db.MainCompanyStructures.Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name

            }), JsonRequestBehavior.AllowGet);
        }

        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
        public ActionResult ClearanceRequestDepartments_Read([DataSourceRequest]DataSourceRequest request)
        {
              List<int> userworkunitIds = UserInformationHelper.GetUserHeadWorkUnitAndChildWorkUnits(User.Identity.Name);
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var subordinates = EmployeeHelper.GetSubordinates_FilteredByHead(empId);
            IQueryable<ClearanceRequest> clearanceprovideres = db.ClearanceRequests.Where(x=>subordinates.Contains(x.EmployeeId)&&x.IsSendForApproval);
            DataSourceResult result = clearanceprovideres.ToDataSourceResult(request, clearanceProvider => new {
                ClearanceRequestId = clearanceProvider.ClearanceRequestId,
                OrgStructureId = clearanceProvider.OrgStructureId,
                EmployeeId = clearanceProvider.EmployeeId,
                LookupId = clearanceProvider.LookupId,
                Remark = clearanceProvider.Remark,
                Status = clearanceProvider.Status,
                CurrentApprovalStep = clearanceProvider.CurrentApprovalStep,
                DepartmentOnlineApprovedBy = clearanceProvider.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = clearanceProvider.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = clearanceProvider.IsDepartmentOnlineApproved,
                DepartmentSendForApprovalBy = clearanceProvider.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = clearanceProvider.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = clearanceProvider.IsDepartmentSendForApproval,
                ForwardedFrom = clearanceProvider.ForwardedFrom,
                ForwardedTo = clearanceProvider.ForwardedTo,
                IsFinalApproval = clearanceProvider.IsFinalApproval,
                IsSendForApproval = clearanceProvider.IsSendForApproval,
                SendForApprovalTime = clearanceProvider.SendForApprovalTime,
                IsOnlineApproved = clearanceProvider.IsOnlineApproved,
                IsOnlineTransferred = clearanceProvider.IsOnlineTransferred,
                IsOnlineTransferredBy = clearanceProvider.IsOnlineTransferredBy,
                IsVoid = clearanceProvider.IsVoid,
                OnlineApprovedBy = clearanceProvider.OnlineApprovedBy,
                OnlineApprovedTime = clearanceProvider.OnlineApprovedTime,
                OnlineTransferredTime = clearanceProvider.OnlineTransferredTime,
                SendForApprovalBy = clearanceProvider.SendForApprovalBy,
                VoidTime = clearanceProvider.VoidTime,
                OrgBranchName = clearanceProvider.OrgBranchName,
                VoidBy = clearanceProvider.VoidBy,
                DateCreated = clearanceProvider.DateCreated,
                CreatedBy = clearanceProvider.CreatedBy,
                ModifiedBy = clearanceProvider.ModifiedBy,
                LastModified = clearanceProvider.LastModified
            });

            return Json(result);
        }
        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ClearanceRequestDepartments_Create([DataSourceRequest]DataSourceRequest request, ClearanceRequest clearance)
        {
            ModelState.Clear();
            var exists = db.ClearanceRequests.Where(x =>x.OrgStructureId ==clearance.OrgStructureId&&x.LookupId==clearance.LookupId).Any();
            if (exists)
            {
                ModelState.AddModelError("exists", "Duplication is not allowed!!");
                return Json(new[] { clearance }.ToDataSourceResult(request, ModelState));

            }
            if (db.ClearanceProviderDetails.Any(x => x.Clearance.OrgStructureId == clearance.OrgStructureId))
            {
                ModelState.AddModelError("exists", "there is no Cleerance Approvers For this work unit Set Clearance providers !!");
                return Json(new[] { clearance }.ToDataSourceResult(request, ModelState));

            }
            var provider = db.ClearanceProvideres.FirstOrDefault(x => x.OrgStructureId == clearance.OrgStructureId);
            if (provider != null)
            {
                if (!db.ClearanceProvideres.Any(x => x.ClearanceProviderId == provider.ClearanceProviderId))
                {
                    ModelState.AddModelError("exists", "there is no Cleerance Approvers For this work unit Set Clearance providers !!");
                    return Json(new[] { clearance }.ToDataSourceResult(request, ModelState));

                }

            }
            if (ModelState.IsValid)
            {
                //not working

                //var salsries = db.ClearanceProvideres.Where(x => x.Name.ToUpper(XAPI.Common.exceedculture) == clearanceProvider.Name.ToUpper(XAPI.Common.exceedculture) || x.WorkUnit.ToUpper(XAPI.Common.exceedculture) == clearanceProvider.WorkUnit.ToUpper(XAPI.Common.exceedculture)).FirstOrDefault();
                clearance.IsSendForApproval = true;
                clearance.IsSendForApproval = true;
                clearance.SendForApprovalTime = DateTime.Now;
                clearance.SendForApprovalBy = User.Identity.Name;
                clearance.DateCreated = DateTime.Now;
                clearance.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(clearance);
                db.ClearanceRequests.Add(entity);
                db.SaveChanges();
                clearance.ClearanceRequestId = entity.ClearanceRequestId;
                var approvers = db.ClearanceProviderDetails.Where(x => x.ClearanceProviderId == provider.ClearanceProviderId).ToList();
                foreach (var approver in approvers)
                {
                    var entity1 = new ClearanceRequestApprover
                    {
                        ClearanceRequestId = clearance.ClearanceRequestId,
                        ApproverOrgStructureId = approver.OrgStructureId,
                        OrgStructureId = clearance.OrgStructureId,
                        LookupId = clearance.LookupId,
                        Remark = clearance.Remark,
                        Status = clearance.Status,

                    };
                    db.ClearanceRequestApprovers.Add(entity1);
                    var requestStatus = new ClearanceRequestStatus
                    {
                        ClearanceRequestId = clearance.ClearanceRequestId,
                        OrgStructureId = approver.OrgStructureId,
                        IsAproved = false,
                        IsOnlineApproved = false,

                    };
                    db.ClearanceRequestStatus.Add(requestStatus);
                }
                db.SaveChanges();

                #region operation log
                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                UserHelper.OperationLog(Request.UserHostAddress, "Clearance request ", "Success", User.Identity.Name, User.Identity.Name + " Create Clearance Provider Named " + entity.EmployeeId,Modules.HRM);
               
                #endregion
            }

            return Json(new[] { clearance }.ToDataSourceResult(request, ModelState));
        }
      
         [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ClearanceRequestDepartments_Update([DataSourceRequest]DataSourceRequest request, ClearanceRequest clearanceProvider)
        {
            ModelState.Clear();
            var exists = db.ClearanceRequests.Where(x => x.OrgStructureId == clearanceProvider.OrgStructureId 
                                                    && x.LookupId ==clearanceProvider.LookupId 
                                                    && x.ClearanceRequestId != clearanceProvider.ClearanceRequestId).Any();
            if (exists)
            {
                ModelState.AddModelError("exists", "The same Record exists!!");
            }
            if (ModelState.IsValid)
            {
                clearanceProvider.LastModified = DateTime.Now;
                clearanceProvider.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(clearanceProvider);
                db.ClearanceRequests.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
               
                    UserHelper.OperationLog(Request.UserHostAddress, "Clearance Request ", "Success", User.Identity.Name, User.Identity.Name + " Update Clearance Provider Info of " + entity.EmployeeId,Modules.HRM);
                

                #endregion
            }

            return Json(new[] { clearanceProvider }.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ClearanceRequestDepartments_Destroy([DataSourceRequest]DataSourceRequest request, ClearanceRequest clearanceProvider)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(clearanceProvider);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Clearance provider " + clearanceProvider.EmployeeId);
                #endregion
                db.ClearanceRequests.Attach(entity);
                db.ClearanceRequests.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { clearanceProvider }.ToDataSourceResult(request, ModelState));
        }

        private ClearanceRequest GetMappedObject(ClearanceRequest clearanceProvider)
        {
            var entity = new ClearanceRequest
            {
                ClearanceRequestId = clearanceProvider.ClearanceRequestId,
                OrgStructureId = clearanceProvider.OrgStructureId,
                EmployeeId = clearanceProvider.EmployeeId,
                LookupId = clearanceProvider.LookupId,
                Remark = clearanceProvider.Remark,
                Status = clearanceProvider.Status,
                CurrentApprovalStep = clearanceProvider.CurrentApprovalStep,
                DepartmentOnlineApprovedBy = clearanceProvider.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = clearanceProvider.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = clearanceProvider.IsDepartmentOnlineApproved,
                DepartmentSendForApprovalBy = clearanceProvider.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = clearanceProvider.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = clearanceProvider.IsDepartmentSendForApproval,
                ForwardedFrom = clearanceProvider.ForwardedFrom,
                ForwardedTo = clearanceProvider.ForwardedTo,
                IsFinalApproval = clearanceProvider.IsFinalApproval,
                IsSendForApproval = clearanceProvider.IsSendForApproval,
                SendForApprovalTime = clearanceProvider.SendForApprovalTime,
                IsOnlineApproved = clearanceProvider.IsOnlineApproved,
                IsOnlineTransferred = clearanceProvider.IsOnlineTransferred,
                IsOnlineTransferredBy = clearanceProvider.IsOnlineTransferredBy,
                IsVoid = clearanceProvider.IsVoid,
                OnlineApprovedBy = clearanceProvider.OnlineApprovedBy,
                OnlineApprovedTime = clearanceProvider.OnlineApprovedTime,
                OnlineTransferredTime = clearanceProvider.OnlineTransferredTime,
                SendForApprovalBy = clearanceProvider.SendForApprovalBy,
                VoidTime = clearanceProvider.VoidTime,
                OrgBranchName = clearanceProvider.OrgBranchName,
                VoidBy = clearanceProvider.VoidBy,
                DateCreated = clearanceProvider.DateCreated,
                CreatedBy = clearanceProvider.CreatedBy,
                ModifiedBy = clearanceProvider.ModifiedBy,
                LastModified = clearanceProvider.LastModified
            };
            return entity;
        }
        public ActionResult DepartmentApproval(int id)
        {
            try
            {
                var clearance = db.ClearanceRequests.Find(id);
                if (clearance == null)
                {
                    return Json(new
                    {
                        Message = "Record not found.",
                        Status = "FAIL"
                    }, JsonRequestBehavior.AllowGet);
                }
                if (clearance.IsSendForApproval && !clearance.IsDepartmentOnlineApproved)
                {
                    clearance.IsDepartmentOnlineApproved = true;
                    clearance.DepartmentOnlineApprovedTime = DateTime.Now;
                    clearance.DepartmentOnlineApprovedBy = User.Identity.Name;

                    db.Entry(clearance).State = EntityState.Modified;
                    db.SaveChanges();

                    return Json(new
                    {
                        Message = "You successfully approved this Clearance.",
                        Status = "OK"
                    }, JsonRequestBehavior.AllowGet);
                }

                return Json(new
                {
                    Message = "This Clearance is already approved once. You can't approve again.",
                    Status = "NOT-OK"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log the exception (if using a logging framework)
                return Json(new
                {
                    Message = "An error occurred while processing your request.",
                    Status = "ERROR",
                    Details = ex.Message // Optional: Include exception details for debugging (useful during development).
                }, JsonRequestBehavior.AllowGet);
            }
        }
        //public ActionResult DepartmentApproval(int id)
        //{
        //    var clearance = db.ClearanceRequests.Find(id);
        //    if (clearance != null)
        //    {
        //        if(clearance.IsSendForApproval && !clearance.IsDepartmentOnlineApproved)
        //        {
        //            clearance.IsDepartmentOnlineApproved = true;
        //            clearance.DepartmentOnlineApprovedTime = DateTime.Now;
        //            clearance.DepartmentOnlineApprovedBy = User.Identity.Name;
        //            db.Entry(clearance).State = EntityState.Modified;
        //            db.SaveChanges();
        //            var formattedData = new
        //            {
        //                Message = "You successfully approved this Clearance.",
        //                Status = "OK"
        //            };
        //            return Json(formattedData, JsonRequestBehavior.AllowGet);
        //        }

        //        var formattedData1 = new
        //        {
        //            Message = "This Clearance is already approved Once. you can't approve again.",
        //            Status = "OK"
        //        };
        //        return Json(formattedData1, JsonRequestBehavior.AllowGet);
                
        //    }

        //    var formattedData2 = new
        //    {
        //        Message = "Record is not Found.",
        //        Status = "FAIL"
        //    };
        //    return Json(formattedData2, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult SendForApprovalRequest(int id)
        {
            var clearance = db.ClearanceRequests.Find(id);
            if (clearance != null)
            {
                if (!clearance.IsDepartmentSendForApproval &&clearance.IsSendForApproval)
                {


                    clearance.IsDepartmentSendForApproval = true;
                    clearance.DepartmentSendForApprovalTime = DateTime.Now;
                    clearance.DepartmentSendForApprovalBy = User.Identity.Name;
                    clearance.IsDepartmentOnlineApproved = true;
                    clearance.DepartmentOnlineApprovedTime = DateTime.Now;
                    clearance.DepartmentOnlineApprovedBy = User.Identity.Name;
                    db.Entry(clearance).State = EntityState.Modified;
                    db.SaveChanges();
                    var formattedData1 = new
                    {
                        Message = "You have Sent successfully for Nex Approvers   approval.",
                        Status = "OK"
                    };
                    return Json(formattedData1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var formattedData2 = new
                    {
                        Message = "it is sent Once you can't do again.",
                        Status = "OK"
                    };
                    return Json(formattedData2, JsonRequestBehavior.AllowGet);
                }

            }
            var formattedData3 = new
            {
                Message = "Record not found.",
                Status = "not-OKe"
            };
            return Json(formattedData3, JsonRequestBehavior.AllowGet);

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
