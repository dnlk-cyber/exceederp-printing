using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CRM
{
    public   class InvestmentType : TrackUserOperation
    {
        [Key]
        public int InvestmentTypeId { get; set; }
        public string InvestmentName { get; set; }
        public string Description { get; set; }
    }
}
