using System;
using ExceedERP.Helpers.Finance;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Models;
using Kendo.Mvc.Extensions;

namespace ExceedERP.Web.Controllers
{

    public partial class DashboardController : Controller
	{
		private decimal nth = 1000000;//to change to thousands

		#region finance module

		public JsonResult OtherAccount()
		{
			var list = GetBankAndCashChartData("OTHER_ACCOUNT");
			var labels = GetBankAndCashChartMonths("OTHER_ACCOUNT");
			var formattedData = new
			{
				Labels = labels,
				DataSet = list.ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetCashAndBank()
		{
			var list = GetBankAndCashChartData("CASH_BANK");
			var labels = GetBankAndCashChartMonths("CASH_BANK");
			var formattedData = new
			{
				Labels = labels.ToArray(),
				DataSet = list.ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetIncomeExpense()
		{
			var list = GetIncomeExpensechartData("INCOME_EXPENSE");
			var formattedData = new
			{
				DataProfit = list.Select(x => x.Profit).ToArray(),
				DataExpense = list.Select(x => x.Expense).ToArray(),
				DataIncome = list.Select(x => x.Income).ToArray(),
				Labels = list.Select(x => x.Month).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetApInvoiceAndPayment()
		{
			var list = GetchartData("AP_INVOICE_PAYMENT");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetArInvoiceAndCollection()
		{
			var list = GetchartData("AR_INVOICE_COLLECTION");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetSupplierStatus()
		{
			var list = GetchartData("AP_SUPPLIER");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetApInvoiceStatus()
		{
			var list = GetchartData("AP_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x=>x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetApPaymentStatus()
		{
			var list = GetchartData("AP_PAYMENT_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetArInvoiceStatus()
		{
			var list = GetchartData("AR_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetArCollectionStatus()
		{
			var list = GetchartData("AR_COLLECTION_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetGlStatus()
		{
			var list = GetchartData("GL_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}

		private List<ChartData> GetchartData(string type)
		{
			List<ChartData> list = new List<ChartData>();
			var dictionaries = _db.DashboardDictionaries.Where(x => x.Category == type).ToList().DistinctByField(x => x.Name);
			foreach (var dic in dictionaries)
			{
				var chart = new ChartData
				{
					Label = dic.Name,
					Value = dic.Value
				};

				list.Add(chart);
			}

			return list;
		}

		public class IncomeExpenseChartData
		{
			public string Month { get; set; }
			public decimal Profit { get; set; }
			public decimal Expense { get; set; }
			public decimal Income { get; set; }

		}
		private List<IncomeExpenseChartData> GetIncomeExpensechartData(string type)
		{
			var fiscalyearname = _fiscalYearHelper.GetCurrentFYearName();
			var fiscalYear = _db.GLFiscalYears.FirstOrDefault(x => x.Name == fiscalyearname);
			var periodList = _db.GLPeriods.Where(x => x.GlFiscalYearId == fiscalYear.GlFiscalYearId).OrderBy(x => x.Index).ToList().Select(x => x.Name);
			List<IncomeExpenseChartData> list = new List<IncomeExpenseChartData>();
			var dictionaries = _db.DashboardDictionaries.Where(x => x.Category == type).ToList();
			var uniquedictionaries = dictionaries.Where(x => periodList.Contains(x.Dimesion)).ToList().DistinctByField(x => x.Dimesion);

			var data = dictionaries.ToList();

			foreach (var dic in uniquedictionaries)
			{
				var chart = new IncomeExpenseChartData
				{
					Month = dic.Dimesion
				};

				var profit = data.Where(x=>x.Dimesion==dic.Dimesion).FirstOrDefault(x => x.Name == "Profit");
				if (profit != null) chart.Profit = Math.Round((profit.Value/nth),2);

				var expense = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Expense");
				if (expense != null) chart.Expense = Math.Round((expense.Value/nth),2);

				var income = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Income");
				if (income != null) chart.Income = Math.Round((income.Value/nth),2);


				list.Add(chart);
			}

			return list;
		}

		public class AccountWatchChartData
		{
			public string label { get; set; }
			public List<decimal> data { get; set; }
			public bool fill = false;
		}

		public class AccountWatchListData
		{
			public decimal data { get; set; }
		}
		private List<AccountWatchChartData> GetBankAndCashChartData(string type)
		{
			var fiscalyearname = _fiscalYearHelper.GetCurrentFYearName();
			var fiscalYear = _db.GLFiscalYears.FirstOrDefault(x => x.Name == fiscalyearname);
			var periodList = _db.GLPeriods.Where(x => x.GlFiscalYearId == fiscalYear.GlFiscalYearId).OrderBy(x => x.Index).Select(x => x.Name);

			List<AccountWatchChartData> list = new List<AccountWatchChartData>();
			var dictionaries = _db.DashboardDictionaries.Where(x => x.Category == type).ToList();
			var dimenstions = dictionaries.Where(x => periodList.Contains(x.Dimesion)).ToList().Select(x => x.Dimesion).Distinct();
			var names = dictionaries.ToList().Select(x => x.Name).Distinct();

			var data = dictionaries.ToList();

			foreach (var name in names)
			{
				var chart = new AccountWatchChartData();

				chart.label = name;
				chart.data = new List<decimal>();
				List<AccountWatchListData> dataList = new List<AccountWatchListData>();

				foreach (var dim in dimenstions)
				{
					
					var value = data.Where(x => x.Dimesion == dim).FirstOrDefault(x => x.Name == name);
					if (value != null) 
					chart.data.Add(Math.Round((value.Value/nth),2));
				}

				list.Add(chart);
			}

			return list;
		}

		private string[] GetBankAndCashChartMonths(string type)
		{
			var fiscalyearname = _fiscalYearHelper.GetCurrentFYearName();
			var fiscalYear = _db.GLFiscalYears.FirstOrDefault(x => x.Name == fiscalyearname);
			var periodList = _db.GLPeriods.Where(x => x.GlFiscalYearId == fiscalYear.GlFiscalYearId).OrderBy(x => x.Index).Select(x=>x.Name);

			var dictionaries = _db.DashboardDictionaries.Where(x => x.Category == type).ToList();
			string[] months = dictionaries.Where(x=>periodList.Contains(x.Dimesion)).Select(x => x.Dimesion).Distinct().ToArray();

			return months;
		}
		#endregion

		#region inventory transfer to fiannce status

		public JsonResult GetFinanceIssueStatus()
		{
			var list = GetchartData("FINANCE_ISSUE_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetFinanceTransferSendStatus()
		{
			var list = GetchartData("FINANCE_TRANSFERSEND_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetFinanceTransferReceiveStatus()
		{
			var list = GetchartData("FINANCE_TRANSFERRECEIVE_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetFinanceGrnStatus()
		{
			var list = GetchartData("FINANCE_GRN_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetFinanceStoreReturnStatus()
		{
			var list = GetchartData("FINANCE_STORERETURN_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetFinanceStockAdjustmentStatus()
		{
			var list = GetchartData("FINANCE_STOCKADJUSTMENT_DOC_STATUS");
			var formattedData = new
			{
				Data = list.Select(x => x.Value).ToArray(),
				Labels = list.Select(x => x.Label).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}
		#endregion


		#region Ratio DataSource


		public async Task<JsonResult> GetProfitabilityRatio()
		{


			var list = await GetProfitabilityRatiochartData();

			//List<ProfitabilityRatioViewModel> list = new List<ProfitabilityRatioViewModel>();
			//list.Add(new ProfitabilityRatioViewModel { Operating_Profit_Margin = 100, Return_On_Assets = 100, Return_On_Equity = 100, Month = "January" });
			//list.Add(new ProfitabilityRatioViewModel { Gross_Profit_Margin = 800, Return_On_Equity = 500, Cost_Of_Goods_Sold = 0, Month = "April" });


			var formattedData = new
			{
				Cost_Of_Goods_Sold = list.Select(x => x.Cost_Of_Goods_Sold).ToArray(),
				Gross_Profit_Margin = list.Select(x => x.Gross_Profit_Margin).ToArray(),
				Net_Profit_Margin = list.Select(x => x.Net_Profit_Margin).ToArray(),
				Return_On_Assets = list.Select(x => x.Return_On_Assets).ToArray(),
				Return_On_Equity = list.Select(x => x.Return_On_Equity).ToArray(),
				Operating_Profit_Margin = list.Select(x => x.Operating_Profit_Margin).ToArray(),
				Labels = list.Select(x => x.Month).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}


		public async Task<JsonResult> GetLiquidityRatio()
		{


			var list = await GetLiquidityRatiochartData();


			//List<LiquidityRatioViewModel> list = new List<LiquidityRatioViewModel>();
			//list.Add(new LiquidityRatioViewModel { Net_Working_Capital_Ratio = 100, Quick_Ratio = 100, Current_Ratio = 100, Month = "January" });
			//list.Add(new LiquidityRatioViewModel { Net_Working_Capital_Ratio = 800, Quick_Ratio = 500, Current_Ratio = 0, Month = "April" });


			var formattedData = new
			{
				Cash_Ratio = list.Select(x => x.Cash_Ratio).ToArray(),
				Current_Ratio = list.Select(x => x.Current_Ratio).ToArray(),
				Net_Working_Capital_Ratio = list.Select(x => x.Net_Working_Capital_Ratio).ToArray(),
				Quick_Ratio = list.Select(x => x.Quick_Ratio).ToArray(),
				Labels = list.Select(x => x.Month).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}


		public async Task<JsonResult> GetActivityRatio()
		{


			var list = await GetActivityRatiochartData();

			//List<ActivityRatioViewModel> list = new List<ActivityRatioViewModel>();
			//list.Add(new ActivityRatioViewModel { Average_Collection_Period = 100, Fixed_Asset_Turnover = 100, Inventory_TurnOver = 100, Month = "January" });
			//list.Add(new ActivityRatioViewModel { Inventory_TurnOver = 800, Receivables_TurnOver = 500, Fixed_Asset_Turnover = 0, Month = "April" });

			var formattedData = new
			{
				Asset_TurnOver = list.Select(x => x.Asset_TurnOver).ToArray(),
				Average_Collection_Period = list.Select(x => x.Average_Collection_Period).ToArray(),
				Fixed_Asset_Turnover = list.Select(x => x.Fixed_Asset_Turnover).ToArray(),
				Inventory_TurnOver = list.Select(x => x.Inventory_TurnOver).ToArray(),
				Receivables_TurnOver = list.Select(x => x.Receivables_TurnOver).ToArray(),
				Labels = list.Select(x => x.Month).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}


		public async Task<JsonResult> GetCapitalStructureRatio()
		{


			var list = await GetCapitalStructureRatiochartData();

			//List<CapitalStructureRatioViewModel> list = new List<CapitalStructureRatioViewModel>();
			//list.Add(new CapitalStructureRatioViewModel { Debt_To_Assets_Ratio = 200, Debt_To_Equity_Ratio = 800,  Month = "January" });
			//list.Add(new CapitalStructureRatioViewModel { Debt_To_Assets_Ratio = 800, Debt_To_Equity_Ratio = 500 , Month = "April" });

			var formattedData = new
			{
				Debt_To_Assets_Ratio = list.Select(x => x.Debt_To_Assets_Ratio).ToArray(),
				Debt_To_Equity_Ratio = list.Select(x => x.Debt_To_Equity_Ratio).ToArray(),
				Labels = list.Select(x => x.Month).ToArray()
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Ratio DataSource Util

		//private async Task<List<IncomeExpenseChartData>> GetIncomeExpensechartData(string type)
		//{
		//        var periodList = await FiscalYearHelper.GetRecentNPeriodsInYearAsync(6);
		//    var Periods = periodList.OrderBy(x => x.DateTo).Select(x => x.Name).ToList();

		//    List<IncomeExpenseChartData> list = new List<IncomeExpenseChartData>();
		//    var dictionaries = _db.DashboardDictionaries.Where(x => x.Category == type).ToList();

		//    var uniquedictionaries = dictionaries.Where(x => Periods.Contains(x.Dimesion)).ToList().DistinctByField(x => x.Dimesion);

		//    var data = dictionaries.ToList();

		//    foreach (var dic in uniquedictionaries)
		//    {
		//        var chart = new IncomeExpenseChartData
		//        {
		//            Month = dic.Dimesion
		//        };

		//        var profit = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Profit");
		//        if (profit != null) chart.Profit = Math.Round((profit.Value / nth), 2);

		//        var expense = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Expense");
		//        if (expense != null) chart.Expense = Math.Round((expense.Value / nth), 2);

		//        var income = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Income");
		//        if (income != null) chart.Income = Math.Round((income.Value / nth), 2);


		//        list.Add(chart);
		//    }

		//    return list;
		//}

		private async Task<List<ActivityRatioViewModel>> GetActivityRatiochartData()
		{
			nth = 1;
			FiscalYearHelper _FiscalYearHelper = new FiscalYearHelper();
			var periodList = await _FiscalYearHelper.GetRecentNPeriodsInYearAsync(12);
			var Periods = periodList.OrderBy(x => x.DateTo).Select(x => x.Name).ToList();


			List<ActivityRatioViewModel> list = new List<ActivityRatioViewModel>();
			string Type = "ACTIVITY_RATIO";


			var dictionaries = _db.DashboardDictionaries.Where(x => x.Category == Type).ToList();
			var uniquedictionaries = dictionaries.Where(x => Periods.Contains(x.Dimesion)).ToList().DistinctByField(x => x.Dimesion);

			var data = dictionaries.ToList();

			foreach (var dic in uniquedictionaries)
			{
				var chart = new ActivityRatioViewModel
				{
					Month = dic.Dimesion
				};

				var Asset_TurnOver = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Asset_TurnOver");
				if (Asset_TurnOver != null) chart.Asset_TurnOver = Math.Round((Asset_TurnOver.Value / nth), 2);

				var Average_Collection_Period = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Average_Collection_Period");
				if (Average_Collection_Period != null) chart.Average_Collection_Period = Math.Round((Average_Collection_Period.Value / nth), 2);

				var Fixed_Asset_Turnover = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Fixed_Asset_Turnover");
				if (Fixed_Asset_Turnover != null) chart.Fixed_Asset_Turnover = Math.Round((Fixed_Asset_Turnover.Value / nth), 2);

				var Inventory_TurnOver = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Inventory_TurnOver");
				if (Inventory_TurnOver != null) chart.Inventory_TurnOver = Math.Round((Asset_TurnOver.Value / nth), 2);

				var Receivables_TurnOver = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Receivables_TurnOver");
				if (Receivables_TurnOver != null) chart.Receivables_TurnOver = Math.Round((Receivables_TurnOver.Value / nth), 2);



				list.Add(chart);
			}

			return list;
		}
		private async Task<List<LiquidityRatioViewModel>> GetLiquidityRatiochartData()
		{
			nth = 1;
			FiscalYearHelper _FiscalYearHelper = new FiscalYearHelper();
			var periodList = await _FiscalYearHelper.GetRecentNPeriodsInYearAsync(12);
			var Periods = periodList.OrderBy(x => x.DateTo).Select(x => x.Name).ToList();
			List<LiquidityRatioViewModel> list = new List<LiquidityRatioViewModel>();
			string Type = "LIQUIDITY_RATIO";
			var dictionaries = _db.DashboardDictionaries.Where(x => x.Category == Type).ToList();
			var uniquedictionaries = dictionaries.Where(x => Periods.Contains(x.Dimesion)).ToList().DistinctByField(x => x.Dimesion);

			var data = dictionaries.ToList();

			foreach (var dic in uniquedictionaries)
			{
				var chart = new LiquidityRatioViewModel
				{
					Month = dic.Dimesion
				};

				var Cash_Ratio = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Cash_Ratio");
				if (Cash_Ratio != null) chart.Cash_Ratio = Math.Round((Cash_Ratio.Value / nth), 2);

				var Net_Working_Capital_Ratio = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Net_Working_Capital_Ratio");
				if (Net_Working_Capital_Ratio != null) chart.Net_Working_Capital_Ratio = Math.Round((Net_Working_Capital_Ratio.Value / nth), 2);

				var Quick_Ratio = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Quick_Ratio");
				if (Quick_Ratio != null) chart.Quick_Ratio = Math.Round((Quick_Ratio.Value / nth), 2);



				var Current_Ratio = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Current_Ratio");
				if (Current_Ratio != null) chart.Current_Ratio = Math.Round((Cash_Ratio.Value / nth), 2);


				list.Add(chart);
			}

			return list;
		}
		private async Task<List<CapitalStructureRatioViewModel>> GetCapitalStructureRatiochartData()
		{
			nth = 1;
			FiscalYearHelper _FiscalYearHelper = new FiscalYearHelper();
			var periodList = await _FiscalYearHelper.GetRecentNPeriodsInYearAsync(12);
			var Periods = periodList.OrderBy(x => x.DateTo).Select(x => x.Name).ToList();
			List<CapitalStructureRatioViewModel> list = new List<CapitalStructureRatioViewModel>();
			string Type = "CAPITAL_STRUCTURE_RATIO";
			var dictionaries = _db.DashboardDictionaries.Where(x => x.Category == Type).ToList();
			var uniquedictionaries = dictionaries.Where(x => Periods.Contains(x.Dimesion)).ToList().DistinctByField(x => x.Dimesion);

			var data = dictionaries.ToList();

			foreach (var dic in uniquedictionaries)
			{
				var chart = new CapitalStructureRatioViewModel
				{
					Month = dic.Dimesion
				};

				var Debt_To_Assets_Ratio = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Debt_To_Assets_Ratio");
				if (Debt_To_Assets_Ratio != null) chart.Debt_To_Assets_Ratio = Math.Round((Debt_To_Assets_Ratio.Value / nth), 2);

				var Debt_To_Equity_Ratio = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Debt_To_Equity_Ratio");
				if (Debt_To_Equity_Ratio != null) chart.Debt_To_Equity_Ratio = Math.Round((Debt_To_Equity_Ratio.Value / nth), 2);



				list.Add(chart);
			}

			return list;
		}
		private async Task<List<ProfitabilityRatioViewModel>> GetProfitabilityRatiochartData()
		{
			nth = 1;
			FiscalYearHelper _FiscalYearHelper = new FiscalYearHelper();
			var periodList = await _FiscalYearHelper.GetRecentNPeriodsInYearAsync(6);
			var Periods = periodList.OrderBy(x => x.DateTo).Select(x => x.Name).ToList();
			List<ProfitabilityRatioViewModel> list = new List<ProfitabilityRatioViewModel>();
			string Type = "PROFITABILITY_RATIO";
			var dictionaries = _db.DashboardDictionaries.Where(x => x.Category == Type).ToList();
			var uniquedictionaries = dictionaries.Where(x => Periods.Contains(x.Dimesion)).ToList().DistinctByField(x => x.Dimesion);

			var data = dictionaries.ToList();

			foreach (var dic in uniquedictionaries)
			{
				var chart = new ProfitabilityRatioViewModel
				{
					Month = dic.Dimesion
				};

				var Cost_Of_Goods_Sold = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Cost_Of_Goods_Sold");
				if (Cost_Of_Goods_Sold != null) chart.Cost_Of_Goods_Sold = Math.Round((Cost_Of_Goods_Sold.Value / nth), 2);

				var Gross_Profit_Margin = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Gross_Profit_Margin");
				if (Gross_Profit_Margin != null) chart.Gross_Profit_Margin = Math.Round((Gross_Profit_Margin.Value / nth), 2);

				var Net_Profit_Margin = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Net_Profit_Margin");
				if (Net_Profit_Margin != null) chart.Net_Profit_Margin = Math.Round((Net_Profit_Margin.Value / nth), 2);


				var Operating_Profit_Margin = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Operating_Profit_Margin");
				if (Operating_Profit_Margin != null) chart.Operating_Profit_Margin = Math.Round((Operating_Profit_Margin.Value / nth), 2);

				var Return_On_Assets = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Return_On_Assets");
				if (Return_On_Assets != null) chart.Return_On_Assets = Math.Round((Return_On_Assets.Value / nth), 2);

				var Return_On_Equity = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Return_On_Equity");
				if (Return_On_Equity != null) chart.Return_On_Equity = Math.Round((Return_On_Equity.Value / nth), 2);


				list.Add(chart);
			}

			return list;
		}

		#endregion

		public ActionResult FinanceDashBoard()
		{
			return View();
		}

		public JsonResult RefreshFinanceDashboard()
		{
			_dashboardHelper.UpdateFinanceDocStatusDict();
			var formattedData = new
			{
				Message = "Update Complete.",
				Status = "OK"
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}


		public ActionResult FinanceRatioDashBoard()
		{
			return View();
		}       

		public JsonResult RefreshFinanceRatioDashboard()
		{
			_dashboardHelper.UpdateDashboardFinanceRatio();
			var formattedData = new
			{
				Message = "Update Complete.",
				Status = "OK"
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}    
	   
	}
}