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
    public class ClearanceRequestStatusController : BaseController
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
        public ActionResult ClearanceRequestStatus_Read([DataSourceRequest]DataSourceRequest request, int id )
        {

            IQueryable<ClearanceRequestStatus> clearanceRequestes = db.ClearanceRequestStatus.Where(x=>x.ClearanceRequestId==id);
            DataSourceResult result = clearanceRequestes.ToDataSourceResult(request, clearanceRequest => new {
                ClearanceRequestStatusId = clearanceRequest.ClearanceRequestStatusId,
                ClearanceRequestId = clearanceRequest.ClearanceRequestId,
                OrgStructureId = clearanceRequest.OrgStructureId,
                IsAproved = clearanceRequest.IsAproved,
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
        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ClearanceRequestStatus_Create([DataSourceRequest]DataSourceRequest request, ClearanceRequestStatus clearance,int id)
        {
            ModelState.Clear();
            clearance.ClearanceRequestStatusId = id;

            if (ModelState.IsValid)
            {
                
                clearance.IsSendForApproval = true;
                clearance.IsSendForApproval = true;
                clearance.SendForApprovalTime = DateTime.Now;
                clearance.SendForApprovalBy = User.Identity.Name;
                clearance.DateCreated = DateTime.Now;
                clearance.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(clearance);
                db.ClearanceRequestStatus.Add(entity);
                db.SaveChanges();
                clearance.ClearanceRequestId = entity.ClearanceRequestId;
                #region operation log
                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                UserHelper.OperationLog(Request.UserHostAddress, "Clearance request ", "Success", User.Identity.Name, User.Identity.Name + " Create Clearance Provider Named " + entity.ClearanceRequestStatusId,Modules.HRM);
              
                #endregion
            }

            return Json(new[] { clearance }.ToDataSourceResult(request, ModelState));
        }
      
         [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ClearanceRequestStatus_Update([DataSourceRequest]DataSourceRequest request, ClearanceRequestStatus clearanceRequest)
        {
            ModelState.Clear();
            var exists = db.ClearanceRequestStatus.Where(x => x.OrgStructureId == clearanceRequest.OrgStructureId 
                                                            && x.ClearanceRequestStatusId != clearanceRequest.ClearanceRequestStatusId).Any();
            if (exists)
            {
                ModelState.AddModelError("exists", "The same Record exists!!");
            }
            if (ModelState.IsValid)
            {
                clearanceRequest.LastModified = DateTime.Now;
                clearanceRequest.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(clearanceRequest);
                db.ClearanceRequestStatus.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
               
                    UserHelper.OperationLog(Request.UserHostAddress, "Clearance Request ", "Success", User.Identity.Name, User.Identity.Name + " Update Clearance Provider Info of " + entity.ClearanceRequestStatusId,Modules.HRM);
                

                #endregion
            }

            return Json(new[] { clearanceRequest }.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ClearanceRequestStatus_Destroy([DataSourceRequest]DataSourceRequest request, ClearanceRequestStatus clearanceRequest)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(clearanceRequest);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Clearance provider " + clearanceRequest.ClearanceRequestStatusId);
                #endregion
                db.ClearanceRequestStatus.Attach(entity);
                db.ClearanceRequestStatus.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { clearanceRequest }.ToDataSourceResult(request, ModelState));
        }

        private ClearanceRequestStatus GetMappedObject(ClearanceRequestStatus clearanceRequest)
        {
            var entity = new ClearanceRequestStatus
            {
                ClearanceRequestStatusId = clearanceRequest.ClearanceRequestStatusId,
                ClearanceRequestId = clearanceRequest.ClearanceRequestId,
                OrgStructureId = clearanceRequest.OrgStructureId,
                IsAproved = clearanceRequest.IsAproved,
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
            var clearance = db.ClearanceRequestStatus.Find(id);
            if (clearance != null)
            {
                if(clearance.IsSendForApproval && !clearance.IsDepartmentOnlineApproved)
                {
                    clearance.IsDepartmentOnlineApproved = true;
                    clearance.DepartmentOnlineApprovedTime = DateTime.Now;
                    clearance.DepartmentOnlineApprovedBy = User.Identity.Name;
                    db.Entry(clearance).State = EntityState.Modified;
                    db.SaveChanges();
                    var formattedData = new
                    {
                        Message = "You successfully approved this Clearance.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                var formattedData1 = new
                {
                    Message = "This Clearance is already approved Once. you can't approve again.",
                    Status = "OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
                
            }

            var formattedData2 = new
            {
                Message = "Record is not Found.",
                Status = "FAIL"
            };
            return Json(formattedData2, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SendForApprovalRequest(int id)
        {
            var clearance = db.ClearanceRequestStatus.Find(id);
            if (clearance != null)
            {
                if (!clearance.IsSendForApproval)
                {


                    clearance.IsSendForApproval = true;
                    clearance.IsSendForApproval = true;
                    clearance.SendForApprovalTime = DateTime.Now;
                    clearance.SendForApprovalBy = User.Identity.Name;
                    clearance.IsDepartmentOnlineApproved = true;
                    clearance.DepartmentOnlineApprovedTime = DateTime.Now;
                    clearance.DepartmentOnlineApprovedBy = User.Identity.Name;
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
