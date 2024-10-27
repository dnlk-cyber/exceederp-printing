using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Finance.AP.FundTransfer;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.SelfCare.FundTransfers
{
    public class FundTransferRequestDepartmentValidationController : Controller
	{

		private readonly ExceedDbContext _db = new ExceedDbContext();
		private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();


		[AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
		public ActionResult Index()
		{
			return View();
		}

		[AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
		public ActionResult FundTransferDepartmentValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
		{
			IQueryable<FundTransferDepartmentValidation>FundTransferDepartmentValidations = _db.FundTransferDepartmentValidations.Where(x=>x.FundTransferId==id);
			DataSourceResult result =FundTransferDepartmentValidations.ToDataSourceResult(request,FundTransferDepartmentValidation => new {
				FundTransferDepartmentValidationId =FundTransferDepartmentValidation.FundTransferDepartmentValidationId,
				FundTransferId=id,
				Status =FundTransferDepartmentValidation.Status,
				Remark =FundTransferDepartmentValidation.Remark,
				OrgBranchName =FundTransferDepartmentValidation.OrgBranchName,
				DateCreated =FundTransferDepartmentValidation.DateCreated,
				LastModified =FundTransferDepartmentValidation.LastModified,
				CreatedBy =FundTransferDepartmentValidation.CreatedBy,
				ModifiedBy =FundTransferDepartmentValidation.ModifiedBy
			});

			return Json(result);
		}

		[AuthorizeRoles(HrmConstants.HRDepartmentHead)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult FundTransferDepartmentValidations_Create(int id,[DataSourceRequest]DataSourceRequest request,FundTransferDepartmentValidation fundTransferDepartmentValidation)
		{
			var record = _db.FundTransfers.Find(id);
			if (record != null && record.IsDepartmentOnlineApproved)
				ModelState.AddModelError("approved","cannot manipulate this record,it is already approved");
		
			if (record != null && record.ForwardedTo != User.Identity.Name)
				ModelState.AddModelError("notcorrectuser", "You can't approve this record,it is forwarded to another user.");

			var hasApproved = _db.FundTransferDepartmentValidations
					.Where(x => x.FundTransferId == id)
				.Where(x => x.CreatedBy == User.Identity.Name && x.Status == ApprovalStatuses.Approve).AsQueryable();
			if (hasApproved.Any())
				ModelState.AddModelError("hasapproved", "You can't approve again.You have already approved.");

			if (fundTransferDepartmentValidation.Status == ApprovalStatuses.Approve)
			{
				var lines = _db.FundTransferDetails.Where(x => x.FundTransferId == id);
				if (!lines.Any())
				{
					ModelState.AddModelError("", "Please add fund transfer request line items.");
				}

				foreach (var line in lines)
				{
					if (line.Requested <= 0)
					{
						ModelState.AddModelError("requetes", "Please fill requested amount");
					}

					if (line.Approved <= 0)
					{
						ModelState.AddModelError("above", "Approved should not be more than requested");
					}
				}
			}


			
			if (ModelState.IsValid)
			{
				if (record != null)
				{
					if (fundTransferDepartmentValidation.Status == ApprovalStatuses.Approve)
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
								URL = DocApprovalWorkflowHelper.GetDocUrl(ApprovalDocumentType.ApFundTransferSelfCare, record.FundTransferId),
								Message =
									$"Payment request document {record.FundTransferId} is approved by User: {record.ForwardedTo} at {DateTime.Now}"
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
								URL = DocApprovalWorkflowHelper.GetDocUrl(ApprovalDocumentType.ApFundTransferSelfCare, record.FundTransferId),
								Message =
									$"Payment request document {record.FundTransferId} is approved by User: {record.ForwardedTo} at {DateTime.Now} and forwarded to next user."
							};

							Notify.NotifyUser(notif);

							#endregion

							#region forward to next approver

							decimal docamount = record.FundTransferDetails.Where(x => x.FundTransferId == record.FundTransferId).Sum(x => x.Approved);
							int step = record.CurrentApprovalStep != null ? (int)record.CurrentApprovalStep : 0;
							DocApprovalWorkflowHelper.ForwardToApprover(record.FundTransferId, docamount, "",
								record.ForwardedTo, ApprovalDocumentType.ApFundTransferSelfCare, false,step);

							#endregion
						}
					}

					if (fundTransferDepartmentValidation.Status == ApprovalStatuses.Reject)
					{
						record.IsDepartmentSendForApproval = false;
						record.Status = "Rejected";
						_db.Entry(record).State = EntityState.Modified;
						_db.SaveChanges();
					}

					var entity = new FundTransferDepartmentValidation
					{
						FundTransferId = id,
						Status =fundTransferDepartmentValidation.Status,
						Remark =fundTransferDepartmentValidation.Remark,
						OrgBranchName =fundTransferDepartmentValidation.OrgBranchName,
						DateCreated = DateTime.Now,
						LastModified =fundTransferDepartmentValidation.LastModified,
						CreatedBy = User.Identity.Name,
						ModifiedBy =fundTransferDepartmentValidation.ModifiedBy
					};

					_db.FundTransferDepartmentValidations.Add(entity);
					_db.SaveChanges();
					fundTransferDepartmentValidation.FundTransferDepartmentValidationId = entity.FundTransferDepartmentValidationId;
					#region operation log
					UserHelper.OperationLog(Request.UserHostAddress, "Self care fund transfer department approval", "Success", User.Identity.Name, $"approved request {entity.FundTransferId} ", Modules.Finance);
					#endregion
				}
			}

			return Json(new[] {fundTransferDepartmentValidation }.ToDataSourceResult(request, ModelState));
		}

		[AuthorizeRoles(HrmConstants.HRDepartmentHead)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult FundTransferDepartmentValidations_Update(int id,[DataSourceRequest]DataSourceRequest request,FundTransferDepartmentValidation fundTransferDepartmentValidation)
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
					if (fundTransferDepartmentValidation.Status ==ApprovalStatuses.Approve)
					{
					   
						record.Status = "Approved";
						record.IsOnlineApproved = true;
						record.OnlineApprovedBy = User.Identity.Name;
						record.OnlineApprovedTime = DateTime.Now;
					}
					if (fundTransferDepartmentValidation.Status ==ApprovalStatuses.Reject)
					{
						record.IsSendForApproval = false;
						record.Status = "Rejected";
					}
				}
				#endregion

				if (isvalid)
				{
					var entity = new FundTransferDepartmentValidation
					{
						FundTransferId = id,
						FundTransferDepartmentValidationId =fundTransferDepartmentValidation.FundTransferDepartmentValidationId,
						Status =fundTransferDepartmentValidation.Status,
						Remark =fundTransferDepartmentValidation.Remark,
						OrgBranchName =fundTransferDepartmentValidation.OrgBranchName,
						DateCreated =fundTransferDepartmentValidation.DateCreated,
						LastModified = DateTime.Now,
						CreatedBy =fundTransferDepartmentValidation.CreatedBy,
						ModifiedBy = User.Identity.Name
					};

					_db.FundTransferDepartmentValidations.Attach(entity);
					_db.Entry(entity).State = EntityState.Modified;
					_db.SaveChanges();
				}
			}

			return Json(new[] {fundTransferDepartmentValidation }.ToDataSourceResult(request, ModelState));
		}

		

		protected override void Dispose(bool disposing)
		{
			_db.Dispose();
			base.Dispose(disposing);
		}
	}
}
