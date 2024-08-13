using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Finance.GL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Health
{
    public class HealthExpenseAndTaxSetting
    {
        public int HealthExpenseAndTaxSettingId { get; set; }
        [Display(Name = "Item Name")]
        public int ApSetupItemId { get; set; }
        public ApSetupItem ApSetup { get; set; }
        [Display(Name = "Tax")]
        public int GLTaxRateID { get; set; }
        public GLTaxRate GLTaxRate { get; set; }
        public string Description { get; set; }
    }

}
