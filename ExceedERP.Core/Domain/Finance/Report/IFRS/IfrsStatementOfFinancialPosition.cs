using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.Report.IFRS
{
    public class IfrsStatementOfFinancialPosition
    {
        public int IfrsStatementOfFinancialPositionId { get; set; }
        public virtual ICollection<IfrsSofpAsset> Asstets { get; set; }
        public virtual ICollection<IfrsSofpEquityAndLiablity> EquityAndLiablity { get; set; }
    }

    public class IfrsSofpAsset  
    {
        public int IfrsSofpAssetId { get; set; }
        public int IfrsStatementOfFinancialPositionId { get; set; }
        public virtual IfrsStatementOfFinancialPosition IfrsStatementOfFinancialPosition { get; set; }
        [Required]
        public IfrsSofpAssetTypes Category { get; set; }
        [Required]
        public string Description { get; set; }
        [Display(Name = "Note Number")]
        public string NoteNumber { get; set; }
        public virtual ICollection<IfrsSofpAssetRange> IfrsSofpAssetRanges { get; set; }
    }

    public class IfrsSofpAssetRange : ReportSetupBase
    {
        public int IfrsSofpAssetRangeId { get; set; }
        public int IfrsSofpAssetId { get; set; }
        public virtual IfrsSofpAsset IfrsSofpAsset { get; set; }
    }

    public class IfrsSofpEquityAndLiablity
    {
        public int IfrsSofpEquityAndLiablityId { get; set; }
        public int IfrsStatementOfFinancialPositionId { get; set; }
        public virtual IfrsStatementOfFinancialPosition IfrsStatementOfFinancialPosition { get; set; }
        [Required]
        public IfrsSofpEquityAndLiablityTypes Category { get; set; }
        [Required]
        public string Description { get; set; }
        [Display(Name = "Note Number")]
        public string NoteNumber { get; set; }
        public virtual ICollection<IfrsSofpEquityAndLiablityRange> IfrsSofpEquityAndLiablityRanges { get; set; }
    }

    public class IfrsSofpEquityAndLiablityRange : ReportSetupBase
    {
        public int IfrsSofpEquityAndLiablityRangeId { get; set; }
        public int IfrsSofpEquityAndLiablityId { get; set; }
        public virtual IfrsSofpEquityAndLiablity IfrsSofpEquityAndLiablity { get; set; }
    }
}
