using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.SelfCare.ServiceRequest
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
    public class ServiceRequisitionDepartmentValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
        public ActionResult PurchaseRequisitionUserDepartmentValidations_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<PurchaseRequisitionUserDepartmentValidation> PurchaseRequisitionUserDepartmentValidations = db.PurchaseRequisitionUserDepartmentValidations.Where(x=>x.PurchaseRequisitionID==id);
            DataSourceResult result = PurchaseRequisitionUserDepartmentValidations.ToDataSourceResult(request, PurchaseRequisitionUserDepartmentValidation => new {
                PurchaseRequisitionUserDepartmentValidationID = PurchaseRequisitionUserDepartmentValidation.PurchaseRequisitionUserDepartmentValidationID,
                Status = PurchaseRequisitionUserDepartmentValidation.Status,
                Remark = PurchaseRequisitionUserDepartmentValidation.Remark,
                OrgBranchName = PurchaseRequisitionUserDepartmentValidation.OrgBranchName,
                DateCreated = PurchaseRequisitionUserDepartmentValidation.DateCreated,
                LastModified = PurchaseRequisitionUserDepartmentValidation.LastModified,
                CreatedBy = PurchaseRequisitionUserDepartmentValidation.CreatedBy,
                ModifiedBy = PurchaseRequisitionUserDepartmentValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionUserDepartmentValidations_Create([DataSourceRequest]DataSourceRequest request, PurchaseRequisitionUserDepartmentValidation PurchaseRequisitionUserDepartmentValidation, int id)
        {
            var record = db.PurchaseRequisitions.Find(id);
            if (record != null && record.IsDepartmentOnlineApproved)
                ModelState.AddModelError("approved", "cannot manipulate this record,it is already approved");

            var lines = db.PurchaseRequisitionItems.Where(x => x.PurchaseRequisitionID == id);
            if (!lines.Any())
                ModelState.AddModelError("lines", "Please add line items");

            if (record != null && record.IsDepartmentOnlineApproved)
                ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");

            if (record != null && record.ForwardedTo != User.Identity.Name)
                ModelState.AddModelError("notcorrectuser", "You can't approve this record,it is forwarded to another user.");

            var hasApproved = db.PurchaseRequisitionUserDepartmentValidations
                    .Where(x => x.PurchaseRequisitionID == id)
                .Where(x => x.CreatedBy == User.Identity.Name && x.Status == ApprovalStatuses.Approve).AsQueryable();
            if (hasApproved.Any())
                ModelState.AddModelError("hasapproved", "You can't approve again.You have already approved.");

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (PurchaseRequisitionUserDepartmentValidation.Status == ApprovalStatuses.Approve)
                    {
                        if (record.IsFinalApproval)
                        {
                            record.Status = "Department Approved";
                            record.IsVoid = false;
                            record.IsDepartmentOnlineApproved = true;
                            record.DepartmentOnlineApprovedBy = User.Identity.Name;
                            record.DepartmentOnlineApprovedTime = DateTime.Now;
                            db.Entry(record).State = EntityState.Modified;
                            db.SaveChanges();
                            #region approval notification

                            NotifModels notif = new NotifModels
                            {
                                UserId = record.ForwardedFrom,
                                URL = DocApprovalWorkflowHelper.GetDocUrl(ApprovalDocumentType.ApTravelOrderSelfCare, record.PurchaseRequisitionID),
                                Message =
                                    $"Service request document {record.PurchaseRequisitionID} is approved by User: {record.ForwardedTo} at {DateTime.Now}"
                            };

                            Notify.NotifyUser(notif);

                            #endregion
                        }
                        else
                        {
                            record.Status = "Partially Approved";
                            record.IsDepartmentOnlineApproved = false;
                            record.IsVoid = false;
                            record.DepartmentOnlineApprovedBy = User.Identity.Name;
                            record.DepartmentOnlineApprovedTime = DateTime.Now;
                            db.Entry(record).State = EntityState.Modified;
                            db.SaveChanges();

                            #region approval notification

                            NotifModels notif = new NotifModels
                            {
                                UserId = record.ForwardedFrom,
                                URL = DocApprovalWorkflowHelper.GetDocUrl(ApprovalDocumentType.ApTravelOrderSelfCare, record.PurchaseRequisitionID),
                                Message =
                                    $"Service request document {record.PurchaseRequisitionID} is approved by User: {record.ForwardedTo} at {DateTime.Now} and forwarded to next user."
                            };

                            Notify.NotifyUser(notif);

                            #endregion

                            #region forward to next approver

                            decimal docamount = 0;
                          
                            int step = record.CurrentApprovalStep != null ? (int)record.CurrentApprovalStep : 0;
                            DocApprovalWorkflowHelper.ForwardToApprover(record.PurchaseRequisitionID, docamount, "",
                                record.ForwardedTo, ApprovalDocumentType.PurchaseRequisitionSelfCare, false, step);

                            #endregion
                        }
                    }

                    if (PurchaseRequisitionUserDepartmentValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsDepartmentSendForApproval = false;
                        record.Status = "Rejected";
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    var entity = new PurchaseRequisitionUserDepartmentValidation
                    {
                        PurchaseRequisitionID = id,
                        Status = PurchaseRequisitionUserDepartmentValidation.Status,
                        Remark = PurchaseRequisitionUserDepartmentValidation.Remark,
                        OrgBranchName = PurchaseRequisitionUserDepartmentValidation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = PurchaseRequisitionUserDepartmentValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = PurchaseRequisitionUserDepartmentValidation.ModifiedBy
                    };

                    db.PurchaseRequisitionUserDepartmentValidations.Add(entity);
                    db.SaveChanges();
                    PurchaseRequisitionUserDepartmentValidation.PurchaseRequisitionUserDepartmentValidationID = entity.PurchaseRequisitionUserDepartmentValidationID;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Service request department approval", "Success", User.Identity.Name, $"approved request {entity.PurchaseRequisitionID} ", Modules.Procurement);
                    #endregion
                }
            }

            return Json(new[] { PurchaseRequisitionUserDepartmentValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionUserDepartmentValidations_Update([DataSourceRequest]DataSourceRequest request, PurchaseRequisitionUserDepartmentValidation PurchaseRequisitionUserDepartmentValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisitionUserDepartmentValidation
                {
                    PurchaseRequisitionID = PurchaseRequisitionUserDepartmentValidation.PurchaseRequisitionID,
                    PurchaseRequisitionUserDepartmentValidationID = PurchaseRequisitionUserDepartmentValidation.PurchaseRequisitionUserDepartmentValidationID,
                    Status = PurchaseRequisitionUserDepartmentValidation.Status,
                    Remark = PurchaseRequisitionUserDepartmentValidation.Remark,
                    OrgBranchName = PurchaseRequisitionUserDepartmentValidation.OrgBranchName,
                    DateCreated = PurchaseRequisitionUserDepartmentValidation.DateCreated,
                    LastModified = PurchaseRequisitionUserDepartmentValidation.LastModified,
                    CreatedBy = PurchaseRequisitionUserDepartmentValidation.CreatedBy,
                    ModifiedBy = PurchaseRequisitionUserDepartmentValidation.ModifiedBy
                };

                db.PurchaseRequisitionUserDepartmentValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { PurchaseRequisitionUserDepartmentValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
