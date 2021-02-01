using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.Report.IFRS
{
    public class IfrsStatementOfComprehensiveIncome
    {
        public int IfrsStatementOfComprehensiveIncomeId { get; set; }
        public virtual ICollection<IfrsScoiContinuingOperation> ContinuingOperations { get; set; }
        public virtual ICollection<IfrsScoiDiscontinuedOperation> DiscontinuedOperations { get; set; }
        public virtual ICollection<IfrsScoiOtherComprehensiveIncome> OtherComprehensiveIncomes { get; set; }
    }

    public class IfrsScoiContinuingOperation
    {
        public int IfrsScoiContinuingOperationId { get; set; }
        public int IfrsStatementOfComprehensiveIncomeId { get; set; }
        public virtual IfrsStatementOfComprehensiveIncome IfrsStatementOfComprehensiveIncome { get; set; }
        [Required]
        public IfrsContinuedOpRevenueExpense Category { get; set; }
        [Required]
        public string Description { get; set; }
         [Display(Name = "Note Number")]
        public string NoteNumber { get; set; }
        public virtual ICollection<IfrsScoiContinuingOperationRange> IfrsScoiContinuingOperationRanges { get; set; }
    }

    public class IfrsScoiContinuingOperationRange : ReportSetupBase
    {
        public int IfrsScoiContinuingOperationRangeId { get; set; }
        public int IfrsScoiContinuingOperationId { get; set; }
        public virtual IfrsScoiContinuingOperation IfrsScoiContinuingOperation { get; set; }
    }

    public class IfrsScoiDiscontinuedOperation  
    {
        public int IfrsScoiDiscontinuedOperationId { get; set; }
        public int IfrsStatementOfComprehensiveIncomeId { get; set; }
        public virtual IfrsStatementOfComprehensiveIncome IfrsStatementOfComprehensiveIncome { get; set; }
        [Required]
        public string Description { get; set; }
        [Display(Name = "Note Number")]
        public string NoteNumber { get; set; }
        public virtual ICollection<IfrsScoiDiscontinuedOperationRange> IfrsScoiDiscontinuedOperationRanges { get; set; }
    }

    public class IfrsScoiDiscontinuedOperationRange : ReportSetupBase
    {
        public int IfrsScoiDiscontinuedOperationRangeId { get; set; }
        public int IfrsScoiDiscontinuedOperationId { get; set; }
        public virtual IfrsScoiDiscontinuedOperation IfrsScoiDiscontinuedOperation { get; set; }
    }

    public class IfrsScoiOtherComprehensiveIncome 
    {
        public int IfrsScoiOtherComprehensiveIncomeId { get; set; }
        public int IfrsStatementOfComprehensiveIncomeId { get; set; }
        public virtual IfrsStatementOfComprehensiveIncome IfrsStatementOfComprehensiveIncome { get; set; }
        [Required]
        public string Description { get; set; }
        [Display(Name = "Note Number")]
        public string NoteNumber { get; set; }
        public virtual ICollection<IfrsScoiOtherComprehensiveIncomeRange> IfrsScoiOtherComprehensiveIncomeRanges { get; set; }
    }

    public class IfrsScoiOtherComprehensiveIncomeRange : ReportSetupBase
    {
        public int IfrsScoiOtherComprehensiveIncomeRangeId { get; set; }
        public int IfrsScoiOtherComprehensiveIncomeId { get; set; }
        public virtual IfrsScoiOtherComprehensiveIncome IfrsScoiOtherComprehensiveIncome { get; set; }
    }
}
