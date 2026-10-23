using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Finance.Budget.ViewModel
{
    public class BudgetCashFlowVm
    {

        public List<BudgetCashInFlowVm> InFlowVm { get; set; } = new List<BudgetCashInFlowVm>();
        public List<BudgetCashOutFlowVm> OutFlowVm { get; set; } = new List<BudgetCashOutFlowVm>();
        public List<BudgetCasFlowDiffVm> Diff { get; set; } = new List<BudgetCasFlowDiffVm>();

    }

    public class BudgetCashInFlowVm
    {

        public string AccountCode { get; set; }
        public string AccountDesc { get; set; }
        [Display(Name = "July")]
        public decimal Period1 { get; set; }
        [Display(Name = "August")]
        public decimal Period2 { get; set; }
        [Display(Name = "September")]
        public decimal Period3 { get; set; }
        [Display(Name = "Octobeber")]
        public decimal Period4 { get; set; }
        [Display(Name = "November")]
        public decimal Period5 { get; set; }
        [Display(Name = "December")]
        public decimal Period6 { get; set; }
        [Display(Name = "January")]
        public decimal Period7 { get; set; }
        [Display(Name = "February")]
        public decimal Period8 { get; set; }
        [Display(Name = "March")]
        public decimal Period9 { get; set; }
        [Display(Name = "April")]
        public decimal Period10 { get; set; }
        [Display(Name = "May")]
        public decimal Period11 { get; set; }
        [Display(Name = "June")]
        public decimal Period12 { get; set; }
        public decimal Total { get; set; }
        public void SetPeriodBudget(int period, decimal value)
        {
            switch (period)
            {
                case 1: Period1 = value; break;
                case 2: Period2 = value; break;
                case 3: Period3 = value; break;
                case 4: Period4 = value; break;
                case 5: Period5 = value; break;
                case 6: Period6 = value; break;
                case 7: Period7 = value; break;
                case 8: Period8 = value; break;
                case 9: Period9 = value; break;
                case 10: Period10 = value; break;
                case 11: Period11 = value; break;
                case 12: Period12 = value; break;
                default: throw new ArgumentOutOfRangeException(nameof(period), "Invalid period number");
            }
        }

    }
    public class BudgetCashOutFlowVm
    {

        public string AccountCode { get; set; }
        public string AccountDesc { get; set; }
        [Display(Name = "July")]
        public decimal Period1 { get; set; }
        [Display(Name = "August")]
        public decimal Period2 { get; set; }
        [Display(Name = "September")]
        public decimal Period3 { get; set; }
        [Display(Name = "Octobeber")]
        public decimal Period4 { get; set; }
        [Display(Name = "November")]
        public decimal Period5 { get; set; }
        [Display(Name = "December")]
        public decimal Period6 { get; set; }
        [Display(Name = "January")]
        public decimal Period7 { get; set; }
        [Display(Name = "February")]
        public decimal Period8 { get; set; }
        [Display(Name = "March")]
        public decimal Period9 { get; set; }
        [Display(Name = "April")]
        public decimal Period10 { get; set; }
        [Display(Name = "May")]
        public decimal Period11 { get; set; }
        [Display(Name = "June")]
        public decimal Period12 { get; set; }
        public decimal Total { get; set; }
        public void SetPeriodBudget(int period, decimal value)
        {
            switch (period)
            {
                case 1: Period1 = value; break;
                case 2: Period2 = value; break;
                case 3: Period3 = value; break;
                case 4: Period4 = value; break;
                case 5: Period5 = value; break;
                case 6: Period6 = value; break;
                case 7: Period7 = value; break;
                case 8: Period8 = value; break;
                case 9: Period9 = value; break;
                case 10: Period10 = value; break;
                case 11: Period11 = value; break;
                case 12: Period12 = value; break;
                default: throw new ArgumentOutOfRangeException(nameof(period), "Invalid period number");
            }
        }

    }
    public class BudgetCasFlowDiffVm
    {
        [Display(Name = "July")]
        public decimal Period1 { get; set; }
        [Display(Name = "August")]
        public decimal Period2 { get; set; }
        [Display(Name = "September")]
        public decimal Period3 { get; set; }
        [Display(Name = "Octobeber")]
        public decimal Period4 { get; set; }
        [Display(Name = "November")]
        public decimal Period5 { get; set; }
        [Display(Name = "December")]
        public decimal Period6 { get; set; }
        [Display(Name = "January")]
        public decimal Period7 { get; set; }
        [Display(Name = "February")]
        public decimal Period8 { get; set; }
        [Display(Name = "March")]
        public decimal Period9 { get; set; }
        [Display(Name = "April")]
        public decimal Period10 { get; set; }
        [Display(Name = "May")]
        public decimal Period11 { get; set; }
        [Display(Name = "June")]
        public decimal Period12 { get; set; }
        public decimal Total { get; set; }
       

    }
}
