using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance
{
    public enum RatioCategory
    {
        Profitability,
        Efficiency,
        Liquidity,
        Leverage,
        Activity,
        Investor,
    }
    public class FinancialRatio : TrackUserOperation
    {
        public int FinancialRatioId { get; set; }
        public RatioCategory Category { get; set; }
        public string Name { get; set; }
        public float Minimum { get; set; }
        public float Maximum { get; set; }
        public float Average { get; set; }
        [Display(Name = "Enable ?")]
        public bool Enable { get; set; }
    }
}
