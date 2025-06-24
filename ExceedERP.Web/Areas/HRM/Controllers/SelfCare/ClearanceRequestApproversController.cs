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
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService,HrmConstants.HRDepartmentHead)] 
    public class ClearanceRequestApproversController : BaseController
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
            var dept = db.MainCompanyStructures.Select(x => new { x.OrgStructureId, ApproverOrgStructureId = x.OrgStructureId, x.Name }).ToList();
            ViewData["Department"] = dept;
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

        public ActionResult ClearanceRequestApprovers_Read([DataSourceRequest]DataSourceRequest request)
        {
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var workUnitId = UserInformationHelper.GetEmployeeDepartmentIdByEmpId(empId);
            //List<int> userworkunitIds = UserInformationHelper.GetUserHeadWorkUnitAndChildWorkUnits(User.Identity.Name);
            IQueryable<ClearanceRequestApprover> clearanceRequestes = db.ClearanceRequestApprovers.Where(x=>x.ApproverOrgStructureId==workUnitId);
            DataSourceResult result = clearanceRequestes.ToDataSourceResult(request, clearanceRequest => new {
                ClearanceRequestApproverId = clearanceRequest.ClearanceRequestApproverId,
                ClearanceRequestId = clearanceRequest.ClearanceRequestId,
                OrgStructureId = clearanceRequest.OrgStructureId,
                ApproverOrgStructureId = clearanceRequest.ApproverOrgStructureId,
                EmployeeId = clearanceRequest.EmployeeId,
                LookupId = clearanceRequest.LookupId,
                Remark = clearanceRequest.Remark,
                Status = clearanceRequest.Status,
                CurrentApprovalStep = clearanceRequest.CurrentApprovalStep,
                DepartmentOnlineApprovedBy = clearanceRequest.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = clearanceRequest.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = clearanceRequest.IsDepartmentOnlineApproved,
                DepartmentSendForApprovalBy = clearanceRequest.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = clearanceRequest.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = clearanceRequest.IsDepartmentSendForApproval,
                ForwardedFrom = clearanceRequest.ForwardedFrom,
                ForwardedTo = clearanceRequest.ForwardedTo,
                IsFinalApproval = clearanceRequest.IsFinalApproval,
                IsSendForApproval = clearanceRequest.IsSendForApproval,
                SendForApprovalTime = clearanceRequest.SendForApprovalTime,
                IsOnlineApproved = clearanceRequest.IsOnlineApproved,
                IsOnlineTransferred = clearanceRequest.IsOnlineTransferred,
                IsOnlineTransferredBy = clearanceRequest.IsOnlineTransferredBy,
                IsVoid = clearanceRequest.IsVoid,
                OnlineApprovedBy = clearanceRequest.OnlineApprovedBy,
                OnlineApprovedTime = clearanceRequest.OnlineApprovedTime,
                OnlineTransferredTime = clearanceRequest.OnlineTransferredTime,
                SendForApprovalBy = clearanceRequest.SendForApprovalBy,
                VoidTime = clearanceRequest.VoidTime,
                OrgBranchName = clearanceRequest.OrgBranchName,
                VoidBy = clearanceRequest.VoidBy,
                DateCreated = clearanceRequest.DateCreated,
                CreatedBy = clearanceRequest.CreatedBy,
                ModifiedBy = clearanceRequest.ModifiedBy,
                LastModified = clearanceRequest.LastModified
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ClearanceRequestApprovers_Create([DataSourceRequest]DataSourceRequest request, ClearanceRequestApprover clearance)
        {
            ModelState.Clear();
            var exists = db.ClearanceRequestApprovers.Where(x =>x.OrgStructureId ==clearance.OrgStructureId
                                                                &&x.EmployeeId==clearance.EmployeeId
                                                                &&x.OrgStructureId==clearance.OrgStructureId
                                                                &&x.LookupId==clearance.LookupId).Any();
            if (exists)
            {
                ModelState.AddModelError("exists", "Duplication is not allowed!!");
            }
            if (ModelState.IsValid)
            {
                
                clearance.IsSendForApproval = true;
                clearance.IsSendForApproval = true;
                clearance.SendForApprovalTime = DateTime.Now;
                clearance.SendForApprovalBy = User.Identity.Name;
                clearance.DateCreated = DateTime.Now;
                clearance.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(clearance);
                db.ClearanceRequestApprovers.Add(entity);
                db.SaveChanges();
                clearance.ClearanceRequestId = entity.ClearanceRequestId;
                #region operation log
                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                UserHelper.OperationLog(Request.UserHostAddress, "Clearance request ", "Success", User.Identity.Name, User.Identity.Name + " Create Clearance Provider Named " + entity.EmployeeId,Modules.HRM);
              
                #endregion
            }

            return Json(new[] { clearance }.ToDataSourceResult(request, ModelState));
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ClearanceRequestApprovers_Update([DataSourceRequest]DataSourceRequest request, ClearanceRequestApprover clearanceRequest)
        {
            ModelState.Clear();
            var exists = db.ClearanceRequestApprovers.Where(x => x.OrgStructureId == clearanceRequest.OrgStructureId 
                                               && x.LookupId ==clearanceRequest.LookupId 
                                               && x.EmployeeId ==clearanceRequest.EmployeeId 
                                               && x.ClearanceRequestId != clearanceRequest.ClearanceRequestId).Any();
            if (exists)
            {
                ModelState.AddModelError("exists", "The same Record exists!!");
            }
            if (ModelState.IsValid)
            {
                clearanceRequest.LastModified = DateTime.Now;
                clearanceRequest.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(clearanceRequest);
                db.ClearanceRequestApprovers.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;

                db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
               
                    UserHelper.OperationLog(Request.UserHostAddress, "Clearance Request ", "Success", User.Identity.Name, User.Identity.Name + " Update Clearance Provider Info of " + entity.EmployeeId,Modules.HRM);
                

                #endregion
            }

            return Json(new[] { clearanceRequest }.ToDataSourceResult(request, ModelState));
        }
     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ClearanceRequestApprovers_Destroy([DataSourceRequest]DataSourceRequest request, ClearanceRequestApprover clearanceRequest)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(clearanceRequest);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Clearance provider " + clearanceRequest.EmployeeId);
                #endregion
                db.ClearanceRequestApprovers.Attach(entity);
                db.ClearanceRequestApprovers.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { clearanceRequest }.ToDataSourceResult(request, ModelState));
        }

        private ClearanceRequestApprover GetMappedObject(ClearanceRequestApprover  clearanceRequest)
        {
            var entity = new ClearanceRequestApprover
            {
                ClearanceRequestId = clearanceRequest.ClearanceRequestId,
                OrgStructureId = clearanceRequest.OrgStructureId,
                EmployeeId = clearanceRequest.EmployeeId,
                LookupId = clearanceRequest.LookupId,
                Remark = clearanceRequest.Remark,
                Status = clearanceRequest.Status,
                CurrentApprovalStep = clearanceRequest.CurrentApprovalStep,
                DepartmentOnlineApprovedBy = clearanceRequest.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = clearanceRequest.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = clearanceRequest.IsDepartmentOnlineApproved,
                DepartmentSendForApprovalBy = clearanceRequest.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = clearanceRequest.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = clearanceRequest.IsDepartmentSendForApproval,
                ForwardedFrom = clearanceRequest.ForwardedFrom,
                ForwardedTo = clearanceRequest.ForwardedTo,
                IsFinalApproval = clearanceRequest.IsFinalApproval,
                IsSendForApproval = clearanceRequest.IsSendForApproval,
                SendForApprovalTime = clearanceRequest.SendForApprovalTime,
                IsOnlineApproved = clearanceRequest.IsOnlineApproved,
                IsOnlineTransferred = clearanceRequest.IsOnlineTransferred,
                IsOnlineTransferredBy = clearanceRequest.IsOnlineTransferredBy,
                IsVoid = clearanceRequest.IsVoid,
                OnlineApprovedBy = clearanceRequest.OnlineApprovedBy,
                OnlineApprovedTime = clearanceRequest.OnlineApprovedTime,
                OnlineTransferredTime = clearanceRequest.OnlineTransferredTime,
                SendForApprovalBy = clearanceRequest.SendForApprovalBy,
                VoidTime = clearanceRequest.VoidTime,
                OrgBranchName = clearanceRequest.OrgBranchName,
                VoidBy = clearanceRequest.VoidBy,
                DateCreated = clearanceRequest.DateCreated,
                CreatedBy = clearanceRequest.CreatedBy,
                ModifiedBy = clearanceRequest.ModifiedBy,
                LastModified = clearanceRequest.LastModified
            };
            return entity;
        }

        public ActionResult DepartmentApproval(int id)
        {
            var clearance = db.ClearanceRequestApprovers.Find(id);

            if (clearance == null)
            {
                return Json(new
                {
                    Message = "Record is not found.",
                    Status = "not-ok"
                }, JsonRequestBehavior.AllowGet);
            }

            if (clearance.IsDepartmentOnlineApproved)
            {
                return Json(new
                {
                    Message = "This Clearance is already approved once. You can't approve again.",
                    Status = "OK"
                }, JsonRequestBehavior.AllowGet);
            }

            var clearanceRequest = db.ClearanceRequests.Find(clearance.ClearanceRequestId);
            if (clearanceRequest == null)
            {
                return Json(new
                {
                    Message = "This Clearance is already approved once. You can't approve again.",
                    Status = "OK"
                }, JsonRequestBehavior.AllowGet);
            }

            clearanceRequest.IsDepartmentOnlineApproved = true;
            clearanceRequest.DepartmentOnlineApprovedTime = DateTime.Now;
            clearanceRequest.DepartmentOnlineApprovedBy = User.Identity.Name;
            db.Entry(clearanceRequest).State = EntityState.Modified;
            // Approve clearance
            clearance.IsDepartmentOnlineApproved = true;
            clearance.DepartmentOnlineApprovedTime = DateTime.Now;
            clearance.DepartmentOnlineApprovedBy = User.Identity.Name;
            db.Entry(clearance).State = EntityState.Modified;

            // Update clearance status
            var clearanceStatus = db.ClearanceRequestStatus.FirstOrDefault(x =>
                x.ClearanceRequestId == clearance.ClearanceRequestId &&
                x.OrgStructureId == clearance.ApproverOrgStructureId);
            if (clearanceStatus != null)
            {
                //clearanceStatus.IsAproved = true;
                //clearanceStatus.IsSendForApproval = true;
                clearanceStatus.IsDepartmentOnlineApproved = true;
                //clearanceStatus.IsOnlineApproved = true;
                clearanceStatus.DepartmentOnlineApprovedTime = DateTime.Now;
                clearanceStatus.DepartmentSendForApprovalBy = User.Identity.Name;
                db.Entry(clearanceStatus).State = EntityState.Modified;
            }
            
            db.SaveChanges();

            return Json(new
            {
                Message = "You successfully approved this Clearance.",
                Status = "OK"
            }, JsonRequestBehavior.AllowGet);
        }




        public JsonResult SendForApprovalRequest(int id)
        {
            var clearance = db.ClearanceRequestApprovers.Find(id);
            if (clearance != null)
            {
                if (!clearance.IsSendForApproval)
                {


                    clearance.IsSendForApproval = true;
                    clearance.IsSendForApproval = true;
                    clearance.SendForApprovalTime = DateTime.Now;
                    clearance.SendForApprovalBy = User.Identity.Name;
                    //clearance.IsDepartmentOnlineApproved = true;
                    //clearance.DepartmentOnlineApprovedTime = DateTime.Now;
                    //clearance.DepartmentOnlineApprovedBy = User.Identity.Name;
                    db.Entry(clearance).State = EntityState.Modified;
                    db.SaveChanges();
                    var formattedData1 = new
                    {
                        Message = "You have Sent successfully for  approval.",
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
