using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.Report.IFRS
{
    public class IfrsStatementOfCashFlow
    {
        public int IfrsStatementOfCashFlowId { get; set; }
        public virtual ICollection<IfrsSocfOperatingActivity> OperatingActivities { get; set; }
        public virtual ICollection<IfrsSocfInvestingActivity> InvestingActivities { get; set; }
        public virtual ICollection<IfrsSocfFinancingActivity> FinancingActivities { get; set; }
        public virtual ICollection<IfrsCashAndCashEquivalentRange> IfrsCashAndCashEquivalentRanges { get; set; }

    }

    public class IfrsSocfOperatingActivity 
    {
        public int IfrsSocfOperatingActivityId { get; set; }
        public int IfrsStatementOfCashFlowId { get; set; }
        public virtual IfrsStatementOfCashFlow IfrsStatementOfCashFlow { get; set; }
        [Required]
        public IfrsCashFlowTypes Category { get; set; }
        [Required]
        public string Description { get; set; }
        [Display(Name = "Note Number")]
        public string NoteNumber { get; set; }
        public IfrsCashFlowRule Rule { get; set; }
        public virtual ICollection<IfrsSocfOperatingActivityRange> IfrsSocfOperatingActivityRanges { get; set; }
    }

    public class IfrsSocfOperatingActivityRange : ReportSetupBase
    {
        public int IfrsSocfOperatingActivityRangeId { get; set; }
        public int IfrsSocfOperatingActivityId { get; set; }
        public virtual IfrsSocfOperatingActivity IfrsSocfOperatingActivity { get; set; }
    }

    public class IfrsSocfInvestingActivity
    {
        public int IfrsSocfInvestingActivityId { get; set; }
        public int IfrsStatementOfCashFlowId { get; set; }
        public virtual IfrsStatementOfCashFlow IfrsStatementOfCashFlow { get; set; }
        [Required]
        public IfrsCashFlowTypes Category { get; set; }
        [Required]
        public string Description { get; set; }
        [Display(Name = "Note Number")]
        public string NoteNumber { get; set; }
        public IfrsCashFlowRule Rule { get; set; }
        public virtual ICollection<IfrsSocfInvestingActivityRange> IfrsSocfInvestingActivityRanges { get; set; }
    }
    public class IfrsSocfInvestingActivityRange : ReportSetupBase
    {
        public int IfrsSocfInvestingActivityRangeId { get; set; }
        public int IfrsSocfInvestingActivityId { get; set; }
        public virtual IfrsSocfInvestingActivity IfrsSocfInvestingActivity { get; set; }
    }

    public class IfrsSocfFinancingActivity 
    {
        public int IfrsSocfFinancingActivityId { get; set; }
        public int IfrsStatementOfCashFlowId { get; set; }
        public virtual IfrsStatementOfCashFlow IfrsStatementOfCashFlow { get; set; }
        [Required]
        public IfrsCashFlowTypes Category { get; set; }
        [Required]
        public string Description { get; set; }
        [Display(Name = "Note Number")]
        public string NoteNumber { get; set; }
        public IfrsCashFlowRule Rule { get; set; }
        public virtual ICollection<IfrsSocfFinancingActivityRange> IfrsSocfFinancingActivityRanges { get; set; }
    }
    public class IfrsSocfFinancingActivityRange : ReportSetupBase
    {
        public int IfrsSocfFinancingActivityRangeId { get; set; }
        public int IfrsSocfFinancingActivityId { get; set; }
        public virtual IfrsSocfFinancingActivity IfrsSocfFinancingActivity { get; set; }
      
    }

    public class IfrsCashAndCashEquivalentRange : ReportSetupBase
    {
        public int IfrsCashAndCashEquivalentRangeId { get; set; }
        public int IfrsStatementOfCashFlowId { get; set; }
        public virtual IfrsStatementOfCashFlow IfrsStatementOfCashFlow { get; set; }

    }
}
