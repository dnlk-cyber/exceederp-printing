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

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{

    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser)]
    public class EmployeeTransferRequestDetailsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
     
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser)]
        public ActionResult EmployeeTransferRequestDetails_Read([DataSourceRequest]DataSourceRequest request, int empTRId)
        {
            IQueryable<TransferRequestDetail> requestDetails = db.TransferRequestDetails.Where(x=>x.EmployeeTransferRequestId==empTRId);
            DataSourceResult result = requestDetails.ToDataSourceResult(request, requestDetail => new
            {

                TransferRequestDetailId = requestDetail.TransferRequestDetailId,
                EmployeeTransferRequestId = requestDetail.EmployeeTransferRequestId,
                EmployeeId = requestDetail.EmployeeId,
                Quantity = requestDetail.Quantity,
                JobTitleId = requestDetail.JobTitleId,
                Description = requestDetail.Description,
                CreatedBy = requestDetail.CreatedBy,
                DateCreated = requestDetail.DateCreated,
                LastModified = requestDetail.LastModified,
                ModifiedBy = requestDetail.ModifiedBy,
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeTransferRequestDetails_Create([DataSourceRequest]DataSourceRequest request, TransferRequestDetail requestDetail, int empTRId)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                requestDetail.EmployeeTransferRequestId = empTRId;
                requestDetail.DateCreated = DateTime.Now;
                requestDetail.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(requestDetail);
                db.TransferRequestDetails.Add(entity);
                db.SaveChanges();
                requestDetail.TransferRequestDetailId = entity.TransferRequestDetailId;
            }

            return Json(new[] { requestDetail }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeTransferRequestDetails_Update([DataSourceRequest]DataSourceRequest request, TransferRequestDetail requestDetail)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                requestDetail.LastModified = DateTime.Now;
                requestDetail.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(requestDetail);
                db.TransferRequestDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { requestDetail }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeTransferRequestDetails_Destroy([DataSourceRequest]DataSourceRequest request, TransferRequestDetail requestDetail)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(requestDetail);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  Transfer Request Detail   information of " + requestDetail.TransferRequestDetailId+" "+User.Identity.Name);
                #endregion
                db.TransferRequestDetails.Attach(entity);
                db.TransferRequestDetails.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { requestDetail }.ToDataSourceResult(request, ModelState));
        }
        private TransferRequestDetail GetMappedObject(TransferRequestDetail requestDetail)
        {
            var entity = new TransferRequestDetail
            {
                TransferRequestDetailId = requestDetail.TransferRequestDetailId,
                EmployeeTransferRequestId = requestDetail.EmployeeTransferRequestId,
                EmployeeId = requestDetail.EmployeeId,
                Quantity = requestDetail.Quantity,
                JobTitleId= requestDetail.JobTitleId,
                Description= requestDetail.Description,
                CreatedBy= requestDetail.CreatedBy,
                DateCreated= requestDetail.DateCreated,
                LastModified= requestDetail.LastModified,
                ModifiedBy= requestDetail.ModifiedBy,
                

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
