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
    [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
    public class ClearanceProviderController : BaseController
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
            ViewData["Department"] = db.MainCompanyStructures.Where(x => x.IsActive).ToList();
            ViewData["clearanceType"] = db.Lookups.Where(x=>x.LookupType ==LookUpTypes.CLEARANCE_TYPE).ToList();
            ViewData["JobTitle"] = db.Lookups.Where(x=>x.LookupType ==LookUpTypes.JOB_TITLE).ToList();
            var employess = db.Person_Employee.Select(x => new { x.EmployeeId, HeadId = x.EmployeeId, FullNameaEnglish = x.EmployeeCode + "-" + x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish }).ToList();
            ViewData["EmployeeList"] = employess;
            var employeeIds = db.Placements.Where(x => x.IsHead && x.isCurrent).Select(x=>x.EmployeeId);
            ViewData["HeadEmployeeList"] = employess.Where(x=>employeeIds.Contains(x.EmployeeId)).Select(x=>new { Name = x.FullNameaEnglish }).ToList();

        }
        public JsonResult GetDepartments()
        {
            List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();

            var allDepo = db.MainCompanyStructures.Where(x=>x.IsActive).ToList();
            allDepartment.AddRange(allDepo);
            
            ViewData["Departments"] = allDepartment;
            return Json(db.MainCompanyStructures.Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name

            }), JsonRequestBehavior.AllowGet);
        }

        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
        public ActionResult ClearanceProvideres_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ClearanceProvider> clearanceprovideres = db.ClearanceProvideres.Where(x=>x.IsDepartmentOnlineApproved);
            DataSourceResult result = clearanceprovideres.ToDataSourceResult(request, clearanceProvider => new {
                ClearanceProviderId = clearanceProvider.ClearanceProviderId,
                OrgStructureId = clearanceProvider.OrgStructureId,
                Name = clearanceProvider.Name,
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
        public ActionResult ClearanceProvideres_Create([DataSourceRequest]DataSourceRequest request, ClearanceProvider clearanceProvider)
        {
            ModelState.Clear();
            var exists = db.ClearanceProvideres .Where(x =>x.OrgStructureId ==clearanceProvider.OrgStructureId
                                               &&x.LookupId==clearanceProvider.LookupId
                                               &&x.Name==clearanceProvider.Name).Any();
            if (exists)
            {
                ModelState.AddModelError("exists", "Duplication is not allowed!!");
            }
            if (ModelState.IsValid)
            {
                //not working
                //var salsries = db.ClearanceProvideres.Where(x => x.Name.ToUpper(XAPI.Common.exceedculture) == clearanceProvider.Name.ToUpper(XAPI.Common.exceedculture) || x.WorkUnit.ToUpper(XAPI.Common.exceedculture) == clearanceProvider.WorkUnit.ToUpper(XAPI.Common.exceedculture)).FirstOrDefault();
                clearanceProvider.IsSendForApproval = true;
                clearanceProvider.IsDepartmentOnlineApproved = true;
                clearanceProvider.DepartmentOnlineApprovedTime = DateTime.Now;
                clearanceProvider.DepartmentSendForApprovalBy = User.Identity.Name;
                clearanceProvider.DateCreated = DateTime.Now;
                clearanceProvider.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(clearanceProvider);
                db.ClearanceProvideres.Add(entity);
                db.SaveChanges();
                clearanceProvider.ClearanceProviderId = entity.ClearanceProviderId;

                #region operation log


                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                UserHelper.OperationLog(Request.UserHostAddress, "Clearance Provider ", "Success", User.Identity.Name, User.Identity.Name + " Create Clearance Provider Named " + entity.Name,Modules.HRM);
                
                
                #endregion
            }

            return Json(new[] { clearanceProvider }.ToDataSourceResult(request, ModelState));
        }
      
         [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ClearanceProvideres_Update([DataSourceRequest]DataSourceRequest request, ClearanceProvider clearanceProvider)
        {
            ModelState.Clear();
            var exists = db.ClearanceProvideres.Where(x => x.OrgStructureId == clearanceProvider.OrgStructureId 
                                                && x.LookupId ==clearanceProvider.LookupId 
                                                && x.ClearanceProviderId != clearanceProvider.ClearanceProviderId).Any();
            if (exists)
            {
                ModelState.AddModelError("exists", "The same Record exists!!");
            }
            if (ModelState.IsValid)
            {
                clearanceProvider.IsSendForApproval = true;
                clearanceProvider.IsDepartmentOnlineApproved = true;
                clearanceProvider.DepartmentOnlineApprovedTime = DateTime.Now;
                clearanceProvider.DepartmentSendForApprovalBy = User.Identity.Name;
                clearanceProvider.LastModified = DateTime.Now;
                clearanceProvider.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(clearanceProvider);
                db.ClearanceProvideres.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                UserHelper.OperationLog(Request.UserHostAddress, "Clearance Provider ", "Success", User.Identity.Name, User.Identity.Name + " Update Clearance Provider Info of " + entity.Name,Modules.HRM);
                #endregion
            }

            return Json(new[] { clearanceProvider }.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ClearanceProvideres_Destroy([DataSourceRequest]DataSourceRequest request, ClearanceProvider clearanceProvider)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(clearanceProvider);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Clearance provider " + clearanceProvider.Name);
                #endregion
                db.ClearanceProvideres.Attach(entity);
                db.ClearanceProvideres.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { clearanceProvider }.ToDataSourceResult(request, ModelState));
        }

        private ClearanceProvider GetMappedObject(ClearanceProvider clearanceProvider)
        {
            var entity = new ClearanceProvider
            {
                ClearanceProviderId = clearanceProvider.ClearanceProviderId,
                OrgStructureId = clearanceProvider.OrgStructureId,
                Name = clearanceProvider.Name,
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
        public ActionResult ClearanceApproval(int id)
        {
            var clearanceproviderDetails = db.ClearanceProviderDetails.Where(x => x.ClearanceProviderId == id);
            var clearance = db.ClearanceProvideres.Find(id);
            if (clearance != null&& clearanceproviderDetails.Any())
            {
                if (clearance.IsSendForApproval 
                    && clearance.IsDepartmentOnlineApproved 
                    && !clearance.IsOnlineApproved)
                {
                    clearance.IsOnlineApproved = true;
                    clearance.OnlineApprovedTime = DateTime.Now;
                    clearance.OnlineApprovedBy = User.Identity.Name;
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
            else if(clearance != null && !clearanceproviderDetails.Any())
            {
                var formattedData13 = new
                {
                    Message = "you can't approve add Clearance Approver in detail First.",
                    Status = "OK"
                };
                return Json(formattedData13, JsonRequestBehavior.AllowGet);
            }
            var formattedData2 = new
            {
                Message = "Record is not Found.",
                Status = "FAIL"
            };
            return Json(formattedData2, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
