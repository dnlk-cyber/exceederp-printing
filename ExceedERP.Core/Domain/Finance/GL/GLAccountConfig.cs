using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.GL
{

    public enum AccountCategories
    {
        TradeReceivable,
        AdvanceReceivable,
        TradePayable,
        AdvancePayable, 
    }
   
    public class GLAccountConfig : TrackUserOperation
    {   
        public int GLAccountConfigId { get; set; }
        [Display(Name = "Category")]
        public AccountCategories Category { get; set; }
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }        
        public string Description { get; set; }
    }
   
}
