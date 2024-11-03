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

    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser)]
    public class EmployeeTransferRequestValidationsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
     
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRRecruitmentUser)]
        public ActionResult EmployeeTransferRequestValidations_Read([DataSourceRequest]DataSourceRequest request, int employeeTRId)
        {
            IQueryable<TransferRequestValidation> requestValidations = db.TransferRequestValidations.Where(x=>x.EmployeeTransferRequestId== employeeTRId);
            DataSourceResult result = requestValidations.ToDataSourceResult(request, requestValidation => new
            {

                TransferRequestValidationId = requestValidation.TransferRequestValidationId,
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
        //public ActionResult EmployeeTransferRequestValidations_Create([DataSourceRequest]DataSourceRequest request, TransferRequestValidation requestValidation, int employeeTRId)
        //{
        //    ModelState.Clear();
        //    if (ModelState.IsValid)
        //    {
        //        requestValidation.EmployeeTransferRequestId = employeeTRId;
        //        requestValidation.DateCreated = DateTime.Now;
        //        requestValidation.CreatedBy = User.Identity.Name;
        //        var entity = GetMappedObject(requestValidation);
        //        db.TransferRequestValidations.Add(entity);
        //        db.SaveChanges();
        //        requestValidation.TransferRequestValidationId = entity.TransferRequestValidationId;
        //    }

        //    return Json(new[] { requestValidation }.ToDataSourceResult(request, ModelState));
        //}
        public ActionResult EmployeeTransferRequestValidations_Create([DataSourceRequest]DataSourceRequest request, TransferRequestValidation requestValidation, int employeeTRId)
        {
            ModelState.Clear();
            var record = db.EmployeeTransferRequests.Find(employeeTRId);
            if (record != null && record.IsOnlineApproved)
                ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");
            var check = db.TransferRequestValidations.Where(x => x.EmployeeTransferRequestId == employeeTRId).Any();
            if (check)
            {
                ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");

            }
            if (ModelState.IsValid)
            {

                if (record != null)
                {
                    if (requestValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();

                    }

                    if (requestValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "HR Rejected";
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    requestValidation.EmployeeTransferRequestId = employeeTRId;
                    requestValidation.DateCreated = DateTime.Now;
                    requestValidation.CreatedBy = User.Identity.Name;
                    var entity = GetMappedObject(requestValidation);
                    db.TransferRequestValidations.Add(entity);
                    db.SaveChanges();
                    requestValidation.TransferRequestValidationId = entity.TransferRequestValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Employee Transfer Request approval", "Success", User.Identity.Name, $"approved travel order {entity.EmployeeTransferRequest} ", Modules.HRM);
                    #endregion
                }
            }

            return Json(new[] { requestValidation }.ToDataSourceResult(request, ModelState));
        }
        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeTransferRequestValidations_Update([DataSourceRequest]DataSourceRequest request, TransferRequestValidation requestValidation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                requestValidation.LastModified = DateTime.Now;
                requestValidation.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(requestValidation);
                db.TransferRequestValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { requestValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeTransferRequestValidations_Destroy([DataSourceRequest]DataSourceRequest request, TransferRequestValidation requestValidation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(requestValidation);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee Transfer   information of applicant" + requestValidation.TransferRequestValidationId);
                #endregion
                db.TransferRequestValidations.Attach(entity);
                db.TransferRequestValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { requestValidation }.ToDataSourceResult(request, ModelState));
        }
        private TransferRequestValidation GetMappedObject(TransferRequestValidation requestValidation)
        {
            var entity = new TransferRequestValidation
            {
                TransferRequestValidationId = requestValidation.TransferRequestValidationId,
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
