﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService)]
    public class ClearanceRequestPersonalController : BaseController
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
        public ActionResult ClearanceRequestPersonal_Read([DataSourceRequest]DataSourceRequest request)
        {
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            IQueryable<ClearanceRequest> clearanceprovideres = db.ClearanceRequests.Where(x=>x.EmployeeId==empId);
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
        public ActionResult ClearanceRequestPersonal_Create([DataSourceRequest]DataSourceRequest request, ClearanceRequest clearanceRequest)
        {
            ModelState.Clear();
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            clearanceRequest.EmployeeId = empId;
            clearanceRequest.OrgStructureId = EmployeeHelper.GetEmployeeDepartmentIdByEmpId(empId);
            var exists = db.ClearanceRequests.Where(x => x.OrgStructureId == clearanceRequest.OrgStructureId
                                                        && x.EmployeeId == clearanceRequest.EmployeeId
                                                        && x.LookupId == clearanceRequest.LookupId).Any();
            if (exists)
            {
                ModelState.AddModelError("exists", "Duplication Request is not allowed!!");
            }

            var exist = db.ClearanceRequests
                                .Where(x => x.OrgStructureId == clearanceRequest.OrgStructureId
                                        && x.LookupId == clearanceRequest.LookupId
                                        && x.EmployeeId == clearanceRequest.EmployeeId).Any();
            if (exist)
            {
                ModelState.AddModelError("exists", "Duplication is not allowed!!");
                return Json(new[] { clearanceRequest }.ToDataSourceResult(request, ModelState));

            }
            var clearanceProvider = db.ClearanceProviderDetails.FirstOrDefault(x => x.Clearance.OrgStructureId == clearanceRequest.OrgStructureId);
            if (clearanceProvider == null)
            {
                ModelState.AddModelError("exists", "there is no Cleerance Approvers For this work unit Set Clearance providers !!");
                return Json(new[] { clearanceRequest }.ToDataSourceResult(request, ModelState));

            }
            
            if (!db.ClearanceProviderDetails.Any(x => x.Clearance.ClearanceProviderId == clearanceProvider.ClearanceProviderId))
            {
                    ModelState.AddModelError("exists", "there is no Cleerance Approvers For this work unit Set Clearance providers !!");
                    return Json(new[] { clearanceRequest }.ToDataSourceResult(request, ModelState));


            }
            
           
         

            if (ModelState.IsValid)
            {

                clearanceRequest.DateCreated = DateTime.Now;
                clearanceRequest.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(clearanceRequest);
                db.ClearanceRequests.Add(entity);
                db.SaveChanges();
                clearanceRequest.ClearanceRequestId = entity.ClearanceRequestId;
                var approvers = db.ClearanceProviderDetails
                    .Where(x => x.ClearanceProviderId == clearanceProvider.ClearanceProviderId).ToList();
                foreach (var approver in approvers)
                {
                    var entity1 = new ClearanceRequestApprover
                    {
                        ClearanceRequestId = clearanceRequest.ClearanceRequestId,
                        ApproverOrgStructureId = approver.OrgStructureId,
                        OrgStructureId = clearanceRequest.OrgStructureId,
                        EmployeeId = clearanceRequest.EmployeeId,
                        LookupId = clearanceRequest.LookupId,
                        Remark = clearanceRequest.Remark,
                        Status = clearanceRequest.Status,

                    };
                    db.ClearanceRequestApprovers.Add(entity1);
                    var requestStatus = new ClearanceRequestStatus
                    {
                        ClearanceRequestId = clearanceRequest.ClearanceRequestId,
                        OrgStructureId = approver.OrgStructureId,
                        IsAproved = false,
                        IsOnlineApproved = false,

                    };
                    db.ClearanceRequestStatus.Add(requestStatus);
                }
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Clearance Request ", "Success", User.Identity.Name, User.Identity.Name + " Create Clearance Request Named " + entity.ClearanceRequestId,Modules.HRM);
                #endregion
            }

            return Json(new[] { clearanceRequest }.ToDataSourceResult(request, ModelState));
        }
      
         [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ClearanceRequestPersonal_Update([DataSourceRequest]DataSourceRequest request, ClearanceRequest clearanceRequest)
        {
            ModelState.Clear();
            var exists = db.ClearanceRequests.Where(x => x.OrgStructureId == clearanceRequest.OrgStructureId 
                                                        && x.EmployeeId == clearanceRequest.EmployeeId 
                                                        && x.LookupId == clearanceRequest.LookupId 
                                                        && x.ClearanceRequestId != clearanceRequest.ClearanceRequestId).Any();
            if (exists)
            {
                ModelState.AddModelError("exists", "The same Record exists!!");
            }
            var employee = db.Placements.FirstOrDefault(x => x.EmployeeId == clearanceRequest.EmployeeId && x.isCurrent);
            if (employee != null)
            {
                clearanceRequest.OrgStructureId = employee.OrgStructureId;
            }
            var exist = db.ClearanceRequests.Where(x => x.OrgStructureId == clearanceRequest.OrgStructureId
            && x.LookupId == clearanceRequest.LookupId
            && x.ClearanceRequestId != clearanceRequest.ClearanceRequestId
            && x.EmployeeId == clearanceRequest.EmployeeId).Any();
            if (exist)
            {
                ModelState.AddModelError("exists", "Duplication is not allowed!!");
                return Json(new[] { clearanceRequest }.ToDataSourceResult(request, ModelState));

            }
            if (!db.ClearanceProvideres.Any(x => x.OrgStructureId == clearanceRequest.OrgStructureId))
            {
                ModelState.AddModelError("exists", "there is no Cleerance Approvers For this work unit Set Clearance providers !!");
                return Json(new[] { clearanceRequest }.ToDataSourceResult(request, ModelState));

            }
            var provider = db.ClearanceProvideres.FirstOrDefault(x => x.OrgStructureId == clearanceRequest.OrgStructureId);
            if (provider == null)
            {
                ModelState.AddModelError("exists", "there is no Cleerance Approvers For this work unit Set Clearance providers !!");
                return Json(new[] { clearanceRequest }.ToDataSourceResult(request, ModelState));

            }
            if (!db.ClearanceProvideres.Any(x => x.ClearanceProviderId == provider.ClearanceProviderId))
            {
                ModelState.AddModelError("exists", "there is no Cleerance Approvers For this work unit Set Clearance providers !!");
                return Json(new[] { clearanceRequest }.ToDataSourceResult(request, ModelState));

            }
            
            if (ModelState.IsValid)
            {
                clearanceRequest.LastModified = DateTime.Now;
                clearanceRequest.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(clearanceRequest);
                db.ClearanceRequests.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                var approvers = db.ClearanceProviderDetails.Where(x => x.ClearanceProviderId == provider.ClearanceProviderId).ToList();
                foreach (var approver in approvers)
                {
                    var approverRecord = db.ClearanceRequestApprovers
                        .Where(x => x.ClearanceRequestId == clearanceRequest.ClearanceRequestId
                        && x.OrgStructureId == approver.OrgStructureId
                        && x.ApproverOrgStructureId == clearanceRequest.OrgStructureId).FirstOrDefault();
                    if (approverRecord == null)
                    {
                        var entity1 = new ClearanceRequestApprover
                        {
                            ClearanceRequestId = clearanceRequest.ClearanceRequestId,
                            ApproverOrgStructureId = approver.OrgStructureId,
                            OrgStructureId = clearanceRequest.OrgStructureId,
                            LookupId = clearanceRequest.LookupId,
                            Remark = clearanceRequest.Remark,
                            Status = clearanceRequest.Status,

                        };
                        db.ClearanceRequestApprovers.Add(entity1);
                    }
                    var statusrRecord = db.ClearanceRequestStatus
                          .Where(x => x.ClearanceRequestId == clearanceRequest.ClearanceRequestId
                          && x.OrgStructureId == approver.OrgStructureId).FirstOrDefault();
                    if (statusrRecord == null)
                    {
                        var requestStatus = new ClearanceRequestStatus
                        {
                            ClearanceRequestId = clearanceRequest.ClearanceRequestId,
                            OrgStructureId = approver.OrgStructureId,
                            IsAproved = false,
                            IsOnlineApproved = false,

                        };
                        db.ClearanceRequestStatus.Add(requestStatus);
                    }

                }
                db.SaveChanges();
                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
               
                    UserHelper.OperationLog(Request.UserHostAddress, "Clearance Request ", "Success", User.Identity.Name, User.Identity.Name + " Update Clearance Request Info of " + entity.ClearanceRequestId,Modules.HRM);
                

                #endregion
            }

            return Json(new[] { clearanceRequest }.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ClearanceRequestPersonal_Destroy([DataSourceRequest]DataSourceRequest request, ClearanceRequest clearanceProvider)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(clearanceProvider);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Clearance request " + clearanceProvider.ClearanceRequestId);
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

        public JsonResult SendForApprovalRequest(int id)
        {
            var clearance = db.ClearanceRequests.Find(id);
            if (clearance == null)
            {
                return Json(new { Message = "Record not found.", Status = "not-OK" }, JsonRequestBehavior.AllowGet);
            }

            if (clearance.IsSendForApproval)
            {
                return Json(new { Message = "Already sent for approval. You cannot send it again.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }

            clearance.IsSendForApproval = true;
            clearance.SendForApprovalTime = DateTime.Now;
            clearance.SendForApprovalBy = User.Identity.Name;
            db.Entry(clearance).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { Message = "Request sent successfully for approval.", Status = "OK" }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
