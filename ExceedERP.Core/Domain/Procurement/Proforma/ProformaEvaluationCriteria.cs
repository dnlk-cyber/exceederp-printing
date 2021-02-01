using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Procurement.Tender;

namespace ExceedERP.Core.Domain.Procurement.Proforma
{
    public class ProformaEvaluationCriteria : TrackUserOperation
    {
        public int ProformaEvaluationCriteriaID { get; set; }
        public int ProformaPurchaseID { get; set; }
        public virtual ProformaPurchase ProformaPurchase { get; set; }
        [Required]
        public float Weight { get; set; }
        public string Description { get; set; }
        public CriteriaType Type { get; set;  }
    }
}
