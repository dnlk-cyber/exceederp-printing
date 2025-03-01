using System;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Construction;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Web.Areas.Maintenance.Models;
using System.Data.Entity;
using static ExceedERP.Web.Areas.Maintenance.Controllers.MaintenanceDashboardController;

namespace ExceedERP.Web.Controllers
{

	public partial class DashboardController : Controller
	{
		private readonly ExceedDbContext _db = new ExceedDbContext();
		private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();
		private readonly DashboardHelper _dashboardHelper = new DashboardHelper();

		public ActionResult RealEstate()
		{
			return View();
		}

		[AuthorizeRoles(CommonConstants.DashboardHr)]
		public ActionResult ImportExportDashboard()
		{
			return View();
		}


		 [AuthorizeRoles(CommonConstants.DashboardInventory)]
		public ActionResult MaintenanceDashboard()
		{


			return View();
		}


		[AuthorizeRoles(CommonConstants.DashboardFixedAsset)]
		public ActionResult ProjectManagementDashboard()
		{
			return View();
		}
		public ActionResult ScoreCardDashboard()
		{
			return View();
		}


		#region Maintenance Dashboard
		public IList<string> getMaintenanceLabels()
		{
			var labelArray = new List<string>();
			labelArray.Add("Pending");
			labelArray.Add("Opened");
			labelArray.Add("WaitingApproval");
			labelArray.Add("Closed");
			labelArray.Add("Delivered");
			labelArray.Add("Rejected");

			return labelArray;
		}
		public JsonResult GetMaintenanceCount(DateTime? startDate, DateTime? endDate)
		{
			var datas = getJobOrdersCountByStatus(startDate, endDate, true);
			var labels = getMaintenanceLabels();

			var count = new Dictionary<string, int>();
			int i = 1;
			var labelDatas = labels.Zip(datas, (n, w) => new { Label = n, Data = w });

			foreach (var labelData in labelDatas)
			{
				count.Add(labelData.Label, labelData.Data);
				i++;
			}
			return Json(count, JsonRequestBehavior.AllowGet);
		}
		public JsonResult getBarChartMaintenanceOrderDatas(DateTime? startDate, DateTime? endDate)

