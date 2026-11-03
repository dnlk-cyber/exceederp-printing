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
    public class RecruitmentRequestPMValidationController : BaseController
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult PMValidations_Read(int id, [DataSourceRequest] DataSourceRequest request)
        {
            var validations = _db.RecruitmentRequestPMValidations
                .Where(x => x.RecruitmentRequestId == id);

            return Json(validations.ToDataSourceResult(request, v => new
            {
                v.RecruitmentRequestPMValidationId,
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
        public ActionResult PMValidations_Create(int id, [DataSourceRequest] DataSourceRequest request, RecruitmentRequestPMValidation validation)
        {
            var recruitReq = _db.RecruitmentRequest.Find(id);
            if (recruitReq != null)
            {
                if (!recruitReq.IsProjectHRApproved)
                {
                    ModelState.AddModelError(string.Empty, "Project HR Approval is required before Project Manager approval.");
                }
                if (recruitReq.IsPMApproved)
                {
                    ModelState.AddModelError(string.Empty, "You Cannot Manipulate this Record: It is Already PM Approved.");
                }
            }

            if (ModelState.IsValid && recruitReq != null)
            {
                if (validation.Status == ApprovalStatuses.Approve)
                {
                    recruitReq.Status = "PM Approved";
                    recruitReq.IsPMApproved = true;
                    recruitReq.PMApprovedBy = User.Identity.Name;
                    recruitReq.PMApprovedDate = DateTime.Now;
                }
                else if (validation.Status == ApprovalStatuses.Reject)
                {
                    recruitReq.Status = "PM Rejected";
                    recruitReq.IsPMApproved = false;
                }

                _db.Entry(recruitReq).State = EntityState.Modified;

                var entity = new RecruitmentRequestPMValidation
                {
                    RecruitmentRequestId = id,
                    Status = validation.Status,
                    Remark = validation.Remark,
                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                _db.RecruitmentRequestPMValidations.Add(entity);
                _db.SaveChanges();
                validation.RecruitmentRequestPMValidationId = entity.RecruitmentRequestPMValidationId;

                UserHelper.OperationLog(Request.UserHostAddress, "RecruitmentRequestPMValidation", "Success", User.Identity.Name, $"PM Validation created for ID {entity.RecruitmentRequestId}", Modules.HRM);
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