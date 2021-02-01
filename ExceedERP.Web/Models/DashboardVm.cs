using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExceedERP.Web.Models
{
    public class ProfitabilityRatioViewModel
    {

        public int DashboardDictionaryId { get; set; }
        public string Month { get; set; }
        public decimal Gross_Profit_Margin { get; set; }
        public decimal Operating_Profit_Margin { get; set; }
        public decimal Net_Profit_Margin { get; set; }

        public decimal Return_On_Assets { get; set; }
        public decimal Return_On_Equity { get; set; }
        public decimal Cost_Of_Goods_Sold { get; set; }
    }

    public class ActivityRatioViewModel
    {

        public int DashboardDictionaryId { get; set; }
        public string Month { get; set; }
        public decimal Inventory_TurnOver { get; set; }
        public decimal Receivables_TurnOver { get; set; }
        public decimal Fixed_Asset_Turnover { get; set; }

        public decimal Asset_TurnOver { get; set; }
        public decimal Average_Collection_Period { get; set; }
    }


    public class LiquidityRatioViewModel
    {

        public int DashboardDictionaryId { get; set; }
        public string Month { get; set; }
        public decimal Current_Ratio { get; set; }
        public decimal Quick_Ratio { get; set; }
        public decimal Cash_Ratio { get; set; }
        public decimal Net_Working_Capital_Ratio { get; set; }

    }


    public class CapitalStructureRatioViewModel
    {

        public int DashboardDictionaryId { get; set; }
        public string Month { get; set; }
        public decimal Debt_To_Assets_Ratio { get; set; }
        public decimal Debt_To_Equity_Ratio { get; set; }

    }


}