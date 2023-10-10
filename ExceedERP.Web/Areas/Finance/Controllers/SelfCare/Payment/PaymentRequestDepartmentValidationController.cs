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
	public class PaymentRequestDepartmentValidationController : Controller
	{

		private readonly ExceedDbContext _db = new ExceedDbContext();
		private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();


		[AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
		public ActionResult Index()
		{
			return View();
		}

		[AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
		public ActionResult PaymentRequestDepartmentValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
		{
			IQueryable<PaymentRequestDepartmentValidation> PaymentRequestDepartmentValidations = _db.PaymentRequestDepartmentValidations.Where(x=>x.PaymentRequestId==id);
			DataSourceResult result = PaymentRequestDepartmentValidations.ToDataSourceResult(request, PaymentRequestDepartmentValidation => new {
				PaymentRequestDepartmentValidationId = PaymentRequestDepartmentValidation.PaymentRequestDepartmentValidationId,
				PaymentRequestId=id,
				Status = PaymentRequestDepartmentValidation.Status,
				Remark = PaymentRequestDepartmentValidation.Remark,
				OrgBranchName = PaymentRequestDepartmentValidation.OrgBranchName,
				DateCreated = PaymentRequestDepartmentValidation.DateCreated,
				LastModified = PaymentRequestDepartmentValidation.LastModified,
				CreatedBy = PaymentRequestDepartmentValidation.CreatedBy,
				ModifiedBy = PaymentRequestDepartmentValidation.ModifiedBy
			});

			return Json(result);
		}

		[AuthorizeRoles(HrmConstants.HRDepartmentHead)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult PaymentRequestDepartmentValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, PaymentRequestDepartmentValidation PaymentRequestDepartmentValidation)
		{
			var record = _db.PaymentRequests.Find(id);
			if (record != null && record.IsDepartmentOnlineApproved)
				ModelState.AddModelError("approved","cannot manipulate this record,it is already approved");

			//var lines = _db.PaymentRequestLines.Where(x => x.PaymentRequestId == id);
			//if (!lines.Any())
			//    ModelState.AddModelError("lines", "Please add line items");

			var lines = _db.PaymentRequestLines.Where(x => x.PaymentRequestId == id);
			if (!lines.Any())
			{
				ModelState.AddModelError("", "Please add payment request line items.");
			}
			

			if (record != null && record.IsDepartmentOnlineApproved)
				ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");

			if (record != null && record.ForwardedTo != User.Identity.Name)
				ModelState.AddModelError("notcorrectuser", "You can't approve this record,it is forwarded to another user.");

			var hasApproved = _db.PaymentRequestDepartmentValidations
					.Where(x => x.PaymentRequestId == id)
				.Where(x => x.CreatedBy == User.Identity.Name && x.Status == ApprovalStatuses.Approve).AsQueryable();
			if (hasApproved.Any())
				ModelState.AddModelError("hasapproved", "You can't approve again.You have already approved.");

			if (ModelState.IsValid)
			{
				if (record != null)
				{
					if (PaymentRequestDepartmentValidation.Status == ApprovalStatuses.Approve)
					{
						if (record.IsFinalApproval)
						{
							record.Status = "Department Approved";
							record.IsDepartmentOnlineApproved = true;
							record.DepartmentOnlineApprovedBy = User.Identity.Name;
							record.DepartmentOnlineApprovedTime = DateTime.Now;
							_db.Entry(record).State = EntityState.Modified;
							_db.SaveChanges();
							#region approval notification

							NotifModels notif = new NotifModels
							{
								UserId = record.ForwardedFrom,
								URL = DocApprovalWorkflowHelper.GetDocUrl(ApprovalDocumentType.ApPaymentRequestSelfCare, record.PaymentRequestId),
								Message =
									$"Payment request document {record.PaymentRequestId} is approved by User: {record.ForwardedTo} at {DateTime.Now}"
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
							_db.Entry(record).State = EntityState.Modified;
							_db.SaveChanges();

							#region approval notification

							NotifModels notif = new NotifModels
							{
								UserId = record.ForwardedFrom,
								URL = DocApprovalWorkflowHelper.GetDocUrl(ApprovalDocumentType.ApPaymentRequestSelfCare, record.PaymentRequestId),
								Message =
									$"Payment request document {record.PaymentRequestId} is approved by User: {record.ForwardedTo} at {DateTime.Now} and forwarded to next user."
							};

							Notify.NotifyUser(notif);

							#endregion

							#region forward to next approver

							decimal docamount = record.PaymentRequestLines.Where(x => x.PaymentRequestId == record.PaymentRequestId).Sum(x => x.TotalAmount);
							int step = record.CurrentApprovalStep != null ? (int)record.CurrentApprovalStep : 0;
							DocApprovalWorkflowHelper.ForwardToApprover(record.PaymentRequestId, docamount, "",
								record.ForwardedTo, ApprovalDocumentType.ApPaymentRequestSelfCare, false,step);

							#endregion
						}
					}

					if (PaymentRequestDepartmentValidation.Status == ApprovalStatuses.Reject)
					{
						record.IsDepartmentSendForApproval = false;
						record.Status = "Rejected";
						_db.Entry(record).State = EntityState.Modified;
						_db.SaveChanges();
					}

					var entity = new PaymentRequestDepartmentValidation
					{
						PaymentRequestId = id,
						Status = PaymentRequestDepartmentValidation.Status,
						Remark = PaymentRequestDepartmentValidation.Remark,
						OrgBranchName = PaymentRequestDepartmentValidation.OrgBranchName,
						DateCreated = DateTime.Now,
						LastModified = PaymentRequestDepartmentValidation.LastModified,
						CreatedBy = User.Identity.Name,
						ModifiedBy = PaymentRequestDepartmentValidation.ModifiedBy
					};

					_db.PaymentRequestDepartmentValidations.Add(entity);
					_db.SaveChanges();
					PaymentRequestDepartmentValidation.PaymentRequestDepartmentValidationId = entity.PaymentRequestDepartmentValidationId;
					#region operation log
					UserHelper.OperationLog(Request.UserHostAddress, "Self care payment request department approval", "Success", User.Identity.Name, $"approved request {entity.PaymentRequestId} ", Modules.Finance);
					#endregion
				}
			}

			return Json(new[] { PaymentRequestDepartmentValidation }.ToDataSourceResult(request, ModelState));
		}

		[AuthorizeRoles(HrmConstants.HRDepartmentHead)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult PaymentRequestDepartmentValidations_Update(int id,[DataSourceRequest]DataSourceRequest request, PaymentRequestDepartmentValidation PaymentRequestDepartmentValidation)
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
					if (PaymentRequestDepartmentValidation.Status ==ApprovalStatuses.Approve)
					{
					   
						record.Status = "Approved";
						record.IsOnlineApproved = true;
						record.OnlineApprovedBy = User.Identity.Name;
						record.OnlineApprovedTime = DateTime.Now;
					}
					if (PaymentRequestDepartmentValidation.Status ==ApprovalStatuses.Reject)
					{
						record.IsSendForApproval = false;
						record.Status = "Rejected";
					}
				}
				#endregion

				if (isvalid)
				{
					var entity = new PaymentRequestDepartmentValidation
					{
						PaymentRequestId = id,
						PaymentRequestDepartmentValidationId = PaymentRequestDepartmentValidation.PaymentRequestDepartmentValidationId,
						Status = PaymentRequestDepartmentValidation.Status,
						Remark = PaymentRequestDepartmentValidation.Remark,
						OrgBranchName = PaymentRequestDepartmentValidation.OrgBranchName,
						DateCreated = PaymentRequestDepartmentValidation.DateCreated,
						LastModified = DateTime.Now,
						CreatedBy = PaymentRequestDepartmentValidation.CreatedBy,
						ModifiedBy = User.Identity.Name
					};

					_db.PaymentRequestDepartmentValidations.Attach(entity);
					_db.Entry(entity).State = EntityState.Modified;
					_db.SaveChanges();
				}
			}

			return Json(new[] { PaymentRequestDepartmentValidation }.ToDataSourceResult(request, ModelState));
		}

		

		protected override void Dispose(bool disposing)
		{
			_db.Dispose();
			base.Dispose(disposing);
		}
	}
}
