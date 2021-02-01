using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Tender.Lot
{
    public class LotEvaluationByDocument : TrackUserOperation
    {
        public int LotEvaluationByDocumentId { get; set; }
        public int LotID { get; set; }
        public virtual Lot Lot { get; set; }
        [Display(Name = "Supplier ID")]//supplier database
        public string SupplierId { get; set; }
        [Display(Name = "New Supplier")]//manual supplier
        public bool IsNewSupplier { get; set; }
        [Display(Name = "Supplier Name")]//manual supplier
        public string SupplierName { get; set; }
        [Display(Name = "Document Type")]
        public string Document { get; set; }
        [Display(Name = "Not Available?")]
        public bool IsAvailable { get; set; }
        public string Remark { get; set; }
    }
}