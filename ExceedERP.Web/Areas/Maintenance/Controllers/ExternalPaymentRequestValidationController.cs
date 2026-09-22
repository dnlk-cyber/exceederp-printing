using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OperationStatuses = ExceedERP.Core.Domain.Maintenance.OperationStatuses;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    public class ExternalPaymentRequestValidationController : Controller
    {
        // GET: Maintenance/ExternalPaymentRequestValidation
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Admin_Read([DataSourceRequest] DataSourceRequest request, string Id)
        {
            var validations = db.ExternalMaintenancePaymentRequestValidations
                            .Where(x => x.ExternalMaintenancePaymenntrequestId.ToString() == Id)
                            .ToList();

            var result = validations.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Admin_Create(int id, [DataSourceRequest]DataSourceRequest request, ExternalMaintenancePaymentRequestValidation ExternalPaymentValidation)
        {
            var record = db.ExternalMaintenancePaymentRequests.Find(id);
            if (record != null && record.TotalAmount <= 0)
                ModelState.AddModelError("payable", "Net payable should be more than 0.");

            //if (record.PaymentType == PaymentType.Credit) //PaymentType.Payment && record.PurchasePaymentType == PurchasePaymentType.Credit && PurchasePaymentValidation.Status==ApprovalStatuses.Approve)
            //{
            //    //check if there is grn
            //    var grns = _db.PurchasePaymentGrns.Where(x => x.PurchasePaymentId == record.PurchasePaymentId);
            //    if (!grns.Any())
            //        ModelState.AddModelError("grn", "Please add GRNs first.");

            //}
            if (record != null && record.IsOnlineApproved)
                ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");

            if (record != null && record.ForwardedTo != User.Identity.Name)
                ModelState.AddModelError("notcorrectuser", "You can't approve this record,it is forwarded to another user.");

            var hasApproved = db.ExternalMaintenancePaymentRequestValidations
                    .Where(x => x.ExternalMaintenancePaymenntrequestId == id)
                .Where(x => x.CreatedBy == User.Identity.Name && x.Status == ApprovalStatuses.Approve).AsQueryable();
            if (hasApproved.Any())
                ModelState.AddModelError("hasapproved", "You can't approve again.You have already approved.");
            if (record != null && record.IsOnlineApproved)
            {
                ModelState.AddModelError(String.Empty, "You Can Not Manipulate This Record: It is Already Approved");
            }
            if (record != null && !record.IsSendForApproval)
            {
                ModelState.AddModelError(String.Empty, "You Can Not Manipulate This Record: It is not send for Approval");
            }
            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (ExternalPaymentValidation.Status == ApprovalStatuses.Approve)
                    {

                        if (record.IsFinalApproval)
                        {
                            record.Status = "Approved";
                            record.IsOnlineApproved = true;
                            record.IsVoid = false;
                            record.OnlineApprovedBy = User.Identity.Name;
                            record.OnlineApprovedTime = DateTime.Now;
                            db.Entry(record).State = EntityState.Modified;
                            db.SaveChanges();
                            #region approval notification
                            NotifModels notif = new NotifModels
                            {
                                UserId = record.ForwardedFrom,
                                URL = DocApprovalWorkflowHelper.GetDocUrl(Core.Domain.Common.DocumentApproval.ApprovalDocumentType.ExternalMaintenanceRequestPayment, record.ExternalMaintenancePaymenntrequestId),
                                Message =
                                    $"External maintenance payment request document {record.ExternalMaintenancePaymenntrequestId} is approved by User: {record.ForwardedTo} at {DateTime.Now}"
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
                            db.Entry(record).State = EntityState.Modified;
                            db.SaveChanges();

                            #region approval notification

                            NotifModels notif = new NotifModels
                            {
                                UserId = record.ForwardedFrom,
                                URL = DocApprovalWorkflowHelper.GetDocUrl(Core.Domain.Common.DocumentApproval.ApprovalDocumentType.ExternalMaintenanceRequestPayment, record.ExternalMaintenancePaymenntrequestId),
                                Message =
                                      $"External maintenance payment request document {record.ExternalMaintenancePaymenntrequestId} is approved by User: {record.ForwardedTo} at {DateTime.Now} and forwarded to next user."
                            };

                            Notify.NotifyUser(notif);

                            #endregion

                            #region forward to next approver
                            decimal docamount = record.TotalAmount;
                            int step = record.CurrentApprovalStep != null ? (int)record.CurrentApprovalStep : 0;
                            DocApprovalWorkflowHelper.ForwardToApprover(record.ExternalMaintenancePaymenntrequestId, docamount, "",
                                record.ForwardedTo, Core.Domain.Common.DocumentApproval.ApprovalDocumentType.ExternalMaintenanceRequestPayment, false, step);

                            #endregion

                        }
                    }

                    if (ExternalPaymentValidation.Status == ApprovalStatuses.Reject)
                    {

                        record.Status = Enum.GetName(typeof(OperationStatuses), 8);
                        record.IsOnlineApproved = false;
                        record.IsSendForApproval = false;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now.Date;
                        db.ExternalMaintenancePaymentRequests.Attach(record);
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    var entity = new ExternalMaintenancePaymentRequestValidation
                    {
                        ExternalMaintenancePaymenntrequestId = id,
                        Status = ExternalPaymentValidation.Status,
                        Remark = ExternalPaymentValidation.Remark,
                        OrgBranchName = ExternalPaymentValidation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = ExternalPaymentValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = ExternalPaymentValidation.ModifiedBy
                    };

                    db.ExternalMaintenancePaymentRequestValidations.Add(entity);
                    db.SaveChanges();
                    ExternalPaymentValidation.ExternalMaintenancePaymentRequestValidationId = entity.ExternalMaintenancePaymentRequestValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "External Maintenance payment approval", "Success", User.Identity.Name, $"approved payment {entity.ExternalMaintenancePaymenntrequestId} ", Modules.Maintenance);
                    #endregion
                }
            }

            return Json(new[] { ExternalPaymentValidation }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult ApproveJobRequest(int id)
        {
            var request = new DataSourceRequest();
            // var jobRequest = db.JobRequest.Find(id);

            //try
            //{

           
            //  var model = db.AdministrativeJobApproval.Find(id);

            var Request = db.ExternalMaintenancePaymentRequests.FirstOrDefault(x => x.ExternalMaintenancePaymenntrequestId == id);           
            if (Request != null/* && fleet != null*/)
            {

                //if (model.Status == ApprovalStatuses.Approve)
                //{
                Request.IsOnlineApproved = true;
                Request.OnlineApprovedBy = User.Identity.Name;
                Request.OnlineApprovedTime = DateTime.Now;
                Request.Status = ApprovalStatuses.Approve.ToString();
               
                db.Entry(Request).State = EntityState.Modified;
                db.SaveChanges();
                var adminApproval = db.ExternalMaintenancePaymentRequestValidations.FirstOrDefault(x => x.ExternalMaintenancePaymenntrequestId == id);
                adminApproval.Status = ApprovalStatuses.Approve;
                db.Entry(adminApproval).State = EntityState.Modified;
                db.SaveChanges();
            }
            var formattedData = new
            {
                Message = "Approved Successefuly.",
                Status = "Approved"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RejectJobRequest(int id)

        {
            var Request = db.ExternalMaintenancePaymentRequests.FirstOrDefault(x => x.ExternalMaintenancePaymenntrequestId == id);
            // var jobRequest = db.JobRequest.Find(id);

            try
            {
                if (Request != null)
                {

                    Request.IsSendForApproval = false;
                    Request.IsOnlineApproved = false;
                    Request.Status = MaintenanceConst.Rejected;
                    db.ExternalMaintenancePaymentRequests.Attach(Request);
                    db.Entry(Request).State = EntityState.Modified;
                    db.SaveChanges();
                    var adminApproval = db.ExternalMaintenancePaymentRequestValidations.FirstOrDefault(x => x.ExternalMaintenancePaymenntrequestId == id);
                    adminApproval.Status = ApprovalStatuses.Reject;
                    db.Entry(adminApproval).State = EntityState.Modified;
                    db.Entry(adminApproval).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }

            catch (Exception e)
            {
                return Json(e.Message, JsonRequestBehavior.AllowGet);

            }
            var formattedData = new
            {
                Message = "Rejected Successefuly.",
                Status = "Rejected"
            };
            return Json("Rejected Successefuly.", JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}