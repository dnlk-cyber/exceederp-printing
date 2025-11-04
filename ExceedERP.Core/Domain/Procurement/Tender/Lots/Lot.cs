using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;

namespace ExceedERP.Core.Domain.Procurement.Tender.Lots
{
    public class Lot : TrackUserOperation
    {
        public int LotID { get; set; }
        [Display(Name = "Tender")]
        public int TenderID { get; set; }
        public virtual Tender Tender { get; set; }
        [Display(Name = "Lot Number")]
        public int LotNumber { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        [Display(Name = "Evaluate Total Finance")]
        public bool EvaluateTotalFinance { get; set; }
        public virtual ICollection<LotPurchaseRequisition> LotPurchaseRequisitions { get; set; }
        public virtual ICollection<LotItems> LotItems { get; set; }
        public virtual ICollection<BidPurchasingSuppliers> BidPurchasingSuppliers { get; set; }
        public virtual ICollection<BidPParticipatingSuppliers> BidPParticipatingSuppliers { get; set; }
        public virtual ICollection<LotRequiredDocument> LotRequiredDocuments { get; set; }
        public virtual ICollection<LotEvaluationByDocument> LotEvaluationByDocuments { get; set; }
        public virtual ICollection<TenderEvaluationCriteria> EvaluationCriterias { get; set; }
        public virtual ICollection<TenderEvaluationResult> EvaluationResults { get; set; }
        public virtual ICollection<BidAwardedSupplier> BidAwardedSupplier { get; set; }
        public virtual ICollection<LotFinancialOffer> LotFinancialOffer { get; set; }
        public virtual ICollection<ParticipantsItem> ParticipantsItems { get; set; }
        public virtual ICollection<PassFailResult> TenderPassFailResults { get; set; }
        public virtual ICollection<TenderTechnical> TenderTechnicals { get; set; }
    }

    
}
