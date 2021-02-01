using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.Report.GAAP
{
    public class GaapBalanceSheet
    {
        public int GaapBalanceSheetId { get; set; }
        public virtual ICollection<GaapBalanceSheetAsset> Assets { get; set; }
        public virtual ICollection<GaapBalanceSheetLiability> Liabilities { get; set; }
        public virtual ICollection<GaapBalanceSheetCapital> Capitals { get; set; }
        public GaapReportType ReportType { get; set; }
    }

    public class GaapBalanceSheetAsset
    {
        public int GaapBalanceSheetAssetId { get; set; }
        public int GaapBalanceSheetId { get; set; }
        public virtual GaapBalanceSheet GaapBalanceSheet { get; set; }
        [Required]
        public GaapAssetTypes Category { get; set; }
        [Required]
        public string Description { get; set; }
        [Display(Name = "Note Number")]
        public string NoteNumber { get; set; }
        public virtual ICollection<GaapBalanceSheetAssetRange> GaapBalanceSheetAssetRanges { get; set; }
    }

    public class GaapBalanceSheetAssetRange : ReportSetupBase
    {
        public int GaapBalanceSheetAssetRangeId { get; set; }
        public int GaapBalanceSheetAssetId { get; set; }
        public virtual GaapBalanceSheetAsset Asset { get; set; }
    }

    public class GaapBalanceSheetLiability
    {
        public int GaapBalanceSheetLiabilityId { get; set; }
        public int GaapBalanceSheetId { get; set; }
        public virtual GaapBalanceSheet GaapBalanceSheet { get; set; }
        [Required]
        public GaapLiablityTypes Category { get; set; }
        [Required]
        public string Description { get; set; }
        [Display(Name = "Note Number")]
        public string NoteNumber { get; set; }
        public virtual ICollection<GaapBalanceSheetLiabilityRange> GaapBalanceSheetLiabilityRanges { get; set; }
    }

    public class GaapBalanceSheetLiabilityRange : ReportSetupBase
    {
        public int GaapBalanceSheetLiabilityRangeId { get; set; }
        public int GaapBalanceSheetLiabilityId { get; set; }
        public virtual GaapBalanceSheetLiability Liability { get; set; }
    }

    public class GaapBalanceSheetCapital
    {
        public int GaapBalanceSheetCapitalId { get; set; }
        public int GaapBalanceSheetId { get; set; }
        public virtual GaapBalanceSheet GaapBalanceSheet { get; set; }
        [Required]
        public GaapEquityTypes Category { get; set; }
        [Required]
        public string Description { get; set; }
        [Display(Name = "Note Number")]
        public string NoteNumber { get; set; }
        public virtual ICollection<GaapBalanceSheetCapitalRange> GaapBalanceSheetCapitalRanges { get; set; }
    }

    public class GaapBalanceSheetCapitalRange : ReportSetupBase
    {
        public int GaapBalanceSheetCapitalRangeId { get; set; }
        public int GaapBalanceSheetCapitalId { get; set; }
        public virtual GaapBalanceSheetCapital Capital { get; set; }
    }


}
