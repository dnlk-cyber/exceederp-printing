using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.Report.IFRS
{
    public class IfrsStatementOfChangesInEquity
    {
        public int IfrsStatementOfChangesInEquityId { get; set; }
        public virtual ICollection<IfrsSofpEquityAndLiablity> EquityAndLiablity { get; set; }

        [Required]
        public IfrsStatementOfChangesInEquityTypes Category { get; set; }

        [Display(Name = "Note Number")]
        public string NoteNumber { get; set; }

        public virtual ICollection<IfrsStatementOfChangesInEquityRange> IfrsStatementOfChangesInEquityRanges { get; set; }
    }

    public class IfrsStatementOfChangesInEquityRange : ReportSetupBase
    {
        public int IfrsStatementOfChangesInEquityRangeId { get; set; }
        public int IfrsStatementOfChangesInEquityId { get; set; }
        public virtual IfrsStatementOfChangesInEquity IfrsStatementOfChangesInEquity { get; set; }
    }

}
