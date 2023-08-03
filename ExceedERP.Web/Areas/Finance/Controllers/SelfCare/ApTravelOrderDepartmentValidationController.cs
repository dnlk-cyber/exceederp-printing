using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Finance.AP.PaymentRequest;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP
{
    public class ApTravelOrderDepartmentValidationController : Controller
    {

        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();


        [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
        public ActionResult ApTravelOrderDepartmentValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApTravelOrderDepartmentValidation> ApTravelOrderDepartmentValidations = _db.ApTravelOrderDepartmentValidations.Where(x=>x.ApTravelOrderId==id);
            DataSourceResult result = ApTravelOrderDepartmentValidations.ToDataSourceResult(request, ApTravelOrderDepartmentValidation => new {
                ApTravelOrderDepartmentValidationId = ApTravelOrderDepartmentValidation.ApTravelOrderDepartmentValidationId,
                ApTravelOrderId=id,
                Status = ApTravelOrderDepartmentValidation.Status,
                Remark = ApTravelOrderDepartmentValidation.Remark,
                OrgBranchName = ApTravelOrderDepartmentValidation.OrgBranchName,
                DateCreated = ApTravelOrderDepartmentValidation.DateCreated,
                LastModified = ApTravelOrderDepartmentValidation.LastModified,
                CreatedBy = ApTravelOrderDepartmentValidation.CreatedBy,
                ModifiedBy = ApTravelOrderDepartmentValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrderDepartmentValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, ApTravelOrderDepartmentValidation ApTravelOrderDepartmentValidation)
        {
            var record = _db.ApTravelOrders.Find(id);
            if (record != null && record.IsDepartmentOnlineApproved)
                ModelState.AddModelError("approved","cannot manipulate this record,it is already approved");

            var lines = _db.ApTravelOrderDepartures.Where(x => x.ApTravelOrderId == id);
            if (!lines.Any())
                ModelState.AddModelError("lines", "Please add line items");

            if (record != null && record.IsOnlineApproved)
                ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");

            if (record != null && record.ForwardedTo != User.Identity.Name)
                ModelState.AddModelError("notcorrectuser", "You can't approve this record,it is forwarded to another user.");

            var hasApproved = _db.ApTravelOrderDepartmentValidations
                    .Where(x => x.ApTravelOrderId == id)
                .Where(x => x.CreatedBy == User.Identity.Name && x.Status == ApprovalStatuses.Approve).AsQueryable();
            if (hasApproved.Any())
                ModelState.AddModelError("hasapproved", "You can't approve again.You have already approved.");

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (ApTravelOrderDepartmentValidation.Status == ApprovalStatuses.Approve)
                    {
                        if (record.IsFinalApproval)
                        {
                            record.Status = "Approved";
                            record.IsDepartmentOnlineApproved = true;
                            record.DepartmentOnlineApprovedBy = User.Identity.Name;
                            record.DepartmentOnlineApprovedTime = DateTime.Now;
                            _db.Entry(record).State = EntityState.Modified;
                            _db.SaveChanges();
                            #region approval notification

                            NotifModels notif = new NotifModels
                            {
                                UserId = record.ForwardedFrom,
                                URL = DocApprovalWorkflowHelper.GetDocUrl(ApprovalDocumentType.ApPaymentRequestSelfCare, record.ApTravelOrderId),
                                Message =
                                    $"Payment request document {record.ApTravelOrderId} is approved by User: {record.ForwardedTo} at {DateTime.Now}"
                            };

                            Notify.NotifyUser(notif);

                            #endregion
                        }
                        else
                        {
                            record.Status = "Partially Approved";
                            record.IsDepartmentOnlineApproved = false;
                            record.IsVoid = false;
                            record.OnlineApprovedBy = User.Identity.Name;
                            record.OnlineApprovedTime = DateTime.Now;
                            _db.Entry(record).State = EntityState.Modified;
                            _db.SaveChanges();

                            #region approval notification

                            NotifModels notif = new NotifModels
                            {
                                UserId = record.ForwardedFrom,
                                URL = DocApprovalWorkflowHelper.GetDocUrl(ApprovalDocumentType.ApTravelOrderSelfCare, record.ApTravelOrderId),
                                Message =
                                    $"Payment request document {record.ApTravelOrderId} is approved by User: {record.ForwardedTo} at {DateTime.Now} and forwarded to next user."
                            };

                            Notify.NotifyUser(notif);

                            #endregion

                            #region forward to next approver

                            decimal docamount = record.AdvanceAmount;
                            DocApprovalWorkflowHelper.ForwardToApprover(record.ApTravelOrderId, docamount, "",
                                record.ForwardedTo, ApprovalDocumentType.ApTravelOrderSelfCare, false, (int)record.CurrentApprovalStep);

                            #endregion
                        }
                    }

                    if (ApTravelOrderDepartmentValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsDepartmentSendForApproval = false;
                        record.Status = "Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    var entity = new ApTravelOrderDepartmentValidation
                    {
                        ApTravelOrderId = id,
                        Status = ApTravelOrderDepartmentValidation.Status,
                        Remark = ApTravelOrderDepartmentValidation.Remark,
                        OrgBranchName = ApTravelOrderDepartmentValidation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = ApTravelOrderDepartmentValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = ApTravelOrderDepartmentValidation.ModifiedBy
                    };

                    _db.ApTravelOrderDepartmentValidations.Add(entity);
                    _db.SaveChanges();
                    ApTravelOrderDepartmentValidation.ApTravelOrderDepartmentValidationId = entity.ApTravelOrderDepartmentValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Self care payment request department approval", "Success", User.Identity.Name, $"approved request {entity.ApTravelOrderId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { ApTravelOrderDepartmentValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrderDepartmentValidations_Update(int id,[DataSourceRequest]DataSourceRequest request, ApTravelOrderDepartmentValidation ApTravelOrderDepartmentValidation)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region ger parent record status
                ApPaybleInvoices record = _db.APPaybleInvoices.Find(id);

                if (record != null && record.IsOnlineApproved)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record,it is already approved";
                    //return View("Error");
                }
                else if (record != null && !record.IsOnlineApproved)
                {
                    //validate 
                    if (ApTravelOrderDepartmentValidation.Status ==ApprovalStatuses.Approve)
                    {
                       
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                    }
                    if (ApTravelOrderDepartmentValidation.Status ==ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }
                }
                #endregion

                if (isvalid)
                {
                    var entity = new ApTravelOrderDepartmentValidation
                    {
                        ApTravelOrderId = id,
                        ApTravelOrderDepartmentValidationId = ApTravelOrderDepartmentValidation.ApTravelOrderDepartmentValidationId,
                        Status = ApTravelOrderDepartmentValidation.Status,
                        Remark = ApTravelOrderDepartmentValidation.Remark,
                        OrgBranchName = ApTravelOrderDepartmentValidation.OrgBranchName,
                        DateCreated = ApTravelOrderDepartmentValidation.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = ApTravelOrderDepartmentValidation.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.ApTravelOrderDepartmentValidations.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }

            return Json(new[] { ApTravelOrderDepartmentValidation }.ToDataSourceResult(request, ModelState));
        }

        

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
