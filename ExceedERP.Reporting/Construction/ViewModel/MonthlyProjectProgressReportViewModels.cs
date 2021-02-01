using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.ViewModel
{
    //class MonthlyProjectProgressReportViewModels
    //{
    //}
    public class DetailedWorkProgressSummarySubProjectTypeViewModel
    {
        public DetailedWorkProgressSummarySubProjectTypeViewModel()
        {
            Blocks = new List<DetailedWorkProgressSummaryBlockViewModel>();
        }
        public string Name { get; set; }
        public decimal TotalCost { get; set; }
        public List<DetailedWorkProgressSummaryBlockViewModel> Blocks { get; set; }
    }
    public class DetailedWorkProgressSummaryBlockViewModel
    {
        public DetailedWorkProgressSummaryBlockViewModel()
        {
            StructureList = new List<DetailedWorkProgressSummaryStructureViewModel>();
        }
        public string Name { get; set; }
        public decimal TotalCost { get; set; }

        public decimal PlannedAmountThisMonth { get; set; }
        public decimal ActualAccomplishedAmountThisMonth { get; set; }
        public decimal PercentageOfActualToPlannedAmountThisMonth { get; set; }

        public decimal PlannedAmountTillPrevMonth { get; set; }
        public decimal PlannedAmountToDate { get; set; }

        public decimal AccomplishedAmountTillPrevMonth { get; set; }
        public decimal AccomplishedAmountToDate { get; set; }
        public decimal PercentageOfActualToPlannedAmountToDate { get; set; }

        public decimal AccomplishedVsContractValueThisMonth { get; set; }
        public decimal AccomplishedVsContractValueToDate { get; set; }
        public List<DetailedWorkProgressSummaryStructureViewModel> StructureList { get; set; }
    }
    public class DetailedWorkProgressSummaryStructureViewModel
    {
        public DetailedWorkProgressSummaryStructureViewModel()
        {
            CategoryList = new List<DetailedWorkProgressSummaryCategoryViewModel>();
        }
        public string Name { get; set; }
        public decimal TotalCost { get; set; }
        public List<DetailedWorkProgressSummaryCategoryViewModel> CategoryList { get; set; }
    }
    public class DetailedWorkProgressSummaryCategoryViewModel
    {
        public DetailedWorkProgressSummaryCategoryViewModel()
        {
            WorkItems = new List<DetailedWorkProgressWorkItemViewModel>();
        }
        public int OrderIndex { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public decimal ContractValue { get; set; }
        public decimal PlannedToAccomplishThisMonth { get; set; }
        public decimal ActualAccomplishedThisMonth { get; set; }
        public decimal PercentageOfActualToPlannedThisMonth { get; set; }

        public decimal PlannedToAccomplishTillPrevMonth { get; set; }
        public decimal PlannedToAccomplishToDate { get; set; }

        public decimal ActualWorkAccomplishedTillPrevMonth { get; set; }
        public decimal ActualWorkAccomplishedToDate { get; set; }
        public decimal PercentageOfActualToPlannedTillPrevMonth { get; set; }

        public decimal RatioOfActualAccomplishedToContractValueThisMonth { get; set; }
        public decimal RatioOfActualAccmoplishedToContractValueToDate { get; set; }

        public string Unit { get; set; }
        public decimal QuantityOfWorkAccomplishedThisMonth { get; set; }
        public decimal QuantityOfWorkAccomplishedTillPrevMonth { get; set; }
        public decimal QuantityOfWorkAccomplishedToDate { get; set; }

        public List<DetailedWorkProgressWorkItemViewModel> WorkItems { get; set; }

    }

    public class DetailedWorkProgressWorkItemViewModel
    {
        public string ItemCode { get; set; }
        public bool HasChildItems { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public decimal ContractQuantity { get; set; }
        public decimal ActualTakeOffQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal PreviousPlannedQuantity { get; set; }
        public decimal ToDatePlannedQuantity { get; set; }
        public decimal ToDateActualQuantity { get; set; }
        public decimal TotalAccVsContractQuantity { get; set; }

        public decimal QuantityOfWorkPlannedThisMonth { get; set; }
        public decimal QuantityOfWorkExecutedTillPrevMonth { get; set; }
        public decimal QuantityOfWorkExcutedThisMonth { get; set; }
        public decimal ExcVsPlanThisMonth { get; set; }

        public decimal CostOfWorkTillPrevMonth { get; set; }
        public decimal CostOfWorkThisMonth { get; set; }
        public decimal CostOfWorkToDate { get; set; }
    }
    public class WorkProgressReportPeriodViewModel
    {
        public int PeriodId { get; set; }
        public string  Name { get; set; }

    }
    // miscellaneous of Expenses
    public class MiscellaneousExpenseViewModel
    {
        public string Description { get; set; }
        public decimal PreviousAmount { get; set; }
        public decimal ThisMonthAmount { get; set; }
        public decimal ToDateAmount { get; set; }
        public string Remark { get; set; }
    }

    // indirect fuel and lubricant cost
    public class IndirectFuelAndLubCostViewModel
    {
        public string Description { get; set; }
        public string Unit { get; set; }
        public decimal  UnitPrice { get; set; }
        public decimal PreviousQuantity { get; set; }
        public decimal ThisMonthQuantity { get; set; }
        public decimal ToDateQuantity { get; set; }
        public decimal PreviousAmount { get; set; }
        public decimal ThisMonthAmount { get; set; }
        public decimal ToDateAmount { get; set; }
        public string Remark { get; set; }
    }

    // direct fuel and lubricant cost
    public class DirectFuelAndLubCostViewModel
    {
        public string Description { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal PreviousQuantity { get; set; }
        public decimal ThisMonthQuantity { get; set; }
        public decimal ToDateQuantity { get; set; }
        public decimal PreviousAmount { get; set; }
        public decimal ThisMonthAmount { get; set; }
        public decimal ToDateAmount { get; set; }
        public string Remark { get; set; }
    }
}
