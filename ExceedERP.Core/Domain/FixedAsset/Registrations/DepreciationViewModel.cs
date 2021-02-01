using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Registrations
{
    public class  DepreciationViewModel : TrackUserOperation
    {
        public string Branch { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Tag { get; set; }
        public string Manufacturer { get; set; }
        [Display(Name = "Receive Reference")]
        public string ReceiveReference { get; set; }
        [Display(Name = "Issue Reference")]
        public string IssueReference { get; set; }
        [Display(Name = "Invoice No")]
        public string InvoiceNo { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Purchase Date")]
        public DateTime PurchaseDate { get; set; }
        [Display(Name = "Original Value")]
        public decimal OriginalValue { get; set; }
        [Display(Name = "Depreciation Amount")]
        public decimal DepreciationAmount { get; set; }
        [Display(Name = "Accumulated AnnualDepreciation")]
        public decimal AccumulatedDepreciation { get; set; }
        [Display(Name = "Book Value")]
        public decimal BookValue { get; set; }
        [Display(Name = "Fiscal Year")]
        public int GlFiscalYearId { get; set; }
    }
}