		{

			var orderLabelList = getMaintenanceLabels();
			var orderDataList = getJobOrdersCountByStatus(startDate, endDate, false);
			var orderStatuses = getMaintenanceCentreJobOrderStatuses(startDate, endDate);
			var OrderVsDates = new Mapper();
			var OrderVsDateServices = new Mapper();
			var orderserviceDataList = getOrdersMaintenanceJoServicesCount(startDate, endDate);
			OrderVsDates.labels = orderLabelList;
			OrderVsDates.datas = orderDataList;
			OrderVsDateServices.labels = orderLabelList;
			OrderVsDateServices.datas = orderserviceDataList;
			var result = new Dictionary<string, Object>();
			result.Add("OrderVsDate", OrderVsDates);
			result.Add("OrderSatuseVsServices", OrderVsDateServices);
			result.Add("OrderStatuses", orderStatuses);
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		public Dictionary<string, object> getMaintenanceCentreJobOrderStatuses(DateTime? startDate, DateTime? endDate)
		{
			var maintenanceCenters = _db.MaintenanceCenter.Where(q => _db.JobOrder.Select(s => s.MaintenanceCenter).Distinct().Contains(q.Name)).Select(s => s.Name);
			var statuses = new List<string>();
			statuses.Add(MaintenanceConst.Open);
			statuses.Add(MaintenanceConst.Closed);
			statuses.Add(MaintenanceConst.Delivered);
			statuses.Add(MaintenanceConst.Rejected);

			var labDatas = new Dictionary<string, object>();
			var labels = new List<string>();
			var datas = new List<List<int>>();
			var jobOrders = getJobOrdersBnDate(startDate, endDate, false).ToList();
			foreach (var status in statuses)
			{
				var data = new List<int>();
				foreach (var mainCenter in maintenanceCenters)
				{


					var sts = jobOrders.Where(q => q.Status == status && q.MaintenanceCenter == mainCenter)
						.ToList().Count();
					data.Add(sts);
				}
				datas.Add(data);

			}
			labDatas.Add("stackedBarLabels", maintenanceCenters);
			labDatas.Add("stackedBarDataSets", statuses);
			labDatas.Add("stackedBarDatas", datas);




			return labDatas;


		}


		public IList<int> getOrdersMaintenanceJoServicesCount(DateTime? startDate, DateTime? endDate)
		{
			var serviceQty = new List<int>();
			var jobOrders = getJobOrdersByDate(startDate, endDate, false);
			var data1 = jobOrders.ElementAt(0);
			var data2 = jobOrders.ElementAt(1);
			var data3 = jobOrders.ElementAt(1);
			var data4 = jobOrders.ElementAt(3);
			var data5 = jobOrders.ElementAt(4);
			var data6 = jobOrders.ElementAt(5);
			serviceQty.Add(_db.JobOrderDetail.Where(q => data1.Contains(q.JobOrderID)).ToList().Count());
			serviceQty.Add(_db.JobOrderDetail.Where(q => data2.Contains(q.JobOrderID)).ToList().Count());
			serviceQty.Add(_db.JobOrderDetail.Where(q => data3.Contains(q.JobOrderID)).ToList().Count());
			serviceQty.Add(_db.JobOrderDetail.Where(q => data4.Contains(q.JobOrderID)).ToList().Count());
			serviceQty.Add(_db.JobOrderDetail.Where(q => data5.Contains(q.JobOrderID)).ToList().Count());
			serviceQty.Add(_db.JobOrderDetail.Where(q => data6.Contains(q.JobOrderID)).ToList().Count());
			return serviceQty;
		}
		public JsonResult GetInExMainteenanceJobOrders(DateTime? startDate, DateTime? endDate)
		{
			if (startDate == null)
			{
				startDate = DateTime.Now.AddDays(-60);
			}

			if (endDate == null)
			{
				endDate = DateTime.Now;
			}

			var labels = new List<string>();
			var datas = new List<decimal>();
			var labDatas = new Mapper();
			labels.Add("External");
			labels.Add("Internal");
			var jobRequests = _db.JobOrder.Where(q => q.DateCreated >= startDate && q.DateCreated <= endDate).Select(s => s.Referance);
			datas.Add(_db.JobRequest.Where(q => jobRequests.Contains(q.Num) && q.Source == JobRequestSource.ExternalVehicle).ToList().Count());
			datas.Add(_db.JobRequest.Where(q => jobRequests.Contains(q.Num) && q.Source == JobRequestSource.CompanyVehicle).ToList().Count());
			labDatas.labels = labels;
			labDatas.datas = datas;
			var result = new Dictionary<string, Object>();
			result.Add("MaintenanceVehicles", labDatas);
			result.Add("TotalProducts", labDatas);
			return Json(result, JsonRequestBehavior.AllowGet);
		}


		public IList<int> getJobOrdersCountByStatus(DateTime? startDate, DateTime? endDate, bool status)
		{
			var counts = new List<int>();
			var datas = getJobOrdersByDate(startDate, endDate, status);
			foreach (var data in datas)
			{
				counts.Add(data.Count());
			}
			return counts;
		}
		public IList<IEnumerable<Guid>> getJobOrdersByDate(DateTime? startDate, DateTime? endDate, bool status, bool flag = false)
		{
			if (startDate == null)
			{
				startDate = DateTime.Now.AddDays(-60);
			}

			if (endDate == null)
			{
				endDate = DateTime.Now;
			}
			var orders = new List<IEnumerable<Guid>>();
			var datas = new List<string>();
			var jobOrders = getJobOrdersBnDate(startDate, endDate, status);
			var pending = jobOrders.ToList()
				.Where(q => q.Status == MaintenanceConst.Pending)
				.Select(s => new { s.JobOrderID, s.DateCreated, s.Status, s.MaintenanceCenter });
			var opened = jobOrders.ToList()
				.Where(q => q.Status == MaintenanceConst.Open)
				.Select(s => new { s.JobOrderID, s.DateCreated, s.Status, s.MaintenanceCenter });
			var waitingApproval = jobOrders.ToList()
				.Where(q => q.Status == MaintenanceConst.WaitingApproval)
				.Select(s => new { s.JobOrderID, s.DateCreated, s.Status, s.MaintenanceCenter });
			var closed = jobOrders.ToList()
				.Where(q => q.Status == MaintenanceConst.Closed)
				.Select(s => new { s.JobOrderID, s.DateCreated, s.Status, s.MaintenanceCenter });
			var delivered = jobOrders.ToList()
				.Where(q => q.Status == MaintenanceConst.Delivered)
				.Select(s => new { s.JobOrderID, s.DateCreated, s.Status, s.MaintenanceCenter });
			var rejected = jobOrders.ToList()
				.Where(q => q.Status == MaintenanceConst.Rejected)
				.Select(s => new { s.JobOrderID, s.DateCreated, s.Status, s.MaintenanceCenter });

			//if(flag)
			//{

			//}
			orders.Add(pending.Select(s => s.JobOrderID));
			orders.Add(opened.Select(s => s.JobOrderID));
			orders.Add(waitingApproval.Select(s => s.JobOrderID));
			orders.Add(closed.Select(s => s.JobOrderID));
			orders.Add(delivered.Select(s => s.JobOrderID));
			orders.Add(rejected.Select(s => s.JobOrderID));

			return orders;
		}
		public List<JobOrder> getJobOrdersBnDate(DateTime? startDate, DateTime? endDate, bool status)
		{
			var jobOrders = _db.JobOrder.ToList();
			if (!status)
			{
				if (startDate == null)
				{
					startDate = DateTime.Now.AddDays(-30);
				}

				if (endDate == null)
				{
					endDate = DateTime.Now;
				}
				jobOrders = _db.JobOrder.Where(q => q.DateCreated >= startDate && q.DateCreated <= endDate).ToList();

			}
			return jobOrders;
		}
		#endregion

		


		[AuthorizeRoles(CommonConstants.DashboardHr)]
		public ActionResult HrDashboard()
		{
			return View();
		}
	   

		[AuthorizeRoles(CommonConstants.DashboardInventory)]
		public ActionResult InventoryDashboard()
		{
	 

			return View();
		}


		[AuthorizeRoles(CommonConstants.DashboardFixedAsset)]
		public ActionResult FixedAssetDashboard()
		{
			return View();
		}

		[AuthorizeRoles(CommonConstants.DashboardProcurement)]
		public ActionResult ProcurementDashboard()
		{

			return View();
		}

	   
		public ActionResult ConstructionDashboard()
		{
		   

			return View();
		}

		

		public ActionResult FleetDashboard()
		{
			return View();
		}
		public class ChartData
		{
			public decimal Value { get; set; }
			public string Label { get; set; }
		}

	   

		
		#region inventory 

		public JsonResult GetItemBalances(string store)
		{
			List<ChartData> list = new List<ChartData>();

			var itembalance = _db.InventoryItembalances.Where(x=>x.StoreCode!=null).ToList();

			if (!String.IsNullOrWhiteSpace(store))
				itembalance = itembalance.Where(x => x.StoreCode == store).ToList();

		   
			var dictionaries = itembalance.ToList().DistinctByField(x => x.ItemCode);
			foreach (var dic in dictionaries)
			{
				var chart = new ChartData
				{
					Label = dic.InventoryItem.Name,
					Value = itembalance.Where(x=>x.ItemCode==dic.ItemCode).Sum(x=>x.Balance)
				};

				list.Add(chart);
			}

			

			var formattedData = new
			{
				Data = list.Select(x=>x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetInventoryValue()
		{
			var storeItemAssignments = _db.StoreItemAssignments.ToList();
			decimal value = 0;
			foreach (var assignment in storeItemAssignments)
			{
				value = value + assignment.AvgPrice * assignment.Balance;
			}
			
			var formattedData = new
			{
				Data = new decimal[] {value},
				Labels = new string[] { "Value" }
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetInventoryStatus()
		{
			var stores = _db.InventoryStores.Count();
			var category = _db.ItemCategorys.Count();
			var items = _db.InventoryItems.Count();
			var formattedData = new
			{
				Data = new decimal[] { stores, category, items },
				Labels = new string[] { "Stores", "Categories", "Items" }
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetSrStatus()
		{
			var list = GetchartData("SR_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetIssueStatus()
		{
			var list = GetchartData("ISSUE_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetTransferSendStatus()
		{
			var list = GetchartData("TRANSFERSEND_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetTransferReceiveStatus()
		{
			var list = GetchartData("TRANSFERRECEIVE_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetGrnStatus()
		{
			var list = GetchartData("GRN_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetStoreReturnStatus()
		{
			var list = GetchartData("STORERETURN_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetStockAdjustmentStatus()
		{
			var list = GetchartData("STOCKADJUSTMENT_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region fixed asset

		public JsonResult GetFixedAssetItemCount()
		{
			List<ChartData> list = new List<ChartData>();
			var assetRegistrations = _db.AssetRegistrations.AsQueryable();
			var categories = assetRegistrations.Select(x => x.FixedAssetCategoryID).ToList().Distinct();
			foreach (var cat in categories)
			{
				var data = new ChartData
				{
					Label = _db.FixedAssetCategories.Find(cat)?.CategoryName,
					Value = assetRegistrations.Count(x => x.FixedAssetCategoryID == cat)
				};

				list.Add(data);
			}

			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetFixedAssetValue()
		{
			var assetRegistrations = _db.AssetRegistrations.Where(x=>x.Status==RegistrationStatus.Active).AsQueryable();
			var formattedData = new
			{
				Data = new decimal[] { assetRegistrations.Sum(x=>x.BookValue)/1000000 },
				Labels = new string[] { "Value" }
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetFixedAssetStatus()
		{
			var subcategories = _db.FixedAssetSubCategories.Count();
			var categories = _db.FixedAssetCategories.Count();
			var items = _db.AssetRegistrations.Count(x => x.Status == RegistrationStatus.Active);
			var formattedData = new
			{
				Data = new decimal[] { categories, subcategories, items },
				Labels = new string[] { "Categories", "Sub Categories", "Items" }
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetFixedAssetSrStatus()
		{
			var list = GetchartData("SR_FIXEDASSET_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetFixedAssetIssueStatus()
		{
			var list = GetchartData("ISSUE_FIXEDASSET_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetFixedAssetTransferSendStatus()
		{
			var list = GetchartData("MACHINETRANSFER_FIXEDASSET_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetFixedAssetGrnStatus()
		{
			var list = GetchartData("GRN_FIXEDASSET_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetFixedAssetStoreReturnStatus()
		{
			var list = GetchartData("STORERETURN_FIXEDASSET_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetFixedAssetStockAdjustmentStatus()
		{
			var list = GetchartData("STOCKADJUSTMENT_FIXEDASSET_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		
		public JsonResult AssetByCondition()
		{
			List<ChartData> list = new List<ChartData>();
			var conditions = _db.DashboardDictionaries.Where(x => x.Category == "ASSET_BY_STATUS").ToList();
			
			foreach (var cat in conditions)
			{
				var data = new ChartData
				{
					Label = cat.Name,
					Value = cat.Value
				};

				list.Add(data);
			}

			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}

		//helpers

		#endregion

		#region procurement
		public JsonResult GetPaymentAmountStatus()
		{
			var list = GetchartData("PURCHASEPAYMENTAMT_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetPaymentRequestStatus()
		{
			var list = GetchartData("PURCHASEPAYMENT_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetProcurementStatus()
		{
			var list = GetchartData("PROCUREMENT");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetPrStatus()
		{
			var list = GetchartData("PR_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetPoStatus()
		{
			var list = GetchartData("PO_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetProformaStatus()
		{
			var list = GetchartData("PROFORMA_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetTenderStatus()
		{
			var list = GetchartData("TENDER_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetContractStatus()
		{
			var list = GetchartData("CONTRACT_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetServiceGrntStatus()
		{
			var list = GetchartData("SERVICEGRN_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetPurchasedItems()
		{
			var list = GetPurchasedChartData("PURCHASED_ITEMS");
			var labels = GetPurchasedSectors("PURCHASED_ITEMS");
			var formattedData = new
			{
				Labels = labels,
				DataSet = list.ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}

		//helpers
		private List<AccountWatchChartData> GetPurchasedChartData(string type)
		{
			
			List<AccountWatchChartData> list = new List<AccountWatchChartData>();
			var dictionaries = _db.DashboardDictionaries.Where(x => x.Category == type).ToList();
			var dimenstions = dictionaries.ToList().Select(x => x.Dimesion).Distinct();
			var names = dictionaries.ToList().Select(x => x.Name).Distinct();
			

			foreach (var name in names)
			{
				var chart = new AccountWatchChartData();

				chart.label = name;
				chart.data = new List<decimal>();
				List<AccountWatchListData> dataList = new List<AccountWatchListData>();

				foreach (var dim in dimenstions)
				{

					var value = dictionaries.Where(x => x.Dimesion == dim).FirstOrDefault(x => x.Name == name);
					if (value != null)
						chart.data.Add(Math.Round((value.Value / nth), 2));
				}

				list.Add(chart);
			}

			return list;
		}

		private string[] GetPurchasedSectors(string type)
		{
			
			var dictionaries = _db.DashboardDictionaries.Where(x => x.Category == type).ToList();
			string[] sectors = dictionaries.Select(x => x.Dimesion).Distinct().ToArray();

			return sectors;
		}
		#endregion

		#region hrm
		public JsonResult GetEmployeeCount()
		{
			var contrat = _db.Person_Employee.Where(x => x.IsActive && x.Status == EmployeeStatus.CONTRACT).ToList();
			var permanent = _db.Person_Employee.Where(x => x.IsActive && x.Status == EmployeeStatus.PERMANENT).ToList();
			var probation = _db.Person_Employee.Where(x => x.IsActive && x.Status == EmployeeStatus.PROBATION).ToList();
			var others = _db.Person_Employee.Where(x => x.IsActive  /*(x.Status == EmployeeStatus.FREELANCER || x.Status == EmployeeStatus.NO_STATUS*/ && (x.Status != EmployeeStatus.CONTRACT
																																					  && x.Status != EmployeeStatus.PERMANENT
																																					  && x.Status != EmployeeStatus.PROBATION)).ToList();
			var formattedData = new
			{
				Data = new int[] { contrat.Count(), permanent.Count(), probation.Count(), others.Count() },
				Labels = new string[] { "Contract", "Permanent", "Probation", "Others" }
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetContratEmployee()
		{
			var MaleContratemployees = _db.Person_Employee
				.Where(x => x.IsActive && x.Gender == Gender.MALE && x.Status == EmployeeStatus.CONTRACT).ToList();
			var femaleContEmployees = _db.Person_Employee
				.Where(x => x.IsActive && x.Gender == Gender.FEMALE && x.Status == EmployeeStatus.CONTRACT).ToList();
			var others = _db.Person_Employee
			   .Where(x => x.IsActive && x.Gender != Gender.FEMALE
										   && x.Gender != Gender.MALE
										   && x.Status == EmployeeStatus.CONTRACT).ToList();

			var formattedData = new
			{
				Data = new int[]
				{

					MaleContratemployees.Count(), femaleContEmployees.Count(),others.Count(),

					MaleContratemployees.Count()+femaleContEmployees.Count()+others.Count()

				},
				Labels = new string[] { "Male", "Female", "Others", "Total" }
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetPermanentEmployee()
		{
			var MalePermaemployees = _db.Person_Employee
				.Where(x => x.IsActive && x.Gender == Gender.MALE && x.Status == EmployeeStatus.PERMANENT).ToList();
			var femaleEmployees = _db.Person_Employee
				.Where(x => x.IsActive && x.Gender == Gender.FEMALE && x.Status == EmployeeStatus.PERMANENT).ToList();
			var others = _db.Person_Employee
			   .Where(x => x.IsActive
								   && x.Gender != Gender.FEMALE
								   && x.Gender != Gender.MALE
									   && x.Status == EmployeeStatus.PERMANENT).ToList();


			var formattedData = new
			{
				Data = new int[]
				{

					MalePermaemployees.Count(), femaleEmployees.Count(),others.Count(),

					MalePermaemployees.Count()+femaleEmployees.Count()+others.Count()

				},
				Labels = new string[] { "Male", "Female", "Others", "Total" }
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetProbationEmployee()
		{
			var MaleProbationemployees = _db.Person_Employee
				.Where(x => x.IsActive && x.Gender == Gender.MALE && x.Status == EmployeeStatus.PROBATION).ToList();
			var femaleProbationEmployees = _db.Person_Employee
				.Where(x => x.IsActive && x.Gender == Gender.FEMALE
				&& x.Status == EmployeeStatus.PROBATION).ToList();
			var others = _db.Person_Employee.Where(x => x.IsActive
								&& x.Gender != Gender.MALE
								&& x.Gender != Gender.FEMALE
								&& x.Status == EmployeeStatus.PROBATION).ToList();

			var formattedData = new
			{
				Data = new int[]
				{

					MaleProbationemployees.Count(), femaleProbationEmployees.Count(),others.Count(),

					MaleProbationemployees.Count()+femaleProbationEmployees.Count()+others.Count()

				},
				Labels = new string[] { "Male ", "Female", "Others", "Total" }
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetOtherEmployee()
		{
			var MaleOtheremployees = _db.Person_Employee
				.Where(x => x.IsActive && x.Gender == Gender.MALE && x.Status != EmployeeStatus.CONTRACT
																  && x.Status != EmployeeStatus.PERMANENT
																  && x.Status != EmployeeStatus.PROBATION).ToList();

			var femaleOtherEmployees = _db.Person_Employee
				.Where(x => x.IsActive && x.Gender == Gender.FEMALE && x.Status != EmployeeStatus.CONTRACT
																	&& x.Status != EmployeeStatus.PERMANENT
																	&& x.Status != EmployeeStatus.PROBATION).ToList();


			var otherEmployees = _db.Person_Employee
				.Where(x => x.IsActive && x.Gender != Gender.MALE
										&& x.Gender != Gender.FEMALE
										&& x.Status != EmployeeStatus.CONTRACT
										&& x.Status != EmployeeStatus.PERMANENT
										&& x.Status != EmployeeStatus.PROBATION).ToList();




			var formattedData = new
			{
				Data = new int[]
				{

					MaleOtheremployees.Count(), femaleOtherEmployees.Count(),otherEmployees.Count(),

					MaleOtheremployees.Count()+femaleOtherEmployees.Count()+otherEmployees.Count()

				},
				Labels = new string[] { "Male ", "Female", "Others", "Total" }
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}

		[AcceptVerbs(HttpVerbs.Get)]
		public ActionResult GetEmployeeType()
		{
			var formattedData = new
			{
				Data = new List<List<int>>(),
				Labels = new List<string>()
			};
			var employeeTypeIdLists = _db.Person_Employee.Where(x => x.IsActive).Select(x => x.EmployeeTypeId).Distinct();
			var employeeTypeIds = _db.Person_Employee.Where(x => x.IsActive).Select(x => x.EmployeeTypeId).Distinct().ToArray();
			foreach (var employeeTypeId in employeeTypeIds)
			{
				var employeeType = _db.EmployeeTypes.Find(employeeTypeId);
				if (employeeType != null)
				{
					var employeeCount = _db.Person_Employee
						.Where(x => x.EmployeeTypeId == employeeTypeId && x.IsActive)
						.Count();
					formattedData.Data.Add(new List<int> { employeeCount });
					formattedData.Labels.Add(employeeType.Name);
				}
			}

			//var totalEmployeeCount = _db.Person_Employee
			//    .Where(x => employeeTypeIdLists.Contains(x.EmployeeTypeId) && x.IsActive)
			//    .Count();
			//formattedData.Data.Add(new List<int> { totalEmployeeCount });
			//formattedData.Labels.Add("Total");

			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}

		//public ActionResult GetEmployeeType()
		//{
		//    var formattedData = new
		//    {
		//        Data = new List<List<int>>(),
		//        Labels = new List<string>()
		//    };

		//    var employeeTypeIds = _db.Person_Employee.Where(x => x.IsActive).Select(x => x.EmployeeTypeId).Distinct().ToArray();
		//    for (var i = 0; i < employeeTypeIds.Length; i++)
		//    {
		//        var employeeType = _db.EmployeeTypes.Find(employeeTypeIds[i]);
		//        if (employeeType != null)
		//        {
		//            var productionCount = _db.Person_Employee
		//                .Where(x => x.EmployeeTypeId == employeeType.EmployeeTypeId && x.IsActive).Count();
		//            formattedData.Data.Add(new List<int> { productionCount });
		//            formattedData.Labels.Add(employeeType.Name);
		//        }
		//    }
		//    formattedData.Data.Add(new List<int> { _db.Person_Employee
		//                .Where(x =>employeeTypeIds.Contains( x.EmployeeTypeId)&&x.IsActive).Count() });
		//    formattedData.Labels.Add("Total");
		//    return Json(formattedData, JsonRequestBehavior.AllowGet);
		//}
		[AcceptVerbs(HttpVerbs.Get)]
		public JsonResult GetEmployeeByAssignmentType()
		{

			var Newemployees = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.NEW).ToList();
			var promotedEmployees = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.PROMOTION).ToList();
			var demotedEmployees = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.DEMOTION).ToList();
			var recomendedEmployee = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.RECOMENDATION).ToList();
			var TransfredEmployee = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.TRANSFER).ToList();
			var structureChange = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.STRUCTURE_CHANGE).ToList();
			var assignment = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.ASSIGNMENT).ToList();
			var yearlyPromoted = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.YEARLY_PROMOTION).ToList();
			var others = _db.Placements.Where(x => x.isCurrent
													&& x.AssignmentType != AssignmentType.NEW
													&& x.AssignmentType != AssignmentType.PROMOTION
													&& x.AssignmentType != AssignmentType.YEARLY_PROMOTION
													&& x.AssignmentType != AssignmentType.RECOMENDATION
													&& x.AssignmentType != AssignmentType.TRANSFER
													&& x.AssignmentType != AssignmentType.STRUCTURE_CHANGE
													&& x.AssignmentType != AssignmentType.ASSIGNMENT
													&& x.AssignmentType != AssignmentType.ANNUAL_SALARY_INCREMENT).ToList();


			var formattedData = new
			{
				Data = new int[]
				{
					Newemployees.Count(), promotedEmployees.Count(), demotedEmployees.Count(),
					recomendedEmployee.Count(), TransfredEmployee.Count(), structureChange.Count(), assignment.Count(),
					yearlyPromoted.Count(),others.Count()
				},
				Labels = new string[]
				{
					"NEW", "PROMOTION", "DEMOTION", "RECOMENDATION", "TRANSFER", "STRUCTURE_CHANGE", "ASSIGNMENT","YEARLY_PROMOTION","Others"
				}
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);

		}

		[AcceptVerbs(HttpVerbs.Get)]
		public JsonResult GetEmployeeByResponsibilityLevel()
		{
			var staff = _db.Placements.Where(x => x.isCurrent && x.ResponsibilityLevel == ResponsibilityLevel.STAFF).ToList();
			var low = _db.Placements.Where(x => x.isCurrent && x.ResponsibilityLevel == ResponsibilityLevel.LOW_MANAGMENT).ToList();
			var medium = _db.Placements.Where(x => x.isCurrent && x.ResponsibilityLevel == ResponsibilityLevel.MEDIUM_MANAGMENT).ToList();
			var top = _db.Placements.Where(x => x.isCurrent && x.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT).ToList();

			var formattedData = new
			{
				Data = new int[]
				{
					staff.Count() ,low.Count(), medium.Count(), top.Count()
				},
				Labels = new string[] { "Staff", "Low Management", "Medium Management", "Top Management" }
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);

		}

		[AcceptVerbs(HttpVerbs.Get)]
		public JsonResult GetlLeave()
		{

			var todayD = DateTime.Now.Date;
			var today = new DateTime(todayD.Year, todayD.Month, 1);
			var prevMonth = today.AddMonths(-1);
			var secondMonth = today.AddMonths(-2);

			var todayEnd = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
			var prevMonthEnd = new DateTime(prevMonth.Year, prevMonth.Month, DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month));
			var secondMonthEnd = new DateTime(secondMonth.Year, secondMonth.Month, DateTime.DaysInMonth(secondMonth.Year, secondMonth.Month));



			#region Curremt Month
			var annualleaveCurrent = _db.Leaves.Count(x => x.LeaveType == LeaveType.ANNUAL && x.From >= today && x.To <= todayEnd);
			var merrageleaveCurrent = _db.Leaves.Count(x => x.LeaveType == LeaveType.MARRIAGE && x.From >= today && x.To <= todayEnd);
			var maternityleaveCurrent = _db.Leaves.Count(x => x.LeaveType == LeaveType.MATERNITY && x.From >= today && x.To <= todayEnd);
			var paternityleaveCurrent = _db.Leaves.Count(x => x.LeaveType == LeaveType.PATERNITY && x.From >= today && x.To <= todayEnd);
			var sickleaveCurrent = _db.Leaves.Count(x => x.LeaveType == LeaveType.SICK_LEAVE && x.From >= today && x.To <= todayEnd);
			var otherleaveCurrent = _db.Leaves.Count(x => x.LeaveType == LeaveType.OTHER && x.From >= today && x.To <= todayEnd);
			#endregion
			#region second Month
			var annualleavesecond = _db.Leaves.Count(x => x.LeaveType == LeaveType.ANNUAL && x.From >= prevMonth && x.To <= prevMonthEnd);
			var merrageleavesecond = _db.Leaves.Count(x => x.LeaveType == LeaveType.MARRIAGE && x.From >= prevMonth && x.To <= prevMonthEnd);
			var maternityleavesecond = _db.Leaves.Count(x => x.LeaveType == LeaveType.MATERNITY && x.From >= prevMonth && x.To <= prevMonthEnd);
			var paternityleavesecond = _db.Leaves.Count(x => x.LeaveType == LeaveType.PATERNITY && x.From >= prevMonth && x.To <= prevMonthEnd);
			var sickleavesecond = _db.Leaves.Count(x => x.LeaveType == LeaveType.SICK_LEAVE && x.From >= prevMonth && x.To <= prevMonthEnd);
			var otherleavesecond = _db.Leaves.Count(x => x.LeaveType == LeaveType.OTHER && x.From >= prevMonth && x.To <= prevMonthEnd);
			#endregion
			#region third Month
			var annualleavethird = _db.Leaves.Count(x => x.LeaveType == LeaveType.ANNUAL && x.From >= secondMonth && x.To <= secondMonthEnd);
			var merrageleavethird = _db.Leaves.Count(x => x.LeaveType == LeaveType.MARRIAGE && x.From >= secondMonth && x.To <= secondMonthEnd);
			var maternityleavethird = _db.Leaves.Count(x => x.LeaveType == LeaveType.MATERNITY && x.From >= secondMonth && x.To <= secondMonthEnd);
			var paternityleavethird = _db.Leaves.Count(x => x.LeaveType == LeaveType.PATERNITY && x.From >= secondMonth && x.To <= secondMonthEnd);
			var sickleavethird = _db.Leaves.Count(x => x.LeaveType == LeaveType.SICK_LEAVE && x.From >= secondMonth && x.To <= secondMonthEnd);
			var otherleavethird = _db.Leaves.Count(x => x.LeaveType == LeaveType.OTHER && x.From >= secondMonth && x.To <= secondMonthEnd);
			#endregion


			var formattedData = new
			{
				Data = new int[]
				{
					 annualleavethird, annualleavesecond, annualleaveCurrent ,

				},
				Labels = new string[] { "Before Previous Month", "Previous Month ", "Current Month " }
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);

		}

		[AcceptVerbs(HttpVerbs.Get)]
		public JsonResult GetTraining()
		{
			var todayD = DateTime.Now.Date;
			var today = new DateTime(todayD.Year, todayD.Month, 1);
			var fyearId = _fiscalYearHelper.GetCurrentFYearId();
			var fiscalYear = _db.GLFiscalYears.Find(fyearId);
			DateTime CurrentYearStartDate = fiscalYear != null ? fiscalYear.DateFrom : new DateTime(todayD.Year, 1, 1);
			DateTime CurrentYearEnddate = fiscalYear != null ? fiscalYear.DateTo : new DateTime(todayD.Year, 12, 1);
			var prevYearStartDate = CurrentYearStartDate.AddYears(-1);
			var prevYearEndDate = CurrentYearEnddate.AddYears(-1);
			//var prevYearStart = today.AddYears(-1);
			//var todayD = DateTime.Now.Date;
			//var today = new DateTime(todayD.Year, todayD.Month, 1);
			var prevMonth = today.AddMonths(-1);
			var secondMonth = today.AddMonths(-2);

			var todayEnd = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
			var prevMonthEnd = new DateTime(prevMonth.Year, prevMonth.Month, DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month));
			var secondMonthEnd = new DateTime(secondMonth.Year, secondMonth.Month, DateTime.DaysInMonth(secondMonth.Year, secondMonth.Month));


			#region Curremt year
			var trainings = from c in _db.CourseTrackings
							join
								t in _db.Trainee
								on c.CourseTrackingId equals t.CourseTrackingId

							select new { c.From, c.To, c.TrainingType };

			var trainingCurrent = trainings.Count(x => x.From >= CurrentYearStartDate && x.To <= CurrentYearEnddate && x.TrainingType == TrainingType.TRAINING);



			#endregion
			#region before Current year
			var trainingBeforeCurrentYear = trainings.Count(x => x.From >= prevYearStartDate && x.To <= prevYearEndDate && x.TrainingType == TrainingType.TRAINING);

			#endregion
			#region third Month
			var trainingthird = trainings.Count(x => x.From >= secondMonth && x.To <= secondMonthEnd && x.TrainingType == TrainingType.TRAINING);

			#endregion


			var formattedData = new
			{
				Data = new int[]
				{

					 trainingBeforeCurrentYear,trainingCurrent

				},
				Labels = new string[] { "Before Current", "Current  " }
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}

		[AcceptVerbs(HttpVerbs.Get)]
		public JsonResult GetEducation()
		{
			var todayD = DateTime.Now.Date;
			var today = new DateTime(todayD.Year, todayD.Month, 1);
			var fyearId = _fiscalYearHelper.GetCurrentFYearId();
			var fiscalYear = _db.GLFiscalYears.Find(fyearId);
			DateTime CurrentYearStartDate = fiscalYear != null ? fiscalYear.DateFrom : new DateTime(todayD.Year, 1, 1);
			DateTime CurrentYearEnddate = fiscalYear != null ? fiscalYear.DateTo : new DateTime(todayD.Year, 12, 1);
			var prevYearStartDate = CurrentYearStartDate.AddYears(-1);
			var prevYearEndDate = CurrentYearEnddate.AddYears(-1);
			var prevMonth = today.AddMonths(-1);
			var secondMonth = today.AddMonths(-2);

			var todayEnd = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
			var prevMonthEnd = new DateTime(prevMonth.Year, prevMonth.Month, DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month));
			var secondMonthEnd = new DateTime(secondMonth.Year, secondMonth.Month, DateTime.DaysInMonth(secondMonth.Year, secondMonth.Month));


			#region Curremt Month
			var education = from c in _db.CourseTrackings
							join
								t in _db.Trainee
								on c.CourseTrackingId equals t.CourseTrackingId

							select new { c.From, c.To, c.TrainingType };

			var educationCurrent = education.Count(x => x.From >= CurrentYearStartDate && x.To <= CurrentYearEnddate && x.TrainingType == TrainingType.EDUCATION);



			#endregion
			#region Before this Year
			var educationBeforeCurrentYear = education.Count(x => x.From >= prevYearStartDate && x.To <= prevYearEndDate && x.TrainingType == TrainingType.EDUCATION);

			#endregion
			//#region third Month
			//var educationthird = education.Count(x => x.From >= secondMonth && x.To <= secondMonthEnd && x.TrainingType == TrainingType.EDUCATION);

			//#endregion


			var formattedData = new
			{
				Data = new int[]
				{
						educationBeforeCurrentYear,educationCurrent

				},
				Labels = new string[] { "Before Current", "Current " }
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}

		[AcceptVerbs(HttpVerbs.Get)]
		public JsonResult GetAbsentEmployee()
		{

			var todayD = DateTime.Now.Date;
			var currentMonthStartDate = new DateTime(todayD.Year, todayD.Month, 1);
			var prevMonth = currentMonthStartDate.AddMonths(-1);
			var secondMonth = currentMonthStartDate.AddMonths(-2);

			var currentManthEndDate = new DateTime(currentMonthStartDate.Year, currentMonthStartDate.Month, DateTime.DaysInMonth(currentMonthStartDate.Year, currentMonthStartDate.Month));
			var prevMonthEnd = new DateTime(prevMonth.Year, prevMonth.Month, DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month));
			var secondMonthEnd = new DateTime(secondMonth.Year, secondMonth.Month, DateTime.DaysInMonth(secondMonth.Year, secondMonth.Month));

			#region Absent

			int index = 0;
			GLPeriod periodOne = new GLPeriod();
			GLPeriod periodTwo = new GLPeriod();
			string namecurrent = "";
			string nameOne = "";
			string nameTwo = "";
			List<string> allPeriodsname = new List<string>();
			var currentPeriod = _db.GLPeriods.FirstOrDefault(x => currentMonthStartDate >= x.DateFrom && currentManthEndDate <= x.DateTo);
			if (currentPeriod != null)
			{
				namecurrent = currentPeriod.Name;
				var allPeriods = _db.GLPeriods.Where(x => x.DateTo <= currentPeriod.DateTo && x.GLPeriodId != currentPeriod.GLPeriodId).OrderBy(x => x.DateTo).ToList();
				foreach (var a in allPeriods)
				{
					if (index == 0)
					{
						periodOne = new GLPeriod();
						periodOne = a;
						nameOne = a.Name;
					}
					else if (index == 1)
					{
						periodTwo = new GLPeriod();
						periodTwo = a;
						nameTwo = a.Name;
					}

					else
					{
						break;
					}
					index++;
				}
			}


			if (nameOne != "")
			{
				allPeriodsname.Add(nameOne);

			}
			if (nameTwo != "")
			{
				allPeriodsname.Add(nameTwo);

			}
			if (namecurrent != "")
			{
				allPeriodsname.Add(namecurrent);

			}

			#region Curremt Month
			var lateComming = _db.LateCommings.ToList();
			var abcets = _db.EmpPayrollNoDaysWorkeds.ToList();

			var absentCurrent = abcets.Where(x => currentPeriod != null && x.GLPeriodId >= currentPeriod.GLPeriodId).ToList().Select(x => x.EmployeeId).Distinct().Count();
			// var lateCurrent = lateComming.Where(x => currentPeriod != null && (x.Date >= currentPeriod.DateFrom && x.Date <= currentPeriod.DateTo)).ToList().Sum(x => (int)x.Time);
			var lateCurrent = lateComming.Where(x => x.Date >= currentMonthStartDate && x.Date <= currentManthEndDate).ToList().Select(x => x.EmployeeId).Distinct().Count();

			#endregion
			#region second Month
			//var absentsecond = abcets.Where(x => x.GLPeriodId >= periodTwo.GLPeriodId).ToList().Sum(x => x.Days);
			var absentsecond = abcets.Where(x => x.GLPeriodId >= periodTwo.GLPeriodId).ToList().Select(x => x.EmployeeId).Distinct().Count();
			var latesecond = lateComming.Where(x => x.Date >= periodTwo.DateFrom && x.Date <= periodTwo.DateTo).ToList().Select(x => x.EmployeeId).Distinct().Count();
			#endregion
			#region third Month
			var absentthird = abcets.Where(x => x.GLPeriodId >= periodOne.GLPeriodId).ToList().Select(x => x.EmployeeId).Distinct().Count();
			var latethird = lateComming.Where(x => x.Date >= periodOne.DateFrom && x.Date <= periodOne.DateTo).ToList().Select(x => x.EmployeeId).Distinct().Count();
			#endregion

			#endregion


			//var formattedData = new
			//{
			//    Data = new int[,]
			//    {

			//         { absentthird,latethird, absentsecond,latesecond, absentCurrent,  lateCurrent }

			//    },
			//    Labels = new string[,] { { "Absent Before Two month", "Late Coming Before Two month", "Absent Before month", "Late Coming Before  month", "Absent Current Month", "Late Coming Current Month" } }
			//};
			//return Json(formattedData, JsonRequestBehavior.AllowGet);
			var formattedData = new
			{
				Data = new int[]
			   {
					 absentCurrent,  lateCurrent
			   },
				Labels = new string[] { "Absent", "Late Coming" }
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}

		[AcceptVerbs(HttpVerbs.Get)]
		public JsonResult GetVacantByBranch()
		{
			#region  Vacant Postion By Branch
			//var listOfBranchs = _db.Branch.OrderBy(x => x.Name).ToList();
			//var userBranch = UserInformationHelper.GetUserBranchs(User.Identity.Name).FirstOrDefault();
			decimal quntity = 0;
			decimal assignedQuant = 0;
			decimal vacantQuantity = 0;

			var mapping = _db.JobTitleMapping.Where(x => x.IsActive).ToList();
			if (mapping.Any())
			{
				quntity = mapping.Sum(x => x.Quntity);
			}
			var placments = _db.Placements.Where(x => x.isCurrent).ToList();
			if (placments.Any())
			{
				assignedQuant = placments.Count();
			}

			#endregion

			var formattedData = new
			{
				Data = new decimal[]
				{

					quntity - assignedQuant,assignedQuant

				},
				Labels = new string[] { "Vacant Position", "Assigned Quantity" }
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}

		[AcceptVerbs(HttpVerbs.Get)]
		public JsonResult GetTermination()
		{


			var fyearId = _fiscalYearHelper.GetCurrentFYearId();
			var fiscalYear = _db.GLFiscalYears.Find(fyearId);
			var todayD = DateTime.Now.Date;
			var today = new DateTime(todayD.Year, todayD.Month, 1);
			DateTime CurrentYearStartDate = fiscalYear != null ? fiscalYear.DateFrom : new DateTime(todayD.Year, 1, 1);
			DateTime CurrentYearEnddate = fiscalYear != null ? fiscalYear.DateTo : new DateTime(todayD.Year, 12, 1);
			var prevYearStartDate = CurrentYearStartDate.AddYears(-1);
			var prevYearEndDate = CurrentYearEnddate.AddYears(-1);
			var prevYearStart = today.AddYears(-1);
			var prevMonth = today.AddYears(-1);
			var secondMonth = today.AddMonths(-2);

			var todayEnd = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
			var prevMonthEnd = new DateTime(prevMonth.Year, prevMonth.Month, DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month));
			var secondMonthEnd = new DateTime(secondMonth.Year, secondMonth.Month, DateTime.DaysInMonth(secondMonth.Year, secondMonth.Month));

			var allTermination = _db.Terminations.ToList();

			#region Current Year

			var terminationCurrent =
				allTermination.Count(x => x.TerminationDate >= CurrentYearStartDate && x.TerminationDate <= CurrentYearEnddate);

			#endregion

			#region last year 

			var terminationsecond =
				allTermination.Count(x => x.TerminationDate >= prevYearStartDate && x.TerminationDate <= prevYearEndDate);

			#endregion


			#region third Month

			var terminationthird = allTermination.Count(x =>
				x.TerminationDate <= secondMonth && x.TerminationDate <= secondMonthEnd);

			#endregion

			#region commented
			//#region Termination
			//var terminationTypes = _db.Lookups.Where(x => x.LookupType == LookUpTypes.TERMINATIONTYPE).ToList();


			//var formattedData = new
			//{
			//    Data = new int[]{},
			//    Labels = new string[] { }
			//};
			//foreach (var t in terminationTypes)
			//{
			//    #region Curremt Month

			//    //var terminations = _db.Terminations.Where(x => x.LookupId == t.LookupId).ToList();

			//    //var terminationCurrent =
			//    //    terminations.Count(x => x.TerminationDate >= today && x.TerminationDate <= todayEnd);



			//    #endregion

			//    #region second Month

			//    //var terminationsecond =
			//    //    terminations.Count(x => x.TerminationDate >= prevMonth && x.TerminationDate <= prevMonthEnd);

			//    #endregion

			//    #region third Month

			//    //var terminationthird = terminations.Count(x =>
			//    //    x.TerminationDate >= secondMonth && x.TerminationDate <= secondMonthEnd);

			//    #endregion



			//    #endregion

			//     formattedData = new
			//    {
			//        Data = new int[]
			//        {

			//            terminationthird,terminationsecond, terminationCurrent

			//        },
			//        Labels = new string[] {"Month Before Prev","Prev Month","Current Month"}
			//    };
			//}

			#endregion

			var formattedData = new
			{
				Data = new int[]
				{

						terminationsecond, terminationCurrent



				},
				Labels = new string[] { "Last Year", "Current Year" }
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}

		[AcceptVerbs(HttpVerbs.Get)]
		public JsonResult GetAccidentRecored()
		{
			#region Accident Record
			var fyearId = _fiscalYearHelper.GetCurrentFYearId();
			var fiscalYear = _db.GLFiscalYears.Find(fyearId);
			var todayD = DateTime.Now.Date;
			var today = new DateTime(todayD.Year, todayD.Month, 1);
			DateTime CurrentYearStartDate = fiscalYear != null ? fiscalYear.DateFrom : new DateTime(todayD.Year, 1, 1);
			DateTime CurrentYearEnddate = fiscalYear != null ? fiscalYear.DateTo : new DateTime(todayD.Year, 12, 1);
			var prevYearStartDate = CurrentYearStartDate.AddYears(-1);
			var prevYearEndDate = CurrentYearEnddate.AddYears(-1);
			string[] types = { "On Work Place", "Not On Work Place" };

			var CurrentAccidentIns = _db.AccidentRecords.Where(x => x.Date >= CurrentYearStartDate && x.Date <= CurrentYearEnddate).Where(x => x.IsInWorkPlace == true).ToList();
			var CurrentAccidentOuts = _db.AccidentRecords.Where(x => x.Date >= today && x.Date <= CurrentYearEnddate).Where(x => x.IsInWorkPlace == false).ToList();

			#region Curremt Month


			var LastaccidentIn = _db.AccidentRecords.Where(x => x.IsInWorkPlace == true)
										.Where(x => x.Date >= prevYearStartDate && x.Date <= prevYearEndDate).Select(x => x.EmployeeId).Distinct().Count();
			var Lataccidentout = _db.AccidentRecords.Where(x => x.Date >= prevYearStartDate && x.Date <= prevYearEndDate).Select(x => x.EmployeeId).Distinct().Count();





			var formattedData = new
			{
				Data = new int[]
					{

					   LastaccidentIn,Lataccidentout, CurrentAccidentIns.Count(), CurrentAccidentOuts.Count()

					},
				Labels = new string[] { "On Work Place", "Out of Work Place", " On Work Place", " Out of Work Place" }
			};


			#endregion


			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}

		[AcceptVerbs(HttpVerbs.Get)]
		public JsonResult GetVacancy()
		{

			var fyearId = _fiscalYearHelper.GetCurrentFYearId();
			var fiscalYear = _db.GLFiscalYears.Find(fyearId);
			var todayD = DateTime.Now.Date;
			var today = new DateTime(todayD.Year, todayD.Month, 1);
			DateTime CurrentYearStartDate = fiscalYear != null ? fiscalYear.DateFrom : new DateTime(todayD.Year, 1, 1);
			DateTime CurrentYearEnddate = fiscalYear != null ? fiscalYear.DateTo : new DateTime(todayD.Year, 12, 1);
			var prevYearStartDate = CurrentYearStartDate.AddYears(-1);
			var prevYearEndDate = CurrentYearEnddate.AddYears(-1);


			var prevMonth = today.AddMonths(-1);
			var secondMonth = today.AddMonths(-2);

			var todayEnd = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
			var prevMonthEnd = new DateTime(prevMonth.Year, prevMonth.Month, DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month));
			var secondMonthEnd = new DateTime(secondMonth.Year, secondMonth.Month, DateTime.DaysInMonth(secondMonth.Year, secondMonth.Month));

			#region Vacancy
			string[] vacancytypes = { "INTERNAL", "EXTERNAL" };
			int vacancyCurrent = 0;
			int vacancysecond = 0;
			int vacancythird = 0;

			var formattedData = new
			{
				Data = new int[]
				{ },
				Labels = new string[] { }
			};

			foreach (var t in vacancytypes)
			{
				var vacancies = new List<Vacancy>();
				var allVacancy = from v in _db.VaccancyPlans
								 join r in _db.RecruitmentVacancy
								 on v.VacancyId equals r.VacancyId
								 select new { v.From, v.To, v.RecruSource };

				#region Curremt Month

				if (t == "INTERNAL")
				{
					// vacancies = allVacancy.Where(v => ).ToList();

					vacancyCurrent = allVacancy.Count(x => x.From >= CurrentYearStartDate && x.To <= CurrentYearEnddate && x.RecruSource == RecruitmentSource.INTERNAL);
					vacancysecond = vacancies.Count(x => x.From >= prevYearStartDate && x.To <= prevYearEndDate && x.RecruSource == RecruitmentSource.INTERNAL);
					//vacancythird = vacancies.Count(x => x.From >= secondMonth && x.To <= secondMonthEnd && x.RecruSource == RecruitmentSource.INTERNAL);
				}
				else
				{
					vacancyCurrent = allVacancy.Count(x => x.From >= CurrentYearStartDate && x.To <= CurrentYearEnddate && x.RecruSource == RecruitmentSource.EXTERNAL);
					vacancysecond = vacancies.Count(x => x.From >= prevYearStartDate && x.To <= prevYearEndDate && x.RecruSource == RecruitmentSource.EXTERNAL);
					//vacancythird = vacancies.Count(x => x.From >= secondMonth && x.To <= secondMonthEnd && x.RecruSource == RecruitmentSource.EXTERNAL);

				}


				#endregion

				formattedData = new
				{
					Data = new int[]
					 {

						vacancysecond, vacancyCurrent

					 },
					Labels = new string[] { "Before Current ", "Current" }
				};
			}

			#endregion

			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}

		[AcceptVerbs(HttpVerbs.Get)]
		public JsonResult GetEmployeeDispline()
		{


			var fyearId = _fiscalYearHelper.GetCurrentFYearId();
			var fiscalYear = _db.GLFiscalYears.Find(fyearId);

			//var todayD = DateTime.Now.Date;
			//var today = new DateTime(todayD.Year, todayD.Month, 1);
			//var prevMonth = today.AddMonths(-1);
			//var todayEnd = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
			//var prevMonthEnd = new DateTime(prevMonth.Year, prevMonth.Month, DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month));
			var todayD = DateTime.Now.Date;
			var today = new DateTime(todayD.Year, todayD.Month, 1);
			DateTime CurrentYearStartDate = fiscalYear != null ? fiscalYear.DateFrom : new DateTime(todayD.Year, 1, 1);
			DateTime CurrentYearEnddate = fiscalYear != null ? fiscalYear.DateTo : new DateTime(todayD.Year, 12, 1);
			var prevYearStartDate = CurrentYearStartDate.AddYears(-1);
			var prevYearEndDate = CurrentYearEnddate.AddYears(-1);

			#region Curremt Month

			var CurrentMonth = _db.DisciplinaryCases.Where(x => x.TransgressionDate >= CurrentYearStartDate && x.TransgressionDate <= CurrentYearEnddate).Select(x => x.EmployeeId).Distinct().Count();



			#endregion
			#region before Current Month
			var disciplinaryCases = _db.DisciplinaryCases.Where(x => x.TransgressionDate >= prevYearStartDate && x.TransgressionDate <= prevYearEndDate).Select(x => x.EmployeeId).Distinct().Count();

			#endregion



			var formattedData = new
			{
				Data = new int[]
				{

					 disciplinaryCases,CurrentMonth

				},
				Labels = new string[] { "Last Year", "Current Year " }
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		[AcceptVerbs(HttpVerbs.Get)]
		public JsonResult GetNewHiredEmployeeCopy()
		{
			var todayD = DateTime.Now.Date;
			var fyearId = _fiscalYearHelper.GetCurrentFYearId();

			var fiscalYear = _db.GLFiscalYears.Find(fyearId);
			var currentYearStartDate = fiscalYear != null ? fiscalYear.DateFrom : new DateTime(todayD.Year, 7, 1);
			var currentYearEndDate = fiscalYear != null ? fiscalYear.DateTo : new DateTime(todayD.AddYears(1).Year, 6, 1);
			var lastYearStartDate = currentYearStartDate.AddYears(-1);
			var lastYearEndDate = currentYearEndDate.AddYears(-1);
			var employeeIds = _db.Placements.Where(x => x.isCurrent).Select(x => x.EmployeeId).Distinct().ToList();
			var employeesQuery = _db.Person_Employee.Where(x => employeeIds.Contains(x.EmployeeId));
			#region Curremt year

			var HiredCurrentYear = employeesQuery.Count(x => x.HiredDate >= currentYearStartDate);



			#endregion
			#region Last year
			var hiredLastYear = employeesQuery.Count(x => x.HiredDate >= lastYearStartDate && x.HiredDate <= lastYearEndDate);

			#endregion



			var formattedData = new
			{
				Data = new int[]
				{

					hiredLastYear,HiredCurrentYear,

				},
				Labels = new string[] { "Previous  Year", "Current Year", }
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}

        public JsonResult GetNewHiredEmployee()
        {
            var today = DateTime.Now.Date;
            var fiscalYearId = _fiscalYearHelper.GetCurrentFYearId();
            var fiscalYear = _db.GLFiscalYears.Find(fiscalYearId);
            var currentYearStartDate = fiscalYear?.DateFrom ?? new DateTime(today.Year, 7, 1);
            var currentYearEndDate = fiscalYear?.DateTo ?? new DateTime(today.Year + 1, 6, 30);
            var lastYearStartDate = currentYearStartDate.AddYears(-1);
            var lastYearEndDate = currentYearEndDate.AddYears(-1);
            //var employeeIds = _db.Placements
            //                     .Where(x => x.isCurrent)
            //                     .Select(x => x.EmployeeId).ToList();
            var employeesQuery = _db.Person_Employee.AsQueryable();
            /* .Where(x => employeeIds.Contains(x.EmployeeId))*/
            ;
            // Current year hires
            var hiredCurrentYear = employeesQuery.Where(x => x.IsPlaced)
                                   .Count(x => x.HiredDate >= currentYearStartDate
                                                && x.HiredDate <= currentYearEndDate);
            // Last year hires
            var hiredLastYear = employeesQuery.Where(x => x.IsPlaced)
                                .Count(x => x.HiredDate >= lastYearStartDate && x.HiredDate <= lastYearEndDate);
            var formattedData = new
            {
                Data = new[] { hiredLastYear, hiredCurrentYear },
                Labels = new[] { "Last Year", "Current Year" }
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Get)]
		public JsonResult GetPayrollPerMonth()
		{
			List<PayrollDashboardViewModel> payrollProcess = new List<PayrollDashboardViewModel>();

			var fiscalyearname = _fiscalYearHelper.GetCurrentFYearName();
			var fiscalYear = _db.GLFiscalYears.FirstOrDefault(x => x.Name == fiscalyearname);
			var payrolList = _db.PayrollProcessViewModel.ToList();
			if (fiscalYear != null)
			{

				var periods = _db.GLPeriods.Where(x => x.GlFiscalYearId == fiscalYear.GlFiscalYearId).OrderBy(x => x.Index).ToList();
				for (int i = 0; i < periods.Count(); i++)
				{

					PayrollDashboardViewModel payroll = new PayrollDashboardViewModel();
					payroll.PayrollPeriod = new List<string>();
					payroll.PayrollData = new List<decimal>();
					payroll.PayrollPeriod.Add(periods[i].Name);
					var payrollPeriod = payrolList.Where(x => x.GLPeriodId == periods[i].GLPeriodId).ToList();
					if (payrollPeriod.Any())
					{
						payroll.PayrollData.Add(payrollPeriod.Sum(x => x.GrossEarning));
					}
					payrollProcess.Add(payroll);


				}


			}
			var formattedData = new
			{
				Data = payrollProcess.Select(x => x.PayrollData).ToArray(),
				Labels = payrollProcess.Select(x => x.PayrollPeriod).ToArray()
			};

			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		//public JsonResult GetPayrollPerMonth_old()
		//{
		//    List<PayrollDashboardViewModel> payrollProcess = new List<PayrollDashboardViewModel>();

		//    var fiscalyearname = _fiscalYearHelper.GetCurrentFYearName();

		//    var fiscalYear = _db.GLFiscalYears.FirstOrDefault(x => x.Name == fiscalyearname);

		//    decimal totalAllowance = 0;
		//    decimal totalDeduction = 0;
		//    decimal totalAddition = 0;
		//    decimal totalEarn = 0;
		//    decimal totalLoan = 0;


		//    var payrolList = _db.PayrollProcessViewModel.ToList();
		//    if (fiscalYear != null)
		//    {

		//        var periods =
		//            _db.GLPeriods.Where(x => x.GlFiscalYearId == fiscalYear.GlFiscalYearId).OrderBy(x => x.Index).ToList();

		//        for (int i = 0; i < periods.Count(); i++)
		//        {
		//            totalAllowance = 0;
		//            totalDeduction = 0;
		//            totalAddition = 0;
		//            totalEarn = 0;
		//            totalLoan = 0;

		//            PayrollDashboardViewModel payroll = new PayrollDashboardViewModel();
		//            payroll.PayrollData = new List<PayrollData>();
		//            payroll.PayrollPeriod = new String[1] { periods[i].Name };
		//            var payrollPeriod = payrolList.Where(x => x.GLPeriodId == periods[i].GLPeriodId).ToList();

		//            if (payrollPeriod.Any())
		//            {
		//                var data = new PayrollData();
		//                data.Earn = payrollPeriod.Sum(x => x.GrossEarning);
		//                data.Loan = payrollPeriod.Sum(x => x.TotalLoan);
		//                data.Allowance = payrollPeriod.Sum(x => x.TotalAllowance);
		//                data.Deduction = payrollPeriod.Sum(x => x.TotalDeductions);
		//                data.Addition = payrollPeriod.Sum(x => x.TotalAdditions);

		//                payroll.PayrollData.Add(data); //new decimal[5] { totalEarn, totalLoan, totalAllowance, totalDeduction, totalAddition };
		//                payrollProcess.Add(payroll);

		//            }



		//        }


		//    }
		//    var formattedData = new
		//    {
		//        Data = payrollProcess.Select(x => x.PayrollData),
		//        Labels = payrollProcess.Select(x => x.PayrollPeriod).ToArray()
		//    };

		//    return Json(formattedData, JsonRequestBehavior.AllowGet);
		//}

		public class PayrollDashboardViewModel
		{
			public List<string> PayrollPeriod { get; set; }
			public List<decimal> PayrollData { get; set; }
		}

		public class PayrollData
		{
			public decimal Earn { get; set; }
			public decimal Loan { get; set; }
			public decimal Allowance { get; set; }
			public decimal Deduction { get; set; }
			public decimal Addition { get; set; }

		}


		#endregion



		#endregion



		#region fleet dashboard
		public JsonResult GetFleetOwnershipCount()
		{
			var own = _db.Fleets.Where(x => !x.IsExternal).ToList();

			var rental = _db.Fleets.Where(x => x.IsExternal).ToList();


			var retrunData = new
			{
				Data = new int[] { own.Count(), rental.Count(), },
				Labels = new string[] { "Own", "Rental" }
			};

			return Json(retrunData, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetOwnEquipmentByCategoryData()
		{
			var fleetData = _db.Fleets.Where(x => !x.IsExternal)
				.Join(_db.EquipmentIdentity, f => f.EquipmentIdentityID, et => et.EquipmentIdentityID, (f, et) => new { f, et })
				.GroupBy(fe => fe.et.EquipmentCategoryName)
				.Select(g => new
				{
					EquipmentType = g.Key,
					Count = g.Count()
				})
				.ToList();

			var result = new
			{
				Labels = fleetData.Select(fd => fd.EquipmentType.ToString()).ToArray(),
				Data = fleetData.Select(fd => fd.Count).ToArray()
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetRentalEquipmentByCategoryData()
		{
			var fleetData = _db.Fleets.Where(x => x.IsExternal)
				.Join(_db.EquipmentIdentity, f => f.EquipmentIdentityID, et => et.EquipmentIdentityID, (f, et) => new { f, et })
				.GroupBy(fe => fe.et.EquipmentCategoryName)
				.Select(g => new
				{
					EquipmentType = g.Key,
					Count = g.Count()
				})
				.ToList();

			var result = new
			{
				Labels = fleetData.Select(fd => fd.EquipmentType.ToString()).ToArray(),
				Data = fleetData.Select(fd => fd.Count).ToArray()
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetMachineryAggregateHours()
		{
			var totalDownHours = _db.MachineryWorkingTimeSheets.Sum(m => m.DownHours);
			var totalIdleHours = _db.MachineryWorkingTimeSheets.Sum(m => m.IdleHours);
			var totalOperationalHours = _db.MachineryWorkingTimeSheets.Sum(m =>
				(DbFunctions.DiffHours(m.MorningTimeStart, m.MorningTimeEnd).Value +
				 DbFunctions.DiffHours(m.AfternoonTimeStart, m.AfternoonTimeEnd).Value +
				 DbFunctions.DiffHours(m.NightTimeStart, m.NightTimeEnd).Value));

			var result = new
			{
				DownHour = totalDownHours,
				IdleHour = totalIdleHours,
				OperationalHour = totalOperationalHours
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetLightVehicleAggregateHours()
		{
			var totalDownHours = _db.LightVehicleTimeSheets.Sum(m => m.DownHours);
			var totalIdleHours = _db.LightVehicleTimeSheets.Sum(m => m.IdleHours);
			var totalOperationalHours = _db.LightVehicleTimeSheets.Sum(m =>
				(DbFunctions.DiffHours(m.MorningTimeStart, m.MorningTimeEnd).Value +
				 DbFunctions.DiffHours(m.AfternoonTimeStart, m.AfternoonTimeEnd).Value +
				 DbFunctions.DiffHours(m.NightTimeStart, m.NightTimeEnd).Value));

			var result = new
			{
				DownHour = totalDownHours,
				IdleHour = totalIdleHours,
				OperationalHour = totalOperationalHours
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetDryLoadAggregateHours()
		{
			//var totalDownHours = _db.DryLoadingTimeSheets.Sum(m => m.d);
			var totalIdleHours = _db.DryLoadingTimeSheets.Sum(m => m.IdleHour);
			var totalOperationalHours = _db.DryLoadingTimeSheets.Sum(m => m.WorkingHour);


			var result = new
			{
				DownHour = 0,//totalDownHours,
				IdleHour = totalIdleHours,
				OperationalHour = totalOperationalHours
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetWagonDrillAggregateHours()
		{
			var totalDownHours = _db.WagonDrillTimesheets.Sum(m => m.DownTime);
			var totalIdleHours = _db.WagonDrillTimesheets.Sum(m => m.IdleHour);
			var totalOperationalHours = _db.WagonDrillTimesheets.Sum(m => m.OperationNetWorkingHour);


			var result = new
			{
				DownHour = totalDownHours,
				IdleHour = totalIdleHours,
				OperationalHour = totalOperationalHours
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetDumpTruckAggregateHours()
		{
			var totalDownHours = _db.DumpTruckTimeSheets.Sum(m => m.DownHour);
			var totalIdleHours = _db.DumpTruckTimeSheets.Sum(m => m.IdleHour);
			var totalOperationalHours = _db.DumpTruckTimeSheets.Sum(m => m.NetworkingHour);


			var result = new
			{
				DownHour = totalDownHours,
				IdleHour = totalIdleHours,
				OperationalHour = totalOperationalHours
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetTotalFleetOID()
		{
			var mtotalDownHours = _db.MachineryWorkingTimeSheets.Sum(m => m.DownHours);
			var mtotalIdleHours = _db.MachineryWorkingTimeSheets.Sum(m => m.IdleHours);
			var mtotalOperationalHours = (decimal)_db.MachineryWorkingTimeSheets.Sum(m =>
				(DbFunctions.DiffHours(m.MorningTimeStart, m.MorningTimeEnd).Value +
				 DbFunctions.DiffHours(m.AfternoonTimeStart, m.AfternoonTimeEnd).Value +
				 DbFunctions.DiffHours(m.NightTimeStart, m.NightTimeEnd).Value));

			var lvtotalDownHours = _db.LightVehicleTimeSheets.Sum(m => m.DownHours);
			var lvtotalIdleHours = _db.LightVehicleTimeSheets.Sum(m => m.IdleHours);
			var lvtotalOperationalHours = (decimal)_db.LightVehicleTimeSheets.Sum(m =>
				(DbFunctions.DiffHours(m.MorningTimeStart, m.MorningTimeEnd).Value +
				 DbFunctions.DiffHours(m.AfternoonTimeStart, m.AfternoonTimeEnd).Value +
				 DbFunctions.DiffHours(m.NightTimeStart, m.NightTimeEnd).Value));

			//var dltotalDownHours = _db.DryLoadingTimeSheets.Sum(m => m.d);
			var dltotalIdleHours = _db.DryLoadingTimeSheets.Sum(m => m.IdleHour);
			var dltotalOperationalHours = _db.DryLoadingTimeSheets.Sum(m => m.WorkingHour);

			var wdtotalDownHours = _db.WagonDrillTimesheets.Sum(m => m.DownTime);
			var wdtotalIdleHours = _db.WagonDrillTimesheets.Sum(m => m.IdleHour);
			var wdtotalOperationalHours = _db.WagonDrillTimesheets.Sum(m => m.OperationNetWorkingHour);

			var dttotalDownHours = _db.DumpTruckTimeSheets.Sum(m => m.DownHour);
			var dttotalIdleHours = _db.DumpTruckTimeSheets.Sum(m => m.IdleHour);
			var dttotalOperationalHours = _db.DumpTruckTimeSheets.Sum(m => m.NetworkingHour);

			var result = new
			{
				DownHour = mtotalDownHours + lvtotalDownHours + 0 + wdtotalDownHours + dttotalDownHours,
				IdleHour = mtotalIdleHours + lvtotalIdleHours + dltotalIdleHours + wdtotalIdleHours + dttotalIdleHours,
				OperationalHour = mtotalOperationalHours + lvtotalOperationalHours + dltotalOperationalHours + dttotalOperationalHours + wdtotalOperationalHours
			};
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		#endregion

		public JsonResult RefreshInventoryDashboard()
		{
			//_dashboardHelper.UpdateInventoryDocStatusDict();
			_dashboardHelper.UpdateDashboardItemBalance();
			_dashboardHelper.UpdateInventoryDocStatusDict();
			var formattedData = new
			{
				Message = "Update Complete.",
				Status = "OK"
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}

	   
		public JsonResult RefreshProcurementDashboard()
		{
			_dashboardHelper.UpdateProcurementDocStatusDict();
			var formattedData = new
			{
				Message = "Update Complete.",
				Status = "OK"
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}

		public JsonResult RefreshFixedAssetDashboard()
		{
			_dashboardHelper.UpdateFixedAssetDocStatusDict();
			var formattedData = new
			{
				Message = "Update Complete.",
				Status = "OK"
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}

		#region finance
		public JsonResult RefreshInvoiceAndSupplier()
		{
			_dashboardHelper.UpdateFinanceInvoiceAndSuppliers();
			var formattedData = new
			{
				Message = "Update Complete.",
				Status = "OK"
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult RefreshCashAndBank()
		{
			_dashboardHelper.UpdateFinanceCashAndBank();
			var formattedData = new
			{
				Message = "Update Complete.",
				Status = "OK"
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult RefreshOtherAccounts()
		{
			_dashboardHelper.UpdateFinanceOtherAccount();
			var formattedData = new
			{
				Message = "Update Complete.",
				Status = "OK"
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult RefresIncomeAndExpense()
		{
			_dashboardHelper.UpdateFinanceIncomeExpense();
			var formattedData = new
			{
				Message = "Update Complete.",
				Status = "OK"
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		#endregion
	  
	}
}