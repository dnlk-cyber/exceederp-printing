using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Tender.Lot
{
    public class LotRequiredDocument : TrackUserOperation
    {
        public int LotRequiredDocumentId { get; set; }
        public int LotID { get; set; }
        [Display(Name = "Document Name")]
        public string Document { get; set; }
        [Display(Name = "Is Mandatory ?")]
        public bool Mandatory { get; set; }
        public string Remark { get; set; }
    }
}