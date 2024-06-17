using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Planning.BusinessPlanDevelopment
{
    public class ShareAndDividend: TrackUserSettingOperation
    {
        public int ShareAndDividendId { get; set; }
        [Display(Name = "Share Company Name")]
        public string ShareCompanyName { get; set; }
        [Display(Name = "No of Share")]
        public decimal SubscribedShareAmtNoofShare { get; set; }
        [Display(Name = "Birr")]
        public decimal SubscribedShareAmtBirr { get; set; }
        [Display(Name = "No of Share")]
        public decimal PaidUpShareAmtNoOfShare { get; set; }
        [Display(Name = "Birr")]
        public decimal PaidUpShareAmtBirr { get; set; }
        public decimal RemainingShare { get; set; }
        [Display(Name = "Fiscal Year")]
        public int GlFiscalYearId { get; set; }
        public GlFiscalYear GlFiscalYear { get; set; }

    }

    public class ShareAndDividendFiscalYearDetail
    {
        public int ShareAndDividendFiscalYearDetailId { get; set; }
        [Display(Name = "Plan to pay share")]
        public decimal PlanToPayShare { get; set; }
        [Display(Name = "Paid share")]
        public decimal PaidShare { get; set; }
        [Display(Name = "Dividend  Obtaind")]
        public decimal DividendObtaind { get; set; }
        public int ShareAndDividendId { get; set; }
        public virtual ShareAndDividend GetShareAndDividend { get; set; } 

    }
}
