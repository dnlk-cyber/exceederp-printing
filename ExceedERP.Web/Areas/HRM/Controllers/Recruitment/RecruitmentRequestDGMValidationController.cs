using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
    public class RecruitmentRequestDGMValidationController : BaseController
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult DGMValidations_Read(int id, [DataSourceRequest] DataSourceRequest request)
        {
            var validations = _db.RecruitmentRequestDGMValidations
                .Where(x => x.RecruitmentRequestId == id);

            return Json(validations.ToDataSourceResult(request, v => new
            {
                v.RecruitmentRequestDGMValidationId,
                v.RecruitmentRequestId,
                v.Status,
                v.Remark,
                v.OrgBranchName,
                v.DateCreated,
                v.CreatedBy,
                v.LastModified,
                v.ModifiedBy
            }));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DGMValidations_Create(int id, [DataSourceRequest] DataSourceRequest request, RecruitmentRequestDGMValidation validation)
        {
            var recruitReq = _db.RecruitmentRequest.Find(id);
            if (recruitReq != null)
            {
                if (!recruitReq.IsOnlineApproved)
                {
                    ModelState.AddModelError(string.Empty, "Head Office HR Approval is required before DGM approval.");
                }
                if (recruitReq.IsDGMApproved)
                {
                    ModelState.AddModelError(string.Empty, "You Cannot Manipulate this Record: It is Already DGM Approved.");
                }
            }

            if (ModelState.IsValid && recruitReq != null)
            {
                if (validation.Status == ApprovalStatuses.Approve)
                {
                    recruitReq.Status = "DGM Approved";
                    recruitReq.IsDGMApproved = true;
                    recruitReq.DGMApprovedBy = User.Identity.Name;
                    recruitReq.DGMApprovedDate = DateTime.Now;
                }
                else if (validation.Status == ApprovalStatuses.Reject)
                {
                    recruitReq.Status = "DGM Rejected";
                    recruitReq.IsDGMApproved = false;
                }

                _db.Entry(recruitReq).State = EntityState.Modified;

                var entity = new RecruitmentRequestDGMValidation
                {
                    RecruitmentRequestId = id,
                    Status = validation.Status,
                    Remark = validation.Remark,
                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                _db.RecruitmentRequestDGMValidations.Add(entity);
                _db.SaveChanges();
                validation.RecruitmentRequestDGMValidationId = entity.RecruitmentRequestDGMValidationId;

                UserHelper.OperationLog(Request.UserHostAddress, "RecruitmentRequestDGMValidation", "Success", User.Identity.Name, $"DGM Validation created for ID {entity.RecruitmentRequestId}", Modules.HRM);
            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}