using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Need;
using ExceedERP.Core.Domain.Procurement.Plan;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.NeedAndPlan
{

    public class ProcurementNeedValidationController : Controller
	{
		private ExceedDbContext db = new ExceedDbContext();


		[AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser, ProcurementRoles.ProcurementUserDepartmentApprover, ProcurementRoles.ProcurementPlanApprover, ProcurementRoles.ProcurementPlanUser)]
		public ActionResult Index()
		{
			return View();
		}

		[AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser, ProcurementRoles.ProcurementUserDepartmentApprover, ProcurementRoles.ProcurementPlanApprover, ProcurementRoles.ProcurementPlanUser)]

		public ActionResult AnnualNeedAssesmentValidations_Read(int id, [DataSourceRequest]DataSourceRequest request)
		{
			IQueryable<AnnualNeedAssesmentValidation> annualneedassesmentvalidations = db.AnnualNeedAssesmentValidations.Where(x => x.AnnualNeedAssesmentID == id);
			DataSourceResult result = annualneedassesmentvalidations.ToDataSourceResult(request, annualNeedAssesmentValidation => new
			{
				AnnualNeedAssesmentValidationID = annualNeedAssesmentValidation.AnnualNeedAssesmentValidationID,
				Status = annualNeedAssesmentValidation.Status,
				Remark = annualNeedAssesmentValidation.Remark,

				OrgBranchName = annualNeedAssesmentValidation.OrgBranchName,
				DateCreated = annualNeedAssesmentValidation.DateCreated,
				LastModified = annualNeedAssesmentValidation.LastModified,
				CreatedBy = annualNeedAssesmentValidation.CreatedBy,
				ModifiedBy = annualNeedAssesmentValidation.ModifiedBy
			});

			return Json(result);
		}

		[AuthorizeRoles(ProcurementRoles.ProcurementPlanApprover)]

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult AnnualNeedAssesmentValidations_Create(int id, [DataSourceRequest]DataSourceRequest request, AnnualNeedAssesmentValidation annualNeedAssesmentValidation)
		{

		   
			var record = db.AnnualNeedAssesments.Find(id);

			if (record != null && record.IsOnlineApproved)
			{
				ModelState.AddModelError(String.Empty, "You Can Not Manipulate This Record: It is Already Approved");
			}
			if (ModelState.IsValid)
			{
				if (record != null)
				{
					if (annualNeedAssesmentValidation.Status == ApprovalStatuses.Approve)
					{
						record.IsOnlineApproved = true;
						record.Status = "User Approved";
						record.OnlineApprovedBy = User.Identity.Name;
						record.OnlineApprovedTime = DateTime.Now;
						//transfer to plan
						GeneratingPlanItems(id);
					}

					if (annualNeedAssesmentValidation.Status == ApprovalStatuses.Reject)
					{

						record.Status = Enum.GetName(typeof(OperationStatuses), 8);
						record.OnlineApprovedBy = User.Identity.Name;
						record.OnlineApprovedTime = DateTime.Now;
					}
					db.AnnualNeedAssesments.Attach(record);
					db.Entry(record).State = EntityState.Modified;
				   // db.SaveChanges();

					var entity = new AnnualNeedAssesmentValidation
					{

						Status = annualNeedAssesmentValidation.Status,
						Remark = annualNeedAssesmentValidation.Remark,
						AnnualNeedAssesmentID = id,
						OrgBranchName = annualNeedAssesmentValidation.OrgBranchName,
						DateCreated = DateTime.Now,
						LastModified = annualNeedAssesmentValidation.LastModified,
						CreatedBy = User.Identity.Name,
						ModifiedBy = annualNeedAssesmentValidation.ModifiedBy
					};

					db.AnnualNeedAssesmentValidations.Add(entity);
					db.SaveChanges();
					annualNeedAssesmentValidation.AnnualNeedAssesmentValidationID =
						entity.AnnualNeedAssesmentValidationID;

					#region operation log
					UserHelper.OperationLog(Request.UserHostAddress, "procurement need approval", "Success", User.Identity.Name, $"approved need {entity.AnnualNeedAssesmentID} ", Modules.Procurement);
					#endregion
				}

			}

			return Json(new[] { annualNeedAssesmentValidation }.ToDataSourceResult(request, ModelState));
		}

		[AuthorizeRoles(ProcurementRoles.ProcurementPlanApprover)]

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult AnnualNeedAssesmentValidations_Update([DataSourceRequest]DataSourceRequest request, AnnualNeedAssesmentValidation annualNeedAssesmentValidation)
		{
			if (ModelState.IsValid)
			{
				var entity = new AnnualNeedAssesmentValidation
				{
					AnnualNeedAssesmentValidationID = annualNeedAssesmentValidation.AnnualNeedAssesmentValidationID,
					Status = annualNeedAssesmentValidation.Status,
					Remark = annualNeedAssesmentValidation.Remark,
					OrgBranchName = annualNeedAssesmentValidation.OrgBranchName,
					DateCreated = annualNeedAssesmentValidation.DateCreated,
					LastModified = annualNeedAssesmentValidation.LastModified,
					CreatedBy = annualNeedAssesmentValidation.CreatedBy,
					ModifiedBy = annualNeedAssesmentValidation.ModifiedBy
				};

				db.AnnualNeedAssesmentValidations.Attach(entity);
				db.Entry(entity).State = EntityState.Modified;
				db.SaveChanges();


			}

			return Json(new[] { annualNeedAssesmentValidation }.ToDataSourceResult(request, ModelState));
		}

		[AuthorizeRoles(ProcurementRoles.ProcurementPlanApprover)]

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult AnnualNeedAssesmentValidations_Destroy([DataSourceRequest]DataSourceRequest request, AnnualNeedAssesmentValidation annualNeedAssesmentValidation)
		{
			if (ModelState.IsValid)
			{
				var entity = new AnnualNeedAssesmentValidation
				{
					AnnualNeedAssesmentValidationID = annualNeedAssesmentValidation.AnnualNeedAssesmentValidationID,
					Status = annualNeedAssesmentValidation.Status,
					Remark = annualNeedAssesmentValidation.Remark,
					OrgBranchName = annualNeedAssesmentValidation.OrgBranchName,
					DateCreated = annualNeedAssesmentValidation.DateCreated,
					LastModified = annualNeedAssesmentValidation.LastModified,
					CreatedBy = annualNeedAssesmentValidation.CreatedBy,
					ModifiedBy = annualNeedAssesmentValidation.ModifiedBy
				};

				db.AnnualNeedAssesmentValidations.Attach(entity);
				db.AnnualNeedAssesmentValidations.Remove(entity);
				db.SaveChanges();
			}

			return Json(new[] { annualNeedAssesmentValidation }.ToDataSourceResult(request, ModelState));
		}

		private void GeneratingPlanItems(int id)
		{
			var need = db.AnnualNeedAssesments.Find(id);

			if (need != null)
			{
				var plan = db.ProcurementPlans
					.Where(x => x.GlFiscalYearId == need.GlFiscalYearId)
					.Where(x => x.BranchId == need.BranchId)
					.Where(x => x.BusinessUnitId == need.BusinessUnitId)
					.FirstOrDefault();

				#region if existing

				if (plan != null)
				{
					foreach (var item in need.AnnualNeedAssesmentItem)
					{
						var existingPlanItem = db.ProcurementPlanItems
							.Where(x => x.BudgetSource == item.BudgetSource)
							.Where(x => x.CatCode == item.CatCode)
							.Where(x => x.ItmCode == item.ItmCode)
							.Where(x => x.ProcurementPlanId == plan.ProcurementPlanId)
							.FirstOrDefault();
						
						if (existingPlanItem!=null)
						{
							existingPlanItem.Quantity += item.Quantity;
							db.ProcurementPlanItems.Attach(existingPlanItem);
							db.Entry(existingPlanItem).State = EntityState.Modified;
							db.SaveChanges();
						}
						else
						{
							ProcurementPlanItem planItem = new ProcurementPlanItem
							{
								CatCode = item.CatCode,
								CategoryName = item.CategoryName,
								ItmCode = item.ItmCode,
								ItemName = item.ItemName,
								Name = item.Name,
								ModelNo=item.ModelNo,
								PartNo = item.PartNo,
								Quantity = item.Quantity,
								ItemSpecification = item.ItemSpecification,
								UoM = item.UoM,
								ItemType = item.ItemType,
								Type = item.Type,
								UnitPrice = item.Price,
								Period1 = item.Period1,
								Period2 = item.Period2,
								Period3 = item.Period3,
								Period4 = item.Period4,
								Period5 = item.Period5,
								Period6 = item.Period6,
								Period7 = item.Period7,
								Period8 = item.Period8,
								Period9 = item.Period9,
								Period10 = item.Period10,
								Period11 = item.Period11,
								Period12 = item.Period12,
								BudgetSource = item.BudgetSource,
								ProcurementCategory = item.ProcurementCategory,
								ProcurementPlanId = plan.ProcurementPlanId,
								TenderFloatingTime = DateTime.Today,
								PurchaseDeliveryTime = DateTime.Today
							};
							db.ProcurementPlanItems.Add(planItem);
							db.SaveChanges();
						}

					}

				}

				#endregion

				#region if not existing

				else
				{
					#region create plan

					var inventoryPlan = new ProcurementPlan
					{
						BranchId = need.BranchId,
						BusinessUnitId = need.BusinessUnitId,
						GlFiscalYearId = need.GlFiscalYearId,
						Remark = need.Remark,
						Status = need.Status,
					   
						OrgBranchName = need.BranchId.ToString(),
						DateCreated = DateTime.Now,
						LastModified = need.LastModified,
						CreatedBy = User.Identity.Name,
						ModifiedBy = need.ModifiedBy
					};

					db.ProcurementPlans.Add(inventoryPlan);
					db.SaveChanges();

					#endregion

					foreach (var item in need.AnnualNeedAssesmentItem)
					{
						ProcurementPlanItem planItem = new ProcurementPlanItem
						{
							CatCode = item.CatCode,
							CategoryName = item.CategoryName,
							ItmCode = item.ItmCode,
							ItemName = item.ItemName,
							PartNo = item.PartNo,
							Quantity = item.Quantity,
							ItemSpecification = item.ItemSpecification,
							UoM = item.UoM,
							ItemType = item.ItemType,
							Type = item.Type,
							UnitPrice = 0,
							Period1 = item.Period1,
							Period2 = item.Period2,
							Period3 = item.Period3,
							Period4 = item.Period4,
							Period5 = item.Period5,
							Period6 = item.Period6,
							Period7 = item.Period7,
							Period8 = item.Period8,
							Period9 = item.Period9,
							Period10 = item.Period10,
							Period11 = item.Period11,
							Period12 = item.Period12,
							BudgetSource = item.BudgetSource,
							ProcurementCategory = item.ProcurementCategory,
							ProcurementPlanId = inventoryPlan.ProcurementPlanId,
							TenderFloatingTime = DateTime.Today,
							PurchaseDeliveryTime = DateTime.Today
						};
						db.ProcurementPlanItems.Add(planItem);
						db.SaveChanges();

					}
				}

				#endregion

				need.IsTransfferedToPlan = true;
				db.AnnualNeedAssesments.Attach(need);
				db.Entry(need).State = EntityState.Modified;
				db.SaveChanges();
			}
		}

		protected override void Dispose(bool disposing)
		{
			db.Dispose();
			base.Dispose(disposing);
		}
	}
}
