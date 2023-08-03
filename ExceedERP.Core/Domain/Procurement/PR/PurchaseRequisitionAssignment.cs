using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Procurement.Settings;

namespace ExceedERP.Core.Domain.Procurement.PR
{
    public class PurchaseRequisitionAssignment : Operations
    {
        public int PurchaseRequisitionAssignmentId { get; set; }
        public int PurchaseRequisitionID { get; set; }
        public virtual PurchaseRequisition PurchaseRequisition { get; set; }
        [Display(Name = "Procurement Officer")]
        public int ProcurementOfficer { get; set; }
        [Display(Name = "Method")]
        public ProcurementMethod ProcurementMethod { get; set; }
        [Display(Name = "Category")]
        public PurchaseCategory PurchaseCategory { get; set; }
        [Display(Name = "Tender Type")]
        public TenderType TenderType { get; set; }
        public virtual ICollection<PurchaseRequisitionReassignmentItem> PurchaseRequisitionReassignmentItems { get; set; }
        public virtual ICollection<PurchaseRequisitionReassignmentValidation> PurchaseRequisitionReassignmentValidations { get; set; }
    }
}