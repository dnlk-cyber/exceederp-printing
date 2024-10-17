using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FixedAsset.Depreciation;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.FixedAsset;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Depreciations
{


    public class AnnualDepreciationController : Controller
	{
		readonly ExceedDbContext _db = new ExceedDbContext();
		private readonly FixedAssetDistributionHelper _distributionHelper = new FixedAssetDistributionHelper();


		[AuthorizeRoles(FixedAssetConstants.FixedAssetDepreciationUser, FixedAssetConstants.FixedAssetDepreciationApprover)]
		public ActionResult Index()
		{

			PopulateViewData();
			return View();

		}

		private void PopulateViewData()
		{
		   
			var branches = _db.Branch;
			var years = _db.GLFiscalYears;
			var periods = _db.GLPeriods;
			var assetRegistrations = _db.AssetRegistrations;
			var groups = _db.FixedAssetGroups;

			ViewData["Years"] = years;
			ViewData["Periods"] = periods;
			ViewData["Branches"] = branches;
			ViewData["Groups"] = groups;
			ViewData["Assets"] = assetRegistrations;

		}

		[AuthorizeRoles(FixedAssetConstants.FixedAssetDepreciationUser, FixedAssetConstants.FixedAssetDepreciationApprover)]
		public ActionResult AnnualDepreciations_Read([DataSourceRequest]DataSourceRequest request)
		{
			IQueryable<AnnualDepreciation> depreciations = _db.AnnualDepreciations;

			DataSourceResult result = depreciations.ToDataSourceResult(request, depreciation => new
			{
				AnnualDepreciationId = depreciation.AnnualDepreciationId,
				GlFiscalYearId = depreciation.GlFiscalYearId,
				BranchId = depreciation.BranchId,
				BusinessUnitId = depreciation.BusinessUnitId,
				FixedAssetGroupID = depreciation.FixedAssetGroupID,
				Status = depreciation.Status,
				Remark = depreciation.Remark,
				IsVoid = depreciation.IsVoid,
				VoidBy = depreciation.VoidBy,
				VoidTime = depreciation.VoidTime,
				IsOnlineApproved = depreciation.IsOnlineApproved,
				OnlineApprovedBy = depreciation.OnlineApprovedBy,
				OnlineApprovedTime = depreciation.OnlineApprovedTime,
				IsSendForApproval = depreciation.IsSendForApproval,
				SendForApprovalBy = depreciation.SendForApprovalBy,
				SendForApprovalTime = depreciation.SendForApprovalTime,
				IsOnlineTransferred = depreciation.IsOnlineTransferred,
				IsOnlineTransferredBy = depreciation.IsOnlineTransferredBy,
				OnlineTransferredTime = depreciation.OnlineTransferredTime,

				DateCreated = depreciation.DateCreated,
				CreatedBy = depreciation.CreatedBy,
				LastModified = depreciation.LastModified,
				ModifiedBy = depreciation.ModifiedBy,
			});

			return Json(result);
		}

		[AuthorizeRoles(FixedAssetConstants.FixedAssetDepreciationUser, FixedAssetConstants.FixedAssetDepreciationApprover)]
		public ActionResult AnnualDepreciationItems_Read([DataSourceRequest]DataSourceRequest request,int id)
		{
			IQueryable<AnnualDepreciationItem> annualDepreciationItems = _db.AnnualDepreciationItems.Where(x=>x.AnnualDepreciationId==id);

			DataSourceResult result = annualDepreciationItems.ToDataSourceResult(request, annualDepreciationItem => new
			{
				AnnualDepreciationId = annualDepreciationItem.AnnualDepreciationId,
				AssetRegistrationId = annualDepreciationItem.AssetRegistrationId,
				DepreciationAmount = annualDepreciationItem.DepreciationAmount,
				BookValue = annualDepreciationItem.BookValue,
				
			});

			return Json(result);
		}

		//[AuthorizeRoles(FixedAssetConstants.FixedAssetDepreciationUser)]
		//public ActionResult ReverseDepreciation(int year, int period, string store)
		//{

		//    var registrations = _db.AssetRegistrations.Where(x => x.StoreCode == store).ToList();

		//    foreach (var registration in registrations)
		//    {
		//        string stPeriod = period.ToString();
		//        var reverseValue =
		//            _db.DeprciationReverses.FirstOrDefault(
		//                x => x.RegistrationID == registration.AssetRegistrationId && x.Period == stPeriod);
		//        var depreciations = _db.annual.FirstOrDefault(x => x.AssetRegistrationId == registration.AssetRegistrationId && x.GlFiscalYearId == period);
		//        decimal prevValue = depreciations.BookValue - reverseValue.BookValue;
		//        depreciations.BookValue = reverseValue.BookValue;
		//       // depreciations.Accomodate -= prevValue;

		//    }
		//    var depreciationState =
		//       _db.DepreciationPeriods.FirstOrDefault(x => x.StoreCode == store && x.Period == period);
		//    depreciationState.Status = DepreciationStatus.Reversed;
		//    _db.Entry(depreciationState).State = EntityState.Modified;
		//    _db.SaveChanges();
		//    return RedirectToAction("Index");
		//}

		//[AuthorizeRoles(FixedAssetConstants.FixedAssetDepreciationApprover)]
		//public ActionResult CloseDepreciation(int year, int period, string store)
		//{
		//    //var depreciationState =_db.DepreciationPeriods.FirstOrDefault(x => x.StoreCode == store && x.Period == period);
		//    //depreciationState.Status = DepreciationStatus.closed;
		//    //_db.Entry(depreciationState).State = EntityState.Modified;
		//    //_db.SaveChanges();
		//    //return RedirectToAction("Index");
		//}

		[AuthorizeRoles(FixedAssetConstants.FixedAssetDepreciationUser)]
		public JsonResult RunDepreciation(string fiscalyearid, string branchid, string groupid)
		{
			List<string> errors = new List<string>();

			if (!String.IsNullOrEmpty(fiscalyearid) && !String.IsNullOrEmpty(branchid))
			{
				decimal minimumAssetValue = FixedAssetConfigurationHelper.GetAssetMinimumLevel();
				int yearidd = int.Parse(fiscalyearid);
				int branchidd = int.Parse(branchid);

				var branch = _db.Branch.Find(branchidd);
				var glyear = _db.GLFiscalYears.Find(yearidd);

				int fixedgroupid = 0;
				if (!String.IsNullOrEmpty(groupid))
					fixedgroupid = int.Parse(groupid);


				if (glyear != null && branch != null)
				{
					DateTime dateTo = glyear.DateTo.Date;
					DateTime dateFrom = glyear.DateFrom.Date;
					var rules = _db.DepretiationRules.ToList(); //for sistribution
					var groups = _db.FixedAssetGroups.AsNoTracking().ToList();
				   

					if (fixedgroupid != 0)
						groups = groups.Where(x => x.FixedAssetGroupID == fixedgroupid).ToList();

					var activeAssets = _db.AssetRegistrations
						.Where(x => x.Status == RegistrationStatus.Active || x.Status == RegistrationStatus.Revalued)        
						.Where(x=>x.PurchasePrice>=minimumAssetValue)
						.AsQueryable();

					if (fixedgroupid != 0)
						activeAssets = activeAssets.Where(x => x.FixedAssetCategory.FixedAssetGroupID == fixedgroupid).AsQueryable();

					activeAssets = activeAssets
							.Where(x => x.BranchId == branchidd)
							.Where(x => x.GRNDate <= dateTo).AsQueryable();

					var annualDepreciation = _db.AnnualDepreciations
						.Where(x => x.GlFiscalYearId == glyear.GlFiscalYearId)
						.Where(x => x.BranchId == branch.BranchId).FirstOrDefault();

					if (annualDepreciation == null)
					{
						annualDepreciation = new AnnualDepreciation
						{
							GlFiscalYearId = yearidd,
							BranchId = branchidd,
							FixedAssetGroupID=fixedgroupid,
							DateCreated = DateTime.Now,
							CreatedBy = User.Identity.Name,
							Status="Generated"
						};
						_db.AnnualDepreciations.Add(annualDepreciation);
						_db.SaveChanges();
					}
					List<AnnualDepreciationItem> depreList = new List<AnnualDepreciationItem>();
					var activeList = activeAssets.ToList();
					foreach (var asset in activeList)
					{
						#region skip if deprecation is done for this item and year

						var exist = _db.AnnualDepreciationItems
							.Where(x => x.AnnualDepreciation.GlFiscalYearId == glyear.GlFiscalYearId)
							.Where(x => x.AssetRegistrationId == asset.AssetRegistrationId)
							.ToList();
						if (exist.Any())
							continue;

						#endregion

						var assetGroup = groups.FirstOrDefault(x =>
							x.FixedAssetGroupID == asset.FixedAssetCategory.FixedAssetGroupID);
						if (assetGroup != null && (asset.GRNDate != null))
						{
							#region straight line based depreciation

							var depreciationPercentage = assetGroup.StraightLinePercentage;
							double days = 0;
							days = dateFrom > (DateTime) asset.GRNDate
								? (dateTo - dateFrom).TotalDays
								: (dateTo - (DateTime) asset.GRNDate).TotalDays;
							decimal rate = (decimal) depreciationPercentage;
							decimal depreciated = (rate * asset.Quanity * asset.PurchasePrice * (decimal) days) / (365 * 100);


							_db.Entry(asset).State = EntityState.Modified;
							_db.SaveChanges();
							//register the for item
							var depre = new AnnualDepreciationItem
							{
								AnnualDepreciationId = annualDepreciation.AnnualDepreciationId,
								AssetRegistrationId = asset.AssetRegistrationId,
								DepreciationAmount = depreciated,
								BookValue = asset.BookValue,
								DateCreated = DateTime.Now,
								CreatedBy = User.Identity.Name,
							};

							depreList.Add(depre);                           
							//accounting

							#endregion
						}                      

						else
						{
							string error = "Depreciation rule not found for asset " +
										   asset.FixedAssetCategory.CategoryName;
							errors.Add(error);

						}

					}

					//save depreciation list
					_db.AnnualDepreciationItems.AddRange(depreList);
					_db.SaveChanges();

					#region distribution

					_distributionHelper.UpdateAnnualDepreciationDistribution(annualDepreciation
						.AnnualDepreciationId);

					#endregion
				}
				else
				{
					var formattedData = new
					{
						Message = "Please select period and branch",
						Status = "NOT_OK"
					};
					return Json(formattedData, JsonRequestBehavior.AllowGet);
				}

			}
			else
			{
				var formattedData = new
				{
					Message = "Please select period and branch",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			if (errors.Any())
			{
				var formattedData = new
				{
					Message = errors,
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			var formattedData1 = new
			{
				Message = "Depreciation complete.",
				Status = "OK"
			};
			return Json(formattedData1, JsonRequestBehavior.AllowGet);

		}

		[AuthorizeRoles(FixedAssetConstants.FixedAssetDepreciationUser)]
		public JsonResult SendForApproval(int id)
		{
			var record = _db.AnnualDepreciations.Find(id);
			#region check segment length validation

			var distributions = _db.AnnualDepreciationDistributions.Where(x => x.AnnualDepreciationId == id).ToList();
			AccountHelper accountHelper = new AccountHelper();
			foreach (var dist in distributions)
			{
				string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
				if (!String.IsNullOrEmpty(message))
				{
					var formattedData = new
					{
						Message = message,
						Status = "NOT_OK"
					};
					return Json(formattedData, JsonRequestBehavior.AllowGet);
				}

			}


			#endregion
		   

			if (record == null)
			{
				var formattedData = new
				{
					Message = "The record is not found.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			if (record.AnnualDepreciationItems.Count == 0)
			{
				var formattedData = new
				{
					Message = "There are no depreciation items.",
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

			if (!record.IsSendForApproval && !record.IsOnlineApproved && record.AnnualDepreciationItems.Count > 0)
			{
				record.Status = Enum.GetName(typeof(OperationStatuses), 4);
				record.IsSendForApproval = true;
				record.SendForApprovalBy = User.Identity.Name;
				record.SendForApprovalTime = DateTime.Now;
				_db.AnnualDepreciations.Attach(record);
				_db.Entry(record).State = EntityState.Modified;
				_db.SaveChanges();
				var formattedData = new
				{
					Message = "Successfully Sent.",
					Status = "OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}
			return null;
		}

		[AuthorizeRoles(FinanceConstants.AccountPayableInvoice)]
		public JsonResult TransferDistribution(string id, string description)
		{
			int myid = int.Parse(id);
			var record = _db.AnnualDepreciations.Find(myid);
			if (record != null && !record.IsOnlineApproved)
			{
				var formattedData = new
				{
					Message = "The record is not approved.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			if (record != null && !record.IsOnlineTransferred)
			{
				#region check if already transffered 
				var exist = _db.GLRecordJournals
				   .Where(x => x.Source == JournalSources.PPE_Annual_Distribution.ToString())
				   .Where(x => x.SourceId == record.AnnualDepreciationId.ToString())
				   .FirstOrDefault();
				if (exist != null)
				{
					var formattedData = new
					{
						Message = "The record is already transffered, it can't be retransferred.",
						Status = "NOT_OK"
					};
					return Json(formattedData, JsonRequestBehavior.AllowGet);
				}
				#endregion
				#region check segment length validation
				AccountHelper accountHelper = new AccountHelper();
				foreach (var dist in record.AnnualDepreciationDistributions)
				{
					string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
					if (!String.IsNullOrEmpty(message))
					{
						var formattedData = new
						{
							Message = message,
							Status = "NOT_OK"
						};
						return Json(formattedData, JsonRequestBehavior.AllowGet);
					}

				}


				#endregion
				#region check if debit and credit are equal
				decimal debit = 0;
				decimal credit = 0;
				var distribution = record.AnnualDepreciationDistributions.ToList();
				if (distribution.Any())
				{
					debit = distribution.Sum(x => x.Debit);
					credit = distribution.Sum(x => x.Credit);
				}
				if (debit != credit)
				{
					var formattedData = new
					{
						Message = "Total debit and total credit is not balanced.",
						Status = "NOT_OK"
					};
					return Json(formattedData, JsonRequestBehavior.AllowGet);
				}
				if (debit == 0)
				{
					var formattedData = new
					{
						Message = "Total debit and total credit is zero.",
						Status = "NOT_OK"
					};
					return Json(formattedData, JsonRequestBehavior.AllowGet);
				}
				#endregion

				#region send to GL
			   
				string glyear = "";
				string glperiod = "";
				#region get period

				FiscalYearHelper yearHelper = new FiscalYearHelper();
				int lastPeriodId = yearHelper.GetFiscalYearLastPeriod(record.GlFiscalYearId);
				var period = _db.GLPeriods.Find(lastPeriodId);
				if (period!=null)
				{
					var myPeriod = period;
					glperiod = myPeriod.Name;
					glyear = myPeriod.GlFiscalYear.Name.ToString();
					if (myPeriod.Status == FiscalYearStatus.Close)
					{
						var formattedData = new
						{
							Message = "The period " + glperiod +
									  " is closed,the record cannot be transfered to closed period.",
							Status = "NOT_OK"
						};
						return Json(formattedData, JsonRequestBehavior.AllowGet);
					}

				}
				else
				{
					var formattedData = new
					{
						Message = "Period not found.",
						Status = "NOT_OK"
					};
					return Json(formattedData, JsonRequestBehavior.AllowGet);
				}

				#endregion

				if (record.AnnualDepreciationDistributions.Any())
				{
					GlRecordJournal journal = new GlRecordJournal
					{
						DateCreated = DateTime.Now,
						CreatedBy = User.Identity.Name,
						JournalName = "PPE Annual Distribution",
						Description = !String.IsNullOrEmpty(description) ? description : "PPE Annual Distribution",
						FiscalYear = glyear,
						PeriodName = glperiod,
						CategoryName = "PPE Annual Distribution",
						SourcesName = "PPE",
						JournalNumber = "PPEMD." + record.AnnualDepreciationId.ToString(),
						SourceId = record.AnnualDepreciationId.ToString(),
						Source = JournalSources.PPE_Annual_Distribution.ToString(),
						EffectiveDate = period.DateTo,
						OrgBranchName = record.OrgBranchName,
						Status = "Approved",
						Remark = record.Remark,
						PaymentReference = "",
						ReferenceDoc = "",
						InvoiceReferenceDoc = "",
						IsOnlineApproved = true,
						OnlineApprovedBy = User.Identity.Name,
						OnlineApprovedTime = DateTime.Now
					};
					_db.GLRecordJournals.Add(journal);
					_db.SaveChanges();
					foreach (var i in record.AnnualDepreciationDistributions)
					{
						#region debit and credit

						var journalDetail = new GLRecordJournalDetail
						{
							DateCreated = DateTime.Now,
							CreatedBy = User.Identity.Name,
							GLRecordJournal = journal,
							AccountCode = i.AccountCode,
							Debit = i.Debit,
							Credit = i.Credit,
							AdditionalDescription = i.AdditionalDescription,
						};
						_db.GLRecordJournalDetails.Add(journalDetail);
						_db.SaveChanges();
						#endregion
					}


					#region update record

					record.IsOnlineTransferred = true;
					_db.Entry(record).State = EntityState.Modified;
					_db.SaveChanges();

					#endregion

					#region split journal
					var journaldetails = _db.GLRecordJournalDetails
						.Where(x => x.GlRecordJournalId == journal.GlRecordJournalId).ToList();
					foreach (var detail in journaldetails)
					{
						AccountHelper helper = new AccountHelper();
						helper.SplitJournal(detail.GlRecordJournalDetailId);
					}
					#endregion

					var formattedData = new
					{
						Message = "Successfully transfered.",
						Status = "OK"
					};
					return Json(formattedData, JsonRequestBehavior.AllowGet);

				}
				#endregion

			}
			else
			{
				var formattedData = new
				{
					Message = "You cannot transfer this transaction, it is already transfered.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			return null;
		}

		public static int DiffMonths(DateTime startDate, DateTime endDate)
		{
			return (startDate.Year * 12 + startDate.Month + startDate.Day / System.DateTime.DaysInMonth(startDate.Year, startDate.Month))
				   - (endDate.Year * 12 + endDate.Month + endDate.Day / System.DateTime.DaysInMonth(endDate.Year, endDate.Month));
		}

		protected override void Dispose(bool disposing)
		{
			_db.Dispose();
			base.Dispose(disposing);
		}
	}
}