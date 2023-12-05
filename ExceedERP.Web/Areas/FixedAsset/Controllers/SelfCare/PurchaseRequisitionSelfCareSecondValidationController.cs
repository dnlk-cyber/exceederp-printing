using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;
//using ExceedERP.Core.Domain.Procurement.PR;
//using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.SelfCare
{
    public class PurchaseRequisitionSelfCareSecondValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead, ProcurementRoles.ProcurementServiceRequisitionSecondApprover)]
        public ActionResult PurchaseRequisitionSecondValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PurchaseRequisitionSecondValidation> purchaserequisitionsecondvalidations = db.PurchaseRequisitionSecondValidations.Where(x=>x.PurchaseRequisitionID==id);
            DataSourceResult result = purchaserequisitionsecondvalidations.ToDataSourceResult(request, purchaseRequisitionSecondValidation => new {
                PurchaseRequisitionSecondValidationId = purchaseRequisitionSecondValidation.PurchaseRequisitionSecondValidationId,
                Status = purchaseRequisitionSecondValidation.Status,
                Remark = purchaseRequisitionSecondValidation.Remark,
                OrgBranchName = purchaseRequisitionSecondValidation.OrgBranchName,
                DateCreated = purchaseRequisitionSecondValidation.DateCreated,
                LastModified = purchaseRequisitionSecondValidation.LastModified,
                CreatedBy = purchaseRequisitionSecondValidation.CreatedBy,
                ModifiedBy = purchaseRequisitionSecondValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementServiceRequisitionSecondApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionSecondValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, PurchaseRequisitionSecondValidation purchaseRequisitionSecondValidation)
        {
            var purchaseRequisition = db.PurchaseRequisitions.Find(id);
            if (purchaseRequisition != null && purchaseRequisition.IsMiddleOnlineApproved)
            {
                ModelState.AddModelError(String.Empty, "You Can Not Manipulate this Record: It is Already Approved");
            }
            if (ModelState.IsValid)
            {
                if (purchaseRequisition != null)
                {
                    if (purchaseRequisitionSecondValidation.Status == ApprovalStatuses.Approve)
                    {

                        purchaseRequisition.Status = "Second Approved";
                        purchaseRequisition.IsMiddleOnlineApproved = true;
                        purchaseRequisition.MiddleOnlineApprovedBy = User.Identity.Name;
                        purchaseRequisition.MiddleOnlineApprovedTime = DateTime.Now;
                        db.Entry(purchaseRequisition).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                    else if (purchaseRequisitionSecondValidation.Status == ApprovalStatuses.Reject)
                    {
                        purchaseRequisition.IsMiddleSendForApproval = false;
                        purchaseRequisition.Status = "Second Approval Rejected";
                        db.Entry(purchaseRequisition).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                   
                    var entity = new PurchaseRequisitionSecondValidation
                    {
                        PurchaseRequisitionID = id,
                        Status = purchaseRequisitionSecondValidation.Status,
                        Remark = purchaseRequisitionSecondValidation.Remark,
                        OrgBranchName = purchaseRequisitionSecondValidation.OrgBranchName,
                        DateCreated = purchaseRequisitionSecondValidation.DateCreated,
                        LastModified = purchaseRequisitionSecondValidation.LastModified,
                        CreatedBy = purchaseRequisitionSecondValidation.CreatedBy,
                        ModifiedBy = purchaseRequisitionSecondValidation.ModifiedBy
                    };

                    db.PurchaseRequisitionSecondValidations.Add(entity);
                    db.SaveChanges();
                    purchaseRequisitionSecondValidation.PurchaseRequisitionSecondValidationId =
                    entity.PurchaseRequisitionSecondValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "PurchaseRequisitionSecondValidation", "Success", User.Identity.Name, $"created PurchaseRequisitionSecondValidation  {entity.PurchaseRequisitionSecondValidationId} PurchReq {entity.PurchaseRequisitionID}", Modules.FixedAsset);
                    #endregion
                }
            }

            return Json(new[] { purchaseRequisitionSecondValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementServiceRequisitionSecondApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionSecondValidations_Update([DataSourceRequest]DataSourceRequest request, PurchaseRequisitionSecondValidation purchaseRequisitionSecondValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisitionSecondValidation
                {
                    PurchaseRequisitionSecondValidationId = purchaseRequisitionSecondValidation.PurchaseRequisitionSecondValidationId,
                    Status = purchaseRequisitionSecondValidation.Status,
                    Remark = purchaseRequisitionSecondValidation.Remark,
                    OrgBranchName = purchaseRequisitionSecondValidation.OrgBranchName,
                    DateCreated = purchaseRequisitionSecondValidation.DateCreated,
                    LastModified = purchaseRequisitionSecondValidation.LastModified,
                    CreatedBy = purchaseRequisitionSecondValidation.CreatedBy,
                    ModifiedBy = purchaseRequisitionSecondValidation.ModifiedBy
                };

                db.PurchaseRequisitionSecondValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "PurchaseRequisitionSecondValidation", "Success", User.Identity.Name, $"Updated PurchaseRequisitionSecondValidation  {entity.PurchaseRequisitionSecondValidationId} PurchReq {entity.PurchaseRequisitionID}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { purchaseRequisitionSecondValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
