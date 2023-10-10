using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PaymentRequest;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.PaymentRequest
{
    public class PurchasePaymentValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementPaymentUser, ProcurementRoles.ProcurementPaymentApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementPaymentUser, ProcurementRoles.ProcurementPaymentApprover)]
        public ActionResult PurchasePaymentValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PurchasePaymentValidation> PurchasePaymentValidations = _db.PurchasePaymentValidations.Where(x=>x.PurchasePaymentId==id);
            DataSourceResult result = PurchasePaymentValidations.ToDataSourceResult(request, PurchasePaymentValidation => new {
                PurchasePaymentValidationId = PurchasePaymentValidation.PurchasePaymentValidationId,
                Status = PurchasePaymentValidation.Status,
                Remark = PurchasePaymentValidation.Remark,
                OrgBranchName = PurchasePaymentValidation.OrgBranchName,
                DateCreated = PurchasePaymentValidation.DateCreated,
                LastModified = PurchasePaymentValidation.LastModified,
                CreatedBy = PurchasePaymentValidation.CreatedBy,
                ModifiedBy = PurchasePaymentValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementPaymentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePaymentValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, PurchasePaymentValidation PurchasePaymentValidation)
        {
            var record = _db.PurchasePayments.Find(id);
            if (record.PaymentType ==PaymentType.Credit) //PaymentType.Payment && record.PurchasePaymentType == PurchasePaymentType.Credit && PurchasePaymentValidation.Status==ApprovalStatuses.Approve)
            {
                //check if there is grn
                var grns = _db.PurchasePaymentGrns.Where(x => x.PurchasePaymentId == record.PurchasePaymentId);
                if (!grns.Any())
                    ModelState.AddModelError("grn", "Please add GRNs first.");              

            }
            if (record != null && record.IsOnlineApproved)
                ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");

            if (record != null && record.ForwardedTo != User.Identity.Name)
                ModelState.AddModelError("notcorrectuser", "You can't approve this record,it is forwarded to another user.");

            var hasApproved = _db.PurchasePaymentValidations
                    .Where(x => x.PurchasePaymentId == id)
                .Where(x => x.CreatedBy == User.Identity.Name && x.Status == ApprovalStatuses.Approve).AsQueryable();
            if (hasApproved.Any())
                ModelState.AddModelError("hasapproved", "You can't approve again.You have already approved.");

            if (record != null && record.IsOnlineApproved)
            {
                ModelState.AddModelError(String.Empty, "You Can Not Manipulate This Record: It is Already Approved");
            }
            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (PurchasePaymentValidation.Status == ApprovalStatuses.Approve)
                    {
                        if (record.IsFinalApproval)
                        {
                            record.Status = "Approved";
                            record.IsOnlineApproved = true;
                            record.IsVoid = false;
                            record.OnlineApprovedBy = User.Identity.Name;
                            record.OnlineApprovedTime = DateTime.Now;
                            _db.Entry(record).State = EntityState.Modified;
                            _db.SaveChanges();

                            #region approval notification

                            NotifModels notif = new NotifModels
                            {
                                UserId = record.ForwardedFrom,
                                URL = DocApprovalWorkflowHelper.GetDocUrl(ApprovalDocumentType.PurchasePaymentRequest, record.PurchasePaymentId),
                                Message =
                                    $"Purchase payment request document {record.PurchasePaymentId} is approved by User: {record.ForwardedTo} at {DateTime.Now}"
                            };

                            Notify.NotifyUser(notif);

                            #endregion
                        }
                        else
                        {
                            record.Status = "Partially Approved";
                            record.IsOnlineApproved = false;
                            record.IsVoid = false;
                            record.OnlineApprovedBy = User.Identity.Name;
                            record.OnlineApprovedTime = DateTime.Now;
                            _db.Entry(record).State = EntityState.Modified;
                            _db.SaveChanges();

                            #region approval notification

                            NotifModels notif = new NotifModels
                            {
                                UserId = record.ForwardedFrom,
                                URL = DocApprovalWorkflowHelper.GetDocUrl(ApprovalDocumentType.PurchaseOrder, record.PurchasePaymentId),
                                Message =
                                      $"Purchase payment request document {record.PurchasePaymentId} is approved by User: {record.ForwardedTo} at {DateTime.Now} and forwarded to next user."
                            };

                            Notify.NotifyUser(notif);

                            #endregion

                            #region forward to next approver

                            decimal docamount = record.NetPayable;
                            int step = record.CurrentApprovalStep != null ? (int)record.CurrentApprovalStep : 0;
                            DocApprovalWorkflowHelper.ForwardToApprover(record.PurchasePaymentId, docamount, "",
                                record.ForwardedTo, ApprovalDocumentType.PurchasePaymentRequest, false, step);

                            #endregion

                        }
                    }

                    if (PurchasePaymentValidation.Status == ApprovalStatuses.Reject)
                    {

                        record.Status = Enum.GetName(typeof(OperationStatuses), 8);
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now.Date;
                        _db.PurchasePayments.Attach(record);
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                  
                var entity = new PurchasePaymentValidation
                {
                    PurchasePaymentId = id,
                    Status = PurchasePaymentValidation.Status,
                    Remark = PurchasePaymentValidation.Remark,
                    OrgBranchName = PurchasePaymentValidation.OrgBranchName,
                    DateCreated =DateTime.Now,
                    LastModified = PurchasePaymentValidation.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = PurchasePaymentValidation.ModifiedBy
                };

                _db.PurchasePaymentValidations.Add(entity);
                _db.SaveChanges();
                PurchasePaymentValidation.PurchasePaymentValidationId = entity.PurchasePaymentValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Purchase payment approval", "Success", User.Identity.Name, $"approved payment {entity.PurchasePaymentId} ", Modules.Procurement);
                    #endregion
                }
            }

            return Json(new[] { PurchasePaymentValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementPaymentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePaymentValidations_Update([DataSourceRequest]DataSourceRequest request, PurchasePaymentValidation PurchasePaymentValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchasePaymentValidation
                {
                    PurchasePaymentValidationId = PurchasePaymentValidation.PurchasePaymentValidationId,
                    Status = PurchasePaymentValidation.Status,
                    Remark = PurchasePaymentValidation.Remark,
                    OrgBranchName = PurchasePaymentValidation.OrgBranchName,
                    DateCreated = PurchasePaymentValidation.DateCreated,
                    LastModified = PurchasePaymentValidation.LastModified,
                    CreatedBy = PurchasePaymentValidation.CreatedBy,
                    ModifiedBy = PurchasePaymentValidation.ModifiedBy
                };

                _db.PurchasePaymentValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { PurchasePaymentValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementPaymentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePaymentValidations_Destroy([DataSourceRequest]DataSourceRequest request, PurchasePaymentValidation PurchasePaymentValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchasePaymentValidation
                {
                    PurchasePaymentValidationId = PurchasePaymentValidation.PurchasePaymentValidationId,
                    Status = PurchasePaymentValidation.Status,
                    Remark = PurchasePaymentValidation.Remark,
                    OrgBranchName = PurchasePaymentValidation.OrgBranchName,
                    DateCreated = PurchasePaymentValidation.DateCreated,
                    LastModified = PurchasePaymentValidation.LastModified,
                    CreatedBy = PurchasePaymentValidation.CreatedBy,
                    ModifiedBy = PurchasePaymentValidation.ModifiedBy
                };

                _db.PurchasePaymentValidations.Attach(entity);
                _db.PurchasePaymentValidations.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { PurchasePaymentValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
