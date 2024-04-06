using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{

    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser, HrmConstants.HRAdmin)]
    public class EmployeeTransferRequestDepartmentValidationsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
     
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRRecruitmentUser)]
        public ActionResult EmployeeTransferRequestDepartmentValidations_Read([DataSourceRequest]DataSourceRequest request, int employeeTansReqId)
        {
            IQueryable<TransferRequestDepartmentValidation> requestValidations = db.TransferRequestDepartmentValidations.Where(x=>x.EmployeeTransferRequestId== employeeTansReqId);
            DataSourceResult result = requestValidations.ToDataSourceResult(request, requestValidation => new
            {

                TransferRequestDepartmentValidationId = requestValidation.TransferRequestDepartmentValidationId,
                EmployeeTransferRequestId = requestValidation.EmployeeTransferRequestId,
                Status = requestValidation.Status,
                Remark = requestValidation.Remark,
                CreatedBy = requestValidation.CreatedBy,
                DateCreated = requestValidation.DateCreated,
                LastModified = requestValidation.LastModified,
                ModifiedBy = requestValidation.ModifiedBy,
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeTransferRequestDepartmentValidations_Create([DataSourceRequest]DataSourceRequest request, TransferRequestDepartmentValidation requestValidation, int employeeTansReqId)
        {
            ModelState.Clear();
            var record = db.EmployeeTransferRequests.Find(employeeTansReqId);
            if (record != null && record.IsDepartmentOnlineApproved)
                ModelState.AddModelError("approved", "cannot manipulate this record,it is already approved");

            var lines = db.TransferRequestDetails.Where(x => x.EmployeeTransferRequestId == employeeTansReqId);
            if (!lines.Any())
                ModelState.AddModelError("lines", "Please add line items");

            if (record != null && record.IsDepartmentOnlineApproved)
                ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");

            //if (record != null && record.ForwardedTo != User.Identity.Name)
            //    ModelState.AddModelError("notcorrectuser", "You can't approve this record,it is forwarded to another user.");

            var hasApproved = db.TransferRequestDepartmentValidations
                    .Where(x => x.EmployeeTransferRequestId == employeeTansReqId)
                .Where(x => x.CreatedBy == User.Identity.Name && x.Status == ApprovalStatuses.Approve).AsQueryable();
            if (hasApproved.Any())
                ModelState.AddModelError("hasapproved", "You can't approve again.You have already approved.");

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (requestValidation.Status == ApprovalStatuses.Approve)
                    {
                        
                        record.Status = "Department Approved";
                        record.IsDepartmentOnlineApproved = true;
                        record.DepartmentOnlineApprovedBy = User.Identity.Name;
                        record.DepartmentOnlineApprovedTime = DateTime.Now;
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();

                      
                    }

                    if (requestValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsDepartmentSendForApproval = false;
                        record.Status = "Rejected";
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (requestValidation.Status == ApprovalStatuses.Pending)
                    {
                        record.IsDepartmentSendForApproval = false;
                        record.Status = "Pending";
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    requestValidation.EmployeeTransferRequestId = employeeTansReqId;
                    requestValidation.DateCreated = DateTime.Now;
                    requestValidation.CreatedBy = User.Identity.Name;
                    var entity = GetMappedObject(requestValidation);
                    db.TransferRequestDepartmentValidations.Add(entity);
                    db.SaveChanges();
                    requestValidation.TransferRequestDepartmentValidationId = entity.TransferRequestDepartmentValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Employee Transfer Request department approval", "Success", User.Identity.Name, $"approved request {entity.EmployeeTransferRequestId} ", Modules.HRM);
                    #endregion
                }
            }
            return Json(new[] { requestValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeTransferRequestDepartmentValidations_Update([DataSourceRequest]DataSourceRequest request, TransferRequestDepartmentValidation requestValidation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                requestValidation.LastModified = DateTime.Now;
                requestValidation.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(requestValidation);
                db.TransferRequestDepartmentValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { requestValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeTransferRequestDepartmentValidations_Destroy([DataSourceRequest]DataSourceRequest request, TransferRequestDepartmentValidation requestValidation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(requestValidation);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee Transfer Request Department Validation   information " + requestValidation.TransferRequestDepartmentValidationId);
                #endregion
                db.TransferRequestDepartmentValidations.Attach(entity);
                db.TransferRequestDepartmentValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { requestValidation }.ToDataSourceResult(request, ModelState));
        }
        private TransferRequestDepartmentValidation GetMappedObject(TransferRequestDepartmentValidation requestValidation)
        {
            var entity = new TransferRequestDepartmentValidation
            {
                TransferRequestDepartmentValidationId = requestValidation.TransferRequestDepartmentValidationId,
                EmployeeTransferRequestId = requestValidation.EmployeeTransferRequestId,
                Status = requestValidation.Status,
                Remark = requestValidation.Remark,
                CreatedBy= requestValidation.CreatedBy,
                DateCreated= requestValidation.DateCreated,
                LastModified= requestValidation.LastModified,
                ModifiedBy= requestValidation.ModifiedBy,
                

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
