using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP.Travel
{
	public class ApTravelOrderController : Controller
	{
		private ExceedDbContext _db = new ExceedDbContext();

		[AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableTravelOrderApprover, FinanceConstants.AccountPayableTravelOrder)]
		public ActionResult Index()
		{
			PopulateViewData();			
			return View();
		}

		private void PopulateViewData()
		{
			var branches = _db.Branch;
			var structure = _db.MainCompanyStructures;
			var employee = _db.Person_Employee;
			var selectedEmployee = employee.Select(
				i => new
				{
					Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
					Code = i.EmployeeId
				}).AsQueryable();
			ViewData["personEmployees"] = selectedEmployee;

			ViewData["Structure"] = structure;

			ViewData["Branches"] = branches;

			var items = _db.ApSetupItems.ToList();
			ViewData["Items"] = items;
		}
		[AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableTravelOrderApprover, FinanceConstants.AccountPayableTravelOrder)]
		public ActionResult ApTravelOrders_Read([DataSourceRequest]DataSourceRequest request)
		{
			List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

			IQueryable<ApTravelOrder> aptravelorders = _db.ApTravelOrders
				.Where(x => mybraches.Contains(x.BranchId))
				.Where(x => x.IsDepartmentOnlineApproved);

			if (FinanceConfigurationHelper.IsPRBudgetControlEnabled())
			{
				aptravelorders = aptravelorders.Where(x => x.IsBudgetApproved);
			}
			if (GlobalHelper.IsMultipleBusinessUnit())
			{
				List<int> myBusinessUnits = UserInformationHelper.GetUserBusinessUnits(User.Identity.Name);
				aptravelorders = aptravelorders.Where(x => myBusinessUnits.Contains(x.BusinessUnitId));
			}

			DataSourceResult result = aptravelorders.ToDataSourceResult(request, apTravelOrder => new {
				ApTravelOrderId = apTravelOrder.ApTravelOrderId,               
				BranchId = apTravelOrder.BranchId,
				BusinessUnitId = apTravelOrder.BusinessUnitId,
				CreatedAtBranchId = apTravelOrder.CreatedAtBranchId,
				CreatedAtBusinessUnitId = apTravelOrder.CreatedAtBusinessUnitId,
				OrgStructureId = apTravelOrder.OrgStructureId,
				PurposeOfTravel = apTravelOrder.PurposeOfTravel,
				RequestDate = apTravelOrder.RequestDate,
				AdvanceAmount = apTravelOrder.AdvanceAmount,
				TotalDuration = apTravelOrder.TotalDuration,

				TransportReference = apTravelOrder.TransportReference,
				Reference = apTravelOrder.Reference,
				EmployeeId = apTravelOrder.EmployeeId,
				IsForOther = apTravelOrder.IsForOther,

				ForwardedFrom = apTravelOrder.ForwardedFrom,
				ForwardedTo = apTravelOrder.ForwardedTo,
				IsFinalApproval = apTravelOrder.IsFinalApproval,
				CurrentApprovalStep = apTravelOrder.CurrentApprovalStep,

				IsBudgetApproved = apTravelOrder.IsBudgetApproved,
				BudgetApprovedBy = apTravelOrder.BudgetApprovedBy,
				BudgetApprovedTime = apTravelOrder.BudgetApprovedTime,

				IsDepartmentOnlineApproved = apTravelOrder.IsDepartmentOnlineApproved,
				DepartmentOnlineApprovedBy = apTravelOrder.DepartmentOnlineApprovedBy,
				DepartmentOnlineApprovedTime = apTravelOrder.DepartmentOnlineApprovedTime,
				IsDepartmentSendForApproval = apTravelOrder.IsDepartmentSendForApproval,
				DepartmentSendForApprovalBy = apTravelOrder.DepartmentSendForApprovalBy,
				DepartmentSendForApprovalTime = apTravelOrder.DepartmentSendForApprovalTime,

				
				IsOnlineTransferred = apTravelOrder.IsOnlineTransferred,
				IsOnlineTransferredBy = apTravelOrder.IsOnlineTransferredBy,
				OnlineTransferredTime = apTravelOrder.OnlineTransferredTime,
				Status = apTravelOrder.Status,
				IsOnlineApproved = apTravelOrder.IsOnlineApproved,
				OnlineApprovedBy = apTravelOrder.OnlineApprovedBy,
				OnlineApprovedTime = apTravelOrder.OnlineApprovedTime,
				IsVoid = apTravelOrder.IsVoid,
				VoidBy = apTravelOrder.VoidBy,
				VoidTime = apTravelOrder.VoidTime,
				IsSendForApproval = apTravelOrder.IsSendForApproval,
				SendForApprovalBy = apTravelOrder.SendForApprovalBy,
				SendForApprovalTime = apTravelOrder.SendForApprovalTime,
				OrgBranchName = apTravelOrder.OrgBranchName,
				DateCreated = apTravelOrder.DateCreated,
				LastModified = apTravelOrder.LastModified,
				CreatedBy = apTravelOrder.CreatedBy,
				ModifiedBy = apTravelOrder.ModifiedBy
			});

			return Json(result);
		}

		[AuthorizeRoles(FinanceConstants.AccountPayableTravelOrder)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult ApTravelOrders_Create([DataSourceRequest]DataSourceRequest request, ApTravelOrder apTravelOrder)
		{
			#region owner approval

			bool canOwnerApprove = DocApprovalHelper.CanOwnerApprove(ApprovalDocumentType.ApTravelOrder);
			if (canOwnerApprove)
				apTravelOrder.ForwardedTo = User.Identity.Name;

			#endregion

			if (ModelState.IsValid)
			{
				var entity = new ApTravelOrder
				{
					
					BranchId = apTravelOrder.BranchId,
					BusinessUnitId = apTravelOrder.BusinessUnitId,
					CreatedAtBranchId = apTravelOrder.CreatedAtBranchId,
					CreatedAtBusinessUnitId = apTravelOrder.CreatedAtBusinessUnitId,
					OrgStructureId = apTravelOrder.OrgStructureId,
					PurposeOfTravel = apTravelOrder.PurposeOfTravel,
					RequestDate = apTravelOrder.RequestDate.Date,
					AdvanceAmount = apTravelOrder.AdvanceAmount,
					TotalDuration = apTravelOrder.TotalDuration,

					TransportReference = apTravelOrder.TransportReference,
					Reference = apTravelOrder.Reference,
					IsOnlineTransferred = apTravelOrder.IsOnlineTransferred,
					IsOnlineTransferredBy = apTravelOrder.IsOnlineTransferredBy,
					OnlineTransferredTime = apTravelOrder.OnlineTransferredTime,
					EmployeeId = apTravelOrder.EmployeeId,
					IsForOther = apTravelOrder.IsForOther,
					

					Status = apTravelOrder.Status,

					ForwardedFrom = User.Identity.Name,
					ForwardedTo = User.Identity.Name,
					IsFinalApproval = apTravelOrder.IsFinalApproval,
					CurrentApprovalStep = apTravelOrder.CurrentApprovalStep,

					IsBudgetApproved = apTravelOrder.IsBudgetApproved,
					BudgetApprovedBy = apTravelOrder.BudgetApprovedBy,
					BudgetApprovedTime = apTravelOrder.BudgetApprovedTime,

					IsOnlineApproved = apTravelOrder.IsOnlineApproved,
					OnlineApprovedBy = apTravelOrder.OnlineApprovedBy,
					OnlineApprovedTime = apTravelOrder.OnlineApprovedTime,

					IsVoid = apTravelOrder.IsVoid,
					VoidBy = apTravelOrder.VoidBy,
					VoidTime = apTravelOrder.VoidTime,
					IsSendForApproval = apTravelOrder.IsSendForApproval,
					SendForApprovalBy = apTravelOrder.SendForApprovalBy,
					SendForApprovalTime = apTravelOrder.SendForApprovalTime,
					OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
					DateCreated = apTravelOrder.DateCreated,
					LastModified = apTravelOrder.LastModified,
					CreatedBy = apTravelOrder.CreatedBy,
					ModifiedBy = apTravelOrder.ModifiedBy
				};

				_db.ApTravelOrders.Add(entity);
				_db.SaveChanges();
				apTravelOrder.ApTravelOrderId = entity.ApTravelOrderId;

				#region operation log
				UserHelper.OperationLog(Request.UserHostAddress, "AP travel order", "Success", User.Identity.Name, $"created expense claim {entity.ApTravelOrderId} ", Modules.Finance);
				#endregion
			}

			return Json(new[] { apTravelOrder }.ToDataSourceResult(request, ModelState));
		}

		[AuthorizeRoles(FinanceConstants.AccountPayableTravelOrder)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult ApTravelOrders_Update([DataSourceRequest]DataSourceRequest request, ApTravelOrder apTravelOrder)
		{
			if (ModelState.IsValid)
			{
				var entity = new ApTravelOrder
				{
					ApTravelOrderId = apTravelOrder.ApTravelOrderId,				   
					BranchId = apTravelOrder.BranchId,
					BusinessUnitId = apTravelOrder.BusinessUnitId,
					CreatedAtBranchId = apTravelOrder.CreatedAtBranchId,
					CreatedAtBusinessUnitId = apTravelOrder.CreatedAtBusinessUnitId,
					OrgStructureId = apTravelOrder.OrgStructureId,
					PurposeOfTravel = apTravelOrder.PurposeOfTravel,
					RequestDate = apTravelOrder.RequestDate,
					AdvanceAmount = apTravelOrder.AdvanceAmount,
					TotalDuration = apTravelOrder.TotalDuration,

					TransportReference = apTravelOrder.TransportReference,
					Reference = apTravelOrder.Reference,
					EmployeeId = apTravelOrder.EmployeeId,
					IsForOther = apTravelOrder.IsForOther,

					ForwardedFrom = apTravelOrder.ForwardedFrom,
					ForwardedTo = apTravelOrder.ForwardedTo,
					IsFinalApproval = apTravelOrder.IsFinalApproval,
					CurrentApprovalStep = apTravelOrder.CurrentApprovalStep,

					IsBudgetApproved = apTravelOrder.IsBudgetApproved,
					BudgetApprovedBy = apTravelOrder.BudgetApprovedBy,
					BudgetApprovedTime = apTravelOrder.BudgetApprovedTime,

					IsOnlineTransferred = apTravelOrder.IsOnlineTransferred,
					IsOnlineTransferredBy = apTravelOrder.IsOnlineTransferredBy,
					OnlineTransferredTime = apTravelOrder.OnlineTransferredTime,
					Status = apTravelOrder.Status,
					IsOnlineApproved = apTravelOrder.IsOnlineApproved,
					OnlineApprovedBy = apTravelOrder.OnlineApprovedBy,
					OnlineApprovedTime = apTravelOrder.OnlineApprovedTime,
					IsVoid = apTravelOrder.IsVoid,
					VoidBy = apTravelOrder.VoidBy,
					VoidTime = apTravelOrder.VoidTime,
					IsSendForApproval = apTravelOrder.IsSendForApproval,
					SendForApprovalBy = apTravelOrder.SendForApprovalBy,
					SendForApprovalTime = apTravelOrder.SendForApprovalTime,
					OrgBranchName = apTravelOrder.OrgBranchName,
					DateCreated = apTravelOrder.DateCreated,
					LastModified = apTravelOrder.LastModified,
					CreatedBy = apTravelOrder.CreatedBy,
					ModifiedBy = apTravelOrder.ModifiedBy
				};

				_db.ApTravelOrders.Attach(entity);
				_db.Entry(entity).State = EntityState.Modified;
				_db.SaveChanges();

				#region operation log
				UserHelper.OperationLog(Request.UserHostAddress, "AP travel order", "Success", User.Identity.Name, $"updated expense claim {entity.ApTravelOrderId} ", Modules.Finance);
				#endregion
			}

			return Json(new[] { apTravelOrder }.ToDataSourceResult(request, ModelState));
		}

		[AuthorizeRoles(FinanceConstants.AccountPayableTravelOrder)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult ApTravelOrders_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelOrder apTravelOrder)
		{
			if (ModelState.IsValid)
			{
				var entity = new ApTravelOrder
				{
					ApTravelOrderId = apTravelOrder.ApTravelOrderId,				   
					BranchId = apTravelOrder.BranchId,
					BusinessUnitId = apTravelOrder.BusinessUnitId,
					CreatedAtBranchId = apTravelOrder.CreatedAtBranchId,
					CreatedAtBusinessUnitId = apTravelOrder.CreatedAtBusinessUnitId,
					OrgStructureId = apTravelOrder.OrgStructureId,
					PurposeOfTravel = apTravelOrder.PurposeOfTravel,
					RequestDate = apTravelOrder.RequestDate,
					AdvanceAmount = apTravelOrder.AdvanceAmount,
					TotalDuration = apTravelOrder.TotalDuration,


					Reference = apTravelOrder.Reference,
					IsOnlineTransferred = apTravelOrder.IsOnlineTransferred,
					IsOnlineTransferredBy = apTravelOrder.IsOnlineTransferredBy,
					OnlineTransferredTime = apTravelOrder.OnlineTransferredTime,
					Status = apTravelOrder.Status,
					IsOnlineApproved = apTravelOrder.IsOnlineApproved,
					OnlineApprovedBy = apTravelOrder.OnlineApprovedBy,
					OnlineApprovedTime = apTravelOrder.OnlineApprovedTime,
					IsVoid = apTravelOrder.IsVoid,
					VoidBy = apTravelOrder.VoidBy,
					VoidTime = apTravelOrder.VoidTime,
					IsSendForApproval = apTravelOrder.IsSendForApproval,
					SendForApprovalBy = apTravelOrder.SendForApprovalBy,
					SendForApprovalTime = apTravelOrder.SendForApprovalTime,
					OrgBranchName = apTravelOrder.OrgBranchName,
					DateCreated = apTravelOrder.DateCreated,
					LastModified = apTravelOrder.LastModified,
					CreatedBy = apTravelOrder.CreatedBy,
					ModifiedBy = apTravelOrder.ModifiedBy
				};

				_db.ApTravelOrders.Attach(entity);
				_db.ApTravelOrders.Remove(entity);
				_db.SaveChanges();

				#region delete log
				UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted travel order" + apTravelOrder.ApTravelOrderId + " " + apTravelOrder.Reference, Modules.Finance);
				#endregion
			}

			return Json(new[] { apTravelOrder }.ToDataSourceResult(request, ModelState));
		}

		[AuthorizeRoles(FinanceConstants.AccountPayableTravelOrder)]
		public JsonResult SendForApproval(int id)
		{
			var record = _db.ApTravelOrders.Find(id);
			if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.ApTravelOrderDepartures.Count > 0)
			{
				record.Status = Enum.GetName(typeof(OperationStatuses), 4);
				record.IsSendForApproval = true;
				record.SendForApprovalBy = User.Identity.Name;
				record.SendForApprovalTime = DateTime.Now;
				_db.ApTravelOrders.Attach(record);
				_db.Entry(record).State = EntityState.Modified;
				_db.SaveChanges();
				#region approval workflow
				//bool isNewRecord = true;
				//var validations = record.ApTravelOrderValidations.Where(x => x.Status == ApprovalStatuses.Approve).AsQueryable();
				//if (validations.Any())
				//    isNewRecord = false;
				//decimal docamount = record.AdvanceAmount;
				//int docid = record.ApTravelOrderId;
				//DocApprovalWorkflowHelper.ForwardToApprover(docid, docamount, "", User.Identity.Name, ApprovalDocumentType.ApTravelOrder, isNewRecord);
				#endregion

				var formattedData = new
				{
					Message = "Successfully Sent.",
					Status = "OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			if (record == null)
			{
				var formattedData = new
				{
					Message = "The record is not found.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			if (record.ApTravelOrderDepartures.Count == 0)
			{
				var formattedData = new
				{
					Message = "Please add  departure line items first.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			if (record.IsSendForApproval)
			{
				var formattedData = new
				{
					Message = "The record is already sent for approval you cannot send again.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			if (record.IsOnlineApproved)
			{
				var formattedData = new
				{
					Message = "The record is already approved.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}
			return null;
		}

		
		[AuthorizeRoles(FinanceConstants.AccountPayableVoid)]
		public JsonResult Void(int id)
		{
			var record = _db.ApTravelOrders.Find(id);
			if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.ApTravelOrderDepartures.Count > 0)
			{
				#region validate claim

				var payments = _db.ApTravelOrders.Where(x => x.ApTravelOrderId == record.ApTravelOrderId)
					.ToList();
				if (payments.Any())
				{
					var formattedData1 = new
					{
						Message = $"The travel order number {record.ApTravelOrderId} is already applied in Travel Order \n hence cannot be void.",
						Status = "NOT_OK"
					};
					return Json(formattedData1, JsonRequestBehavior.AllowGet);
				}
				#endregion
				record.Status = Enum.GetName(typeof(OperationStatuses), 10);
				record.IsOnlineApproved = false;
				record.IsSendForApproval = false;
				record.VoidBy = User.Identity.Name;
				record.VoidTime = DateTime.Now;
				record.IsVoid = false;
				record.VoidBy = User.Identity.Name;
				record.VoidTime = DateTime.Now;
				_db.ApTravelOrders.Attach(record);
				_db.Entry(record).State = EntityState.Modified;
				_db.SaveChanges();


				//copy
				//CopyRecord(id);

				#region operation log
				UserHelper.OperationLog(Request.UserHostAddress, "travel order", "Success", User.Identity.Name, $"void travel order {id} ", Modules.Finance);
				#endregion

				var formattedData = new
				{
					Message = "Void Successful.",
					Status = "OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}
			if (record == null)
			{
				var formattedData = new
				{
					Message = "The record is not found.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			if (record.ApTravelOrderDepartures.Count == 0)
			{
				var formattedData = new
				{
					Message = "Please add departure lines and try again.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}
			if (record.IsVoid)
			{
				var formattedData = new
				{
					Message = "The record is already void.You can not void again.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}
			if (record.IsOnlineTransferred)
			{
				var formattedData = new
				{
					Message = "The record is transferred to GL.You can not void .",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}
			if (!record.IsOnlineApproved)
			{
				var formattedData = new
				{
					Message = "The record is not approved hence you can modify by editing.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}


			return null;
		}
		protected override void Dispose(bool disposing)
		{
			_db.Dispose();
			base.Dispose(disposing);
		}
	}
}
