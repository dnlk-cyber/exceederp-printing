using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
    public class RecruitmentRequestSecondValidationController : BaseController
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RecruitmentRequestSecondValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<RecruitmentRequestSecondValidation> recruitmentRequestSecondValidations = _db.RecruitmentRequestSecondValidations.Where(x=>x.RecruitmentRequestId==id);
            DataSourceResult result = recruitmentRequestSecondValidations.ToDataSourceResult(request, fixedAssetRequisitionSecondValidation => new {
                RecruitmentRequestSecondValidationId = fixedAssetRequisitionSecondValidation.RecruitmentRequestSecondValidationId,
                RecruitmentRequestId = fixedAssetRequisitionSecondValidation.RecruitmentRequestId,
                Status = fixedAssetRequisitionSecondValidation.Status,
                Remark = fixedAssetRequisitionSecondValidation.Remark,
                OrgBranchName = fixedAssetRequisitionSecondValidation.OrgBranchName,
                DateCreated = fixedAssetRequisitionSecondValidation.DateCreated,
                LastModified = fixedAssetRequisitionSecondValidation.LastModified,
                CreatedBy = fixedAssetRequisitionSecondValidation.CreatedBy,
                ModifiedBy = fixedAssetRequisitionSecondValidation.ModifiedBy
            });

            return Json(result);
        }

        //FixedAsset Requisition Second Validation
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RecruitmentRequestSecondValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, RecruitmentRequestSecondValidation fixedAssetRequisitionSecondValidation)
        {
            var storeRequisition = _db.RecruitmentRequest.Find(id);
            if (storeRequisition != null && storeRequisition.IsMiddleOnlineApproved)
            {
                ModelState.AddModelError(String.Empty, "You Can Not Manipulate this Record: It is Already Approved");
            }

            if (ModelState.IsValid)
            {
                if (storeRequisition != null)
                {
                    if (fixedAssetRequisitionSecondValidation.Status == ApprovalStatuses.Approve)
                    {

                        storeRequisition.Status = "Second Approved";
                        storeRequisition.IsMiddleOnlineApproved = true;
                        storeRequisition.MiddleOnlineApprovedBy = User.Identity.Name;
                        storeRequisition.MiddleOnlineApprovedTime = DateTime.Now;
                        _db.Entry(storeRequisition).State = EntityState.Modified;
                        _db.SaveChanges();

                    }
                    else if (fixedAssetRequisitionSecondValidation.Status == ApprovalStatuses.Reject)
                    {
                        storeRequisition.MiddleOnlineApprovedBy = "";
                        storeRequisition.IsMiddleSendForApproval = false;
                        storeRequisition.Status = "Second Approval Rejected";
                        _db.Entry(storeRequisition).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                   

                    var entity = new RecruitmentRequestSecondValidation
                    {
                        RecruitmentRequestId = id,
                        Status = fixedAssetRequisitionSecondValidation.Status,
                        Remark = fixedAssetRequisitionSecondValidation.Remark,
                        OrgBranchName = fixedAssetRequisitionSecondValidation.OrgBranchName,
                        DateCreated = fixedAssetRequisitionSecondValidation.DateCreated,
                        LastModified = fixedAssetRequisitionSecondValidation.LastModified,
                        CreatedBy = fixedAssetRequisitionSecondValidation.CreatedBy,
                        ModifiedBy = fixedAssetRequisitionSecondValidation.ModifiedBy
                    };

                    _db.RecruitmentRequestSecondValidations.Add(entity);
                    _db.SaveChanges();
                    fixedAssetRequisitionSecondValidation.RecruitmentRequestSecondValidationId = entity.RecruitmentRequestSecondValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "RecruitmentRequestSecondValidation", "Success", User.Identity.Name, $"created FixedAssetRequisitionSecondValidation  {entity.RecruitmentRequestSecondValidationId} Tag {entity.RecruitmentRequestId}", Modules.HRM);
                    #endregion
                }
            }

            return Json(new[] { fixedAssetRequisitionSecondValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
