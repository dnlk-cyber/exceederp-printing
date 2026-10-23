using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Budget.Controllers.TravelRequests
{
    [Authorize]
	public class TravelExpenseClaimRequestBudgetController : Controller
	{


		private readonly ExceedDbContext _db = new ExceedDbContext();

		[AuthorizeRoles(BudgetRoles.BudgetPayment, BudgetRoles.BudgetPaymentApproval)]
		public ActionResult Index()
		{
			PopulateViewData();
			return View();
		}

		private void PopulateViewData()
		{
			var employee = _db.Person_Employee;
			var selectedEmployee = employee.Select(
				i => new
				{
					Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
					Code = i.EmployeeId
				}).AsQueryable();
			ViewData["personEmployees"] = selectedEmployee;
			

			var branches = _db.Branch;
			var costCenter = _db.BranchCostCenter;
			var selectedcostCenter = costCenter.Select(
				i => new
				{
					Name = i.MainCompanyStructure.Name,
					Code = i.OrgStructureId
				}).AsQueryable();



			ViewData["costCenters"] = selectedcostCenter;

			ViewData["Branches"] = branches;

            var items = _db.ApSetupItems.ToList();
            ViewData["Items"] = items;
        }
		public string CheckStatus(int masterId)
		{
			var reques = _db.RequestesStatus.FirstOrDefault(x => x.RequestId == masterId && x.ReferenceType == "TravelExpenseClaim");
			if (reques != null)
			{
				var checkApprove = _db.RequestesStatus.FirstOrDefault(x => x.RequestId == masterId && x.ReferenceType == "TravelExpenseClaim" && x.Status == RequestStatus.APPROVE);
				if (checkApprove != null)
				{
					return "approved";
				}
				var deptApproval = _db.RequestesStatus.FirstOrDefault(x => x.RequestId == masterId && x.ReferenceType == "TravelExpenseClaim" && x.Status == RequestStatus.DEPARTMENT_APPROVAL);
				if (deptApproval != null)
				{
					return "Dapproved";
				}
				return "true";
			}
			return "false";
		}

		[AuthorizeRoles(BudgetRoles.BudgetPayment, BudgetRoles.BudgetPaymentApproval)]
		public ActionResult ApTravelExpenseClaims_Read([DataSourceRequest]DataSourceRequest request)
		{
			IQueryable<ApTravelExpenseClaim> aptravelexpenseclaims = _db.ApTravelExpenseClaims
				.Where(x => x.IsDepartmentOnlineApproved)
				.Where(x => x.Difference!=0);

			DataSourceResult result = aptravelexpenseclaims.ToDataSourceResult(request, apTravelExpenseClaim => new
			{
				ApTravelOrderId = apTravelExpenseClaim.ApTravelOrderId,
				ApTravelExpenseClaimId = apTravelExpenseClaim.ApTravelExpenseClaimId,
				
				BranchId = apTravelExpenseClaim.BranchId,
				BusinessUnitId = apTravelExpenseClaim.BusinessUnitId,
				FromBranchId = apTravelExpenseClaim.FromBranchId,
				FromBusinessUnitId = apTravelExpenseClaim.FromBusinessUnitId,
				CreatedAtBranchId = apTravelExpenseClaim.CreatedAtBranchId,
				CreatedAtBusinessUnitId = apTravelExpenseClaim.CreatedAtBusinessUnitId,
				CreatedAtOrgStructureId = apTravelExpenseClaim.CreatedAtOrgStructureId,
				EmployeeId = apTravelExpenseClaim.EmployeeId,
				IsForOther = apTravelExpenseClaim.IsForOther,
				OrgStructureId = apTravelExpenseClaim.OrgStructureId,
				PurposeOfTravel = apTravelExpenseClaim.PurposeOfTravel,
				ClaimDate = apTravelExpenseClaim.ClaimDate,
				Difference = apTravelExpenseClaim.Difference,
				Reference = apTravelExpenseClaim.Reference,

				IsDepartmentOnlineApproved = apTravelExpenseClaim.IsDepartmentOnlineApproved,
				DepartmentOnlineApprovedBy = apTravelExpenseClaim.DepartmentOnlineApprovedBy,
				DepartmentOnlineApprovedTime = apTravelExpenseClaim.DepartmentOnlineApprovedTime,
				IsDepartmentSendForApproval = apTravelExpenseClaim.IsDepartmentSendForApproval,
				DepartmentSendForApprovalBy = apTravelExpenseClaim.DepartmentSendForApprovalBy,
				DepartmentSendForApprovalTime = apTravelExpenseClaim.DepartmentSendForApprovalTime,

				ForwardedFrom = apTravelExpenseClaim.ForwardedFrom,
				ForwardedTo = apTravelExpenseClaim.ForwardedTo,
				IsFinalApproval = apTravelExpenseClaim.IsFinalApproval,
				CurrentApprovalStep = apTravelExpenseClaim.CurrentApprovalStep,

				IsBudgetApproved = apTravelExpenseClaim.IsBudgetApproved,
				BudgetApprovedBy = apTravelExpenseClaim.BudgetApprovedBy,
				BudgetApprovedTime = apTravelExpenseClaim.BudgetApprovedTime,

				IsOnlineTransferred = apTravelExpenseClaim.IsOnlineTransferred,
				IsOnlineTransferredBy = apTravelExpenseClaim.IsOnlineTransferredBy,
				OnlineTransferredTime = apTravelExpenseClaim.OnlineTransferredTime,
				Status = apTravelExpenseClaim.Status,
				IsOnlineApproved = apTravelExpenseClaim.IsOnlineApproved,
				OnlineApprovedBy = apTravelExpenseClaim.OnlineApprovedBy,
				OnlineApprovedTime = apTravelExpenseClaim.OnlineApprovedTime,
				IsVoid = apTravelExpenseClaim.IsVoid,
				VoidBy = apTravelExpenseClaim.VoidBy,
				VoidTime = apTravelExpenseClaim.VoidTime,
				IsSendForApproval = apTravelExpenseClaim.IsSendForApproval,
				SendForApprovalBy = apTravelExpenseClaim.SendForApprovalBy,
				SendForApprovalTime = apTravelExpenseClaim.SendForApprovalTime,
				OrgBranchName = apTravelExpenseClaim.OrgBranchName,
				DateCreated = apTravelExpenseClaim.DateCreated,
				LastModified = apTravelExpenseClaim.LastModified,
				CreatedBy = apTravelExpenseClaim.CreatedBy,
				ModifiedBy = apTravelExpenseClaim.ModifiedBy
			});

			return Json(result);
		}

		[AuthorizeRoles(BudgetRoles.BudgetPayment)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult ApTravelExpenseClaims_Create([DataSourceRequest]DataSourceRequest request, ApTravelExpenseClaim apTravelExpenseClaim)
		{
			

			if (ModelState.IsValid)
			{
			  
				var entity = new ApTravelExpenseClaim
				{
					ApTravelOrderId = apTravelExpenseClaim.ApTravelOrderId,
					BranchId = apTravelExpenseClaim.BranchId,
					BusinessUnitId = apTravelExpenseClaim.BusinessUnitId,
					FromBranchId = apTravelExpenseClaim.FromBranchId,
					FromBusinessUnitId = apTravelExpenseClaim.FromBusinessUnitId,
					CreatedAtBranchId = apTravelExpenseClaim.CreatedAtBranchId,
					CreatedAtBusinessUnitId = apTravelExpenseClaim.CreatedAtBusinessUnitId,
					EmployeeId = apTravelExpenseClaim.EmployeeId,
					IsForOther = apTravelExpenseClaim.IsForOther,
					OrgStructureId = apTravelExpenseClaim.OrgStructureId,

					PurposeOfTravel = apTravelExpenseClaim.PurposeOfTravel,
					ClaimDate = apTravelExpenseClaim.ClaimDate,
					Difference = apTravelExpenseClaim.Difference,
					Reference = apTravelExpenseClaim.Reference,

					ForwardedFrom = apTravelExpenseClaim.ForwardedFrom,
					ForwardedTo = apTravelExpenseClaim.ForwardedTo,
					IsFinalApproval = apTravelExpenseClaim.IsFinalApproval,
					CurrentApprovalStep = apTravelExpenseClaim.CurrentApprovalStep,

					IsBudgetApproved = apTravelExpenseClaim.IsBudgetApproved,
					BudgetApprovedBy = apTravelExpenseClaim.BudgetApprovedBy,
					BudgetApprovedTime = apTravelExpenseClaim.BudgetApprovedTime,

					IsDepartmentOnlineApproved = apTravelExpenseClaim.IsDepartmentOnlineApproved,
					DepartmentOnlineApprovedBy = apTravelExpenseClaim.DepartmentOnlineApprovedBy,
					DepartmentOnlineApprovedTime = apTravelExpenseClaim.DepartmentOnlineApprovedTime,
					IsDepartmentSendForApproval = apTravelExpenseClaim.IsDepartmentSendForApproval,
					DepartmentSendForApprovalBy = apTravelExpenseClaim.DepartmentSendForApprovalBy,
					DepartmentSendForApprovalTime = apTravelExpenseClaim.DepartmentSendForApprovalTime,

					IsOnlineTransferred = apTravelExpenseClaim.IsOnlineTransferred,
					IsOnlineTransferredBy = apTravelExpenseClaim.IsOnlineTransferredBy,
					OnlineTransferredTime = apTravelExpenseClaim.OnlineTransferredTime,
					Status = apTravelExpenseClaim.Status,
					IsOnlineApproved = apTravelExpenseClaim.IsOnlineApproved,
					OnlineApprovedBy = apTravelExpenseClaim.OnlineApprovedBy,
					OnlineApprovedTime = apTravelExpenseClaim.OnlineApprovedTime,
					IsVoid = apTravelExpenseClaim.IsVoid,
					VoidBy = apTravelExpenseClaim.VoidBy,
					VoidTime = apTravelExpenseClaim.VoidTime,
					IsSendForApproval = apTravelExpenseClaim.IsSendForApproval,
					SendForApprovalBy = apTravelExpenseClaim.SendForApprovalBy,
					SendForApprovalTime = apTravelExpenseClaim.SendForApprovalTime,
					OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
					DateCreated = DateTime.Now,
					LastModified = apTravelExpenseClaim.LastModified,
					CreatedBy = User.Identity.Name,
					ModifiedBy = apTravelExpenseClaim.ModifiedBy
				};

				_db.ApTravelExpenseClaims.Add(entity);
				_db.SaveChanges();
				apTravelExpenseClaim.ApTravelExpenseClaimId = entity.ApTravelExpenseClaimId;

				#region operation log
				UserHelper.OperationLog(Request.UserHostAddress, "Self care travel expense claim", "Success", User.Identity.Name, $"created claim {entity.ApTravelExpenseClaimId} ", Modules.Finance);
				#endregion
			}

			return Json(new[] { apTravelExpenseClaim }.ToDataSourceResult(request, ModelState));
		}

		[AuthorizeRoles(BudgetRoles.BudgetPayment)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult ApTravelExpenseClaims_Update([DataSourceRequest]DataSourceRequest request, ApTravelExpenseClaim apTravelExpenseClaim)
		{
			if (ModelState.IsValid)
			{
				var entity = new ApTravelExpenseClaim
				{
					ApTravelOrderId = apTravelExpenseClaim.ApTravelOrderId,
					ApTravelExpenseClaimId = apTravelExpenseClaim.ApTravelExpenseClaimId,
					BranchId = apTravelExpenseClaim.BranchId,
					BusinessUnitId = apTravelExpenseClaim.BusinessUnitId,
					FromBranchId = apTravelExpenseClaim.FromBranchId,
					FromBusinessUnitId = apTravelExpenseClaim.FromBusinessUnitId,
					CreatedAtBranchId = apTravelExpenseClaim.CreatedAtBranchId,
					CreatedAtBusinessUnitId = apTravelExpenseClaim.CreatedAtBusinessUnitId,
					EmployeeId = apTravelExpenseClaim.EmployeeId,
					IsForOther = apTravelExpenseClaim.IsForOther,
					OrgStructureId = apTravelExpenseClaim.OrgStructureId,
					PurposeOfTravel = apTravelExpenseClaim.PurposeOfTravel,
					ClaimDate = apTravelExpenseClaim.ClaimDate,
					Difference = apTravelExpenseClaim.Difference,
					Reference = apTravelExpenseClaim.Reference,

					ForwardedFrom = apTravelExpenseClaim.ForwardedFrom,
					ForwardedTo = apTravelExpenseClaim.ForwardedTo,
					IsFinalApproval = apTravelExpenseClaim.IsFinalApproval,
					CurrentApprovalStep = apTravelExpenseClaim.CurrentApprovalStep,

					IsBudgetApproved = apTravelExpenseClaim.IsBudgetApproved,
					BudgetApprovedBy = apTravelExpenseClaim.BudgetApprovedBy,
					BudgetApprovedTime = apTravelExpenseClaim.BudgetApprovedTime,

					IsDepartmentOnlineApproved = apTravelExpenseClaim.IsDepartmentOnlineApproved,
					DepartmentOnlineApprovedBy = apTravelExpenseClaim.DepartmentOnlineApprovedBy,
					DepartmentOnlineApprovedTime = apTravelExpenseClaim.DepartmentOnlineApprovedTime,
					IsDepartmentSendForApproval = apTravelExpenseClaim.IsDepartmentSendForApproval,
					DepartmentSendForApprovalBy = apTravelExpenseClaim.DepartmentSendForApprovalBy,
					DepartmentSendForApprovalTime = apTravelExpenseClaim.DepartmentSendForApprovalTime,

					IsOnlineTransferred = apTravelExpenseClaim.IsOnlineTransferred,
					IsOnlineTransferredBy = apTravelExpenseClaim.IsOnlineTransferredBy,
					OnlineTransferredTime = apTravelExpenseClaim.OnlineTransferredTime,
					Status = apTravelExpenseClaim.Status,
					IsOnlineApproved = apTravelExpenseClaim.IsOnlineApproved,
					OnlineApprovedBy = apTravelExpenseClaim.OnlineApprovedBy,
					OnlineApprovedTime = apTravelExpenseClaim.OnlineApprovedTime,
					IsVoid = apTravelExpenseClaim.IsVoid,
					VoidBy = apTravelExpenseClaim.VoidBy,
					VoidTime = apTravelExpenseClaim.VoidTime,
					IsSendForApproval = apTravelExpenseClaim.IsSendForApproval,
					SendForApprovalBy = apTravelExpenseClaim.SendForApprovalBy,
					SendForApprovalTime = apTravelExpenseClaim.SendForApprovalTime,
					OrgBranchName = apTravelExpenseClaim.OrgBranchName,
					DateCreated = apTravelExpenseClaim.DateCreated,
					LastModified = DateTime.Now,
					CreatedBy = apTravelExpenseClaim.CreatedBy,
					ModifiedBy = User.Identity.Name
				};

				_db.ApTravelExpenseClaims.Attach(entity);
				_db.Entry(entity).State = EntityState.Modified;
				_db.SaveChanges();

				#region operation log
				UserHelper.OperationLog(Request.UserHostAddress, "Self care travel expense claim", "Success", User.Identity.Name, $"updated claim {entity.ApTravelExpenseClaimId} ", Modules.Finance);
				#endregion
			}

			return Json(new[] { apTravelExpenseClaim }.ToDataSourceResult(request, ModelState));
		}

		[AuthorizeRoles(BudgetRoles.BudgetPayment)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult ApTravelExpenseClaims_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelExpenseClaim apTravelExpenseClaim)
		{
			if (ModelState.IsValid)
			{
				var entity = new ApTravelExpenseClaim
				{
					ApTravelOrderId = apTravelExpenseClaim.ApTravelOrderId,
					ApTravelExpenseClaimId = apTravelExpenseClaim.ApTravelExpenseClaimId,

					BranchId = apTravelExpenseClaim.BranchId,
					BusinessUnitId = apTravelExpenseClaim.BusinessUnitId,
					FromBranchId = apTravelExpenseClaim.FromBranchId,
					FromBusinessUnitId = apTravelExpenseClaim.FromBusinessUnitId,
					CreatedAtBranchId = apTravelExpenseClaim.CreatedAtBranchId,
					CreatedAtBusinessUnitId = apTravelExpenseClaim.CreatedAtBusinessUnitId,
					EmployeeId = apTravelExpenseClaim.EmployeeId,
					IsForOther = apTravelExpenseClaim.IsForOther,
					OrgStructureId = apTravelExpenseClaim.OrgStructureId,
					PurposeOfTravel = apTravelExpenseClaim.PurposeOfTravel,
					ClaimDate = apTravelExpenseClaim.ClaimDate,
					Difference = apTravelExpenseClaim.Difference,
					Reference = apTravelExpenseClaim.Reference,
					IsOnlineTransferred = apTravelExpenseClaim.IsOnlineTransferred,
					IsOnlineTransferredBy = apTravelExpenseClaim.IsOnlineTransferredBy,
					OnlineTransferredTime = apTravelExpenseClaim.OnlineTransferredTime,
					Status = apTravelExpenseClaim.Status,
					IsOnlineApproved = apTravelExpenseClaim.IsOnlineApproved,
					OnlineApprovedBy = apTravelExpenseClaim.OnlineApprovedBy,
					OnlineApprovedTime = apTravelExpenseClaim.OnlineApprovedTime,
					IsVoid = apTravelExpenseClaim.IsVoid,
					VoidBy = apTravelExpenseClaim.VoidBy,
					VoidTime = apTravelExpenseClaim.VoidTime,
					IsSendForApproval = apTravelExpenseClaim.IsSendForApproval,
					SendForApprovalBy = apTravelExpenseClaim.SendForApprovalBy,
					SendForApprovalTime = apTravelExpenseClaim.SendForApprovalTime,
					OrgBranchName = apTravelExpenseClaim.OrgBranchName,
					DateCreated = apTravelExpenseClaim.DateCreated,
					LastModified = apTravelExpenseClaim.LastModified,
					CreatedBy = apTravelExpenseClaim.CreatedBy,
					ModifiedBy = apTravelExpenseClaim.ModifiedBy
				};

				_db.ApTravelExpenseClaims.Attach(entity);
				_db.ApTravelExpenseClaims.Remove(entity);
				_db.SaveChanges();
			}

			return Json(new[] { apTravelExpenseClaim }.ToDataSourceResult(request, ModelState));
		}


		[AuthorizeRoles(BudgetRoles.BudgetVoid)]
		public JsonResult Void(int id)
		{
			var record = _db.ApTravelExpenseClaims.Find(id);
			if (record != null && record.IsBudgetApproved && !record.IsOnlineApproved)
			{

				record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.IsDepartmentOnlineApproved = false;
                record.IsDepartmentSendForApproval = false;
                record.IsBudgetApproved = false;
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsVoid = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.ApTravelExpenseClaims.Attach(record);
				_db.Entry(record).State = EntityState.Modified;
				_db.SaveChanges();


				//copy
				//CopyRecord(id);
				#region operation log
				UserHelper.OperationLog(Request.UserHostAddress, "PR budget Void", "Success", User.Identity.Name, $"void PR {id} ", Modules.Budget);
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

			if (!record.IsBudgetApproved)
			{
				var formattedData = new
				{
					Message = "The record is not approved hence you can modify by editing.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			if (record.IsOnlineApproved)
			{
				var formattedData = new
				{
					Message = "The record is approved at finance, it should be void there first.",
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
