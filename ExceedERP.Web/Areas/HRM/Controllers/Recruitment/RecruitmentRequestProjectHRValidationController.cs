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
    public class RecruitmentRequestProjectHRValidationController : BaseController
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult ProjectHRValidations_Read(int id, [DataSourceRequest] DataSourceRequest request)
        {
            var validations = _db.RecruitmentRequestProjectHRValidations
                .Where(x => x.RecruitmentRequestId == id);

            return Json(validations.ToDataSourceResult(request, v => new
            {
                v.RecruitmentRequestProjectHRValidationId,
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
        public ActionResult ProjectHRValidations_Create(int id, [DataSourceRequest] DataSourceRequest request, RecruitmentRequestProjectHRValidation validation)
        {
            var recruitReq = _db.RecruitmentRequest.Find(id);
            if (recruitReq != null && recruitReq.IsProjectHRApproved)
            {
                ModelState.AddModelError(string.Empty, "You Cannot Manipulate this Record: It is Already Approved by Project HR");
            }

            if (ModelState.IsValid && recruitReq != null)
            {
                if (validation.Status == ApprovalStatuses.Approve)
                {
                    recruitReq.Status = "Project HR Approved";
                    recruitReq.IsProjectHRApproved = true;
                    recruitReq.ProjectHRApprovedBy = User.Identity.Name;
                    recruitReq.ProjectHRApprovedDate = DateTime.Now;
                }
                else if (validation.Status == ApprovalStatuses.Reject)
                {
                    recruitReq.Status = "Project HR Rejected";
                    recruitReq.IsProjectHRApproved = false;
                }

                _db.Entry(recruitReq).State = EntityState.Modified;

                var entity = new RecruitmentRequestProjectHRValidation
                {
                    RecruitmentRequestId = id,
                    Status = validation.Status,
                    Remark = validation.Remark,
                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                _db.RecruitmentRequestProjectHRValidations.Add(entity);
                _db.SaveChanges();
                validation.RecruitmentRequestProjectHRValidationId = entity.RecruitmentRequestProjectHRValidationId;

                UserHelper.OperationLog(Request.UserHostAddress, "RecruitmentRequestProjectHRValidation", "Success", User.Identity.Name, $"Project HR Validation created for ID {entity.RecruitmentRequestId}", Modules.HRM);
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