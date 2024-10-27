using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Procurement.Settings;

namespace ExceedERP.Core.Domain.Procurement.Proforma
{

    public class ProformaPurchase : NewOperations
    {
        public int ProformaPurchaseID { get; set; }
        [Required]
        [Display(Name = "Reference")]
        public string ProformaReference { get; set; }
        [Required]
        public string Description { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd :hh mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Float Date")]
        public DateTime FloatDate { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd :hh mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Closing Date")]
        public DateTime ClosingDate { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd :hh mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Evaluation Date")]
        public DateTime EvaluationDate { get; set; }
        [Display(Name = "Store Requisition Number")]
        public int? StoreRequisitionID { get; set; }
        [DisplayName("Purchase Requisition")]
        public int? PurchaseRequisitionID { get; set; }
        [DisplayName("PR Assignment")]
        public int PurchaseRequisitionAssignmentId { get; set; }
        [Display(Name = "Category")]
        public PurchaseCategory PurchaseCategory { get; set; }
        [Display(Name = "Procurement Officer")]
        public int ProcurementOfficer { get; set; }
        [DisplayName("Is Fullfilled")]
        public bool IsFullfilled { get; set; }
        [DisplayName("Purchase Status")]
        public PurchaseStatus PurchaseStatus { get; set; }
        [Display(Name = "Evaluate Total Finance")]
        public bool EvaluateTotalFinance { get; set; }
        [Display(Name = "Branch/Project")]
        public int BranchId { get; set; }
        public int BusinessUnitId { get; set; }
        public virtual ICollection<ProformaPurchaseRequisition> ProformaPurchaseRequisitions { get; set; }
        public virtual ICollection<ProformaItem> ProformaItem { get; set; }
        public virtual ICollection<RequestedSupplier> ProformaRequestedSuppliers { get; set; }
        public virtual ICollection<ParticipatingSupplier> ProformaParticipatingSuppliers { get; set; }
        public virtual ICollection<WinningSupplier> ProformaWinningSupplier { get; set; }
        public virtual ICollection<ProformaEvaluationCriteria> ProformaEvaluationCriteria { get; set; }
        public virtual ICollection<ProformaEvaluationResult> ProformaEvaluationResult { get; set; }
        public virtual ICollection<PurchaseValidation> PurchaseValidation { get; set; }
        public virtual ICollection<ProformaUploadDbModel> ProformaUploadDbModel { get; set; }
        public virtual ICollection<ProformaTeam> ProformaTeams { get; set; }
    }
    public class ProformaUploadDbModel
    {
        public int ProformaUploadDbModelId { get; set; }
        public int ProformaPurchaseID { get; set; }
        public virtual ProformaPurchase ProformaPurchase { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }


}
