using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Requisitions;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead,  FixedAssetConstants.FixedAssetRequistionSecondApprover)]
    public class FixedAssetRequisitionSecondSelfCareValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

       
        public ActionResult FixedAssetRequisitionSecondValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FixedAssetRequisitionSecondValidation> fixedassetrequisitionsecondvalidations = _db.FixedAssetRequisitionSecondValidations.Where(x=>x.FixedAssetRequisitionId==id);
            DataSourceResult result = fixedassetrequisitionsecondvalidations.ToDataSourceResult(request, fixedAssetRequisitionSecondValidation => new {
                FixedAssetRequisitionSecondValidationId = fixedAssetRequisitionSecondValidation.FixedAssetRequisitionSecondValidationId,
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

        [AuthorizeRoles(FixedAssetConstants.FixedAssetRequistionSecondApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetRequisitionSecondValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, FixedAssetRequisitionSecondValidation fixedAssetRequisitionSecondValidation)
        {
            var storeRequisition = _db.FixedAssetRequisitions.Find(id);
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

                   

                    var entity = new FixedAssetRequisitionSecondValidation
                    {
                        FixedAssetRequisitionId = id,
                        Status = fixedAssetRequisitionSecondValidation.Status,
                        Remark = fixedAssetRequisitionSecondValidation.Remark,
                        OrgBranchName = fixedAssetRequisitionSecondValidation.OrgBranchName,
                        DateCreated = fixedAssetRequisitionSecondValidation.DateCreated,
                        LastModified = fixedAssetRequisitionSecondValidation.LastModified,
                        CreatedBy = fixedAssetRequisitionSecondValidation.CreatedBy,
                        ModifiedBy = fixedAssetRequisitionSecondValidation.ModifiedBy
                    };

                    _db.FixedAssetRequisitionSecondValidations.Add(entity);
                    _db.SaveChanges();
                    fixedAssetRequisitionSecondValidation.FixedAssetRequisitionSecondValidationId =
                        entity.FixedAssetRequisitionSecondValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetRequisitionSecondValidation", "Success", User.Identity.Name, $"created FixedAssetRequisitionSecondValidation  {entity.FixedAssetRequisitionSecondValidationId} Tag {entity.FixedAssetRequisitionId}", Modules.FixedAsset);
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
