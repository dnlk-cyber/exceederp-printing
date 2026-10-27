using System;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Global;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser, HrmConstants.HRAdmin)]
    public class GuarantorOrSuretiesController : BaseController
    {

       
        private ExceedDbContext db = new ExceedDbContext();
      
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            ViewData["isEthiopicDate"] = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate;
            var employeeIds = db.Placements.Select(x => x.EmployeeId).AsQueryable();
            ViewData["EmployeeList"] = db.Person_Employee
                                        .Where(x => employeeIds.Contains(x.EmployeeId))
                                        .Select(x => new { EmployeeId = x.EmployeeId,
                                            HeadId = x.EmployeeId,
                                            EmployeeCode = x.EmployeeCode,
                                            FullNameAmharic = x.EmployeeCode + " - " + x.FirstName + " " + x.MiddleName + " " + x.LastName,
                                            FullNameaEnglish = x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish
                                        }).AsQueryable();
           


        }
        public ActionResult GuarantorOrSurety_Read([DataSourceRequest]DataSourceRequest request,int garanteeEmployeeId)
        {



            IQueryable<GuarantorOrSurety> guarantors = db.GuarantorOrSureties.Where(x => x.EmployeeId==garanteeEmployeeId);

            DataSourceResult result = guarantors.ToDataSourceResult(request, guarantor => new
            {
                GuarantorOrSuretyId = guarantor.GuarantorOrSuretyId,
                EmployeeId = guarantor.EmployeeId,
                Remark = guarantor.Remark,
                OrgBranchName = guarantor.OrgBranchName,
                Status = guarantor.Status,
                IsVoid = guarantor.IsVoid,
                VoidBy = guarantor.VoidBy,
                VoidTime = guarantor.VoidTime,
                IsOnlineApproved = guarantor.IsOnlineApproved,
                OnlineApprovedBy = guarantor.OnlineApprovedBy,
                OnlineApprovedTime = guarantor.OnlineApprovedTime,
                IsOnlineTransferred = guarantor.IsOnlineTransferred,
                IsOnlineTransferredBy = guarantor.IsOnlineTransferredBy,
                OnlineTransferredTime = guarantor.OnlineTransferredTime,
                IsSendForApproval = guarantor.IsSendForApproval,
                SendForApprovalBy = guarantor.SendForApprovalBy,
                CurrentApprovalStep = guarantor.CurrentApprovalStep,
                DepartmentOnlineApprovedBy = guarantor.DepartmentOnlineApprovedBy,
                DepartmentSendForApprovalBy = guarantor.DepartmentSendForApprovalBy,
                DepartmentOnlineApprovedTime = guarantor.DepartmentOnlineApprovedTime,
                DepartmentSendForApprovalTime = guarantor.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = guarantor.IsDepartmentSendForApproval,
                ForwardedFrom = guarantor.ForwardedFrom,
                ForwardedTo = guarantor.ForwardedTo,
                IsDepartmentOnlineApproved = guarantor.IsDepartmentOnlineApproved,
                IsFinalApproval = guarantor.IsFinalApproval,
                SendForApprovalTime = guarantor.SendForApprovalTime,
                DateCreated = guarantor.DateCreated,
                LastModified = guarantor.LastModified,
                CreatedBy = guarantor.CreatedBy,
                ModifiedBy = guarantor.ModifiedBy,
            });

            return Json(result);
        }
        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser, HrmConstants.HRAdmin)]
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
           


            IQueryable<GuarantorOrSurety> guarantors = db.GuarantorOrSureties.Where(x=>x.IsSendForApproval);

            DataSourceResult result = guarantors.ToDataSourceResult(request, guarantor => new
            {
                GuarantorOrSuretyId = guarantor.GuarantorOrSuretyId,
                EmployeeId = guarantor.EmployeeId,
                Remark = guarantor.Remark,
                OrgBranchName = guarantor.OrgBranchName,
                Status = guarantor.Status,
                IsVoid = guarantor.IsVoid,
                VoidBy = guarantor.VoidBy,
                VoidTime = guarantor.VoidTime,
                IsOnlineApproved = guarantor.IsOnlineApproved,
                OnlineApprovedBy = guarantor.OnlineApprovedBy,
                OnlineApprovedTime = guarantor.OnlineApprovedTime,
                IsOnlineTransferred = guarantor.IsOnlineTransferred,
                IsOnlineTransferredBy = guarantor.IsOnlineTransferredBy,
                OnlineTransferredTime = guarantor.OnlineTransferredTime,
                IsSendForApproval = guarantor.IsSendForApproval,
                SendForApprovalBy = guarantor.SendForApprovalBy,
                CurrentApprovalStep = guarantor.CurrentApprovalStep,
                DepartmentOnlineApprovedBy = guarantor.DepartmentOnlineApprovedBy,
                DepartmentSendForApprovalBy = guarantor.DepartmentSendForApprovalBy,
                DepartmentOnlineApprovedTime = guarantor.DepartmentOnlineApprovedTime,
                DepartmentSendForApprovalTime = guarantor.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = guarantor.IsDepartmentSendForApproval,
                ForwardedFrom = guarantor.ForwardedFrom,
                ForwardedTo = guarantor.ForwardedTo,
                IsDepartmentOnlineApproved = guarantor.IsDepartmentOnlineApproved,
                IsFinalApproval = guarantor.IsFinalApproval,
                SendForApprovalTime = guarantor.SendForApprovalTime,
                DateCreated = guarantor.DateCreated,
                LastModified = guarantor.LastModified,
                CreatedBy = guarantor.CreatedBy,
                ModifiedBy = guarantor.ModifiedBy,
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, GuarantorOrSurety guarantor)
        {
            ModelState.Clear();
         

           
           

            if (db.GuarantorOrSureties.Any(x => x.EmployeeId == guarantor.EmployeeId))
            {
                ModelState.AddModelError("", "Record Exists!!");
                return Json(new[] { guarantor }.ToDataSourceResult(request, ModelState));
                
            }

           

            if (ModelState.IsValid)
            {

                guarantor.DateCreated = DateTime.Now;
                guarantor.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(guarantor);
                db.GuarantorOrSureties.Add(entity);
                db.SaveChanges();
                guarantor.GuarantorOrSuretyId = entity.GuarantorOrSuretyId;

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "guarantor", "Success", User.Identity.Name, User.Identity.Name + " add new guarantor at date" + DateTime.Today, Modules.HRM);

                #endregion
               

            }
            return Json(new[] { guarantor }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GuarantorOrSuretyPerEmployee_Create([DataSourceRequest]DataSourceRequest request, GuarantorOrSurety guarantor,int garanteeEmployeeId)
        {
            ModelState.Clear();
            if (db.GuarantorOrSureties.Any(x => x.EmployeeId == garanteeEmployeeId))
            {
                ModelState.AddModelError("", "Record exists!!");
                return Json(new[] { guarantor }.ToDataSourceResult(request, ModelState));
            }
            guarantor.EmployeeId = garanteeEmployeeId;
            if (ModelState.IsValid)
            {
                guarantor.DateCreated = DateTime.Now;
                guarantor.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(guarantor);
                db.GuarantorOrSureties.Add(entity);
                db.SaveChanges();
                guarantor.GuarantorOrSuretyId = entity.GuarantorOrSuretyId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "guarantor", "Success", User.Identity.Name, User.Identity.Name + " add new guarantor at date" + DateTime.Today, Modules.HRM);
                #endregion
               

            }
            return Json(new[] { guarantor }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, GuarantorOrSurety guarantor)
        {
            ModelState.Clear();

         
         

          
            if (db.GuarantorOrSureties.Any(x => x.EmployeeId == guarantor.EmployeeId
                                                &&x.GuarantorOrSuretyId!=guarantor.GuarantorOrSuretyId))
            {
                ModelState.AddModelError("", "Record already  existed !");
                return Json(new[] { guarantor }.ToDataSourceResult(request, ModelState));
            }
            
            if (ModelState.IsValid)
            {
                guarantor.LastModified = DateTime.Now;
                guarantor.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(guarantor);
                db.GuarantorOrSureties.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                UserHelper.OperationLog(Request.UserHostAddress, "guarantor", "Success", User.Identity.Name, $"updated guarantor", Modules.HRM);
            }

            return Json(new[] { guarantor }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Destroy([DataSourceRequest]DataSourceRequest request, GuarantorOrSurety guarantor)
        {
            ModelState.Clear();
            if (db.GuarantorOrSuretyDetails.Any(x => x.GuarantorOrSuretyId == guarantor.GuarantorOrSuretyId))
            {
                ModelState.AddModelError("", "Record already  existed !");
                return Json(new[] { guarantor }.ToDataSourceResult(request, ModelState));
            }

            if (ModelState.IsValid)
            {
                guarantor.LastModified = DateTime.Now;
                guarantor.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(guarantor);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted guarantor information of employee " + guarantor.EmployeeId);
                #endregion
                db.GuarantorOrSureties.Attach(entity);
                db.GuarantorOrSureties.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { guarantor }.ToDataSourceResult(request, ModelState));
        }

        private GuarantorOrSurety GetMappedObject(GuarantorOrSurety guarantor)
        {
            var entity = new GuarantorOrSurety
            {
                GuarantorOrSuretyId = guarantor.GuarantorOrSuretyId,
                EmployeeId = guarantor.EmployeeId,
                Remark = guarantor.Remark,
                OrgBranchName = guarantor.OrgBranchName,
                Status = guarantor.Status,
                IsVoid = guarantor.IsVoid,
                VoidBy = guarantor.VoidBy,
                VoidTime = guarantor.VoidTime,
                IsOnlineApproved = guarantor.IsOnlineApproved,
                OnlineApprovedBy = guarantor.OnlineApprovedBy,
                OnlineApprovedTime = guarantor.OnlineApprovedTime,
                IsOnlineTransferred = guarantor.IsOnlineTransferred,
                IsOnlineTransferredBy = guarantor.IsOnlineTransferredBy,
                OnlineTransferredTime = guarantor.OnlineTransferredTime,
                IsSendForApproval = guarantor.IsSendForApproval,
                SendForApprovalBy = guarantor.SendForApprovalBy,
                CurrentApprovalStep = guarantor.CurrentApprovalStep,
                DepartmentOnlineApprovedBy = guarantor.DepartmentOnlineApprovedBy,
                DepartmentSendForApprovalBy = guarantor.DepartmentSendForApprovalBy,
                DepartmentOnlineApprovedTime = guarantor.DepartmentOnlineApprovedTime,
                DepartmentSendForApprovalTime = guarantor.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = guarantor.IsDepartmentSendForApproval,
                ForwardedFrom = guarantor.ForwardedFrom,
                ForwardedTo = guarantor.ForwardedTo,
                IsDepartmentOnlineApproved = guarantor.IsDepartmentOnlineApproved,
                IsFinalApproval = guarantor.IsFinalApproval,
                SendForApprovalTime = guarantor.SendForApprovalTime,
                DateCreated = guarantor.DateCreated,
                LastModified = guarantor.LastModified,
                CreatedBy = guarantor.CreatedBy,
                ModifiedBy = guarantor.ModifiedBy,
            };
            return entity;
        }
      
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }

   
}
