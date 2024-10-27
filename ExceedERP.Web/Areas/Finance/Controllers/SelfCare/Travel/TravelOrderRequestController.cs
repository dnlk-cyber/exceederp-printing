using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.SelfCare.Travel
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
	public class TravelOrderRequestController : Controller
	{
	   

		private ExceedDbContext _db = new ExceedDbContext();
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
					Name = i.EmployeeCode + " "  + i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
					Code = i.EmployeeId
				}).AsQueryable();
			ViewData["personEmployees"] = selectedEmployee;
			var branches = _db.Branch;
			var structure = _db.MainCompanyStructures;


			ViewData["Structure"] = structure;

			ViewData["Branches"] = branches;

			if (GlobalHelper.IsMultipleBusinessUnit())
			{
				var businessUnits = _db.BusinessUnits.ToList();
				ViewData["BusinessUnits"] = businessUnits;
			}

			var items = _db.ApSetupItems.ToList();
			ViewData["Items"] = items;
		}

	   
		public ActionResult TravelOrderRequest_Read([DataSourceRequest]DataSourceRequest request)
		{
			List<int> userworkunit = UserInformationHelper.GetUserHeadWorkUnitAndChildWorkUnits(User.Identity.Name);
			IQueryable<ApTravelOrder> aptravelorders = _db.ApTravelOrders;

			if (System.Web.HttpContext.Current.User.IsInRole(HrmConstants.HREmployeeSelfService)
				 && System.Web.HttpContext.Current.User.IsInRole(HrmConstants.HRDepartmentHead))
			{

				aptravelorders = aptravelorders.Where(x => userworkunit.Contains(x.CreatedAtOrgStructureId)
				|| x.CreatedAtOrgStructureId == 0//if they are not vis
				|| x.ForwardedTo == User.Identity.Name
				|| x.ForwardedFrom == User.Identity.Name
				|| x.CreatedBy == User.Identity.Name);

			}
			else if (System.Web.HttpContext.Current.User.IsInRole(HrmConstants.HREmployeeSelfService))
			{
				aptravelorders = aptravelorders.Where(x => x.CreatedBy == User.Identity.Name);
			}

			DataSourceResult result = aptravelorders.ToDataSourceResult(request, apTravelOrder => new
			{
				ApTravelOrderId = apTravelOrder.ApTravelOrderId,
				BranchId = apTravelOrder.BranchId,
				BusinessUnitId = apTravelOrder.BusinessUnitId,
				FromBranchId = apTravelOrder.FromBranchId,
				FromBusinessUnitId = apTravelOrder.FromBusinessUnitId,
				CreatedAtBranchId = apTravelOrder.CreatedAtBranchId,
				CreatedAtBusinessUnitId = apTravelOrder.CreatedAtBusinessUnitId,
				CreatedAtOrgStructureId = apTravelOrder.CreatedAtOrgStructureId,
				EmployeeId = apTravelOrder.EmployeeId,
				IsForOther = apTravelOrder.IsForOther,
				OrgStructureId = apTravelOrder.OrgStructureId,
				PurposeOfTravel = apTravelOrder.PurposeOfTravel,
				RequestDate = apTravelOrder.RequestDate,
				AdvanceAmount = apTravelOrder.AdvanceAmount,
				TotalDuration = apTravelOrder.TotalDuration,
				TransportReference = apTravelOrder.TransportReference,
				Reference = apTravelOrder.Reference,
				VehicleDistributionID = apTravelOrder.VehicleDistributionID,
                Claimed = apTravelOrder.Claimed,
                Paid = apTravelOrder.Paid,

				IsDepartmentOnlineApproved = apTravelOrder.IsDepartmentOnlineApproved,
				DepartmentOnlineApprovedBy = apTravelOrder.DepartmentOnlineApprovedBy,
				DepartmentOnlineApprovedTime = apTravelOrder.DepartmentOnlineApprovedTime,
				IsDepartmentSendForApproval = apTravelOrder.IsDepartmentSendForApproval,
				DepartmentSendForApprovalBy = apTravelOrder.DepartmentSendForApprovalBy,
				DepartmentSendForApprovalTime = apTravelOrder.DepartmentSendForApprovalTime,

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
			});

			return Json(result);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult TravelOrderRequest_Create([DataSourceRequest]DataSourceRequest request, ApTravelOrder apTravelOrder)
		{
			#region data 
			if (apTravelOrder.EmployeeId == 0)
			{
				ModelState.AddModelError("requester", "Please select requestor");
			}
			if (apTravelOrder.EmployeeId != 0)
			{

				//var draftTravel=_db.ApTravelOrders
				//	.Where(x=>x.EmployeeId==apTravelOrder.EmployeeId)
				//	.Where(x=>x.IsDepartmentOnlineApproved==false)
				//	.Where(x=>x.IsVoid==false)
				//	.FirstOrDefault();

				//if(draftTravel != null)
				//	ModelState.AddModelError("requester", $"Please complete your previous request {draftTravel.ApTravelOrderId}");


				var placement = _db.Placements.FirstOrDefault(x => x.EmployeeId == apTravelOrder.EmployeeId && x.isCurrent);
				if (placement != null)
				{
					apTravelOrder.OrgStructureId = placement.OrgStructureId;
					apTravelOrder.BranchId = placement.BranchId;
					apTravelOrder.BusinessUnitId = placement.BusinessUnitId;
				}
				else
				{
					ModelState.AddModelError("placement", "Employee has not current placemnt");
				}
			}

			if(apTravelOrder.FromBranchId==0)
				apTravelOrder.FromBranchId = apTravelOrder.BranchId;
			if (apTravelOrder.FromBusinessUnitId == 0)
				apTravelOrder.FromBusinessUnitId = apTravelOrder.BusinessUnitId;

			apTravelOrder.CreatedAtBranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name);
			apTravelOrder.CreatedAtBusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
			apTravelOrder.CreatedAtOrgStructureId = UserInformationHelper.GetUserPlacementWorkUnit(User.Identity.Name);
		   
			#endregion

			if (ModelState.IsValid)
			{
			 
			  
				var entity = new ApTravelOrder
				{
					EmployeeId = apTravelOrder.EmployeeId,
					BranchId = apTravelOrder.BranchId,
					BusinessUnitId = apTravelOrder.BusinessUnitId,
					FromBranchId = apTravelOrder.FromBranchId,
					FromBusinessUnitId = apTravelOrder.FromBusinessUnitId,
					CreatedAtBranchId = apTravelOrder.CreatedAtBranchId,
					CreatedAtBusinessUnitId = apTravelOrder.CreatedAtBusinessUnitId,
					CreatedAtOrgStructureId = apTravelOrder.CreatedAtOrgStructureId,
					OrgStructureId = apTravelOrder.OrgStructureId,
					PurposeOfTravel = apTravelOrder.PurposeOfTravel,
					RequestDate = apTravelOrder.RequestDate,
					AdvanceAmount = apTravelOrder.AdvanceAmount,
					TotalDuration = apTravelOrder.TotalDuration,
					TransportReference = apTravelOrder.TransportReference,
					Reference = apTravelOrder.Reference,
                    Claimed = apTravelOrder.Claimed,
                    Paid = apTravelOrder.Paid,
                    IsOnlineTransferred = apTravelOrder.IsOnlineTransferred,
					IsOnlineTransferredBy = apTravelOrder.IsOnlineTransferredBy,
					OnlineTransferredTime = apTravelOrder.OnlineTransferredTime,
					Status = apTravelOrder.Status,

					ForwardedFrom = User.Identity.Name,
					ForwardedTo = User.Identity.Name,
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
					DateCreated = DateTime.Now,
					LastModified = apTravelOrder.LastModified,
					CreatedBy = User.Identity.Name,
					ModifiedBy = apTravelOrder.ModifiedBy
				};

				_db.ApTravelOrders.Add(entity);
				_db.SaveChanges();
				apTravelOrder.ApTravelOrderId = entity.ApTravelOrderId;

				#region operation log
				UserHelper.OperationLog(Request.UserHostAddress, "Self care travel order", "Success", User.Identity.Name, $"created expense claim {entity.ApTravelOrderId} ", Modules.Finance);
				#endregion
			}

			return Json(new[] { apTravelOrder }.ToDataSourceResult(request, ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult TravelOrderRequest_Update([DataSourceRequest]DataSourceRequest request, ApTravelOrder apTravelOrder)
		{
			#region data 
			if (apTravelOrder.EmployeeId == 0)
			{
				ModelState.AddModelError("requester", "Please select requestor");
			}
			if (apTravelOrder.EmployeeId != 0)
			{
				var placement = _db.Placements.FirstOrDefault(x => x.EmployeeId == apTravelOrder.EmployeeId && x.isCurrent);
				if (placement != null)
				{
					apTravelOrder.OrgStructureId = placement.OrgStructureId;
					apTravelOrder.BranchId = placement.BranchId;
					apTravelOrder.BusinessUnitId = placement.BusinessUnitId;
				}
				else
				{
					ModelState.AddModelError("placement", "Employee has not current placemnt");
				}
			}

			if (apTravelOrder.FromBranchId == 0)
				apTravelOrder.FromBranchId = apTravelOrder.BranchId;
			if (apTravelOrder.FromBusinessUnitId == 0)
				apTravelOrder.FromBusinessUnitId = apTravelOrder.BusinessUnitId;

			#endregion

			if (ModelState.IsValid)
			{
				var entity = new ApTravelOrder
				{
					ApTravelOrderId = apTravelOrder.ApTravelOrderId,
					BranchId = apTravelOrder.BranchId,
					BusinessUnitId = apTravelOrder.BusinessUnitId,
					FromBranchId = apTravelOrder.FromBranchId,
					FromBusinessUnitId = apTravelOrder.FromBusinessUnitId,
					CreatedAtBranchId = apTravelOrder.CreatedAtBranchId,
					CreatedAtBusinessUnitId = apTravelOrder.CreatedAtBusinessUnitId,
					CreatedAtOrgStructureId = apTravelOrder.CreatedAtOrgStructureId,
					EmployeeId = apTravelOrder.EmployeeId,
					IsForOther = apTravelOrder.IsForOther,
					OrgStructureId = apTravelOrder.OrgStructureId,
					PurposeOfTravel = apTravelOrder.PurposeOfTravel,
					RequestDate = apTravelOrder.RequestDate,
					AdvanceAmount = apTravelOrder.AdvanceAmount,
					TotalDuration = apTravelOrder.TotalDuration,
					TransportReference = apTravelOrder.TransportReference,
					Reference = apTravelOrder.Reference,
                    Claimed = apTravelOrder.Claimed,
                    Paid = apTravelOrder.Paid,
                    IsOnlineTransferred = apTravelOrder.IsOnlineTransferred,
					IsOnlineTransferredBy = apTravelOrder.IsOnlineTransferredBy,
					OnlineTransferredTime = apTravelOrder.OnlineTransferredTime,
					Status = apTravelOrder.Status,

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
					LastModified = DateTime.Now,
					CreatedBy = apTravelOrder.CreatedBy,
					ModifiedBy = User.Identity.Name
				};

				_db.ApTravelOrders.Attach(entity);
				_db.Entry(entity).State = EntityState.Modified;
				_db.SaveChanges();

				#region operation log
				UserHelper.OperationLog(Request.UserHostAddress, "Self care travel order", "Success", User.Identity.Name, $"updated expense claim {entity.ApTravelOrderId} ", Modules.Finance);
				#endregion
			}

			return Json(new[] { apTravelOrder }.ToDataSourceResult(request, ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult TravelOrderRequest_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelOrder apTravelOrder)
		{
			if (ModelState.IsValid)
			{
				var entity = new ApTravelOrder
				{
					ApTravelOrderId = apTravelOrder.ApTravelOrderId,                  
					BranchId = apTravelOrder.BranchId,
					BusinessUnitId = apTravelOrder.BusinessUnitId,
					FromBranchId = apTravelOrder.FromBranchId,
					FromBusinessUnitId = apTravelOrder.FromBusinessUnitId,
					CreatedAtBranchId = apTravelOrder.CreatedAtBranchId,
					CreatedAtBusinessUnitId = apTravelOrder.CreatedAtBusinessUnitId,
					CreatedAtOrgStructureId = apTravelOrder.CreatedAtOrgStructureId,
					EmployeeId = apTravelOrder.EmployeeId,
					IsForOther = apTravelOrder.IsForOther,
					OrgStructureId = apTravelOrder.OrgStructureId,
					PurposeOfTravel = apTravelOrder.PurposeOfTravel,
					RequestDate = apTravelOrder.RequestDate,
					AdvanceAmount = apTravelOrder.AdvanceAmount,
					TotalDuration = apTravelOrder.TotalDuration,
					Reference = apTravelOrder.Reference,
                    Claimed = apTravelOrder.Claimed,
                    Paid = apTravelOrder.Paid,
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
			}

			return Json(new[] { apTravelOrder }.ToDataSourceResult(request, ModelState));
		}


		public JsonResult SendforDepartmentApproval(int id)
		{
			var record = _db.ApTravelOrders.Find(id);
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

			if (!FinanceConfigurationHelper.IgnoreTravelDeparture())
			{

				var lines = _db.ApTravelOrderDepartures.Where(x => x.ApTravelOrderId == id);
				var paymentLines = _db.ApTravelOrderAdvancePayments.Where(x => x.ApTravelOrderId == id);
				if (!lines.Any())
				{
					errMsg = "Please add departure items.";
					var formattedData = new
					{
						Message = errMsg,
						Status = "NOT_OK"
					};
					return Json(formattedData, JsonRequestBehavior.AllowGet);
				}

				if (!lines.Any(x => x.Type == DepartureType.Departure))
				{
					errMsg = "Please add departure line.";
					var formattedData = new
					{
						Message = errMsg,
						Status = "NOT_OK"
					};
					return Json(formattedData, JsonRequestBehavior.AllowGet);
				}

				if (!lines.Any(x => x.Type == DepartureType.Return))
				{
					errMsg = "Please add return line.";
					var formattedData = new
					{
						Message = errMsg,
						Status = "NOT_OK"
					};
					return Json(formattedData, JsonRequestBehavior.AllowGet);
				}

				if (!paymentLines.Any())
				{
					errMsg = "Please add advance payment.";
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

				#region validate date overlap overlap
				var currentAdvancePayments = _db.ApTravelOrderAdvancePayments.Where(x => x.ApTravelOrderId == id).ToList();
				if (currentAdvancePayments.Any(x=>x.Type==TravelPaymentType.PerDiem))
				{
					var prevtravelOrders = _db.ApTravelOrders
					.Where(x => x.ApTravelOrderId != id)
					.Where(x => x.EmployeeId == record.EmployeeId)
					.Where(x => x.IsVoid == false)
					.Select(x => x.ApTravelOrderId)
					.ToList();

					var departures = _db.ApTravelOrderDepartures.Where(x => prevtravelOrders.Contains(x.ApTravelOrderId)).ToList();
					var currentDepartures = _db.ApTravelOrderDepartures.Where(x => x.ApTravelOrderId == id).ToList();
					foreach (var departure in currentDepartures)
					{
						bool overlap = departures.Any(x =>
							(departure.Date < x.ArrivalDate && departure.ArrivalDate > x.Date) || // Original overlap check
							(departure.Date >= x.Date && departure.ArrivalDate <= x.ArrivalDate) // Date range completely inside another
						);

						if (overlap)
						{
							errMsg = "Departure date or arrival date overlapped previous travel order.";

							var formattederrData = new
							{
								Message = errMsg,
								Status = "NOT_OK"
							};
							return Json(formattederrData, JsonRequestBehavior.AllowGet);
						}

						var travelDetails = _db.ApTravelOrderDepartures
						.Where(t => prevtravelOrders.Contains(t.ApTravelOrderId))
						.GroupBy(t => t.ApTravelOrderId)
						 .Select(g => new
						 {
							 TravelOrderId = g.Key,
							 DepartureDate = g.Where(x => x.Type == DepartureType.Departure).Select(x => x.Date).FirstOrDefault(),
							 ReturnDate = g.Where(x => x.Type == DepartureType.Return).Select(x => x.ArrivalDate).FirstOrDefault()
						 })
						.ToList();

						bool overlapacross = travelDetails.Any(x =>
						 (departure.Date >= x.DepartureDate && departure.ArrivalDate <= x.ReturnDate)
							);

						if (overlapacross)
						{
							errMsg = "Departure date or arrival date overlapped previous travel order.";

							var formattederrData = new
							{
								Message = errMsg,
								Status = "NOT_OK"
							};
							return Json(formattederrData, JsonRequestBehavior.AllowGet);
						}
					}
				}

				#endregion

				record.Status = Enum.GetName(typeof(OperationStatuses), 16);
				record.IsDepartmentSendForApproval = true;
				record.DepartmentSendForApprovalBy = User.Identity.Name;
				record.DepartmentSendForApprovalTime = DateTime.Now;
				_db.Entry(record).State = EntityState.Modified;
				_db.SaveChanges();
				#region approval workflow

				bool isNewRecord = true;
				var validations = record.ApTravelOrderDepartmentValidations.Where(x => x.Status == ApprovalStatuses.Approve).AsQueryable();
				if (validations.Any())
					isNewRecord = false;
				decimal docamount = record.AdvanceAmount;
				int docid = record.ApTravelOrderId;
				int step = record.CurrentApprovalStep != null ? (int)record.CurrentApprovalStep : 0;
				DocApprovalWorkflowHelper.ForwardToApprover(docid, docamount, "", record.ForwardedTo, ApprovalDocumentType.ApTravelOrderSelfCare, isNewRecord,step);
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
			var record = _db.ApTravelOrders.Find(id);
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
				_db.ApTravelOrders.Attach(record);
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