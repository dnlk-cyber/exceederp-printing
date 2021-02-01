using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Procurement.Tender
{
    public enum CriteriaType
    {
        Financial = 0, Technical = 1, Pass_Fail = 2
    }
    public class TenderEvaluationCriteria : TrackUserOperation
    {
        public int TenderEvaluationCriteriaID { get; set; }
        public int LotID { get; set; }
        public virtual Lot.Lot Lot { get; set; }
        [Required]
        public float Weight { get; set; }
        public string Description { get; set; }
        public CriteriaType Type { get; set; }
    }
}
