using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.SelfCare.Travel
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
	public class TravelExpenseClaimRequestController : Controller
	{
		// GET: Finance/ApTravelExpenseClaimRequest

		private readonly ExceedDbContext _db = new ExceedDbContext();
   
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
					Name = i.EmployeeCode + " " + i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
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
			if (GlobalHelper.IsMultipleBusinessUnit())
			{
				var businessUnits = _db.BusinessUnits.ToList();
				ViewData["BusinessUnits"] = businessUnits;
			}

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
	   

		public ActionResult TravelExpenseClaims_Read([DataSourceRequest]DataSourceRequest request)
		{
			List<int> userworkunit = UserInformationHelper.GetUserHeadWorkUnitAndChildWorkUnits(User.Identity.Name);
			IQueryable<ApTravelExpenseClaim> aptravelexpenseclaims = _db.ApTravelExpenseClaims;

			if (System.Web.HttpContext.Current.User.IsInRole(HrmConstants.HREmployeeSelfService)
				 && System.Web.HttpContext.Current.User.IsInRole(HrmConstants.HRDepartmentHead))
			{

				aptravelexpenseclaims = aptravelexpenseclaims.Where(x => userworkunit.Contains(x.CreatedAtOrgStructureId)
				|| x.CreatedAtOrgStructureId == 0//if they are not vis
				|| x.ForwardedTo == User.Identity.Name
				|| x.ForwardedFrom == User.Identity.Name
				|| x.CreatedBy == User.Identity.Name);

			}
			else if (System.Web.HttpContext.Current.User.IsInRole(HrmConstants.HREmployeeSelfService))
			{
				aptravelexpenseclaims = aptravelexpenseclaims.Where(x => x.CreatedBy == User.Identity.Name);
			}

		   
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

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult TravelExpenseClaims_Create([DataSourceRequest]DataSourceRequest request, ApTravelExpenseClaim apTravelExpenseClaim)
		{
			#region data 
			var travelorder = _db.ApTravelOrders.Find(apTravelExpenseClaim.ApTravelOrderId);
			if (travelorder != null)
			{
				apTravelExpenseClaim.FromBranchId = travelorder.FromBranchId;
				apTravelExpenseClaim.FromBusinessUnitId = travelorder.FromBusinessUnitId;

			}
			apTravelExpenseClaim.EmployeeId=travelorder.EmployeeId;
			//if (apTravelExpenseClaim.EmployeeId == 0)
			//{
			//    ModelState.AddModelError("requester", "Please select requestor");
			//}
			if (apTravelExpenseClaim.EmployeeId != 0)
			{               
				//if(travelorder.EmployeeId!=apTravelExpenseClaim.EmployeeId)
				//    ModelState.AddModelError("requester", "The requester is not the same as the travel order requester");

				var placement = _db.Placements.FirstOrDefault(x => x.EmployeeId == apTravelExpenseClaim.EmployeeId && x.isCurrent);
				if (placement != null)
				{
					apTravelExpenseClaim.OrgStructureId = placement.OrgStructureId;
					apTravelExpenseClaim.BranchId = placement.BranchId;
					apTravelExpenseClaim.BusinessUnitId = placement.BusinessUnitId;
				}
				else
				{
					ModelState.AddModelError("placement", "Employee has not current placemnt");
				}
			}

			apTravelExpenseClaim.CreatedAtBranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name);
			apTravelExpenseClaim.CreatedAtBusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
			apTravelExpenseClaim.CreatedAtOrgStructureId = UserInformationHelper.GetUserPlacementWorkUnit(User.Identity.Name);
			#endregion

			if (ModelState.IsValid)
			{
			   
				var entity = new ApTravelExpenseClaim
				{
					ApTravelOrderId = apTravelExpenseClaim.ApTravelOrderId,

					EmployeeId = apTravelExpenseClaim.EmployeeId,
					BranchId = apTravelExpenseClaim.BranchId,
					BusinessUnitId = apTravelExpenseClaim.BusinessUnitId,
					FromBranchId = apTravelExpenseClaim.FromBranchId,
					FromBusinessUnitId = apTravelExpenseClaim.FromBusinessUnitId,
					CreatedAtBranchId = apTravelExpenseClaim.CreatedAtBranchId,
					CreatedAtBusinessUnitId = apTravelExpenseClaim.CreatedAtBusinessUnitId,
					CreatedAtOrgStructureId = apTravelExpenseClaim.CreatedAtOrgStructureId,
					OrgStructureId = apTravelExpenseClaim.OrgStructureId,
					IsForOther = apTravelExpenseClaim.IsForOther,
				 
					PurposeOfTravel = apTravelExpenseClaim.PurposeOfTravel,
					ClaimDate = apTravelExpenseClaim.ClaimDate,
                    Difference = apTravelExpenseClaim.Difference,

                    ForwardedFrom = User.Identity.Name,
					ForwardedTo = User.Identity.Name,
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

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult TravelExpenseClaims_Update([DataSourceRequest]DataSourceRequest request, ApTravelExpenseClaim apTravelExpenseClaim)
		{
			#region data 
			var travelorder = _db.ApTravelOrders.Find(apTravelExpenseClaim.ApTravelOrderId);
			if (travelorder != null)
			{
				apTravelExpenseClaim.FromBranchId = travelorder.FromBranchId;
				apTravelExpenseClaim.FromBusinessUnitId = travelorder.FromBusinessUnitId;

			}
			
			apTravelExpenseClaim.EmployeeId = travelorder.EmployeeId;
			//if (apTravelExpenseClaim.EmployeeId == 0)
			//{
			//    ModelState.AddModelError("requester", "Please select requestor");
			//}
			if (apTravelExpenseClaim.EmployeeId != 0)
			{
				//if (travelorder.EmployeeId != apTravelExpenseClaim.EmployeeId)
				//    ModelState.AddModelError("requester", "The requester is not the same as the travel order requester");

				var placement = _db.Placements.FirstOrDefault(x => x.EmployeeId == apTravelExpenseClaim.EmployeeId && x.isCurrent);
				if (placement != null)
				{
					apTravelExpenseClaim.OrgStructureId = placement.OrgStructureId;
					apTravelExpenseClaim.BranchId = placement.BranchId;
					apTravelExpenseClaim.BusinessUnitId = placement.BusinessUnitId;
				}
				else
				{
					ModelState.AddModelError("placement", "Employee has not current placemnt");
				}
			}

			 #endregion
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
					CreatedAtOrgStructureId = apTravelExpenseClaim.CreatedAtOrgStructureId,
					EmployeeId = apTravelExpenseClaim.EmployeeId,
					IsForOther = apTravelExpenseClaim.IsForOther,
					OrgStructureId = apTravelExpenseClaim.OrgStructureId,
					PurposeOfTravel = apTravelExpenseClaim.PurposeOfTravel,
					ClaimDate = apTravelExpenseClaim.ClaimDate,
					
					Reference = apTravelExpenseClaim.Reference,
                    Difference = apTravelExpenseClaim.Difference,

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

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult TravelExpenseClaims_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelExpenseClaim apTravelExpenseClaim)
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
					CreatedAtOrgStructureId = apTravelExpenseClaim.CreatedAtOrgStructureId,
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


		public JsonResult SendforDepartmentApproval(int id)
		{
			var record = _db.ApTravelExpenseClaims.Find(id);
			string errMsg = "";

			if (record != null && record.IsDepartmentOnlineApproved)
			{
				errMsg = "You cannot manipulate this record,it is already approved";
				var formattedData = new
				{
					Message = errMsg,
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}
			if (record != null && record.IsDepartmentSendForApproval)
			{
				errMsg = "This record is already sent for department approval.";
				var formattedData = new
				{
					Message = errMsg,
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			var lines = _db.ApTravelExpenseExpenses.Where(x => x.ApTravelExpenseClaimId == id);
			if (!lines.Any())
			{
				errMsg = "Please add line expense items.";
				var formattedData = new
				{
					Message = errMsg,
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			if (!FinanceConfigurationHelper.IgnoreTravelDeparture())
			{

				var departureLines = _db.APTravelClaimDepartures.Where(x => x.ApTravelExpenseClaimId == id);
			  
				if (!departureLines.Any())
				{
					errMsg = "Please add departure items.";
					var formattedData = new
					{
						Message = errMsg,
						Status = "NOT_OK"
					};
					return Json(formattedData, JsonRequestBehavior.AllowGet);
				}

				if (!departureLines.Any(x => x.Type == DepartureType.Departure))
				{
					errMsg = "Please add departure line.";
					var formattedData = new
					{
						Message = errMsg,
						Status = "NOT_OK"
					};
					return Json(formattedData, JsonRequestBehavior.AllowGet);
				}

				if (!departureLines.Any(x => x.Type == DepartureType.Return))
				{
					errMsg = "Please add return line.";
					var formattedData = new
					{
						Message = errMsg,
						Status = "NOT_OK"
					};
					return Json(formattedData, JsonRequestBehavior.AllowGet);
				}

			   
			}
			if (record != null && !record.IsDepartmentOnlineApproved && !record.IsDepartmentSendForApproval)
			{
				record.Status = Enum.GetName(typeof(OperationStatuses), 16);
				record.IsDepartmentSendForApproval = true;
				record.DepartmentSendForApprovalBy = User.Identity.Name;
				record.DepartmentSendForApprovalTime = DateTime.Now;
				_db.Entry(record).State = EntityState.Modified;
				_db.SaveChanges();
				#region approval workflow

				bool isNewRecord = true;
				var validations = record.TravelExpenseDepartmentValidations.Where(x => x.Status == ApprovalStatuses.Approve).AsQueryable();
				if (validations.Any())
					isNewRecord = false;
				decimal docamount = record.Expenses.Sum(x => x.Total);
				int docid = record.ApTravelExpenseClaimId;
				int step = record.CurrentApprovalStep != null ? (int)record.CurrentApprovalStep : 0;
				DocApprovalWorkflowHelper.ForwardToApprover(docid, docamount, "", record.ForwardedTo, ApprovalDocumentType.ApTravelExpenseClaimSelfCare, isNewRecord,step);
				#endregion

				var formattedData = new
				{
					Message = "Successfully Sent.",
					Status = "OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);

			}
			return null;
		}

		[AuthorizeRoles(HrmConstants.HRDepartmentHead)]
		public JsonResult Void(int id)
		{
			var record = _db.ApTravelExpenseClaims.Find(id);
			if (record != null && record.IsDepartmentOnlineApproved && !record.IsBudgetApproved && !record.IsOnlineApproved && !record.IsVoid)
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
				record.ForwardedFrom = User.Identity.Name;
				record.ForwardedTo = User.Identity.Name;
				_db.ApTravelExpenseClaims.Attach(record);
				_db.Entry(record).State = EntityState.Modified;
				_db.SaveChanges();

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


			if (!record.IsDepartmentOnlineApproved)
			{
				var formattedData = new
				{
					Message = "The record is not approved hence you can modify by editing.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			if (record.IsBudgetApproved)
			{
				var formattedData = new
				{
					Message = "The record is approved at budget hence you can't void here.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			if (record.IsOnlineApproved)
			{
				var formattedData = new
				{
					Message = "The record is approved at finance hence you can't void here.",
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
