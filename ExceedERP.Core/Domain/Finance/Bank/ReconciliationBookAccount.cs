using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.Bank
{
   
    public class ReconciliationBookAccount : TrackUserOperation
    {
        public int ReconciliationBookAccountId { get; set; }
        [Required]
        public string AccountCode { get; set; }
        [Required]
        [Display(Name = "Type")]
        public string Type { get; set; }
        public string ReferenceDoc { get; set; }
        [Required]
        public string Reference { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string AdditionalDescription { get; set; }//comes from distribution line
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }
        public bool Reconciled { get; set; }
        public bool Check { get; set; }
        //ap ,ar,or gl
        public string Source { get; set; }
        public string SourceID { get; set; }
        public RecordType RecordType { get; set; }
        public string AccountSegment { get; set; }
        public string BranchSegment { get; set; }
        public string CostCenterSegment { get; set; }
        public string SubAccountSegment { get; set; }
        public string ChannelSegment { get; set; }
        public string BusinessUnitSegment { get; set; }
        public string Segment1 { get; set; }
        public string Segment2 { get; set; }
        public string Segment3 { get; set; }
        public string Segment4 { get; set; }
        public string Segment5 { get; set; }
        public string Segment6 { get; set; }
        public string Segment7 { get; set; }
        public string Segment8 { get; set; }
    }
}
