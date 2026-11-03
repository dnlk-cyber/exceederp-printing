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
    public class RecruitmentRequestGMValidationController : BaseController
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult GMValidations_Read(int id, [DataSourceRequest] DataSourceRequest request)
        {
            var validations = _db.RecruitmentRequestGMValidations
                .Where(x => x.RecruitmentRequestId == id);

            return Json(validations.ToDataSourceResult(request, v => new
            {
                v.RecruitmentRequestGMValidationId,
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
        public ActionResult GMValidations_Create(int id, [DataSourceRequest] DataSourceRequest request, RecruitmentRequestGMValidation validation)
        {
            var recruitReq = _db.RecruitmentRequest.Find(id);
            if (recruitReq != null)
            {
                if (!recruitReq.IsDGMApproved)
                {
                    ModelState.AddModelError(string.Empty, "DGM Approval is required before General Manager authorization.");
                }
                if (recruitReq.IsGMApproved)
                {
                    ModelState.AddModelError(string.Empty, "You Cannot Manipulate this Record: It is Already GM Authorized.");
                }
            }

            if (ModelState.IsValid && recruitReq != null)
            {
                if (validation.Status == ApprovalStatuses.Approve)
                {
                    recruitReq.Status = "GM Authorized";
                    recruitReq.IsGMApproved = true;
                    recruitReq.GMApprovedBy = User.Identity.Name;
                    recruitReq.GMApprovedDate = DateTime.Now;
                }
                else if (validation.Status == ApprovalStatuses.Reject)
                {
                    recruitReq.Status = "GM Rejected";
                    recruitReq.IsGMApproved = false;
                }

                _db.Entry(recruitReq).State = EntityState.Modified;

                var entity = new RecruitmentRequestGMValidation
                {
                    RecruitmentRequestId = id,
                    Status = validation.Status,
                    Remark = validation.Remark,
                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                _db.RecruitmentRequestGMValidations.Add(entity);
                _db.SaveChanges();
                validation.RecruitmentRequestGMValidationId = entity.RecruitmentRequestGMValidationId;

                UserHelper.OperationLog(Request.UserHostAddress, "RecruitmentRequestGMValidation", "Success", User.Identity.Name, $"GM Authorization created for ID {entity.RecruitmentRequestId}", Modules.HRM);
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