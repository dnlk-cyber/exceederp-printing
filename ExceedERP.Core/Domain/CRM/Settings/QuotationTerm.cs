using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CRM
{
    public class QuotationTerm : TrackUserOperation
    {
        public int QuotationTermId { get; set; }

        public string Desription { get; set; }
        public string Remark { get; set; }
        [Display(Name ="Quotation (in days)")]
        public int QuotationInDays { get; set; }

    }
}
