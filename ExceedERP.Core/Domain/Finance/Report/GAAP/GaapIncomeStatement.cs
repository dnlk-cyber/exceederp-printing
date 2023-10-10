using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.Report.GAAP
{
    public class GaapIncomeStatement
    {
        public int GaapIncomeStatementId { get; set; }
        public virtual ICollection<GaapIncomeStatementOperationRevenue> Revenues { get; set; }
        public virtual ICollection<GaapIncomeStatementOperationExpense> Expenses { get; set; }
        public GaapReportType ReportType { get; set; }
        
    }

    public class GaapIncomeStatementOperationRevenue
    {
        public int GaapIncomeStatementOperationRevenueId { get; set; }
        public int GaapIncomeStatementId { get; set; }
        public virtual GaapIncomeStatement GaapIncomeStatement { get; set; }
        [Required]
        public IncomeStatementRevenueCategory Category { get; set; }
        [Required]
        public string Description { get; set; }
        [Display(Name = "Note Number")]
        public string NoteNumber { get; set; }
        public virtual ICollection<GaapIncomeStatementOperationRevenueRange> GaapIncomeStatementOperationRevenueRanges { get; set; }
    }

    public class GaapIncomeStatementOperationRevenueRange : ReportSetupBase
    {
        public int GaapIncomeStatementOperationRevenueRangeId { get; set; }
        public int GaapIncomeStatementOperationRevenueId { get; set; }
        public virtual GaapIncomeStatementOperationRevenue GaapIncomeStatementOperationRevenue { get; set; }
    }

    public class GaapIncomeStatementOperationExpense
    {
        public int GaapIncomeStatementOperationExpenseId { get; set; }
        public int GaapIncomeStatementId { get; set; }
        public virtual GaapIncomeStatement GaapIncomeStatement { get; set; }
        [Required]
        public IncomeStatementExpenseCategory Category { get; set; }
        [Required]
        public string Description { get; set; }
        [Display(Name = "Note Number")]
        public string NoteNumber { get; set; }
        public virtual ICollection<GaapIncomeStatementOperationExpenseRange> GaapIncomeStatementOperationExpenseRanges { get; set; }
    }

    public class GaapIncomeStatementOperationExpenseRange : ReportSetupBase
    {
        public int GaapIncomeStatementOperationExpenseRangeId { get; set; }
        public int GaapIncomeStatementOperationExpenseId { get; set; }
        public virtual GaapIncomeStatementOperationExpense GaapIncomeStatementOperationExpense { get; set; }
    }


}
